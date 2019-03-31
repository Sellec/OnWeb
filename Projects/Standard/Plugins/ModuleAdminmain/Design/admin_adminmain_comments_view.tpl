<script type='text/javascript'>
$(document).ready(function() {
    $("#block").hide();
    changeTitle("Просмотр комментариев");
    $(".i_adminmenu ul").show();
});
</script>

<h2>Комментарии</h2>
<table id="items_results" class="admtable">
 <tr>
  <th>№</th>
  <th>Текст</th>
  <th width="135"></th>
  <th width="110"></th>
 </tr>
 <{foreach from=$comments item=ad key=id name=comments}>
 <tr>
  <td class="center"><{$ad.comm_id}></td>
  <td><{$ad.comm_text}></td>
  <td><{if isset($ad.comm_user_email) && $ad.comm_user_email != ""}><b>@</b> <a href="mailto:<{$ad.comm_user_email}>" title=""><{$ad.comm_user_name}></a><{elseif isset($ad.comm_user_id) && $ad.comm_user_id != 0}><a href="/users/<{$ad.comm_user_name}>" title="" target="_blank"><{$ad.comm_user_name}><{else}><{$ad.comm_user_name}><{/if}></td>
  <td><{if isset($ad.comm_module) && $ad.comm_module != ""}><a href="/<{if $ad.comm_module == "gallery"}>gallery/photo-<{else}><{$ad.comm_module}>/<{/if}><{if $ad.comm_module == "pages"}><{$ad.urlname}><{else}><{$ad.comm_item_id}><{/if}>#comments" title="" target="_blank">Читать</a>&nbsp;&rarr;<{/if}><br /><{$ad.comm_time|strftime:"%d.%m.%Y %H:%M"}></td>
 </tr>
 <{/foreach}>
</table>