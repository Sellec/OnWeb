using OnUtils.Architecture.AppCore;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
    sealed class UnknownFieldTypeRender : CoreComponentBase<ApplicationCore>, ICustomFieldRender<UnknownFieldType>
    {
        MvcHtmlString ICustomFieldRender<UnknownFieldType>.RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        {
            return MvcHtmlString.Empty;
        }

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
        }
        #endregion
    }
}
