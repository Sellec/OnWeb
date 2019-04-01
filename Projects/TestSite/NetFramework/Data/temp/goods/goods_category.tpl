<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
    <div class="subpath"><a href="/" title="Главная">Главная</a> / <a href="/@Module.UrlName" title="Каталог продукции">Каталог продукции</a><{if isset($data.parent)}> / <{$data.parent}><{/if}></div>
    
    <h1><{$data.name}></h1>
    
    <{if $data_cats|@count > 0}>
    <ul id="goods_category_in">
    <{foreach from=$data_cats item=ad key=id}>
        <li><a href="/@Module.UrlName/cat/<{$id}>" title="<{$ad.name|re_quote}>" class="a_title"><{$ad.name}></a></li>
    <{/foreach}>
    </ul><div class="wrapper"></div>
    <{/if}>
    
    <{if isset($data_items) && count($data_items)>0}>
    <ul id="items_category">
    <{foreach from=$data_items item=ad key=id name=data_items}>
    <li>
     <a name="prd_<{$id}>"></a>
     <div class="item_photo"><a href="/@Module.UrlName/item/<{$ad.id}>" title="<{$ad.name|re_quote}>" <{if $ad.photo.item_icon|strlen>0}>class="item_popup" style="background-image:url(/<{$ad.photo.item_icon}>);"<{else}>class="item_popup ip_none" style="background-image:url(/data/img/news_logo.gif);"<{/if}>></a></div>
     <a href="/@Module.UrlName/item/<{$ad.id}>" title="<{$ad.name|re_quote}>" class="ititle item_popup"><{$ad.name}></a>
     <p>Вид оболочки: <strong><{$ad.type}></strong></p>
     <div class="wrapper"></div>
     </li>
    <{/foreach}>
    </ul>
    <{/if}>
    
    <{if count($data_items)>0 && $pages.pages>1}>
    <div class="wrapper h10"></div>
    <ul id="pages">
     <li class="pg"><span>Страница:</span></li>
     <{if $pages.curpage > 1 && $pages.curpage <= $pages.pages}><li><a href="/@Module.UrlName/cat/<{$data.id}>/page-<{$pages.curpage-1}><{if isset($smarty.get.on)}>/<{$smarty.get.on}><{/if}><{if isset($smarty.get.sort)}>/<{$smarty.get.sort}><{/if}>" title="">&lt; Назад</a></li><{/if}>
     <{section name=pagessec start=0 loop=$pages.pages step=1}>
     <{assign var="i" value=$smarty.section.pagessec.index+1}>
     <{if $i==$pages.curpage}>
     <li <{if !isset($smarty.get.show)}>class="active"<{/if}>><{if isset($smarty.get.show) && $smarty.get.show == "all"}><a href="/@Module.UrlName/cat/<{$data.id}>/page-<{$i}><{if isset($smarty.get.on)}>/<{$smarty.get.on}><{/if}><{if isset($smarty.get.sort)}>/<{$smarty.get.sort}><{/if}>" title=""><{$i}></a><{else}><{$i}><{/if}></li>
     <{else}>
     <li><a href="/@Module.UrlName/cat/<{$data.id}>/page-<{$i}><{if isset($smarty.get.on)}>/<{$smarty.get.on}><{/if}><{if isset($smarty.get.sort)}>/<{$smarty.get.sort}><{/if}>" title=""><{$i}></a></li>
     <{/if}><{/section}>
     <{if $pages.curpage < $pages.pages}><li><a href="/@Module.UrlName/cat/<{$data.id}>/page-<{$pages.curpage+1}><{if isset($smarty.get.on)}>/<{$smarty.get.on}><{/if}><{if isset($smarty.get.sort)}>/<{$smarty.get.sort}><{/if}>" title="">Вперед &gt;</a></li><{/if}>
    </ul>
    <{/if}>
    <div class="wrapper h10"></div>

    <{if count($data_items) == 0 && count($data_cats) == 0 && strlen($data.description)>0}><{$data.description}>
    <div class="wrapper h25"></div><{/if}>
    
<{/block}>