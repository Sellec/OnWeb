<{foreach from=$local_data item=ad key=id}>
<{if $ad.type == 'url'}>
 <{urls type=check_url url=$ad.data assign=ret}>
 <li<{if $ret==1}> class='current<{if isset($ad.data.class)}> <{$ad.data.class}><{/if}>'<{else}><{if isset($ad.data.class)}> class='<{$ad.data.class}>'<{/if}><{/if}>><a href="<{$ad.data.url|replace:'&':'&amp;'}>" <{if isset($addData.class)}> class="<{$addData.class}>"<{/if}>title=""><{if isset($addData.wrap)}><<{$addData.wrap}>><{/if}><{$ad.data.text}><{if isset($addData.wrap)}></<{$addData.wrap}>><{/if}></a></li>
<{elseif $ad.type == 'sub'}>
 <li><a href="" title="" class="noclick"><{$ad.data.text}></a>
  <{if isset($ad.data.data) && $ad.data.data|@count >0 }><ul><{include file="editablemenu_sub.tpl" local_data=$ad.data.data}></ul><{/if}>
 </li>
<{elseif $ad.type == 'suburl'}>

 <{urls type=check_url url=$ad.data assign=ret}>
 <li<{if $ret==1}> class='current<{if isset($ad.data.class)}> <{$ad.data.class}><{/if}>'<{else}><{if isset($ad.data.class)}> class='<{$ad.data.class}>'<{/if}><{/if}>><a href="<{$ad.data.url|replace:'&':'&amp;'}>" title=""><{$ad.data.text}></a>

  <{if isset($ad.data.data) && $ad.data.data|@count >0 }><ul><{include file="editablemenu_sub.tpl" local_data=$ad.data.data}></ul><{/if}>
 </li>
<{/if}>
<{/foreach}>

