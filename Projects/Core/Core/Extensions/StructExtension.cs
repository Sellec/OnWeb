namespace System
{
    /// <summary>
    /// </summary>
    public static class StructExtension
    {
        public static string ToStringFriendly<TEnum>(this TEnum objectValue) where TEnum : struct, IConvertible
        {
            var t = typeof(TEnum);
            var t2 = objectValue.GetType();

            var collection = OnUtils.Utils.TypeHelper.EnumFriendlyNames<TEnum>();
            return collection.ContainsKey(objectValue) ? collection[objectValue] : objectValue.ToString();
        }

    }
}
