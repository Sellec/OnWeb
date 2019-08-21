using JournalingA = OnUtils.Application.Journaling;

namespace OnWeb.Modules.Adminmain.Model
{
    static class JournalQueries
    {
        public class QueryJournalData : JournalingA.DB.QueryJournalData
        {
            public int Count { get; set; }
        }

        public class JournalData : JournalingA.Model.JournalData
        {
            public int Count { get; set; }
        }


    }
}