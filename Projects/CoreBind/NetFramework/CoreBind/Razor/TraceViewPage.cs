using OnUtils.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace OnWeb.CoreBind.Razor
{
    using Core.Modules;

    public interface IModuleProvider
    {
        ModuleCore Module { get; }
    }

    public abstract class TraceViewPage<TModel> : WebViewPage<TModel>, IModuleProvider
    {
        private Guid PageUniqueIdentifier = Guid.NewGuid();

        public TraceViewPage()
        {

        }

        //public class HelperCustom : HtmlHelper<TModel>
        //{
        //    public HelperCustom(ViewContext viewContext, IViewDataContainer viewDataContainer)
        //        : base(viewContext, viewDataContainer)
        //    {
        //    }

        //    public HelperCustom(ViewContext viewContext, IViewDataContainer viewDataContainer, System.Web.Routing.RouteCollection routeCollection)
        //        : base(viewContext, viewDataContainer, routeCollection)
        //    {
        //    }
        //}

        //public override void InitHelpers()
        //{
        //    base.InitHelpers();

        //    //this.Html = new HelperCustom(this.Html.ViewContext, this.Html.ViewDataContainer);
        //}

        public void PropagateSection(string sectionName)
        {
            if (this.IsSectionDefined(sectionName))
            {
                this.DefineSection(sectionName, delegate() { this.Write(this.RenderSection(sectionName)); });
            }
        }

        public void PropagateSections(params string[] sections)
        {
            foreach (var s in sections) PropagateSection(s);
        }

        protected override string NormalizeLayoutPagePath(string layoutPagePath)
        {
            IEnumerable<string> searchLocations = null;
            var engine = ViewEngines.Engines.Where(x => x is Providers.ResourceProvider).Select(x => x as Providers.ResourceProvider).FirstOrDefault();
            if (engine != null)
            {
                var d = this.Context;
                var path = engine.GetFilePath(engine.GetModuleNameFromContext(this.ViewContext.Controller.ControllerContext), layoutPagePath, true, out searchLocations);
                if (!string.IsNullOrEmpty(path)) return path;
            }

            Debug.WriteLine("LayoutPage not found '{0}' for view '{1}'.", layoutPagePath, this.VirtualPath);
            throw new System.IO.FileNotFoundException($"Для представления '{this.VirtualPath}' не найден макет '{layoutPagePath}'.\r\nРасположения для поиска:\r\n - " + string.Join(";\r\n - ", searchLocations) + ".", layoutPagePath);
        }

        /// <summary>
        /// Рисование указанного шаблона. Позволяет указать просто имя шаблона без путей, чтобы движок сам смог определить место поиска.
        /// Рекомендуется вместо <see cref="RenderPartialExtensions.RenderPartial(HtmlHelper, string)"/>.
        /// </summary>
        public MvcHtmlString RenderPartial(string template)
        {
            if (!template.EndsWith(".tpl") && !template.EndsWith(".cshtml") && !template.EndsWith(".vbhtml")) template += ".cshtml";
            if (template.EndsWith(".tpl")) template = template.Replace(".tpl", ".cshtml");
            if (!template.StartsWith("~/")) template = "~/" + template;

            return Html.Partial(template);
        }

        /// <summary>
        /// Рисование указанного шаблона. Позволяет указать просто имя шаблона без путей, чтобы движок сам смог определить место поиска.
        /// Рекомендуется вместо <see cref="RenderPartialExtensions.RenderPartial(HtmlHelper, string, object)"/>.
        /// </summary>
        public MvcHtmlString RenderPartial(string template, object model)
        {
            if (!template.EndsWith(".tpl") && !template.EndsWith(".cshtml") && !template.EndsWith(".vbhtml")) template += ".cshtml";
            if (template.EndsWith(".tpl")) template = template.Replace(".tpl", ".cshtml");
            if (!template.StartsWith("~/")) template = "~/" + template;

            return Html.Partial(template, model);
        }

        #region Свойства
        /// <summary>
        /// Предоставляет доступ к настройкам сайта.
        /// </summary>
        public Core.Configuration.CoreConfiguration Config
        {
            get => AppCore.Config;
        }

        /// <summary>
        /// Возвращает объект ядра, в контексте которого запущено приложение.
        /// </summary>
        public ApplicationCore AppCore
        {
            get => Module.AppCore;
        }

        /// <summary>
        /// Возвращает текущий модуль, вызывающий представление.
        /// </summary>
        public ModuleCore Module
        {
            get => ViewData["Module"] as ModuleCore;
        }

        /// <summary>
        /// Возвращает контекст, ассоциированный с пользователем, от имени которого сгенерировано представление.
        /// </summary>
        public IUserContext CurrentUserContext
        {
            get => AppCore.GetUserContextManager().GetCurrentUserContext();
        }

        public string Title
        {
            get { return this.ViewData.ContainsKey("Title") ? this.ViewData["Title"]?.ToString() : ""; }
            set { this.ViewData["Title"] = value; }
        }

        public string Description
        {
            get { return this.ViewData.ContainsKey("Description") ? this.ViewData["Description"]?.ToString() : ""; }
            set { this.ViewData["Description"] = value; }
        }

        public string Keywords
        {
            get { return this.ViewData.ContainsKey("Keywords") ? this.ViewData["Keywords"]?.ToString() : ""; }
            set { this.ViewData["Keywords"] = value; }
        }

            #endregion
        }

    }
