<script type='text/javascript'>
try {
mItemsArray = new Array();
<{foreach from=$data_items item=ad key=id}>
mItemsArray[<{$id}>] = {"id":<{$ad.id}>,"name":"<{$ad.name|replace:'"':''}>","category":"<{$ad.category|replace:'"':''}>","sklad_count":"<{$ad.sklad_count}>"};
<{/foreach}>

mCatsArray = new Array();
<{foreach from=$data_cats item=ad key=id}>
mCatsArray[<{$id}>] = {"id":<{$ad.id}>,"name":"<{$ad.name}>"};
<{/foreach}>
} catch(err) { alert(err); }

$(document).ready(function() {
    $("#block").hide();
    $('#cats_results').tablefill(mCatsArray,function(tr_elem,data){
        try {
            $(tr_elem).children().each(function(ichild){
                if ( ichild == 0 ) $(this)[0].innerHTML = data['id'];
                else if ( ichild == 1 ) $(this).find("a").text(data["name"]);
            });  
            $(tr_elem).find('a.cat_view').attr('href',$(tr_elem).find('a.cat_view').attr('href')+data['id']);
            $(tr_elem).find('a.cat_edit').attr('href',$(tr_elem).find('a.cat_edit').attr('href')+data['id']);

            $(tr_elem).find('a.cat_delete').click(function(){
                cats_delete($(this).parent().parent()[0]);
                return false;
            });
            
            
        } catch(err) { alert(err); }
    });

    $('a.this_view').click(function(){
        try {
            var aj = new ajaxRequest();
            aj.load($(this).attr('href'),'cmain');
            return false;    
        } catch (err) { alert(err); }
        return false;
    });
    
    changeTitle('Просмотр категорий и товаров');
});
</script>
<{include file="admin/admin_goods_manage_catsitems.tpl"}>

<h2><{$data.name}></h2>
<strong>Подкатегории:</strong><br>
<div id='cats_result' style="color:red;margin-top:10px;"></div>
<table id='cats_results' class='admtable'>
 <tr>
  <th style="width:15px">№</th>
  <th style="width:250px">Название</th>
  <th>Действия</th>
 </tr>
 <tr id='notfounded'>
  <td colspan='4'>Ничего не найдено</td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center"></td>
  <td><a href='/admin/mnadmin/@Module.UrlName/cats/' class='cat_view'>просмотр</a></td>
  <td>
   <a href='/admin/mnadmin/@Module.UrlName/cats_edit/' class='cat_edit'>редактировать</a><br>
   <a href='' class="cat_delete">удалить</a><br>
  </td>
 </tr>  
</table>
