using OnUtils.Architecture.AppCore;
using System.Collections.Generic;
#if NETFULL
using System.Web.Mvc;
#elif NETCORE
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MvcHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace OnWeb.Core.ModuleExtensions.CustomFields
{
    using Field;

    /// <summary>
    /// Представляет рендер для пользовательского поля.
    /// </summary>
    public interface ICustomFieldRender<out TFieldType> : IComponentTransient<ApplicationCore> where TFieldType : FieldType
    {
        // todo составить описание метода аналогично стандартным расширениям asp.net mvc.
        MvcHtmlString RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters);
    }
}
