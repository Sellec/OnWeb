<{extends "baseCommon.tpl"}>
<{block 'title'}>Обратная связь<{/block}>

<{block 'body'}>
<script type="text/javascript">
$(document).ready(function(){
    aj = new ajaxRequest();
    aj.load_form('help_frm',null,'result');
    $("#help_frm").submit(function(){$("#sending_message").fadeIn();})
});
</script>
      
      <h1>Контактная информация</h1>
      <{getCustom type=config option=help_info}>
<{/block}>