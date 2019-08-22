using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

public static class FormExtensions
{
    private static RouteValueDictionary RouteValuesDictionary(HtmlHelper htmlHelper, string controller, string actionName)
    {
        var dict = new RouteValueDictionary();
        dict["controller"] = controller;
        dict["action"] = actionName;

        return dict;
    }

    private static object RouteValuesObject(HtmlHelper htmlHelper, string controller, string actionName)
    {
        return new { controller = controller, action = actionName };
    }

    //public static MvcForm BeginForm<TModule>(this HtmlHelper htmlHelper, TModule module, string actionName = null, object htmlAttributes = null) where TModule : ModuleCore
    //{
    //    return htmlHelper.BeginForm(module, actionName, FormMethod.Post, htmlAttributes);
    //}

    //public static MvcForm BeginForm<TModule>(this HtmlHelper htmlHelper, TModule module, string actionName = null, FormMethod method = FormMethod.Post, object htmlAttributes = null) where TModule : ModuleCore
    //{
    //    var routeName = "User";
    //    if (HttpContext.Current != null && 
    //        HttpContext.Current.Items != null && 
    //        HttpContext.Current.Items["isAdmin"] != null && 
    //        true.Equals(HttpContext.Current.Items["isAdmin"]))
    //    {
    //        routeName = "AdminFull";
    //        if (HttpContext.Current.Items["isAjax"] != null && true.Equals(HttpContext.Current.Items["isAjax"])) routeName = "Admin";
    //    }

    //    var action = !string.IsNullOrEmpty(actionName) ? actionName : "index";
    //    return htmlHelper.BeginForm(action, module.getName(), method, htmlAttributes);
    //    //return htmlHelper.BeginRouteForm(routeName + "_Default", RouteValuesObject(htmlHelper, module.getName(), action), method, htmlAttributes);
    //}

    //public static MvcForm BeginFormAjax<TModule>(this HtmlHelper htmlHelper, TModule module, string actionName = null, FormMethod method = FormMethod.Post, object htmlAttributes = null) where TModule : ModuleCore
    //{
    //    var routeName = "User";
    //    if (HttpContext.Current != null &&
    //        HttpContext.Current.Items != null &&
    //        HttpContext.Current.Items["isAdmin"] != null &&
    //        true.Equals(HttpContext.Current.Items["isAdmin"]))
    //    {
    //        routeName = "Admin";
    //    }
    //    var action = !string.IsNullOrEmpty(actionName) ? actionName : "index";
    //    return htmlHelper.BeginRouteForm(routeName + "_Default", RouteValuesObject(htmlHelper, module.getName(), action), method, htmlAttributes);
    //}

}
