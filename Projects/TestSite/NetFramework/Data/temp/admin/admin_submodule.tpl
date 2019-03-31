 <li><a href='' class='inmenu'><{$data_mods.name}></a>
  <ul>
  <{foreach from=$data_mods.links item=ad2 key=id2}>
  <{if is_array($ad2)}>
   <{include file="admin/admin_submodule.tpl" data_mods=$ad2}>
  <{else}>
   <li><a href='<{$id2}>' class=''><{$ad2}></a></li>
  <{/if}>
  <{/foreach}>
  </ul>
