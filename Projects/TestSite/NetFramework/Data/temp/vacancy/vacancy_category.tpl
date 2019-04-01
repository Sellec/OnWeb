<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
     <div class="subpath"><a href="/" title="Главная страница">Главная</a> &rarr; <a href="/@Module.UrlName" title="Вакансии">Вакансии</a> &rarr; <{$data.name}></div>
     <h1><{$data.name}></h1>
      <{if isset($data_vacancy) && $data_vacancy|@count > 0}>
      <Br /><ul id="vacancy_more">
      <{foreach from=$data_vacancy item=ad key=id}>
       <li>
        <div class='vacancy_capt'><span><{if $smarty.now|strftime:"%d" == $ad.date|strftime:"%d" && $smarty.now|strftime:"%m" == $ad.date|strftime:"%m"}>Сегодня<{else}><{$ad.date|strftime:"%e %b %Y"}><{/if}></span> | <a href='/vacancy/<{$ad.urlname}>/<{$ad.id}>' title="<{$ad.name|re_quote}>"><{$ad.name}></a></div>
        <{*<p><{$ad.text|strip_tags|truncate:1500:"..."}></p>
        <div class="wrapper"></div>*}>
       </li>
       
       <{*<li>
        <div class='vacancy_capt'><{if $smarty.now|strftime:"%d" == $ad.date|strftime:"%d" && $smarty.now|strftime:"%m" == $ad.date|strftime:"%m"}>Сегодня<{else}><{$ad.date|strftime:"%e %b %Y"}><{/if}> | <a href='/vacancy/<{$ad.urlname}>/<{$ad.id}>' title="<{$ad.name|re_quote}>"><{$ad.name}></a></div>
        <p><{$ad.text|strip_tags|truncate:1500:"..."}></p>
        <div class="wrapper"></div>
       </li>*}>
       <{/foreach}>
      </ul><{else}>Вакансии не найдены!<{/if}>
       
       <{if isset($pages) && $pages.pages>1}>
        <ul id="pages">
         <li class="index"><span>Страница:</span></li>
         <{if $pages.curpage > 1 && $pages.curpage <= $pages.pages}><li><a href="/@Module.UrlName/<{$data.urlname}>/page-<{$pages.curpage-1}>" title="">&lt; Назад</a></li><{/if}>
         <{if $pages.curpage > 6}><li><a href="/@Module.UrlName/<{$data.urlname}>" title="">1</a></li><li>...</li><{/if}>
         <{foreach from=$pages.stpg item=ad key=id}>
         <li><a href="/@Module.UrlName/vacancy/<{$data.urlname}>/page-<{$id}>" title=""><{$id}></a></li>
         <{/foreach}>
         <li <{if !isset($smarty.get.show)}>class="active"<{/if}>><{if isset($smarty.get.show) && $smarty.get.show == "all"}><a href="/vacancy/<{$data.urlname}>/page-<{$i}>" title=""><{$pages.curpage}></a><{else}><{$pages.curpage}><{/if}></li>
         <{foreach from=$pages.fnpg item=ad key=id}>
         <li><a href="/vacancy/<{$data.urlname}>/page-<{$id}>" title=""><{$id}></a></li>
         <{/foreach}>
         <{if $pages.curpage < $pages.np}><li>...</li><li><a href="/vacancy/<{$data.urlname}>/page-<{$pages.pages}>" title=""><{$pages.pages}></a></li><{/if}>
         <{if $pages.curpage < $pages.pages}><li><a href="/vacancy/<{$data.urlname}>/page-<{$pages.curpage+1}>" title="">Вперед &gt;</a></li><{/if}>
        </ul>
        <div class="wrapper h10"></div>
       <{/if}>
       
       <div class="pages">
            <hr />
            <{if isset($data.description) && strlen($data.description)>0}><{$data.description}><{/if}>
            <p><{$warranty}></p>
       </div>      
<{/block}>
