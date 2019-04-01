<{extends "baseCommon.tpl"}>
<{block 'title'}>Подписка на рассылки<{/block}>

<{block 'body'}>
<script type='text/javascript'>
$(function() {
    try {
        var divsettings = {tl:{radius:15},tr:{radius:15},bl:{radius:15},br:{radius:15},antiAlias: true}
        var ddivObj = document.getElementById("news_div");
        curvyCorners(divsettings, ddivObj);        
        
        aj = new ajaxRequest();
        aj.load_form('subscribe_form',null,'subscription_result');
        
        $('a#reload_cap').click(function(){
            alert('Перезагрузка картинки...');

            return false;
        });
    } catch(err) { alert(err); } 
});
</script>

 <table id="structure">
  <tr>
   <td id="lefttd">
    <{include file="menu_catalog.tpl"}>
    <{include file="menu_accessories.tpl"}>
   </td>
   <td id="righttd">
   <{include file="banner_block.tpl"}>
   <{include file="search_block.tpl"}>   
   <h1><img src="/data/img/caption.png" alt="">Подписка на рассылки:</h1>
   <div id="news_div" style="margin-top:0;">
    <br>
   <p>Выберите рассылки, на которые вы собираетесь подписаться, и введите свой email-адрес и код проверки с картинки.</p>
   <p><strong>Доступные рассылки:</strong></p>
   <p color='red'><{$result}></p>
   <form action='/@Module.UrlName/subscribe' method='post' id="subscribe_form">
    <table>
     <tr>
      <td>
       <select name='subscribe[]' multiple size='10'>
       <{foreach from=$data_subscriptions item=ad key=id}>
        <option value='<{$ad.id}>'><{$ad.name}></option>
       <{/foreach}>
       </select><br>
      </td>
      <td>
       <div><label for="captcha">Email:</label>
       <input type='text' name='email' size='20'></div>
       <div><label for="captcha">Код проверки:</label>
       <input type='text' name='c_num' size='20'></div>
       <div><img src='/captch/code' id="captcha_code"></div>
      </td>
     </tr>
     <tr>
      <td colspan="2">
       <input type='submit' value='Подписаться'>
       <div id='subscription_result'></div>
      </td>
     </tr>
    </table>
   </form>
   </div>
   </td>
  </tr>
 </table>
<{/block}>