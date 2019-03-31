<script type='text/javascript'>
$(document).ready(function() {
    $("#block").hide();
    try {                         
        changeTitle('Просмотр информации по phpinfo');
    } catch(err) { alert(err); }
    
});
</script>
<h2>Информация о php</h2>
<{$code}>
