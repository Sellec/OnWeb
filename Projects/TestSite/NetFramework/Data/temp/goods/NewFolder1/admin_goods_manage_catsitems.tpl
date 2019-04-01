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

function cats_delete_end(tname,resid)
{
    try {
        $('#'+tname).find('#tr_res_'+resid).remove();
    } catch(err) { alert(err); }
}

function cats_delete_res(id,stat,text)
{
    try {
        alert(text);
        if ( stat == 1 )
        {
            cats_delete_end('cats_results',id);
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

function item_delete(tr_node)
{
    try {  
        var data = tr_node;//.id.match(/tr_res_(\d+)/i);
        if ( data == null ) { return ;}

        //if ( typeof(mItemsArray[data[1]]) == 'undefined' ) { return ; }
        if ( confirm('Вы действительно хотите удалить наименование №'+data+'?') )
        {
            var aj = new ajaxRequest();
            aj.load("/admin/madmin/@Module.UrlName/item_delete/"+String(data),'items_result');
        }
    } catch(err) { alert(err); }
}

function item_delete_end(tname,resid)
{
    try {
        $('#'+tname).find('#tr_res_'+resid).remove();
    } catch(err) { alert(err); }
}

function item_delete_res(id,stat,text)
{
    try {
        alert(text);
        if ( stat == 1 )
        {
            item_delete_end('items_results',id);
            /*var a = new Array();
            for ( var i in mItemsArray )
            {
                if ( i != id )
                {
                    a[i] = mItemsArray[i];
                }
            }
            mItemsArray = a;*/
        }
    } catch(err) { alert(err); }
}

</script>
