namespace OnWeb.Plugins.Communication.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TelegramContacts")]
    partial class TelegramContacts
    {
        [Key]
        [Required]
        [StringLength(100)]
        public string contactID { get; set; }

        [Required]
        [StringLength(250)]
        public string contactName { get; set; }
    }
}
