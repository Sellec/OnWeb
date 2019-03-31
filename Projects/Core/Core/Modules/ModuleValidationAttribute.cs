using OnWeb;
using OnWeb.Core;

namespace System.ComponentModel.DataAnnotations
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class ModuleValidationAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            return IsValid(value);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is int)) return new ValidationResult("В качестве идентификатора модуля должен выступать номер.");
            var moduleID = (int)value;

            var module = DeprecatedSingletonInstances.ModulesManager.GetModule(moduleID);
            if (module == null) return new ValidationResult($"Модуль с идентификатором {moduleID} не найден.");

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }
    }
}