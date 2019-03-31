<script type='text/javascript'>
$(document).ready(function(){
    try {
    $("#block").hide();
    
    changeTitle('Просмотр списка зон');
    $(".i_adminmenu ul").show();
    } catch(err) {alert(err);}
});
</script>

<form action='/admin/mnadmin/@Module.Name/zones_edit' method='post' id='form_ae'>
<h2>Просмотр списка зон</h2>

Выберите:<br>

<select name='id'>
<{foreach from=$zones_data item=ad key=id}>
 <option value='<{$id}>'><{$ad.zone_name}></option>
<{/foreach}>
</select>

<input name='edit' type='submit' value='Редактировать'>&nbsp;
<input name='delete' type='submit' value='Удалить'>
</form><br>

