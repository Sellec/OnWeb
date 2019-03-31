using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnWeb.CoreBind.Routing
{
    class LanguageRouteConstraint : IRouteConstraint
    {
        private string[] _languages;

        public LanguageRouteConstraint(string[] languages)
        {
            _languages = languages;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var _value = values[parameterName];
            var __value = _value != null ? _value.ToString() : "";

            if (parameterName == "language")
            {
                return true;
                if (!string.IsNullOrEmpty(__value)) return _languages.Contains(__value);
                else return true;
            }

            if (parameterName == "controller" || parameterName == "action")
            {
                if ("admin".Equals(values["controller"]))
                {
                    if ("madmin".Equals(values["action"]) || "mnadmin".Equals(values["action"]))
                        return false;
                }
            }

            //string value = values[parameterName].ToString();

            return true;
        }
    }

}
