<{extends "baseCommon.tpl"}>
<{block 'title'}>Вакансии<{/block}>

<{block 'body'}>
<script type='text/javascript'>
$(document).ready(function(){
	$("form#ask_q").requestJSON({
		before: function(){
			$("img#iask_q").fadeIn();
			$("input[type=submit]", this).attr('disabled', true);
		},
		after: function(result, message){
			$("img#iask_q").hide();
			$("input[type=submit]", this).removeAttr('disabled');

			if (message.length > 0) 
			{
				$("#sending_message").fadeOut();
				open_modal("#mod_error");
				$("#mod_error").html(nl2br(message));
			}
		}
	});
	
	$("input[name=dateBirth]").datepicker({
		showOtherMonths: true,
		selectOtherMonths: true,
		changeMonth: true,
		changeYear: true,
		dateFormat: "dd-mm-yy",
		yearRange: "-100:+0",
	});
	$.datepicker.regional['ru'];
});
</script>

	  <div class="subpath"><a href="/" title="Главная страница">Главная</a> &rarr; Вакансии</div>
	  <h1>Вакансии</h1>
	  <p><{$SiteConfig.vacancy_index}></p>
	  
	  <form action='/contacts/personalS' method='post' id='ask_q' enctype="multipart/form-data"> 
	   
	   <div style='margin-top:15px;margin-bottom:15px;'><label>Поля, помеченные звездочкой <span style='color:red;'>*</span>, обязательны для заполнения.</label></div>
	   <div><label>Вакансия: <span>*</span></label><br /><input type='text' class='text' size='45' name="vacancy" /></div>
	   <div><label>Фамилия: <span>*</span></label><br /><input type='text' class='text' size='15' name="lastname" /></div>
	   <div><label>Имя: <span>*</span></label><br /><input type='text' class='text' size='25' name="firstname" /></div>
	   <div><label>Отчество:</label><br /><input type='text' class='text' size='25' name="middlename" /></div>
	   <div><label>Контактный телефон: <span>*</span></label><br /><input type='text' class='text' size='25' name="phone" /></div>
	   <div><label>Адрес эл. почты: <span>*</span></label><br /><input type='text' class='text' size='25' name="email" /></div>
	   <div>
		<label>Пол: <span>*</span></label><br />
		<select name="sex"><option>Мужской</option><option>Женский</option></select>
	   </div>
	   <div><label>Дата рождения: <span>*</span></label><br /><input type='text' class='text' size='15' name="dateBirth" /></div>
	   <div><label>Образование:</label><br /><textarea cols="60" rows="5" name="education"></textarea></div>
	   <div><label>Опыт работы:</label><br /><textarea cols="60" rows="5" name="works"></textarea></div>

	   <div><label>Прикрепить файл:</label><input type='file' size='25' name="file_upload" class='file_upload' /></div>
	   <div class="c_div">
		   <{CaptchManager::reRender()}>
	   </div>
	   
	   <input type='submit' value='Задать вопрос' class="subm" style="width:480px;" />
	   <img src='/data/img/loading.gif' id="iask_q" style='display:none;'>
	  </form>                        
	  
	  
	  <{*if isset($data_news) && $data_news|@count > 0}>
	  <ul id="news_more">
	  <{foreach from=$data_news item=ad key=id}>
	   <li>
		<div class='news_img'><a href='/news/<{$ad.id}>' title="<{$ad.name|re_quote}>"<{if (isset($ad.photo[0]))}> style="background-image:url(/<{$ad.photo[0].photo_preview_value}>);"<{/if}>></a></div>
		<div class='news_capt'><{if $smarty.now|strftime:"%d" == $ad.date|strftime:"%d" && $smarty.now|strftime:"%m" == $ad.date|strftime:"%m"}>Сегодня<{else}><{$ad.date|strftime:"%e %b %Y"}><{/if}> | <a href='/news/<{$ad.id}>' title="<{$ad.name|re_quote}>"><{$ad.name}></a></div>
		<p><{$ad.text|strip_tags|truncate:1500:"..."}></p>
		<div class="wrapper"></div>
	   </li>
	   <{/foreach}>
	  </ul><{else}>Вакансии не найдены!<{/if}>

   <{if isset($pages) && $pages.pages>1}>
	<ul id="pages">
	 <li class="index"><span>Страница:</span></li>
	 <{if $pages.curpage > 1 && $pages.curpage <= $pages.pages}><li><a href="/news/page-<{$pages.curpage-1}>" title="">&lt; Назад</a></li><{/if}>
	 <{if $pages.curpage > 6}><li><a href="/news/page-1" title="">1</a></li><li>...</li><{/if}>
	 <{foreach from=$pages.stpg item=ad key=id}>
	 <li><a href="/news/page-<{$id}>" title=""><{$id}></a></li>
	 <{/foreach}>
	 <li <{if !isset($smarty.get.show)}>class="active"<{/if}>><{if isset($smarty.get.show) && $smarty.get.show == "all"}><a href="/news/page-<{$i}>" title=""><{$pages.curpage}></a><{else}><{$pages.curpage}><{/if}></li>
	 <{foreach from=$pages.fnpg item=ad key=id}>
	 <li><a href="/news/page-<{$id}>" title=""><{$id}></a></li>
	 <{/foreach}>
	 <{if $pages.curpage < $pages.np}><li>...</li><li><a href="/news/page-<{$pages.pages}>" title=""><{$pages.pages}></a></li><{/if}>
	 <{if $pages.curpage < $pages.pages}><li><a href="/news/page-<{$pages.curpage+1}>" title="">Вперед &gt;</a></li><{/if}>
	</ul>
	<div class="wrapper h10"></div>
   <{/if*}>
<{/block}>