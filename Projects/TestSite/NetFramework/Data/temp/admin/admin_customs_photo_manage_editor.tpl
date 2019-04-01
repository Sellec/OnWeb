<div id='customs_photo_manage_editor'>

<script type='text/javascript'>
function customs_photo_manage_photo_edit_result(res,text,main_file,preview_file)
{
    $('#customs_photo_manage_result').text(text);
    $('#action_result').text(text);
    endAnim($("#action_result"))

    customs_photo_manage_photo_editor_update_images(main_file,preview_file);
}

function editor_function(mode,preview)
{
    var aj = new ajaxRequest();
    aj.setPOST('preview',preview);
    
    var need_load = 0;
    
    var mother_div = 'modes_main_div';
    if ( preview == 1 ) mother_div = 'modes_preview_div';
    
    $("#editor_loading_img").fadeIn();
    $("#admin_botgr,#action_result").fadeOut();
    if ( mode == 'crop' )
    {
        need_load = 1;
        
        aj.setPOST('mode','crop');
        aj.setPOST('crop_x',$('div#'+mother_div).find('input[name=crop_x]').val());
        aj.setPOST('crop_y',$('div#'+mother_div).find('input[name=crop_y]').val());
        aj.setPOST('crop_width',$('div#'+mother_div).find('input[name=crop_w]').val());
        aj.setPOST('crop_height',$('div#'+mother_div).find('input[name=crop_h]').val());
        aj.userOnLoad = function(){$("#editor_loading_img").fadeOut();
            if ( mother_div == 'modes_preview_div' && $("#modes_preview_div select.pre_select option:selected").attr("rel") != 0 ) editor_function('resize',1);
        };
    } else if ( mode == 'preview_new' )
    {
        need_load = 1;

        aj.userOnLoad = function(){$("#editor_loading_img").fadeOut();};
        aj.setPOST('mode','preview_new');
    } else if ( mode == 'resize' )
    {
        need_load = 1;

        aj.setPOST('mode','resize');
        aj.userOnLoad = function(){$("#editor_loading_img").fadeOut();};
        aj.setPOST('resize_select',$('div#'+mother_div).find('select[name=resize_select] option:selected').val());
    } else if ( mode == 'rotate' )
    {
        need_load = 1;
        
        aj.userOnLoad = function(){$("#editor_loading_img").fadeOut();};
        aj.setPOST('mode','rotate');
        aj.setPOST('rotate_angle',$('div#'+mother_div).find('input[name=rotate_angle]').val());
    } else if ( mode == 'label' )
    {
        need_load = 1;
        
        aj.userOnLoad = function(){$("#editor_loading_img").fadeOut();};
        aj.setPOST('mode','label');
        aj.setPOST('label_text',$('#editor_label').val());
    } else if ( mode == 'comment' )
    {
        need_load = 1;
        
        aj.userOnLoad = function(){$("#editor_loading_img").fadeOut();};
        aj.setPOST('mode','comment');
        aj.setPOST('comment_text',$('#editor_comment').val());
    } else if ( mode == 'author' )
    {
        need_load = 1;
        
        aj.userOnLoad = function(){$("#editor_loading_img").fadeOut();};
        aj.setPOST('mode','author');
        aj.setPOST('author_name',$('#editor_author').val());
    }
    
    $('img#preview_image').imgAreaSelect({hide:true}); 
    $('img#main_image').imgAreaSelect({hide:true}); 
    if ( need_load == 1 ) aj.load('/admin/madmin/@Module.UrlName/photo_manage_photo_edit/<{$data.photo_id}>','customs_photo_manage_result');
}

var mCustomsPhotoCurrentMode;
function customs_photo_manage_photo_editor_mode(mode)
{                        
    $('#'+mCustomsPhotoCurrentMode+'_div').hide();
    mCustomsPhotoCurrentMode = mode;
    $('#'+mCustomsPhotoCurrentMode+'_div').show();
}

function customs_photo_manage_photo_editor_update_images(main,preview)
{
    $('img#main_image').attr('src',main+'?'+Math.random()).load(function(){
        $('img#main_image').imgAreaSelect({ 
            movable: true,
            resizable: true,
            onSelectEnd: imageselection_main 
        });
    });

    $('img#preview_image').attr('src',preview+'?'+Math.random()).load(function(){
        $('img#preview_image').imgAreaSelect({ 
            movable: true,
            resizable: true,
            onSelectEnd: imageselection_preview 
        });
    }); 
    
}       

