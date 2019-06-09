namespace OnWeb.Core.DB
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("passwords_remember")]
    public partial class PasswordRemember
    {
        public int id { get; set; }

        public int user_id { get; set; }

        [Required]
        [StringLength(32)]
        public string code { get; set; }
    }
}
