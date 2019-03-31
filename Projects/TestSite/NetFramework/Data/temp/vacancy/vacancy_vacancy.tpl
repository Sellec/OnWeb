<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
    <div class="subpath"><a href="/" title="Главная страница">Главная</a> &rarr; <a href="/vacancy" title="Вакансии">Вакансии</a> &rarr; <a href='/vacancy/<{$data.urlname}>' title="<{$data.vacancy_category_name|re_quote}>"><{$data.vacancy_category_name}></a> &rarr; <{$data.name}></div>
    <h1><{$data.name}></h1>
    <div class="pages"><{$data.text}></div>
    
    <div class="buttons">
     <small><{if $smarty.now|strftime:"%d" == $data.date|strftime:"%d" && $smarty.now|strftime:"%m" == $data.date|strftime:"%m"}>Сегодня, <{$data.date|strftime:"%H:%M"}><{else}><{$data.date|strftime:"%e %b %Y"}><{/if}></small>
     <script type="text/javascript" src="//yandex.st/share/share.js" charset="utf-8"></script>
     <div class="yashare-auto-init" data-yashareType="button" data-yashareQuickServices="yaru,vkontakte,facebook,twitter,odnoklassniki,moimir,lj"></div> 
    </div>
    
<{/block}>