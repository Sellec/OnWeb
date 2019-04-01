<script type='text/javascript'>
$(document).ready(function(){
    try {
    $("#block").hide();
    aj = new ajaxRequest();
    aj.load_form('form_ae',null,'subscription_s_result');
    
    <{if $data.id == -1}>
    changeTitle('Добавление подписки');
    $('div#added_item_photo').hide();
    <{else}>
    changeTitle("Редактирование подписки: <{$data.name|java_string}>");
    <{/if}>
    getResultAnim($("#form_ae"),$("#subscription_s_result"));
    } catch(err) {alert(err);}
});
</script>

<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/add_save' method='post' id='form_ae'>
<{else}>
<form action='/admin/madmin/@Module.UrlName/edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>

<{if $data.name == ""}><h2>Добавление подписки</h2><{else}><h2>Редактирование подписки</h2><{/if}>

<table width='900' id="items_results" style="color:#000;">    
 <tr>
  <th colspan="2">Подписка</th>
 </tr>
 <tr>
  <td width="175">Название:</td>
  <td width="725"><input type="text" name="subscr_name" value="<{$data.name}>"></td>
 </tr>
 <tr>
  <td>Описание:</td>
  <td><textarea rows='5' cols='40' name='subscr_descr'><{$data.description}></textarea></td>
 </tr>
 <tr>
  <td>Статус:</td>
  <td>
    <select name='subscr_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно</option>
    </select>
  </td>
 </tr>
</table>

<div id='subscription_s_result' style="padding:0 0 10px 10px;font-size:14px;"></div>

<{if $data.id == -1}>
<input type='submit' value='Добавить'> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none">
<{else}>
<input type='submit' value='Сохранить'> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none">
<{/if}>
</form><br>

