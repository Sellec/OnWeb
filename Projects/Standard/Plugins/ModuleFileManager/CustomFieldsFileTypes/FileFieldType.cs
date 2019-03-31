using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Web.Mvc;
using TraceCore.Data;

namespace OnWeb.Plugins.FileManager.CustomFieldsFileTypes
{
    using TraceWeb.ModuleExtensions.CustomFields.Data;
    using TraceWeb.ModuleExtensions.CustomFields.Field;

    public class FileFieldType : FieldType
    {
        class c : RequiredAttribute
        {
            public string GetErrorMessageString()
            {
                return ErrorMessageString;
            }
        }

        [ThreadStatic]
        private static UnitOfWork<TraceWeb.DB.File> DB;

        private static UnitOfWork<TraceWeb.DB.File> GetDB()
        {
            if (DB == null) DB = new UnitOfWork<TraceWeb.DB.File>();
            return DB;
        }

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
                else if (value is TraceWeb.DB.File) valuesPrepared.Add((value as TraceWeb.DB.File).IdFile);
                else valuesInvalid.Add(value?.ToString()?.Truncate(0, 10, "..."));
            }

            var filesFound = DataAccessProvider.Get<TraceWeb.DB.File>().Where(x => valuesPrepared.Contains(x.IdFile)).Select(x => x.IdFile).ToList();
            if (field.IsValueRequired && filesFound.Count == 0) return CreateResultForEmptyValue(field);

            var filesUnknown = valuesPrepared.Where(x => !filesFound.Contains(x)).ToList();

            if (filesUnknown.Count > 0 || valuesInvalid.Count > 0)
            {
                var errors = new List<string>();
                if (valuesInvalid.Count > 0) errors.Add("Следующие значения некорректны:\r\n - " + string.Join(";\r\n - ", valuesInvalid) + ".");
                if (filesUnknown.Count > 0) errors.Add("Следующие файлы не найдены: №" + string.Join(", №", filesUnknown) + ".");

                return new ValuesValidationResult(string.Join("\r\n", errors));
            }

            return new ValuesValidationResult(filesFound.Select(x => (object)x));
        }

        protected virtual ValuesValidationResult CreateResultForEmptyValue(IField field)
        {
            var requiredAttribute = new c();
            return new ValuesValidationResult(string.Format(requiredAttribute.GetErrorMessageString(), field.GetDisplayName()));
        }

        public override IEnumerable<CustomAttributeBuilder> GetCustomAttributeBuildersForModel(IField field)
        {
            if (field.IsValueRequired)
            {
                if (field.IsMultipleValues)
                {
                    var requiredAttribute = typeof(RequiredAttributeForMultipleValue).GetConstructor(Type.EmptyTypes);
                    return new CustomAttributeBuilder(requiredAttribute, new object[] { }).SingleAsEnumerable();
                }
                else
                {
                    var requiredAttribute = typeof(RequiredAttributeForSingleValue).GetConstructor(Type.EmptyTypes);
                    return new CustomAttributeBuilder(requiredAttribute, new object[] { }).SingleAsEnumerable();
                }
            }
            return null;
        }

        public override MvcHtmlString RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        {
            var label = htmlAttributes?.GetValueOrDefault("label", "Выберите файл")?.ToString() ?? "Выберите файл";
            var containerID = $"FileUploadField_{field.IdField}_{new Random().Next(100000, 999999)}";

            var presentOptions = additionalParameters.FirstOrDefault() as FilePresentOptions;
            if (presentOptions == null) { presentOptions = new FilePresentOptions(); }

            var optionsFromAttributes = htmlAttributes?.GetValueOrDefault("uploadOptions");
            if (optionsFromAttributes != null)
            {
                var typeDictionary = TraceCore.Types.TypeHelpers.ExtractGenericInterface(optionsFromAttributes.GetType(), typeof(IDictionary<,>));
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
                if (module != null) uploadOptions["formData"] = new { moduleName = module.Name };
            }
            uploadOptions["multiple"] = field.IsMultipleValues;

            return RenderHtmlEditorInternal<TModel>(html, field, label, containerID, uploadOptions, presentOptions);
        }

        protected virtual MvcHtmlString RenderHtmlEditorInternal<TModel>(HtmlHelper<TModel> html, IField field, string label, string containerID, IDictionary<string, object> uploadOptions, FilePresentOptions presentOptions)
        { 
            var divBuilder = new TagBuilder("div");
            divBuilder.InnerHtml = label;
            divBuilder.MergeAttribute("id", containerID);
            divBuilder.AddCssClass("FileUploadField");
            if (!string.IsNullOrEmpty(field.alias))
            {
                divBuilder.AddCssClass($"FileUploadField_{field.alias}");
                divBuilder.AddCssClass($"FieldAlias_{field.alias}");
            }

            var str = $@"
                <script type='text/javascript'>$(function(){{ 
                    $('div#FileUploadField_Files_{containerID} input.buttonDelete').unbind('click').click(function(){{ $(this).parent().remove(); }});
                    var {containerID} = $('#{containerID}').requestFileUploadSingle({Newtonsoft.Json.JsonConvert.SerializeObject(uploadOptions)}); 
                    {(!string.IsNullOrEmpty(field.alias) ? $"FileUploadField_{field.alias} = {containerID};" : "")}
                    {containerID}.getElement().bind('requestFileUploadSingleAfter', function(e, result, message, data) {{ {containerID}.reset(); if (result == JsonResult.OK) $('div#FileUploadField_Files_{containerID}').{(field.IsMultipleValues ? "append" : "html")}(""<span class='FileUploadField' data-idfile='"" + data + ""'><input type='button' class='buttonDelete' value='X'>&nbsp;<input type='checkbox' checked name='fieldValue_{field.IdField}[]' value='"" + data + ""'>&nbsp;<label>файл №"" + data + ""</label></span>""); }});
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
                    str += $"<label>{IdFile}</label>";
                    str += "</span>";
                }
            }

            str += "</div>";
           
            return MvcHtmlString.Create(str);
        }

        public override int IdType
        {
            get { return 10; }
        }

        public override string TypeName
        {
            get { return "Файл"; }
        }

        public override bool IsRawOrSourceValue
        {
            get { return false; }
        }

        public override FieldValueType? ForcedIdValueType
        {
            get { return FieldValueType.KeyFromSource; }
        }
    }
}
