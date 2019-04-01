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
    $("#form_ae").submit(function(){
        var div_res = $("#cat_result");
        $("#loading_img").show();
        div_res.fadeIn("slow");
        setTimeout(function(){div_res.fadeOut("slow")}, 2500 );
        var opros = setInterval(function(){
            if ( div_res.text() != "" ) {
                setTimeout(function(){div_res.text("");$("#loading_img").hide();}, 3500 );
                clearInterval(opros);
            };
        }, 1000);
        return false;
    });
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
    <select name='cat_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
  </td>
 </tr>
</table>

<{if $data.id == -1}>
<input type='submit' value='Добавить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{else}>
<input type='submit' value='Сохранить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{/if}>
</form>
