<script type='text/javascript'>
$(document).ready(function() {
    $("#block").hide();
    try {                         
        changeTitle('Просмотр лог-файла ошибок MySQL');
    } catch(err) { alert(err); }
    
});
</script>
<h2>Ошибки MySQL</h2>
<div>
<textarea cols=150 rows=20 readonly style="overflow:auto"><{$data}></textarea>
</div>
