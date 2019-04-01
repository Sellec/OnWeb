<{extends "baseCommon.tpl"}>
<{block 'title'}>Фоторепортажи<{/block}>

<{block 'body'}>
<script type="text/javascript">
$(function(){
    $("#gall_photo img").css({"width":$("#gall_photo").width()})
});
</script>
    <div class="subpath"><a href="/" title="">Главная</a> &rarr; <a href="/@Module.UrlName" title="">Фотоархив</a> &rarr; <a href="/@Module.UrlName/gallery/<{$data.gallery}>" title=""><{$data.gallery_name}></a></div>
    <p style="text-align:right;font-size:13px;margin:2px 0;">Фото <{$data.id}></p>
    <div id="gall_photo">
     <a href="/data/photo/<{$data.photo.main_file}>" title=""><img src="/data/photo/<{$data.photo.main_file}>" alt="" /></a>
    </div>
    <p><{$data.text}></p>
    <div class="bottom_view"><{$data.date|strftime:"%e %b %Y"}> <p>Просмотров: <{$data.count_views}></p></div>
    <div class="prevnext">
     <div class="prev_photo"><{if isset($prev)}><a href="/@Module.UrlName/photo/<{$prev.id}>" title="">&larr; Предыдущее</a><{/if}></div>
     <div class="next_photo"><{if isset($next)}><a href="/@Module.UrlName/photo/<{$next.id}>" title="">Следующее &rarr;</a><{/if}></div>
    </div>
    
    <{if @isset($data.comments)}><{include file=customs_comments_view.tpl comments=$data.comments}><{else}><{include file="customs_comments_view.tpl"}><{/if}>
<{/block}>
