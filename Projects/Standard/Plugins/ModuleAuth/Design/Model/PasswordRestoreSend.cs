using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.Auth.Design.Model
{
    public class PasswordRestoreSend
    {
        public DB.User User { get; set; }

        public string Code { get; set; }

        public string CodeType { get; set; }
    }
}