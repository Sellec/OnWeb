using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

public static class PHPHelpers
{
    public static IHtmlString number_format(this object obj, int? decimals = null, string decimalSep = ",", string thousandsSep = " ")
    {
        IHtmlString str = null;
        try
        {
            decimal? r = null;
            //if (obj is decimal || obj is byte || obj is short || obj is int || obj is long || obj is float || obj is double) r = (decimal)obj;

            if (!r.HasValue)
            {
                decimal r2 = 0;
                if (decimal.TryParse(obj.ToString(), out r2)) r = r2;
            }

            if (r.HasValue)
            {
                var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                nfi.NumberGroupSeparator = "{thdsep}";// thousandsSep;
                nfi.NumberDecimalSeparator = "{dcmlsep}";// decimalSep;

                string formatted = r.Value.ToString("N" + (decimals.HasValue ? decimals.Value : 0), nfi).Replace("{dcmlsep}", decimalSep).Replace("{thdsep}", thousandsSep); // "1 234 897.11"
                str = MvcHtmlString.Create(formatted);
            }
        }
        catch (Exception) { }

        //Debug.WriteLineNoLog("number_format({0}; {1}; {2}; {3}) = {4}", obj, decimals, decimalSep, thousandsSep, str);

        return str;
        //if (obj is int || obj is float || obj is decimal || obj is double || obj is short || )
    }
}
