using System.Collections.Generic;

namespace OnWeb.Types
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class UserState
    {
        public const int Active = 0;               //Активен
        public const int WaitForAcceptEmail = 1;   //Ожидает подтверждение по Email
        public const int WaitForAcceptAdmin = 2;   //Ожидает подтверждение администратором
        public const int Declined = 3;             //Отклонено
        public const int Disabled = 4;             //Отключен

        /// <summary>
        /// Возвращает текстовое описание конкретного состояния учетной записи пользователя.
        /// </summary>
        /// <returns></returns>
        public static string GetName(int State)
        {
            switch (State)
            {
                default: return "Неизвестно";
                case Active: return "Активен";
                case WaitForAcceptEmail: return "Ожидает подтверждение по Email";
                case WaitForAcceptAdmin: return "Ожидает подтверждение администратором";
                case Declined: return "Отклонено";
                case Disabled: return "Отключен";
            }
        }

        /// <summary>
        /// Возвращает список возможных состояний учетной записи пользователя.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetList()
        {
            return new Dictionary<int, string> {
                { Active, GetName(Active) },
                { WaitForAcceptEmail, GetName(WaitForAcceptEmail) },
                { WaitForAcceptAdmin, GetName(WaitForAcceptAdmin) },
                { Declined, GetName(Declined) },
                { Disabled, GetName(Disabled) }
            };
        }
    }
}
