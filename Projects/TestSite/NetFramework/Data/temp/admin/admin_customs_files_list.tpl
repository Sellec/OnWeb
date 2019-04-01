<script type='text/javascript'>
function customs_files_list_delete_result(result_id,result_text)
{
    if ( result_id == 0 ) alert(result_text);
    else {
        $('.customs_files_list_id_'+result_id).hide();
    }
}
$(document).ready(function(){
    $('a.customs_files_list_delete').click(function(){
        try {
        var data = $(this).parent().attr('class').match(/customs_files_list_id_(\d+)/i);
        if ( data == null ) { return false;}

        aj = new ajaxRequest();
        aj.load('/admin/madmin/@Module.UrlName/files_list_delete/'+data[1],'customs_files_list_delete_result');
        } catch(err) {alert(err);}
        return false;
    });
});
</script>

<div id='customs_files_list_delete_result' style='display:none;'></div>
Список прикрепленных файлов:<br>
<ul>
<{foreach from=$files_list item=ad key=id}>
<li class='customs_files_list_id_<{$id}>'><input class='customs_files_list_id_<{$id}>' type='text' readonly size='50' value='<{$ad.file_value}>'> (<{$ad.file_type}>) (<a href='' class='customs_files_list_delete'>Удалить</a>)
<{/foreach}>
</ul>
