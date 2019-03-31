<div id="item_div">
    <div id="item_left">
        <h1><{$data.name}></h1>
        <h2><{$data.type}></h2>
     
        <{if $data.photo|@count > 1}>
        <div id="item_photo">
            <{if isset($data.photo.item_photo) && $data.photo.item_photo|strlen>0}><a href="/<{$data.photo.item_photo}>" title="<{$data.name|re_quote}>" class="onlightbox"><img src="/<{if isset($data.photo.item_bimage)}><{$data.photo.item_bimage}><{else}>data/img/news_img.gif<{/if}>" alt="" /></a><{else}>
            <img src="/<{if isset($data.photo.item_bimage)}><{$data.photo.item_bimage}><{else}>data/img/news_img.gif<{/if}>" alt="" />
            <{/if}>
        </div>
        <{/if}>
        
        <div class="item_article"><{$data.article}></div>
    </div>
    <div id="item_right">
        <{if $data.description != ""}><div id="item_description"><{$data.description}></div><{/if}>
        <table id="item_info">
            <tr>
                <td class="ii_weight">&nbsp;</td>
                <td><{$data.weight|nl2br}></td>
                <td class="ii_time"></td>
                <td><{$data.time|nl2br}></td>
            </tr>
            <tr>
                <td colspan="4" height="20"></td>
            </tr>
            <tr>
                <td class="ii_type"></td>
                <td><{$data.type|nl2br}></td>
                <td class="ii_pack"></td>
                <td><{$data.pack|nl2br}></td>
            </tr>
        </table>
        <{$data.description_2|nl2br}>
    </div>
</div>