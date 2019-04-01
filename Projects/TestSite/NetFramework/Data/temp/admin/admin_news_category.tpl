<script type='text/javascript'>
try {
mNewsArray = new Array();
<{foreach from=$data_news item=ad key=id}>
mNewsArray[<{$id}>] = {"id":<{$ad.id}>,"name":"<{$ad.name|java_string}>","category":"<{$ad.category}>"};
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
            if (data['id']==1) $(tr_elem).find(".cat_delete").hide();
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
    
    $('#news_results').tablefill(mNewsArray,function(tr_elem,data){
        try {
            $(tr_elem).children().each(function(ichild){
                if ( ichild == 0 ) $(this).find('a').text(data['id']);
                else if ( ichild == 1 ) $(this)[0].innerHTML = data["name"];
            });  
            $(tr_elem).find('a.news_view').attr('href',$(tr_elem).find('a.news_view').attr('href')+data['id']);
            $(tr_elem).find('a.news_edit').attr('href',$(tr_elem).find('a.news_edit').attr('href')+data['id']);
            $(tr_elem).find('a.cat_delete').click(function(){
                news_delete($(this).parent().parent()[0]);
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
<{include file="admin/admin_news_manage_catsnews.tpl"}>

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
   <a href='' class="cat_delete">удалить</a><br class="cat_delete">
   <a href='/admin/mnadmin/@Module.UrlName/cats_edit/' class='cat_edit'>редактировать</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/cats/' class='cat_view'>просмотр</a>
  </td>
 </tr>  
</table>

Новости:<br>
<div id='news_result' style="color:red;margin-top:10px;"></div>
<table id='news_results' class="admtable">
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
   <a href='/admin/mnadmin/@Module.UrlName/news_view/' class='news_view' target='_blank'></a>
  </td>
  <td></td>
  <td>
   <a href='' class="news_delete">удалить</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/news_edit/' class='news_edit'>редактировать</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/news_view/' class='news_view' target='_blank'>просмотр</a>
  </td>
 </tr>  
</table>
