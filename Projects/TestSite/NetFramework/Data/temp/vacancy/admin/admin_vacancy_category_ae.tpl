<script type='text/javascript'>
$(document).ready(function(){
    $("#block").hide();
    aj = new ajaxRequest();
    aj.load_form('form_ae',null,'action_result');
    aj.userOnLoad = function(){endAnim($("#action_result"))};
    stAnim();

    <{if $data.id == -1}>
    changeTitle('Добавление категории');
    <{else}>
    changeTitle("Редактирование категории: <{$data.name|java_string}>");
    <{/if}>
   
    var ckeditor = CKEDITOR.replace( 'text_block',{removePlugins: 'save'} );
    $('#save_func').click(function(){
        CKEDITOR.instances.text_block.updateElement();
    });

    CKFinder.setupCKEditor( ckeditor, '/ckfinder/' );

   $("input[name='cat_name']").blur(function(){
        var str = $(this).val();
        if (str != '' && $("input[name='cat_urlname']").val() == ''){
            $.ajax({
                type    : "POST",
                url     : "/admin/madmin/@Module.UrlName/@Module.UrlName_cat_add_urlname",
                data    : { name : str }
            }).done(function(res){
                $("input[name='cat_urlname']").val(res);
            }) 
        }
    })
   
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
  <th width='100'>Поле</th>
  <th width='500'>Значение</th>  
 </tr>     
 <tr>
  <td>Название категории:</td>
  <td><input type='text' name='cat_name' size='40' maxlength='200' value='<{$data.name}>'></td>
 </tr>
 <tr>
  <td>Адрес категории:</td>
  <td>
   <input type='text' name='cat_urlname' size='40' maxlength='200' value='<{$data.urlname}>' />
   <br /><small>Меняйте только если знаете, о чем идет речь</small>
  </td>
 </tr>
 <tr>
  <td>Описание категории:</td>
  <td><textarea name='cat_descr' rows='5' cols='50' id='text_block'><{$data.description}></textarea></td>
 </tr>
 <tr style="display:none;">
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
<input type='submit' value='Добавить' id='save_func'> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{else}>
<input type='submit' value='Сохранить' id='save_func'> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{/if}>
</form>
