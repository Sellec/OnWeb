<{extends "baseCustomer.tpl"}>

<{block 'body'}>
<script type="text/javascript">
$(document).ready(function(){
    $('a.tag_delete').click(function(){
        var link = $(this);
        if (confirm("Удалить этот объект?"))
            $.requestJSON("/@Module.Name/tag_delete/"+$(this).prop('rel'), null, function(result, message)
            {
                if (result == JsonResult.OK) link.parent().remove();
                else if (message.length > 0) alert(message);
            });
        
        return false;
    });
});
</script>
<{if isset($tags) && $tags|@count>0}>
 <{foreach from=$tags item=ad key=id}>
  <div class='tag_cont'><{$ad->ShowDataInCustomer()}> <a href='' class='tag_delete' rel='<{$ad->IdEntity}>'>Удалить</a></div>
 <{/foreach}>
<{else}>
 <{if $tag == 'Избранное'}>В избранном ничего нет.<{/if}>
<{/if}>
<{/block}>
