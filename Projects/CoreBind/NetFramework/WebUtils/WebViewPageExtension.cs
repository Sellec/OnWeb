using OnUtils.Application.Users;
using OnWeb;
using OnWeb.Core.Modules;
using OnWeb.Modules.WebCoreModule;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace System
{
    /// <summary>
    /// </summary>
    public static class WebViewPageExtension
    {
        public static void PropagateSection(this WebViewPage page, string sectionName)
        {
            if (page.IsSectionDefined(sectionName))
            {
                page.DefineSection(sectionName, delegate () { page.Write(page.RenderSection(sectionName)); });
            }
        }

        public static void PropagateSections(this WebViewPage page, params string[] sections)
        {
            foreach (var s in sections) page.PropagateSection(s);
        }

        /// <summary>
        /// Рисование указанного шаблона. Позволяет указать просто имя шаблона без путей, чтобы движок сам смог определить место поиска.
        /// Рекомендуется вместо <see cref="RenderPartialExtensions.RenderPartial(HtmlHelper, string)"/>.
        /// </summary>
        public static MvcHtmlString RenderPartial(this WebViewPage page, string template)
        {
            if (!template.EndsWith(".tpl") && !template.EndsWith(".cshtml") && !template.EndsWith(".vbhtml")) template += ".cshtml";
            if (template.EndsWith(".tpl")) template = template.Replace(".tpl", ".cshtml");
            if (!template.StartsWith("~/")) template = "~/" + template;

            return page.Html.Partial(template);
        }

        /// <summary>
        /// Рисование указанного шаблона. Позволяет указать просто имя шаблона без путей, чтобы движок сам смог определить место поиска.
        /// Рекомендуется вместо <see cref="RenderPartialExtensions.RenderPartial(HtmlHelper, string, object)"/>.
        /// </summary>
        public static MvcHtmlString RenderPartial(this WebViewPage page, string template, object model)
        {
            if (!template.EndsWith(".tpl") && !template.EndsWith(".cshtml") && !template.EndsWith(".vbhtml")) template += ".cshtml";
            if (template.EndsWith(".tpl")) template = template.Replace(".tpl", ".cshtml");
            if (!template.StartsWith("~/")) template = "~/" + template;

            return page.Html.Partial(template, model);
        }

        #region Свойства
        /// <summary>
        /// Предоставляет доступ к настройкам сайта.
        /// </summary>
        public static WebCoreConfiguration GetConfig(this IViewDataContainer page)
        {
            return page.GetAppCore().WebConfig;
        }

        /// <summary>
        /// Возвращает объект ядра, в контексте которого запущено приложение.
        /// </summary>
        public static WebApplication GetAppCore(this IViewDataContainer page)
        {
            return page.GetModule().GetAppCore();
        }

        /// <summary>
        /// Возвращает текущий модуль, вызывающий представление.
        /// </summary>
        public static IModuleCore GetModule(this IViewDataContainer page)
        {
            return (IModuleCore)page.ViewData["Module"];
        }

        /// <summary>
        /// Возвращает контекст, ассоциированный с пользователем, от имени которого сгенерировано представление.
        /// </summary>
        public static IUserContext GetCurrentUserContext(this IViewDataContainer page)
        {
            return page.ViewData["CurrentUserContext"] as IUserContext;
        }

        public static string GetTitle(this IViewDataContainer page)
        {
            return page.ViewData.ContainsKey("Title") ? page.ViewData["Title"]?.ToString() : "";
        }

        public static void SetTitle(this IViewDataContainer page, string value)
        {
            page.ViewData["Title"] = value;
        }

        public static string GetDescription(this IViewDataContainer page)
        {
            return page.ViewData.ContainsKey("Description") ? page.ViewData["Description"]?.ToString() : "";
        }

        public static void SetDescription(this IViewDataContainer page, string value)
        {
            page.ViewData["Description"] = value;
        }

        public static string GetKeywords(this IViewDataContainer page)
        {
            return page.ViewData.ContainsKey("Keywords") ? page.ViewData["Keywords"]?.ToString() : "";
        }

        public static void SetKeywords(this IViewDataContainer page, string value)
        {
            page.ViewData["Keywords"] = value;
        }
        #endregion
    }
}
