<script type='text/javascript'>
function configsavemodule()
{
	return true;
}

$(document).ready(function(){
	aj = new ajaxRequest();
	aj.load_form('config_news_form', configsavemodule, 'config');
	
	CKEDITOR.replace( 'vacancy_index', {filebrowserUploadUrl : '/admin/madmin/fm/upload2/userfs', removePlugins: 'save'} );
	$('#save_func_').click(function(){
		CKEDITOR.instances.vacancy_index.updateElement();
	});
});
</script>
<form action='/admin/madmin/adminmenu/modconfigsave/<{$mod}>' method='post' id='config_news_form'>
<br><hr><br>
<table width='100%'>
 <tr>
  <td>
   Количество показываемых на главной записей:
  </td>
  <td>
   <input type='text' name='show_defaultnum' value='<{$conf.show_defaultnum}>'>
  </td>
 </tr>
 <tr>
  <td>
   Количество записей, показываемых на одной странице:
  </td>
  <td>
   <input type='text' size='100' name='show_categorynum' value='<{$conf.show_categorynum}>'>
  </td>
 </tr>
 <tr>
  <td>
   Приветственный текст на странице вакансий.
  </td>
  <td>
   <textarea name='vacancy_index' id='vacancy_index' rows='5' cols='60'><{$conf.vacancy_index}></textarea>
  </td>
 </tr>
   
   
 
</table> 
<input type='submit' id="save_func_" value='Сохранить'>
</form>