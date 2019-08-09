using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Modules.FileManager.MVC
{
    public class FileRenderOptions
    {
        public string UploadedFileInfoJS { get; set; }

        public Func<int, string> UploadedFileInfo { get; set; }
    }
}