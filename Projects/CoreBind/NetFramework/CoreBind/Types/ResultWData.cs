using OnUtils.Data.Validation;
using System;
using System.Web.Mvc;

namespace OnWeb.CoreBind.Types
{
    /// <summary>
    /// Хранит стандартный для движка результат выполнения функции. Может использоваться в качестве результата выполнения ajax-запроса, 
    /// в этом случае будет возвращен стандартный формат ответа для $.requestJSON.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ResultWData<TData>
    {
        /// <summary>
        /// Создает новый экземпляр типа <see cref="ResultWData{TData}"/>.
        /// </summary>
        public ResultWData(bool success = false, string message = null, TData data = default(TData))
        {
            this.Success = success;
            this.Message = message;
            this.Data = data;
            this.ModelState = null;
        }

        #region Методы
        /// <summary>
        /// Добавляет в результат информацию об исключении.
        /// Свойство <see cref="Success"/> устанавливается в False.
        /// Свойство <see cref="Message"/> заполняется информацией об ошибке в исключении. В зависимости от типа исключения информация может выглядеть по-разному.
        /// </summary>
        /// <param name="ex"></param>
        public void FromException(Exception ex)
        {
            if (ex == null) return;

            this.Success = false;
            if (ex is EntityValidationException)
            {
                var exVal = ex as EntityValidationException;
                this.Message = "Возникли ошибки во время проверки данных при сохранении в базу:\r\n" + exVal.CreateComplexMessage();
            }
            //else if (ex is System.Data.Entity.Core.UpdateException)
            //{

            //}
            else
            {
                this.Message += ex.Message;

                var exx = ex.InnerException;
                var prefix = " - ";
                while (exx != null)
                {
                    var exxType = exx.GetType();

                    if (!string.IsNullOrEmpty(this.Message)) this.Message += "\r\n" + prefix;
                    this.Message += exx.Message;

                    exx = exx.InnerException;
                    prefix = "  " + prefix;
                }
            }
        }

        /// <summary>
        /// Устанавливает успешный результат.
        /// Свойство <see cref="Message"/> устанавливается равным <paramref name="message"/>, а свойство <see cref="Success"/> в True.
        /// </summary>
        public void FromSuccess(string message)
        {
            this.Message = message;
            this.Success = true;
        }

        /// <summary>
        /// Устанавливает неуспешный результат.
        /// Свойство <see cref="Message"/> устанавливается равным <paramref name="message"/>, а свойство <see cref="Success"/> в False.
        /// </summary>
        public void FromFail(string message)
        {
            this.Message = message;
            this.Success = false;
        }
        #endregion

        #region Свойства
        /// <summary>
        /// Результат выполнения операции - успех или неуспех.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Сопроводительное сообщение. Может быть сообщение об успехе, сообщение об ошибке, пустая строка.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Данные как результат выполнения операции. Может быть null (default(T)).
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// Данные о результатах проверки модели для возврата на страницу.
        /// </summary>
        public ModelStateDictionary ModelState { get; set; }
        #endregion
    }

    /// <summary>
    /// Нетипизированный вариант <see cref="ResultWData{TData}"/>.
    /// </summary>
    public class ResultWData : ResultWData<object>
    {
        /// <summary>
        /// Создает новый экземпляр типа <see cref="ResultWData"/>.
        /// </summary>
        public ResultWData(bool success = false, string message = null, object data = null) : base(success, message, data)
        {
        }
    }
}
