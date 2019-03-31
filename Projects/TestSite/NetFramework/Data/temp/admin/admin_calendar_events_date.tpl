<script type='text/javascript'>
$(document).ready(function(){
    $('a.events_deattach').click(function(){
        aj = new ajaxRequest();
        aj.load($(this).attr('href'),'date_result');
        return false;
    });
});
</script>

<{$result}>
<div id='date_result'></div>
<div>
 События за <{$dates.day}>.<{$dates.month}>.<{$dates.year}>:<br>
 <{foreach from=$data item=ad key=id}>
 <br>-------> <{$ad.module_name}>  <-------<br>
  <ul id="small_view">  
   <{foreach from=$ad.events item=ad2 key=id2}>
   <li class="li"><{$ad2.data_header}> - <a class='events_deattach' href='/admin/madmin/<{$id}>/calendar_apply_event2/<{$id2}>/<{$ad2.data_header}>/<{$dates.day}>/<{$dates.month}>/<{$dates.year}>/1'>Открепить от даты</a></li>  
   <{/foreach}>
  </ul>
 <{/foreach}>

</div>   
   
