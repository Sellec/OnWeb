using OnUtils.Application.Journaling.DB;
using System;
using System.Collections.Generic;

namespace OnWeb.Plugins.Customer.Model
{
    public class History
    {
        public DateTime DateStart;
        public DateTime DateEnd;
        public IList<Journal> Records;
    }
}