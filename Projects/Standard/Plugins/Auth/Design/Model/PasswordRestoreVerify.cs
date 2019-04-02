using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.Auth.Design.Model
{
    public class PasswordRestoreVerify : Auth.Model.PasswordRestoreSave
    {
        public string CodeType { get; set; }
    }
}