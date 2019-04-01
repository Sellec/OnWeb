<script type='text/javascript'>
$(document).ready(function(){
    try {
    $("#block").hide();
    
    /*aj = new ajaxRequest();
    aj.load_form('form_ae',null,'cmain');*/
    
    changeTitle('Просмотр списка записей');
            
    aj = new ajaxRequest();
    aj.userOnLoad = function(){endAnim($("#action_result"))};
    aj.load_form('form_save',null,'action_result');
    stAnim();

    CKEDITOR.replace( 'text_block',{removePlugins: 'save'} );
    CKEDITOR.replace( 'text_block_2',{removePlugins: 'save'} );
    
    $('#save_func').click(function(){
        CKEDITOR.instances.text_block.updateElement();
        CKEDITOR.instances.text_block_2.updateElement();
    });

    
    } catch(err) {alert(err);}
});
</script>

<form action='/admin/mnadmin/@Module.UrlName/vacancy' method='post' id='form_ae'>
<h2>Просмотр вакансии</h2>

Выберите категорию вакансии:<br>

<select name='cat'>
 <option value='-1'>Все категории</option>
<{foreach from=$cats_data item=ad key=id}>
 <option value='<{$id}>'><{$ad}></option>
<{/foreach}>
</select>

<input type='submit' value='Перейти в просмотр' /> <img src="/data/img/loading.gif" alt="" id="loading_img" style="display:none" />
</form><br /><br />

<h2>Настройки раздела:</h2>
<p><strong>Текст на главной странице вакансий:</strong></p>
<form action='/admin/madmin/@Module.UrlName/@Module.UrlName_index_save' method='post' id='form_save'>
<textarea name='body' rows='10' cols='10' id="text_block"><{$data.index}></textarea></p>

<p><strong>Текст под списком вакансий:</strong></p>
<textarea name='body_warranty' rows='10' cols='10' id="text_block_2"><{$data.warranty}></textarea></p>

<button id='save_func'>Сохранить</button> <img src="/data/img/design/loading.gif" alt="" id="loading_img">
</form>