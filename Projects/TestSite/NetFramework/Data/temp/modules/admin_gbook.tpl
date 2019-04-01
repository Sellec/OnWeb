<script type='text/javascript'>
function configsavemodule()
{
    return true;
}

$(document).ready(function(){
    aj = new ajaxRequest();
    aj.load_form('config_news_form',configsavemodule,'config');
});
</script>
<form action='/admin/madmin/@Module.UrlName/modconfigsave/<{$mod}>' method='post' id='config_news_form'>
<br><hr><br>
<table width='100%'>
 <tr>
  <td>
   Количество записей на одной странице:
  </td>
  <td>
   <input type='text' size='100' name='show_categorynum' value='<{$conf.show_categorynum}>'>
  </td>
 </tr>
</table> 
<input type='submit' value='Сохранить'>
</form>
