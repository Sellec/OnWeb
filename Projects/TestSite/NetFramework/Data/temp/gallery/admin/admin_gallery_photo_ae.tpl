<script type='text/javascript'>
var Days = {'01':31,'02':28,'03':31,'04':30,'05':31,'06':30,'07':31,'08':31,'09':30,'10':31,'11':30,'12':31};

$(document).ready(function(){
    $("#block").hide();
    aj = new ajaxRequest();
    aj.userOnLoad = function(){endAnim($("#action_result"))};
    aj.load_form('form_ae',null,'photo_result');
    stAnim();
    
    <{if $data.id == -1}>
    changeTitle('Добавление фотографии');
    <{else}>
    changeTitle("Редактирование фотографии: <{$data.name|java_string}>");
    <{/if}>

    $('select[name=photo_hour] option').remove();
    $('select[name=photo_minute] option').remove();
    var elem = $('select[name=photo_hour]')[0];
    for ( var i=1; i <= 24; i++ )
    {
        elem.options[elem.options.length] = new Option(i,i);
    }
    var elem = $('select[name=photo_minute]')[0];
    for ( var i=1; i <= 60; i++ )
    {
        elem.options[elem.options.length] = new Option(i,i);
    }
    $('select[name=photo_hour]').val(<{$data.date|strftime:"%k"}>);
    $('select[name=photo_minute]').val(<{$data.date|strftime:"%M"}>);


    $('select[name=photo_month]').click(function(){
        var month = $('select[name=photo_month] option[value='+$('select[name=photo_month]').val()+']').val();
        //alert(month);
        
        $('select[name=photo_day] option').remove();
        var elem = $('select[name=photo_day]')[0];
        for ( var i=1; i <= Days[month]; i++ )
        {
            elem.options[elem.options.length] = new Option(i,i);
        }
    });
    $('select[name=photo_month]').val("<{$data.date|strftime:"%m"}>");
    $('select[name=photo_month]').click();
    $('select[name=photo_day]').val(<{$data.date|strftime:"%e"}>);
});

</script>
<{include file="admin/admin_gallery_manage_catsgals.tpl"}>

<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/photo_add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/photo_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление фотографии</h2><{else}><h2>Редактирование фотографии</h2><{/if}>
<table width='900' id='table_results'>    
 <tr>
  <th width='200'>Поле</th>
  <th width='400'>Значение</th>  
 </tr>     
 <tr>
  <td>Название фотографии:</td>
  <td><input type='text' name='photo_name' size='40' maxlength='200' value='<{$data.name}>'></td>
 </tr>
 <tr style="display:none;">
  <td>Описание:</td>
  <td><textarea name='photo_text' rows='10' cols='60'><{$data.text}></textarea></td>
 </tr>
 <tr>
  <td>Галерея:</td>
  <td>
    <select name='photo_gall'>
<{foreach from=$galls_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.gallery}>selected<{/if}>><{$ad.name}></option>
<{/foreach}>
    </select>
  </td>
 </tr>
 <tr>
  <td>Статус:</td>
  <td>
    <select name='photo_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
  </td>
 </tr>
 <tr style="display:none;">
  <td>Дата добавления:</td>
  <td>
    (День/Месяц/Год) : <select name='photo_day'></select> / 
    <select name='photo_month'>
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
           <input type='text' name='photo_year' size='4' maxlength='4' value='<{$data.date|strftime:"%Y"}>'><br>
    (Час:Минута) : <select name='photo_hour'></select> : <select name='photo_minute'></select><br>
  </td>
 </tr>
  <tr>
  <td>Изображение:</td>
  <td>
    <img class='image' src='/data/photo/<{$data.photo.preview_file}>'><br>
  </td>
 </tr>
</table>

<{if $data.id == -1}>
<input type='submit' value='Добавить' id="b_add"> <img src="/data/img/loading.gif" alt="" id="loading_img">

<{else}>
<input type='submit' value='Сохранить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{/if}>
</form>
<div id='photo_result' style="padding:10px 5px;"></div>