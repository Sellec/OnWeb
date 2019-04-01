<{extends "baseCommon.tpl"}>

<{block 'body'}>
<{*<script type='text/javascript'>
$(function() {
    try {
    <{foreach from=$data_items item=ad key=id}>aj_<{$id}> = new ajaxRequest();aj_<{$id}>.load_form('add_form_<{$id}>',null,'cart_add_result_<{$id}>');<{/foreach}>
    } catch(err) { alert(err); } 
});
</script>*}>

    <div class="subpath"><strong>Вы здесь:</strong> <a href="/" title="Главная">Главная</a> &rarr; Каталог продукции</div>
    
    <h1>Каталог продукции</h1>
   <{if $data_cats|@count>0}>
    <ul id="goods_category">
    <{foreach from=$data_cats item=ad key=id}>
        <li><{*<p><a href="/@Module.UrlName/cat/<{$id}>" title="<{$ad.name|re_quote}>" class="a_title"><{$ad.name}></a>*}></p>
        <a href="/@Module.UrlName/cat/<{$id}>" title="<{$ad.name|re_quote}>" class="a_img" style="background-image:url(/<{if isset($ad.image) && $ad.image|strlen>0}><{$ad.image}><{else}>data/img/nophoto.jpg<{/if}>);"></a>
        <div class="a_text"><{$ad.description}><a href="/@Module.UrlName/cat/<{$id}>" title="<{$ad.name|re_quote}>" class="a_text_link"></a></div>
        <div class="wrapper"></div>
        </li>
    <{/foreach}>
    </ul>
   <div class="wrapper h25"></div>
   <{/if}>    
<{/block}>