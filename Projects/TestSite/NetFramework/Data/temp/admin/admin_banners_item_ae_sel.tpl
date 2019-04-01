<script type='text/javascript'>
<{if $data.id == -1}>
<{/if}>

$(document).ready(function(){
    try {
    $("#block").hide();
    /*
    aj = new ajaxRequest();
    aj.load_form('form_ae',null,'cmain');
    */
    <{if $data.id == -1}>
    changeTitle('Добавление баннера - выбор категории');
    $('div#added_item_photo').hide();
    <{else}>
    changeTitle('Редактирование баннера "<{$data.name}> - выбор категории"');
    <{/if}>

    $("select[name='cat'] option").click(function(){
        if ($(this).val() == 1 || $(this).val() == 3) $("select[name='pos']").hide();
        else $("select[name='pos']").show();
    })
    } catch(err) {alert(err);}
});
</script>

<{if $data.id == -1}>
<form action='/admin/mnadmin/@Module.UrlName/item_add' method='post' id='form_ae'>
<{else}>
<form action='/admin/mnadmin/@Module.UrlName/item_edit/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление баннера</h2><{else}><h2>Редактирование баннера</h2><{/if}>

Выберите категорию баннера:<br>

<select name='cat'>
<{foreach from=$cats_data item=ad key=id}>
 <option value='<{$id}>'<{if $id == $data.category}> selected="selected"<{/if}>><{$ad}></option>
<{/foreach}>
</select>
<select name='pos' style="display:none;">
 <option>Позиция</option>
 <option value='1'>Поз. 1</option>
 <option value='2'>Поз. 2</option>
 <option value='3'>Поз. 3</option>
</select>

<{if $data.id == -1}>
<input type='submit' value='Перейти в добавление'> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none">
<{else}>
<input type='submit' value='Вернуться к редактированию'> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none">
<{/if}>
</form><br>

