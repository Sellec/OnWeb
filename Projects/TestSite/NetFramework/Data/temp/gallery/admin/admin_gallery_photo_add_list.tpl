<script type='text/javascript'>
$(document).ready(function(){
    $("#block").hide();
    changeTitle('Множественное добавление фотографий');

    aj = new ajaxRequest();
    aj.userOnLoad = function(){endAnim($("#action_result"));};
    aj.load_form('form_ae',null,'action_result');
    stAnim();
    
    $("#load_more").click(function(){$("#form_ae").resetForm();return false;})
    $("#allgal").click(function(){$("#table_results select option").each(function(){
        if ( $(this).val() == $("#table_results select option:selected").val() ) $(this).attr("selected","yes");
        })
        return false;
    })
    var ii = 0;
    var settings = {
        upload_url: "/admin/madmin/@Module.UrlName/upload_gall", 

        post_params: 
        {
            "_PHPSESSNAME" : "<{getCustom type=session_get data=name}>",
            "_PHPSESSID" : "<{getCustom type=session_get data=id}>"
        },
                
        file_size_limit : "10 MB",
        file_types : "*.*",
        file_types_description : "All Files",
        file_upload_limit : 10,
        file_queue_limit : 10,
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
            var clonedimg = $('#areaclone').clone();
            $(clonedimg)[0].id = 'areaclone_' + ii;
            $(clonedimg).show();
            $(clonedimg).find(".up_image").attr("src",'/'+filename).load(function() {
                var edited = 1;
                $(clonedimg).find(".crop_1").parent().hide();
                $(clonedimg).find(".crop_1").attr('src',filename);
                $(clonedimg).find(".photo_name").attr("value",filename);
                //if ( $(clonedimg).find(".up_image").width() < 600 ) {alert('Вы выбрали слишком маленькое изображение!');}
            });
            $(clonedimg).find(".up_image").show();
            
            $(clonedimg).appendTo($("#newimages"));
            ii = ii + 1;
            $("#b_add").attr("disabled",false);
        }
    };        
    var swfu = new uploadRequest(settings);
});

</script>
<{include file="admin/admin_gallery_manage_catsgals.tpl"}>

<strong>Выберите до 10 файлов:<br /></strong>
<div id='fsUploadProgress'></div>
<div id="divStatus">0 файлов загружено</div>
<div>
 <span id="spanButtonPlaceHolder"></span>
</div><br />
<form action='/admin/madmin/@Module.UrlName/photo_add_list_save' method='post' id='form_ae'>
<h2>Добавление фотографий</h2>
<select name='photo_gall'>
 <{foreach from=$galls_data item=ad key=id}>
 <option value='<{$id}>'><{$ad.name}></option>
 <{/foreach}>
</select>

<div id="newimages">
 <div id="areaclone" style="display:none;margin-bottom:10px;">
  <input type='hidden' name='photo_name[]' class='photo_name' value=''>
  <input type='hidden' name='coords[]' class='coords' value=''>
  <input type='hidden' name='sizes[]' class='sizes' value=''>
  <img src='' alt='' class='up_image' style="width:400px;" />
 </div>
</div>

<br><br><input type='submit' value='Добавить' id="b_add" disabled="disabled">
</form>