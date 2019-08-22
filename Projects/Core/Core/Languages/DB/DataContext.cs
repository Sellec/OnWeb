using System;
using System.Collections.Generic;
using System.Text;
using OnUtils.Data;

namespace OnWeb.Languages.DB
{
    class DataContext : Core.DB.CoreContextBase
    {
        public IRepository<Language> Language { get; set; }
    }
}
