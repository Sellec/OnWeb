using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.Auth.Model
{
    public class AuthLoginData
    {
        public string login { get; set; }

        public string pass { get; set; }

        public string urlFrom { get; set; }
    }
}