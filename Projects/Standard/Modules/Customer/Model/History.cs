using OnUtils.Application.Journaling.Model;
using System;
using System.Collections.Generic;

namespace OnWeb.Modules.Customer.Model
{
    public class History
    {
        public DateTime DateStart;
        public DateTime DateEnd;
        public IList<JournalData> Records;
    }
}