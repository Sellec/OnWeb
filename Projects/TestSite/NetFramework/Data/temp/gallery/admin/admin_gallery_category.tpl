<script type='text/javascript'>
mGalleryArray = <{$data_gallery|@jsobject}>;
mCatsArray = <{$data_cats|@jsobject}>;

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
    },null);
    
    $('#gals_results').tablefill(mGalleryArray,function(tr_elem,data){
        try {
            $(tr_elem).children().each(function(ichild){
                if ( ichild == 0 ) $(this)[0].innerHTML = data['id'];
                else if ( ichild == 1 ) $(this).find('a').text(data["name"]);
            });  
            $(tr_elem).find('a.gals_view').attr('href',$(tr_elem).find('a.gals_view').attr('href')+data['id']);
            $(tr_elem).find('a.gals_edit').attr('href',$(tr_elem).find('a.gals_edit').attr('href')+data['id']);

            $(tr_elem).find('a.gals_delete').click(function(){
                galls_delete($(this).parent().parent()[0]);
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
    
    changeTitle('Просмотр категорий и товаров');
});
</script>
<{include file="admin/admin_gallery_manage_catsgals.tpl"}>

<h2><{$data.name}></h2>
<strong>Подкатегории:</strong><br>
<div id='cats_result' style="color:red;margin-top:10px;"></div>
<table id='cats_results' class="admtable">
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
   <a href='/admin/mnadmin/@Module.UrlName/cats/' class='cat_view'></a>
  </td>
  <td><a href='/admin/mnadmin/@Module.UrlName/cats/' class='cat_view'></a></td>
  <td>
   <a href='/admin/mnadmin/@Module.UrlName/cats_edit/' class='cat_edit'>редактировать</a><br>
   <a href='' class="cat_delete">удалить</a><br>
  </td>
 </tr>  
</table>

Галереи:<br>
<div id='gals_result' style="color:red;margin-top:10px;"></div>
<table id='gals_results' class="admtable">
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
  <td><a href='/admin/mnadmin/@Module.UrlName/galls_view/' class='gals_view'></a></td>
  <td>
   <a href='/admin/mnadmin/@Module.UrlName/galls_edit/' class='gals_edit'>редактировать</a><br>
   <a href='' class="gals_delete">удалить</a><br>
  </td>
 </tr>  
</table>
