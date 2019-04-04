using OnUtils.Architecture.AppCore;
using System.Collections.Generic;
using System.Web.Mvc;

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
