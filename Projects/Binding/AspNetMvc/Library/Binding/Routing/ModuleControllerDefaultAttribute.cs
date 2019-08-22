namespace OnWeb.Binding.Routing
{
    /// <summary>
    /// Обозначает контроллер модуля по-умолчанию.
    /// </summary>
    public sealed class ModuleControllerDefaultAttribute : Core.Modules.ModuleControllerAttribute
    {
        /// <summary>
        /// Создает новый экземпляр атрибута.
        /// </summary>
        public ModuleControllerDefaultAttribute() : base(ControllerTypeDefault.TypeID)
        {
        }
    }
}
