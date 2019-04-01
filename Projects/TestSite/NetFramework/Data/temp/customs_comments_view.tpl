<{assign var=isAdmin value=$Module->isAdmin()}>
<script type='text/javascript'>
var RecaptchaOptions = {theme : 'clean',lang : 'ru'};
$(function(){
    aj = new ajaxRequest();
    aj.load_form('comment_form',null,'comment_result');
    
    <{if $isAdmin}>
    $('a.comment_delete').click(function(){
        var clicked = $(this);
        aj = new ajaxRequest();
        aj.userOnLoad = function(){clicked.parent().fadeOut()};
        aj.load($(this).attr('href'),'comments_result');
        return false;
    });
    <{/if}>
});
</script>
<div class="topborder"></div>
<p class="caption little"><a name="comments" title=""></a><a href="#write_own" title="Оставить комментарий" class="towrite">комментировать</a><h3 style="margin-top:-5px;">Комментарии и отзывы <{if isset($comments) && $comments|@count>0}> <span>(<{$comments|@count}>)</span><{/if}></h3></p>

<div id="site_comments">
<!--SITE COMMENTS-->
<{if isset($comments) && $comments|@count > 0}>
<div id='comments_result'></div>
<ul id="comm_view">
 <{foreach from=$comments item=ad key=id}>
  <li>
   &raquo; <{if $ad.comm_user_id != 0 & ($ad.user_id != NULL)}>
   <{*<a href="/@Module.UrlName/finduser/<{$ad.user_login}>" title="<{$ad.user_login}>"><{$ad.user_login}></a> <span class="date"><{$ad.comm_time|strftime:"%d-%m-%Y %H:%M:%S"}></span>*}>
   <strong><{$ad.user_login}></strong> <span class="date"><{$ad.comm_time|strftime:"%d-%m-%Y %H:%M:%S"}></span>
   <{else}>
    <{if isset($ad.comm_user_email) && $ad.comm_user_email !=""}><a href="mailto:<{$ad.comm_user_email}>" title=""><{$ad.comm_user_name}></a><{else}><b><{$ad.comm_user_name}></b><{/if}> <span class="date"><{$ad.comm_time|strftime:"%d-%m-%Y %H:%M:%S"}></span>
   <{/if}>
   <{if $ad.comm_user_id == $UserManager->getID() || $isAdmin}>
   <a class='comment_delete' href="/@Module.UrlName/comment_delete/<{$ad.comm_id}><{if isset($photo_view)}>&gall=1<{/if}>" title="Удалить">Удалить</a>
   <{/if}>
   <p class="pages"><{$ad.comm_text|nl2br}></p>
  </li>  
 <{/foreach}>
</ul>
<{else}>
<p class="nomess">Здесь пока никто ничего не писал...</p>
<ul id="comm_view">
 <li></li>
</ul>
<{/if}>
<h3>Оставить отзыв:<a name="write_own" title=""></a></h3>
<form action='/@Module.UrlName/comment_add/<{$data.id}><{if isset($photo_view)}>&gall=1<{/if}>' method='post' id="comment_form">
 <div class="form"><label for="text">Текст <span class="req">*</span>:</label><br />
 <textarea name='comm_text' rows="5" cols="68"></textarea></div>
 <{if !$IsAuthorized}>
 <div class="half"><label for="name">Имя:</label><br />
 <input type='text' name='comm_name' size='20'></div>
 <div class="half"><label for="email">Email:</label><br />
 <input type='text' name='comm_email' size='20'></div>
 <{/if}>
 <div class="half"><label>Код проверки <span class="req">*</span>:</label>
  <input type='text' name='c_num' id='captch_code' /></div>
  <div class="half"><img src='/captch/code' alt='' class='c_code' /></div>
  <{*<div class="pages"><p>Оставляя комментарий, вы соглашаетесь с правилами публикации данного сайта: <a href="/help/rules" title="" class="ontop">ознакомиться с правилами</a>.</p></div>*}>
 <input type='submit' value='&nbsp;Добавить комментарий&nbsp;'> <img src="/data/img/loading.gif" alt="Идет отправка комментария" id="loading_img" />
 <div id='comment_result' class='form'></div>
</form>
<!--SITE COMMENTS.END-->
</div>
<div id="vk_comments">
<!--VK COMMENTS-->
<div id="vk_comments"></div>
<script type="text/javascript">
VK.Widgets.Comments("vk_comments", {limit: 10, width: "460", attach: "*"});
</script>
<!--VK COMMENTS.END-->
</div>
<div class="wrapper"></div>