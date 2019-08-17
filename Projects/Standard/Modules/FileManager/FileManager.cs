using MimeDetective;
using OnUtils.Application;
using OnUtils.Application.Journaling;
using OnUtils.Data;
using OnUtils.Tasks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Transactions;
using System.Web.Mvc;

namespace OnWeb.Modules.FileManager
{
    using Core.Modules;
    using DictionaryFiles = Dictionary<int, DB.File>;

    /// <summary>
    /// Менеджер, позволяющий управлять файлами в хранилище файлов (локально или cdn).
    /// </summary>
    [ModuleCore("Управление файлами")]
    public class FileManager : ModuleCore<FileManager>, IUnitOfWorkAccessor<DB.DataContext>
    {
        private static FileManager _thisModule = null;
        private static ConcurrentDictionary<string, int> _servicesFlags = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// </summary>
        protected override void InitModuleCustom()
        {
            _thisModule = this;

            /*
             * Обслуживание индексов запускаем один раз при старте и раз в несколько часов
             * */
            TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(MaintenanceIndexes), DateTime.Now.AddSeconds(30), () => MaintenanceIndexesStatic());
            TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(MaintenanceIndexes) + "_hourly6", Cron.HourInterval(6), () => MaintenanceIndexesStatic());

            /*
             * Прекомпиляция шаблонов при запуске.
             * */
            //if (!Debug.IsDeveloper)
            //    Tasks.TasksManager.SetTask(typeof(Module).FullName + "_" + nameof(RazorPrecompilationStatic), DateTime.Now.AddMinutes(1), () => RazorPrecompilationStatic());

#if DEBUG
            /*
             * Регулярная сборка мусора для сборки в режиме отладки.
             * */
            TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(GCCollect) + "_minutely1", Cron.MinuteInterval(1), () => GCCollectStatic());
