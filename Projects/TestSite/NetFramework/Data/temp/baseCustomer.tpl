<{extends "baseCommon.tpl"}>
<{block 'title'}>Личный кабинет<{/block}>

<{block 'body'}>
 <div class="subpath"><a href="/" title="Главная страница">Главная</a></div>
 <h1>Личный кабинет</h1>
 
 <ul id='customer_panel'>
  <li><a href='/@Module.Name'>Общая информация</a></li>
  <li><a href='/@Module.Name/pchange' class='propanel'>Изменить пароль</a></li>
  <{*<li><a href='/@Module.Name/datas' class='propanel'>Личные данные</a></li>*}>
  <li><a href='/@Module.Name/subscribes' class='propanel'>Подписки</a></li>
 </ul>
 <div class="wrapper"></div>
 
 <div id="topdiv">
  <div id="profile_content">
   <{$smarty.block.child}>
  </div>
 </div>
<{/block}>
