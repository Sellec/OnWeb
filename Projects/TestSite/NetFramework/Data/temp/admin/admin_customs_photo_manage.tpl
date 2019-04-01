<div id='customs_photo_manage_main'><!-- style="border:1px solid blue;"> -->
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------<br>

<script type='text/javascript'>
try {
mPhotosArray = new Array();
<{foreach from=$photo_list item=ad key=id}>mPhotosArray[<{$id}>] = { "id":"<{$id}>", "photo_value":"<{$ad.photo_value}>", "photo_width": <{$ad.photo_width}>, "photo_height": <{$ad.photo_height}>, "photo_label":"<{$ad.photo_label|java_string}>" };<{/foreach}>
<{foreach from=$photo_labels_list item=_ad key=_id}><{foreach from=$_ad item=ad key=id}>mPhotosArray[<{$id}>] = { "id":"<{$id}>", "photo_value":"<{$ad.photo_value}>", "photo_width": <{$ad.photo_width}>, "photo_height": <{$ad.photo_height}>, "photo_label":"<{$ad.photo_label|java_string}>" };<{/foreach}><{/foreach}>
} catch (err) { alert(err); }
var mItemID = -1;
              
function customs_photo_manage_loadeditor(photo_id)
{
    var aj = new ajaxRequest();
    aj.load("/admin/madmin/@Module.UrlName/photo_manage_editor/"+photo_id,'customs_photo_manage_for_editor');

}              

function customs_photo_manage_loadconnect()
{
    _customs_photo_manage_loadconnect(<{$data_id}>,<{$data_type}>);
}


</script>

<div id='customs_photo_manage_for_editor'></div>
<h4>Управление фотографиями для записи №<{$data_id}>.</h4>
<a href='' id='customs_photo_manage_loadphoto'>Загрузить новую фотографию</a>&nbsp;|&nbsp;
<a href='' id='customs_photo_manage_refreshlist'>Обновить список фотографий</a>
<div id='customs_photo_manage_loadphoto_div'></div>

<div id='customs_photo_manage_result'></div>
<table id='table_results' class='tablesorter'><thead>
 <tr>
  <th style="width:15px">№</th>
  <th style="width:250px">Имя файла</th>
  <th style="width:250px">Метка</th>
  <th>Действия</th>
 </tr>
 </thead><tbody></tbody>
 <tr id='notfounded' style='display:none;'>
  <td class="center" colspan='10'>
   Ничего не найдено.
  </td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center"></td>
  <td class="center"></td>  
  <td class="center"></td>  
  <td>
   <a href='' class="customs_photo_manage_photo_delete">удалить</a><br>
   <a href='' class="customs_photo_manage_photo_edit">править</a><br>
  </td>
 </tr>  
</table>

<br><a href='' id='customs_photo_manage_close'>Закрыть управление фотографиями</a><br>
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------<br>
</div>
