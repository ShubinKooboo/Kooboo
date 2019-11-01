﻿using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class Jobs : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "Jobs";

        public string Icon => "";

        public string Url => "System/Jobs";

        public int Order => 9;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Jobs", context);
        }
    }
}

//   new MenuItem{ Name = Hardcoded.GetValue("Jobs", context), Url = AdminUrl("System/Jobs", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Jobs },