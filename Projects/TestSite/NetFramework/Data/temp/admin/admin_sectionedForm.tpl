<script type='text/javascript'>
var ItemsFormSettings = null;

function registerForm(settings)
{
    var defaults = {
        items           :   {},
        listRowHandler  :   function(){},
        listTableHandler:   function(){},
        urlItemAdd      :   null,
        urlItemSave     :   null,
        urlItemDelete   :   null,
    };
    
    ItemsFormSettings = $.extend(defaults, settings);

    updateItems();
}

function updateItems()
{
    try 
    {
        $('#itemsTable').tablefill(ItemsFormSettings.items, function(tr_elem, data){ 
            
            ItemsFormSettings.listRowHandler(tr_elem, data);
            ItemsFormSettings.listTableHandler(this);

            $(tr_elem).find(".delete").click(function(){ itemActions(data, 'delete') });
            $(tr_elem).find(".edit").click(function(){ itemActions(data, 'edit') });
        });

    } catch(err) { alert(err); }
}

function itemActions(data, action)
{
    try 
    {  
        $('#itemsTableResult').text('');
        
        if (data == null)
        {
            if (action == 'add')
            {
                Item = data;
                for(var i in data) $("[name='" + i +"']").val(data[i]);
                
                $("td.itemTable th#itemCaption").text("Добавление новой роли");
                $("td.itemTable").show();
            }
            else if (action == 'save')
            {
                var itemData = $('form#itemForm').serializeArray();
                $.requestJSON(ItemUrlAdd, itemData, function(result, message, data){
                    if (result == JsonResult.OK)
                    {
                        $("td.itemTable").hide();

                        Items[data] = itemData;
                        updateItems();
                    }
                    $("#itemTableResult").html(message);
                });
            }
        }
        else
        {
            if (action == 'edit')
            {
                Item = data;
                
                for(var i in data) $("[name='" + i +"']").val(data[i]);
                
                $("td.itemTable th#itemCaption").text("Редактирование роли '" + data.Name + "'");
                $("td.itemTable").show();
            }
            else if (action == 'delete')
            {
                if (confirm('Вы действительно хотите удалить роль "' + data.NameRole + '"?'))
                {
                    $.requestJSON(ItemUrlDelete + data.IdRole, null, function(result, message){
                        if (result == JsonResult.OK) 
                        {
                            delete Items[data.IdRole];
                            updateItems();
                        }
                        if (message != null && message.length > 0) $("#itemsTableResult").html(message);
                    });
                }
            }
            else if (action == 'save')
            {
                var itemData = $('form#itemForm').serializeArray();
                $.requestJSON(ItemUrlSave, itemData, function(result, message){
                    if (result == JsonResult.OK)
                    {
                        $("td#roleTable").hide();

                        $.each(Items, function(key, value){ if (value == Item) Items[key] = itemData; });
                        Item = itemData;
                        updateItems();
                    }
                    if (message != null && message.length > 0) $("#itemTableResult").html(message);
                });
            }
        }
    } 
    catch(err) { $("#itemsTableResult").html(err); }
}

$(document).ready(function(){
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
});
</script>
<{block 'script'}><{/block}>
<h2>Настройка ролей</h2>

<table>
 <tr>
  <td class='itemsTable' style='vertical-align:top;'>
  
    <{block 'itemsList'}>
    <table id='itemsTable' style="width:450px;" class='tablesorter'><thead>
     <tr>
      <th colspan='2'>Список объектов | <input type='button' id='itemAdd' value='Добавить'></th>
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
    <{/block}>
    <div id='itemsTableResult'></div>

  </td>
  <td class='itemTable' style='padding-left:20px;vertical-align:top;'>
  
    <form method='post' id='itemForm'>
    
    <{block 'itemForm'}>
    <input type='hidden' name='IdItem' value='0'>
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
      <td><input type='text' name='NameRole'></td>
     </tr>
    </table>
    <{/block}>

    <input type='submit' value='Сохранить'>
    <input type='button' value='Отменить' class='ButtonClose'>
    
    </form>
    <div id='itemTableResult'></div>

  </td>
  
  <{block 'otherTDs'}
  <{/block}>

 </tr>
</table>
