<script type='text/javascript'>
$(document).ready(function(){
    try {
    $("#block").hide();
    
    changeTitle('Просмотр списка баннеров - выбор категории');
    } catch(err) {alert(err);}
});
</script>

<form action='/admin/mnadmin/@Module.UrlName/items' method='post' id='form_ae'>
<h2>Просмотр баннеров</h2>

Выберите категорию:<br>

<select name='cat'>
 <option value='-1'>Все категории</option>
<{foreach from=$cats_data item=ad key=id}>
 <option value='<{$id}>'><{$id}> <{$ad}></option>
<{/foreach}>
</select>

<input type='submit' value='Перейти в просмотр'> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none">
</form><br>

