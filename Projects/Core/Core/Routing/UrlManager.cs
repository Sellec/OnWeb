using OnUtils;
using OnUtils.Application;
using OnUtils.Application.DB;
using OnUtils.Application.Journaling;
using OnUtils.Application.Modules;
using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.Routing
{
    using ExecutionResultUrl = ExecutionResult<string>;
    using ExecutionResultUrlList = ExecutionResult<Dictionary<int, string>>;

    /// <summary>
    /// Менеджер маршрутизации. Позволяет получать и управлять адресами сущностей.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Compiler", "CS0618")]
    public class UrlManager : CoreComponentBase<ApplicationCore>, IComponentSingleton<ApplicationCore>, IUnitOfWorkAccessor<UnitOfWork<DB.Routing, ModuleConfig>>, IAutoStart
    {
        private static Dictionary<string, string> TRANSLATETABLE = new Dictionary<string, string>() {
            { "а", "a" }, { "б", "b" }, { "в", "v" }, { "г", "g" }, { "д", "d" }, { "е", "e" }, { "ж", "g" }, { "з", "z" },
            { "и", "i" }, { "й", "y" }, { "к", "k" }, { "л", "l" }, { "м", "m" }, { "н", "n" }, { "о", "o" }, { "п", "p" },
            { "р", "r" }, { "с", "s" }, { "т", "t" }, { "у", "u" }, { "ф", "f" }, { "ы", "y" }, { "э", "e" }, { "ё", "yo" },
            { "х", "h" }, { "ц", "ts" }, { "ч", "ch" }, { "ш", "sh" }, { "щ", "shch" }, { "ъ", "" }, { "ь", "" }, { "ю", "yu" },
            { "я", "ya" }, { "*", "" }, { "\"", "" }, { "'", "" }, { "|", "" }, { "~", "" },
            { "`", "" }, { "@", "" }, { "#", "" }, { "№", "" }, { "$", "" }, { ";", "" }, { "%", "" }, { "^", "" }, { ":", "" },
            { "&", "-" }, { "?", "" }, { "(", "" }, { ")", "" }, { "<", "" }, { ">", "" }, { "\n", "" }, { " }, {", "" },
            { "  ", " " }, { " ", "-" }, { "!", "" }, { "+", "" }, { "=", "" }, { "…", "" }, { "...", "" }, { ".", "-" },
            { ", ", "-" }, { ",", "-" }, { "--", "-" }
        };

        /// <summary>
        /// Транслитерует текст <paramref name="text"/>. Если <paramref name="lowercaseText"/> равен true, то текст преобразуется в нижний регистр.
        /// </summary>
        public static string Translate(string text, bool lowercaseText = true)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var str = text.Trim();
            if (lowercaseText) str = str.ToLower();

            for (int i = 0; i < 50; i++)
            {
                var strOld = str;
                str = str.Replace(TRANSLATETABLE.Keys.ToArray(), TRANSLATETABLE.Values.ToArray());
                if (str == strOld) break;
            }

            var urlname = str;
            return urlname;
        }

        /// <summary>
        /// Комбинирует параметры <paramref name="urlParts"/> в строку адреса, разделенную знаком '/'.
        /// </summary>
        public static string CombineUrlParts(params string[] urlParts)
        {
            var url = new List<string>();
            var args2 = new HashSet<string>();

            foreach (var _v in urlParts)
            {
                var v = _v;
                if (!string.IsNullOrEmpty(v))
                {
                    while (v.Length > 0 && v.First() == '/') v = v.Substring(1).Trim();
                    while (v.Length > 0 && v.Last() == '/') v = v.Substring(0, v.Length - 1).Trim();
                    v = v.Trim();
                }
                if (!string.IsNullOrEmpty(v)) url.Add(v);
            }

            return "/" + string.Join("/", url);
        }

        /// <summary>
        /// </summary>
        public UrlManager()
        {
        }

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
            DeprecatedSingletonInstances.UrlManager = this;
            AppCore.Get<JournalingManager>().RegisterJournalTyped<UrlManager>("Журнал менеджера адресов");
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
            if (DeprecatedSingletonInstances.UrlManager == this) DeprecatedSingletonInstances.UrlManager = null;
        }
        #endregion

        /// <summary>
        /// Регистрирует адрес для сущности {<paramref name="IdItem"/>/<paramref name="IdItemType"/>} для модуля <paramref name="module"/>.
        /// </summary>
        /// <param name="module">Модуль, к которому относится сущность.</param>
        /// <param name="IdItem">См. описание <see cref="RegisterItem.IdItem"/>.</param>
        /// <param name="IdItemType">См. описание <see cref="RegisterItem.IdItemType"/>.</param>
        /// <param name="action">См. описание <see cref="RegisterItem.action"/>.</param>
        /// <param name="Arguments">См. описание <see cref="RegisterItem.Arguments"/>.</param>
        /// <param name="Url">См. описание <see cref="RegisterItem.Url"/>.</param>
        /// <param name="UniqueKey">См. описание <see cref="RegisterItem.UniqueKey"/>.</param>
        /// <returns>Возвращает объект <see cref="ExecutionResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="module"/> равен null.</exception>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="action"/> содержит пустое значение (пустая строка или null).</exception>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="Url"/> содержит пустое значение (пустая строка или null).</exception>
        [ApiReversible]
        public ExecutionResult Register(ModuleCore module, int IdItem, int IdItemType, string action, IEnumerable<ActionArgument> Arguments, string Url, string UniqueKey = null)
        {
            return Register(module, new RegisterItem()
            {
                IdItem = IdItem,
                IdItemType = IdItemType,
                action = action,
                Arguments = new List<ActionArgument>(Arguments),
                Url = Url,
                UniqueKey = UniqueKey,
            }.ToEnumerable());
        }

        /// <summary>
        /// Регистрирует список адресов для сущностей <paramref name="items"/> для модуля <paramref name="module"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="module"/> равен null.</exception>
        /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="items"/> равен null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Возникает, если последовательность <paramref name="items"/> не содержит элементов.</exception>
        /// <exception cref="ArgumentNullException">Возникает, если в одном из элементов последовательности <paramref name="items"/> свойство <see cref="RegisterItem.action"/> содержит пустое значение (пустая строка или null).</exception>
        /// <exception cref="ArgumentNullException">Возникает, если в одном из элементов последовательности <paramref name="items"/> свойство <see cref="RegisterItem.Url"/> содержит пустое значение (пустая строка или null).</exception>
        /// <exception cref="ArgumentException">Возникает, если комбинация {IdItem/IdItemType/action/UniqueKey} в последовательности повторяется несколько раз.</exception>
        [ApiReversible]
        public ExecutionResult Register(ModuleCore module, IEnumerable<RegisterItem> items)
        {
            try
            {
                //setError(null); //todo

                if (module == null) throw new ArgumentNullException(nameof(module));
                if (items == null) throw new ArgumentNullException(nameof(items));
                if (items.Count() == 0) throw new ArgumentOutOfRangeException(nameof(items), "Должен содержать хотя бы одно значение.");

                items = items.Where(x => !string.IsNullOrEmpty(x.Url));
                items.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.action)) throw new ArgumentNullException(nameof(x.action));
                    if (string.IsNullOrEmpty(x.Url)) throw new ArgumentNullException(nameof(x.Url));
                });

                if (items.Count() == 0) return new ExecutionResult(true);

                var groups = items.GroupBy(x => new { x.IdItem, x.IdItemType, x.action, x.UniqueKey }, x => x).Where(x => x.ToList().Count > 1).ToList();
                if (groups.Count > 0)
                {
                    var keys = groups.Select(x => $"(IdItem={x.Key.IdItem}, IdItemType={x.Key.IdItemType}, action={x.Key.action}, UniqueKey={x.Key.UniqueKey})");
                    throw new ArgumentException("Следующие ключи повторяются несколько раз: " + string.Join(", ", keys), nameof(items));
                }

                var idUser = AppCore.GetUserContextManager().GetCurrentUserContext().IdUser;

                var itemsToRegister = items.Select(x => new DB.Routing
                {
                    IdModule = module.ID,
                    IdItem = x.IdItem,
                    IdItemType = x.IdItemType,
                    Action = x.action,
                    Arguments = Newtonsoft.Json.JsonConvert.SerializeObject(x.Arguments),
                    UrlFull = "/" + x.Url.Trim('/').ToLower(),
                    UniqueKey = string.IsNullOrEmpty(x.UniqueKey) ? null : x.UniqueKey,
                    DateChange = DateTime.Now.Timestamp(),
                    IdUserChange = idUser,
                    IdRoutingType = DB.RoutingType.eTypes.Main,
                    IsFixedLength = true,
                }).ToList();

                int sql = 0;

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope())
                {
                    db.DataContext.QueryTimeout = 2 * 60 * 1000;

                    try
                    {
                        sql = db.Repo1.InsertOrDuplicateUpdate(itemsToRegister,
                            new UpsertField(nameof(DB.Routing.UrlFull)),
                            new UpsertField(nameof(DB.Routing.Arguments)),
                            new UpsertField(nameof(DB.Routing.DateChange)),
                            new UpsertField(nameof(DB.Routing.IdRoutingType)),
                            new UpsertField(nameof(DB.Routing.IsFixedLength))
                        );
                        scope.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType().FullName == "System.Data.SqlClient.SqlException")
                            Debug.WriteLineNoLog($"UrlManager.register({System.Threading.Thread.CurrentThread.ManagedThreadId}) !! with {itemsToRegister.Count} at {DateTime.Now.ToString()} with {ex.Message}");

                        throw;
                    }
                }

                if (sql == itemsToRegister.Count)
                {
                    return new ExecutionResult(true);
                }
                else
                {
                    this.RegisterEvent(
                        EventType.Error,
                        "register: ошибка при регистрации адресов.",
                        $"Модуль: {(module == null ? "не указан" : module.ID.ToString())}\r\n" +
                        (itemsToRegister.Count() > 1 ? "Часть адресов не была зарегистрирована. Операция отменена." : $"Не удалось зарегистрировать адрес '{items.First().Url}'")
                    );
                    return new ExecutionResult(false, itemsToRegister.Count() > 1 ? "Часть адресов не была зарегистрирована. Операция отменена." : $"Не удалось зарегистрировать адрес '{items.First().Url}'");
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                this.RegisterEvent(
                    EventType.Error,
                    "register: ошибка при регистрации адресов.",
                    $"Модуль: {(module == null ? "не указан" : module.ID.ToString())}",
                    exception: ex
                );
                return new ExecutionResult(false, "Возникла ошибка во время регистрации адресов.");
            }
        }

        /// <summary>
        /// Удаляет адрес для сущности {<paramref name="IdItem"/>/<paramref name="IdItemType"/>} для модуля <paramref name="module"/>.
        /// </summary>
        /// <param name="module">Модуль, к которому относится сущность.</param>
        /// <param name="IdItem">См. описание <see cref="RegisterItem.IdItem"/>.</param>
        /// <param name="IdItemType">См. описание <see cref="RegisterItem.IdItemType"/>.</param>
        /// <param name="action">См. описание <see cref="RegisterItem.action"/>.</param>
        /// <param name="UniqueKey">См. описание <see cref="RegisterItem.UniqueKey"/>.</param>
        /// <returns>Возвращает объект <see cref="ExecutionResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        [ApiReversible]
        public ExecutionResult Unregister(ModuleCore module, string action, int IdItem, int IdItemType = 1, string UniqueKey = null)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope())
                {
                    if (string.IsNullOrEmpty(UniqueKey))
                        db.Repo1.Where(x => x.IdModule == module.ID && x.IdItem == IdItem && x.IdItemType == IdItemType && x.Action == action).Delete();
                    else
                        db.Repo1.Where(x => x.IdModule == module.ID && x.IdItem == IdItem && x.IdItemType == IdItemType && x.Action == action && x.UniqueKey == UniqueKey).Delete();

                    scope.Commit();
                }

                return new ExecutionResult(true);
            }
            catch (Exception ex)
            {
                this.RegisterEvent(
                    EventType.Error,
                    "register: ошибка при удалении адреса.",
                    $"Модуль: {(module == null ? "не указан" : module.ID.ToString())}\r\naction='{action}'\r\nIdItem={IdItem}\r\nIdItemType={IdItemType}\r\nUniqueKey='{UniqueKey}'",
                    exception: ex
                );
                return new ExecutionResult(false, "Возникла ошибка во время удаления адреса.");
            }
        }

        /// <summary>
        /// Возвращает адреса для сущностей типа <paramref name="IdItemType"/> с идентификаторами из списка <paramref name="IdItemList"/> для модуля <paramref name="module"/>.
        /// </summary>
        /// <param name="module">Модуль, к которому относится регистрируемая сущность.</param>
        /// <param name="IdItemList">Список идентификаторов сущностей, для которых необходимо получить адреса. См. также описание <see cref="RegisterItem.IdItem"/>.</param>
        /// <param name="IdItemType">См. описание <see cref="RegisterItem.IdItemType"/>.</param>
        /// <param name="UniqueKey">См. описание <see cref="RegisterItem.UniqueKey"/>.</param>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionResultUrlList"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// В случае успеха свойство <see cref="ExecutionResultUrlList.Result"/> содержит коллекцию пар {идентификатор:url}, причем количество пар равно количеству идентификаторов в <paramref name="IdItemList"/>. 
        /// Если для какого-то идентификатора не был получен адрес, то значение в паре будет равно null.
        /// </returns>
        /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="module"/> равен null.</exception>
        /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="IdItemList"/> равен null.</exception>
        [ApiReversible]
        public ExecutionResultUrlList GetUrl(ModuleCore module, IEnumerable<int> IdItemList, int IdItemType, string UniqueKey = null)
        {
            try
            {
                //setError(null); // todo

                if (module == null) throw new ArgumentNullException(nameof(module));
                if (IdItemList == null) throw new ArgumentNullException(nameof(IdItemList));

                using (var db = this.CreateUnitOfWork())
                {
                    db.DataContext.QueryTimeout = 60 * 1000;

                    var coll = IdItemList.GroupBy(x => x).Select(x => x.Key).ToDictionary(x => x, x => string.Empty);

                    var queryResults = db.Repo1.
                        Where(x => x.IdModule == module.ID && coll.Keys.ToList().Contains(x.IdItem) && x.IdItemType == IdItemType && x.UniqueKey == UniqueKey).
                        Select(x => new { x.IdItem, x.UrlFull });

                    var results = queryResults.ToList();
                    results.ForEach(x => coll[x.IdItem] = x.UrlFull);

                    return new ExecutionResultUrlList(true, null, coll);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception ex)
            {
                this.RegisterEvent(
                    EventType.Error,
                    "register: ошибка при получении адресов",
                    $"Модуль: {(module == null ? "не указан" : module.ID.ToString())}\r\n" +
                    $"IdItemList='{string.Join(", ", IdItemList)}'\r\nIdItemType={IdItemType}\r\nUniqueKey='{UniqueKey}'",
                    exception: ex
                );
                return new ExecutionResultUrlList(false, "Возникла ошибка во время получения списка адресов.");
            }
        }

        /// <summary>
        /// Возвращает адрес для сущности {<paramref name="IdItem"/>/<paramref name="IdItemType"/>} для модуля <paramref name="module"/>.
        /// </summary>
        /// <param name="module">Модуль, к которому относится регистрируемая сущность.</param>
        /// <param name="IdItem">См. описание <see cref="RegisterItem.IdItem"/>.</param>
        /// <param name="IdItemType">См. описание <see cref="RegisterItem.IdItemType"/>.</param>
        /// <param name="UniqueKey">См. описание <see cref="RegisterItem.UniqueKey"/>.</param>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionResultUrl"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// В случае успеха свойство <see cref="ExecutionResultUrlList.Result"/> содержит url сущности. Если url не был найден, то <see cref="ExecutionResultUrl.Result"/> равно null.
        /// </returns>
        /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="module"/> равен null.</exception>
        [ApiReversible]
        public ExecutionResultUrl GetUrl(ModuleCore module, int IdItem, int IdItemType, string UniqueKey = null)
        {
            var result = GetUrl(module, new int[] { IdItem }, IdItemType, UniqueKey);
            return new ExecutionResultUrl(result.IsSuccess, result.Message, result.IsSuccess ? result.Result[IdItem] : null);
        }

    }
}

