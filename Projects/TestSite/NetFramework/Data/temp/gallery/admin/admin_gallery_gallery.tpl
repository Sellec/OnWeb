<script type='text/javascript'>
try {
mPhotoArray = new Array();
<{foreach from=$data_photo item=ad key=id}>
mPhotoArray[<{$id}>] = {"id":<{$ad.id}>,"thumb":"<{$ad.photo.preview_file}>","full":"<{$ad.photo.photo_file}>"};
<{/foreach}>
} catch(err) { alert(err); }

$(document).ready(function() {
    $("#block").hide();
    $('#photos_results').tablefill(mPhotoArray,function(tr_elem,data){
        try {
            $(tr_elem).children().each(function(ichild){
                if ( ichild == 0 ) $(this)[0].innerHTML = data['id'];
                else if ( ichild == 1 ) $(this).find("img").attr("src","/data/photo/" + data["thumb"]);
            });  
            $(tr_elem).find('a.photo_view').attr('href','/data/photo/'+data["full"]);
            $(tr_elem).find('a.photo_edit').attr('href',$(tr_elem).find('a.photo_edit').attr('href')+data['id']);

            $(tr_elem).find('a.photo_delete').click(function(){
                photo_delete($(this).parent().parent()[0]);
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
    
    changeTitle('Просмотр галереи "<{$data.name}>"');
});
</script>
<{include file="admin/admin_gallery_manage_catsgals.tpl"}>

<h2><{$data.name}></h2>
<strong>Фотографии:</strong><br>
<div id='photo_result' style="color:red;margin-top:10px;"></div>
<table id='photos_results' class="admtable">
 <tr>
  <th style="width:15px">№</th>
  <th style="width:165px">Фото</th>
  <th>Действия</th>
 </tr>
 <tr id='notfounded'>
  <td colspan='4'>Ничего не найдено</td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center"></td>
  <td><a href='' class='photo_view' target="_blank"><img src="" alt=""></a></td>
  <td>
   <a href='/admin/madmin/@Module.UrlName/photo_edit/' class='photo_edit'>редактировать</a><br>
   <a href='' class="photo_delete">удалить</a><br>
  </td>
 </tr>  
</table>


