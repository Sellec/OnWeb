<script type='text/javascript'>
<{if $data.id == -1}>
<{/if}>

$(document).ready(function(){
    try {
    $("#block").hide();
    
    aj = new ajaxRequest();
    aj.load_form('form_ae',null,'cmain');
    
    <{if $data.id == -1}>
    changeTitle('Добавление товара - выбор категории');
    $('div#added_item_photo').hide();
    <{else}>
    changeTitle('Редактирование товара "<{$data.name}> - выбор категории"');
    <{/if}>

    } catch(err) {alert(err);}
});
</script>
<script type='text/javascript' src='/data/js/ajaxupload.3.5.js'></script>

<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/item_add' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/item_edit/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление товара</h2><{else}><h2>Редактирование товара</h2><{/if}>

Выберите категорию товара:<br>

<select name='cat'>
<{foreach from=$cats_data item=ad key=id}>
 <option value='<{$id}>' <{if $id == $data.category}>selected<{/if}>><{$ad}></option>
<{/foreach}>
</select>

<{if $data.id == -1}>
<input type='submit' value='Перейти в добавление'> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none">
<{else}>
<input type='submit' value='Вернуться к редактированию'> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none">
<{/if}>
</form><br>

