<script type='text/javascript'>
var Lists = <{$lists}>;
var ListData = new Array();
var ListScheme = new Array();
var ListDataID = 0;

function showResult(text, id)
{
    var div_res = $("#" + id);
    if (text != undefined && text.length > 0)
    {
        div_res.html(text);
        div_res.fadeIn("slow");
    } else div_res.hide();
}

function editMode(mode, data)
{
    showResult('', 'lists_edit_result');
    if (mode == 'add')
    {
        $('#addbutton').val('Добавить');
        $('#list_edit').show();
    } else if (mode == 'edit')
    {
        $('#addbutton').val('Сохранить');
        $('#list_edit').show();
    } else $('#list_edit').hide();
}

function updateLists()
{
    $('select[name=ValueList1] option, select[name=ValueList2] option').remove();
    $('select[name=ValueList1], select[name=ValueList2]').append($("<option></option>").attr("value", 0).text('Нет')); 
    for(var t in Lists) $('select[name=ValueList1], select[name=ValueList2]').append($("<option></option>").attr("value",t).text(Lists[t]['ListName'])); 
        

    $('#custom_lists').tablefill(Lists, function(tr_elem,data){
        $(tr_elem).find('td.class').text(data['IdList']);
        $(tr_elem).find('td.name').text(data['ListName']);

        $(tr_elem).find('a.delete').click(function(){
            try{
                if (confirm("Вы уверены, что хотите удалить список '" + data['ListName'] + "'?"))
                {
                    showResult('', 'lists_result');
                    $.requestJSON("/admin/madmin/@Module.UrlName/custom_lists_delete/"+data['IdList'], null, function(result, message)
                    {
                        if (result == JsonResult.OK) 
                        {
                            $(tr_elem).remove();
                            var a = new Array();
                            for (var i in Lists) if (i != id) a[i] = Lists[i];
                            Lists = a;
                        }
                        if (message.length > 0) showResult(message, 'lists_result');
                    });
                }
            } catch(err) { showResult("Не получилось удалить список.\r\n" + err, 'lists_result'); }
            return false;
        });
        
        /*$(tr_elem).find('a.edit').click(function(){
            try{
                if ($('input[name=ListName]').val().length == 0 ) showResult('Название списка не должно быть пустым.', 'lists_edit_result');
                if ($('input[name=ViewScheme]').val().length == 0 ) showResult('Схема списка не должна быть пустой.', 'lists_edit_result');
                else {
                    showResult('', 'lists_edit_result');
                    
                    var _data = {
                        'ListName'      :   $('input[name=ListName]').val(),
                        'ViewScheme'    :   $('input[name=ViewScheme]').val(),
                        'ValueStr1'     :   $('input[name=ValueStr1]').val(),
                        'ValueStr2'     :   $('input[name=ValueStr2]').val(),
                        'ValueList1'    :   $('select[name=ValueList1]').val(),
                        'ValueList2'    :   $('select[name=ValueList2]').val(),
                        'ValueBool1'    :   $('input[name=ValueBool1]').val(),
                        'ValueBool2'    :   $('input[name=ValueBool2]').val(),
                        'ValueBool3'    :   $('input[name=ValueBool3]').val(),
                    };
                    
                    $.requestJSON("/admin/madmin/@Module.UrlName/custom_lists_edit/" + data['IdList'], _data, function(result, message, data)
                    {
                        if (message != undefined && message.length > 0) showResult(message, 'lists_edit_result');
                    });
                }
            } catch(err) { showResult("Не получилось отредактировать список.\r\n" + err, 'lists_edit_result'); }
            return false;
        });*/
        
        $(tr_elem).find('a.edit_data').click(function(){
            try{
                showResult('', 'lists_data_result');
                $.requestJSON("/admin/madmin/@Module.UrlName/custom_list_data_edit/"+data['IdList'], null, function(result, message, responseData){
                    if (message.length > 0) showResult(message, 'lists_data_result');
                    if (result == JsonResult.OK) updateListData(data['IdList'], responseData['data'], responseData['scheme']);
                });
            } catch(err) { showResult("Не получилось удалить список.\r\n" + err, 'lists_data_result'); }
            return false;
        });
        
        $(tr_elem).find('a.pages_view').attr('href',$(tr_elem).find('a.pages_view').attr('href')+data['id']);
        $(tr_elem).find('a.pages_edit').attr('href',$(tr_elem).find('a.pages_edit').attr('href')+data['id']);
        $(tr_elem).find('a.v_view').attr('href',$(tr_elem).find('a.v_view').attr('href')+data['id']);

    });
    
}

