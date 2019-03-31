using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web.Mvc;

using OnWeb.Core.Items;

namespace System
{
    public static class SelectListItemExtension
    {
        public static IEnumerable<SelectListItem> AsSelectListItem(this IEnumerable<ItemBase> items)
        {
            return items.Select(x => new SelectListItem() { Text = x.Caption, Value = x.ID.ToString() });
        }

        public static IEnumerable<SelectListItem> SelectListWithSelected(this IEnumerable<SelectListItem> listSource, object selectedValue)
        {
            return SelectListWithSelected(listSource, selectedValue.ToEnumerable());
        }

        public static IEnumerable<SelectListItem> SelectListWithSelected(this IEnumerable<SelectListItem> listSource, IEnumerable<object> selectedValues)
        {
            var list = listSource;
            if (selectedValues != null && selectedValues.Count() > 0)
            {
                var values = selectedValues.Select(x => x.ToString().ToLower()).ToList();
                list = listSource.Select(x => new SelectListItem() { Disabled = x.Disabled, Group = x.Group, Text = x.Text, Value = x.Value, Selected = values.Contains(x.Value.ToLower()) });
            }

            return list;
        }

        public static IEnumerable<SelectListItem> SelectListWithSelected<T>(this IEnumerable<ItemBase> listSource, T selectedValue)
        {
            return SelectListWithSelected(listSource, selectedValue.ToEnumerable());
        }

        public static IEnumerable<SelectListItem> SelectListWithSelected<T>(this IEnumerable<ItemBase> listSource, IEnumerable<T> selectedValues)
        {
            var list = listSource.AsSelectListItem();
            if (selectedValues != null && selectedValues.Count() > 0)
            {
                var values = selectedValues.Select(x => x.ToString().ToLower()).ToList();
                list = list.Select(x => new SelectListItem() { Disabled = x.Disabled, Group = x.Group, Text = x.Text, Value = x.Value, Selected = values.Contains(x.Value.ToLower()) });
            }

            return list;
        }


    }
}
