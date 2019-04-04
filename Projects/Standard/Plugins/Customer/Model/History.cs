using System;
using System.Collections.Generic;

namespace OnWeb.Plugins.Customer.Model
{
    using Core.DB;

    public class History
    {
        public DateTime DateStart;
        public DateTime DateEnd;
        public IList<Journal> Records;
    }
}