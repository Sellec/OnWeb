<script type='text/javascript'>
var Days = {'01':31,'02':28,'03':31,'04':30,'05':31,'06':30,'07':31,'08':31,'09':30,'10':31,'11':30,'12':31};

$(document).ready(function(){
    $("#block").hide();
    aj = new ajaxRequest();
    aj.load_form('form_ae',null,'video_result');
    
    <{if $data.id == -1}>
    changeTitle('Добавление видеофайла');
    <{else}>
    changeTitle("Редактирование видеофайла: <{$data.name|java_string}>");
    <{/if}>
    $("#loading_img, #video_result").hide();
    getResultAnim($("#form_ae"),$("#video_result"));
    
    $('select[name=video_hour] option').remove();
    $('select[name=video_minute] option').remove();
    var elem = $('select[name=video_hour]')[0];
    for ( var i=1; i <= 24; i++ )
    {
        elem.options[elem.options.length] = new Option(i,i);
    }
    var elem = $('select[name=video_minute]')[0];
    for ( var i=1; i <= 60; i++ )
    {
        elem.options[elem.options.length] = new Option(i,i);
    }
    $('select[name=video_hour]').val(<{$data.date|strftime:"%k"}>);
    $('select[name=video_minute]').val(<{$data.date|strftime:"%M"}>);

    $('select[name=video_month]').click(function(){
        var month = $('select[name=video_month] option[value='+$('select[name=video_month]').val()+']').val();
        
        $('select[name=video_day] option').remove();
        var elem = $('select[name=video_day]')[0];
        for ( var i=1; i <= Days[month]; i++ )
        {
            elem.options[elem.options.length] = new Option(i,i);
        }
    });
    $('select[name=video_month]').val("<{$data.date|strftime:"%m"}>");
    $('select[name=video_month]').click();
    $('select[name=video_day]').val(<{$data.date|strftime:"%e"}>);
    
    /*video_canload(<{$data.id}>);*/
    
    $('#upload1').hide();
});

</script>
<{include file="admin/admin_gallery_manage_catsgals.tpl"}>

<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/video_add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/video_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление видеофайла</h2><{else}><h2>Редактирование видеофайла</h2><{/if}>
<table width='900' id='table_results'>    
 <tr>
  <th width='200'>Поле</th>
  <th width='400'>Значение</th>  
 </tr>     
 <tr>
  <td>Название фотографии:</td>
  <td><input type='text' name='video_name' size='40' maxlength='200' value='<{$data.name}>'></td>
 </tr>
 <tr>
  <td>Описание:</td>
  <td><textarea name='video_text' rows='10' cols='60'><{$data.text}></textarea></td>
 </tr>
 <tr>
  <td>Галерея:</td>
  <td>
    <select name='video_gall'>
<{foreach from=$galls_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.gallery}>selected<{/if}>><{$ad.name}></option>
<{/foreach}>
    </select>
  </td>
 </tr>
 <tr>
  <td>Статус:</td>
  <td>
    <select name='video_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
  </td>
 </tr>
 <tr style="display:none;">
  <td>Дата добавления:</td>
  <td>
    (День/Месяц/Год) : <select name='video_day'></select> / 
    <select name='video_month'>
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
           <input type='text' name='video_year' size='4' maxlength='4' value='<{$data.date|strftime:"%Y"}>'><br>
    (Час:Минута) : <select name='video_hour'></select> : <select name='video_minute'></select><br>
  </td>
 </tr>
  <tr id="dis">
  <td>Файл:</td>
  <td>
    <img class='image' src='/data/video/<{$data.file.preview_file}>'><br>
    <a class='image' id="upload1" href="#">Загрузить новое изображение...</a>
    <span class='image' id="progress1"></span>
  </td>
 </tr>
</table>

<{if $data.id == -1}>
<input type='submit' value='Добавить' id="b_add"> <img src="/data/img/loading.gif" alt="" id="loading_img">

<{else}>
<input type='submit' value='Сохранить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{/if}>
</form>
<div id='video_result' style="padding:10px 5px;"></div>
<input type='submit' value='Добавить новый видеофайл' id="add_new" style="display:none;">