using OnUtils.Application.Modules;
using OnUtils.Application.Modules.ItemsCustomize;
using OnUtils.Application.Modules.ItemsCustomize.Data;

namespace OnWeb.Core.DB
{
    using Items;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    public enum UserState : short
    {
        [Display(Name = "Активна")]
        Active = 0,

        [Display(Name = "Регистрация ожидает подтверждения по Email")]
        RegisterNeedConfirmation = 1,

        [Display(Name = "Регистрация ожидает модерирования")]
        RegisterWaitForModerate = 2,

        [Display(Name = "Регистрация отклонена")]
        RegisterDecline = 3,

        [Display(Name = "Отключена")]
        Disabled = 4,
    }

    [Table("users")]
    [DisplayName("Пользователь")]
    [System.Diagnostics.DebuggerDisplay("User: id={ID}")]
    public partial class User : ItemBase, IItemCustomized
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int IdUser { get; set; }

        /// <summary>
        /// См. <see cref="IdUser"/>.
        /// </summary>
        public override int ID
        {
            get => IdUser;
            set { }
        }

        /// <summary>
        /// См. <see cref="name"/>. 
        /// </summary>
        public override string Caption
        {
            get => !string.IsNullOrEmpty(name) ? name : !string.IsNullOrEmpty(email) ? email : IdUser.ToString();
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

        [StringLength(100)]
        [Display(Name = "Имя"), Required]
        public string name { get; set; }

        [StringLength(1000)]
        [Display(Name = "О себе"), DataType(DataType.MultilineText)]
        public string about { get; set; }

        [Display(Name = "Фотография")]
        public int? IdPhoto { get; set; }

        [StringLength(64)]
        [Display(Name = "Пароль"), Required, DataType(DataType.Password)]
        public string password { get; set; }

        [StringLength(5)]
        public string salt { get; set; }

        public byte Superuser { get; set; }

        [Display(Name = "Состояние учетной записи"), Required]
        public UserState State { get; set; }

        [StringLength(100)]
        public string StateConfirmation { get; set; }

        public int AuthorizationAttempts { get; set; }

        public short Block { get; set; }

        public int BlockedUntil { get; set; }

        [StringLength(500)]
        public string BlockedReason { get; set; }

        [StringLength(100)]
        public string IP_reg { get; set; }

        public int DateReg { get; set; }

        public int DateChange { get; set; }

        [StringLength(200)]
        public string UniqueKey { get; set; }

        /// <summary>
        /// Время последнего изменения на основе <see cref="DateChange"/>. 
        /// </summary>
        public override DateTime DateChangeBase
        {
            get => DateChange.FromTimestamp();
            set => DateChange = value.Timestamp();
        }

        public override ModuleCore<WebApplication> OwnerModule => base.OwnerModule;

        public int IdUserChange { get; set; }

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
