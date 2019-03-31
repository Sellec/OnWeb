 <div id="hot_search">
  <div id="search_top">
   <span>&nbsp; Поиск по сайту</span>
   <form action="http://yandex.ru/sitesearch" method="get" id="ya_search">
    <label for="text_search">Я ищу...</label>
    <input type="hidden" name="searchid" value="159613"/><input name="text" id="text_search"/>
    <input type="submit" value="" id="subm_search"/>
   </form>
  </div>
  
  <{if isset($zone_data)}>
  
  <{getBannerData zone_id=$zone_data.zone_id assign=menu_banner sizex=180 sizey=80}>
  <ul id="top_adv_block">
  <{foreach from=$menu_banner item=ad key=id name=banrs}>
   <{if $smarty.foreach.banrs.first}><li class="bfirst"><{else}><li><{/if}>
   <a <{if $ad.type==1 && $ad.url|strlen>0}>
       <{if $ad.url|substr:0:1 == "/"}>href="<{$ad.url|replace:'&':'&amp;'}>" <{else}>
       href="http://<{$ad.url|replace:'http://':''}>" target="_blank"<{/if}>
  <{elseif $ad.type==1 && $ad.url|strlen==0}> href="#" 
  <{else}>
   href="/actual/<{$ad.id}>"
  <{/if}> title="<{$ad.name|replace:'"':''}>"><img src="/<{$ad.banner_image}>" alt="<{$ad.name|re_quote}>" /></a></li>
  <{/foreach}>
  </ul>

  <{else}>
  
  <{getCustom type=getBannerData id=1 assign=menu_banner sizex=180 sizey=80}>
  <ul id="top_adv_block">
  <{foreach from=$menu_banner item=ad key=id name=banrs}>
   <{if $smarty.foreach.banrs.first}><li class="bfirst"><{else}><li><{/if}>
   <a <{if $ad.type==1 && $ad.url|strlen>0}>
       <{if $ad.url|substr:0:1 == "/"}>href="<{$ad.url|replace:'&':'&amp;'}>" <{else}>
       href="http://<{$ad.url|replace:'http://':''}>" target="_blank"<{/if}>
  <{elseif $ad.type==1 && $ad.url|strlen==0}> href="#" 
  <{else}>
   href="/actual/<{$ad.id}>"
  <{/if}> title="<{$ad.name|replace:'"':''}>"><img src="/<{$ad.banner_image}>" alt="<{$ad.name|re_quote}>" /></a></li>
  <{/foreach}>
  </ul>
  <{/if}> 
  
  <{*<div id="chat_top">
   <script type="text/javascript" charset="UTF-8" src="http://goeg.chatovod.ru/widget/mini.js?width=215&popup=1"></script>
  </div>*}>
 </div>
 <div class="wrapper"></div>