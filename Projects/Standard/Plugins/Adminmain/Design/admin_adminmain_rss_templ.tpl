<{php}>setlocale(LC_ALL, "en_US.UTF-8");<{/php}>
<?xml version="1.0" encoding="utf-8" ?>
<rss version="2.0" xmlns:atom="http://www.w3.org/2005/Atom">
<channel>
<title><![CDATA[Сайт Егорьевска - популярный портал goEgorievsk.ru]]></title>
<link>@Url.ContentFullPath("")/</link>
<atom:link href="@Url.ContentFullPath("rss.xml")" rel="self" type="application/rss+xml" />
<description><![CDATA[Городские новости и информация, интересная как егорьевцам, так и гостям Егорьевска]]></description>
<image>
    <url>@Url.ContentFullPath("data/img/rss_goegorievsk.gif")</url>
    <link>@Url.ContentFullPath("")</link>
    <title><![CDATA[Сайт Егорьевска - популярный портал goEgorievsk.ru]]></title>
</image>
<language>ru</language>
<copyright>goEgorievsk.ru 2010</copyright>

<ttl>60</ttl>
<{foreach from=$data_mods item=ad key=id}>
<item>
<title><![CDATA[<{$ad.name}>]]></title>
<pubDate><{if isset($ad.date) && $ad.date != 0}><{$ad.date|strftime:"%a, %d %b %Y"}> <{$ad.date|v:"%H:%M:%S"}> +0300<{else}><{$time|strftime:"%a, %d %b %Y"}> <{$time|strftime:"%H:%M:%S"}> +0300<{/if}></pubDate>
<link>@Url.ContentFullPath("<{$ad.id}>")</link>
<guid isPermaLink="true">@Url.ContentFullPath("<{$ad.id}>")</guid>
<description>
<![CDATA[<{$ad.text|strip_tags|truncate:850}>]]>
</description>
</item>
<{/foreach}>
</channel>
</rss>