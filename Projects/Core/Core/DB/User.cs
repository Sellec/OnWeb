using OnUtils.Application.DB;
using OnUtils.Application.Items;
using OnUtils.Application.Modules;

namespace OnWeb.Core.DB
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo ������ �����������.
    public enum UserState : short
    {
        [Display(Name = "�������")]
        Active = 0,

        [Display(Name = "����������� ������� ������������� �� Email")]
        RegisterNeedConfirmation = 1,

        [Display(Name = "����������� ������� �������������")]
        RegisterWaitForModerate = 2,

        [Display(Name = "����������� ���������")]
        RegisterDecline = 3,

        [Display(Name = "���������")]
        Disabled = 4,
    }

    [Table("users")]
    [ItemType(ModuleCore.ItemType)]
    [System.Diagnostics.DebuggerDisplay("User: id={ID}")]
    public partial class User : ItemBase
    {
        public User() : base(OnUtils.Application.DeprecatedSingletonInstances.ModulesManager.GetModule<Plugins.Customer.ModuleCustomer>())
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        /// <summary>
        /// ��. <see cref="id"/>.
        /// </summary>
        public override int ID
        {
            get => id;
            set { }
        }

        /// <summary>
        /// ��. <see cref="name"/>. 
        /// </summary>
        public override string Caption
        {
            get => !string.IsNullOrEmpty(name) ? name : !string.IsNullOrEmpty(email) ? email : id.ToString();
            set => name = value;
        }

        [DefaultValue("")]
        [StringLength(128)]
        [Display(Name = "Email-�����"), DataType(DataType.EmailAddress), EmailAddress]
        public string email { get; set; }

        [DefaultValue("")]
        [StringLength(100)]
        [Display(Name = "�������"), DataType(DataType.PhoneNumber), PhoneFormat]
        public string phone { get; set; }

        [StringLength(100)]
        [Display(Name = "���"), Required]
        public string name { get; set; }

        [StringLength(1000)]
        [Display(Name = "� ����"), DataType(DataType.MultilineText)]
        public string about { get; set; }

        [Display(Name = "����������")]
        public int? IdPhoto { get; set; }

        [StringLength(64)]
        [Display(Name = "������"), Required, DataType(DataType.Password)]
        public string password { get; set; }

        [StringLength(5)]
        public string salt { get; set; }

        public byte Superuser { get; set; }

        [Display(Name = "��������� ������� ������"), Required]
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
        /// ����� ���������� ��������� �� ������ <see cref="DateChange"/>. 
        /// </summary>
        public override DateTime DateChangeBase
        {
            get => DateChange.FromTimestamp();
            set => DateChange = value.Timestamp();
        }

        public override Uri Url
        {
            get => null;           
        }

        public int IdUserChange { get; set; }

        [StringLength(2000)]
        [Display(Name = "����������� � ����"), DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [StringLength(2000)]
        [Display(Name = "����������� ��������������"), DataType(DataType.MultilineText)]
        public string CommentAdmin { get; set; }

    }
}
