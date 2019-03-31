<{if isset($fields) && is_array($fields)}>
<{foreach from=$fields item=ad key=id}>
 <{if $ad.field_type == 1 || $ad.field_type == 5 || $ad.field_type == 7 || $ad.field_type == 9}>
 <tr>
  <td width="175"><{$ad.field_name}>:</td>
  <td><{$ad.field_value}> <{$ad.NameEnding}></td>
 </tr>
 <{elseif $ad.field_type == 2 || $ad.field_type == 3 || $ad.field_type == 8}>
 <tr>
  <td width="175"><{$ad.field_name}>:</td>
  <td>
   <{foreach from=$ad.field_data item=ad2 key=id2}>
    <{if $ad2.value|in_array:$ad.field_value}> <{$ad2.text}> <{$ad.NameEnding}> <{/if}>
   <{/foreach}>
  </td>
 </tr>
 <{elseif $ad.field_type == 4}>
 <tr class="tcapt">
  <td colspan='2'><{$ad.field_name}></td>
 </tr>
 <{elseif $ad.field_type == 6}>
 <tr>
  <td width="175"><{$ad.field_name}>:</td>
  <td>
   <{foreach from=$ad.field_data item=ad2 key=id2}>
    <{if $id2|in_array:$ad.field_value}> <{$ad2.name}> <{$ad.NameEnding}> <{/if}>
   <{/foreach}>
  </td>
 </tr>
 <{/if}>
<{/foreach}>
<{/if}>
