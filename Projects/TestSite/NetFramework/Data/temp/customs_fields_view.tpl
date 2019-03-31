<{foreach from=$fields item=ad key=id name=fd}>
<{assign var=zebra value=$smarty.foreach.fd.iteration%2}> 
<{if $ad.type == 1 || $ad.type == 5 || $ad.type == 7 || $ad.type == 9}>
 <{if strlen($ad.value)>0 && $ad.value != "0"}><tr<{if $zebra==0}> class="zebra"<{/if}>>
  <td width="175" class="tabletitle"><{$ad.name}>:</td>
  <td><{$ad.value}></td>
 </tr><{/if}>
 <{elseif $ad.type == 2 || $ad.type == 3 || $ad.type == 8}>
 <{if is_array($ad.value) && $ad.data[$ad.value[0]].text != "Не указано"}><tr<{if $zebra==0}> class="zebra"<{/if}>>
  <td width="175" class="tabletitle"><{$ad.name}>:</td>
  <td>
   <{foreach from=$ad.data item=ad2 key=id2}>
    <{if $ad2.value|in_array:$ad.value}> <{$ad2.text}> <{/if}>
   <{/foreach}>
  </td>
 </tr><{/if}>
 <{elseif $ad.type == 11}>
 <tr<{if $zebra==0}> class="zebra"<{/if}>>
  <td width="175" class="tabletitle"><{$ad.name}>:</td>
  <td><{foreach from=$ad.values item=ad2 key=id2}> <{$ad2}><br> <{/foreach}></td>
 </tr>
 <{elseif $ad.type == 4}>
 <tr class="tcapt">
  <td colspan='2'><{$ad.name}></td>
 </tr>
 <{elseif $ad.type == 6}>
 <tr<{if $zebra==0}> class="zebra"<{/if}>>
  <td width="175" class="tabletitle"><{$ad.name}>:</td>
  <td>
   <{foreach from=$ad.data item=ad2 key=id2}>
    <{if $id2|in_array:$ad.value}> <{$ad2.name}> <{/if}>
   <{/foreach}>
  </td>
 </tr>
 <{/if}>
<{/foreach}>
