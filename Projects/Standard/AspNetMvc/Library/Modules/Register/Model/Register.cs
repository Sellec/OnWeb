using OnUtils.Application.Items;
using OnUtils.Application.Modules.ItemsCustomize;
using OnUtils.Application.Modules.ItemsCustomize.Data;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace OnWeb.Modules.Register.Model
{
    using Core.DB;
    using Core.Items;

    [reCAPTCHA.Model]
    [ItemTypeAlias(typeof(User))]
    public class Register : ItemBase, IItemCustomized
    {
        /// <summary>
        /// </summary>
        public override int ID
        {
            get => 0;
            set { }
        }

        /// <summary>
        /// См. <see cref="name"/>. 
        /// </summary>
        public override string Caption
        {
            get;
            set;
        }

        [StringLength(128)]
        [Display(Name = "Email-адрес"), DataType(DataType.EmailAddress), EmailAddress]
        public string email { get; set; }

        [StringLength(100)]
        [Display(Name = "Телефон"), DataType(DataType.PhoneNumber), PhoneFormat]
        public string phone { get; set; }

        [Display(Name = "Email-адрес или телефон"), Required(ErrorMessage = "Следует указать адрес электронной почты или номер телефона.")]
        public string EmailOrPhone
        {
            get => email + phone;
        }

        [StringLength(100)]
        [Display(Name = "Имя")]
        public string name { get; set; }

        [StringLength(64)]
        [Display(Name = "Пароль"), Required, DataType(DataType.Password)]
        public string password { get; set; }

        public DefaultSchemeWData Fields => FieldsBase;

        public DefaultSchemeWData fields => fieldsBase;

        public dynamic FieldsDynamic => FieldsDynamicBase;

        public ReadOnlyDictionary<string, FieldData> fieldsNamed => fieldsNamedBase;
    }
}