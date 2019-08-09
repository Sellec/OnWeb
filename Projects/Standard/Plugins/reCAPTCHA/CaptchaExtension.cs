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

            if (helper.ViewContext.Controller is OnWeb.Core.Modules.ModuleControllerBase controller)
            {
                var appCore = controller.AppCore;
                if (appCore != null)
                {
                    var module = appCore.Get<OnWeb.Modules.reCAPTCHA.ModuleReCaptcha>();
                    var cfg = module?.GetConfiguration<OnWeb.Modules.reCAPTCHA.ModuleReCaptchaConfiguration>();
                    if (cfg != null && cfg.IsEnabledValidation && !string.IsNullOrEmpty(cfg.PublicKey))
                    {
                        var tagBuilder = new TagBuilder("button");
                        foreach (var attr in attrs) tagBuilder.MergeAttribute(attr.Key, attr.Value.ToString());
                        tagBuilder.SetInnerText(text);
                        tagBuilder.AddCssClass("captchaInvisible");
                        tagBuilder.MergeAttribute("data-sitekey", cfg.PublicKey);

                        return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
                    }
                }
            }
            
            var tagBuilder2 = new TagBuilder("input");
            foreach (var attr in attrs) tagBuilder2.MergeAttribute(attr.Key, attr.Value.ToString());
            tagBuilder2.MergeAttribute("value", text);
            tagBuilder2.MergeAttribute("type", "submit");

            return new MvcHtmlString(tagBuilder2.ToString(TagRenderMode.Normal));
        }

    }
}
