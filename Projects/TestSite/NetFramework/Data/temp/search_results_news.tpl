       <li><a href="/news/<{$data.id}>" title="<{$data.name}>"><{$data.name}></a>
       <{if isset($data.description)}><p><{$data.text|truncate:200}></p><{/if}>
       <small><a href="/news/cat/<{$data.cat_id}>" title="<{$data.cat_name}>"><{$data.cat_name}></a> | <{$data.date}></small>
       </li>
