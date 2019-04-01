<script type='text/javascript'>
try {
mVideoArray = new Array();
<{foreach from=$data_video item=ad key=id}>
mVideoArray[<{$id}>] = {"id":<{$ad.id}>,"name":"<{$ad.name|java_string}>","thumb":"<{$ad.file.preview_file}>"};
<{/foreach}>
} catch(err) { alert(err); }

$(document).ready(function() {
    $("#block").hide();
    $('#videos_results').tablefill(mVideoArray,function(tr_elem,data){
        try {
            $(tr_elem).children().each(function(ichild){
                if ( ichild == 0 ) $(this).find('a').text(data['id']);
                else if ( ichild == 1 ) $(this)[0].innerHTML = data["name"];
                else if ( ichild == 2 ) $(this).find("img").attr("src","/data/video/" + data["thumb"]);
            });  
            $(tr_elem).find('a.video_view').attr('href',$(tr_elem).find('a.video_view').attr('href')+data['id']);
            $(tr_elem).find('a.video_edit').attr('href',$(tr_elem).find('a.video_edit').attr('href')+data['id']);

            $(tr_elem).find('a.video_delete').click(function(){
                video_delete($(this).parent().parent()[0]);
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
    
    changeTitle("Просмотр галереи: <{$data.name|java_string}>");
});
</script>
<{include file="admin/admin_gallery_manage_catsgals.tpl"}>

<h2><{$data.name}></h2>
<strong>Видеофайлы:</strong><br>
<div id='video_result' style="color:red;margin-top:10px;"></div>
<table id='videos_results' class='tablesorter'><thead>
 <tr>
  <th style="width:15px">№</th>
  <th style="width:250px">Название</th>
  <th style="width:165px">Видео</th>
  <th>Действия</th>
 </tr>
 </thead><tbody></tbody>
 <tr id='notfounded'>
  <td colspan='4'>Ничего не найдено</td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center">
   <a href='/admin/mnadmin/@Module.UrlName/video_view/' class='video_view'></a>
  </td>
  <td></td>
  <td><img src="" alt="" width=75%></td>
  <td>
   <a href='' class="video_delete">удалить</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/video_edit/' class='video_edit'>редактировать</a><br>
   <a href='/admin/mnadmin/@Module.UrlName/video_view/' class='video_view'>просмотр</a>
  </td>
 </tr>  
</table>


