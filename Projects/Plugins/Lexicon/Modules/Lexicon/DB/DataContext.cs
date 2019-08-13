using OnUtils.Data;

namespace OnWeb.Modules.Lexicon.DB
{
    class DataContext : Core.DB.CoreContext
    {
        public IRepository<WordCase> WordCase { get; set; }
    }
}
