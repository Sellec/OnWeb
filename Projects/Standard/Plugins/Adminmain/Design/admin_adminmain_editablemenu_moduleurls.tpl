<script type='text/javascript'>
try {
if ( typeof(mURLArray) != 'undefined' ) {delete mURLArray;}
mURLArray = new Array();
<{foreach from=$data_urls item=ad key=id}>
mURLArray["<{$ad.url|java_string}>"] = "<{$ad.text|java_string}>";
<{/foreach}>
update_urls();
} catch(err) { alert(err); }
</script>
