<script type='text/javascript'>
function configsavemodule()
{
    return true;
}

$(document).ready(function(){
    aj = new ajaxRequest();
    aj.load_form('config_menu_form',configsavemodule,'config');

    aj2 = new ajaxRequest();
    aj2.load_form('file_upload',null,'banner_progress');

});

// Execute all the scripts inside of the newly-injected HTML
$("script", self).each(function(){
    eval.call(window,this.text || this.textContent || this.innerHTML || "");
});

</script>
<br><hr><br>
<form action='/admin/madmin/@Module.UrlName/modconfigsave/<{$mod}>' method='post' id='config_menu_form'>
<table width='100%'>
 <tr style="display:none">
  <td>
   По какому критерию показывать меню на главной странице:
  </td>
  <td>
   <select name='show_default'>
    <option value='-2' selected>Случайные из всех категорий</option>
   </select>
  </td>
 </tr>
 <tr>
  <td>
   Максимальный размер фотографии (в кб), для закачивания на сервер:
  </td>
  <td>
   <input type='text' name='photomaxsize' value='<{$conf.photomaxsize}>'>
  </td>
 </tr>
 <tr>
  <td>
   Сколько пунктов меню отображать на странице:
  </td>
  <td>
   <input type='text' name='show_categorynum' value='<{$conf.show_categorynum}>'>
  </td>
 </tr>
</table> 
<input type='submit' value='Сохранить'>
</form>
