<{extends "baseCommon.tpl"}>
<{block 'title'}>Результаты поиска<{/block}>

<{block 'body'}>
      <{foreach from=$data item=ad key=id}>
      <h2><{$ad.options.name}></h2>
      <{if $ad.search_results|@count==0 && $ad.search_code|@count==0}>
      Ничего не было найдено<{else}>
      <ul class="search_results">
       <{foreach from=$ad.search_results item=ad2 key=id2}>
       <li><a href="<{$ad2.link}>" title=""><{$ad2.name}></a>
       <{if isset($ad2.description)}><p><{$ad2.description}></p><{/if}>
       </li>
       <{/foreach}>
      </ul>
      <{if $ad.search_code|@count>0}>
      <ul class="search_results">
      <{$ad.search_code}>
      </ul>
      <{/if}>
      <{/if}>
      <{/foreach}>
      <{if isset($err)}><h1>Поиск по сайту</h1><div class="pages"><{$err}></div><{/if}>
<{/block}>