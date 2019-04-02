using System;
using System.Collections.Generic;

namespace OnWeb.Plugins.Customer.Model
{
    public class HistoryUserLog
    {
        public DateTime DateStart;
        public DateTime DateEnd;
        public IList<TraceWeb.DB.UserLogHistory> Records;
    }
}