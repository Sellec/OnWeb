<script type='text/javascript'>
try {
mPagesArray = new Array();
<{foreach from=$data_pages item=ad key=id}>
mPagesArray[<{$id}>] = {"id":<{$ad.id}>,"name":"<{$ad.name|java_string}>","category":"<{$ad.category}>"};
<{/foreach}>

mCatsArray = new Array();
<{foreach from=$data_cats item=ad key=id}>
mCatsArray[<{$id}>] = {"id":<{$ad.id}>,"name":"<{$ad.name|java_string}>"};
<{/foreach}>
} catch(err) { alert(err); }

$(document).ready(function() {
    $("#block").hide();
    $('#cats_results').tablefill(mCatsArray,function(tr_elem,data){
        try {
            $(tr_elem).children().each(function(ichild){
                if ( ichild == 0 ) $(this).find('a').text(data['id']);
                else if ( ichild == 1 ) $(this)[0].innerHTML = data["name"];
            });  
            $(tr_elem).find('a.cat_view').attr('href',$(tr_elem).find('a.cat_view').attr('href')+data['id']);
            $(tr_elem).find('a.cat_edit').attr('href',$(tr_elem).find('a.cat_edit').attr('href')+data['id']);

            $(tr_elem).find('a.cat_delete').click(function(){
                cats_delete($(this).parent().parent()[0]);
                return false;
            });
            
            
        } catch(err) { alert(err); }
    });
    
    $('#pages_results').tablefill(mPagesArray,function(tr_elem,data){
        try {
            $(tr_elem).children().each(function(ichild){
                if ( ichild == 0 ) $(this).find('a').text(data['id']);
                else if ( ichild == 1 ) $(this)[0].innerHTML = data["name"];
            });  
            $(tr_elem).find('a.pages_view').attr('href',$(tr_elem).find('a.pages_view').attr('href')+data['id']);
            $(tr_elem).find('a.pages_edit').attr('href',$(tr_elem).find('a.pages_edit').attr('href')+data['id']);

            $(tr_elem).find('a.pages_delete').click(function(){
                pages_delete($(this).parent().parent()[0]);
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
<{include file="admin/admin_pages_manage_catspages.tpl"}>

<h2><{$data.name}></h2>
<strong>Подкатегории:</strong><br>
<div id='cats_result' style="color:red;margin-top:10px;"></div>
<table id='cats_results' class='tablesorter'><thead>
 <tr>
  <th style="width:15px">№</th>
  <th style="width:250px">Название</th>
  <th>Действия</th>
 </tr>
 </thead><tbody></tbody>
 <tr id='notfounded'>
  <td colspan='4'>Ничего не найдено</td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center">
   <a href='/admin/mnadmin/@Module.UrlName/cats/' class='cat_view'></a>
  </td>
  <td></td>
  <td>
   <a href='' class="cat_delete">удалить</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/cats_edit/' class='cat_edit'>редактировать</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/cats/' class='cat_view'>просмотр</a>
  </td>
 </tr>  
</table>

Страницы:<br>
<div id='pages_result' style="color:red;margin-top:10px;"></div>
<table id='pages_results'>
 <tr>
  <th style="width:15px">№</th>
  <th style="width:250px">Название</th>
  <th>Действия</th>
 </tr>
 <tr id='notfounded'>
  <td colspan='4'>Ничего не найдено</td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center">
   <a href='/admin/mnadmin/@Module.UrlName/pages_view/' class='pages_view' target='_blank'></a>
  </td>
  <td></td>
  <td>
   <a href='' class="pages_delete">удалить</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/pages_edit/' class='pages_edit'>редактировать</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/pages_view/' class='pages_view' target='_blank'>просмотр</a>
  </td>
 </tr>  
</table>
