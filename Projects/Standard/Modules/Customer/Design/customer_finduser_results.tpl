<script type='text/javascript'>
try { if (typeof(mFindUserObj)!='undefined'){
<{foreach from=$data item=ad key=id}>mFindUserObj.addUser(<{$id}>,'<{$ad.login}>');<{/foreach}>
mFindUserObj.callLoadHandler();
}} catch(err) {alert(err)};
</script>
