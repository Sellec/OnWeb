<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
      <div class="subpath"><a href="/" title="Главная страница">Главная</a> &rarr; <a href="/articles" title="">Публикации</a><{if isset($data_parent)}> &rarr; <a href="/articles/cat/<{$data_parent.id}>" title="<{$data_parent.name|re_quote}>"><{$data_parent.name}></a><{/if}></div>
      <h1><{$data.name}></h1>
      <{if isset($data_articles) && $data_articles|@count > 0}>
      <ul class="articles_center">
       <{foreach from=$data_articles item=ad key=id}>
       <li>
        <a href="/articles/view/<{$ad.id}>" title="<{$ad.name|re_quote}>" class="articles_capt"><{$ad.name}></a>
        <p class="articles_head"><{if $smarty.now|strftime:"%d" == $ad.date|strftime:"%d" && $smarty.now|strftime:"%m" == $ad.date|strftime:"%m"}>Сегодня<{else}><{$ad.date|strftime:"%e %b %Y"}><{/if}></p>
        <p<{if isset($ad.photo) && $ad.photo|@count>0}> class="is_photo is_text"><a href="/articles/view/<{$ad.id}>" title="<{$ad.name|re_quote}>"><img src="/<{$ad.photo[0].photo_preview_value}>" alt="<{$ad.name|re_quote}>" /></a><{else}> class="is_text"><{/if}><{$ad.text|strip_tags|truncate:350}></p>
        <div></div>
       </li>
       <{/foreach}>
      </ul><{else}>Публикации не найдены!<{/if}>
       <{if isset($pages) && $pages.pages>1}>
        <ul id="pages">
         <li class="index"><span>Страница:</span></li>
         <{if $pages.curpage > 1 && $pages.curpage <= $pages.pages}><li><a href="/articles/cat/<{$data.id}>/page-<{$pages.curpage-1}>" title="">&lt; Назад</a></li><{/if}>
         <{if $pages.curpage > 6}><li><a href="/articles/cat/<{$data.id}>" title="">1</a></li><li>...</li><{/if}>
         <{foreach from=$pages.stpg item=ad key=id}>
         <li><a href="/articles/cat/<{$data.id}>/<{$id}>" title=""><{$id}></a></li>
         <{/foreach}>
         <li <{if !isset($smarty.get.show)}>class="active"<{/if}>><{if isset($smarty.get.show) && $smarty.get.show == "all"}><a href="/articles/cat/<{$data.id}>/<{$i}>" title=""><{$pages.curpage}></a><{else}><{$pages.curpage}><{/if}></li>
         <{foreach from=$pages.fnpg item=ad key=id}>
         <li><a href="/articles/cat/<{$data.id}>/<{$id}>" title=""><{$id}></a></li>
         <{/foreach}>
         <{if $pages.curpage < $pages.np}><li>...</li><li><a href="/articles/cat/<{$data.id}>/<{$pages.pages}>" title=""><{$pages.pages}></a></li><{/if}>
         <{if $pages.curpage < $pages.pages}><li><a href="/articles/cat/<{$data.id}>/<{$pages.curpage+1}>" title="">Вперед &gt;</a></li><{/if}>
        </ul>
        <div class="wrapper h10"></div>
       <{/if}>      
<{/block}>