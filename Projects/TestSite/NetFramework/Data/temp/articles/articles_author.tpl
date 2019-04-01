<{extends "baseCommon.tpl"}>
<{block 'title'}>Публикации автора <{$data.user_profile_name}><{/block}>

<{block 'body'}>
      <div class="subpath"><a href="/" title="Главная страница">Главная</a> &rarr; <a href="/articles" title="">Публикации</a></div>
      <h1>Публикации автора: <{$data.user_profile_name}></h1>
      <{if isset($data_articles) && $data_articles|@count > 0}>
      <ul class="articles_center">
       <{foreach from=$data_articles item=ad key=id}>
       <li>
        <a href="/@Module.UrlName/view/<{$ad.id}>" title="<{$ad.name|re_quote}>" class="articles_capt"><{$ad.name}></a>
        <p class="articles_head"><{if $smarty.now|strftime:"%d" == $ad.date|strftime:"%d" && $smarty.now|strftime:"%m" == $ad.date|strftime:"%m"}>Сегодня<{else}><{$ad.date|strftime:"%e %b %Y"}><{/if}> | <a href="/@Module.UrlName/cat/<{$ad.category}>" title=""><{$ad.catname}></a> | <a href="/@Module.UrlName/view/<{$ad.id}>" title="<{$ad.name|re_quote}>">Комментарии</a><{if (isset($ad.comments_count) && $ad.comments_count > 0)}> (<{$ad.comments_count}>)<{/if}></p>
        <p><a href="/@Module.UrlName/view/<{$ad.id}>" title="<{$ad.name|re_quote}>"><img src="<{if isset($ad.photo) && $ad.photo|@count>0}>/<{$ad.photo[0].photo_preview_value}><{else}>/data/img/nophoto_articles.jpg<{/if}>" alt="<{$ad.name|re_quote}>" /></a><{$ad.text|strip_tags|truncate:350}></p>
        <div></div>
       </li>
       <{/foreach}>
      </ul><{else}>Публикации не найдены!<{/if}>
       <{if isset($pages) && $pages.pages>0}>
        <ul id="pages">
         <li class="index"><span>Страница:</span></li>
         <{if $pages.curpage > 1 && $pages.curpage <= $pages.pages}><li><a href="/@Module.UrlName/author/<{$data.name}>/page-<{$pages.curpage-1}>" title="">&lt; Назад</a></li><{/if}>
         <{if $pages.curpage > 6}><li><a href="/@Module.UrlName/author/<{$data.name}>" title="">1</a></li><li>...</li><{/if}>
         <{foreach from=$pages.stpg item=ad key=id}>
         <li><a href="/articlesauthor/<{$data.name}>/page-<{$id}>" title=""><{$id}></a></li>
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