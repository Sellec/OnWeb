<script type='text/javascript'>
function customs_files_upload_form_result(result_id,result_text,file_address)
{
    if ( result_id == 0 ) alert(result_text);
    else if ( result_id == 1 ) alert(result_text+': '+file_address);
    else {
        switch (result_id) {
    
    
        }
    }
}

function customs_files_upload_form_changeaddr(id)
{
    if ( typeof(swfu) != 'undefined' )
    {
        swfu.setUploadURL("/admin/madmin/@Module.UrlName/files_upload_form2/"+id)
    }
}

$(document).ready(function(){
    try{
    var settings = {
        upload_url: "/admin/madmin/@Module.UrlName/files_upload_form2/<{$data_id}>", 

        post_params: 
        {
            "_PHPSESSNAME" : "<{getCustom type=session_get data=name}>",
            "_PHPSESSID" : "<{getCustom type=session_get data=id}>",
            "customs_file_upload_show" : 0,
        },
                
        file_size_limit : "10 MB",
        file_types : "*.*",
        file_types_description : "All Files",
        file_upload_limit : 1,
        file_queue_limit : 1,
        file_post_name : 'customs_file_upload',

        button_image_url : "/data/swfupload/buttons/XPButtonUploadText_61x22.png",
        button_placeholder_id : "file_upload_form_button",
        button_width: 61,
        button_height: 22,
        button_text: '',
        button_text_style: "",
        button_text_left_padding: '',
        button_text_top_padding: '',                
        
        statusElement: $('#divStatus'),
        
        successFunc : function(filename){},
        
        file_dialog_complete_handler : function(numFilesSelected, numFilesQueued) 
        {
            try {
                /*this.startUpload();*/
            } catch (ex)  {
                this.customSettings.mainUploadObject.debug(ex);
                this.debug(ex);
            }
        },
        
        file_queued_handler : function(file) 
        {
            try {
                var txtFileName = $('#txtFileName').val(file.name);
            } catch (ex)  {
                this.customSettings.mainUploadObject.debug(ex);
                this.debug(ex);
            }
        },
        
    };        
    
    var swfu = new uploadRequest(settings,'fsUploadProgress');
    
    $('#btnCancel').click(function(){
        swfu.cancelQueue();
    });
    
    $('a.file_upload_form_close').click(function(){
        try {       
            $('div.file_upload_form').hide();
        } catch(err) {alert(err);}
        return false; 
    });
    
    $('div.file_upload_form').show();
    
    $('#file_upload_form_submit').click(function(){
        try { 
            swfu.swfUploader.addPostParam('customs_file_upload_show',$('#file_upload_form_show').val());
            swfu.startUpload();
        } catch(err) {alert(err);}
        return false;
    });
    } catch(err) {alert(err);}
});
</script>
<div class='file_upload_form' style='display:none;'>
 <div id='fsUploadProgress'></div>
 <div id="divStatus">0 файлов загружено</div>
 <input type="text" id="txtFileName" disabled="true" style="border: solid 1px; background-color: #FFFFFF;" />
 <span id="file_upload_form_button"></span>
 <a href='' id='file_upload_form_submit'>Загрузить файл</a><br>
 Как определить файл: 
 <select id='file_upload_form_show'>
  <option value='0'>Скрытый</option>
  <option value='1'>Показывать ссылку при просмотре</option>
 </select><br>
 <a href='' class='file_upload_form_close'>Закрыть форму</a>
</div>

<br><br>