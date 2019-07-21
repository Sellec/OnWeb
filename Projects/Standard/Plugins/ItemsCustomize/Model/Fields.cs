using OnUtils.Application.DB;
using OnUtils.Application.Modules.ItemsCustomize.Field;
using OnUtils.Application.Modules.ItemsCustomize.Scheme;
using System.Collections.Generic;

namespace OnWeb.Plugins.ItemsCustomize.Model
{
    using Core.Modules;

    public class Fields
    {
        public Dictionary<uint, string> Schemes { get; set; }

        public Dictionary<ItemType, Dictionary<SchemeItem, string>> SchemeItems { get; set; }

        public Dictionary<int, IField> FieldsList { get; set; }

        public bool AllowSchemesManage { get; set; }
    }
}
