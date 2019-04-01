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

function news_delete(tr_node)
{
    try {  
        var data = tr_node.id.match(/tr_res_(\d+)/i);
        if ( data == null ) { return ;}

        if ( typeof(mNewsArray[data[1]]) == 'undefined' ) { return ; }
        if ( confirm('Вы действительно хотите удалить новость №'+data[1]+' - "'+mNewsArray[data[1]]['name']+'"?') )
        {
            var aj = new ajaxRequest();
            aj.load("/admin/madmin/@Module.UrlName/news_delete/"+String(data[1]),'news_result');
        }
    } catch(err) { alert(err); }
}

function news_delete_res(id,stat,text)
{
    try {
        alert(text);
        if ( stat == 1 )
        {
            $('#news_results').find('#tr_res_'+id).remove();
            var a = new Array();
            for ( var i in mNewsArray ) if ( i != id ) a[i] = mNewsArray[i];
            mNewsArray = a;
        }
    } catch(err) { alert(err); }
}

</script>
