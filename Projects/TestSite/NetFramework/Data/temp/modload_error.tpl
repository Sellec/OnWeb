<{extends 'baseCommon.tpl'}>

<{block 'title'}>Ошибка загрузки модуля<{/block}>

<{block 'body'}>
<h1>Ошибка загрузки модуля</h1>
<p>Ошибка при загрузке модуля '<{$moduleName|htmlspecialchars}>': <{$error}></p>

<{if $tryAuthorize}>
  <div id="enter_reg">
      <div id="er_title"><a href="/login" title="" class="er_enter er_active">Вход в систему</a> <a href="/reg" title="" class="er_reg">Регистрация</a></div>
      <form action='/login/login' method='post' id="enter_login">
       <div><label for="login">Эл. почта:</label><input name='login' type='text' class='itext' /></div>
       <div><label for="pass">Пароль:</label><input name='pass' type='password' class='itext' /></div>
       <div><input type='submit' name="login_enter" value='Войти' class='isubmit' /></div>
       <div class="add_links"><a href="/reg/remind" title="Восстановление забытого пароля">Забыли пароль?</a> | <a href="/reg" title="">Регистрация</a></div>
      </form>
  </div>
<{/if}>

<{/block}>
