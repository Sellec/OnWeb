using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OnUtils.Data;
using OnWeb.Modules.EditableMenu.DB;
using OnWeb.Core.Modules;
using OnWeb.Design.Additionals.Plugins.Model;

namespace System.Web.Mvc
{
    public static class EditableMenuExtension
    {
        [ThreadStatic]
        private static Dictionary<int, Menu> _dbCache;

        public static IHtmlString EditableMenu(this HtmlHelper url, int IdMenu, bool WrapText = false, string Class = null)
        {

            var code = "";

            if (_dbCache == null) _dbCache = new Dictionary<int, Menu>();
            var data = _dbCache.GetValueOrDefault(IdMenu);

            if (data == null)
                using (var db = new UnitOfWork<Menu>())
                {
                    data = db.Repo1.Where(x => x.id == IdMenu).FirstOrDefault();
                    if (data != null) _dbCache.SetWithExpiration(IdMenu, data, TimeSpan.FromMinutes(5));
                }

            if (data != null)
            {
                var d = rec_code(data.code);
                var urls = d.Item1;


                var controller = url.ViewContext.Controller as ModuleControllerBase;
                if (controller != null)
                {
                    code = controller.ViewString("editablemenu.cshtml", new EditableMenu()
                    {
                        EditableMenuData = urls,
                        Class = Class,
                        WrapText = WrapText
                    });
                }
            }
            else code = "Меню не найдено.";

            return MvcHtmlString.Create(code);
        }

        private static Tuple<List<EditableMenuItem>, int> rec_code(string code, int _offset = 0)
        {
            var offset = _offset;
            var offsetPrev = 0;

            var data = new List<EditableMenuItem>();

            while (true)
            {
                //offset += offsetPrev;
                var res1 = Regex.Match(code.Substring(offset), @"\[sub=([^\]]+)\]"); //sub'
                var res2 = Regex.Match(code.Substring(offset), @"\[\/sub\]"); //sub_'
                var res3 = Regex.Match(code.Substring(offset), @"\[url=([^\]]+)\]([^\[]+)\[\/url\]"); //url'
                var res4 = Regex.Match(code.Substring(offset), @"\[suburl=([^\]]+)\]"); //suburl'
                var res5 = Regex.Match(code.Substring(offset), @"\[\/suburl\]"); //suburl_'

                offsetPrev = offset;

                if (!res1.Success && !res2.Success && !res3.Success && !res4.Success && !res5.Success) break;

                var offset_1 = res1.Success ? res1.Groups[0].Index + res1.Length + offsetPrev : 1000000000;
                //if ( $res1 > 0 ) $offset_1 = $sub[0][1] + strlen($sub[0][0]); else $offset_1 = 10000000000;

                var offset_2 = res2.Success ? res2.Groups[0].Index + res2.Length + offsetPrev : 1000000000;
                //if ( $res2 > 0 ) $offset_2 = $sub_[0][1] + strlen($sub_[0][0]); else $offset_2 = 10000000000;

                var offset_4 = res4.Success ? res4.Groups[0].Index + res4.Length + offsetPrev : 1000000000;
                //if ( $res4 > 0 ) $offset_4 = $suburl[0][1] + strlen($suburl[0][0]); else $offset_4 = 10000000000;

                var offset_5 = res5.Success ? res5.Groups[0].Index + res5.Length + offsetPrev : 1000000000;
                //if ( $res5 > 0 ) $offset_5 = $suburl_[0][1] + strlen($suburl_[0][0]); else $offset_5 = 10000000000;

                var offset_3 = res3.Success ? res3.Groups[0].Index + res3.Length + offsetPrev : 1000000000;
                //if ( $res3 > 0 ) $offset_3 = $url[0][1] + strlen($url[0][0]); else $offset_3 = 10000000000;

                offset = Math.Min(offset_1, Math.Min(offset_2, Math.Min(offset_3, Math.Min(offset_4, offset_5))));
                //$offset = min($offset_1,$offset_2,$offset_3,$offset_4,$offset_5);

                if (offset == offset_1 && res1.Success)
                {
                    var d = rec_code(code, offset_1);

                    data.Add(new EditableMenuGroup()
                    {
                        Caption = res1.Groups[1].Value,
                        Items = d.Item1
                    });

                    offset = d.Item2;
                }
                else if (offset == offset_2 && res2.Success)
                {
                    offset = offset_2;
                    return new Tuple<List<EditableMenuItem>, int>(data, offset);// array('data'=>$data, 'offset'=>$offset);
                }
                else if (offset == offset_4 && res4.Success)
                {
                    var d = rec_code(code, offset_4);

                    var arr = res4.Groups[1].Value.Split(',');
                    var dat = new EditableMenuLinkGroup() { Check = false, Class = "" };
                    for (int k = 0; k < arr.Length; k++)
                    {
                        if (k == 0) continue;
                        var ar1 = arr[k].Split('=');
                        if (ar1.Length > 0)
                        {
                            if (ar1[0].Equals("check", StringComparison.OrdinalIgnoreCase)) dat.Check = false;
                            else if (ar1[0].Equals("class", StringComparison.OrdinalIgnoreCase)) dat.Class = ar1[1];
                            else if (ar1[0].Equals("text", StringComparison.OrdinalIgnoreCase)) dat.Caption = ar1[1];
                        }
                    }
                    dat.Url = arr[0];

                    dat.Items = d.Item1;

                    data.Add(dat);

                    offset = d.Item2;
                }
                else if (offset == offset_5 && res5.Success)
                {
                    offset = offset_5;
                    return new Tuple<List<EditableMenuItem>, int>(data, offset);// array('data'=>$data, 'offset'=>$offset);
                }
                else if (offset == offset_3 && res3.Success)
                {
                    offset = offset_3;

                    var arr = res3.Groups[1].Value.Split(',');// explode(",",$url[1][0]);
                    var dat = new EditableMenuLink() { Check = false, Class = "" }; //$dat = array('check'=>false);
                    for (int k = 0; k < arr.Length; k++) //foreach ( $arr as $k =>$v )
                    {
                        //if (k == 0) continue;
                        //var ar1 = arr[k].Split('=');
                        //if (ar1[0] == "check")
                        //{
                        //    dat[ar1[0]] = (ar1[1].Length == 2) ? new List<object>() : null;
                        //}
                        //else dat[ar1[0]] = ar1[1];
                    }
                    dat.Url = arr[0];
                    dat.Caption = res3.Groups[2].Value;

                    data.Add(dat);
                }
            }

            return new Tuple<List<EditableMenuItem>, int>(data, offset);
        }

    }
}