namespace OnWeb.Modules.Materials.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class PageCategory
    {
        public int id { get; set; }

        public int sub_id { get; set; }

        [Required]
        [StringLength(500)]
        public string name { get; set; }

        [Required]
        public string urlname { get; set; }

        [Required]
        public string description { get; set; }

        [Required]
        [StringLength(20)]
        public string language { get; set; }

        public short status { get; set; }
    }
}
