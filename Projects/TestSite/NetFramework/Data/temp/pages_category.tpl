<{extends "baseCommon.tpl"}>
<{block 'title'}><{$data.name}><{/block}>

<{block 'body'}>
 <div id="content_top"><img src="/data/img/content_tl.gif" alt="" /></div>
  <div id="content_left">
   <div id="content">
    <div id="cont_left_page">
     <!-- Левая колонка.START -->
      <p class="subpath"><a href="/" title="Главная страница">Главная</a></p>
      <p class="caption"><img src="/data/img/icon_city.png" alt="<{$data.name|re_quote}>" /> <{$data.name}></p>
      <p class="descr"><{$data.description}></p>
      <{if isset($data_pages) && $data_pages|@count > 0}>
      <ul id="pages_main">
      <{foreach from=$data_pages item=ad key=id}>
       <li>
        <{if isset($ad.photo) && $ad.photo|@count > 0}><img src="/<{$ad.photo[0].photo_preview_value}>" alt="<{$ad.name|re_quote}>" /><{/if}><a href='/pages/<{$ad.urlname}>' class='category_view'><{$ad.name}></a><br />
        <{$ad.body|strip_tags|trim|truncate:600:"..."}>
        <div class="wrapper"></div>
       </li>
       <{/foreach}>
      </ul><{else}>Публикации не найдены!<{/if}>
     <!-- Левая колонка.END -->
    </div>
    <div id="cont_center_page">
     <div id="cont_right">
      <!-- Правая колонка.START -->
      <!-- Правая колонка.END -->
     </div>
     <!-- Центральная колонка.START -->
      <{if isset($data_cats) && $data_cats|@count > 0}>
      <p class="caption">Категории публикаций</p>
      <ul id="cats_more">
      <{foreach from=$data_cats item=ad key=id}>
       <li>&rarr; <a href="/pages/cat/<{$ad.id}>" title="<{$ad.name|re_quote}>"><{$ad.name}></a></li>
      <{/foreach}>
      </ul>
      <{/if}>
     <!-- Центральная колонка.END -->
    </div>
    <div class="wrapper"></div>
   </div>
  </div>
 <div id="content_bottom"><img src="/data/img/content_bl.gif" alt="" /></div>
<{/block}>