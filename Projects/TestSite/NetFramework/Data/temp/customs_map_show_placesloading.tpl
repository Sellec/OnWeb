<script type='text/javascript'>
$(document).ready(function(){
try {
    var adding = "";
    var _tagLoadedData = new Array();
    <{foreach from=$data_items item=ad key=id}>
    _tagLoadedData[<{$id}>] = {"name":"<{$ad.name|java_string}>","description":"<{$ad.description|strip_tags|truncate:350|java_string}>","coord_x":<{$data_maps[$id].obj_coord_x}>,"coord_y":<{$data_maps[$id].obj_coord_y}>,"fields":{}};
    <{if isset($ad.Fields='') && $ad.Fields|@count >0}>
      <{foreach from=$ad.Fields item=ad2 key=id2}>
     <{if $ad2.field_type == 1 && $ad2.field_value != "" }>added = "<label><{$ad2.field_name|java_string}>:</label> <{$ad2.field_value|java_string}> </li>";
     <{elseif $ad2.field_type == 7 && $ad2.field_value != "" }>added = "<label><{$ad2.field_name|java_string}>:</label> <{$ad2.field_value|java_string}> </li>";
     <{elseif $ad2.field_type == 2 && $ad2.field_value != "" }>
      added = "<label><{$ad2.field_name|java_string}>:</label> ";
      <{foreach from=$ad2.field_data item=ad3 key=id3}><{if $ad3.value|in_array:$ad2.field_value}> added += "<{$ad3.text|java_string}>; "; <{/if}><{/foreach}>
      added += "<br />";
     <{elseif $ad2.field_type == 3 && $ad2.field_value != "" }>
      added = "<label><{$ad2.field_name}>:</label> ";
      <{foreach from=$ad2.field_data item=ad3 key=id3}><{if $ad3.value|in_array:$ad2.field_value}> added += "<{$ad3.text|java_string}>; "; <{/if}><{/foreach}>
      added += "<br />";
     <{elseif $ad2.field_type == 4 && $ad2.field_value != "" }>added += "<label><{$ad2.field_name}>:</label> <{if $ad2.field_name}>Есть<{/if}></li>";
     <{/if}>
    _tagLoadedData[<{$id}>]['fields'][<{$id2}>] = added;
    <{/foreach}>
    <{/if}>
    <{/foreach}>

    mapShowPlacesLoadingComplete(<{$data_id}>,_tagLoadedData);
} catch(err) {alert("0: "+err);}
});
</script>
