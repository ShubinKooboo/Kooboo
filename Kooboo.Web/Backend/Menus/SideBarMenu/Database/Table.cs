﻿using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System.Collections.Generic;

namespace Kooboo.Web.Menus.SideBarMenu.Database
{
    public class Table : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.Database;

        public string Name => "Table";

        public string Icon => "";

        public string Url => "Storage/Database";

        public int Order => 1;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext context)
        {
            return Hardcoded.GetValue("Table", context);
        }
    }
}

//new MenuItem { Name = Hardcoded.GetValue("Table",context), Url = AdminUrl("Storage/Database", siteDb) }