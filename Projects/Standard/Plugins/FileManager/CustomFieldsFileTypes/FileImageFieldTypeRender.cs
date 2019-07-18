using OnUtils.Application.Modules.Extensions.CustomFields.Data;
using OnUtils.Application.Modules.Extensions.CustomFields.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.FileManager.CustomFieldsFileTypes
{
    using Core;
    using Core.Modules.Extensions.CustomFields;
    using CoreBind.Razor;


    class FileImageFieldTypeRender : CoreComponentBase, ICustomFieldRender<FileImageFieldType>
    {
        MvcHtmlString ICustomFieldRender<FileImageFieldType>.RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        {
            var label = htmlAttributes?.GetValueOrDefault("label", "Выберите файл")?.ToString() ?? "Выберите файл";
            var containerID = $"FileUploadField_{field.IdField}_{new Random().Next(100000, 999999)}";

            var presentOptions = additionalParameters.FirstOrDefault() as FilePresentOptions;
            if (presentOptions == null) { presentOptions = new FilePresentOptions(); }

            var optionsFromAttributes = htmlAttributes?.GetValueOrDefault("uploadOptions");
            if (optionsFromAttributes != null)
            {
                var typeDictionary = OnUtils.Types.TypeHelpers.ExtractGenericInterface(optionsFromAttributes.GetType(), typeof(IDictionary<,>));
                if (typeDictionary != null)
                {
                    var dictSimple = (optionsFromAttributes as System.Collections.IDictionary);

                    if (typeDictionary.GenericTypeArguments[0] != typeof(string) || typeDictionary.GenericTypeArguments[1] != typeof(object) || dictSimple.IsReadOnly)
                    {
                        var newUploadOptions = new Dictionary<string, object>();
                        foreach (var key in dictSimple.Keys) newUploadOptions[key.ToString()] = $"{dictSimple[key]}";
                        optionsFromAttributes = newUploadOptions;
                    }
                }
                else optionsFromAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(optionsFromAttributes);
            }
            else optionsFromAttributes = new Dictionary<string, object>();

            var uploadOptions = optionsFromAttributes as IDictionary<string, object>;
            uploadOptions["formData"] = new { moduleName = "" };

            if (html.ViewDataContainer is IModuleProvider)
            {
                var module = (html.ViewDataContainer as IModuleProvider).Module;
                if (module != null) uploadOptions["formData"] = new { moduleName = module.UrlName };
            }
            uploadOptions["multiple"] = field.IsMultipleValues;

            return RenderHtmlEditorInternal<TModel>(html, field, label, containerID, uploadOptions, presentOptions);
        }

        private MvcHtmlString RenderHtmlEditorInternal<TModel>(HtmlHelper<TModel> html, IField field, string label, string containerID, IDictionary<string, object> uploadOptions, FilePresentOptions presentOptions)
        {
            uploadOptions["allowedTypes"] = "jpg, jpeg, png, gif, bmp";
            uploadOptions["acceptFiles"] = "image/";

            var divBuilder = new TagBuilder("div");
            divBuilder.InnerHtml = label;
            divBuilder.MergeAttribute("id", containerID);
            divBuilder.AddCssClass("FileUploadField");
            if (!string.IsNullOrEmpty(field.alias))
            {
                divBuilder.AddCssClass($"FileUploadField_{field.alias}");
                divBuilder.AddCssClass($"FieldAlias_{field.alias}");
            }

            var imagePresentOptions = presentOptions as ImagePresentOptions;
            var htmlSizes = new List<string>();
            if (imagePresentOptions != null && imagePresentOptions.MaxWidth.HasValue) { htmlSizes.Add($"max-width:{imagePresentOptions.MaxWidth}px;width: expression(this.width > {imagePresentOptions.MaxWidth } ? {imagePresentOptions.MaxWidth}: true);"); }
            if (imagePresentOptions != null && imagePresentOptions.MaxHeight.HasValue) { htmlSizes.Add($"max-height:{imagePresentOptions.MaxHeight}px;height: expression(this.height > {imagePresentOptions.MaxHeight } ? {imagePresentOptions.MaxHeight}: true);"); }
            var htmlSizesAttrs = "style='" + string.Join("; ", htmlSizes) + "'";

            var str = $@"
                <script type='text/javascript'>$(function(){{ 
                    $('div#FileUploadField_Files_{containerID} input.buttonDelete').unbind('click').click(function(){{ $(this).parent().remove(); }});
                    var {containerID} = $('#{containerID}').requestFileUploadSingle({Newtonsoft.Json.JsonConvert.SerializeObject(uploadOptions)}); 
                    {(!string.IsNullOrEmpty(field.alias) ? $"FileUploadField_{field.alias} = {containerID};" : "")}
                    {containerID}.getElement().bind('requestFileUploadSingleAfter', function(e, result, message, data) {{ " +
                        $"{containerID}.reset();" +
                        $"if (result == JsonResult.OK) {{" +
                            $@"$('div#FileUploadField_Files_{containerID}').{(field.IsMultipleValues ? "append" : "html")}(" +
                                $@"""<span class='FileUploadField' data-idfile='"" + data + ""'>" +
                                    $@"<input type='button' class='buttonDelete' value='X'>&nbsp;" +
                                    $@"<input type='checkbox' checked name='fieldValue_{field.IdField}[]' value='"" + data + ""'>&nbsp;" +
                                    $@"<img src='/fm/File/"" + data + ""' {htmlSizesAttrs} />" +
                                $@"</span>""" +
                            $@");" +
                            $@"$('div#FileUploadField_Files_{containerID} input.buttonDelete').unbind('click').click(function(){{ $(this).parent().remove(); }});" +
                        $@"}}" +
                    $@"}});                    
                }});</script>
                {divBuilder.ToString(TagRenderMode.Normal)}
            ";

            str += $"<div id='FileUploadField_Files_{containerID}'>";

            if (field is FieldData)
            {
                var fieldData = field as FieldData;

                var IdList = fieldData.Where(x => x is int).Select(x => (int)x).ToList();
                //var filesList = GetDB().Repo1.Where(x => IdList.Contains(x.IdFile)).ToDictionary(x => x.IdFile, x => x);
                foreach (var IdFile in IdList)
                {
                    //var file = filesList.GetValueOrDefault(IdFile);
                    //str += $"<span class='FileUploadField {(file == null ? "FileNotFound" : "")}' data-idfile='{IdFile}'>";
                    str += $"<span class='FileUploadField' data-idfile='{IdFile}'>";
                    str += "<input type='button' class='buttonDelete' value='X'>&nbsp;";
                    str += $"<input type='checkbox' checked name='fieldValue_{field.IdField}[]' value='{IdFile}'>&nbsp;";
                    //str += $"<label>{file?.NameFile ?? "Файл не найден"}</label>";
                    str += $"<img src='/fm/image/{IdFile}' {htmlSizesAttrs} />";
                    str += "</span>";
                }
            }

            str += $"</div><input type='hidden' name='fieldValue_{field.IdField}[]'>";

            return MvcHtmlString.Create(str);
        }

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
    }
}