function imageselection_preview(img, selection)
{
    $('div#modes_preview_div').find('input[name=crop_x]').val(selection.x1);
    $('div#modes_preview_div').find('input[name=crop_y]').val(selection.y1);
    $('div#modes_preview_div').find('input[name=crop_w]').val(selection.width);
    $('div#modes_preview_div').find('input[name=crop_h]').val(selection.height);
}
function imageselection_main(img, selection)
{
    $('div#modes_main_div').find('input[name=crop_x]').val(selection.x1);
    $('div#modes_main_div').find('input[name=crop_y]').val(selection.y1);
    $('div#modes_main_div').find('input[name=crop_w]').val(selection.width);
    $('div#modes_main_div').find('input[name=crop_h]').val(selection.height);
}

$(document).ready(function(){
    $('a#customs_photo_manage_editor_close').click(function(){
        $('#customs_photo_manage_editor').parent().hide();
        $('img#preview_image,img#main_image').imgAreaSelect({hide:true});
        return false;
    });
    
    $('#customs_photo_manage_editor').parent().show();
    
    $('a.editor_funcs').click(function(){
        editor_function($(this).attr('id'),0);
        return false;
    });        
    
    $('a.editor_funcs_preview').click(function(){
        editor_function($(this).attr('id'),1);
        return false;
    });        
    
    $('a.modes_select').click(function(){
        customs_photo_manage_photo_editor_mode($(this).attr('id'));
        if ( !$(this).hasClass("editor_highlighted") ) $('a.modes_select').toggleClass("editor_highlighted");
        return false;             
    });
    
    $('a#editor_label_save').click(function(){
        editor_function('label',0);
        return false;             
    });
    
    $('a#editor_comment_save').click(function(){
        editor_function('comment',0);
        return false;             
    });
    
    $('a#editor_author_save').click(function(){
        editor_function('author',0);
        return false;             
    });
    
    
    $("#modes_preview_div select.pre_select").change(function(){
        var opt = $(this).find(":selected");
        var reg = /(\d+)_(\d+)/;
        var sizes = reg.exec(opt.val());
        $('img#preview_image').imgAreaSelect({hide:true});
        $('img#preview_image').imgAreaSelect({show: true,movable: true,resizable: true,aspectRatio:opt.attr("rel"),minWidth:sizes[1],minHeight:sizes[2],onSelectEnd: imageselection_preview });
    })
    
    customs_photo_manage_photo_editor_update_images('<{$data.photo_value}>','<{$data.photo_preview_value}>');
    customs_photo_manage_photo_editor_mode('modes_main');             
    $("#modes_main").addClass("editor_highlighted");
    $("#help_click").hover(function(e){
        $("body").prepend('<div id="help_frm"><b></b><br /></div>')
        e = e || window.event
        if (e.pageX == null && e.clientX != null ) { 
            var html = document.documentElement
            var body = document.body
    
            e.pageX = e.clientX + (html && html.scrollLeft || body && body.scrollLeft || 0) - (html.clientLeft || 0)
            e.pageY = e.clientY + (html && html.scrollTop || body && body.scrollTop || 0) - (html.clientTop || 0)
        }
        $("#help_frm").css({'top':e.pageX-45,'left':e.pageY+25});
        $("#help_frm").fadeIn();
    },function(){
        $("#help_frm").fadeOut();
    })
    $("a.label_variant").click(function(){
        $(this).parent().find("#editor_label").val($(this).attr("href"));
        return false;
    })
    
});
</script>
<div id='customs_photo_manage_result' style="display:none"></div>
<h4>Визуальный редактор, фотография №<{$data.photo_id}>. <img src="/data/img/loading.gif" alt="Идет загрузка" id="editor_loading_img" style="display:none" /></h4>

<div>
<input type='text' readonly size='50' id="it_bimage" value="<{$data.photo_value}>" style="display:none;" /><br>
Комментарий:&nbsp;<input type='text' size='20' id="editor_comment" value="<{$data.photo_comment}>">&nbsp;
<a href='' alt='Сохранить' title='Сохранить' id='editor_comment_save'><img style="vertical-align:middle" src='data/img/admin/save.gif' width='20' height='20' alt='Сохранить'></a><br>

