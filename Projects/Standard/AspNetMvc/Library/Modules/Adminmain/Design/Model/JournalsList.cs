using OnUtils.Application.Journaling;
using OnUtils.Application.Journaling.Model;
using System;

namespace OnWeb.Modules.Adminmain.Design.Model
{
    public class JournalsList
    {
        public JournalInfo JournalName { get; set; }

        public int EventsCount { get; set; }

        public DateTime? EventLastDate { get; set; }

        public EventType? EventLastType { get; set; }
    }
}