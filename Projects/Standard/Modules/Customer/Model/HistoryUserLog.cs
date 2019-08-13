using System;
using System.Collections.Generic;

namespace OnWeb.Modules.Customer.Model
{
    using Core.DB;

    public class HistoryUserLog
    {
        public DateTime DateStart;
        public DateTime DateEnd;
        public IList<UserLogHistory> Records;
    }
}