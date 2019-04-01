<script type='text/javascript'>
$(document).ready(function() {
    $("#block").hide();
    try {
        changeTitle('Загрузка файлов');
        
        /*aj = new ajaxRequest();
        aj.load_form('form_upload',null,'upload_result');*/

    } catch(err) { alert(err); }
    
});

function uploaded_file(origin_file,saved_file,path)
{
    alert('('+origin_file+')'+saved_file+' + '+path+' = '+(path+saved_file));
}

</script>
<{$result}>
<div id='upload_result'></div>

<h2>Загрузка файлов</h2>
<form action='/admin/mnadmin/@Module.UrlName/upload2' method='post' id='form_upload' enctype='multipart/form-data'>
Выберите папку, в которую следует загрузить файл:<br>
<select name='directory'>
<{foreach from=$dirs item=v key=k}>
 <option value='<{$v}>'><{$v}></option>
<{/foreach}>
</select><br><br>

Выберите файл для закачивания: <input type='file' name='uploadedFiles[]'><br>
<input type='submit' value='Загрузить'>
</form>
