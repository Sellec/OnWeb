<script type='text/javascript'>
try {
mNewsArray = new Array();
<{foreach from=$data_news item=ad key=id}>
mNewsArray[<{$id}>] = {"id":<{$ad.id}>,"name":"<{$ad.name|java_string}>","category":"<{$ad.category}>","status":"<{$ad.status}>"};
<{/foreach}>
mFilterValue = -1;
} catch(err) { alert(err); }

function update_news()
{
    try {
        $('#news_results').tablefill(mNewsArray,function(tr_elem,data){
            $(tr_elem).children().each(function(ichild){
                if ( ichild == 0 ) $(this)[0].innerHTML = data["id"];
                else if ( ichild == 1 ) $(this).find('a').text(data['name']);
            });  
            
            $(tr_elem).find('a.news_delete').click(function(){
                news_delete($(this).parent().parent()[0]);
                return false;
            });
            
            if (data["status"]==0) $(tr_elem).addClass("tr_off");
            
            $(tr_elem).find('a.news_view').attr('href',$(tr_elem).find('a.news_view').attr('href')+data['id']);
            $(tr_elem).find('a.news_edit').attr('href',$(tr_elem).find('a.news_edit').attr('href')+data['id']);

        },function(data){
            if ( mFilterValue != -1 && data['category'] != mFilterValue ) return false;
            return true;            
        });
    } catch(err) { alert(err); }
}

$(document).ready(function() {
    $("#block").hide();
    try {                         
        changeTitle('Просмотр списка новостей без фильтрации по категориям');

        update_news();
        
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
                update_news();
            } catch (err) { alert(err); }
        });
        
    } catch(err) { alert(err); }
    
});
</script>
<{include file="admin/admin_news_manage_catsnews.tpl"}>
<h2>Новости</h2>
<p>В списке есть только те категории, в которых были найдены новости. Пустые категории не отображены.</p>
Фильтр: <select id='category_select'>
 <option value='-1'>Все новости</option>
<{foreach from=$data_cats item=ad key=id}>
 <option value='<{$ad.id}>'><{$ad.name}></option>
<{/foreach}>
</select><br>
<div id='news_result' style="color:red;margin-top:10px;"></div>
<table id='news_results' class='tablesorter'><thead>
 <tr>
  <th style="width:15px">№</th>
  <th style="width:450px">Название</th>
  <th>Действия</th>
 </tr>
 </thead><tbody></tbody>
 <tr id='notfounded'>
  <td colspan='4'>Ничего не найдено</td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center"></td>
  <td><a href='/admin/mnadmin/@Module.UrlName/news_view/' class='news_view' target='_blank'></a></td>
  <td>
   <a href='/admin/mnadmin/@Module.UrlName/news_edit/' class='news_edit'>редактировать</a><br>
   <a href='' class="news_delete">удалить</a><br>
  </td>
 </tr>  
</table>
