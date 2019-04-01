<script type='text/javascript'>
$(document).ready(function() {
    $("#block").hide();
    $('a.this_view').click(function(){
        try {
            var aj = new ajaxRequest();
            aj.load($(this).attr('href'),'cmain');
            return false;    
        } catch (err)
        {
            alert(err);
        }
        return false;
    });
    $("img.pr_photo").css({"cursor":"pointer"});
    $("img.pr_photo").click(function(){
        $("#photo_view > img").attr('src','data/photo/<{$data.photo[0].main_file}>');
        $("#photo_view").fadeIn();
    });
    $("#photo_view").click(function(){$(this).fadeOut("")});
    changeTitle('Просмотр баннера: <{$data.name}>');
});

</script>

<h2><a href='/admin/madmin/@Module.UrlName/cats/<{$data.category}>' class='this_view'><{$data.banners_category_name}></a> &raquo; <{$data.name}></h2>
<div style="float:left;width:150px;height:250px;text-align:center">
 <img src="/data/photo/<{$data.photo[0].preview_file}>" alt="" class='pr_photo'><br><br>
 <{foreach from=$data.photo item=ad key=id}>
 <p><img src="/data/photo/<{$ad.mini_file}>"></p>
 <{/foreach}>
</div>
<div style="float:left;width:650px;">
 <p><{$data.description}></p>
 
</div>
