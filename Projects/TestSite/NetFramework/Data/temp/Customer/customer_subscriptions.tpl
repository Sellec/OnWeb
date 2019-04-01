<{extends "baseCustomer.tpl"}>

<{block 'body'}>
<script type="text/javascript">
$(document).ready(function(){
    $('a.unsubscribe').click(function(){
        var link = $(this);
        $.requestJSON("/@Module.UrlName/subscribesUnsubscribe/"+$(this).prop('rel'), null, function(result, message)
        {
            if (result == JsonResult.OK) 
            {
                link.parent().find('.subscribe').show();
                link.hide();
            }
            else if (message.length > 0) alert(message);
        });
        
        return false;
    });

    $('a.subscribe').click(function(){
        var link = $(this);
        
        $.requestJSON("/@Module.UrlName/subscribesSubscribe/"+$(this).prop('rel'), null, function(result, message)
        {
            if (result == JsonResult.OK) 
            {
                link.parent().find('.unsubscribe').show();
                link.hide();
            }
            else if (message.length > 0) alert(message);
        });
        
        return false;
    });

});
</script>

<h2>Рассылки, от которых Вы можете отписаться</h2>
<table style='width:600px' border='1' class='info_table'>
 <tr>
  <th>Название подписки</th>
  <th>Действия</th>
 </tr>
 <{foreach from=$listByEmail item=ad key=id}>
 <tr>
  <td><{$ad.name}></td>
  <td>
   <a href='' rel='<{$id}>' class='unsubscribe' <{if !$ad.IsEnabled}>style='display:none;'<{/if}>>отписаться</a>
   <a href='' rel='<{$id}>' class='subscribe'   <{if $ad.IsEnabled}>style='display:none;'<{/if}>>подписаться</a>
  </td>
 </tr>
 <{/foreach}>
</table><br>

<h2>Рассылки, присвоенные в связи с ролями аккаунта</h2>
<table style='width:600px' border='1' class='info_table'>
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
</table><br>

<{/block}>
