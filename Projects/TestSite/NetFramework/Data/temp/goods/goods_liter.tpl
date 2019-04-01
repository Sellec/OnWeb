<{extends "baseCommon.tpl"}>
<{block 'title'}>Продукция на букву "<{$liter}>"<{/block}>

<{block 'body'}>
    <div class="subpath"><a href="/" title="Главная">Главная</a> / <a href="/@Module.UrlName" title="Каталог продукции">Каталог продукции</a> / Продукция на букву "<{$liter}>"</div>
    
    <h1>Каталог продукции</h1>
    
    <{if isset($data_liters) && $data_liters|@count>0}>
    <div id="top_liters">
    <{foreach from=$data_liters item=ad key=id name=liters}>
        <a href="/@Module.UrlName/liter/<{$ad.url}>" title="<{$ad.liter}>"><{$ad.liter}></a><{if !$smarty.foreach.liters.last}> | <{/if}>
    <{/foreach}></div>
    <{/if}>
    
    <{$result}>
    <{if isset($data_items)}>
    <{if $data_items|@count>0}>
    <ul id="items_category">
    <{foreach from=$data_items item=ad key=id name=data_items}>
    <li>
     <a name="prd_<{$id}>"></a>
     <div class="item_photo"><a href="/@Module.UrlName/item/<{$ad.id}>" title="<{$ad.name|re_quote}>" class="item_popup" style="background-image:url(/<{if isset($ad.photo.item_icon)}><{$ad.photo.item_icon}><{else}>data/img/nophoto.jpg<{/if}>)"></a></div>
     <a href="/@Module.UrlName/item/<{$ad.id}>" title="<{$ad.name|re_quote}>" class="ititle item_popup"><{$ad.name}></a>
     <p>Вид оболочки: <strong><{$ad.type}></strong></p>
     <div class="wrapper"></div>
     </li>
    <{/foreach}>
    </ul><{/if}>
    <{/if}>

    <div class="wrapper h10"></div>

<{/block}>