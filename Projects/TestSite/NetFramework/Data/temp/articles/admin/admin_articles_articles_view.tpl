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
    
    changeTitle("Просмотр статьи: <{$data.name|java_string}>");
});

</script>

<h2><a href='/admin/madmin/@Module.Name/cats/<{$data.category}>' class='this_view'><{$data.articles_category_name}></a> &raquo; <{$data.name}></h2>
<p><{$data.text}></p>
