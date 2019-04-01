<script type='text/javascript'>
try {
mPhotosArray = new Array();
<{foreach from=$data.photo item=ad key=id}>
mPhotosArray[<{$id}>] = { "id":"<{$id}>", "preview_file":"<{$ad.preview_file}>", "main_file":"<{$ad.main_file}>" };
<{/foreach}>
} catch (err) { alert(err); }

function photo_delete(tr_node)
{
    try {  
        var data = tr_node.id.match(/tr_res_(\d+)/i);
        if ( data == null ) { return ;}

        if ( typeof(mPhotosArray[data[1]]) == 'undefined' ) { return ; }
        if ( confirm('Вы действительно хотите удалить фотографию '+mPhotosArray[data[1]]['main_file']+'"?') )
        {
            var aj = new ajaxRequest();
            aj.load("/admin/madmin/@Module.UrlName/item_photo_delete/<{$data.id}>/"+mPhotosArray[data[1]]['main_file'],'photo_result');
        }
    } catch(err) { alert(err); }
}

function photo_delete_res(id,stat,text)
{
    try {
        alert(text);
        if ( stat == 1 )
        {
            var a = new Array();
            for ( var i in mPhotosArray )
            {
                if ( mPhotosArray[i]['main_file'] != id )
                {
                    a[i] = mPhotosArray[i];
                } else {
                    $('#table_results').find('#tr_res_'+i).remove();
                }
            }
            mPhotosArray = a;
        }
    } catch(err) { alert(err); }
}


$(document).ready(function(){
    $("#block").hide();
    try {
        $("#photo_view").click(function(){$(this).fadeOut("");});
        $(".preview").css({"cursor":"pointer"});
        $('#table_results').tablefill(mPhotosArray,function(tr_elem,data){
            $(tr_elem).children().each(function(ichild){
                if ( ichild == 0 ) $(this)[0].innerHTML = data['id'];
                else if ( ichild == 1 ) $(this)[0].innerHTML = data['main_file'];
                else if ( ichild == 2 ) $(this).find('img.preview').attr('src','data/photo/'+data['preview_file']);
            });                
        },null,function(){
            $('img.preview').click(function(){
                try {
                    var data = $(this).parent().parent().attr('id').match(/tr_res_(\d+)/i);
                    if ( data == null ) { return ;}

                    if ( typeof(mPhotosArray[data[1]]) == 'undefined' ) { return ; }
                    $("#photo_view > img").attr('src','data/photo/'+mPhotosArray[data[1]]['main_file']);
                    $("#photo_view").fadeIn();
                } catch(err) { alert(err); }
            });

            $('a.photo_delete').click(function(){
                photo_delete($(this).parent().parent()[0]);
                return false;
            });
            
        });


        aj = new ajaxRequest();
        aj.load_form('form_add_photo',null,'add_photo_result');
        
        changeTitle('Управление фотографиями товара');
        getResultAnim($("#form_add_photo"),$("#add_photo_result"));
    } catch (err) { alert(err); }
});
</script>
<h2>Фотографии товара №<{$data.id}></h2>
<div id='photo_result'></div>
 <table id='table_results' class='admtable'>
 <tr>
  <th style="width:15px">№</th>
  <th style="width:250px">Имя файла</th>
  <th style="width:250px">Просмотр</th>
  <th>Действия</th>
 </tr>
 <tr id='not_founded' style='display:none;'>
  <td class="center" colspan='10'>
   Ничего не найдено.
  </td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center"></td>
  <td class="center"></td>  
  <td class="center"><img src='' class='preview'></td>  
  <td>
   <a href='' class="photo_delete">удалить</a><br>
  </td>
 </tr>  
 <tr>
  <td class="center" colspan='10'>
   <form action='/admin/madmin/@Module.UrlName/item_photo_new/<{$data.id}>' method='post' id='form_add_photo'>
    Выберите изображение, нажав кнопку "Обзор", затем нажмите "Загрузить".
    <input type="file" name='photo[]' class="radio" value=''>&nbsp;<input type='submit' value='Загрузить' style="padding:0;"><br>
    <div id='add_photo_result'></div>
   </form>
   <p style="float:left"><a href="/admin/mnadmin/@Module.UrlName/item_photo/<{$data.id}>" title="">Обновить список фотографий</a></p>
  </td>
 </tr>
</table>

