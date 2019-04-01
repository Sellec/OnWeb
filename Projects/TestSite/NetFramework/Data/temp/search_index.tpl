<{extends "baseCommon.tpl"}>
<{block 'title'}>Поиск по сайту<{/block}>

<{block 'body'}>
<script type="text/javascript">
$(function(){
    $("#conf_submit").click(function(){
        if ( !$("#search_list option:selected").length ){
            $("#search_list option").each(function(){
                if ($(this).val() != -1) $(this).attr("selected",true);
            })
            $("#form_ae").submit();
            return false;
        }
    })
})
</script>

      <div class="subpath"><a href="/" title="Главная страница">Главная</a></div>
      <h1>Поиск по сайту</h1>
      <form action='/@Module.UrlName/search' method='post' id="search_page">
      <div><label>Что ищем:</label><br /><input type='text' name='search'></div>
      <div><label>Выберите раздел для поиска:</label><br />
      <select name='searchengine[]' id="search_list" multiple size=5>
      <{foreach from=$data item=ad key=id}><option value='<{$id}>'><{$ad.options.name}></option><{/foreach}>
      <option value='-1'>Искать во всех разделах (поиск осуществляется дольше)</option>
      </select></div>
      <div><input type='submit' value='Искать' id="conf_submit" /></div>
      </form>
<{/block}>