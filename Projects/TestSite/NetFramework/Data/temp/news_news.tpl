<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
    <div class="subpath"><a href="/" title="Главная страница">Главная</a> &rarr; <{if $data.category > 1}><a href="/@Module.UrlName" title="Новости">Новости</a> &rarr; <{/if}><a href='/@Module.UrlName/cat/<{$data.category}>' title="<{$data.news_category_name|re_quote}>"><{$data.news_category_name}></a> &rarr; <{$data.name}></div>
    <h1><{$data.name}></h1>
    <div class="pages">
     <{if isset($data.photo[0])}>
     <div class="news_topphoto"><a href="/<{$data.photo[0].photo_value}>" title="<{$data.name|re_quote}>" class="onlightbox" rel="gallery"><img src="/<{$data.photo[0].photo_preview_value}>" alt="<{$data.name|re_quote}>" /></a></div>
     <{/if}>
    <{$data.text}></div>
    
    <{if isset($data.photo) && $data.photo|@count > 1}>
     <div></div>
     <ul id="news_preview">
      <{foreach from=$data.photo item=ad key=id name=photos}>
       <{if !$smarty.foreach.photos.first}><li><a href="/<{$ad.photo_value}>" title="<{if isset($ad.photo_comment) && $ad.photo_comment != ""}><{$ad.photo_comment|re_quote}><{else}><{$data.name|re_quote}><{/if}>" class="onlightbox" rel="gallery"><img src="/<{$ad.photo_preview_value}>" alt="<{if isset($ad.photo_comment) && $ad.photo_comment != ""}><{$ad.photo_comment|re_quote}><{else}><{$data.name|re_quote}><{/if}>" /></a></li><{/if}>
      <{/foreach}>
     </ul><div class="wrapper"></div>
    <{/if}>

    <div class="buttons">
     <small><{if $smarty.now|strftime:"%d" == $data.date|strftime:"%d" && $smarty.now|strftime:"%m" == $data.date|strftime:"%m"}>Сегодня, <{$data.date|strftime:"%H:%M"}><{else}><{$data.date|strftime:"%e %b %Y"}><{/if}></small>
     <script type="text/javascript" src="//yandex.st/share/share.js" charset="utf-8"></script>
     <div class="yashare-auto-init" data-yashareType="button" data-yashareQuickServices="yaru,vkontakte,facebook,twitter,odnoklassniki,moimir,lj"></div> 
    </div>
    
<{/block}>
