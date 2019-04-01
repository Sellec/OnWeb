<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
  <div class="subpath"><a href="/" title="">Главная</a> &rarr; <a href="/@Module.UrlName" title="">Фотогалерея</a></div>
  <h1><{$data.name}></h1>
  <{if isset($data.description) && $data.description|strlen>0}><div class="pages"><p><{$data.description}></p></div><{/if}>
  <{if isset($data_cats) && $data_cats|@count>0}>
  <ul id="category">
  <{foreach from=$data_cats item=ad key=id}>
   <li><a href='/@Module.UrlName/cat/<{$ad.id}>' title="<{$ad.name|re_quote}>"><{$ad.name}></a></li>
  <{/foreach}>
  </ul>
  <{/if}>
  <{if isset($data_gals) && $data_gals|@count >0}>
  <ul id="gal_gals">
   <{foreach from=$data_gals item=ad key=id name=galphoto}>
   <{math equation=x%2 x=$smarty.foreach.galphoto.iteration assign=nomar}>
   <li<{if $nomar==0}> style="margin-right:0;"<{/if}>><a href='/@Module.UrlName/gallery/<{$ad.id}>' class='category_view'><img src="/data/photo/<{$ad.photo.preview_file}>" alt="<{$ad.name|re_quote}>" /></a>
   <a href='/@Module.UrlName/gallery/<{$ad.id}>' title='<{$ad.name|re_quote}>'><{$ad.name}></a>
   <p><{if $smarty.now|strftime:"%d" == $ad.date|strftime:"%d" && $smarty.now|strftime:"%m" == $ad.date|strftime:"%m"}>Сегодня<{else}><{$ad.date|strftime:"%e %b %Y"}><{/if}></p><div class="wrapper"></div></li>
   <{/foreach}>
  </ul>  
  <{else}>Фотоархив пуст...<{/if}>
<{/block}>
