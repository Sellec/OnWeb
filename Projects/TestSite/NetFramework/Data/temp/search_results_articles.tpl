       <li><a href="/articles/<{$data.id}>" title="<{$data.name}>" class="linkname"><{$data.name}></a>
       <{if isset($data.text)}><p><{$data.text|strip_tags|truncate:200}></p><{/if}>
       <small><{if $smarty.now|strftime:"%d" == $data.date|strftime:"%d" && $smarty.now|strftime:"%m" == $data.date|strftime:"%m"}>Сегодня<{else}><{$data.date|strftime:"%e %b %Y"}><{/if}> | <a href="/articles/cat/<{$data.cat_id}>" title="<{$data.cat_name}>"><{$data.cat_name}></a></small>
       </li>
