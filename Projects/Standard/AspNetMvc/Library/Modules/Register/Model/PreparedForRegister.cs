﻿using OnUtils.Application.Items;
using OnUtils.Application.Modules.ItemsCustomize;
using OnUtils.Application.Modules.ItemsCustomize.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnWeb.Modules.Register.Model
{
    using Core.DB;
    using Core.Items;

    [ItemTypeAlias(typeof(User))]
    public class PreparedForRegister : ItemBase, IItemCustomized
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
            get => !string.IsNullOrEmpty(name) ? name : !string.IsNullOrEmpty(email) ? email : string.Empty;
            set => name = value;
        }

        [DefaultValue("")]
        [StringLength(128)]
        [Display(Name = "Email-адрес"), DataType(DataType.EmailAddress), EmailAddress]
        public string email { get; set; }

        [DefaultValue("")]
        [StringLength(100)]
        [Display(Name = "Телефон"), DataType(DataType.PhoneNumber), PhoneFormat]
        public string phone { get; set; }

        [Display(Name = "Email-адрес или телефон"), Required]
        public string EmailOrPhone
        {
            get => email + phone;
        }

        [StringLength(100)]
        [Display(Name = "Имя"), Required]
        public string name { get; set; }

        [StringLength(1000)]
        [Display(Name = "О себе"), DataType(DataType.MultilineText)]
        public string about { get; set; }

        [StringLength(64)]
        [Display(Name = "Пароль"), Required, DataType(DataType.Password)]
        public string password { get; set; }

        [StringLength(5)]
        public string salt { get; set; }

        public byte Superuser { get; set; }

        /// <summary>
        /// Если состояние не задано, то устанавливается согласно настройкам сайта.
        /// </summary>
        [Display(Name = "Состояние учетной записи"), Required]
        public UserState? State { get; set; }

        [StringLength(100)]
        public string IP_reg { get; set; }

        public DateTime DateReg { get; set; }

        [StringLength(200)]
        public string UniqueKey { get; set; }

        /// <summary>
        /// Время последнего изменения на основе <see cref="DateReg"/>. 
        /// </summary>
        public override DateTime DateChangeBase
        {
            get => DateReg;
            set => DateReg = value;
        }

        [StringLength(2000)]
        [Display(Name = "Комментарий о себе"), DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [StringLength(2000)]
        [Display(Name = "Комментарий администратора"), DataType(DataType.MultilineText)]
        public string CommentAdmin { get; set; }

        public DefaultSchemeWData Fields => FieldsBase;

        public DefaultSchemeWData fields => fieldsBase;

        public dynamic FieldsDynamic => FieldsDynamicBase;

        public ReadOnlyDictionary<string, FieldData> fieldsNamed => fieldsNamedBase;
    }
}