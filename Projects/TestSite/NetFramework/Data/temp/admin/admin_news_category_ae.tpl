<script type='text/javascript'>
$(document).ready(function(){
    $("#block").hide();
    aj = new ajaxRequest();
    aj.load_form('form_ae',null,'cat_result');
    
    <{if $data.id == -1}>
    changeTitle('Добавление категории');
    <{else}>
    changeTitle("Редактирование категории: <{$data.name|java_string}>");
    <{/if}>
   
    $("#loading_img, #cat_result").hide();
    getResultAnim($("#form_ae"),$("#cat_result"));
});
</script>
<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/cats_add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/cats_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление категории</h2><{else}><h2>Редактирование категории</h2><{/if}>
<div id='cat_result'></div>
<table width='900' id='table_results' class="admtable">    
 <tr>
  <th width='200'>Поле</th>
  <th width='400'>Значение</th>  
 </tr>     
 <tr>
  <td>Название категории:</td>
  <td><input type='text' name='cat_name' size='40' maxlength='200' value='<{$data.name}>'></td>
 </tr>
 <tr>
  <td>Описание категории:</td>
  <td><textarea name='cat_descr' rows='5' cols='50'><{$data.description}></textarea></td>
 </tr>
 <tr>
  <td>Родительская категории:</td>
  <td>
    <select name='cat_sub'>
     <option value='-1' <{if -1 == $data.sub_id}>selected<{/if}>>Корневая директория</option>
<{foreach from=$cats_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.sub_id}>selected<{/if}>><{$ad}></option>
<{/foreach}>
    </select>
  </td>
 </tr>
 <tr>
  <td>Статус:</td>
  <td>
  <{if $data.id == -1}>
    <select name='cat_status'>
     <option value='0'>Отключено</option>
     <option value='1' selected>Доступно для просмотра</option>
    </select>
  <{else}>
    <select name='cat_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
   <{/if}>  
  </td>
 </tr>
</table>

<{if $data.id == -1}>
<input type='submit' value='Добавить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{else}>
<input type='submit' value='Сохранить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{/if}>
</form>
