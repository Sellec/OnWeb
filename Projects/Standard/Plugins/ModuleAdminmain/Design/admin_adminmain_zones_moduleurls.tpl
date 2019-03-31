<script type='text/javascript'>
try {
if ( typeof(mURLArray) != 'undefined' ) {delete mURLArray;}
mURLArray = new Array();
<{foreach from=$data_urls item=ad key=id}>
mURLArray["<{$ad.id}>"] = {'name':"<{$ad.name|java_string}>",'selected':<{$ad.selected}>};
<{/foreach}>
update_urls();
} catch(err) { alert(err); }

$(document).ready(function(){
    $('input[name=module]').val("<{$module|java_string}>");
});
</script>
