using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public static class CaptchaExtension
    {
        public static IHtmlString RecaptchaInvisible(this HtmlHelper helper, string text, object htmlAttributes = null)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var tagBuilder = new TagBuilder("button");
            foreach (var attr in attrs) tagBuilder.MergeAttribute(attr.Key, attr.Value.ToString());
            tagBuilder.SetInnerText(text);
            tagBuilder.AddCssClass("captchaInvisible");
            tagBuilder.MergeAttribute("data-sitekey", "6LeA7T0UAAAAAJV0aC93lTYu_RkT2f-AJ9Bp6luN");// ApplicationCore.Instance.Config.Get("reCaptchaSiteKey", ""));

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

    }
}
