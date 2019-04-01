<{include file="admin/admin_custom_extensions.tpl"}>
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

var Days = {'01':31,'02':28,'03':31,'04':30,'05':31,'06':30,'07':31,'08':31,'09':30,'10':31,'11':30,'12':31};

function customs_ids(id)
{
    f_customExtensionsActivate(id);
    $('a#photos_manage').click(function(){
        aj = new ajaxRequest();
        aj.load('/admin/madmin/@Module.UrlName/photo_manage/'+id+'/1','photos_manage_div');
        return false;
    });
}

$(document).ready(function(){
    $("#block").hide();
    aj = new ajaxRequest();
    aj.userOnLoad = function(){endAnim($("#action_result"))};
    aj.load_form('form_ae',null,'action_result');
    stAnim();
    
    var ckeditor = CKEDITOR.replace( 'text_block',{removePlugins: 'save'} );
    $('#save_func').click(function(){
        CKEDITOR.instances.text_block.updateElement();
    });

    CKFinder.setupCKEditor( ckeditor, '/ckfinder/' );
    
    <{if $data.id == -1}>
    changeTitle('Добавление статьи');
    <{else}>
    changeTitle("Редактирование статьи: <{$data.name|java_string}>");
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
    //f_customExtensionsNeedModules(new Array('photo','map'));
});
</script>
<input type="hidden" value="110,80" id="auto_resize" />
<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/articles_add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/articles_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление статьи</h2><{else}><h2>Редактирование статьи</h2><{/if}>

<table width='900' id='table_results' class="admtable">    
 <tr>
  <th width='100'>Поле</th>
  <th width='400'>Значение</th>  
 </tr>     
 <tr>
  <td>Заголовок статьи:</td>
  <td><input type='text' name='articles_name' size='40' maxlength='200' value='<{$data.name}>'></td>
 </tr>
   <tr>
  <td>Текст статьи:</td>
  <td><textarea name='articles_text' rows='5' cols='50' id="text_block"><{$data.text}></textarea></td>
 </tr>
 <tr>
  <td>Категория:</td>
  <td>
    <select name='articles_cat'>
<{foreach from=$cats_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.category}>selected<{/if}>><{$ad}></option>
<{/foreach}>
    </select>
  </td>
 </tr>
 <tr>
  <td>Статус:</td>
  <td>
    <select name='articles_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
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
<button id='save_func'>Добавить</button> <img src="/data/img/loading.gif" alt="" id="loading_img">
<{else}>
<button id='save_func'>Сохранить</button>  <img src="/data/img/loading.gif" alt="" id="loading_img">
<{/if}>
</form>
