<script type='text/javascript'>
function in_array(what, where) 
{
    for(var i=0; i<where.length; i++) if(what['value'] == where[i]['value']) return true;
    return false;
}

function getType(value)
{
    var data = value.match(/(\d+)_([\+\-]?\d+)/i);
    if ( data == null ) return 0;
    return data[1];
}

$(document).ready(function() {
    try {
        $("#block").hide();
        changeTitle('Сортировка страниц');             

        aj = new ajaxRequest();
        aj.load_form('form_ae',null,'conf_result');
        
        $('a#move_up').click(function(){
            try {
                var selected = $('#pages_place option:selected').val();
                var type = getType(selected);
                
                var arr = new Array();
                $('#pages_place option').each(function(i,child){
                    arr[arr.length] = {'text':$(child).text(),'value':$(child).val()};
                });

                if ( type == 1 )
                for ( var i=0; i < arr.length; i++ )
                {
                    if ( arr[i]['value'] == selected && i > 1 )
                    {
                        var type2 = getType(arr[i-1]['value']);
                        if ( type == 1 && type2 == 2 ) break;
                        
                        var t = arr[i-1]['text'];
                        var v = arr[i-1]['value'];
                        
                        arr[i-1]['text'] = arr[i]['text'];
                        arr[i-1]['value'] = arr[i]['value'];
                        
                        arr[i]['text'] = t;
                        arr[i]['value'] = v;
                        
                        break;
                    }
                }
                else if ( type == 2 )
                {
                    var sel2 = -1;
                    for ( var i=0; i < arr.length; i++ )
                    {
                        if ( arr[i]['value'] == selected && i > 1 )
                        {
                            var arr2 = new Array();
                            arr2[arr2.length] = arr[i];
                            
                            for ( var i2=i+1; i2 < arr.length; i2++ )
                            {
                                var type3 = getType(arr[i2]['value']);
                                if ( type3 == 2 ) break;
                                
                                arr2[arr2.length] = arr[i2];                                
                            }
                            
                            var arr_new = new Array();
                            for ( var i2=0; i2 < arr.length; i2++ )
                            {
                                if ( i2 == sel2 )
                                {
                                    for ( var i3=0; i3 < arr2.length; i3++ )
                                    {
                                        arr_new[arr_new.length] = arr2[i3];                                
                                    }
                                }
                                if ( in_array(arr[i2],arr2) ) continue;
                                arr_new[arr_new.length] = arr[i2];                                
                            }
                            arr = arr_new;
                            
                            break;
                        }
                        
                        var type2 = getType(arr[i]['value']);
                        if ( type2 == 2 ) sel2 = i;                        
                    }
                    
                }
                
                
                $('#pages_place option').remove();
                for ( var i=0; i < arr.length; i++ )
                {
                    var opt = new Option(arr[i]['text'],arr[i]['value']);
                    if ( getType(arr[i]['value']) == 2 ) $(opt).addClass('my_fields');
                    $('#pages_place')[0].options[$('#pages_place')[0].options.length] = opt;
                }
                $('#pages_place option[value='+selected+']').attr('selected','selected');
            } catch (err) { alert(err); }
            return false;       
        });
        
        $('a#move_down').click(function(){
            try {
                var selected = $('#pages_place option:selected').val();
                var type = getType(selected);
                
                var arr = new Array();
                $('#pages_place option').each(function(i,child){
                    arr[arr.length] = {'text':$(child).text(),'value':$(child).val()};
                });

                if ( type == 1 )
                for ( var i=0; i < arr.length; i++ )
                {
                    if ( arr[i]['value'] == selected && i < (arr.length-1) )
                    {
                        var type2 = getType(arr[i+1]['value']);
                        if ( type == 1 && type2 == 2 ) break;

                        var t = arr[i+1]['text'];
                        var v = arr[i+1]['value'];
                        
                        arr[i+1]['text'] = arr[i]['text'];
                        arr[i+1]['value'] = arr[i]['value'];
                        
                        arr[i]['text'] = t;
                        arr[i]['value'] = v;
                        
                        break;
                    }
                }
                else if ( type == 2 )
                {
                    var sel2 = -2;
                    var sel = -1;
                    for ( var i=0; i < arr.length; i++ )
                    {
                        var type2 = getType(arr[i]['value']);
                        if ( type2 == 2 && sel2 == -1 )
                        {
                            sel2 = i;
                            
                            var arr2 = new Array();
                            arr2[arr2.length] = arr[sel];
                            for ( var i2=sel+1; i2 < arr.length; i2++ )
                            {
                                var type3 = getType(arr[i2]['value']);
                                if ( type3 == 2 ) break;
                                
                                arr2[arr2.length] = arr[i2];                                
                            }
                            
                            for ( var i2=i+1; i2 < arr.length; i2++ )
                            {
                                var type3 = getType(arr[i2]['value']);
                                if ( type3 == 2 ) break;
                                sel2 = i2;
                                if ( i2 == (arr.length-1) ) break;
                            }
                            
                            var arr_new = new Array();
                            for ( var i2=0; i2 < arr.length; i2++ )
                            {
                                if ( in_array(arr[i2],arr2) ) continue;
                                arr_new[arr_new.length] = arr[i2];                                
                                if ( i2 == sel2 )
                                {
                                    for ( var i3=0; i3 < arr2.length; i3++ )
                                    {
                                        arr_new[arr_new.length] = arr2[i3];                                
                                    }
                                }
                            }
                            arr = arr_new;
                            
                            break;
                        }

                        if ( arr[i]['value'] == selected ) {sel = i;sel2 = -1;}
                    }
                    
                }
                
                
                $('select#pages_place option').remove();
                for ( var i=0; i < arr.length; i++ )
                {
                    var opt = new Option(arr[i]['text'],arr[i]['value']);
                    if ( getType(arr[i]['value']) == 2 ) $(opt).addClass('my_fields');
                    $('#pages_place')[0].options[$('#pages_place')[0].options.length] = opt;
                }
                $('#pages_place option[value='+selected+']').attr('selected','selected');
            } catch (err) { alert(err); }
            return false;       
        });
        
        $('#conf_submit').click(function(){
            $('select#pages_place option').attr('selected','selected');
        });
    } catch(err) { alert(err); }
});

</script>
<h2>Сортировка страниц</h2>
<br>
<table id="items_results">
 <tr>
  <td>
   <form action='/admin/madmin/@Module.UrlName/pages_place_save' method='post' id='form_ae'>
   <select id='pages_place' name='pages_place[]' size=10 multiple>
   
   <{foreach from=$data_cats item=ad key=id}>
    <option value='2_<{$id}>' style="background-color:#C0C0C0"><{$ad}></option>
     <{foreach from=$data_cpages[$id] item=ad2 key=id2}>
      <option value='1_<{$data_pages[$id2].id}>'><{$data_pages[$id2].name}></option>
     <{/foreach}>
   <{/foreach}>
   </select><br>
   <a href='' id='move_up'>Поднять поле выше</a><br>
   <a href='' id='move_down'>Опустить поле ниже</a><br>

   <input type='submit' id='conf_submit' value='Сохранить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
   </form>
   <div id='conf_result'></div>
  </td>
 </tr>
</table>
