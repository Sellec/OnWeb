using OnUtils;

namespace OnWeb.Core.Users
{
#pragma warning disable CS1591 // todo внести комментарии.
    /// <summary>
    /// Описывает пользовательскую сущность (см. <see cref="IEntitiesManager"/>).
    /// </summary>
    public abstract class UserEntity
    {
        public int IdEntity { get; set; } = 0;

        public int IdUser { get; set; } = 0;

        public string Tag { get; set; } = string.Empty;

        public ExecutionResult Init(int idEntity, string tag, string data = null)
        {
            this.IdEntity = idEntity;
            this.Tag = tag;
            return this.OnInit(data) ?? new ExecutionResult(false, $"Инициализатор {nameof(OnInit)} типа '{this.GetType().FullName}' не вернул ответ.");
        }

        protected abstract ExecutionResult OnInit(string data);

        protected virtual object GetDataForSave()
        {
            return null;
        }
    }

}
