<script type='text/javascript'>
function configsavemodule()
{
    return true;
}

$(document).ready(function(){
    aj = new ajaxRequest();
    aj.load_form('config_gall_form',configsavemodule,'config');
});
</script>
<form action='/admin/madmin/@Module.UrlName/modconfigsave/<{$mod}>' method='post' id='config_gall_form'>
<br><hr><br>
<table width='100%'>
 <tr>
  <td>
   По какому критерию показывать галереи на главной странице:
  </td>
  <td>
   <select name='show_default'>
    <option value='-2' <{if $conf.show_default == -2}>selected<{/if}>>Случайные из всех категорий</option>
    <option value='-1' <{if $conf.show_default == -1}>selected<{/if}>>Случайные из корневых категорий</option>
   </select>
  </td>
 </tr>
 <tr>
  <td>
   Количество показываемых на главной:
  </td>
  <td>
   <input type='text' name='show_defaultnum' value='<{$conf.show_defaultnum}>'>
  </td>
 </tr>
 <tr>
  <td>
   Количество галерей, показываемых на одной странице:
  </td>
  <td>
   <input type='text' size='100' name='show_categorynum' value='<{$conf.show_categorynum}>'>
  </td>
 </tr>
 <tr>
  <td>
   Максимальный размер фотографии (в кб), для закачивания на сервер:
  </td>
  <td>
   <input type='text' size='100' name='photomaxsize' value='<{$conf.photomaxsize}>'>
  </td>
 </tr>
</table> 
<input type='submit' value='Сохранить'>
</form>
