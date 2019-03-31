<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
      <div class="subpath"><a href="/" title="Главная страница">Главная</a> &rarr; <a href="/articles" title="">Статьи</a> &rarr; <a href='/articles/cat/<{$data.category}>' title="<{$data.articles_category_name|re_quote}>"><{$data.articles_category_name}></a></div>
      <h1><{$data.name}></h1>
      <div class="pages articles"><{$data.text}></div>
      <{if isset($data.photo) && $data.photo|@count >0}>
      <div id="articles_preview">
       <{foreach from=$data.photo item=ad key=id name=data_photo}>
       <{if !$smarty.foreach.data_photo.first}><div><a href="/<{$ad.photo_value}>" title="<{if isset($ad.photo_comment) && $ad.photo_comment != ""}><{$ad.photo_comment|re_quote}><{/if}>" rel="gallery"><img src="/<{$ad.photo_preview_value}>" alt="<{if isset($ad.photo_comment) && $ad.photo_comment != ""}><{$ad.photo_comment|re_quote}><{/if}>" /></a></div><{/if}>
       <{/foreach}>
      </div>
      <{/if}>
      <div></div>
      <small><{$data.date|strftime:"%e %b %Y"}></small>
      <div class="buttons">
       <script type="text/javascript" src="//yandex.st/share/share.js" charset="utf-8"></script>
       <div class="yashare-auto-init" data-yashareType="button" data-yashareQuickServices="yaru,vkontakte,facebook,twitter,odnoklassniki,moimir,lj"></div> 
      </div>
<{/block}>