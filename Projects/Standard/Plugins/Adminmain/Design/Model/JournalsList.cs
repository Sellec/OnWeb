using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.ModuleAdminmain.Design.Model
{
    using Core.DB;
    using Core.Journaling;

    public class JournalsList
    {
        public JournalName JournalName { get; set; }

        public int EventsCount { get; set; }

        public DateTime? EventLastDate { get; set; }

        public EventType? EventLastType { get; set; }
    }
}