<script type='text/javascript'>
$(document).ready(function(){
    $("#block").hide();

    changeTitle('Управление поисковыми модулями');
    $(".i_adminmenu ul").show();               

    $('input.editable_modules').change(function(){
    try {
        var aj = new ajaxRequest();
        aj.setPOST('module_id',$(this).attr('name'));
        aj.setPOST('status',$(this).attr('value'));
        aj.load('/admin/madmin/@Module.UrlName/engines_edit_save','div_result');        
    } catch(err) {alert(err);}
    });
    
});
</script>
<h2>Управление поисковыми модулями</h2>

<div id="div_result" style="text-align: center;border:solid 1px red;background: #fff1f1;"></div>
<table id='table_results' class='tablesorter'>
<thead>
 <tr>
  <th style="width:50px">Название</th>
  <th style="width:250px">Имя файла</th>
  <th style="width:100px">Опции</th>
 </tr>
 </thead><tbody>
 <{foreach from=$engines item=ad key=id}>
 <tr>
  <td class="center"><{$ad.m_name}></td>
  <td class="center"><{$ad.m_filename}></td>  
  <td class="center">
   <input type='radio' class='editable_modules' name='<{$id}>' value='1' <{if isset($ad.engine_status)&&$ad.engine_status==1}>checked<{/if}>>&nbsp;Включено<br>
   <input type='radio' class='editable_modules' name='<{$id}>' value='0' <{if !isset($ad.engine_status)||$ad.engine_status==0}>checked<{/if}>>&nbsp;Выключено<br>
  </td>  
 </tr>  
 <{/foreach}>
 </tbody> 
</table>
   
