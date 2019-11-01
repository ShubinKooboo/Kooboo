﻿using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class Roles : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "Roles";

        public string Icon => "";

        public string Url => "System/Roles";

        public int Order => 11;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Roles", context);
        }
    }
}

//new MenuItem{ Name = Hardcoded.GetValue("Roles", context), Url = AdminUrl("System/Roles", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Domains }