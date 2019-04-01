<script type='text/javascript'>
try {
mItemsArray = new Array();
<{foreach from=$data_items item=ad key=id}>
mItemsArray[<{$id}>] = {"id":<{$ad.id}>,"name":"<{$ad.name|replace:'"':'\"'}>","category":"<{$ad.category}>","prev_photo":"<{if isset($ad.banner_image)}><{$ad.banner_image}><{/if}>"};
<{/foreach}>
mFilterValue = -1;
} catch(err) { alert(err); }

function update_items()
{
    try {
        $('#itemss_results').tablefill(mItemsArray,function(tr_elem,data){
            $(tr_elem).children().each(function(ichild){
                if ( ichild == 0 ) $(this).find('a').text(data['id']);
                else if ( ichild == 1) $(this).find('img.p_photo').attr('src',data['prev_photo']);
                else if ( ichild == 2 ) 
                {
                    $(this).find('span.name').text(data["name"]);
                    $(this).find('input.name_edit').val(data["name"]);
                }
            });
            $(tr_elem).find('input[name=item_id]').val(data['id']);
            
            $(tr_elem).find('a.item_delete').click(function(){
                item_delete($(this).parent().parent()[0]);
                return false;
            });
            
            $(tr_elem).find('a.item_view').attr('href',$(tr_elem).find('a.item_view').attr('href')+data['id']);
            $(tr_elem).find('a.item_edit').attr('href',$(tr_elem).find('a.item_edit').attr('href')+data['id']+'&catid='+<{$smarty.post.cat}>);
            $(tr_elem).find('a.item_photo').attr('href',$(tr_elem).find('a.item_photo').attr('href')+data['id']);
            

        },function(data){
            if ( mFilterValue != -1 && data['category'] != mFilterValue ) return false;
            return true;            
        },function(){
            change_mode(2);
            
            $('a.name_edit').click(function(){
                try {
                    aj = new ajaxRequest();
                    var id = $(this).parent().parent().find('input[name=item_id]').val();
                    
                    aj.setPOST('item_name',$(this).parent().parent().find('input.name_edit').val());
                    aj.load('/admin/madmin/@Module.UrlName/item_edit_save/'+id,'items_result');
                    var div_res = $("#items_result");
                    div_res.fadeIn("slow");
                    setTimeout(function(){div_res.fadeOut("slow")}, 2500);
                    var opros = setInterval(function(){
                        if ( div_res.text() != "" ) {
                            setTimeout(function(){div_res.text("");}, 3500 );
                            clearInterval(opros);
                        };
                    }, 1000);
                } catch(err) { alert(err); }
                return false;
            });
            
        });
    } catch(err) { alert(err); }
}

function change_mode(type)
{
    if ( type == 1 )
    {
        $('#items_results').find('.name_edit').show();
        $('#items_results').find('.price_edit').show();
    } else if ( type == 2 )
    {
        $('#items_results').find('.name_edit').hide();
        $('#items_results').find('.price_edit').hide();
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
                $(child).parent().parent().find('span.price').text($(child).parent().parent().find('input.price_edit').val());
                return false;
            }
        });
    }
}

$(document).ready(function() {
    $("#block").hide();
    try {
        changeTitle('Просмотр списка баннеров без фильтрации по категориям');

        update_items();
        
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
       
    } catch(err) { alert(err); }
    
});
</script>
<{include file="admin/admin_banners_manage_catsitems.tpl"}>
<h2>Товары</h2>
<p>В списке есть только те категории, в которых были найдены баннеры. Пустые категории не отображены.</p>
Фильтр: <select id='category_select'>
 <option value='-1'>Все баннеры</option>
<{foreach from=$data_cats item=ad key=id}>
 <option value='<{$ad.id}>'><{$ad.name}></option>
<{/foreach}>
</select><br>

<a href='' id='mode_edit'>Режим редактирования</a>&nbsp;|&nbsp;
<a href='' id='mode_view'>Режим просмотра</a>&nbsp;
<div id='items_result' style="color:red;margin-top:10px;"></div>
<table id='items_results' class='tablesorter'><thead>
 <tr>
  <th style="width:15px">№</th>
  <th style="width:135px">Фото</th>
  <th style="width:250px">Название</th>
  <th>Действия</th>
 </tr>
 </thead><tbody></tbody>
 <tr id='notfounded'>
  <td colspan='4'>Ничего не найдено</td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center">
   <a href='/admin/mnadmin/@Module.UrlName/item/' class='item_view' target='_blank'></a>
   <input type='hidden' name='item_id' value=''>
  </td>
  <td class="center"><img class="p_photo" alt=""></td>
  <td><span class='name'></span><br class='name_edit'><input type='text' class='name_edit' name='item_name' size='20' maxlength='200' value='' style='margin-bottom:10px;'></td>
  <td>
   <a href='' class='name_edit'>Сохранить изменения</a><br class='name_edit'><br class='name_edit'>
   <a href='' class="item_delete">удалить</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/item_edit/' class='item_edit'>редактировать</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/item/' class='item_view' target='_blank'>просмотр</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/item_photo/' class='item_photo' target='_blank'>фотография</a><br>
  </td>
 </tr>  
</table>
<p style="margin:10px 0 25 35px;"><a href="javascript:scroll(0,0);" title="">К началу страницы</a></p>
