using OnUtils.Application.Journaling.Model;
using System.Collections.Generic;

namespace OnWeb.Modules.Adminmain.Design.Model
{
    public class JournalDetails
    {
        public JournalInfo JournalName { get; set; }

        public List<JournalData> JournalData { get; set; }
    }
}