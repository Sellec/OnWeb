using OnUtils.Application.Journaling;
using OnUtils.Application.Journaling.DB;
using System;

namespace OnWeb.Modules.Adminmain.Design.Model
{
    public class JournalsList
    {
        public JournalName JournalName { get; set; }

        public int EventsCount { get; set; }

        public DateTime? EventLastDate { get; set; }

        public EventType? EventLastType { get; set; }
    }
}