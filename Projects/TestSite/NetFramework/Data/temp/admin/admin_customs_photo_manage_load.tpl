<script type='text/javascript'>
function customs_photo_manage_loadsave_result(res,text,file)
{
    $('#customs_photo_manage_photo_result').text(text);
    if ( res == 1 && typeof(customs_photo_manage_refreshlist) != 'undefined' ) customs_photo_manage_refreshlist();
}

function _customs_photo_manage_loadconnect(id,type)
{
    try {
        mAddedID = id;
        $('span#customs_photo_manage_load_iid').text(mAddedID);
        
        var settings = {
            upload_url: "/admin/madmin/@Module.UrlName/photo_manage_loadsave/"+id+"/"+type, 
            
            file_post_name : 'customs_photo_upload',
            
            button_text : 'Выбрать',

            post_params: 
            {
                "_PHPSESSNAME" : "<{getCustom type=session_get data=name}>",
                "_PHPSESSID" : "<{getCustom type=session_get data=id}>",
                "load_visualeditor" : 0,
                "load_resize" : -1,
                "label" : '',
            },
            
            statusElement: $('#divStatus'),
            
            successFunc : function(code){
            },

            file_dialog_complete_handler : function(numFilesSelected, numFilesQueued) 
            {
                try {

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
                                                             
        var swf_photo_uploader = new uploadRequest(settings,'customs_photo_manage_photo_result');        

        $('#photo_upload_form_submit').click(function(){
            try { 
                swf_photo_uploader.swfUploader.addPostParam('load_module','@Module.UrlName');
                if ( $('#load_visualeditor').is(":checked") ) swf_photo_uploader.swfUploader.addPostParam('load_visualeditor',1);
                if ( $('#load_resize').is(":checked") ) swf_photo_uploader.swfUploader.addPostParam('load_resize',$('#load_resize_select option:selected').val());
                //if ( $('#auto_resize').val() != "" ) swf_photo_uploader.swfUploader.addPostParam('auto_resize',$('#auto_resize').val());
                if ( $('#label').is(":checked") ) swf_photo_uploader.swfUploader.addPostParam('label',$('#label_text').val());
                swf_photo_uploader.startUpload();
            } catch(err) {alert(err);}
            return false;
        });
        
    } catch(err) {alert(err);}
}

$(document).ready(function(){
    try {
        if ( typeof(customs_photo_manage_loadconnect) != 'undefined' ) customs_photo_manage_loadconnect();
        
        $("#cats_results .label_variant").hide();
        var load_resize = function(){
            if ( $('#load_resize').is(':checked') ) $('#load_resize_select').show();
            else $('#load_resize_select').hide();
        };
        $('#load_resize').change(load_resize);
        $('#label').change(function(){
            if ( $('#label').is(':checked') ){
                $('#label_text').show();
                $("#cats_results .label_variant").show();
            }
            else {
                $('#label_text').hide();
                $("#cats_results .label_variant").hide();
            }
        });
        $("#cats_results .label_variant").click(function(){
            $(this).parent().find("#label_text").val($(this).attr("href"))
            return false;
        });
        $('#loaded_image').hide();
    } catch (err) { alert(err); }
});
</script>
<div id='customs_photo_manage_photo_result'></div>
<table id='cats_results' class='admtable'>
 <tr>     
  <th colspan='2'>Загрузка фотографий для записи №<span id='customs_photo_manage_load_iid'></span></th>
 </tr>
 <tr>
  <td width='450'>
   <input type='checkbox' id='load_visualeditor' checked='checked'>&nbsp;Загрузить визуальный редактор после загрузки изображения.<br>
   <input type='checkbox' id='load_resize'>&nbsp;Изменить размеры изображения после загрузки.<br>
   <select id='load_resize_select' style='display:none;'>
    <option value='800_600'>800x600</option>
    <option value='1024_768'>1024x768</option>
   </select>
   <div style="display:none"><input type='checkbox' id='label'>&nbsp;Указать метку.<br>
   <a href="hidden" title="" class="label_variant">hidden</a></div>
  </td>
  <td class="center">
   <div id='fsUploadProgress'></div>
   <div id="divStatus">0 файлов загружено</div>
   <input type="text" id="txtFileName" disabled="true" style="border: solid 1px; background-color: #FFFFFF;" />
   <span id="spanButtonPlaceHolder"></span>
   <a href='' id='photo_upload_form_submit'>Загрузить файл</a>
    
   <img src='' id='loaded_image'>
  </td>
 </tr>
</table>