function updateListData(id, data, scheme)
{
    ListDataID = id;
    ListData = data;
    ListScheme = scheme;
    
    $('#custom_lists_data').show();
    $('#custom_lists_data tr.trAdded').remove();

    $('#custom_lists_data').find('th, td').hide();
    $('#custom_lists_data').find('th.Controls, td.Controls').show();
    
    for (var t in ListScheme) 
    {
        $('#custom_lists_data').find('th.' + t).text(ListScheme[t]).show();
        $('#custom_lists_data').find('td.' + t).show();
    }
    
    if (ListScheme['ValueList1Data'] != undefined) 
        for(var t in ListScheme['ValueList1Data']) $('#custom_lists_data select.cld_ValueList1').append($("<option></option>").attr("value",t).text(ListScheme['ValueList1Data'][t])); 
    if (ListScheme['ValueList2Data'] != undefined) 
        for(var t in ListScheme['ValueList2Data']) $('#custom_lists_data select.cld_ValueList2').append($("<option></option>").attr("value",t).text(ListScheme['ValueList2Data'][t])); 
 
    
    $('#custom_lists_data').tablefill(ListData, function(tr_elem,data){
        $(tr_elem).find('td, th').hide();
        $(tr_elem).find('td.Controls, th.Controls').show();
        $(tr_elem).addClass('trAdded');
        
        for (var t in ListScheme) 
        {
            if (data[t] != undefined)
            {
                $(tr_elem).find('td.' + t + ', th.' + t).show();
                $(tr_elem).find('.cld_' + t).val(data[t]);
            }
        }
        
        $(tr_elem).find('.cld_ValueBool1').attr('checked', $(tr_elem).find('.cld_ValueBool1').val() > 0);
        $(tr_elem).find('.cld_ValueBool2').attr('checked', $(tr_elem).find('.cld_ValueBool2').val() > 0);
        $(tr_elem).find('.cld_ValueBool3').attr('checked', $(tr_elem).find('.cld_ValueBool3').val() > 0);
        
        if (ListScheme['ValueList1Data'] != undefined) 
            for(var t in ListScheme['ValueList1Data']) $(tr_elem).find('select.cld_ValueList1').append($("<option></option>").attr("value",t).text(ListScheme['ValueList1Data'][t])); 
        if (ListScheme['ValueList2Data'] != undefined) 
            for(var t in ListScheme['ValueList2Data']) $(tr_elem).find('select.cld_ValueList2').append($("<option></option>").attr("value",t).text(ListScheme['ValueList2Data'][t])); 
        
        $(tr_elem).find('.delete').click(function(){
            try{
                showResult('', 'lists_data_result');
                $.requestJSON("/admin/madmin/@Module.UrlName/custom_list_data_delete/"+data['IdData'], function(result, message)
                {
                    if (result == JsonResult.OK) 
                    {
                        $(tr_elem).remove();
                        var a = new Array();
                        for (var i in ListData) if (i != id) a[i] = ListData[i];
                        ListData = a;
                    }
                    if (message.length > 0) showResult(message, 'lists_data_result');
                });
            } catch(err) { showResult("Не получилось удалить элемент списка.\r\n" + err, 'lists_data_result'); }
            return false;
        });
        
        $(tr_elem).find('.edit').click(function(elem){
            try{
                showResult('', 'lists_data_result');
                
                var _data = {
                    'ValueStr1' : $(this).parent().parent().find('input.cld_ValueStr1').val(),
                    'ValueStr2' : $(this).parent().parent().find('input.cld_ValueStr2').val(),
                    'ValueList1' : $(this).parent().parent().find('select.cld_ValueList1').val(),
                    'ValueList2' : $(this).parent().parent().find('select.cld_ValueList2').val(),
                    'ValueBool1' : $(this).parent().parent().find('input.cld_ValueBool1').attr('checked')?1:0,
                    'ValueBool2' : $(this).parent().parent().find('input.cld_ValueBool2').attr('checked')?1:0,
                    'ValueBool3' : $(this).parent().parent().find('input.cld_ValueBool3').attr('checked')?1:0,
                };
                
                $.requestJSON("/admin/madmin/@Module.UrlName/custom_list_data_editsave/" + data['IdData'], _data, function(success, message)
                {
                    showResult(message, 'lists_data_result');
                });
            } catch(err) { showResult("Не получилось отредактировать элемент списка.\r\n" + err, 'lists_data_result'); }
            return false;
        });
        
    });
    
}

