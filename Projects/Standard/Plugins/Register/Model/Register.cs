using System.ComponentModel.DataAnnotations;

namespace OnWeb.Plugins.Register.Model
{
    using Core.DB;
    using Core.Items;

    [ItemTypeAlias(typeof(User))]
    public class Register : ItemBase<Customer.Module>
    {
        /// <summary>
        /// </summary>
        public override int ID
        {
            get { return 0; }
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
            get { return email + phone; }
        }

        [StringLength(100)]
        [Display(Name = "Имя")]
        public string name { get; set; }

        [StringLength(64)]
        [Display(Name = "Пароль"), Required, DataType(DataType.Password)]
        public string password { get; set; }
    }
}