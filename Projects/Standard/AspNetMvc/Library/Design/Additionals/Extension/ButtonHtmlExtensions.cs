using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

public static class HtmlButtonExtension
{

    public static MvcHtmlString Button(this HtmlHelper helper, object htmlAttributes)
    {
        return Button(helper, string.Empty, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString Button(this HtmlHelper helper, string innerHtml, IDictionary<string, object> htmlAttributes)
    {
        var builder = new TagBuilder("input");
        builder.InnerHtml = innerHtml;
        builder.Attributes.Add("type", "button");
        builder.MergeAttributes(htmlAttributes);
        return MvcHtmlString.Create(builder.ToString());
    }
}
