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
    
    changeTitle("Просмотр видеофайла: <{$data.name|java_string}>");
});

</script>

<h2><a href='/admin/madmin/@Module.UrlName/galls_view/<{$data.gallery}>' class='this_view'><{$data.gallery_name}></a> &raquo; <{$data.name}></h2>
<p><{$data.text}></p>

<div id="video_div" style="padding:0;padding-bottom:50px;">
 <p style="padding:0 15px;margin-top:15px;font-size:14px;"><a href="/data/video/<{$data.file.main_file}>" title="" id="video_image"><img src="/data/video/<{$data.file.preview_file}>" alt="" style="float:left;margin:0 10px 10px 10px;"></a><{$data.text}><br></p>
 <small style="margin-left:15px;margin-top:10px;"><{$data.date|strftime:"%e %b %Y"}></small><br>
</div>
