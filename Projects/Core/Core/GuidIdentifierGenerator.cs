using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core
{
    /// <summary>
    /// Перечисление видов идентификаторов, которые могут быть зашифрованы в <see cref="Guid"/>.
    /// </summary>
    public enum GuidType
    {
        /// <summary>
        /// Неизвестный формат <see cref="Guid"/>.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Идентификатор модуля.
        /// </summary>
        Module = 1,

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        User = 2,
    }

    /// <summary>
    /// Предоставляет методы для генерации <see cref="Guid"/> на основе численных идентификаторов и получения идентификаторов обратно на основе <see cref="Guid"/>.
    /// </summary>
    public static class GuidIdentifierGenerator
    {
        private const string IdentifierPrefix = "00001234";

        /// <summary>
        /// Возвращает <see cref="Guid"/> на основе переданных идентификатора и типа идентификатора.
        /// </summary>
        /// <exception cref="ArgumentException">Возникает, если <paramref name="type"/> равен <see cref="GuidType.Unknown"/>.</exception>
        public static Guid GenerateGuid(GuidType type, int identifier)
        {
            switch (type)
            {
                case GuidType.Unknown:
                    throw new ArgumentException($"В качестве типа идентификатора нельзя передавать {nameof(GuidType.Unknown)}.", nameof(type));

                case GuidType.Module:
                case GuidType.User:
                    return new Guid($"{IdentifierPrefix}-{((int)type).ToString("0000-0000")}-000{(identifier < 0 ? 1 : 0)}-{Math.Abs(identifier).ToString("000000000000")}");

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Пробует распознать тип переданного идентификатора <paramref name="guid"/>"/> и, в случае соответствия формата, помещает тип и численное значение идентификатора в <paramref name="type"/> и <paramref name="identifier"/> соответственно. 
        /// Если формат не определен, то в <paramref name="type"/> и <paramref name="identifier"/> помещается <see cref="GuidType.Unknown"/> и 0 соответственно.
        /// </summary>
        /// <returns>Возвращает true, если формат идентификатора распознан и false в противном случае.</returns>
        public static bool TryParseGuid(Guid guid, out GuidType type, out int identifier)
        {
            type = GuidType.Unknown;
            identifier = 0;

            var parts = guid.ToString("D").Split('-');
            if (parts[0] != IdentifierPrefix) return false;

            var possibleGuidType = int.Parse(parts[1] + parts[2], System.Globalization.NumberStyles.HexNumber);
            switch (possibleGuidType)
            {
                case (int)GuidType.Module:
                case (int)GuidType.User:
                    var signType = int.Parse(parts[3], System.Globalization.NumberStyles.HexNumber) != 0 ? -1 : 1;
                    if (int.TryParse(parts[4], out int parsedValue))
                    {
                        type = (GuidType)possibleGuidType;
                        identifier = signType * parsedValue;
                        return true;
                    }
                    return false;

                default:
                    return false;
            }
        }
    }
}
