﻿using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class SiteUser : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "SiteUser";

        public string Icon => "";

        public string Url => "System/SiteUser";

        public int Order => 10;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("SiteUser", context);
        }
    }
}

// new MenuItem{ Name = Hardcoded.GetValue("SiteUser", context), Url = AdminUrl("System/SiteUser", siteDb), ActionRights = Sites.Authorization.Actions.Systems.SiteUser }