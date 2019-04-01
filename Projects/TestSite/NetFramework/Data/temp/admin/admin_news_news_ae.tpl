<script type='text/javascript'>
<{if $data.id == -1}>
function item_add_result(res,id)
{
    try {
        if ( res == 1 )
        {
            mAddedID = id;
            customs_ids(mAddedID);
        }
    } catch(err) {alert(err);}
}
<{/if}>

function customs_ids(id)
{
    $('a#photos_manage').click(function(){
        aj = new ajaxRequest();
        aj.load('/admin/madmin/@Module.UrlName/photo_manage/'+id+'/1','photos_manage_div');
        return false;
    });
}

var Days = {'01':31,'02':28,'03':31,'04':30,'05':31,'06':30,'07':31,'08':31,'09':30,'10':31,'11':30,'12':31};
$(document).ready(function(){
    $("#block").hide();
    aj = new ajaxRequest();
    aj.userOnLoad = function(){endAnim($("#action_result"))};
    aj.load_form('form_ae',null,'action_result');
    stAnim();
    
    var ckeditor = CKEDITOR.replace( 'news_text',{removePlugins: 'save'} );
    $('#save_func').click(function(){
        CKEDITOR.instances.news_text.updateElement();
    });

    CKFinder.setupCKEditor( ckeditor, '/ckfinder/' );
        
    <{if $data.id == -1}>
    changeTitle('Добавление новости');
    $('#calendar_apply').hide();
    <{else}>
    changeTitle("Редактирование новости: <{$data.name|java_string}>");
    <{/if}>

    $('select[name=news_hour] option').remove();
    $('select[name=news_minute] option').remove();
    var elem = $('select[name=news_hour]')[0];
    for ( var i=1; i <= 24; i++ )
    {
        elem.options[elem.options.length] = new Option(i,i);
    }
    var elem = $('select[name=news_minute]')[0];
    for ( var i=1; i <= 60; i++ )
    {
        elem.options[elem.options.length] = new Option(i,i);
    }
    $('select[name=news_hour]').val(<{$data.date|strftime:"%k"}>);
    $('select[name=news_minute]').val(<{$data.date|strftime:"%M"}>);


    $('select[name=news_month]').click(function(){
        var month = $('select[name=news_month] option[value='+$('select[name=news_month]').val()+']').val();
        //alert(month);
        
        $('select[name=news_day] option').remove();
        var elem = $('select[name=news_day]')[0];
        for ( var i=1; i <= Days[month]; i++ )
        {
            elem.options[elem.options.length] = new Option(i,i);
        }
    });
    $('select[name=news_month]').val("<{$data.date|strftime:"%m"}>");
    $('select[name=news_month]').click();
    $('select[name=news_day]').val(<{$data.date|strftime:"%e"}>);

    customs_ids(<{$data.id}>);
    
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
    
    $("#spanButtonDel").click(function(){
        $('#it_bimage').val('');
        $('#up_image').attr('src', "#");
        return false;
    })
});
</script>

<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/news_add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/news_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление новости</h2><{else}><h2>Редактирование новости</h2><{/if}>

<a href='' id='photos_manage'>Управление фотографиями</a><div id='photos_manage_div'></div>

<table width='900' id='table_results' class="admtable">    
 <tr>
  <th width='100'>Поле</th>
  <th width='400'>Значение</th>  
 </tr>     
 <tr>
  <td>Заголовок новости:</td>
  <td><input type='text' name='news_name' size='40' maxlength='200' value='<{$data.name}>'></td>
 </tr>
 <tr>
  <td>Анонс новости:</td>
  <td><textarea name='news_short_text' rows='6' cols='80' id="news_short_text"><{$data.short_text}></textarea></td>
 </tr>
 <tr>
  <td>Текст новости:</td>
  <td><textarea name='news_text' rows='10' cols='60' id="news_text"><{$data.text}></textarea></td>
 </tr>
 <tr>
  <td>Категория:</td>
  <td>
    <select name='news_cat'>
<{foreach from=$cats_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.category}>selected<{/if}>><{$ad}></option>
<{/foreach}>
    </select>
  </td>
 </tr>
 <tr>
  <td>Изображение:<br /><br />
  <small>Добавляется изображение 150x150 пикселей.</small>
  </td>
  <td>
    <input type='text' name='item_image' size='30' id="it_bimage" style="float:left;margin:0 10px 0 0;" value="<{$data.image}>">
    <span id="spanButtonPlaceHolder" style="float:left;margin:0 10px 5px 10px;vertical-align:top;"></span>
    <a href="" id="spanButtonDel" style="float:left;margin:0 10px 5px 10px;vertical-align:top;">удалить</a> <br><br>
    <img src='/<{$data.image}>' alt='' id='up_image' style='margin-bottom:10px;' />
  </td>                                         
 </tr>
 <tr>
  <td>Статус:</td>
  <td>
  <{if $data.id == -1}>
    <select name='news_status'>
     <option value='0'>Отключено</option>
     <option value='1' selected>Доступно для просмотра</option>
    </select>
  <{else}>
    <select name='news_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
   <{/if}>  
  </td>
 </tr>
 <tr>
  <td>Дата добавления:</td>
  <td>
    (День/Месяц/Год) : <select name='news_day'></select> / 
    <select name='news_month'>
            <option value='01'>Январь</option>
            <option value='02'>Февраль</option>
            <option value='03'>Март</option>
            <option value='04'>Апрель</option>
            <option value='05'>Май</option>
            <option value='06'>Июнь</option>
            <option value='07'>Июль</option>
            <option value='08'>Август</option>
            <option value='09'>Сентябрь</option>
            <option value='10'>Октябрь</option>
            <option value='11'>Ноябрь</option>
            <option value='12'>Декабрь</option>
           </select> /
           <input type='text' name='news_year' size='4' maxlength='4' value='<{$data.date|strftime:"%Y"}>'><br>
    (Час:Минута) : <select name='news_hour'></select> : <select name='news_minute'></select><br>
  </td>
 </tr>
</table>

<{if $data.id == -1}>
<button id='save_func'>Добавить</button> <img src="/data/img/design/loading.gif" alt="" id="loading_img">
<{else}>
<button id='save_func'>Сохранить</button> <img src="/data/img/design/loading.gif" alt="" id="loading_img">
<{/if}>
</form>
<div id='news_result' style="padding:10px 5px;"></div>
