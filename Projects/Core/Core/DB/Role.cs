namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo ������ �����������.
    [Table("Role")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRole { get; set; }

        [Display(Name = "�������� ����")]
        [Required(ErrorMessage = "�������� ���� �� ����� ���� ������")]
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
    }
}
