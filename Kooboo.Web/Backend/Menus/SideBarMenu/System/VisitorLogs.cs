﻿using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class VisitorLogs : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "VisitorLogs";

        public string Icon => "";

        public string Url => "System/VisitorLogs";

        public int Order => 7;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("VisitorLogs", context);
        }
    }
}

//new MenuItem{ Name = Hardcoded.GetValue("VisitorLogs",context),Url = AdminUrl("System/VisitorLogs", siteDb),ActionRights = Sites.Authorization.Actions.Systems.VisitorLogs },