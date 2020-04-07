﻿using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Logistics.Models;

namespace Kooboo.Sites.Logistics
{
    public class KLogisticsMethodWrapper
    {
        private RenderContext Context { get; set; }

        private ILogisticsMethod LogisticsMethod { get; set; }

        public KLogisticsMethodWrapper(ILogisticsMethod logisticsMethod, RenderContext context)
        {
            this.LogisticsMethod = logisticsMethod;
            this.Context = context;
        }

        public ILogisticsResponse CreateOrder(object value)
        {
            var request = ParseRequest(value);

            var sitedb = this.Context.WebSite.SiteDb();

            var repo = sitedb.GetSiteRepository<Repository.LogisticsRequestRepository>();
            repo.AddOrUpdate(request);

            var result = this.LogisticsMethod.CreateOrder(request);

            if (!string.IsNullOrWhiteSpace(result.logisticsMethodReferenceId))
            {
                request.ReferenceId = result.logisticsMethodReferenceId;
            }

            if (!string.IsNullOrWhiteSpace(request.ReferenceId))
            {
                repo.AddOrUpdate(request);
            }

            return result;
        }


        [KIgnore]
        public LogisticsRequest ParseRequest(object dataobj)
        {
            Dictionary<string, object> additionals = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            LogisticsRequest request = new LogisticsRequest();

            System.Collections.IDictionary idict = dataobj as System.Collections.IDictionary;

            IDictionary<string, object> dynamicobj = null;

            if (idict == null)
            {
                dynamicobj = dataobj as IDictionary<string, object>;
                foreach (var item in dynamicobj)
                {
                    additionals[item.Key] = item.Value;
                }
            }
            else
            {
                foreach (var item in idict.Keys)
                {
                    if (item != null)
                    {
                        additionals[item.ToString()] = idict[item];
                    }
                }
            }

            request.Additional = additionals;


            var id = GetValue<string>(idict, dynamicobj, "id", "requestId", "paymentrequestid");
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (Guid.TryParse(id, out Guid requestid))
                {
                    request.Id = requestid;
                }
            }

            request.Name = GetValue<string>(idict, dynamicobj, "name", "title");
            request.Description = GetValue<string>(idict, dynamicobj, "des", "description", "detail");
            request.SenderInfo.Address = GetValue<string>(idict, dynamicobj, "senderaddress");
            request.SenderInfo.City = GetValue<string>(idict, dynamicobj, "sendercity");
            request.SenderInfo.County = GetValue<string>(idict, dynamicobj, "sendercountry");
            request.SenderInfo.Mobile = GetValue<string>(idict, dynamicobj, "sendermobile");
            request.SenderInfo.Phone = GetValue<string>(idict, dynamicobj, "senderphone");
            request.SenderInfo.Prov = GetValue<string>(idict, dynamicobj, "senderprovince");
            request.SenderInfo.Name = GetValue<string>(idict, dynamicobj, "sendername");
            request.ReceiverInfo.Address = GetValue<string>(idict, dynamicobj, "receiveraddress");
            request.ReceiverInfo.City = GetValue<string>(idict, dynamicobj, "receivercity");
            request.ReceiverInfo.County = GetValue<string>(idict, dynamicobj, "receivercountry");
            request.ReceiverInfo.Mobile = GetValue<string>(idict, dynamicobj, "receivermobile");
            request.ReceiverInfo.Phone = GetValue<string>(idict, dynamicobj, "receiverphone");
            request.ReceiverInfo.Prov = GetValue<string>(idict, dynamicobj, "receiverprovince");
            request.ReceiverInfo.Name = GetValue<string>(idict, dynamicobj, "receivername");
            if (this.PaymentMethod != null)
            {
                request.LogisticsMethod = PaymentMethod.Name;
            }

            request.OrderId = GetValue<Guid>(idict, dynamicobj, "orderId", "orderid");

            request.ReferenceId = GetValue<string>(idict, dynamicobj, "ref", "reference");

            return request;
        }

        private T GetValue<T>(System.Collections.IDictionary idict, IDictionary<string, object> Dynamic, params string[] fieldnames)
        {
            var type = typeof(T);

            object Value = null;

            foreach (var item in fieldnames)
            {
                if (idict != null)
                {
                    Value = Accessor.GetValueIDict(idict, item, type);
                }
                else if (Dynamic != null)
                {
                    Value = Accessor.GetValue(Dynamic, item, type);
                }

                if (Value != null)
                {
                    break;
                }
            }

            if (Value != null)
            {
                return (T)Value;
            }

            return default(T);
        }
    }
}
