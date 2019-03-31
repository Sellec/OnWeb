using OnUtils.Application.Users;
using OnUtils.Utils;
using System;

namespace OnWeb
{
    /// <summary>
    /// Методы расширений для раздела пользовательских менеджеров и контекстов.
    /// </summary>
    public static class UsersExtensions
    {
        /// <summary>
        /// Возвращает идентификатор пользователя, ассоциированного с контекстом. См. <see cref="Core.DB.User.id"/>.
        /// </summary> 
        public static int GetIdUser(this IUserContext context)
        {
            if (context is Core.Users.UserManager coreContext) return coreContext.IdUser;
            throw new ArgumentException("Контекст пользователя не является контекстом, используемым в веб-ядре.", nameof(context));
        }

        /// <summary>
        /// Возвращает данные пользователя, ассоциированного с контекстом. См. <see cref="Core.DB.User"/>.
        /// </summary>
        public static Core.DB.User GetData(this IUserContext context)
        {
            if (context is Core.Users.UserManager coreContext) return coreContext.GetData();
            throw new ArgumentException("Контекст пользователя не является контекстом, используемым в веб-ядре.", nameof(context));
        }

        ///// <summary>
        ///// todo Возвращает список объектов пользователя указанного контекста. Если параметр <paramref name="tag"/> не пуст, то полученные объекты фильтруются по тегу.
        ///// </summary>
        ///// <returns>Возвращает объект <see cref="ExecutionResultEntities"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        //public static ExecutionResultEntities GetEntities(this IUserContext context, string tag = null)
        //{
        //    var entitiesResult = context.GetAppCore()?.Get<Core.Users.IEntitiesManager>()?.GetUserEntities(context.GetIdUser(), tag);
        //    return entitiesResult;
        //}

        /// <summary>
        /// Хеширование пароля
        /// </summary>
        public static string hashPassword(string password, string salt = null)
        {
            if (!string.IsNullOrEmpty(salt))
                return (password.MD5() + salt).MD5();
            else
                return password.MD5();//hash("sha256", "4dd93fb3c54fc" . $password);
        }

        /// <summary>
        /// Генерирует случайный пароль указанной длины.
        /// </summary>
        public static string PasswordGenerate(int length, string chars = null, int? seed = null)
        {
            const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@\"#№$;%^:?&*()-_=+{}[]'\\/|<>. ";
            if (chars == null) chars = _chars;

            return StringsHelper.GenerateRandomString(chars, length, seed);
        }

        /// <summary>
        /// Преобразование номера телефона
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string preparePhone(string phone)
        {
            var phoneUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
            try
            {
                var phoneParsed = phoneUtil.Parse(phone, "RU");
                if (phoneUtil.IsValidNumber(phoneParsed)) return phoneUtil.Format(phoneParsed, PhoneNumbers.PhoneNumberFormat.E164);
                else throw new Exception("Некорректный номер телефона.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
