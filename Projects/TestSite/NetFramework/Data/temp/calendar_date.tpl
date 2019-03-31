<{extends 'baseCommon.tpl'}>
<{block 'title'}><{if $time==false}>Ошибка<{else}>События за <{$time|strftime:"%d.%m.%Y"}><{/if}><{/block}>

<{block 'body'}>
<script type='text/javascript'>
$(function() {
    try {
    } catch(err) { alert(err); }
});
</script>
<{$result}>
<div>
 <{foreach from=$data item=ad key=id}>
 <br>-------> <{$id}>  <-------<br>
  <ul id="small_view">  
   <{foreach from=$ad item=ad2 key=id2}>
   <li class="li"><a href='<{$id2}>' target='_blank'><{$ad2}></a></li>  
   <{/foreach}>
  </ul>
 <{/foreach}>

</div>   
<{/block}>