$(document).ready(function() {
    try {
        $("#block").hide();
        changeTitle('Просмотр настраиваемых списков');
        
        editMode();
        
        updateLists();

        $('#add').click(function(){editMode('add');return false;});
        $('#closebutton').click(function(){editMode();return false;});
        
        $('#addbutton').click(function(){
            try{
                if ($('input[name=ListName]').val().length == 0 ) showResult('Название списка не должно быть пустым.', 'lists_edit_result');
                if ($('input[name=ViewScheme]').val().length == 0 ) showResult('Схма списка не должна быть пустой.', 'lists_edit_result');
                else {
                    showResult('', 'lists_edit_result');
                    
                    var data = {
                        'ListName'      :   $('input[name=ListName]').val(),
                        'ViewScheme'    :   $('input[name=ViewScheme]').val(),
                        'ValueStr1'     :   $('input[name=ValueStr1]').val(),
                        'ValueStr2'     :   $('input[name=ValueStr2]').val(),
                        'ValueList1'    :   $('select[name=ValueList1]').val(),
                        'ValueList2'    :   $('select[name=ValueList2]').val(),
                        'ValueBool1'    :   $('input[name=ValueBool1]').val(),
                        'ValueBool2'    :   $('input[name=ValueBool2]').val(),
                        'ValueBool3'    :   $('input[name=ValueBool3]').val(),
                    };
                    
                    $.requestJSON("/admin/madmin/@Module.UrlName/custom_lists_add", data, function(result, message, id)
                    {
                        if (result == JsonResult.OK) 
                        {
                            Lists[id] = data;
                            Lists[id]['id'] = Lists[id]['IdList'] = id;
                            
                            updateLists();
                        }

                        if (message.length > 0) showResult(message, 'lists_edit_result');
                    })
                }
            } catch(err) { showResult("Не получилось добавить список.\r\n" + err, 'lists_edit_result'); }
            return false;
        });
        
        $('#list_data .add').click(function(elem){
            try{
                showResult('', 'lists_data_result');
                
                var data = {
                    'ValueStr1' : $(this).parent().parent().find('input.cld_ValueStr1').val(),
                    'ValueStr2' : $(this).parent().parent().find('input.cld_ValueStr2').val(),
                    'ValueList1' : $(this).parent().parent().find('select.cld_ValueList1').val(),
                    'ValueList2' : $(this).parent().parent().find('select.cld_ValueList2').val(),
                    'ValueBool1' : $(this).parent().parent().find('input.cld_ValueBool1').attr('checked')?1:0,
                    'ValueBool2' : $(this).parent().parent().find('input.cld_ValueBool2').attr('checked')?1:0,
                    'ValueBool3' : $(this).parent().parent().find('input.cld_ValueBool3').attr('checked')?1:0,
                };
                
                $.requestJSON("/admin/madmin/@Module.UrlName/custom_list_data_add/" + ListDataID, data, function(result, message, id)
                {
                    if (result == JsonResult.OK)
                    {
                        ListData[id] = { 
                            'IdData' : id,
                            'IdList' : ListDataID,
                        };
                        for(var t in data) ListData[id][t] = data[t];
                        updateListData(ListDataID, ListData, ListScheme);
                    } 

                    if (message.length > 0) showResult(message, 'lists_data_result');
                });
            } catch(err) { showResult("Не получилось добавить элемент в список.\r\n" + err, 'lists_data_result'); }
            return false;
        });
        

    } catch(err) { alert(err); }
});
</script>

<h2>Просмотр настраиваемых списков | <a href='' id='add'>Добавить</a></h2>

