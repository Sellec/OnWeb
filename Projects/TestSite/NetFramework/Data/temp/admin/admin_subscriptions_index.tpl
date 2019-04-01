<script type='text/javascript'>
var Items = <{$items|jsobject}>;
var Item = null;
var Roles = <{$roles|jsobject}>;

function updateItems()
{
    try 
    {
        $('#itemsTable').tablefill(Items, function(tr_elem, data){
            $(tr_elem).find('td').each(function(ichild){
                if (ichild == 0) $(this).text(data.name);
            });

            $(tr_elem).find(".delete").click(function(){ itemActions(data, 'delete') });
            $(tr_elem).find(".edit").click(function(){ itemActions(data, 'edit') });
        });

    } catch(err) { alert(err); }
}

function itemSetField(key, value)
{
    $("table#itemTable [name='" + key + "']").each(function(e,e1){
        switch ($(this).prop('type'))
        {
            case "hidden":
            case "text":
            case "select-one":
                $(this).val(value);
                break;
                
            case "checkbox":
                var b = value == "0" || value == "false";
                $(this).attr('checked', !b);
                break;
            
            default:
                alert($(this) + ': ' + $(this).prop('type'));
        }
    });
}

function itemActions(data, action)
{
    try 
    {  
        $('#itemsTableResult').text('');
        
        if (action == 'add' || action == 'edit')
        {
            Item = data;
            
            for (var i in data) itemSetField(i, data[i]);
            
            $("td.itemTable th#itemCaption").text(action == 'add' ? "Добавление" : "Редактирование");
            $("td.itemTable").show();
            
            itemActions(data, 'selectSubscription');
        }
        else if (action == 'save')
        {
            $.requestJSON('/admin/madmin/@Module.UrlName/subscriptionSave', $('form#itemForm').serializeArray(), function(result, message, data2){
                if (result == JsonResult.OK)
                {
                    $("td.itemTable").hide();

                    Items[data2.Key] = data2.Value;
                    updateItems();
                }
                $("td.itemTable .Result").html(message);
            });
        }
        else if (action == 'delete')
        {
            if (confirm('Вы действительно хотите удалить "' + data.name + '"?'))
            {
                $.requestJSON('/admin/madmin/@Module.UrlName/subscriptionDelete/' + data.id, null, function(result, message){
                    if (result == JsonResult.OK) 
                    {
                        var obj = Object.prototype.toString.call(data[i]) === "[object Array]" ? new Array() : {};
                        for (var i in Items) 
                            if (data != Items[i])
                                obj[Object.prototype.toString.call(data[i]) === "[object Array]" ? obj.Length : i] = Items[i];
                        
                        updateItems();
                    }
                    if (message != null && message.length > 0) $("#itemsTableResult").html(message);
                });
            }
        }
        else if (action == 'save2')
        {
            $.requestJSON('/admin/madmin/@Module.UrlName/subscriptionSave2/' + data.id, $('form#itemForm2').serializeArray(), function(result, message, data2){
                if (result == JsonResult.OK)
                {
                    $("td.itemTable").hide();

                    //Items[data2.Key] = data2.Value;
                    updateItems();
                }
                $("td.itemTable .Result").html(message);
            });
        }
        else if (action == 'selectSubscription')
        {
            $("select[name='subscribers[]'] option, select[name='roles[]'] option").remove();
            if (data != undefined && data != null)
            {
                $.each(data.Emails, function(key, value){ $("select[name='subscribers[]']").append($("<option></option>").val(value.email).text(value.email).attr('selected', true)) });
                $.each(Roles, function(key, value){ $("select[name='roles[]']").append($("<option></option>").val(key).text(value.NameRole)) });
                $.each(data.Roles, function(key, value){ $("select[name='roles[]'] option[value='" + key + "']").attr('selected', true) });
            }
        }
    } 
    catch(err) { $("#itemsTableResult").html(err); }
}

$(document).ready(function(){
    changeTitle('Листы рассылки');
    $("#block").hide();
    
    $("input.ButtonClose").click(function(){ 
        $(this).parent().parent().closest('td').hide(); 
    }).click();
    
    $("input#itemAdd").click(function(){ 
        itemActions(null, 'add');
    });
    
    $('form#itemForm').submit(function(e){
        itemActions(Item, 'save');
        e.preventDefault();
    });
    
    $('form#itemForm2').submit(function(e){
        itemActions(Item, 'save2');
        e.preventDefault();
    });
    
    updateItems();
});
</script>
<h2>Листы рассылки</h2>

<table>
 <tr>
  <td id='itemsTable' style='vertical-align:top;'>
  
    <table id='itemsTable' style="width:450px;" class='tablesorter'><thead>
     <tr>
      <th colspan='2'>Листы рассылки | <input type='button' id='itemAdd' value='Добавить'></th>
     </tr>
     <tr>
      <th>Название</th>
      <th style="width:200px">Действия</th>
     </tr>
     </thead><tbody>
     <tr id='obraz' style='display:none;background-color:#efefef;'>
      <td></td>
      <td>
       <input type='button' class='edit' value='Редактировать'><br>
       <input type='button' class='delete' value='Удалить'>
      </td>
     </tr>
     </tbody>
    </table>
    <div id='itemsTableResult'></div>

  </td>
  <td class='itemTable' style='padding-left:20px;vertical-align:top;'>
  
    <form method='post' id='itemForm'><input type='hidden' name='id'>
    <table id='itemTable' style="width:400px;" class='tablesorter'><thead>
     <tr>
      <th colspan='2' id="itemCaption"></th>
     </tr>
     <tr>
      <th width="150">Параметр</th>
      <th>Значение</th>
     </tr>
     </thead>
     <tr>
      <td>Название:</td>
      <td><input type='text' name='name'></td>
     </tr>
     <tr>
      <td>Разрешить самостоятельное подписывание:</td>
      <td><input type='checkbox' name='AllowSubscribe'></td>
     </tr>
     <tr>
      <td>Состояние:</td>
      <td>
       <select name='status'>
        <option value='0'>Отключено</option>
        <option value='1'>Включено</option>
       </select>
      </td>
     </tr>
     <tr>
      <td colspan='2'>
       <input type='submit' value='Сохранить'>
       <input type='button' value='Отменить' class='ButtonClose'>
      </td>
     </tr>
    </table>
    </form>
    <div class='Result'></div>

  </td>
  <td class='itemTable' style='padding-left:20px;vertical-align:top;'>
  
    <form method='post' id='itemForm2'>
    <table style="width:400px;" class='tablesorter'><thead>
     <tr>
      <th width="150">Подписчики</th>
      <th>Связанные роли</th>
     </tr>
     </thead>
     <tr>
      <td>
       <select name='subscribers[]' size='10' multiple></select>
      </td>
      <td>
       <select name='roles[]' size='10' multiple></select>
      </td>
     </tr>
     <tr>
      <td colspan='2'>
       <input type='submit' value='Сохранить'>
       <input type='button' value='Отменить' class='ButtonClose'>
      </td>
     </tr>
    </table>
    </form>
    <div class='Result'></div>

  </td>

 </tr>
</table>
