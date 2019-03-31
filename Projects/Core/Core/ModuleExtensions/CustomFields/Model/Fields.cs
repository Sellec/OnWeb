using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Model
{
    public class Fields
    {
        public IDictionary<uint, string> Schemes { get; set; }

        public IDictionary<Core.DB.ItemType, IDictionary<Scheme.SchemeItem, string>> SchemeItems { get; set; }

        public IDictionary<int, Field.IField> FieldsList { get; set; }

        public bool AllowSchemesManage { get; set; }
    }
}
