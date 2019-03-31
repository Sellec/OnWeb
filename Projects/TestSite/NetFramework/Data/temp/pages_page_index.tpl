<{include file="html.tpl"}>
<head>
<title><{getCustom type=config option=SiteFullName}></title>
<{include file="headers.tpl"}>
<script type='text/javascript' src='/data/js/jquery.easing.1.3.js'></script>
</head>
<body id="index_page">
<div id="container">
 <div id="header">
  <{getCustom type=config option=SiteFullName assign="site_title"}>
  <div id="logo"><div class="logo"></div></div>
  <div id="top_menu"><ul>
   <{editableMenu id=1}>
  </ul></div>
 </div>
  
  <div id="center_frame">                     
   <h2>Свежие новости</h2>
   <div id="cf_news"><div>
       <{getCustom type=getNews assign=news}>
       <{if $news|@count>0}>
       <ul>
        <{foreach from=$news item=ad key=id}>
        <li><a href='/news/<{$ad.id}>' title="<{$ad.name|re_quote}>" style="<{if strlen($ad.image)>0}>background-image:url(/<{$ad.image}>)<{else}><{if (isset($ad.photo[0]))}>background-image:url(/<{$ad.photo[0].photo_preview_value}>)<{/if}><{/if}>" class='news_img'></a>
            <a href="/news/<{$ad.id}>" title=""><{$ad.name}></a>
            <br /><small><{$ad.date|strftime:"%d %B %Y"}></small><p><{$ad.text|strip_tags|truncate:250}></p></li>
        <{/foreach}>
       </ul>
       <{/if}>
   </div></div>
   <div id="cf_block"><div>
       <a href="/pages/kompaniya" title="Егорьевская колбасно-гастрономическая фабрика" class="main_link"></a>
       <object type="application/x-shockwave-flash" data="/data/fla/intro.swf" width="362" height="226">
         <param name="movie" value="/data/fla/intro.swf" />
         <param name="quality" value="high" />
         <param name="menu" value="false" />
         <param name="wmode" value="opaque" />
         <img src="/data/img/intro.jpg" alt=""  width="362" height="226" />
       </object>
   </div></div>
  </div>
  
</div>
<div id="footer">

</div>
<{include file="google_analytics.tpl"}>
</body>
</html>