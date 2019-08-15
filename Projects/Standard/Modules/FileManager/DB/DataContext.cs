using OnUtils.Data;

namespace OnWeb.Modules.FileManager.DB
{
    class DataContext : Core.DB.CoreContext
    {
        public IRepository<File> File { get; set; }
        public IRepository<FileRemoveQueue> FileRemoveQueue { get; set; }
    }
}