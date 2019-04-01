<script type='text/javascript'>
function configsavemodule()
{
    return true;
}

$(document).ready(function(){
    aj = new ajaxRequest();
    aj.load_form('config_goods_form',configsavemodule,'config');

    aj2 = new ajaxRequest();
    aj2.load_form('file_upload',null,'banner_progress');

});

// Execute all the scripts inside of the newly-injected HTML
$("script", self).each(function(){
    eval.call(window,this.text || this.textContent || this.innerHTML || "");
});

</script>

<br /><br />
<form action='/admin/madmin/@Module.UrlName/modconfigsave/<{$mod}>' method='post' id='config_goods_form'>
<table width='100%' id='modules_table'>
 <tr>
  <td style="width:250px;">
   Товаров на главной странице:
  </td>
  <td>
   <input type='text' name='show_defaultnum' value='<{$conf.show_defaultnum}>'>
  </td>
 </tr>
 <tr>
  <td style="width:250px;">
   Товаров, на одной странице в категории:
  </td>
  <td>
   <input type='text' name='show_categorynum' value='<{$conf.show_categorynum}>'>
  </td>
 </tr>
 <tr>
  <td>
   Максимальный размер фотографии (в кб):
  </td>
  <td>
   <input type='text' size='100' name='photomaxsize' value='<{$conf.photomaxsize}>'>
  </td>
 </tr>
 <tr style="display:none;">
  <td>
   Показывать количество товара на складе:
  </td>
  <td>
   <select name='show_skladcount'>
    <option value='1' <{if $conf.show_skladcount == 1}>selected<{/if}>>Да</option>
    <option value='0' <{if $conf.show_skladcount == 0}>selected<{/if}>>Нет</option>
   </select>
  </td>
 </tr>
</table> 
<input type='submit' value='Сохранить'>
</form>
