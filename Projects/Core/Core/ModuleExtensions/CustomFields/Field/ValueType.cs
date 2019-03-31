using System.ComponentModel.DataAnnotations;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field
{
#pragma warning disable CS1591 // todo внести комментарии.
    /// <summary>
    /// Список видов данных в значениях полей.
    /// </summary>
    public enum FieldValueType : int
    {
        [Display(Name = "Любой текст")]
        Default = 0,

        [Display(Name = "Любой текст")]
        String = 1,

        [Display(Name = "Целое число")]
        Int = 2,

        [Display(Name = "Дробное число (разделитель точка)")]
        FloatDot = 3,

        [Display(Name = "Дробное число (разделитель запятая)")]
        FloatComma = 4,

        [Display(Name = "Ключ из списка значений")]
        KeyFromSource = 5,

        [Display(Name = "Номер телефона")]
        Phone = 6,

        [Display(Name = "Email-адрес")]
        Email = 7,

        [Display(Name = "URL")]
        URL = 8,

        [Display(Name = "Да/Нет")]
        Boolean = 9,
    }
}
