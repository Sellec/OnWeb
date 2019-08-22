<{extends "baseCustomer.tpl"}>

<{block 'body'}>
<script type="text/javascript">
$(document).ready(function(){
    $('a.unsubscribe').click(function(){
        var link = $(this);
        $.requestJSON("/@Module.Name/subscribesUnsubscribe/"+$(this).prop('rel'), null, function(result, message)
        {
            if (result == JsonResult.OK) link.parent().parent().remove();
            else if (message.length > 0) alert(message);
        });
        
        return false;
    });
});
</script>

Листы рассылки, от которых Вы можете отписаться<br>
<table style='width:400px'>
 <tr>
  <th>Название подписки</th>
  <th>Действия</th>
 </tr>
 <{foreach from=$listByEmail item=ad key=id}>
 <tr>
  <td><{$ad.name}></td>
  <td>
   <a href='' rel='<{$id}>' class='unsubscribe'>отписаться</a>
  </td>
 </tr>
 <{/foreach}>
</table>
<br><br>

Листы рассылки, присвоенные Вам в связи с Вашими ролями<br>
<table style='width:400px'>
 <tr>
  <th>Название подписки</th>
  <th>Роли</th>
 </tr>
 <{foreach from=$listByRole item=ad key=id}>
 <tr>
  <td><{$ad.name}></td>
  <td>
   <{foreach from=$ad.Roles item=ad2 key=id2}>
   <{$ad2}><br>
   <{/foreach}>
  </td>
 </tr>
 <{/foreach}>
</table>
<br><br>

<{/block}>