<div style="display:none">Автор:&nbsp;<input type='text' size='20' id="editor_author" value="<{$data.photo_author}>">&nbsp;
<a href='' alt='Сохранить' title='Сохранить' id='editor_author_save'><img style="vertical-align:middle" src='data/img/admin/save.gif' width='20' height='20' alt='Сохранить'></a><br>

Метка:&nbsp;<input type='text' size='20' id="editor_label" value="<{$data.photo_label}>">&nbsp;
<a href='' alt='Сохранить' title='Сохранить' id='editor_label_save'><img style="vertical-align:middle" src='data/img/admin/save.gif' width='20' height='20' alt='Сохранить'></a><br>
<a href="logo" title="" class="label_variant">logo</a>, <a href="poster" title="" class="label_variant">poster</a>, <a href="hidden" title="" class="label_variant">hidden</a><br><br>
</div>
</div>    
<div style="width:400px;" class="photo_manager_panel">
 
 <p><a href='' id='modes_main' class='modes_select'>Основное изображение</a>&nbsp; | &nbsp;
 <a href='' id='modes_preview' class='modes_select'>Превью</a>&nbsp;&nbsp; <a href="" title="" id="help_click">(?)</a></p>
 <div id='modes_main_div' style='display:none;'>
  <div class='manager_toolbar'>
   <select name='resize_select'>
    <option value='600_450'>600x450</option>
    <option value='800_600'>800x600</option>
    <option value='1024_768'>1024x768</option>
   </select>&nbsp;
   <a href='' alt='Изменить размеры' title='Изменить размеры' class='editor_funcs' id='resize'><img src='data/img/admin/resize.gif' width='20' height='20' alt='Изменить размеры'></a>&nbsp;|&nbsp;
   <a href='' alt='Обрезать изображение' title='Обрезать изображение' class='editor_funcs' id='crop'><img src='data/img/admin/crop.gif' width='20' height='20' alt='Обрезать изображение'></a><input type='hidden' name='crop_x'><input type='hidden' name='crop_y'><input type='hidden' name='crop_w'><input type='hidden' name='crop_h'>&nbsp;|&nbsp;
   <input name='rotate_angle' size='5' value='0'>&nbsp;
   <a href='' class='editor_funcs' id='rotate'><img src='data/img/admin/rotate.gif' width='20' height='20' alt='Повернуть изображение' title='Повернуть изображение'></a>&nbsp;
  </div>
  <br />
 
  <img src='' alt='' id='main_image' />
 </div>
 
 <div id='modes_preview_div' style='display:none;'>
  <div class='manager_toolbar'>
   <a href='' class='editor_funcs_preview' id='preview_new'><img src='data/img/admin/new.gif' width='20' height='20' alt='Создание нового превью'></a>&nbsp;|&nbsp;
   <select name='resize_select' class='pre_select'>
    <option value='none' rel='0'>Без ограничений</option>
    <option value='150_150' rel='1:1'>150x150 (Новости)</option>
    <option value='100_100' rel='1:1'>100x100 (Предпросмотр)</option>
    <option value='200_150' rel='4:3'>200x150 (Страницы)</option>
   </select>&nbsp;
   <a href='' class='editor_funcs_preview' id='resize'><img src='data/img/admin/resize.gif' width='20' height='20' alt='Изменить размеры'></a>&nbsp;|&nbsp;
   <a href='' class='editor_funcs_preview' id='crop'><img src='data/img/admin/crop.gif' width='20' height='20' alt='Обрезать изображение'></a><input type='hidden' name='crop_x'><input type='hidden' name='crop_y'><input type='hidden' name='crop_w'><input type='hidden' name='crop_h'>&nbsp;|&nbsp;
   <input name='rotate_angle' size='5' value='0'>&nbsp;
   <a href='' class='editor_funcs_preview' id='rotate'><img src='data/img/admin/rotate.gif' width='20' height='20' alt='Повернуть изображение'></a>&nbsp;
  </div>
  <br />

  <img src='' alt='' id='preview_image' />
 </div>
 
</div>


<br><a href='' id='customs_photo_manage_editor_close'>Закрыть редактор</a><br>
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------<br>
</div>
