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

function articles_delete(tr_node)
{
    try {  
        var data = tr_node.id.match(/tr_res_(\d+)/i);
        if ( data == null ) { return ;}

        if ( typeof(marticlesArray[data[1]]) == 'undefined' ) { return ; }
        if ( confirm('Вы действительно хотите удалить наименование №'+data[1]+' - "'+marticlesArray[data[1]]['name']+'"?') )
        {
            var aj = new ajaxRequest();
            aj.load("/admin/madmin/@Module.UrlName/articles_delete/"+String(data[1]),'articles_result');
        }
    } catch(err) { alert(err); }
}

function articles_delete_res(id,stat,text)
{
    try {
        alert(text);
        if ( stat == 1 )
        {
            $('#articles_results').find('#tr_res_'+id).remove();
            var a = new Array();
            for ( var i in marticlesArray )
            {
                if ( i != id )
                {
                    a[i] = marticlesArray[i];
                }
            }
            marticlesArray = a;
        }
    } catch(err) { alert(err); }
}

</script>
