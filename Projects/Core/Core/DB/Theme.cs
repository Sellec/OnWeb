namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("Theme")]
    public partial class Theme : Items.ItemBase
    {
        [Key]
        public int IdTheme { get; set; }

        [Required]
        [StringLength(200)]
        public string NameTheme { get; set; }

        [Required]
        [StringLength(200)]
        public string FolderName { get; set; }

        public short Active { get; set; }

        public int IdUserChange { get; set; }

        public int DateChange { get; set; }

        #region ItemBase
        /// <summary>
        /// См. <see cref="IdTheme"/>.
        /// </summary>
        public override int ID
        {
            get { return IdTheme; }
            set { IdTheme = value; }
        }

        /// <summary>
        /// См. <see cref="NameTheme"/>.
        /// </summary>
        public override string Caption
        {
            get { return NameTheme; }
            set { NameTheme = value; }
        }

        /// <summary>
        /// Время последнего изменения на основе <see cref="DateChange"/>. 
        /// </summary>
        public override DateTime DateChangeBase
        {
            get { return DateChange.FromTimestamp(); }
            set { DateChange = value.Timestamp(); }
        }
        #endregion


    }
}
