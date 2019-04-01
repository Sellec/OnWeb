<script type='text/javascript'>
$(document).ready(function(){
    $("#block").hide();
    
    changeTitle('Множественное добавление видеофайлов');

    aj = new ajaxRequest();
    aj.load_form('form_ae',null,'video_result');
    $("#load_more").click(function(){$("#form_ae").resetForm();return false;})
});

</script>
<{include file="admin/admin_gallery_manage_catsgals.tpl"}>


<form action='/admin/madmin/@Module.UrlName/video_add_list_save' method='post' id='form_ae'>
<h2>Добавление видеофайлов</h2>
<table width='900' id='table_results'>    
 <tr>
  <th width='200'>Название</th>
  <th width='400'>Описание</th>  
  <th width='200'>Превью</th>  
  <th width='200'>Файл</th>  
  <th width='200'>Галерея</th>  
 </tr>    

<{section name = video start = 1 loop = 6 step = 1}>
 <tr>
  <td><input type='text' name='video_name[]' size='40' maxlength='200' value=''></td>
  <td><textarea rows='4' cols='20' name='video_descr[]'></textarea></td>
  <td><input type='file' name='video_file_preview[]' size='10' value=''></td>
  <td><input type='file' name='video_file[]' size='10' value=''></td>
  <td>
    <select name='video_gall[]'>
<{foreach from=$galls_data item=ad key=id}>
     <option value='<{$id}>'><{$ad.name}></option>
<{/foreach}>
    </select>
  </td>
 </tr>
<{/section}>

</table>

<input type='submit' value='Добавить' id="b_add"> <div id='video_result'></div>
