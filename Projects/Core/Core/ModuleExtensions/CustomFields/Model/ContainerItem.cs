using System.Collections.Generic;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Model
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class SchemeContainerItem
    {
        public class Scheme
        {
            public string Name;
            public Dictionary<int, Field.IField> Fields;
        }

        public CustomFields.Scheme.SchemeItem SchemeItem;

        public Dictionary<uint, Scheme> Schemes = new Dictionary<uint, Scheme>();
    }

}
