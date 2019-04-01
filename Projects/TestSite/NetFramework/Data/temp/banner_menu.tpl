   <div id="subtop">
    <{if isset($data) & isset($data.name)}><{if $data.id == 1}>
    <ul id="submenu">
     <{getCustom type=editableMenu id=3}>
    </ul>
    <{/if}><{/if}>
    <div id="vip_banners">

   <{getCustom type=getBannerData id=2 assign=vip_banner}>
   <{foreach from=$vip_banner item=ad key=id}>
    <a <{if $ad.type==1}>href="<{$ad.url}>" target="_blank"<{else}>href="/@Module.UrlName/item/<{$ad.id}>"<{/if}> title=""><img src="/<{$ad.banner_image}>" alt=""></a>
   <{/foreach}>

    </div>
   </div>
   <h4 class="inr iePNG">Горячие предложения</h4>
   <div id="hot_offers">

   <{getCustom type=getBannerData id=3 assign=hot_banner count=10}>
   <{foreach from=$hot_banner item=ad key=id}>
    <a <{if $ad.type==1}>href="<{$ad.url}>" target="_blank"<{else}>href="/@Module.UrlName/item/<{$ad.id}>"<{/if}> title=""><{$ad.name}></a>
   <{/foreach}>

   </div>
   <h4 class="inr iePNG">Реклама</h4>
   <div id="banners">
   
   <{getCustom type=getBannerData id=1 assign=rekl_banner count=7}>
   <{foreach from=$rekl_banner item=ad key=id}>
    <a <{if $ad.type==1}>href="<{$ad.url}>" target="_blank"<{else}>href="/@Module.UrlName/item/<{$ad.id}>"<{/if}> title=""><img src="/<{$ad.banner_image}>" alt=""></a>
   <{/foreach}>

   </div>