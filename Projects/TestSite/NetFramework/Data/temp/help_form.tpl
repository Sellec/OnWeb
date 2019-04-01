<{extends "baseCommon.tpl"}>
<{block 'title'}>Обратная связь<{/block}>

<{block 'body'}>
<script type="text/javascript">
$(document).ready(function(){
    aj = new ajaxRequest();
    aj.load_form('ask_q',null,'resform');

    $(".field_add").click(function(){
        $(this).before("<input type='text' name='reg_counters[]' class='f_text reg_counters' value='' tabindex='5' /><br />");
        return false;
    })
});
</script>
     
      <h1><a name="mail"></a>Обратная связь</h1>
      <p><{getCustom type=config option=help_info}></p>
      <form action='/@Module.UrlName/form_s' method='post' id='ask_q'>
       <input type='hidden' name="c_code" id='captch_c_code'>
       <div><label>ФИО:</label><br />
       <input type='text' class='text' size='25' name="person" id='form_person' /></div>
       <div><label>Ваша эл. почта: <span>*</span></label><br />
       <input type='text' class='text' size='25' name="email" id='form_email' /></div>
       <div><label>Текст сообщения: <span>*</span></label><br />
       <textarea name="text" id='form_text'></textarea></div>
       <div style="display:none;"><label>Прикрепить файл:</label><br />
       <input type='file' size='25' name="file_upload[]" class='file_upload' /><br />
       <a href="#" title="" class="field_add">[+] Добавить еще файл</a></div>
       <div class="c_div"><label>Код: <span>*</span></label><br />
       <input type='text' class='text_num' name='c_num' />
       <img src='/captch/code' alt='' class='c_code' /></div>
       <input type='submit' value='Задать вопрос' class="subm" style="width:480px;" />
      </form>                        
      <div class="wrapper h10"></div>
      <div id='resform' style='display:none;'></div>
<{/block}>