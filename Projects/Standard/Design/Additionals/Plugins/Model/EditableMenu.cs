using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.Design.Additionals.Plugins.Model
{
    public class EditableMenu
    {
        public string Class { get; set; }

        public bool WrapText { get; set; }

        public List<EditableMenuItem> EditableMenuData { get; set; }

    }

    public class EditableMenuItem
    {
        public string Caption { get; set; }

    }

    public class EditableMenuGroup : EditableMenuItem
    {
        public List<EditableMenuItem> Items { get; set; } = new List<EditableMenuItem>();
    }

    public class EditableMenuLink : EditableMenuItem
    {
        public string Url { get; set; }

        public string Class { get; set; }

        public bool Check { get; set; }


    }

    public class EditableMenuLinkGroup : EditableMenuLink
    {
        public List<EditableMenuItem> Items { get; set; } = new List<EditableMenuItem>();
    }
}
