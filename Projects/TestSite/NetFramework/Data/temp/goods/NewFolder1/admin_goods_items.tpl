<script type='text/javascript'>
function update_items()
{
    try {
        change_mode(2);
        
        $('a.name_edit').click(function(){
            try {
                aj = new ajaxRequest();
                var id = $(this).parent().parent().find('input[name=item_id]').val();
                
                aj.setPOST('item_name',$(this).parent().parent().find('input.name_edit').val());
                aj.userOnLoad = function(){endAnim($("#action_result"))};
                
                $("#admin_botgr,#action_result").fadeOut();
                aj.load('/admin/madmin/@Module.UrlName/item_edit_save/'+id,'action_result');
                $("#loading_img").fadeIn();

            } catch(err) { alert(err); }
            return false;
        });
        
        $('a.item_copy').click(function(){
            try {
                aj = new ajaxRequest();
                var id = $(this).parent().parent().find('input[name=item_id]').val();
                
                aj.load('/admin/madmin/@Module.UrlName/item_copy/'+id,'items_result');
            } catch(err) { alert(err); }
            return false;
        });
    } catch(err) { alert(err); }
}

function change_mode(type)
{
    if ( type == 1 )
    {
        $('#items_results').find('.name_edit').show();
    } else if ( type == 2 )
    {
        $('#items_results').find('.name_edit').hide();
    }
}

function item_edit_result(stat,id)
{
    if ( stat == 1 && id != -1 )
    {
        $('#items_results').find('input[name=item_id]').each(function(i,child){
            if ( $(child).val() == id ) 
            {
                $(child).parent().parent().find('span.name').text($(child).parent().parent().find('input.name_edit').val());
                return false;
            }
        });
    }
}

function item_copy_result(stat,id,newid,result)
{
    alert(result);
}

$(document).ready(function() {
    $("#block").hide();
    try {
        changeTitle('Просмотр списка товаров');
        update_items();
        
        $('a.item_delete').click(function(){
            item_delete($(this).attr("rel"));
            return false;
        });
        
        $('a.this_view').click(function(){
            try {
                var aj = new ajaxRequest();
                aj.load($(this).attr('href'),'cmain');
                return false;    
            } catch (err) { alert(err); }
            return false;       
        });
        
        $('select#category_select').change(function(){
            try {
                mFilterValue = $('select#category_select').val();
                update_items();
            } catch (err) { alert(err); }
        });
        
        $('a#mode_edit').click(function(){
            change_mode(1);
            return false;
        });
        
        $('a#mode_view').click(function(){
            change_mode(2);
            return false;
        });
        
        $( "#items_results tbody" ).sortable({stop:function(){
            var unun = '';
            $("#items_results tbody tr").each(function(){
                if ($(this).attr("id") != "undefined") var tid = $(this).attr("id");
                if(tid.indexOf('tr_res_') + 1) unun = unun + tid +',';
            })
            //$("#order_save input[name='unun']").val(unun);
            //$("#order_save").submit();
            $.ajax({
                type: 'POST',
                url: '/admin/madmin/@Module.UrlName/item_update_order',
                data: {order:unun},
                cache: false,
                dataType: 'json',
                success: function(data) {
                    if (data.err == 1) {
                        alert(data.text);
                    } else {
                        $("#item_result").text("Порядок успешно сохранен")
                        endAnim($("#item_result"));
                    }
                    $("#block").fadeOut();
                }
            });
        }});
        $( "#items_results" ).disableSelection();
    } catch(err) { alert(err); }
    
});
</script>
<{include file="admin/admin_goods_manage_catsitems.tpl"}>
<h2>Товары</h2>
<p id="empty_info">В списке есть только те категории, в которых были найдены товары. Пустые категории не отображены.</p>
Фильтр: <select id='category_select'>
 <option value='-1'>Все товары</option>
<{foreach from=$data_cats item=ad key=id}>
 <option value='<{$ad.id}>'><{$ad.name}></option>
<{/foreach}>
</select><br>

<a href='' id='mode_edit'>Режим редактирования</a>&nbsp;|&nbsp;
<a href='' id='mode_view'>Режим просмотра</a>&nbsp;
<img src="/data/img/loading.gif" alt="" id="loading_img" /> 
<div id='items_result'></div>
<table id='items_results' class='admtable'>
 <thead>
  <tr>
   <th style="width:15px">№</th>
   <th style="width:85px">Фото</th>
   <th style="width:250px">Название</th>
   <th style="width:65px">Тип</th>
   <th style="width:65px">Упаковка</th>
   <th colspan="2">Действия</th>
  </tr>
 </thead>
 <tbody>
 <{if $data_items|@count>0}>
 <{foreach from=$data_items item=ad key=id}>
  <tr id='tr_res_<{$ad.id}>'>
   <td class="center">
    <a href='/@Module.UrlName/cat/<{$ad.category}>#<{$ad.id}>' class='item_view' target='_blank'><{$ad.id}></a>
    <input type='hidden' name='item_id' value='<{$ad.id}>'>
   </td>
   <td class="center" style="position:relative;"><img class="p_photo" alt="" src="<{if isset($ad.photo.item_icon)}>/<{$ad.photo.item_icon}><{else}>/data/img/no.png<{/if}>"></td>
   <td><span class='name'><{$ad.name|re_quote}></span><br class='name_edit'><input type='text' class='name_edit' name='item_name' size='20' maxlength='200' value='<{$ad.name|re_quote}>' style='margin-bottom:10px;'></td>
   <td><span class='type'><{$ad.type}></span></td>
   <td><span class='type'><{$ad.pack}></span></td>
   <td>
    <a href='' class='name_edit'>Сохранить изменения</a> <br class='name_edit'><br class='name_edit'>
    <a href='/admin/mnadmin/@Module.UrlName/item_edit/<{$ad.id}>' class='item_edit'>редактировать</a><br>
    <a href='/@Module.UrlName/cat/<{$ad.category}>#<{$ad.id}>' class='item_view' target='_blank'>просмотр</a><br>
    <a href='' class="item_delete" rel="<{$ad.id}>">удалить</a><br>
   </td>
   <td class="holder"></td>
  </tr>
 <{/foreach}>
 <{else}>
 <tr id='notfounded'>
  <td>Ничего не найдено</td>
 </tr>
 <{/if}>
 </tbody>  
</table>
<p style="margin:10px 0 25 35px;"><a href="javascript:scroll(0,0);" title="">К началу страницы</a></p>
