<script type='text/javascript'>
<{if $data.id == -1}>
function item_add_photoform(res,id)
{
    try {
        mAddedID = id;
        $('span#item_name_id').text(mAddedID);
        $('a.update_link').attr('href',"/admin/mnadmin/@Module.UrlName/item_photo/"+mAddedID);
        $('form#form_add_photo').attr('action',"/admin/madmin/@Module.UrlName/item_photo_new/"+mAddedID);
        $('div#added_item_photo').show();
    } catch(err) {alert(err);}
}
<{/if}>

$(document).ready(function(){
    try {
    $("#block").hide();
    aj = new ajaxRequest();
    aj.userOnLoad = function(){endAnim($("#item_result"));};
    aj.load_form('form_ae',null,'item_result');
    stAnim();
    
    <{if $data.id == -1}>
    changeTitle('Добавление баннера');
    $('div#added_item_photo').hide();
    <{else}>
    changeTitle('Редактирование баннера "<{$data.name}>"');
    <{/if}>
    
    /*select_sort($('select[name=item_cat]'));
    select_sort($('select[name=item_cat2]'));*/

<{if $data.id != -1}>
    $('select#changed_category').change(function(){
        aj = new ajaxRequest();
        aj.setPOST('cat',$('select#changed_category').val());
        aj.load('/admin/madmin/@Module.UrlName/item_edit/<{$data.id}>','cmain');        
    });

<{/if}>

    $('.it_type').click(function(){
        var val = $(this).val();
        if ( val == 1 ) $('#it_url').removeAttr('disabled');
        else $('#it_url').attr('disabled',true);
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
            filename = filename.replace('Array','');
            $('#it_bimage').val(filename);
            $('#load_pr,#it_bimage').show();
            $('#up_image').attr('src', "/"+filename);
            $('#load_pr').hide();
        }
        };        
    
    var swfu = new uploadRequest(settings);
    
    $('#btnCancel').click(function(){
        swfu.cancelQueue();
    })
    
    } catch(err) {alert(err);}
});
</script>
<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/item_add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/item_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление баннера</h2><{else}><h2>Редактирование баннера</h2><{/if}>
<a href="" class="all_turn_on">Включить все опции</a> <a href="" class="all_turn_off">Выключить все опции</a>
<table width='900' id="items_results" class="admtable">    
 <tr>
  <th></th>
  <th></th>
 </tr>
  <tr>
  <td width="175">Название:</td>
  <td><input type='text' name='item_name' size='40' maxlength='200' value='<{$data.name}>' style='margin-bottom:10px;'></td>
 </tr>
 <tr>
  <td>Описание:</td>
  <td><textarea name='item_descr' rows='7' cols='60' id="it_descr"><{$data.description}></textarea></td>
 </tr>
 <tr>
  <td>Изображение баннера:<br /><br />
  <small>Добавляется изображение <{if $data.category == 1}>655x252<{else}>236x110<{/if}> пикселей.</small>
  </td>
  <td>
    <input type='text' name='item_bimage' size='50' id="it_bimage" value="<{$data.banner_image}>"><br><br>
    <img src='<{$data.banner_image}>' alt='' id='up_image' style='margin-bottom:10px;' />

    <br /><span class="legend">Выберите изображение для баннера:</span><br>
    <div id='fsUploadProgress'></div> <img src='/data/img/loading.gif' alt='' id='load_pr' style="display:none">
    <div id="divStatus">0 файлов загружено</div>
    <div>
        <span id="spanButtonPlaceHolder"></span>
    </div>
  </td>                                         
 </tr>
 <tr>
  <td>Тип баннера:</td>
  <td>
    <input type='radio' name='item_type' value='0' class='it_type'<{if $data.type == 0}> checked='checked'<{/if}>> Внутренний &nbsp;<br>
    <input type='radio' name='item_type' value='1' class='it_type'<{if $data.type == 1}> checked='checked'<{/if}>> Внешний URL &nbsp;
  </td>
 </tr>
 <tr>
  <td>URL баннера:</td>
  <td><input type='text' name='item_url' id="it_url" value="<{$data.url}>" <{if $data.type==0}>disabled="disabled" <{/if}>/></td>
 </tr>
 <tr>
  <td>Категория:</td>
  <td>
<{if $data.id == -1}>
    <select disabled>
<{else}>
    <select id='changed_category'>
<{/if}>
<{foreach from=$cats_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.category}>selected<{/if}>><{$ad}></option>
<{/foreach}>
    </select>
    <input type='hidden' name='item_cat' value='<{$data.category}>'>
    <input type='hidden' name='pos' value='<{$data.position}>'>
  </td>
 </tr>
 <tr>
  <td>Статус:</td>
  <td>
  <{if $data.id == -1}>
    <select name='item_status'>
     <option value='0'>Отключено</option>
     <option value='1' selected>Доступно для просмотра</option>
    </select>
  <{else}>
    <select name='item_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
   <{/if}>   
  </td>
 </tr>
</table>
<br>
<div id='item_result' style="padding:0 0 10px 10px;font-size:14px;"></div>
<{if $data.id == -1}>
<input type='submit' value='Добавить' class='subm'> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none">
<{else}>
<input type='submit' value='Сохранить' class='subm'> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none">
<{/if}>
</form><br>