
<script type='text/javascript'>
try {   
mPhotosArray = new Array();
<{foreach from=$photo_list item=ad key=id}>mPhotosArray[<{$id}>] = { "id":"<{$id}>", "photo_value":"<{$ad.photo_value}>", "photo_width": <{$ad.photo_width}>, "photo_height": <{$ad.photo_height}>, "photo_label":"<{$ad.photo_label|java_string}>" };<{/foreach}>
<{foreach from=$photo_labels_list item=_ad key=_id}><{foreach from=$_ad item=ad key=id}>mPhotosArray[<{$id}>] = { "id":"<{$id}>", "photo_value":"<{$ad.photo_value}>", "photo_width": <{$ad.photo_width}>, "photo_height": <{$ad.photo_height}>, "photo_label":"<{$ad.photo_label|java_string}>" };<{/foreach}><{/foreach}>
} catch (err) { alert(err); }

$(document).ready(function(){
    if ( typeof(customs_photo_manage_refreshlist_end) != 'undefined' ) customs_photo_manage_refreshlist_end();
});
</script>
