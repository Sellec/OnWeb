<{extends "baseCustomer.tpl"}>

<{block 'body'}>
<script type="text/javascript">
function check_form()
{
    try {
        if ( $("input[name='password_old']").val().length == 0 ) throw new Error('Старый пароль не может быть пустым!');
        if ( $("input[name='password_new']").val().length == 0 ) throw new Error('Новый пароль не может быть пустым!');
        if ( $("input[name='password_new2']").val().length == 0 ) throw new Error('Подтверждение пароля не может быть пустым!');
        if ( $("input[name='password_new']").val() != $("input[name='password_new2']").val() ) throw new Error('Пароль и подтверждение не совпадают!');
    } catch(err) { alert(err); return false; }
    return true;
}
$(function(){
    $('form#pchangeform').requestJSON({
        before: function(){ $("#result").text(''); },
        after: function(result, message){ $("#result").html(message); }
    });
});
</script>
<div style="margin-top:50px;"><form action='/@Module.UrlName/pchange2' method='post' id='pchangeform'>
  <div><label for="password_old">Старый пароль: <span>*</span></label><input type='password' name='password_old' /></div>
  <div><label for="password_new">Новый пароль:<span>*</span></label><input type='password' name='password_new' /></div>
  <div><label for="password_new2">Еще раз:<span>*</span></label><input type='password' name='password_new2' /></div>
  <div><label for="captch_num">Код проверки:<span>*</span></label><input type='text' name='captch_num' size='20' /></div>
  <div><img id='img_captch_code' src='/captch/code' name="ccode" alt="" /></div>
  <div><input type='submit' class="submit" value='Сменить пароль' /></div>
</form></div>
<div id='result'></div>
<{/block}>
