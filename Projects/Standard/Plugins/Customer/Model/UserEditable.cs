using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnWeb.Plugins.Customer.Model
{
    public class UserEditable
    {
        [StringLength(128)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        public string email { get; set; }

        [StringLength(100)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.PhoneNumber)]
        public string phone { get; set; }

        [Required]
        [StringLength(100)]
        public string name { get; set; }

        [StringLength(1000)]
        public string about { get; set; }

        [StringLength(2000)]
        public string Comment { get; set; }

    }
}