namespace OnWeb.Core.DB
{
    using Items;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("Role")]
    public class Role : ItemBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRole { get; set; }

        [Display(Name = "Название роли")]
        [Required(ErrorMessage = "Название роли не может быть пустым")]
        [StringLength(200)]
        public string NameRole { get; set; }

        [ScaffoldColumn(false)]
        public int IdUserCreate { get; set; }

        [ScaffoldColumn(false)]
        public int DateCreate { get; set; }

        [ScaffoldColumn(false)]
        public int IdUserChange { get; set; }

        [ScaffoldColumn(false)]
        public int DateChange { get; set; }

        [StringLength(100)]
        public string UniqueKey { get; set; }

        #region ItemBase
        public override int ID
        {
            get => IdRole;
            set => IdRole = value;
        }

        public override string Caption
        {
            get => NameRole;
            set => NameRole = value;
        }
        #endregion
    }
}
