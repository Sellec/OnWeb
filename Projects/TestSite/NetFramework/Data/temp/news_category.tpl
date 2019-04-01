<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
     <div class="subpath"><a href="/" title="Главная страница">Главная</a> <{if $data.id > 1}>&rarr; <a href="/@Module.UrlName" title="Новости">Новости</a><{/if}> &rarr; <{$data.name}></div>
     <h1><{$data.name}></h1>
      <{if isset($data_news) && $data_news|@count > 0}>
      <ul id="news_more">
      <{foreach from=$data_news item=ad key=id}>
       <li>
        <div class='news_img'><a href='/@Module.UrlName/news/<{$ad.id}>' title="<{$ad.name|re_quote}>" style="<{if strlen($ad.image)>0}>background-image:url(/<{$ad.image}>)<{else}><{if (isset($ad.photo[0]))}>background-image:url(/<{$ad.photo[0].photo_preview_value}>)<{/if}><{/if}>"></a></div>
        <div class='news_capt'><{if $smarty.now|strftime:"%d" == $ad.date|strftime:"%d" && $smarty.now|strftime:"%m" == $ad.date|strftime:"%m"}>Сегодня<{else}><{$ad.date|strftime:"%e %b %Y"}><{/if}> | <a href='/@Module.UrlName/news/<{$ad.id}>' title="<{$ad.name|re_quote}>"><{$ad.name}></a></div>
        <p><{$ad.text|strip_tags|truncate:1500:"..."}></p>
        <div class="wrapper"></div>
       </li>
       <{/foreach}>
      </ul><{else}>Новости не найдены!<{/if}>
       
       <{if isset($pages) && $pages.pages>1}>
        <ul id="pages">
         <li class="index"><span>Страница:</span></li>
         <{if $pages.curpage > 1 && $pages.curpage <= $pages.pages}><li><a href="/@Module.UrlName/cat/<{$data.id}>/page-<{$pages.curpage-1}>" title="">&lt; Назад</a></li><{/if}>
         <{if $pages.curpage > 6}><li><a href="/@Module.UrlName/cat/<{$data.id}>" title="">1</a></li><li>...</li><{/if}>
         <{foreach from=$pages.stpg item=ad key=id}>
         <li><a href="/@Module.UrlName/cat/<{$data.id}>/page-<{$id}>" title=""><{$id}></a></li>
         <{/foreach}>
         <li <{if !isset($smarty.get.show)}>class="active"<{/if}>><{if isset($smarty.get.show) && $smarty.get.show == "all"}><a href="/@Module.UrlName/cat/<{$data.id}>/page-<{$i}>" title=""><{$pages.curpage}></a><{else}><{$pages.curpage}><{/if}></li>
         <{foreach from=$pages.fnpg item=ad key=id}>
         <li><a href="/@Module.UrlName/cat/<{$data.id}>/page-<{$id}>" title=""><{$id}></a></li>
         <{/foreach}>
         <{if $pages.curpage < $pages.np}><li>...</li><li><a href="/@Module.UrlName/cat/<{$data.id}>/page-<{$pages.pages}>" title=""><{$pages.pages}></a></li><{/if}>
         <{if $pages.curpage < $pages.pages}><li><a href="/@Module.UrlName/cat/<{$data.id}>/page-<{$pages.curpage+1}>" title="">Вперед &gt;</a></li><{/if}>
        </ul>
        <div class="wrapper h10"></div>
       <{/if}>      
<{/block}>
