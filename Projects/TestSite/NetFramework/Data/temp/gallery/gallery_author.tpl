<{extends "baseCommon.tpl"}>
<{block 'title'}>Галареи автора <{$data.user_profile_name}><{/block}>

<{block 'body'}>
    <div class="subpath"><a href="/" title="Главная страница">Главная</a> &rarr; <a href="/@Module.UrlName" title="">Галереи</a></div>
      <h1>Фотографии автора: <{$data.user_profile_name}></h1>
      <{if isset($data_galleries) && $data_galleries|@count >0}>
      <ul id="gal_gals">
       <{foreach from=$data_galleries item=ad key=id}>
       <li><a href='/@Module.UrlName/gallery/<{$ad.id}>' class='category_view'><img src="/data/photo/<{$ad.photo.preview_file}>" alt="<{$ad.name|re_quote}>" /></a>
       <a href='/@Module.UrlName/gallery/<{$ad.id}>' title='<{$ad.name|re_quote}>'><{$ad.name}></a>
       <p><{if $smarty.now|strftime:"%d" == $ad.date|strftime:"%d" && $smarty.now|strftime:"%m" == $ad.date|strftime:"%m"}>Сегодня<{else}><{$ad.date|strftime:"%e %b %Y"}><{/if}>
       <br /><br />Автор: <a href="/@Module.UrlName/author/<{$ad.name}>" title="Страница пользователя <{$ad.author|re_quote}>"><{$ad.author}></a>
       </p><div></div></li>
       <{/foreach}>
      </ul>  
      <{else}>Фотоархив пуст...<{/if}>  
       <{if isset($pages) && $pages.pages>1}>
        <ul id="pages">
         <li class="index"><span>Страница:</span></li>
         <{if $pages.curpage > 1 && $pages.curpage <= $pages.pages}><li><a href="/@Module.UrlName/author/<{$data.name}>/page-<{$pages.curpage-1}>" title="">&lt; Назад</a></li><{/if}>
         <{if $pages.curpage > 6}><li><a href="/@Module.UrlName" title="">1</a></li><li>...</li><{/if}>
         <{foreach from=$pages.stpg item=ad key=id}>
         <li><a href="/@Module.UrlName/author/<{$data.name}>/page-<{$id}>" title=""><{$id}></a></li>
         <{/foreach}>
         <li <{if !isset($smarty.get.show)}>class="active"<{/if}>><{if isset($smarty.get.show) && $smarty.get.show == "all"}><a href="/@Module.UrlName/author/<{$data.name}>/page-<{$i}>" title=""><{$pages.curpage}></a><{else}><{$pages.curpage}><{/if}></li>
         <{foreach from=$pages.fnpg item=ad key=id}>
         <li><a href="/@Module.UrlName/author/<{$data.name}>/page-<{$id}>" title=""><{$id}></a></li>
         <{/foreach}>
         <{if $pages.curpage < $pages.np}><li>...</li><li><a href="/@Module.UrlName/author/<{$data.name}>/page-<{$pages.pages}>" title=""><{$pages.pages}></a></li><{/if}>
         <{if $pages.curpage < $pages.pages}><li><a href="/@Module.UrlName/author/<{$data.name}>/page-<{$pages.curpage+1}>" title="">Вперед &gt;</a></li><{/if}>
        </ul>
        <div class="wrapper h10"></div>
       <{/if}>  
<{/block}>