#endif

            TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(PlaceFileIntoQueue) + "_0", DateTime.Now.AddSeconds(10), () => PlaceFileIntoQueue());
            TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(PlaceFileIntoQueue) + "_minutely5", Cron.MinuteInterval(5), () => PlaceFileIntoQueue());

            // Не запускать не машине разработчика, иначе может быть так, что при подключении базе на удаленном сервере файлы физически останутся, а из базы будут удалены.
            if (!Debug.IsDeveloper)
            {
                TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(RemoveMarkedFiles) + "_0", DateTime.Now.AddSeconds(10), () => RemoveMarkedFiles());
                TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(RemoveMarkedFiles) + "_minutely1", Cron.MinuteInterval(1), () => RemoveMarkedFiles());
            }

            // Не запускать не машине разработчика, иначе может быть так, что при подключении базе на удаленном сервере файлы физически останутся, а из базы будут удалены.
            if (!Debug.IsDeveloper)
            {
                TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(CheckRemovedFiles) + "_0", DateTime.Now.AddSeconds(10), () => CheckRemovedFiles(true));
            }

            ModelMetadataProviders.Current = new MVC.TraceModelMetadataProviderWithFiles();
        }

        //public Dictionary<string, Conversations.ConversationBase> Conversations { get; } = new Dictionary<string, Conversations.ConversationBase>();

        #region FileManager
        /// <summary>
        /// Пытается получить файл с идентификатором <paramref name="idFile"/>.
        /// </summary>
        /// <param name="idFile">Идентификатор файла, который необходимо получить  (см. <see cref="DB.File.IdFile"/>).</param>
        /// <param name="result">В случае успеха содержит данные о файле.</param>
        /// <returns>Возвращает результат поиска файла.</returns>
        [ApiReversible]
        public NotFound TryGetFile(int idFile, out DB.File result)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    result = db.File.Where(x => x.IdFile == idFile && !x.IsRemoved && !x.IsRemoving).FirstOrDefault();
                    return result != null ? NotFound.Success : NotFound.NotFound;
                }
            }
            catch (Exception ex)
            {
                result = null;
                this.RegisterEvent(EventType.Error, "Ошибка получения файла", $"Идентификатор файла: {idFile}.", null, ex);
                return NotFound.Error;
            }
        }

        /// <summary>
        /// Пытается получить файл на основе выражения для поиска.
        /// </summary>
        /// <param name="searchExpression">Выражение, используемое для поиска подходящего файла.</param>
        /// <param name="result">В случае успеха содержит данные о первом подходящем файле.</param>
        /// <returns>Возвращает результат поиска файла.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="searchExpression"/> равен null.</exception>
        /// <exception cref="ArgumentException">Возникает, если <paramref name="searchExpression"/> содержит некорректное выражение.</exception>
        [ApiReversible]
        public NotFound TryGetFile(Expression<Func<DB.File, bool>> searchExpression, out DB.File result)
        {
            if (searchExpression == null) throw new ArgumentNullException(nameof(searchExpression));

            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    try
                    {
                        var query = db.File.Where(x => !x.IsRemoved && !x.IsRemoving).Where(searchExpression).FirstOrDefault();
                        result = query;
                    }
                    catch (NotSupportedException ex)
                    {
                        throw new ArgumentException("Некорректное выражение", nameof(searchExpression));
                    }
                    return result != null ? NotFound.Success : NotFound.NotFound;
                }
            }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                result = null;
                this.RegisterEvent(EventType.Error, "Ошибка получения файла", $"Выражение поиска: {searchExpression.ToString()}.", null, ex);
                return NotFound.Error;
            }
        }

        /// <summary>
        /// Пытается получить список файлов на основе выражения для поиска.
        /// </summary>
        /// <param name="searchExpression">Выражение, используемое для поиска подходящих файлов.</param>
        /// <param name="result">В случае успеха содержит данные обо всех найденных файлах.</param>
        /// <returns>Возвращает результат поиска файлов.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="searchExpression"/> равен null.</exception>
        /// <exception cref="ArgumentException">Возникает, если <paramref name="searchExpression"/> содержит некорректное выражение.</exception>
        [ApiReversible]
        public NotFound TryGetFiles(Expression<Func<DB.File, bool>> searchExpression, out List<DB.File> result)
        {
            if (searchExpression == null) throw new ArgumentNullException(nameof(searchExpression));

            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    try
                    {
                        var query = db.File.Where(x => !x.IsRemoved && !x.IsRemoving).Where(searchExpression);
                        result = query.ToList();
                    }
                    catch (NotSupportedException ex)
                    {
                        throw new ArgumentException("Некорректное выражение", nameof(searchExpression));
                    }
                    return result != null && result.Count > 0 ? NotFound.Success : NotFound.NotFound;
                }
            }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                result = null;
                this.RegisterEvent(EventType.Error, "Ошибка получения файлов", $"Выражение поиска: {searchExpression.ToString()}.", null, ex);
                return NotFound.Error;
            }
        }

        /// <summary>
        /// Возвращает файлы с идентификаторами из списка <paramref name="fileList"/> (см. <see cref="DB.File.IdFile"/>).
        /// </summary>
        /// <returns>
        /// Возвращает коллекцию <see cref="DictionaryFiles"/>, к которой в качестве ключей выступают идентификаторы из списка <paramref name="fileList"/>. 
        /// Для файлов, которые не были найдены, значение по ключу будет равно null.
        /// Возвращает null, если произошла ошибка.
        /// </returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="fileList"/> равен null.</exception>
        [ApiReversible]
        public DictionaryFiles GetList(IEnumerable<int> fileList)
        {
            if (fileList == null) throw new ArgumentNullException(nameof(fileList));

            try
            {
                var ids = new List<int>(fileList);
                if (ids.Count > 0)
                {
                    using (var db = this.CreateUnitOfWork())
                    {
                        var data = db.File.Where(x => ids.Contains(x.IdFile) && !x.IsRemoved && !x.IsRemoving).OrderBy(x => x.NameFile).ToDictionary(x => x.IdFile, x => x);
                        return fileList.ToDictionary(x => x, x => data.ContainsKey(x) ? data[x] : null);
                    }
                }

                return new Dictionary<int, DB.File>();
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка получения списка файлов", $"Идентификаторы файлов: {string.Join(", ", fileList)}.", null, ex);
                return null;
            }
        }

        /// <summary>
        /// Регистрирует новый файл.
        /// </summary>
        /// <param name="nameFile">Имя файла. Не должно содержать специальных символов, не разрешенных в именах файлов (см. <see cref="Path.GetInvalidFileNameChars"/>), иначе будет сгенерировано исключение <see cref="ArgumentException"/>.</param>
        /// <param name="pathFile">Путь к существующему файлу. Файл должен существовать в момент вызова, иначе будет сгенерировано исключение <see cref="FileNotFoundException"/>.</param>
        /// <param name="uniqueKey">Уникальный ключ файла, по которому его можно идентифицировать. Один и тот же ключ может быть указан сразу у многих файлов.</param>
        /// <param name="dateExpires">Дата окончения срока хранения файла, после которой он будет автоматически удален. Если равно null, то устанавливается безлимитный срок хранения.</param>
        /// <param name="result">В случае успешной регистрации содержит данные зарегистрированного файла.</param>
        /// <returns>Возвращает объект <see cref="DB.File"/>, если файл был зарегистрирован, либо null, если произошла ошибка.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="nameFile"/> является пустой строкой или null.</exception>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="pathFile"/> является пустой строкой или null.</exception>
        /// <exception cref="ArgumentException">Возникает, если <paramref name="nameFile"/> содержит специальные символы, не разрешенные в именах файлов (см. <see cref="Path.GetInvalidFileNameChars"/>).</exception>
        /// <exception cref="FileNotFoundException">Возникает, если файл <paramref name="pathFile"/> не найден на диске.</exception>
        [ApiReversible]
        public RegisterResult Register(out DB.File result, string nameFile, string pathFile, string uniqueKey = null, DateTime? dateExpires = null)
        {
            if (string.IsNullOrEmpty(nameFile)) throw new ArgumentNullException(nameof(nameFile));
            if (string.IsNullOrEmpty(pathFile)) throw new ArgumentNullException(nameof(pathFile));
            if (Path.GetInvalidFileNameChars().Any(x => nameFile.Contains(x))) throw new ArgumentException("Содержит символы, не разрешенные в имени файла.", nameof(nameFile));

            result = null;

            var pathFileFull = Path.Combine(AppCore.ApplicationWorkingFolder, pathFile);
            if (!File.Exists(pathFileFull)) return RegisterResult.NotFound; // throw new FileNotFoundException("Файл не существует", pathFile);

            try
            {
                var context = AppCore.GetUserContextManager().GetCurrentUserContext();
                if (!string.IsNullOrEmpty(uniqueKey)) uniqueKey = uniqueKey.Trim();

                var pathFileOld = string.Empty;
                using (var db = this.CreateUnitOfWork())
                {
                    var data = !string.IsNullOrEmpty(uniqueKey) ? (db.File.Where(x => x.UniqueKey == uniqueKey).FirstOrDefault() ?? null) : null;

                    if (data != null && pathFile != data.PathFile) pathFileOld = data.PathFile;

                    var isNew = false;
                    if (data == null)
                    {
                        isNew = true;
                        data = new DB.File();
                    }

                    data.IdModule = 0;
                    data.NameFile = nameFile;
                    data.PathFile = pathFile;
                    data.DateChange = DateTime.Now.Timestamp();
                    data.DateExpire = dateExpires;
                    data.IdUserChange = context.IdUser;
                    data.UniqueKey = uniqueKey;

                    var fileInfo = new FileInfo(pathFileFull);
                    var fileType = fileInfo.GetFileType();

                    data.TypeConcrete = fileType.Mime;

                    if (fileType == MimeTypes.JPEG || fileType == MimeTypes.PNG || fileType == MimeTypes.BMP || fileType == MimeTypes.GIF) data.TypeCommon = FileTypeCommon.Image;

                    if (isNew) db.File.Add(data);

                    if (db.SaveChanges() > 0)
                    {
                        if (!string.IsNullOrEmpty(pathFileOld))
                        {
                            var pathFileFullOld = Path.Combine(AppCore.ApplicationWorkingFolder, pathFileOld);
                            if (File.Exists(pathFileFullOld)) File.Delete(pathFileFullOld);
                        }
                    }
                    result = data;
                    return RegisterResult.Success;
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка регистрации файла", $"nameFile='{nameFile}'.\r\npathFile='{pathFile}'.\r\nuniqueKey='{uniqueKey}'.\r\ndateExpires={dateExpires?.ToString("dd.MM.yyyy HH:mm:ss")}.", null, ex);
                return RegisterResult.Error;
            }
        }

        /// <summary>
        /// Устанавливает новый срок хранения для файла с идентификатором <paramref name="idFile"/> (см. <see cref="DB.File.IdFile"/>).
        /// Если <paramref name="dateExpires"/> равен null, то устанавливается безлимитный срок хранения.
        /// </summary>
        /// <returns>Возвращает true, если срок обновлен, либо false, если произошла ошибка.</returns>
        [ApiReversible]
        public bool UpdateExpiration(int idFile, DateTime? dateExpires = null)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var file = db.File.Where(x => x.IdFile == idFile && !x.IsRemoved && !x.IsRemoving).FirstOrDefault();
                    if (file != null)
                    {
                        file.DateExpire = dateExpires;
                        db.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.RegisterEvent(
                    EventType.Error,
                    "Ошибка обновления срока хранения файла",
                    $"Идентификатор файла: {idFile}.\r\nНовый срок: {dateExpires?.ToString("dd.MM.yyyy HH:mm:ss")}.",
                    null,
                    ex);
                return false;
            }
        }

        /// <summary>
        /// Устанавливает новый срок хранения для файлов с идентификаторами из списка <paramref name="fileList"/> (см. <see cref="DB.File.IdFile"/>).
        /// Если <paramref name="dateExpires"/> равен null, то устанавливается безлимитный срок хранения.
        /// </summary>
        /// <returns>Возвращает true, если срок обновлен, либо false, если произошла ошибка. Возвращает true, если <paramref name="fileList"/> пуст.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="fileList"/> равен null.</exception>
        [ApiReversible]
        public bool UpdateExpiration(int[] fileList, DateTime? dateExpires = null)
        {
            if (fileList == null) throw new ArgumentNullException(nameof(fileList));
            if (fileList.IsNullOrEmpty()) return true;

            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    //Немножечко оптимизируем под параметризацию запроса - если параметров разумное количество, то строим через переменные.
                    var IdList1 = fileList.Length > 0 ? fileList[0] : 0;
                    var IdList2 = fileList.Length > 1 ? fileList[1] : 0;
                    var IdList3 = fileList.Length > 2 ? fileList[2] : 0;
                    var IdList4 = fileList.Length > 3 ? fileList[3] : 0;
                    var IdList5 = fileList.Length > 4 ? fileList[4] : 0;

                    var sql = fileList.Length > 5 ?
                                db.File.Where(x => fileList.Contains(x.IdFile) && !x.IsRemoved && !x.IsRemoving) :
                                db.File.Where(x => (x.IdFile == IdList1 || x.IdFile == IdList2 || x.IdFile == IdList3 || x.IdFile == IdList4 || x.IdFile == IdList5) && !x.IsRemoved && !x.IsRemoving);


                    if (sql.ForEach(file => file.DateExpire = dateExpires) > 0) db.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                this.RegisterEvent(
                    EventType.Error,
                    "Ошибка обновления срока хранения файлов",
                    $"Идентификаторы файлов: {string.Join(", ", fileList)}.\r\nНовый срок: {dateExpires?.ToString("dd.MM.yyyy HH:mm:ss")}.",
                    null,
                    ex);
                return false;
            }
        }

        /// <summary>
        /// Окончательно удаляет из базы и с диска файлы с идентификаторами из списка <paramref name="fileList"/> (см. <see cref="DB.File.IdFile"/>). 
        /// Не подходит для транзакционных блоков, т.к. операцию невозможно отменить.
        /// </summary>
        /// <returns>Возвращает true, если файлы удалены, либо false, если произошла ошибка. Возвращает true, если <paramref name="fileList"/> пуст.</returns>
        [ApiIrreversible]
        public bool RemoveCompletely(params int[] fileList)
        {
            if (fileList.IsNullOrEmpty()) return true;

            try
            {
                var rootDirectory = AppCore.ApplicationWorkingFolder;

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.RequiresNew))
                {
                    if (db.File.Where(x => fileList.Contains(x.IdFile) && !x.IsRemoved && !x.IsRemoving).ForEach(file =>
                    {
                        try
                        {
                            File.Delete(Path.Combine(rootDirectory, file.PathFile));
                        }
                        catch (IOException) { return; }
                        catch (UnauthorizedAccessException) { return; }
                        catch { }

                        db.File.Delete(file);
                    }) > 0)
                    {
                        db.SaveChanges();
                        scope.Commit();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка удаления файлов с истекшим сроком", null, null, ex);
                return false;
            }
        }

        /// <summary>
        /// Помечает на удаление файлы с идентификаторами из списка <paramref name="fileList"/> (см. <see cref="DB.File.IdFile"/>). 
        /// Файлы удаляются фоновым заданием через какое-то время. Рекомендуется для транзакций.
        /// </summary>
        /// <returns>Возвращает true, если файлы помечены на удаление, либо false, если произошла ошибка. Возвращает true, если <paramref name="fileList"/> пуст.</returns>
        [ApiReversible]
        public bool RemoveMark(params int[] fileList)
        {
            if (fileList.IsNullOrEmpty()) return true;

            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Required))
                {
                    if (db.File.Where(x => fileList.Contains(x.IdFile) && !x.IsRemoved && !x.IsRemoving).ForEach(file =>
                    {
                        file.IsRemoving = true;
                    }) > 0)
                    {
                        db.SaveChanges();
                        scope.Commit();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка удаления файлов с истекшим сроком", null, null, ex);
                return false;
            }
        }

        [ApiReversible]
        public void UpdateFileCount()
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var result = db.DataContext.StoredProcedure<object>("Job_FileCountUpdate");
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка обновления количества файловых связей", null, null, ex);
            }
        }
        #endregion

        #region FileManager tasks
        internal static void PlaceFileIntoQueue()
        {
            if (_servicesFlags.AddOrUpdate("PlaceFileIntoQueue", 1, (k, o) => Math.Min(int.MaxValue, o + 1)) > 1) return;

            try
            {
                using (var db = new DB.DataContext())
                {
                    db.DataContext.StoredProcedure<object>("FileManager_PlaceFileIntoQueue");
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                _thisModule?.RegisterEvent(EventType.Error, "Ошибка заполнения очереди удаления", null, ex);
            }
            finally
            {
                _servicesFlags["PlaceFileIntoQueue"] = 0;
            }
        }

        internal static void RemoveMarkedFiles()
        {
            if (_servicesFlags.AddOrUpdate("RemoveMarkedFiles", 1, (k, o) => Math.Min(int.MaxValue, o + 1)) > 1) return;
            int countFiles = 0;

            try
            {
                var executionTimeLimit = TimeSpan.FromSeconds(50);
                var dateStart = DateTime.Now;
                int idFileMax = 0;
                var rootDirectory = _thisModule?.AppCore?.ApplicationWorkingFolder;

                using (var db = new DB.DataContext())
                {
                    while ((DateTime.Now - dateStart) < executionTimeLimit)
                    {
                        var fileToRemoveQuery = (from FileRemoveQueue in db.FileRemoveQueue
                                                 join File in db.File on FileRemoveQueue.IdFile equals File.IdFile into File_j
                                                 from File in File_j.DefaultIfEmpty()
                                                 where FileRemoveQueue.IdFile > idFileMax
                                                 orderby FileRemoveQueue.IdFile ascending
                                                 select new { FileRemoveQueue, File }).Take(50);
                        var fileToRemoveList = fileToRemoveQuery.ToList();
                        if (fileToRemoveList.Count == 0) break;

                        var removeList = new List<int>();
                        var updateList = new List<DB.File>();

                        fileToRemoveList.ForEach(row =>
                        {
                            try
                            {
                                if (row.File == null)
                                {
                                    removeList.Add(row.FileRemoveQueue.IdFile);
                                }
                                else
                                {
                                    if (File.Exists(Path.Combine(rootDirectory, row.File.PathFile)))
                                        File.Delete(Path.Combine(rootDirectory, row.File.PathFile));

                                    removeList.Add(row.FileRemoveQueue.IdFile);

                                    row.File.IsRemoving = false;
                                    row.File.IsRemoved = true;
                                    updateList.Add(row.File);
                                }
                            }
                            catch (IOException) { return; }
                            catch (UnauthorizedAccessException) { return; }
                            catch { }

                            idFileMax = row.FileRemoveQueue.IdFile;
                        });

                        if (removeList.Count > 0 || updateList.Count > 0)
                        {
                            using (var scope = db.CreateScope(TransactionScopeOption.RequiresNew))
                            {
                                if (removeList.Count > 0)
                                {
                                    db.FileRemoveQueue.Where(x => removeList.Contains(x.IdFile)).Delete();
                                    db.SaveChanges<DB.FileRemoveQueue>();
                                }

                                if (updateList.Count > 0)
                                {
                                    if (updateList.Any(x => x.IsRemoving || !x.IsRemoved)) throw new Exception("Флаги удаления сбросились!");

                                    db.File.InsertOrDuplicateUpdate(updateList, new UpsertField(nameof(DB.File.IsRemoved)), new UpsertField(nameof(DB.File.IsRemoving)));
                                }

                                scope.Commit();
                            }
                            countFiles += updateList.Count;
                        }
                    }
                }

            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                _thisModule?.RegisterEvent(EventType.Error, "Ошибка заполнения очереди удаления", null, ex);
            }
            finally
            {
                _servicesFlags["RemoveMarkedFiles"] = 0;
                if (countFiles > 0) _thisModule?.RegisterEvent(EventType.Info, "Удаление файлов", $"Удалено {countFiles} файлов.", null);
            }
        }

        internal static void CheckRemovedFiles(bool isStart)
        {
            if (_servicesFlags.AddOrUpdate("CheckRemovedFiles", 1, (k, o) => Math.Min(int.MaxValue, o + 1)) > 1) return;
            bool isFinalized = false;
            int countFiles = 0;

            try
            {
                if (isStart)
                {
                    _servicesFlags["CheckRemovedFilesMax"] = 0;
                    TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(CheckRemovedFiles) + "_minutely5", Cron.MinuteInterval(5), () => CheckRemovedFiles(false));
                    _thisModule?.RegisterEvent(EventType.Info, "Запуск регулярной задачи проверки файлов.", null);
                }

                var executionTimeLimit = new TimeSpan(0, 4, 30);
                var dateStart = DateTime.Now;
                int idFileMax = _servicesFlags.GetOrAdd("CheckRemovedFilesMax", 0);
                var rootDirectory = _thisModule?.AppCore?.ApplicationWorkingFolder;

                while ((DateTime.Now - dateStart) < executionTimeLimit)
                {
                    using (var db = new DB.DataContext())
                    {
                        var filesQuery = db.File.Where(x => !x.IsRemoved && !x.IsRemoving && x.IdFile > idFileMax).OrderBy(x => x.IdFile).Take(5000);
                        var filesList = filesQuery.ToList();
                        if (filesList.Count == 0)
                        {
                            isFinalized = true;
                            break;
                        }

                        var updateList = new List<DB.File>();

                        filesList.ForEach(file =>
                        {
                            try
                            {
                                if (!File.Exists(Path.Combine(rootDirectory, file.PathFile)))
                                {
                                    file.IsRemoving = true;
                                    updateList.Add(file);
                                }
                            }
                            catch (IOException) { return; }
                            catch (UnauthorizedAccessException) { return; }
                            catch { }

                            idFileMax = file.IdFile;
                        });

                        if (updateList.Count > 0)
                        {
                            using (var scope = db.CreateScope(TransactionScopeOption.RequiresNew))
                            {
                                db.File.InsertOrDuplicateUpdate(updateList, new UpsertField(nameof(DB.File.IsRemoved)), new UpsertField(nameof(DB.File.IsRemoving)));
                                scope.Commit();
                            }
                            countFiles += updateList.Count;
                        }

                        _servicesFlags["CheckRemovedFilesMax"] = idFileMax;
                    }
                }

                if (isFinalized)
                {
                    TasksManager.RemoveTask(typeof(FileManager).FullName + "_" + nameof(CheckRemovedFiles) + "_minutely5");
                    //TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(CheckRemovedFiles) + "_0", DateTime.Now.AddHours(2), () => CheckRemovedFiles(true));
                    TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(CheckRemovedFiles) + "_0", DateTime.Now.AddMinutes(1), () => CheckRemovedFiles(true));
                    _thisModule?.RegisterEvent(EventType.Info, "Удаление регулярной задачи проверки файлов.", null);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                _thisModule?.RegisterEvent(EventType.Error, "Ошибка заполнения очереди удаления", null, ex);
            }
            finally
            {
                _servicesFlags["CheckRemovedFiles"] = 0;
                if (countFiles > 0) _thisModule?.RegisterEvent(EventType.Info, "Проверка удаленных файлов", $"На удаление помечено {countFiles} файлов.", null);
            }
        }
        #endregion

        #region Maintenance indexes tasks
        [ApiIrreversible]
        public static void MaintenanceIndexesStatic()
        {
            var module = _thisModule;
            if (module == null) throw new Exception("Модуль не найден.");

            module.MaintenanceIndexes();
        }

        private void MaintenanceIndexes()
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
                {
                    var result = db.DataContext.StoredProcedure<object>("Maintenance_RebuildIndexes", new { MinimumIndexFragmentstionToSearch = 5 });
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.CriticalError, $"Ошибка обслуживания индексов", null, ex);
                Debug.WriteLine("FileManager.Module.MaintenanceIndexes: {0}", ex.Message);
            }
        }
        #endregion

        #region RazorPrecompilation tasks
        public static void RazorPrecompilationStatic()
        {
            var module = _thisModule;
            if (module == null) throw new Exception("Модуль не найден.");

            module.RazorPrecompilation();
        }

        private void RazorPrecompilation()
        {
            try
            {
                throw new NotImplementedException();
                // todo ApplicationCore.Instance.ResourceManager.GeneratePrecompiled();
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.CriticalError, $"Ошибка прекомпиляции шаблонов", null, ex);
                Debug.WriteLine("FileManager.Module.RazorPrecompilation: {0}", ex.Message);
            }
        }
        #endregion

#if DEBUG
        #region GC collect for debug tasks
        public static void GCCollectStatic()
        {
            var module = _thisModule;
            if (module == null) throw new Exception("Модуль не найден.");

            module.GCCollect();
        }

        private void GCCollect()
        {
            GC.Collect();
        }
        #endregion
#endif

    }
}
