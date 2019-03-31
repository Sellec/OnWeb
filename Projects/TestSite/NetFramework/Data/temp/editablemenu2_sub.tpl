<{foreach from=$local_data item=ad key=id}>
 <{urls type=check_url url=$ad.url assign=ret}>
 <li<{if $ret==1}> class='current'<{/if}>><a href="<{$ad.url|replace:'&':'&amp;'}>" title=""> <{$ad.name}></a>

 <{if isset($ad.subs) && $ad.subs|@count >0 }><ul><{include file="editablemenu2_sub.tpl" local_data=$ad.subs}></ul><{/if}>
 </li>
<{/foreach}>

