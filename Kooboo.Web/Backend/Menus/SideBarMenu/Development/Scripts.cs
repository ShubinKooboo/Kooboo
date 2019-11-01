﻿using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.Development
{
    public class Scripts : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Development;

        public string Name => "Scripts";

        public string Icon => "";

        public string Url => "Development/Scripts";

        public int Order => 5;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Scripts", context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Scripts", context), Url = AdminUrl("Development/Scripts", siteDb), ActionRights = Sites.Authorization.Actions.Developments.Scripts },