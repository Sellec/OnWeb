<script type='text/javascript'>
<{if $data.zone_id==-1}>
function item_add_result(res,id)
{
    if ( res == 1 ) 
    {
        $('input[name=zone_id]').val(id);
        $('td#step2, td#step3').show();
    }
}

<{/if}>

function update_step2()
{
    $('select#module option').remove();
    $('select#zone_modules option:selected').each(function(s1,s2){
        $('select#module')[0].options.add(new Option($(s2).text(),$(s2).val()));        
    });
}

$(document).ready(function() {
    try {
        $("#block").hide();

        <{if $data.zone_id == -1}>
        changeTitle('Добавление зоны');
        $('td#step2, td#step3').hide();
        <{else}>
        changeTitle("Редактирование зоны: <{$data.zone_name|java_string}>");
        <{/if}>
        $(".i_adminmenu ul").show();

        aj = new ajaxRequest();
        aj.load_form('form_ae',null,'item_result');
        aj.userOnLoad = function(){endAnim($("#item_result"))};

        aj2 = new ajaxRequest();
        aj2.load_form('form_ae2',null,'zone_result2');
        
        aj3 = new ajaxRequest();
        aj3.load_form('form_ae3',null,'item_result');
        aj3.userOnLoad = function(){endAnim($("#item_result"))};

        stAnim();
        
        $('select#zone_modules').click(function(){ update_step2(); });
        $('select#zone_modules').change(function(){ update_step2(); });
        
        update_step2();
        
    } catch(err) { alert(err); }
});

function update_urls()
{
    $('#module_urls option').remove();
    
    if ( typeof(mURLArray) != 'undefined' )
    {
        var sel = $('#module_urls');
        for ( var i in mURLArray )
        {
            var selected = false;
            if ( mURLArray[i]['selected'] == 1 ) selected = true;
            var opt = new Option(mURLArray[i]['name'],i,selected,selected);
            sel[0].options[sel[0].options.length] = opt;
        }
    }
}

</script>
<h2>Зона</h2>
<br>
<table id="items_results">
 <tr>
 </tr>
 <tr>
  <td width='400'>
   <{if $data.zone_id == -1}>
   <form action='/admin/madmin/@Module.Name/zones_add_save' method='post' id='form_ae'>
   <{else}>
   <form action='/admin/madmin/@Module.Name/zones_edit_save/<{$data.zone_id}>' method='post' id='form_ae'>
   <{/if}>
   Название:&nbsp;<input type='text' name='zone_name' size='40' maxlength='200' value='<{$data.zone_name}>'><br>
   Адрес:&nbsp;<input type='text' name='zone_addrname' size='40' maxlength='200' value='<{$data.zone_addrname}>'><br>
   Шаблон (при пустом значении используется index_index.tpl):&nbsp;<input type='text' name='zone_template' size='40' maxlength='200' value='<{$data.zone_template}>'><br><br>
   
   <input type='hidden' name='zone_main' value='0'><input type='checkbox' name='zone_main' value='1' <{if $data.zone_main==1}>checked<{/if}>>
   Поставьте галочку, если данная зона может использоваться в качестве общей. Это подразумевает, что без явного указания тематической зоны, на сайте будет загружена данная зона.<br><br>

   Выберите модули, относящиеся к данной тематической зоне:<br>
   <input type='hidden' name='zone_modules[]' value='-1'>
   <select name='zone_modules[]' id='zone_modules' multiple size='10' style="width:270px; ">
    <{foreach from=$data_mods item=ad key=id}>
     <option value='<{$id}>' <{if $id|in_array:$data.zone_modules}>selected<{/if}>><{$ad.name}></option>
    <{/foreach}>
   </select><br><br>
   
   <{if $data.zone_id == -1}>
   <input type='submit' value='Добавить'> <img src="/data/img/loading.gif" alt="" id="loading_img" style='display:none;'>
   <{else}>
   <input type='submit' value='Сохранить'> <img src="/data/img/loading.gif" alt="" id="loading_img" style='display:none;'>
   <{/if}>
   
   </form>
   <div id='zone_result'></div>
  </td>
  <td width='400' id='step2'>
   Выделите один модуль и нажмите "Получить данные", чтобы отметить определенные данные для этой тематической зоны:<br>
   <form action='/admin/madmin/@Module.Name/zones_getmoduledata' method='post' id='form_ae2'>
   <input type='hidden' name='zone_id' value='<{$data.zone_id}>'>
   <select id='module' name='module' size='10' style="width:270px; ">
    <{foreach from=$data_mods item=ad key=id}>
     <option value='<{$id}>'><{$ad.name}></option>
    <{/foreach}>
   </select>
   <input type='submit' value='Получить данные'>
   </form>
   <div id='zone_result2'></div>
  </td>
  <td width='400' id='step3'>
   Выделите необходимые пункты в меню и нажмите кнопку "Отметить для зоны":<br>
   <form action='/admin/madmin/@Module.Name/zones_getmoduledata_save' method='post' id='form_ae3'>
   <input type='hidden' name='zone_id' value='<{$data.zone_id}>'>
   <input type='hidden' name='module' value=''>
   <select name='module_datas[]' id='module_urls' multiple size='10' style="width:270px;">
   
   </select><br>
   <input type='submit' value='Отметить для зоны'>
   </form>
   <div id='zone_result3'></div>
  </td>
 </tr>
</table>
