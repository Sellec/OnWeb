<script type="text/javascript" src='/data/js/ajax.js'></script> 
<script type='text/javascript'>
$(document).ready(function(){
    $("#block").hide();
   
    changeTitle('Список языков');
    $(".i_adminmenu ul").show();
});
</script>

<table width='100%' id='items_results'>
 <tr>
  <th colspan='2'>
   Список языков, установленных в системе
  </th>
 </tr>
 <tr>
  <td width='200'>
   Имя файла языка:
  </td>
  <td width='500'>
   Название языкового модуля:
  </td>
 </tr>
 
 <{foreach from=$data_langs item=name key=file}>
 <tr>
  <td>
   <{$file}>
  </td>
  <td>
   <{$name}>
  </td>
 </tr>
 <{/foreach}>
 
</table> 
