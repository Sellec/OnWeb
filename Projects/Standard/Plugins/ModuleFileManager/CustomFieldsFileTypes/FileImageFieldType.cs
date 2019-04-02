using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using OnUtils.Data;

namespace OnWeb.Plugins.FileManager.CustomFieldsFileTypes
{
    using TraceWeb.ModuleExtensions.CustomFields.Data;
    using TraceWeb.ModuleExtensions.CustomFields.Field;

    public class FileImageFieldType : FileFieldType
    {
        public override ValuesValidationResult Validate(IEnumerable<object> values, IField field)
        {
            if (field.IsValueRequired && (values == null || values.Count() == 0)) return CreateResultForEmptyValue(field);

            var sourceValues = field.data;
            var unknownValues = values.Where(x => sourceValues.Where(y => y.IdFieldValue == (int)x).Count() == 0);

            var valuesPrepared = new HashSet<int>();
            var valuesInvalid = new System.Collections.ObjectModel.Collection<string>();
            foreach (var value in values)
            {
                if (value is int) valuesPrepared.Add((int)value);
                else if (value is DB.File) valuesPrepared.Add((value as DB.File).IdFile);
                else valuesInvalid.Add(value?.ToString()?.Truncate(0, 10, "..."));
            }

            var filesFound = DataAccessProvider.Get<DB.File>().Where(x => valuesPrepared.Contains(x.IdFile)).ToList();
            if (field.IsValueRequired && filesFound.Count == 0) return CreateResultForEmptyValue(field);

            var filesFoundIds = filesFound.Select(x => x.IdFile).ToList();
            var filesUnknown = valuesPrepared.Where(x => !filesFoundIds.Contains(x)).ToList();
            var filesTypeMismatch = filesFound.Where(x => x.TypeCommon != FileTypeCommon.Image).Select(x => x.IdFile).ToList();

            if (filesUnknown.Count > 0 || valuesInvalid.Count > 0 || filesTypeMismatch.Count > 0)
            {
                var errors = new List<string>();
                if (valuesInvalid.Count > 0) errors.Add("Следующие значения некорректны:\r\n - " + string.Join(";\r\n - ", valuesInvalid) + ".");
                if (filesUnknown.Count > 0) errors.Add("Следующие файлы не найдены: №" + string.Join(", №", filesUnknown) + ".");
                if (filesTypeMismatch.Count > 0) errors.Add("Тип файлов не подходит: №" + string.Join(", №", filesTypeMismatch) + ".");

                return new ValuesValidationResult(string.Join("\r\n", errors));
            }

            return new ValuesValidationResult(filesFound.Select(x => (object)x.IdFile));
        }

        protected override MvcHtmlString RenderHtmlEditorInternal<TModel>(HtmlHelper<TModel> html, IField field, string label, string containerID, IDictionary<string, object> uploadOptions, FilePresentOptions presentOptions)
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
                    {containerID}.getElement().bind('requestFileUploadSingleAfter', function(e, result, message, data) {{ {containerID}.reset(); if (result == JsonResult.OK) $('div#FileUploadField_Files_{containerID}').{(field.IsMultipleValues ? "append" : "html")}(""<span class='FileUploadField' data-idfile='"" + data + ""'><input type='button' class='buttonDelete' value='X'>&nbsp;<input type='checkbox' checked name='fieldValue_{field.IdField}[]' value='"" + data + ""'>&nbsp;<img src='/fm/File/"" + data + ""' {htmlSizesAttrs} /></span>""); }});
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

            str += "</div>";

            return MvcHtmlString.Create(str);
        }

        public override int IdType
        {
            get { return 12; }
        }

        public override string TypeName
        {
            get { return "Файл-изображение"; }
        }
    }
}
