<script type='text/javascript'>
$(document).ready(function(){
    try {
    $("#block").hide();
    
    /*aj = new ajaxRequest();
    aj.load_form('form_ae',null,'cmain');*/
    
    changeTitle('Просмотр списка новостей - выбор новости');
    } catch(err) {alert(err);}
});
</script>

<form action='/admin/mnadmin/@Module.UrlName/news' method='post' id='form_ae'>
<h2>Просмотр новостей</h2>

Выберите категорию новостей:<br>

<select name='cat'>
 <option value='-1'>Все категории</option>
<{foreach from=$cats_data item=ad key=id}>
 <option value='<{$id}>'><{$ad}></option>
<{/foreach}>
</select>

<input type='submit' value='Перейти в просмотр' /> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none" />
</form><br>

