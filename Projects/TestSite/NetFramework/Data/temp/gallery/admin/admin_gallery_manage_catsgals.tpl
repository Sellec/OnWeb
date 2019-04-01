<script type='text/javascript'>
function cats_delete(tr_node)
{
    try {  
        var data = tr_node.id.match(/tr_res_(\d+)/i);
        if ( data == null ) { return ;}

        if ( typeof(mCatsArray[data[1]]) == 'undefined' ) { return ; }
        if ( confirm('Вы действительно хотите удалить категорию №'+data[1]+' - "'+mCatsArray[data[1]]['name']+'"?') )
        {
            var aj = new ajaxRequest();
            aj.load("/admin/madmin/@Module.UrlName/cats_delete/"+String(data[1]),'cats_result');
        }
    } catch(err) { alert(err); }
}

function cats_delete_res(id,stat,text)
{
    try {
        alert(text);
        if ( stat == 1 )
        {
            $('#cats_results').find('#tr_res_'+id).remove();
            var a = new Array();
            for ( var i in mCatsArray )
            {
                if ( i != id )
                {
                    a[i] = mCatsArray[i];
                }
            }
            mCatsArray = a;
        }
    } catch(err) { alert(err); }
}

function galls_delete(tr_node)
{
    try {  
        var data = tr_node.id.match(/tr_res_(\d+)/i);
        if ( data == null ) { return ;}

        if ( typeof(mGalleryArray[data[1]]) == 'undefined' ) { return ; }
        if ( confirm('Вы действительно хотите удалить галерею №'+data[1]+' - "'+mGalleryArray[data[1]]['name']+'"?') )
        {
            var aj = new ajaxRequest();
            aj.load("/admin/madmin/@Module.UrlName/galls_delete/"+String(data[1]),'gals_result');
        }
    } catch(err) { alert(err); }
}

function galls_delete_res(id,stat,text)
{
    try {
        alert(text);
        if ( stat == 1 )
        {
            $('#gals_results').find('#tr_res_'+id).remove();
            var a = new Array();
            for ( var i in mGalleryArray )
            {
                if ( i != id )
                {
                    a[i] = mGalleryArray[i];
                }
            }
            mGalleryArray = a;
        }
    } catch(err) { alert(err); }
}

function photo_delete(tr_node)
{
    try {  
        var data = tr_node.id.match(/tr_res_(\d+)/i);
        if ( data == null ) { return ;}

        if ( typeof(mPhotoArray[data[1]]) == 'undefined' ) { return ; }
        if ( confirm('Вы действительно хотите удалить фотографию №'+data[1]+' - "'+mPhotoArray[data[1]]['name']+'"?') )
        {
            var aj = new ajaxRequest();
            aj.load("/admin/madmin/@Module.UrlName/photo_delete/"+String(data[1]),'photo_result');
        }
    } catch(err) { alert(err); }
}

function photo_delete_res(id,stat,text)
{
    try {
        alert(text);
        if ( stat == 1 )
        {
            $('#photos_results').find('#tr_res_'+id).remove();
            var a = new Array();
            for ( var i in mPhotoArray )
            {
                if ( i != id )
                {
                    a[i] = mPhotoArray[i];
                }
            }
            mPhotoArray = a;
        }
    } catch(err) { alert(err); }
}

function photo_canload(id)
{
    try {
    if ( id == -1 ) $("#dis > *").hide();
    else {
        $("#dis > *").fadeIn();
        $("#b_add").fadeOut();
        aj = new ajaxRequest();
        aj.load_file('upload1','photo[]','/admin/madmin/@Module.UrlName/photo_photo_new/'+id,'progress1');
    }
    } catch(err) { alert(err); }
}

function video_delete(tr_node)
{
    try {  
        var data = tr_node.id.match(/tr_res_(\d+)/i);
        if ( data == null ) { return ;}

        if ( typeof(mVideoArray[data[1]]) == 'undefined' ) { return ; }
        if ( confirm('Вы действительно хотите удалить видео №'+data[1]+' - "'+mVideoArray[data[1]]['name']+'"?') )
        {
            var aj = new ajaxRequest();
            aj.load("/admin/madmin/@Module.UrlName/video_delete/"+String(data[1]),'photo_result');
        }
    } catch(err) { alert(err); }
}

function video_delete_res(id,stat,text)
{
    try {
        alert(text);
        if ( stat == 1 )
        {
            $('#videos_results').find('#tr_res_'+id).remove();
            var a = new Array();
            for ( var i in mVideoArray )
            {
                if ( i != id )
                {
                    a[i] = mVideoArray[i];
                }
            }
            mVideoArray = a;
        }
    } catch(err) { alert(err); }
}

function video_canload(id)
{
    try {
    if ( id == -1 ) $("#dis > *").hide();
    else {
        $("#dis > *").fadeIn();
        $("#b_add").fadeOut();
        aj = new ajaxRequest();
        aj.load_file('upload1','video[]','/admin/madmin/@Module.UrlName/video_video_new/'+id,'progress1');
    }
    } catch(err) { alert(err); }
}

</script>
