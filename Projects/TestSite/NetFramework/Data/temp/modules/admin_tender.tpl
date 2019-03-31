<script type='text/javascript'>
$(document).ready(function(){
    aj = new ajaxRequest();
    aj.load_form('config_form', function(){ $("#config1").html("<img src='/data/img/loading.gif'>"); }, 'config1');

    //$('select[name=lotBidRestrictions]').val('<{$conf.lotBidRestrictions}>');
    $('select[name=lotBidRevokeMode]').val('<{$conf.lotBidRevokeMode}>');
    $('select[name=lotBidRevokeSubscription]').val('<{$conf.lotBidRevokeSubscription}>');
    $('select[name=ETPState]').val('<{$conf.ETPState}>');
    
    $("input[name=ExtraTenderStartMethod]").click(function(){
        $("tr.ExtraTenderStartMethod").hide();
        $("tr.ExtraTenderStartMethod" + $("input[name=ExtraTenderStartMethod]:checked").val()).show();
    });
    $("input[name=ExtraTenderStartMethod][value='<{$conf.ExtraTenderStartMethod}>']").attr('checked', true).click();
});

</script>
<br><hr><br>
<form action='/admin/madmin/adminmenu/modconfigsave/<{$mod}>' method='post' id='config_form'>
<table width='100%'>
 <tr>
  <td>Сколько дней следует показывать лоты в разделе "Новые лоты":</td>
  <td><input type='text' name='newLotsDays' value='<{$conf.newLotsDays}>'></td>
 </tr>
 <tr>
  <td>Ключ для обмена даными с ПО "Навигатор":</td>
  <td><input type='text' name='navigatorKey' value='<{$conf.navigatorKey}>'></td>
 </tr>
 <tr>
  <td>Почтовый адрес для рассылки сообщений в случае отмены торгов на ЭТП:</td>
  <td><input type='text' name='lotStateFailedMail' value='<{$conf.lotStateFailedMail}>'></td>
 </tr>
 <{*
 <tr>
  <td>Режим осуществления ставок:</td>
  <td>
   <select name='lotBidRestrictions'>
    <option value='0'>Предыдущая ставка может быть любой</option>
    <option value='1'>Предыдущая ставка не может быть больше предыдущей</option>
   </select>
  </td>
 </tr>*}>
 <tr>
  <td>Режим отмены ставок в лоте:</td>
  <td>
   <select name='lotBidRevokeMode'>
    <option value='0'>Ставки нельзя отменять</option>
    <option value='1'>Если нет более свежих ставок от других поставщиков</option>
    <option value='2'>В любой момент</option>
    <option value='3'>Только последнюю ставку</option>
   </select>
  </td>
 </tr>
 <tr>
  <td>Рассылка в случае отмены ставки:</td>
  <td>
   <select name='lotBidRevokeSubscription'>
    <option value='0'>Не выбрано</option>
    <{foreach from=$subscriptions item=ad key=id}><option value='<{$id}>'><{$ad.name}></option><{/foreach}>   
   </select>
  </td>
 </tr>
 <tr>
  <td>Состояние электронной торговой площадки:</td>
  <td>
   <select name='ETPState'>
    <option value='0'>Включено и работает</option>
    <option value='1'>Остановлено</option>
   </select>
  </td>
 </tr>
 <tr>
  <td>Сообщение в случае приостановки работы ЭТП:</td>
  <td>
   <textarea name="ETPStateMessage" cols="40" rows="5"><{$conf.ETPStateMessage}></textarea>
  </td>
 </tr>
 <tr>
  <td>Длительность дополнительных торгов (в секундах):</td>
  <td><input type='text' name='ExtraTenderTimeLimit' value='<{$conf.ExtraTenderTimeLimit}>'></td>
 </tr>
 <tr>
  <td>Метод запуска дополнительных торгов:</td>
  <td>
   <input type='radio' name='ExtraTenderStartMethod' value='1'>&nbsp;№1. В ближайшие полчаса после включения доп. торгов.<br>
   <small>Например, если кнопка "Доп. торги" в лоте была нажата в 17:04, то торги будут запущены в 17:30. Если нажата в 17:43, то будут запущены в 18:00</small><br><br>
   <input type='radio' name='ExtraTenderStartMethod' value='2'>&nbsp;№2. Немедленно.<br>
   <small>Сразу же после нажатия кнопки "Доп. торги"</small><br><br>
   <input type='radio' name='ExtraTenderStartMethod' value='3'>&nbsp;№3. Через N минут после нажатия кнопки "Доп. торги".<br>
   <small>Таймаут запуска торгов задается в поле ниже.</small><br><br>
  </td>
 </tr>
 <tr class='ExtraTenderStartMethod ExtraTenderStartMethod3'>
  <td>Задержка запуска дополнительных торгов (в секундах) для 3 способа:</td>
  <td><input type='text' name='ExtraTenderStartTimeout' value='<{$conf.ExtraTenderStartTimeout}>'></td>
 </tr>
</table> 
<input type='submit' value='Сохранить'>
</form>
<div id='config1'></div>