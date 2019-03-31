<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
    <div class="subpath"><a href="/" title="Главная страница">Главная</a></div>
    <h1><{$data.name}></h1>
    <{if $data.description != ""}><p id="dataname"><{$data.description}></p><{/if}>

    <div class="buttons">
     <script type="text/javascript" src="//yandex.st/share/share.js" charset="utf-8"></script>
     <div class="yashare-auto-init" data-yashareType="button" data-yashareQuickServices="yaru,vkontakte,facebook,twitter,odnoklassniki,moimir,lj"></div> 
    </div>

    <{*if isset($data.comments)}><{include file="customs_comments_view.tpl" comments=$data.comments}><{else}><{include file="customs_comments_view.tpl"}><{/if*}>
<{/block}>
