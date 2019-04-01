<{extends "baseCommon.tpl"}>
<{block 'title'}>Страница не найдена!<{/block}>

<{block 'body'}>
      <div class="subpath"><a href="/" title="Главная страница">Главная</a></div>
      <h1>Страница не найдена!</h1>
      <p>Запрошенный вами адрес содержит ошибку &mdash; пожалуйста, проверьте его и попробуйте еще раз.<br />
      Если ошибка повторяется, не поленитесь пожаловаться нам с помощью этой формы:</p>
      <form action='/@Module.UrlName/form_s' method='post' id='help_frm'>
       <input type='hidden' name="c_code" id='captch_c_code'>
       <div><label>ФИО:</label><br />
       <input type='text' size='25' name="person" id='form_person' /></div>
       <div><label>Ваша эл. почта: <span>*</span></label><br />
       <input type='text' size='25' name="email" id='form_email' /></div>
       <div><label>Выберите тему: <span>*</span></label><br />
        <select name="subj" id='form_subj'>
         <option value="4">Сообщение об ошибке</option>
         <option value="1">Предложение редакции</option>
         <option value="2">Вопрос от посетителя</option>
         <option value="3">Обращение от организации</option>
         <option value="5">Другое</option>
        </select>       
       </div>
       <div><label>Вопрос или предложение: <span>*</span></label><br />
       <textarea name="text" id='form_text' rows="5" cols="65"></textarea></div>
       <div><label>Код проверки: <span>*</span></label><br />
       <input type='text' name='c_num' id='captch_code' /><br /><img src='/captch/code' alt='' class='c_code'></div>
       <div><span>*</span> &mdash; поля, отмеченные звездочкой, обязательны для заполнения</div>
       <input type='submit' value='Отправить сообщение' /> <img src="/data/img/loading.gif" alt="Идет отправка сообщения" id="sending_message" />
      </form>
      <div id='result'></div>
<{/block}>