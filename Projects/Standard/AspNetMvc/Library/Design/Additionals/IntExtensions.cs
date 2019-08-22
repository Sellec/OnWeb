using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class IntExtensions
    {
        /// <summary>
        /// Склоняет существительные с числительным значением.
        /// </summary>
        /// <param name="quantity">Используемое числительное</param>
        /// <param name="quantityWord1">Склонение для единственного значения (например, "день" для числа 1 или 101 и т.п.).</param>
        /// <param name="quantityWord2">Склонение для значения от 2 до 4 (например, "дня" для чисел 2, 3 или 4, или 104 и т.п.).</param>
        /// <param name="quantityWord3">Склонение для множественного значения (например, "дней" для чисел 7, 18, 116 и т.п.).</param>
        public static string Decliner(this int quantity, string quantityWord1, string quantityWord2, string quantityWord3)
        {
            var qty = Math.Abs(quantity) % 100;
            var n1 = qty % 10;
            if (qty > 10 && qty < 20) return quantityWord3;
            else if (n1 > 1 && n1 < 5) return quantityWord2;
            else if (n1 == 1) return quantityWord1;
            return quantityWord3;
        }



    }
}
