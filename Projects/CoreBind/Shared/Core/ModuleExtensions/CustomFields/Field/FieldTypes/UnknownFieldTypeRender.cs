﻿using OnUtils.Architecture.AppCore;
using System.Collections.Generic;
#if NETFULL
using System.Web.Mvc;
using System.Web.Mvc.Html;
#elif NETCORE
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MvcHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
    sealed class UnknownFieldTypeRender : CoreComponentBase<WebApplicationCore>, ICustomFieldRender<UnknownFieldType>
    {
        MvcHtmlString ICustomFieldRender<UnknownFieldType>.RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        {
            return null;
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
