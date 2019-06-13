using System;

namespace OnWeb.Plugins.Adminmain.Design.Model
{
    using Core.Journaling;
    using Core.Journaling.DB;

    public class JournalsList
    {
        public JournalName JournalName { get; set; }

        public int EventsCount { get; set; }

        public DateTime? EventLastDate { get; set; }

        public EventType? EventLastType { get; set; }
    }
}