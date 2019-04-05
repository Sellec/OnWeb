using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace OnWeb.Plugins.AddressesSystem
{
    using coreb
    class ModuleControllerAdmin : 
    {
        [MenuAction("Адреса (КЛАДР)", "addresses", Module.PERM_ADDRESS)]
        public ActionResult AddressSystem()
        {
            if (Request.Form["changeSettings"] != null)
            {
                if (Request.Form["makeAddressMode"] != null)
                    switch (Request.Form["makeAddressMode"].ToString())
                    {
                        case "1":
                            AppCore.Config["makeAddressMode"] = 1;
                            break;

                        case "0":
                        default:
                            AppCore.Config["makeAddressMode"] = 0;

                            break;
                    }

                if (Request.Form.HasKey("kladrInCloudToken")) AppCore.Config["kladrInCloudToken"] = Request.Form["kladrInCloudToken"];
                if (Request.Form.HasKey("kladrInCloudKey")) AppCore.Config["kladrInCloudKey"] = Request.Form["kladrInCloudKey"];
                if (Request.Form.HasKey("mapsYandexKey")) AppCore.Config["mapsYandexKey"] = Request.Form["mapsYandexKey"];
                if (Request.Form.HasKey("dadataSecretKey")) AppCore.Config["dadataSecretKey"] = Request.Form["dadataSecretKey"];
                if (Request.Form.HasKey("dadataApiKey")) AppCore.Config["dadataApiKey"] = Request.Form["dadataApiKey"];

                AppCore.ConfigurationSave();
            }

            return this.display("admin_adminmain_addresses.tpl");
        }
        
        public ActionResult TestAddress(string address)
        {
            var result = AppCore.Get<Core.Addresses.IManager>().SearchAddress(address);
            return Content(result.Result?.KodAddress);
        }

        public ActionResult TestGeo(string address)
        {
            System.Net.IPAddress ip = null;
            if (System.Net.IPAddress.TryParse(address, out ip))
            {
                var result = AppCore.Get<Core.Addresses.IManager>().GetAddressByIP(ip);
                return Content(result.Result?.KodAddress);
            }
            else return Content("not");
        }
    }
}
