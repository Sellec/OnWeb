<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
    <div class="subpath"><strong>Вы здесь:</strong> <a href="/" title="Главная">Главная</a><{if isset($data.parentlink)}><{$data.parentlink}><{/if}> &rarr; <{$data.name}></div>
    
    <h1><{$data.name}></h1>
    <div class="pages"><{$data.body}></div>
    
    <{if isset($data.photo) && isset($data.photo[0])}>
    <div class="pages_preview">
    <{foreach from=$data.photo item=ad key=id}>
     <div><a href="/<{$ad.photo_value}>" title="<{if isset($ad.photo_comment) && $ad.photo_comment != ""}><{$ad.photo_comment|re_quote}><{else}><{$data.name|re_quote}><{/if}>" rel="gallery"><img src="/<{$ad.photo_preview_value}>" alt="<{if isset($ad.photo_comment) && $ad.photo_comment != ""}><{$ad.photo_comment|re_quote}><{else}><{$data.name|re_quote}><{/if}>" /></a></div>
    <{/foreach}>
    </div>
    <div class="wrapper h10"></div>
    <{/if}>
    
<{/block}>