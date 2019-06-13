using System.Collections.Generic;

namespace OnWeb.Plugins.Adminmain.Design.Model
{
    using Core.Journaling.DB;

    public class JournalDetails
    {
        public JournalName JournalName { get; set; }

        public List<Journal> JournalData { get; set; }
    }
}