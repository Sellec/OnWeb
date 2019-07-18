using OnUtils.Application.Items;

namespace OnWeb.Core.Items
{
    /// <summary>
    /// См. <see cref="ItemBase"/>
    /// </summary>
    public interface IItemBase
    {
        /// <summary>
        /// См. <see cref="ItemBase{TAppCoreSelfReference}.ID"/>.
        /// </summary>
        int ID { get; }

        /// <summary>
        /// См. <see cref="ItemBase{TAppCoreSelfReference}.Caption"/>.
        /// </summary>
        string Caption { get; }
    }
}
