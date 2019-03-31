<script type='text/javascript'>
$(document).ready(function() {
    $('a.this_view').click(function(){
        try {
            var aj = new ajaxRequest();
            aj.load($(this).attr('href'),'cmain');
            return false;    
        } catch (err)
        {
            alert(err);
        }
        return false;
    });
    
    changeTitle("Просмотр страницы: <{$data.name|java_string}>");
});

</script>

<h2><{$data.name}></h2>
<div style="width:650px;margin-bottom:15px;"><{$data.body}></div>