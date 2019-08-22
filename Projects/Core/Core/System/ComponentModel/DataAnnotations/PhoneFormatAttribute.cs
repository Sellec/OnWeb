using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Проверяет телефонные номера.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PhoneFormatAttribute : ValidationAttribute
    {
        /// <summary>
        /// Определяет, является ли значение <paramref name="value"/> допустимым телефонным номером.
        /// </summary>
        /// <returns>Возвращает true, если значение является допустимым телефонным номером, и false, если нет.</returns>
        public override bool IsValid(object value)
        {
            return base.IsValid(value);
        }

        /// <summary>
        /// Определяет, является ли значение <paramref name="value"/> допустимым телефонным номером.
        /// </summary>
        /// <returns>Возвращает результат проверки значения.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phone = string.Empty;
            var phoneToCheck = value?.ToString();

            if (!string.IsNullOrEmpty(phoneToCheck))
            {
                var result = string.Empty;

                try
                {
                    var phoneUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
                    var phoneParsed = phoneUtil.Parse(phoneToCheck, "RU");
                    if (phoneUtil.IsValidNumber(phoneParsed))
                    {
                        phone = phoneUtil.Format(phoneParsed, PhoneNumbers.PhoneNumberFormat.E164);
                    }
                    else
                    {
                        result = $"Некорректный номер телефона в поле '{validationContext.DisplayName}'.";
                    }
                }
                catch (PhoneNumbers.NumberParseException ex)
                {
                    switch (ex.ErrorType)
                    {
                        case PhoneNumbers.ErrorType.INVALID_COUNTRY_CODE:
                            result = $"Неизвестный код страны после знака '+' в поле '{validationContext.DisplayName}'.";
                            break;

                        case PhoneNumbers.ErrorType.NOT_A_NUMBER:
                            result = $"Значение в поле '{validationContext.DisplayName}' не является номером телефона.";
                            break;

                        case PhoneNumbers.ErrorType.TOO_LONG:
                            result = $"Номер телефона в поле '{validationContext.DisplayName}' не может быть длиннее 250 символов.";
                            break;

                        case PhoneNumbers.ErrorType.TOO_SHORT_NSN:
                            result = $"Значение в поле '{validationContext.DisplayName}' слишком короткое для номера телефона";
                            break;

                        case PhoneNumbers.ErrorType.TOO_SHORT_AFTER_IDD:
                            result = $"Часть номера после префикса в поле '{validationContext.DisplayName}' слишком короткая";
                            break;

                        default:
                            result = $"'{validationContext.DisplayName}' - " + ex.Message;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    result = $"'{validationContext.DisplayName}' - " + ex.Message;
                }

                if (!string.IsNullOrEmpty(result)) return new ValidationResult(result);
                else
                {
                    var field = validationContext.ObjectType.GetField(validationContext.MemberName, Reflection.BindingFlags.Instance | Reflection.BindingFlags.Public | Reflection.BindingFlags.NonPublic);
                    if (field != null) field.SetValue(validationContext.ObjectInstance, phone);

                    var prop = validationContext.ObjectType.GetProperty(validationContext.MemberName, Reflection.BindingFlags.Instance | Reflection.BindingFlags.Public | Reflection.BindingFlags.NonPublic);
                    if (prop != null) prop.SetValue(validationContext.ObjectInstance, phone);

                    return ValidationResult.Success;
                }
            }
            else return ValidationResult.Success;
        }
    }
}
