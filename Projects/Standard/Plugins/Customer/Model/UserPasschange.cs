using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnWeb.Modules.Customer.Model
{
    public class UserPasschange
    {
        [StringLength(128)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string passwordOld { get; set; }

        [StringLength(128)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string passwordNew { get; set; }

    }
}