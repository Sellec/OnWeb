using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.Adminmain.Design.Model
{
    using Core.DB;

    public class JournalDetails
    {
        public JournalName JournalName { get; set; }

        public List<Journal> JournalData { get; set; }
    }
}