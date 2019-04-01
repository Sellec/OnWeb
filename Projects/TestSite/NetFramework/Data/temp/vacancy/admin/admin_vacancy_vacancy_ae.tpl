<script type='text/javascript'>
<{if $data.id == -1}>
function item_add_result(res,id)
{
    try {
        if ( res == 1 )
        {
            mAddedID = id;
        }
    } catch(err) {alert(err);}
}
<{/if}>

var Days = {'01':31,'02':28,'03':31,'04':30,'05':31,'06':30,'07':31,'08':31,'09':30,'10':31,'11':30,'12':31};
$(document).ready(function(){
    $("#block").hide();
    aj = new ajaxRequest();
    aj.userOnLoad = function(){endAnim($("#action_result"))};
    aj.load_form('form_ae',null,'action_result');
    stAnim();
    
    var ckeditor = CKEDITOR.replace( 'vacancy_text',{removePlugins: 'save'} );
    $('#save_func').click(function(){
        CKEDITOR.instances.vacancy_text.updateElement();
    });

    CKFinder.setupCKEditor( ckeditor, '/ckfinder/' );
        
    <{if $data.id == -1}>
    changeTitle('Добавление записи');
    $('#calendar_apply').hide();
    <{else}>
    changeTitle("Редактирование записи: <{$data.name|java_string}>");
    <{/if}>

    $('select[name=vacancy_hour] option').remove();
    $('select[name=vacancy_minute] option').remove();
    var elem = $('select[name=vacancy_hour]')[0];
    for ( var i=1; i <= 24; i++ )
    {
        elem.options[elem.options.length] = new Option(i,i);
    }
    var elem = $('select[name=vacancy_minute]')[0];
    for ( var i=1; i <= 60; i++ )
    {
        elem.options[elem.options.length] = new Option(i,i);
    }
    $('select[name=vacancy_hour]').val(<{$data.date|strftime:"%k"}>);
    $('select[name=vacancy_minute]').val(<{$data.date|strftime:"%M"}>);


    $('select[name=vacancy_month]').click(function(){
        var month = $('select[name=vacancy_month] option[value='+$('select[name=vacancy_month]').val()+']').val();
        //alert(month);
        
        $('select[name=vacancy_day] option').remove();
        var elem = $('select[name=vacancy_day]')[0];
        for ( var i=1; i <= Days[month]; i++ )
        {
            elem.options[elem.options.length] = new Option(i,i);
        }
    });
    $('select[name=vacancy_month]').val("<{$data.date|strftime:"%m"}>");
    $('select[name=vacancy_month]').click();
    $('select[name=vacancy_day]').val(<{$data.date|strftime:"%e"}>);
});
</script>

<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/vacancy_add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/vacancy_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление записи</h2><{else}><h2>Редактирование записи</h2><{/if}>

<table width='900' id='table_results' class="admtable">    
 <tr>
  <th width='100'>Поле</th>
  <th width='400'>Значение</th>  
 </tr>     
 <tr>
  <td>Город:</td>
  <td>
    <select name='vacancy_cat'>
<{foreach from=$cats_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.category}>selected<{/if}>><{$ad}></option>
<{/foreach}>
    </select>
  </td>
 </tr>
 <tr>
  <td>Заголовок записи:</td>
  <td><input type='text' name='vacancy_name' size='40' maxlength='200' value='<{$data.name}>'></td>
 </tr>
 <tr>
  <td>Анонс записи:</td>
  <td><textarea name='vacancy_short_text' rows='6' cols='80' id="vacancy_short_text"><{$data.short_text}></textarea></td>
 </tr>
 <tr>
  <td>Текст записи:</td>
  <td><textarea name='vacancy_text' rows='10' cols='60' id="vacancy_text"><{$data.text}></textarea></td>
 </tr>
 <tr>
  <td>Статус:</td>
  <td>
  <{if $data.id == -1}>
    <select name='vacancy_status'>
     <option value='0'>Отключено</option>
     <option value='1' selected>Доступно для просмотра</option>
    </select>
  <{else}>
    <select name='vacancy_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
   <{/if}>  
  </td>
 </tr>
 <tr>
  <td>Дата добавления:</td>
  <td>
    (День/Месяц/Год) : <select name='vacancy_day'></select> / 
    <select name='vacancy_month'>
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
           <input type='text' name='vacancy_year' size='4' maxlength='4' value='<{$data.date|strftime:"%Y"}>'><br>
    (Час:Минута) : <select name='vacancy_hour'></select> : <select name='vacancy_minute'></select><br>
  </td>
 </tr>
</table>

<{if $data.id == -1}>
<button id='save_func'>Добавить</button> <img src="/data/img/design/loading.gif" alt="" id="loading_img">
<{else}>
<button id='save_func'>Сохранить</button> <img src="/data/img/design/loading.gif" alt="" id="loading_img">
<{/if}>
</form>
<div id='vacancy_result' style="padding:10px 5px;"></div>
