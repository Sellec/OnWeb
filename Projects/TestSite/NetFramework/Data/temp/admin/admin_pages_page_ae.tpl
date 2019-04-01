<script type='text/javascript'>
function customs_ids(id)
{
    $('a#photos_manage').click(function(){
        aj = new ajaxRequest();
        aj.load('/admin/madmin/@Module.UrlName/photo_manage/id='+id+'id_type=1','photos_manage_div');
        return false;
    });    
}

$(document).ready(function(){
    try {
    $("#block").hide();
    aj = new ajaxRequest();
    aj.userOnLoad = function(){endAnim($("#action_result"))};
    aj.load_form('form_ae',null,'action_result');
    stAnim();

    <{if $data.id == -1}>
    changeTitle('Добавление страницы');
    <{else}>
    changeTitle("Редактирование страницы: <{$data.name|java_string}>");
    <{/if}>
    
    var ckeditor = CKEDITOR.replace( 'text_block',{removePlugins: 'save'} );
    $('#save_func').click(function(){
        CKEDITOR.instances.text_block.updateElement();
    });

    CKFinder.setupCKEditor( ckeditor, '/ckfinder/' );
        
    $('#subs_clear').click(function(){
        $('#pages_subs option').removeAttr('selected');
        return false;
    });
    
    $('a#move_up').click(function(){
        try {
            var selected = $('#pages_subs option:selected').val();
            
            var arr = new Array();
            $('#pages_subs option').each(function(i,child){
                arr[arr.length] = {'text':$(child).text(),'value':$(child).val()};
            });

            for ( var i=0; i < arr.length; i++ )
            {
                if ( arr[i]['value'] == selected && i > 1 )
                {
                    var t = arr[i-1]['text'];
                    var v = arr[i-1]['value'];
                    
                    arr[i-1]['text'] = arr[i]['text'];
                    arr[i-1]['value'] = arr[i]['value'];
                    
                    arr[i]['text'] = t;
                    arr[i]['value'] = v;
                    
                    break;
                }
            }
            
            $('#pages_subs option').remove();
            for ( var i=0; i < arr.length; i++ )
            {
                var opt = new Option(arr[i]['text'],arr[i]['value']);
                $('#pages_subs')[0].options[$('#pages_subs')[0].options.length] = opt;
            }
            $('#pages_subs option[value='+selected+']').attr('selected','selected');
        } catch (err) { alert(err); }
        return false;       
    });
    
    $('a#move_down').click(function(){
        try {
            var selected = $('#pages_subs option:selected').val();
            
            var arr = new Array();
            $('#pages_subs option').each(function(i,child){
                arr[arr.length] = {'text':$(child).text(),'value':$(child).val()};
            });

            for ( var i=0; i < arr.length; i++ )
            {
                if ( arr[i]['value'] == selected && i < (arr.length-1) )
                {
                    var t = arr[i+1]['text'];
                    var v = arr[i+1]['value'];
                    
                    arr[i+1]['text'] = arr[i]['text'];
                    arr[i+1]['value'] = arr[i]['value'];
                    
                    arr[i]['text'] = t;
                    arr[i]['value'] = v;
                    
                    break;
                }
            }
            
            $('select#pages_subs option').remove();
            for ( var i=0; i < arr.length; i++ )
            {
                var opt = new Option(arr[i]['text'],arr[i]['value']);
                $('#pages_subs')[0].options[$('#pages_subs')[0].options.length] = opt;
            }
            $('#pages_subs option[value='+selected+']').attr('selected','selected');
        } catch (err) { alert(err); }
        return false;       
    });

    $("input[name='pages_name']").blur(function(){
        var str = $(this).val();
        if (str != '' && $("input[name='pages_urlname']").val() == ''){
            $.ajax({
                type    : "POST",
                url     : "/admin/madmin/@Module.UrlName/@Module.UrlName_add_urlname",
                data    : { name : str }
            }).done(function(res){
                $("input[name='pages_urlname']").val(res);
            }) 
        }
    })
    
    customs_ids(<{$data.id}>);
    } catch(err) { alert(err); }
});
</script>

<a href='' id='photos_manage'>Управление фотографиями</a><div id='photos_manage_div'></div>

<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/pages_add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/pages_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление страницы</h2><{else}><h2>Редактирование страницы</h2><{/if}>
  <div id="pages_div">
   <p>Название страницы:<br>
   <input type='text' name='pages_name' size='40' maxlength='200' value='<{$data.name}>'></p>
   <p>Urlname:<br>
   <input type='text' name='pages_urlname' size='40' maxlength='200' value='<{$data.urlname}>'><br /><small>Меняйте только если знаете, о чем идет речь</small></p>
   <textarea name='pages_body' rows='10' cols='10' id="text_block"><{$data.body}></textarea></p>
  </div>
<table width='900' id='table_results' class="admtable">    
 <tr>
  <td>Категория:</td>
  <td>
    <select name='pages_cat'>
     <option value='-2' <{if -2 == $data.category}>selected<{/if}>>Относится к подстраницам</option>
     <option value='-1' <{if -1 == $data.category}>selected<{/if}>>Корневая директория</option>
<{foreach from=$cats_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.category}>selected<{/if}>><{$ad}></option>
<{/foreach}>
    </select>
  </td>
 </tr>
 <tr>
  <td>Связанные подстраницы:</td>
  <td>
    <select id='pages_subs' name='pages_subs[]' multiple size=6>
<{foreach from=$subpages_data item=ad key=id}>
     <option value='<{$ad.id}>' <{if $ad.id|in_array:$data.subs_id}>selected<{/if}>><{$ad.name}></option>
<{/foreach}>
    </select><br />
    <a href='' id='subs_clear'>Очистить выделенные подстраницы</a><br>
    <a href='' id='move_up'>Поднять поле выше</a><br>
    <a href='' id='move_down'>Опустить поле ниже</a><br>
  </td>
 </tr>
 <tr style="display:none">
  <td>Язык:</td>
  <td>
    <select name='pages_lang'>
    
    <{foreach from=$langs_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.language}>selected<{/if}>><{$ad}></option>
    <{/foreach}>
    </select>
  </td>
 </tr>
 <tr>
  <td>SEO заголовок</td>
  <td><input type='text' name='pages_seo_title' size='65' maxlength='255' value='<{$data.seo_title}>' /></td>
 </tr>
 <tr>
  <td>SEO описание</td>
  <td><textarea name='pages_seo_descr' cols='65' rows='5'><{$data.seo_descr}></textarea></td>
 </tr>
 <tr>
  <td>SEO ключевики</td>
  <td><textarea name='pages_seo_kw' cols='65' rows='5'><{$data.seo_kw}></textarea></td>
 </tr>
 <tr>
  <td>Статус:</td>
  <td>
  <{if $data.id == -1}>
    <select name='pages_status'>
     <option value='0'>Отключено</option>
     <option value='1' selected>Доступно для просмотра</option>
    </select>
  <{else}>
    <select name='pages_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
   <{/if}>   
  </td>
 </tr>
</table>

<{if $data.id == -1}>
<button id='save_func'>Добавить</button> <img src="/data/img/design/loading.gif" alt="" id="loading_img">
<{else}>
<button id='save_func'>Сохранить</button> <img src="/data/img/design/loading.gif" alt="" id="loading_img">
<{/if}>
</form>