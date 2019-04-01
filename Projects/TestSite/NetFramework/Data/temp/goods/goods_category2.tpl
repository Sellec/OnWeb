<{extends "baseCommon.tpl"}>
<{block 'title'}>Просмотр по категориям<{/block}>

<{block 'body'}>
    <div class="subpath"><a href="/" title="Главная">Главная</a>
    
    <{foreach from=$cats item=ad key=id}>

    <div class="cat_title">
     <h1><{$ad.name}></h1>
    </div>
    
    <{if isset($smarty.get.un) && isset($smarty.get.model)}>
    Ксенон для <{$smarty.get.model}>. Для этой марки используется <{if $smarty.get.un==1}>универсальный<{else}>обычный<{/if}> ксенон.
    <{/if}>
    
    <{if $ad.sub|@count > 0}>
    <h2>Подкатегории</h2>
    <ul id="goods_category">
     <{foreach from=$ad.sub item=ad2 key=id2}><li><p><a href="/@Module.UrlName/cat/<{$id2}>" title="<{$ad2.name|re_quote}>" class="a_title"><{$ad2.name}></a></p><{/foreach}>
    </ul><div class="wrapper"></div>
    <{/if}>
    
    <{if isset($ad.items)}>
    <ul id="items_category">
    <{foreach from=$ad.items item=ad2 key=id2 name=data_items}>
    <{math equation=x%4 x=$smarty.foreach.data_items.iteration assign=nomar}>
    <li<{if $nomar==0}> class="nomargin"<{/if}>>
     <div class="ititle"><a href="/@Module.UrlName/item/<{$items[$id2]}>" title="<{$items[$id2].name|re_quote}>"><{$items[$id2].name}></a></div>
     <div class="item_photo" id="ip_<{$id2}>">
      <a href="/@Module.UrlName/item/<{$id2}>" title="<{$items[$id2].name|re_quote}>" style="background-image:url(/data/<{if isset($items[$id2].photo[0].preview_file)}>photo/<{$items[$id2].photo[0].preview_file}><{else}>img/nophoto.jpg<{/if}>)"></a>
     </div>
     <div class="item_descr"><{$items[$id2].description|strip_tags|truncate:200:"..."}></div>
     <div class="item_info">
      <div class="price"><{if $items[$id2].price != 0}><{$items[$id2].price|number_format:0:".":" "}> <span>руб.</span><{else}><span>Звоните!</span><{/if}></div>
      <div class="addcart">
       <form action='/@Module.UrlName/cart_add/<{$id2}>' method='post' id='add_form_<{$id2}>' class="addcartid">
        <input type='hidden' value='1' class='it_count' name='count' /><a href="" title="Купить <{$items[$id2].name}>">В корзину</a>
       </form>
       <div id='cart_add_result_<{$id2}>'></div>
      </div>
     </div>
     </li>
    <{/foreach}>
    </ul>
    <{/if}>
    
    </div>
    <div class="wrapper h10"></div>

    <{/foreach}>
<{/block}>