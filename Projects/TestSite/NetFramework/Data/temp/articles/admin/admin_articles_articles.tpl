<script type='text/javascript'>
try {
marticlesArray = new Array();
<{foreach from=$data_articles item=ad key=id}>
marticlesArray[<{$id}>] = {"id":<{$ad.id}>,"name":"<{$ad.name|java_string}>","category":"<{$ad.category}>"};
<{/foreach}>
mFilterValue = -1;
} catch(err) { alert(err); }

function update_articles()
{
    $('#articles_results').tablefill(marticlesArray,function(tr_elem,data){
        $(tr_elem).children().each(function(ichild){
            if ( ichild == 0 ) $(this)[0].innerHTML = data["id"];
            else if ( ichild == 1 ) $(this).find('a').text(data['name']);
        });  
        
        $(tr_elem).find('a.articles_delete').click(function(){
            articles_delete($(this).parent().parent()[0]);
            return false;
        });
        
        $(tr_elem).find('a.articles_view').attr('href',$(tr_elem).find('a.articles_view').attr('href')+data['id']);
        $(tr_elem).find('a.articles_edit').attr('href',$(tr_elem).find('a.articles_edit').attr('href')+data['id']);

    }, function(data){
        if ( mFilterValue != -1 && data['category'] != mFilterValue ) return false;
        return true;            
    });
}

$(document).ready(function() {
    $("#block").hide();
    try {                         
        changeTitle('Просмотр списка статей без фильтрации по категориям');

        update_articles();
        
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
                update_articles();
            } catch (err) { alert(err); }
        });
        
    } catch(err) { alert(err); }
    
});
</script>
<{include file="admin/admin_articles_manage_catsarticles.tpl"}>
<h2>Статьи</h2>
<p>В списке показаны только категории, в которых были найдены статьи.</p>
Фильтр: <select id='category_select'>
 <option value='-1'>Все статьи</option>
<{foreach from=$data_cats item=ad key=id}>
 <option value='<{$ad.id}>'><{$ad.name}></option>
<{/foreach}>
</select><br>
<div id='articles_result' style="color:red;margin-top:10px;"></div>
<table id='articles_results' class='tablesorter'><thead>
 <tr>
  <th style="width:15px">№</th>
  <th style="width:550px">Название</th>
  <th>Действия</th>
 </tr>
 </thead><tbody></tbody>
 <tr id='notfounded'>
  <td colspan='4'>Ничего не найдено</td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center"></td>
  <td>
   <a href='/admin/mnadmin/@Module.UrlName/articles_view/' class='articles_view' target='_blank'></a>
  </td>
  <td>
   <a href='/admin/mnadmin/@Module.UrlName/articles_edit/' class='articles_edit'>редактировать</a><br>
   <a href='' class="articles_delete">удалить</a><br>
  </td>
 </tr>  
</table>
