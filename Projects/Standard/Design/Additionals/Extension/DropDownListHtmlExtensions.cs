using OnUtils.Application.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

public static class DropDownListHtmlExtensions
{
    public static IHtmlString DropDownList(this HtmlHelper html, string name, object selectedValue, IEnumerable<SelectListItem> selectList)
    {
        if (selectedValue != null) selectList = selectList.Select(x => new SelectListItem() { Disabled = x.Disabled, Group = x.Group, Text = x.Text, Value = x.Value, Selected = x.Value.ToLower() == selectedValue.ToString().ToLower() });

        return html.DropDownList(name, selectList);
    }

    public static IHtmlString DropDownList(this HtmlHelper html, string name, object selectedValue, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
    {
        if (selectedValue != null) selectList = selectList.Select(x => new SelectListItem() { Disabled = x.Disabled, Group = x.Group, Text = x.Text, Value = x.Value, Selected = x.Value == selectedValue.ToString() });
        return html.DropDownList(name, selectList, htmlAttributes);
    }

    public static IHtmlString DropDownList(this HtmlHelper html, string name, object selectedValue, IEnumerable<SelectListItem> selectList, object htmlAttributes)
    {
        if (selectedValue != null) selectList = selectList.Select(x => new SelectListItem() { Disabled = x.Disabled, Group = x.Group, Text = x.Text, Value = x.Value, Selected = x.Value == selectedValue.ToString() });
        return html.DropDownList(name, selectList, htmlAttributes);
    }

    public static IHtmlString DropDownList(this HtmlHelper html, string name, IEnumerable<ItemBase> selectList)
    {
        return html.DropDownList(name, selectList.AsSelectListItem());
    }

    public static IHtmlString DropDownList(this HtmlHelper html, string name, IEnumerable<ItemBase> selectList, IDictionary<string, object> htmlAttributes)
    {
        return html.DropDownList(name, selectList.AsSelectListItem(), htmlAttributes);
    }

    public static IHtmlString DropDownList(this HtmlHelper html, string name, IEnumerable<ItemBase> selectList, object htmlAttributes)
    {
        return html.DropDownList(name, selectList.AsSelectListItem(), htmlAttributes);
    }

    public static IHtmlString DropDownList(this HtmlHelper html, string name, object selectedValue, IEnumerable<ItemBase> selectList)
    {
        var selectList2 = selectList.AsSelectListItem();
        if (selectedValue != null) selectList2 = selectList2.Select(x => new SelectListItem() { Disabled = x.Disabled, Group = x.Group, Text = x.Text, Value = x.Value, Selected = x.Value.ToLower() == selectedValue.ToString().ToLower() });

        return html.DropDownList(name, selectList2);
    }

    public static IHtmlString DropDownList(this HtmlHelper html, string name, object selectedValue, IEnumerable<ItemBase> selectList, IDictionary<string, object> htmlAttributes)
    {
        var selectList2 = selectList.AsSelectListItem();
        if (selectedValue != null) selectList2 = selectList2.Select(x => new SelectListItem() { Disabled = x.Disabled, Group = x.Group, Text = x.Text, Value = x.Value, Selected = x.Value == selectedValue.ToString() });
        return html.DropDownList(name, selectList2, htmlAttributes);
    }

    public static IHtmlString DropDownList(this HtmlHelper html, string name, object selectedValue, IEnumerable<ItemBase> selectList, object htmlAttributes)
    {
        var selectList2 = selectList.AsSelectListItem();
        if (selectedValue != null) selectList2 = selectList2.Select(x => new SelectListItem() { Disabled = x.Disabled, Group = x.Group, Text = x.Text, Value = x.Value, Selected = x.Value == selectedValue.ToString() });
        return html.DropDownList(name, selectList2, htmlAttributes);
    }


}
