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
    class ControllerRouteConstraint : IRouteConstraint
    {
        private string[] _languages;

        public ControllerRouteConstraint(string[] languages)
        {
            _languages = languages;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            //string value = values[parameterName].ToString();

            return false;
            throw new NotImplementedException();
        }
    }


}
