<script type='text/javascript'>
var Days = {'01':31,'02':28,'03':31,'04':30,'05':31,'06':30,'07':31,'08':31,'09':30,'10':31,'11':30,'12':31};
$(document).ready(function(){
    $("#block").hide();
    aj = new ajaxRequest();
    aj.userOnLoad = function(){endAnim($("#action_result"))};
    aj.load_form('form_ae',null,'action_result');
    stAnim();
    
    <{if $data.id == -1}>
    changeTitle('Добавление галереи');
    <{else}>
    changeTitle("Редактирование галереи: <{$data.name|java_string}>");
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
});
</script>
<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/galls_add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/galls_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление галереи</h2><{else}><h2>Редактирование галереи</h2><{/if}>
<table width='900' id='table_results' class="admtable">    
 <tr>
  <th width='200'>Поле</th>
  <th width='400'>Значение</th>  
 </tr>     
 <tr>
  <td>Название галереи:</td>
  <td><input type='text' name='gal_name' size='40' maxlength='200' value='<{$data.name}>'></td>
 </tr>
 <tr>
  <td>Описание галереи:</td>
  <td><textarea name='gal_descr' rows='5' cols='50'><{$data.description}></textarea></td>
 </tr>
 <tr>
  <td>Родительская категории:</td>
  <td>
    <select name='gal_sub'>
<{foreach from=$cats_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.sub_id}>selected<{/if}>><{$ad}></option>
<{/foreach}>
    </select>
  </td>
 </tr>
 <tr style="display:none;">
  <td>Тип галереи:</td>
  <td>
    <input type='radio' name='gal_type' value='1' <{if $data.type==1}>checked<{/if}>>&nbsp;Фотогалерея<br>
    <!--<input type='radio' name='gal_type' value='2' <{if $data.type==2}>checked<{/if}>>&nbsp;Видеогалерея<br>-->
  </td>
 </tr>
 <tr>
  <td>Статус:</td>
  <td>
  <{if $data.id == -1}>
    <select name='gal_status'>
     <option value='0'>Отключено</option>
     <option value='1' selected>Доступно для просмотра</option>
    </select>
  <{else}>
    <select name='gal_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
   <{/if}>  
  </td>
 </tr>
<tr>
  <td>Дата добавления:</td>
  <td>
    <select name='news_day'></select> / 
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
    <div style="display:none">(Час:Минута) : <select name='news_hour'></select> : <select name='news_minute'></select></div>
  </td>
 </tr> 
</table>

<{if $data.id == -1}>
<input type='submit' value='Добавить'> <img src="/data/img/loading.gif" alt="" id="loading_img"> <div id='gal_result'></div>
<{else}>
<input type='submit' value='Сохранить'> <img src="/data/img/loading.gif" alt="" id="loading_img"> <div id='gal_result'></div>
<{/if}>
</form>
