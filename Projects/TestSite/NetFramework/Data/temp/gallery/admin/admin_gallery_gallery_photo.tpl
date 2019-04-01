<script type='text/javascript'>
$(document).ready(function() {
    $("#block").hide();

    function photo_del(fname){
        try {
            if ( fname == 'undefined' ) { return ; }
            if ( confirm('Вы действительно хотите удалить фотографию "'+fname+'"?') )
            {
                var aj = new ajaxRequest();
                aj.load("/admin/madmin/@Module.UrlName/item_photo_delete/<{$data.id}>&name="+fname,'photo_result');
            }
        } catch(err) { alert(err); }
    }

    function photo_delete_res(id,stat,text)
    {
        try {
            alert(text);
            if ( stat == 1 )
            {
                $('#photos').find('#file_'+id).remove();
            }
        } catch(err) { alert(err); }
    }
    
   /* aj2 = new ajaxRequest();
    aj2.userOnLoad = function(){endAnim($("#item_result"))};
    aj2.load_form('order_save',null,'item_result');
    stAnim();*/
    
    $( "#photos" ).sortable({stop:function(){
        var unun = '';
        $("#photos li").each(function(){
            unun = unun + $(this).attr("id")+',';
        })
        $("#order_save input[name='unun']").val(unun);
        $("#order_save").submit();
    }});
    $( "#photos" ).disableSelection();
    
    changeTitle('Просмотр галереи');
});
</script>
<{include file="admin/admin_gallery_manage_catsgals.tpl"}>

<h2>Фотографии</h2>
<div id='photo_result' style="color:red;margin-top:10px;"></div>
<table id='photos_results' class="admtable">
 <tr>
  <th style="width:15px">№</th>
  <th style="width:565px">Фото</th>
  <th style="width:100px">Действия</th>
 </tr>
 <tr id='notfounded'>
  <td colspan='3'>Ничего не найдено</td>
 </tr>
 <tr id='obraz' style='display:none;'>
  <td class="center"></td>
  <td><a href='' class='photo_view' target="_blank"><img src="" alt=""></a></td>
  <td>
   <!--<a href='/admin/mnadmin/@Module.UrlName/photo_edit/' class='photo_edit'>редактировать</a><br>-->
   <a href='' class="photo_delete">удалить</a><br>
  </td>
 </tr>  
</table>

<form action="/admin/madmin/@Module.UrlName/item_photo_order" id="order_save" method="POST" style="display:none;">
    <input type="hidden" value="<{$data.id}>" name="id" />
    <input type="hidden" value="" name="unun" />
    <input type="submit" value="&nbsp;Сохранить обновленный порядок&nbsp;" />
</form>

<ul id="photos">
 <{foreach from=$data_photo item=ad key=id}>
  <li id="file_<{$ad.photo.main_file}>">
   <div class="ph_num"><{$id}></div>
   <div class="ph_name"><{$ad.photo.main_file}></div>
   <div class="ph_img"><img src='/data/photo/<{$ad.photo.preview_file}>' rel='/data/photo/<{$ad.photo.main_file}>' class='preview' /></div>
   <div class="ph_link"><a href='' class='ph_del' rel="<{$ad.photo.main_file}>">удалить</a></div>
  </li>
 <{/foreach}>
</ul><br /><br />