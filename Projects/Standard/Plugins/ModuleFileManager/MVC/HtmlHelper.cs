using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString FileUploadFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.FileUploadFor(expression, null, null);
        }

        public static MvcHtmlString FileUploadFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, FilePresentOptions presentOptions)
        {
            return htmlHelper.FileUploadFor(expression, null, presentOptions);
        }

        public static MvcHtmlString FileUploadFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.FileUploadFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), null);
        }

        public static MvcHtmlString FileUploadFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, FilePresentOptions presentOptions)
        {
            return htmlHelper.FileUploadFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), presentOptions);
        }

        public static MvcHtmlString FileUploadFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.FileUploadFor(expression, null, null);
        }

        public static MvcHtmlString FileUploadFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes, FilePresentOptions presentOptions)
        {
            return htmlHelper.EditorFor(expression, "FileUpload", new { htmlAttributes = htmlAttributes, presentOptions = presentOptions });
        }
    }
}