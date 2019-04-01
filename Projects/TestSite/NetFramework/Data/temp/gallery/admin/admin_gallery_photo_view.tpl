<script type='text/javascript'>
$(document).ready(function() {
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
    
    changeTitle("Просмотр фотографии: <{$data.name|java_string}>");
});

</script>

<h2><a href='/admin/madmin/@Module.UrlName/galls_view/<{$data.gallery}>' class='this_view'><{$data.gallery_name}></a></h2>
<p><{$data.text}></p>

<div id="photo_div" style="padding:0;padding-bottom:50px;">
 <p style="padding:0 15px;margin-top:15px;font-size:14px;"><a href="/data/photo/<{$data.photo.main_file}>" title="" id="photo_image"><img src="/data/photo/<{$data.photo.preview_file}>" alt="" style="float:left;margin:0 10px 10px 10px;"></a><{$data.text}><br></p>
 <small style="margin-left:15px;margin-top:10px;"><{$data.date|strftime:"%e %b %Y"}></small><br>
</div>
