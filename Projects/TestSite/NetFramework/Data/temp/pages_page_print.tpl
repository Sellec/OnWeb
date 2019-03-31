<{include file="html.tpl"}>
<head>
<title><{$pagedata.name}> : <{getCustom type=config option=SiteFullName}></title>
<link rel="stylesheet" href="/data/css/main.css" type="text/css" media="screen" />
<link rel="stylesheet" href="/data/css/print.css" type="text/css" media="print" />
<meta name="robots" content="noindex, nofollow" />
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script>
<script type="text/javascript">
$(function(){
    $("input[name='print_photo']").change(function(){
        var print = $(this).attr("checked");
        if ( !print ) $("#pages_preview").hide();
        else $("#pages_preview").show();
    })
})
</script>
<style type="text/css">
@media screen {
    body {padding:20px;width:950px;}
    .print_options {font-weight:bold;}
    .print_info a {float:right;color:#000;text-decoration:none;}
    .print_info small {line-height:14px;}
    hr {height:1px;background-color:#fff;border:0;border-top:1px solid #000;margin:2px 0 10px 0;width:100%;}
}
</style>
</head>
<body>
<a href="javascript:history.go(-1)" title="" class="noprint">&larr; Назад</a>
<p class="print_info"><a>@Url.ContentFullPath("pages/<{$pagedata.urlname}>") &nbsp; <br /><small>При использовании материалов, размещенных на сайте,<br />ссылка на @Url.ContentFullPath("") обязательна!</small></a><strong>&laquo;Егорьевский курьер&raquo; &copy;</strong>
<div class="wrapper"></div></p>
<hr>
<h1><{if isset($pagedata.name)}><{$pagedata.name}><{else}>Страница не найдена<{/if}></h1>
<div class="pages">
<{$pagedata.body}>
</div>
<{if isset($pagedata.photo) && isset($pagedata.photo[0])}>
<div class="print_options">
 <input type="checkbox" name="print_photo" checked="checked" /> Печатать фотографии
</div>
<ul id="pages_preview">
 <{foreach from=$pagedata.photo item=ad key=id}>
 <li><a href="/<{$ad.photo_value}>" title="<{if isset($ad.photo_comment) && $ad.photo_comment != ""}><{$ad.photo_comment|re_quote}><{else}><{$pagedata.name|re_quote}><{/if}>"><img src="/<{$ad.photo_preview_value}>" alt="<{if isset($ad.photo_comment) && $ad.photo_comment != ""}><{$ad.photo_comment|re_quote}><{else}><{$pagedata.name|re_quote}><{/if}>" /></a></li>
 <{/foreach}>
</ul>
<{/if}>
<{include file="google_analytics.tpl"}>
</body>
</html>