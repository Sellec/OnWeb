<script type='text/javascript'>
/*
            $(tr_elem).find('a.pages_delete').click(function(){
                pages_delete($(this).parent().parent()[0]);
                return false;
            });
            */
$(document).ready(function() {
    $("#block").hide();
    try {                         
        changeTitle('Просмотр списка страниц без фильтрации по категориям');
        stAnim();
        
        $('select#category_select').change(function(){
            try {
                mFilterValue = $('select#category_select').val();
                update_pages();
            } catch (err) { alert(err); }
        });
        
        $("a[rel='21'],a[rel='39']").hide();
        $('a.pages_delete').click(function(){
            pages_delete($(this).attr("rel"),$(".pv_"+$(this).attr("rel")).text());
            return false;
        });

        $( "#pages_list" ).sortable({
            stop: function() {
                get_page_sort();
            }
        });
        
        
        $(".page_pos").change(function(){
            var id = $(this).parent().parent().parent().attr("id");
            var str_ord = $(this).parent().find("input[name='page_order']").val();
            var str_par = $(this).parent().find("input[name='page_parent']").val();
            $.ajax({
                type    : "POST",
                url     : "/admin&a=madmin&mod=@Module.UrlName&moda=@Module.UrlName_change_pos",
                data    : { id : id, order : str_ord, parent : str_par }
            }).done(function(res){
                alert(res)
            })
        })
    } catch(err) { alert(err); }
});

function get_page_sort(){
    $(".page_block").each(function(){
        if ($(this).parent().attr("id")=="pages_list"){
            var str_ord = $(this).index();
            var id = $(this).attr("id");
            var str_par = $(this).find("input[name='page_parent']").val();
            $.ajax({
                type    : "POST",
                url     : "/admin&a=madmin&mod=@Module.UrlName&moda=@Module.UrlName_change_pos",
                data    : { id : id, order : str_ord, parent : str_par }
            }).done(function(msg){
                $("#action_result").html(msg);
                endAnim($("#action_result"));
            })
        }
    })
}
</script>
<{include file="admin/admin_pages_manage_catspages.tpl"}>
<h2>Страницы</h2>
<p>В списке есть только те категории, в которых были найдены страницы. Пустые категории не отображены.</p>
Фильтр: <select id='category_select'>
 <option value='-1'>Все страницы</option>
<{foreach from=$data_cats item=ad key=id}>
 <option value='<{$ad.id}>'><{$ad.name}></option>
<{/foreach}>
</select><br>
<div id='pages_result' style="color:red;margin-top:10px;"></div>
<div id='pages_list'>

 <{foreach from=$data_pages item=ad key=id}>
 <div class='page_block' id='page_<{$ad.id}>'>
  <div class='page_info'><div class="pi_num">№<{$ad.id}></div>
   <div class="pi_info"><a href='/admin/mnadmin/@Module.UrlName/pages_view/<{$ad.id}>' class='pv_<{$ad.id}>' target='_blank'><{$ad.name|re_quote}></a><br>
   <input type="text" value="<{$ad.parent}>" name="page_parent" size="2" class="page_pos" /> <input type="text" value="<{$ad.order}>" name="page_order" size="2" class="page_pos" /> <span style="color:#969696;"><{$ad.urlname}></span></div>
   <div class="pi_links">
    <a href='/admin/mnadmin/@Module.UrlName/pages_edit/<{$ad.id}>' class='pages_edit'>редактировать</a><br />
    <a href='' class="pages_delete" rel="<{$ad.id}>">удалить</a>
   </div>
  </div>
  
 </div>
 <{/foreach}>

 </div>
