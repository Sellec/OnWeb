﻿using MimeDetective;
using OnUtils.Application;
using OnUtils.Application.Journaling;
using OnUtils.Data;
using OnUtils.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Mvc;

namespace OnWeb.Modules.FileManager
{
    using Core.Modules;
    using DictionaryFiles = Dictionary<int, DB.File>;

    /// <summary>
    /// Общий тип файла.
    /// </summary>
    public enum FileTypeCommon : int
    {
        /// <summary>
        /// Тип по-умолчанию. Назначается, когда ничего другого подходящего не было найдено.
        /// </summary>
        Default,

        /// <summary>
        /// Все типы изображений
        /// </summary>
        Image,

        /// <summary>
        /// Все типы видео
        /// </summary>
        Video,

    }

    /// <summary>
    /// Менеджер, позволяющий управлять файлами в хранилище файлов (локально или cdn).
    /// </summary>
    [ModuleCore("Управление файлами")]
    public class FileManager : ModuleCore<FileManager>, IUnitOfWorkAccessor<UnitOfWork<DB.File>>
    {
        private static FileManager _thisModule = null;

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
                    result = db.Repo1.Where(x => x.IdFile == idFile).FirstOrDefault();
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
                        var query = db.Repo1.Where(searchExpression).FirstOrDefault();
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
                        var query = db.Repo1.Where(searchExpression);
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
                        var data = db.Repo1.Where(x => ids.Contains(x.IdFile)).OrderBy(x => x.NameFile).ToDictionary(x => x.IdFile, x => x);
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
                    var data = !string.IsNullOrEmpty(uniqueKey) ? (db.Repo1.Where(x => x.UniqueKey == uniqueKey).FirstOrDefault() ?? null) : null;

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

                    if (isNew) db.Repo1.Add(data);

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
                    var file = db.Repo1.Where(x => x.IdFile == idFile).FirstOrDefault();
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
                                db.Repo1.Where(x => fileList.Contains(x.IdFile)) :
                                db.Repo1.Where(x => x.IdFile == IdList1 || x.IdFile == IdList2 || x.IdFile == IdList3 || x.IdFile == IdList4 || x.IdFile == IdList5);


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
                    if (db.Repo1.Where(x => fileList.Contains(x.IdFile)).ForEach(file =>
                    {
                        try
                        {
                            File.Delete(Path.Combine(rootDirectory, file.PathFile));
                        }
                        catch (IOException) { return; }
                        catch (UnauthorizedAccessException) { return; }
                        catch { }

                        db.Repo1.Delete(file);
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
                    if (db.Repo1.Where(x => fileList.Contains(x.IdFile)).ForEach(file =>
                    {
                        file.DateExpire = DateTime.Now.AddSeconds(-1);
                        //todo добавить метку в таблицу для фонового задания.
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

        [ApiIrreversible]
        public void ClearExpired()
        {
            try
            {
                int countCleared = 0;

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.RequiresNew))
                {
                    var rootDirectory = AppCore.ApplicationWorkingFolder;

                    db.Repo1.Where(x => x.DateExpire <= DateTime.Now).ForEach(file =>
                    {
                        try
                        {
                            if (File.Exists(Path.Combine(rootDirectory, file.PathFile)))
                                File.Delete(Path.Combine(rootDirectory, file.PathFile));
                        }
                        catch (IOException) { return; }
                        catch (UnauthorizedAccessException) { return; }
                        catch { }

                        db.Repo1.Delete(file);
                    });

                    countCleared = db.SaveChanges();
                    scope.Commit();
                }

                if (countCleared > 0)
                {
                    this.RegisterEvent(EventType.Info, "Удаление файлов с истекшим сроком", $"Удалено {countCleared} файлов.");
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка удаления файлов с истекшим сроком", null, null, ex);
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

        #region Maintenance indexes
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

        #region RazorPrecompilation
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
        #region GC collect for debug
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