//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
using Kooboo.Lib.Reflection;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Web.Api
{
    // looks like this is not very useful now. only for the api helper.
    public static class ApiContainer
    {
        private static object _locker = new object();

        private static Dictionary<string, IApi> _list;

        public static Dictionary<string, IApi> List
        {
            get
            {
                if (_list == null)
                {
                    lock (_locker)
                    {
                        if (_list == null)
                        {
                            _list = new Dictionary<string, IApi>(StringComparer.OrdinalIgnoreCase);

                            var alldefinedTypes = GetAllDefinedApi();
                            foreach (var item in alldefinedTypes)
                            {
                                var instance = Activator.CreateInstance(item) as IApi;
                                AddApi(_list, instance);
                            }

                            AddApi(_list, new SiteObjectApi<CmsFile>());
                            AddApi(_list, new SiteObjectApi<CmsCssRule>());

                            AddApi(_list, new SiteObjectApi<ExternalResource>());
                            AddApi(_list, new SiteObjectApi<Folder>());
                            AddApi(_list, new SiteObjectApi<Form>());

                            AddApi(_list, new SiteObjectApi<Image>());
                            AddApi(_list, new SiteObjectApi<Script>());
                            AddApi(_list, new SiteObjectApi<ViewDataMethod>());

                            AddApi(_list, new SiteObjectApi<ContentType>());
                            AddApi(_list, new SiteObjectApi<Label>());
                        }
                    }
                }
                return _list;
            }
        }

        private static List<Type> GetAllDefinedApi()
        {
            return AssemblyLoader.LoadTypeByInterface(typeof(IApi));
        }

        public static void Init()
        {
            if (_list == null)
            {
                _list = new Dictionary<string, IApi>(StringComparer.OrdinalIgnoreCase);

                var alldefinedTypes = GetAllDefinedApi();
                foreach (var item in alldefinedTypes)
                {
                    var instance = Activator.CreateInstance(item) as IApi;
                    AddApi(_list, instance);
                }

                AddApi(_list, new SiteObjectApi<CmsFile>());
                AddApi(_list, new SiteObjectApi<CmsCssRule>());

                AddApi(_list, new SiteObjectApi<ExternalResource>());
                AddApi(_list, new SiteObjectApi<Folder>());
                AddApi(_list, new SiteObjectApi<Form>());

                AddApi(_list, new SiteObjectApi<Image>());
                AddApi(_list, new SiteObjectApi<Script>());
                AddApi(_list, new SiteObjectApi<ViewDataMethod>());

                AddApi(_list, new SiteObjectApi<ContentType>());
                AddApi(_list, new SiteObjectApi<Label>());
            }
        }

        internal static void AddApi(Dictionary<string, IApi> currentlist, IApi instance)
        {
            if (instance != null && currentlist != null)
            {
                var name = instance.ModelName;
                if (!currentlist.ContainsKey(name))
                {
                    currentlist[name] = instance;
                }
            }
        }

        // this seems like only for unit test now.
        public static void AddApi(Type apitype)
        {
            lock (_locker)
            {
                if (Activator.CreateInstance(apitype) is IApi instance)
                {
                    var currentlist = List;
                    AddApi(currentlist, instance);
                }
            }
        }

        public static IApi GetApi(string modelName)
        {
            if (!string.IsNullOrEmpty(modelName) && List.ContainsKey(modelName))
            {
                return List[modelName];
            }
            return null;
        }
    }
}