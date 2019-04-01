<script type='text/javascript'>
$(document).ready(function(){
    $("#block").hide();
    aj = new ajaxRequest();
    aj.load_form('form_ae',null,'cat_result');
    
    <{if $data.id == -1}>
    changeTitle('Добавление категории');
    <{else}>
    changeTitle('Редактирование категории "<{$data.name}>"');
    <{/if}>

    $("#loading_img, #cat_result").hide();
    getResultAnim($("#form_ae"),$("#cat_result"));
    
    var ckeditor = CKEDITOR.replace( 'cat_descr',{removePlugins: 'save'} );
    $('#save_func').click(function(){
        CKEDITOR.instances.cat_descr.updateElement();
    });

    AjexFileManager.init({
        returnTo: 'ckeditor',
        editor: ckeditor,
        skin: 'dark'
    });
    
    var settings = {
        upload_url: "/admin/madmin/fm/upload2/userfs", 

        post_params: 
        {
            "_PHPSESSNAME" : "<{getCustom type=session_get data=name}>",
            "_PHPSESSID" : "<{getCustom type=session_get data=id}>"
        },
                
        file_size_limit : "10 MB",
        file_types : "*.*",
        file_types_description : "All Files",
        file_upload_limit : 1,
        file_queue_limit : 1,
        file_post_name : 'upload',

        button_image_url: "/data/swfupload/buttons/TestImageNoText_65x29.png",
        button_width: "65",
        button_height: "29",
        button_placeholder_id: "spanButtonPlaceHolder",
        button_text: 'Загрузить',
        button_text_style: ".theFont { font-size: 16; }",
        button_text_left_padding: 3,
        button_text_top_padding: 3,
        
        statusElement: $('#divStatus'),
        
        successFunc : function(filename){
            $('#it_bimage').val(filename);            
            $('#it_img').attr('src','/'+filename);            
        }
    };        
    
    var swfu = new uploadRequest(settings);
});
</script>
<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/cats_add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/cats_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление категории</h2><{else}><h2>Редактирование категории</h2><{/if}>
<table width='900' id='table_results' class="admtable">    
 <tr>
  <th width='100'>Поле</th>
  <th width='500' colspan='3'>Значение</th>  
 </tr>     
 <tr>
  <td>Название категории:</td>
  <td width='500' colspan='3'><input type='text' name='cat_name' size='40' maxlength='200' value='<{$data.name}>'></td>
 </tr>
 <tr>
  <td>Описание категории:</td>
  <td width='500' colspan='3'><textarea name='cat_descr' id='cat_descr' rows='5' cols='50'><{$data.description}></textarea></td>
 </tr>
 
 <tr>
  <td>Иконка:<img src="/<{if strlen($data.image)>0}><{$data.image}><{else}>data/img/design/off.jpg<{/if}>" id="it_img" /></td>
  <td width='500' colspan='3'>
    <input type='text' name='cat_image' size='50' id="it_bimage" value="<{$data.image}>" /><br><br>
    
    <span class="legend">Очередь загрузки:</span><br>
    <div id='fsUploadProgress'></div>
    <div id="divStatus">0 файлов загружено</div>
    <div><span id="spanButtonPlaceHolder"></span></div>
  </td>                                         
 </tr>
 
 <tr>
  <td>Родительская категории:</td>
  <td width='500' colspan='3'>
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
  <td width='350'>
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

  <td>Порядок:</td>
  <td><input type="text" name="cat_order" value="<{$data.order}>" /></td>
 </tr>
</table>

<{if $data.id == -1}>
<input type='submit' id='save_func' value='Добавить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{else}>
<input type='submit' id='save_func' value='Сохранить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{/if}>
</form>
<p><div id='cat_result'></div></p>