<table width='100%'>
 <tr>
  <td style='width:400px;' valign='top'>
  
   <div id='lists_result'></div>
   <table id='custom_lists' class='tablesorter' style='width:400px'><thead>
    <tr>
     <th style="width:15px">№</th>
     <th>Название</th>
     <th style="width:100px">Действия</th>
    </tr>
    </thead><tbody></tbody>
    <tr id='notfounded'><td colspan='4'>Ничего не найдено</td></tr>
    <tr id='obraz' style='display:none;'>
     <td class="center"></td>
     <td class='name'></td>
     <td>
      <a href='' class='edit_data'>содержимое</a><br>
      <a href='' class='edit'>редактировать</a><br>
      <a href='' class="delete">удалить</a><br>
     </td>
    </tr>  
   </table>
   
  </td>
  <td id='list_edit' valign='top'>
  
   <table id='custom_lists_edit' class='tablesorter' style='width:500px;'>
    <tr>
     <th style="width:150px">Поле</th>
     <th>Значение</th>
    </tr>
    <tr>
     <td>Название</td>
     <td><input type='text' size='30' name='ListName' value=''></td>
    </tr>  
    <tr>
     <td>Схема названия (например, ValueStr1 ValueStr2)</td>
     <td><input type='text' size='30' name='ViewScheme' value=''></td>
    </tr>  
    <tr>
     <td>Название столбца ValueStr1</td>
     <td><input type='text' size='30' name='ValueStr1' value=''></td>
    </tr>  
    <tr>
     <td>Название столбца ValueStr2</td>
     <td><input type='text' size='30' name='ValueStr2' value=''></td>
    </tr>  
    <tr>
     <td>Название столбца ValueBool1</td>
     <td><input type='text' size='30' name='ValueBool1' value=''></td>
    </tr>  
    <tr>
     <td>Название столбца ValueBool2</td>
     <td><input type='text' size='30' name='ValueBool2' value=''></td>
    </tr>  
    <tr>
     <td>Название столбца ValueBool3</td>
     <td><input type='text' size='30' name='ValueBool3' value=''></td>
    </tr>  
    <tr>
     <td>Список, привязанный к столбцу ValueList1</td>
     <td><select name='ValueList1'><option value='0'>Нет</option></select></td>
    </tr>  
    <tr>
     <td>Список, привязанный к столбцу ValueList2</td>
     <td><select name='ValueList2'><option value='0'>Нет</option></select></td>
    </tr>  
    <tr>
     <td colspan='2'>
      <input type='button' id='addbutton' value='Добавить'>&nbsp;
      <input type='button' id='closebutton' value='Отменить'>
     </td>
    </tr>
   </table>
   <b>Столбцы, для которых не задано название, НЕ будут использоваться и отображаться в настройках и схеме.</b>
   <div id='lists_edit_result'></div>
  
  </td>
 </tr>
 <tr>
  <td colspan='2' id='list_data' valign='top'>
  
   <div id='lists_data_result'></div>
   <table id='custom_lists_data' class='tablesorter' style='display:none;'><thead>
    <tr>
     <th style="width:100px" class='ValueStr1'></th>
     <th style="width:100px" class='ValueStr2'></th>
     <th style="width:100px" class='ValueList1'></th>
     <th style="width:100px" class='ValueList2'></th>
     <th style="width:70px" class='ValueBool1'></th>
     <th style="width:70px" class='ValueBool2'></th>
     <th style="width:70px" class='ValueBool3'></th>
     <th style="width:60px" class='Controls'>Действия</th>
    </tr>
    </thead>
    <tr id='notfounded'><td colspan='4'>Ничего не найдено</td></tr>
    <tr id='obraz' style='display:none;'>
     <td class='ValueStr1'><input type='text' class='cld_ValueStr1'></td>
     <td class='ValueStr2'><input type='text' class='cld_ValueStr2'></td>
     <td class='ValueList1'><select class='cld_ValueList1'></select></td>
     <td class='ValueList2'><select class='cld_ValueList2'></select></td>
     <td class='ValueBool1'><input type='checkbox' class='cld_ValueBool1'></td>
     <td class='ValueBool2'><input type='checkbox' class='cld_ValueBool2'></td>
     <td class='ValueBool3'><input type='checkbox' class='cld_ValueBool3'></td>
     <td class='Controls'>
      <input type='button' class="edit" value='Править'>&nbsp;
      <input type='button' class="delete" value='Удалить'>
      </td>
    </tr>
    <tr>
     <td style='background-color:#c0c0c0;' class='ValueStr1'><input type='text' class='cld_ValueStr1'></td>
     <td style='background-color:#c0c0c0;' class='ValueStr2'><input type='text' class='cld_ValueStr2'></td>
     <td style='background-color:#c0c0c0;' class='ValueList1'><select class='cld_ValueList1'></select></td>
     <td style='background-color:#c0c0c0;' class='ValueList2'><select class='cld_ValueList2'></select></td>
     <td style='background-color:#c0c0c0;' class='ValueBool1'><input type='checkbox' class='cld_ValueBool1'></td>
     <td style='background-color:#c0c0c0;' class='ValueBool2'><input type='checkbox' class='cld_ValueBool2'></td>
     <td style='background-color:#c0c0c0;' class='ValueBool3'><input type='checkbox' class='cld_ValueBool3'></td>
     <td style='background-color:#c0c0c0;' class='Controls'><input type='button' class="add" value='Добавить'></td>
    </tr>
   </table>
   <input type='button' id='data_save' value='Сохранить'>
  
  </td>
 </tr>
</table>

