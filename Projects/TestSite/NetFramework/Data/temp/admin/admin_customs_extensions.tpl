<script type='text/javascript'>
try {
var v_customExtensionsActive = -1;
/*
Массив данных по возможным расширениям. key = { active : {0,1}, link : url }, где:
    key:        id ссылки a с атрибутом customExtensionsTable_link
    active:     не исп.
    link:       url-адрес, с которого необходимо подгружать html-код расширения. К link добавляется переменная v_customExtensionsID, задающая id текущего объекта, т.о. link должен оканчиваться на 'id='.
*/
var v_customExtensionsBidnings = {
<{if isset($_extensions_data) && is_array($_extensions_data)}>
<{foreach from=$_extensions_data item=ad key=id}>
    '<{$id}>'     :      {    
        'active'    :   0,
        'link'      :   '<{$ad.link}>'
    },
<{/foreach}>
<{/if}>
};

/*
Переключение между вкладками расширений. id это параметр id ссылки a с атрибутом class=customExtensionsTable_link.
*/
function f_customExtensionsSwitchMode(id)
{
    if ( v_customExtensionsID == -1 ) return ;
    if ( v_customExtensionsActive != -1 ) $('table#customExtensionsTable').find('tr#customExtensionsTable_row_'+v_customExtensionsActive).hide();
    v_customExtensionsActive = id;
    var div = $('table#customExtensionsTable').find('tr#customExtensionsTable_row_'+v_customExtensionsActive);

    try {
        if ( typeof(v_customExtensionsBidnings[id]) == 'undefined' )
        {
            f_customExtensionsDisableLink(id);
            div = 0;
        }
        else if ( v_customExtensionsBidnings[id]['active'] == 0 )
        {
            aj = new ajaxRequest();
            aj.load(v_customExtensionsBidnings[id]['link']+v_customExtensionsID,$('div#customExtensionsTable_row_'+v_customExtensionsActive+'_div')[0]);
            v_customExtensionsBidnings[id]['active'] = 0;
        }
    } catch (err) {div=0;alert(err);}
    if ( div != 0) div.show();
}

/*
Основная функция, подключающая расширения. id обозначает текущий объект. id=false означает что объект не выбран.
*/
function f_customExtensionsActivate2() 
{ 
    if ( v_customExtensionsID == -1 ) $('a.customExtensionsTable_link').attr('disable','disable');
    else $('a.customExtensionsTable_link').removeAttr('disable');
}

/*
Функция, выключающая ссылку, делающая её визуально неактивной.
*/
function f_customExtensionsDisableLink(id)
{
    $('a.customExtensionsTable_link').find('#'+id).attr('disable','disable');
}

/*
Задает список расширений, необходимых для использования. Например, list = new Array('photo').
*/
function f_customExtensionsNeedModules(list)
{
    try {
        $('table#customExtensionsTable tr#customExtensionsTable_rowhead').find('th').hide();
        for ( var i in list ) $('a#'+list[i]).parent().show();
    } catch(err) {};
}

/*
Указывает расширение, необходимое для использования. Например, list = 'photo'.
*/
function f_customExtensionsNeedModule(module)
{
    try {
        $('a#'+module).parent().show();
    } catch(err) {};
}

$(document).ready(function(){
    $('a.customExtensionsTable_link').click(function(){
        f_customExtensionsSwitchMode($(this).attr('id'));
    });
    
    $('tr#customExtensionsTable_rowhead').find('td').hide();
    $('a.customExtensionsTable_link').attr('disable','disable'); 
    
    <{if isset($_extensions_data) && is_array($_extensions_data)}>
    <{foreach from=$_extensions_data item=ad key=id}>
    f_customExtensionsNeedModule('<{$id}>');
    <{/foreach}>
    <{/if}>

    f_customExtensionsActivate2();
});
    
} catch(err) {alert(err);}
</script>
<div id='admextens_results'>
<table id='customExtensionsTable' width='800'>
 <tr>
  <th colspan='7'>Расширения</th>
 </tr>
 <tr>
  <td colspan='7'></td>
 </tr>
 <tr id='customExtensionsTable_rowhead'>
<{if isset($_extensions_data) && is_array($_extensions_data)}>
<{foreach from=$_extensions_data item=ad key=id}>
  <th><a class='customExtensionsTable_link' id='<{$id}>'><{$ad.name}></a></th>
<{/foreach}>
<{/if}>
 </tr>
<{if isset($_extensions_data) && is_array($_extensions_data)}>
<{foreach from=$_extensions_data item=ad key=id}>
 <tr id='customExtensionsTable_row_<{$id}>' style='display:none;'>
  <td colspan='7'><div id='customExtensionsTable_row_<{$id}>_div'></div></td>
 </tr>
<{/foreach}>
<{/if}>
</table>
</div>