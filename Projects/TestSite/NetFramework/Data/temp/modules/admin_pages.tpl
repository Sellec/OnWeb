<script type='text/javascript'>
function configsavemodule()
{
    return true;
}

$(document).ready(function(){
    aj = new ajaxRequest();
    aj.load_form('config_form',configsavemodule,'config');

});

// Execute all the scripts inside of the newly-injected HTML
$("script", self).each(function(){
    eval.call(window,this.text || this.textContent || this.innerHTML || "");
});

</script>
<br><hr><br>
<form action='/admin/madmin/adminmenu/modconfigsave/<{$mod}>' method='post' id='config_form'>
<table width='100%'>
 <tr>
  <td>
   Количество страниц, показываемых на одной странице в категории:
  </td>
  <td>
   <input type='text' name='show_categorynum' value='<{$conf.show_categorynum}>'>
  </td>
 </tr>
 <tr>
  <td>
   Количество статей, показываемых на главной странице:
  </td>
  <td>
   <input type='text' name='defarticles' value='<{$conf.defarticles}>'>
  </td>
 </tr>
</table> 
<input type='submit' value='Сохранить'>
</form>
