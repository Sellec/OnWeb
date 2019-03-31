using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnWeb.Design.Additionals.ClassicStructures
{
    public class PhotoList : Dictionary<string, PhotoList.Photo>
    {
        public class Photo
        {
            public string preview_file;
            public string main_file;
        }

        public Photo this[int index]
        {
            get { return this[index.ToString()]; }
            set { this[index.ToString()] = value; }
        }
    }
}
