<script type='text/javascript'>
try {
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
    },null);

    $('a.this_view').click(function(){
        try {
            var aj = new ajaxRequest();
            aj.load($(this).attr('href'),'cmain');
            return false;    
        } catch (err) { alert(err); }
        return false;
    });
    
    changeTitle('Просмотр категорий и баннеров');
});
</script>
<{include file="admin/admin_banners_manage_catsitems.tpl"}>

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
   <!--<a href='' class="cat_delete">удалить</a><br>-->
   <a href='/admin/mnadmin/@Module.UrlName/cats_edit/' class='cat_edit'>редактировать</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/cats/' class='cat_view'>просмотр</a>
  </td>
 </tr>  
</table>
