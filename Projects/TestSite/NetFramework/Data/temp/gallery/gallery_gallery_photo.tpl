<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
    <div class="subpath"><a href="/" title="">Главная</a> &rarr; <a href="/@Module.UrlName" title="">Фотогалерея</a> &rarr; <a href="/@Module.UrlName/cat/<{$data.sub_id}>" title=""><{$data.catname}></a></div>
    <h1><{$data.name}></h1>
    <{if isset($data.description) && $data.description|strlen>0}><div class="pages"><p><{$data.description}></p></div><{/if}>
    <{if $data_photo|@count <= 0}>Галерея пуста.<{else}>
    <{if isset($data_photo) && $data_photo|@count>0}>
    <{*<div class="bottom_view"><{if $smarty.now|strftime:"%d" == $data.date|strftime:"%d" && $smarty.now|strftime:"%m" == $data.date|strftime:"%m"}>Сегодня<{else}><{$data.date|strftime:"%e %b %Y"}><{/if}>
    <p><{$data.author}></p></div><br />*}>
    <div id="gall_view">
     <{foreach from=$data_photo item=ad key=id name=galphoto}>
     <{math equation=x%3 x=$smarty.foreach.galphoto.iteration assign=nomar}>
     <div<{if $nomar==0}> style="margin-right:0;"<{/if}>><a href='/data/photo/<{$ad.photo.main_file}>' title='<{$data.name|re_quote}>' class="onlightbox" rel="lightbox-tour"><img src="/data/photo/<{$ad.photo.preview_file}>" alt="" /></a></div>
     <{/foreach}>
    </div>
    <{/if}>
    <{/if}>
<{/block}>
