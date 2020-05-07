﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Logistics.Methods.deppon
{
    public class DEPPONSetting : ILogisticsSetting
    {
        private const string name = "DEPPON";

        public bool UseSandBox { get; set; }

        public string Name => name;

        public string LogisticCompanyID { get; set; }

        public string CompanyCode { get; set; }

        public string CustomerCode { get; set; }

        public string APPKey { get; set; }

        public string TransportType { get; set; }//运输方式/产品类型

        public string Sign { get; set; }

        public string ServerURL => UseSandBox ? "http://dpsanbox.deppon.com/sandbox-web" : "";
    }
}
