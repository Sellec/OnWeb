<{extends "baseCustomer.tpl"}>

<{block 'body'}>
<script type="text/javascript">
$(document).ready(function(){
    <{*$('form#datachangeform').requestJSON({
        before: function(){ $("#result").text(''); },
        after: function(result, message){ 
            $("#result").html(message.replace(/\n/g, '<br />'));  
            $("img#img_captch_code").attr('src', '/captch/code/'+Math.random());
        }
    });*}>
    
    //cuf_init('customer_info_edit','li',<{$fields|@jsobject}>);
    
    $(".gettop").click(function(){scroll(0,0);return false}); 
});
</script>
<div style="margin-top:20px;">
<form action='/@Module.UrlName/datas_save' method='post' id='datachangeform'>
    
             <h2>Данные аккаунта</h2>
             <div>
              <label for="email">Эл. почта: </label>
              <input type='text' class='itext input250' name='email' id='email' value='<{$data.email}>' tabindex='1' />
             </div>
             <div class="wrapper"></div>
             
             <div>
              <label>Комментарий: </label>
              <textarea class='itext form_textarea' name='Comment' rows='2' tabindex=''><{$data.Comment}></textarea>
             </div>
             
             <div class="form_half">
              <img src='/captch/code' alt='' id='img_captch_code' />
              <label for="captcha">Код проверки: </label>
              <input type='text' name='c_num' class='reg_num' id='' size='20' tabindex=''/><br />
             </div>

             <div class="form_half_r">
                <input type='submit' class='isubmit' name='confirm_b' id="confirm_b" value='&nbsp;Сохранить&nbsp;' tabindex='' />
             </div>
             <{*<p>Функция редактирования аккаунта недоступна</p>*}>

             <div id='result' style="width:450px;padding:10px 0 0 0;color:red;"></div>
</form>
</div>
<{/block}>
