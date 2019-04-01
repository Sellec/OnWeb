<script type='text/javascript'>
//var Subscriptions = <{$subscriptions|@jsobject}>;

function delete_res_s(id,stat,text)
{
    try {
        $('div#subscription_result').text(text);
        if ( stat == 1 )
        {
            $('select#subscriber_id option[value='+id+']').remove();
        }
    } catch(err) { alert(err); }
}

function delete_res(id,stat,text)
{
    try {
        $('div#subscription_result').text(text);
        if ( stat == 1 )
        {
            $('select#subscription_id option[value='+id+']').remove();
        }
    } catch(err) { alert(err); }
}

function add_res(stat,id,name)
{
    if ( stat == 1 )
    {
        $('select#subscription_id')[0].options[$('select#subscription_id')[0].options.length] = new Option(name,id);
    }
}

$(document).ready(function() {
    $("#block").hide();
    try {
        changeTitle('Просмотр списка подписок');
        
        $('a#mode_edit').click(function(){
            var val = $('select#subscription_id').val();
            $(this).attr('href','/admin/mnadmin/@Module.UrlName/edit/'+val);
        });

        $('a#mode_delete').click(function(){
            var val = $('select#subscription_id').val();
            
            if ( confirm('Вы действительно хотите удалить данные для подписки "'+$('select#subscription_id option[value='+val+']').text()+'"') )
            {
                aj = new ajaxRequest();
                aj.load('/admin/madmin/@Module.UrlName/delete/'+val,'del_res');
            }
            return false;
        });

        $('a#mode_add').click(function(){
            aj = new ajaxRequest();
            aj.load('/admin/madmin/@Module.UrlName/add','subscription_result');
            return false;
        });

        $('a#mode_vss').click(function(){
            $("#del_res").text('');
            $.requestJSON('/admin/madmin/@Module.UrlName/subscribers_list/'+$('select#subscription_id').val(), null, function(result, message, data){
                if (result == JsonResult.OK) 
                {
                    $('select#subscriber_id option').remove();
                    $.each(data, function(key, value){ $('select#subscriber_id').append($("<option></option>").val(key).text(value.email)) });
                }
                if (strlen(message) > 0) $("#del_res").html(message);
            });
            return false;
        });

        $('a#smode_delete').click(function(){
            var val = $('select#subscriber_id').val();
            
            if ( confirm('Вы действительно хотите удалить подписчика "'+$('select#subscriber_id option[value='+val+']').text()+'" в подписке "'+$('select#subscription_id option:selected').text()+'"?') )
            {
                aj = new ajaxRequest();
                aj.load('/admin/madmin/@Module.UrlName/subscribers_delete/'+val,'subscription_result');
                var div_res = $("#subscription_result");
                div_res.fadeIn("slow");
                setTimeout(function(){div_res.fadeOut("slow")}, 2500);
                var opros = setInterval(function(){
                    if ( div_res.text() != "" ) {
                        setTimeout(function(){div_res.text("");}, 3500 );
                        clearInterval(opros);
                    };
                }, 1000);
            }
            return false;
        });

    } catch(err) { alert(err); }
});
</script>
<h2>Подписки</h2>

<table width='800' class='table_fields'>
 <tr>
  <td width='400' class="capt">
    Выберите подписку: <select id='subscription_id' size=10>
    <{foreach from=$subscriptions item=ad key=id}>
     <option value='<{$ad.id}>'><{$ad.name}></option>
    <{/foreach}>
    </select><br>
    <p><a href='' id='mode_vss'>Смотреть список подписчиков</a></p>
    <a href='' id='mode_add'>Добавить</a>&nbsp;|&nbsp;
    <a href='' id='mode_edit'>Редактировать</a>&nbsp;|&nbsp;
    <a href='' id='mode_delete'>Удалить</a>&nbsp;
  </td>
  <td width='400' class="capt">
    Выберите подписчика: <select id='subscriber_id'>
    </select><br>
    <a href='' id='smode_delete'>Удалить</a>&nbsp;
  </td>
 </tr>
</table>

<div id='del_res' style='display:none;'></div>
<div id='subscription_result' style="color:red;margin-top:10px;"></div>