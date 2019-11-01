﻿using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class Settings : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "Settings";

        public string Icon => "";

        public string Url => "System/Settings";

        public int Order => 1;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Settings", context);
        }

        //new MenuItem{ Name = Hardcoded.GetValue("Settings",context), Url = AdminUrl("System/Settings", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Settings
    }
}