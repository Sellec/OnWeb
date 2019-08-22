namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("ModuleSearchSetResult")]
    public partial class ModuleSearchSetResult
    {
        [Key]
        public int IdSearchSetResult { get; set; }

        public int IdSearchSet { get; set; }

        public int IdModule { get; set; }

        public int IdItem { get; set; }

        public byte IdItemType { get; set; }
    }
}
