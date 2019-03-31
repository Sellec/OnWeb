<script type='text/javascript'>
function configsavemodule()
{
    return true;
}

$(document).ready(function(){
    aj = new ajaxRequest();
    aj.load_form('config_news_form',configsavemodule,'config');
    
    $("select[name=IdSubscription]").val('<{$conf.IdSubscription}>');
});
</script>
<form action='/admin/madmin/adminmenu/modconfigsave/<{$mod}>' method='post' id='config_news_form'>
<br><hr><br>
<table width='100%'>
 <tr>
  <td>По какому критерию показывать новости на главной странице:</td>
  <td>
   <select name='show_default'>
    <option value='-2' <{if $conf.show_default == -2}>selected<{/if}>>Случайные из всех категорий</option>
    <option value='-1' <{if $conf.show_default == -1}>selected<{/if}>>Случайные из корневых категорий</option>
   </select>
  </td>
 </tr>
 <tr>
  <td>Количество показываемых на главной новостей:</td>
  <td><input type='text' name='show_defaultnum' value='<{$conf.show_defaultnum}>'></td>
 </tr>
 <tr>
  <td>Количество новостей, показываемых на одной странице:</td>
  <td><input type='text' size='100' name='show_categorynum' value='<{$conf.show_categorynum}>'></td>
 </tr>
 <tr>
  <td>Подписка, используемая для рассылки новостей:</td>
  <td>
   <select name='IdSubscription'>
    <option value='0'>Не выбрано</option>
    <{foreach from=$list item=ad key=id}><option value='<{$id}>'><{$ad.name}></option><{/foreach}>
   </select>
  </td>
 </tr>
</table> 
<input type='submit' value='Сохранить'>
</form>
