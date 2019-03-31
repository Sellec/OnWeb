<script type='text/javascript'>
$('#user_bar_inner form').submit(function(){
    try 
    { 
                
    } 
    catch(err) {}
    
    e.preventDefault();
})
</script>
<div id="user_bar">
 <div id="user_bar_inner">
  <form action='' onsubmit='log_call(this.parentNode);return false;'>
  <ul id="log_in" class="unauthorized">
   <li>Авторизация&nbsp;</li>
   <li><label for="login_login">Логин</label><input type="text" id='login_login' name='loginn' class="inptext" /></li>
   <li><label for="login_pass">Пароль</label><input type="password" id='login_pass' name='pass' class="inptext" /></li>
   <li><input type="submit" value="&nbsp;Войти&nbsp;" class="inpbut" /></li>
   <li><a href="/reg/remind" title="Восстановление забытого пароля">Забыли пароль?</a></li>
   <li class="top_reg"><a href="/reg" title="">Регистрация на сайте</a></li>
  </ul>
  </form>
  <ul id="user_panel" class="authorized">
   <li><img src="/data/img/man.png" alt="" /><{$UserManager->getLogin()}></li>
   <li><a href='/customer' title='Личный кабинет' class="mlogin">Настройки</a></li>
   <{if isset($is_adm) && $is_adm}><li id="admin"><a href="/admin" title="Панель администратора" class="mlogin" target="_blank" id="admin_link"><strong>Управление</strong></a></li><{/if}>
   <li><img src="/data/img/lock.png" alt="" /><a href='' class="log_exit" title='Завершить работу с вашим профилем'>Выйти</a></li>
  </ul>
 </div>
</div>
<div id="top_shadow"></div>

<div id='result_login'></div>
<script type="text/javascript">
 updateUserPanel("<{$IsAuthorized}>", "<{$IsAdminPanel}>"); 
</script>
