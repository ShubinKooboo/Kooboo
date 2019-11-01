//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
using Kooboo.Attributes;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api
{
    public class SiteObjectApi<T> : ISiteObjectApi where T : ISiteObject
    {
        private string _modelname;

        public virtual string ModelName
        {
            get
            {
                if (string.IsNullOrEmpty(_modelname))
                {
                    _modelname = typeof(T).Name;
                }
                return _modelname;
            }
            set
            {
                _modelname = value;
            }
        }

        private Type _modeltype;

        public virtual Type ModelType
        {
            get
            {
                if (_modeltype == null)
                {
                    _modeltype = typeof(T);
                }
                return _modeltype;
            }
            set
            {
                _modeltype = value;
            }
        }

        public virtual bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public virtual bool RequireUser
        {
            get
            {
                return true;
            }
        }

        public virtual Guid AddOrUpdate(ApiCall call)
        {
            T value = default(T);

            if (call.Context.Request.Model != null)
            {
                value = (T)call.Context.Request.Model;
            }
            else if (!string.IsNullOrEmpty(call.Context.Request.Body))
            {
                value = Lib.Helper.JsonHelper.Deserialize<T>(call.Context.Request.Body);
            }

            IRepository repo = call.WebSite.SiteDb().GetRepository(this.ModelType);

            repo.AddOrUpdate(value, call.Context.User.Id);
            return value.Id;
        }

        public virtual Guid Post(ApiCall call)
        {
            return this.AddOrUpdate(call);
        }

        public virtual Guid put(ApiCall call)
        {
            return this.AddOrUpdate(call);
        }

        [RequireParameters("id")]
        public virtual bool Delete(ApiCall call)
        {
            IRepository repo = call.WebSite.SiteDb().GetRepository(this.ModelType);

            if (call.ObjectId != default(Guid))
            {
                var value = repo.Get(call.ObjectId);
                if (value != null)
                {
                    repo.Delete(call.ObjectId);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (!string.IsNullOrEmpty(call.NameOrId))
            {
                var value = repo.GetByNameOrId(call.NameOrId);
                if (value != null)
                {
                    repo.Delete(value.Id, call.Context.User.Id);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public virtual bool Deletes(ApiCall call)
        {
            IRepository repo = call.WebSite.SiteDb().GetRepository(this.ModelType);

            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }
            List<Guid> ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);

            if (ids != null && ids.Any())
            {
                foreach (var item in ids)
                {
                    repo.Delete(item, call.Context.User.Id);
                }
                return true;
            }
            return false;
        }

        public virtual object Get(ApiCall call)
        {
            IRepository repo = call.WebSite.SiteDb().GetRepository(this.ModelType);
            if (call.ObjectId != default(Guid))
            {
                return repo.Get(call.ObjectId);
            }
            else if (!string.IsNullOrEmpty(call.NameOrId))
            {
                return repo.GetByNameOrId(call.NameOrId);
            }
            return Activator.CreateInstance(this.ModelType);
        }

        public virtual List<object> List(ApiCall call)
        {
            IRepository repo = call.WebSite.SiteDb().GetRepository(this.ModelType);
            return repo.All().OrderBy(o => o.Name).ToList<object>();
        }

        public virtual bool IsUniqueName(ApiCall call)
        {
            string name = call.NameOrId;
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            Guid id = call.ObjectId;

            if (id != default(Guid))
            {
                IRepository repo = call.WebSite.SiteDb().GetRepository(this.ModelType);

                var value = repo.Get(id);
                if (value != null)
                {
                    if (value.Name == name)
                    {
                        return true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                IRepository repo = call.WebSite.SiteDb().GetRepository(this.ModelType);

                var value = repo.GetByNameOrId(name);
                if (value != null)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public interface ISiteObjectApi : IApi
    {
        Type ModelType { get; set; }
    }
}