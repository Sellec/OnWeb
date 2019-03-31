<script type='text/javascript'>
$(document).ready(function(){
    aj = new ajaxRequest();
    aj.load_form('config_form',null,'config');
});
</script>
<form action='/admin/madmin/adminmenu/modconfigsave/<{$mod}>' method='post' id='config_form'>
<br><hr><br>
<table width='100%'>
 <tr>
  <td>Выберите фоновое изображение для проверочного числа:</td>
  <td><input type='text' name='conf_img' value='<{$conf.img}>'></td>
 </tr>
 <!-- <tr>
  <td>Выберите шрифт для проверочного числа:</td>
  <td><input type='text' name='conf_font' value='<{$conf.font}>'></td>
 </tr> -->
</table> 
<input type='submit' value='Сохранить'>
</form>
