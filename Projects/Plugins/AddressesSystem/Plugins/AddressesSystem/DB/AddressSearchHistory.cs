namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("AddressSearchHistory")]
    public partial class AddressSearchHistory
    {
        [Key]
        public int IdAddressSearch { get; set; }

        [Required]
        public string NameAddressSearch { get; set; }

        public DateTime DateSearch { get; set; }

        public bool IsSuccess { get; set; }

        [StringLength(32)]
        public string KodAddress { get; set; }

        public Addresses.AddressType AddressType { get; set; }

        [StringLength(50)]
        public string ServiceFound { get; set; }

        public string ServiceAnswer { get; set; }



    }
}
