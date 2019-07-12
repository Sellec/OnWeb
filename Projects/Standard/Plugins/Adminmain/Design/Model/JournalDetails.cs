using OnUtils.Application.Journaling.DB;
using System.Collections.Generic;

namespace OnWeb.Plugins.Adminmain.Design.Model
{
    public class JournalDetails
    {
        public JournalName JournalName { get; set; }

        public List<Journal> JournalData { get; set; }
    }
}