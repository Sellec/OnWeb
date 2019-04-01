<script type='text/javascript' src='/data/js/ajaxupload.3.5.js'></script>
<script type='text/javascript'>
$(document).ready(function(){
    try {
    $("#block").hide();
    aj = new ajaxRequest();
    aj.userOnLoad = function(){endAnim($("#item_result"))};
    aj.load_form('form_ae',null,'item_result');
    stAnim();
    
    <{if $data.id == -1}>
    changeTitle('Добавление товара');
    $('div#added_item_photo').hide();
    <{else}>
    changeTitle('Редактирование товара "<{$data.name}>"');
    <{/if}>
    
     var ckeditor = CKEDITOR.replace( 'it_descr',{removePlugins: 'save'} );
    $('#save_func').click(function(){
        CKEDITOR.instances.it_descr.updateElement();
    });

    CKFinder.setupCKEditor( ckeditor, '/ckfinder/' );
    
    $(".gettop").click(function(){scroll(0,0);return false}); 
   
<{if $data.id != -1}>
    $('select#changed_category').change(function(){
        aj = new ajaxRequest();
        aj.setPOST('cat',$('select#changed_category').val());
        aj.load('/admin/madmin/@Module.UrlName/item_edit/<{$data.id}>','cmain');        
    });
<{/if}>

    var btnUpload   =   $('#img_icon_btn');
    var status      =   $('#img_icon_stat');
    startUp(btnUpload,status,"img_icon");
    
    var btnUpload_2   =   $('#img_bimage_btn');
    var status_2      =   $('#img_bimage_stat');
    startUp(btnUpload_2,status_2,"img_bimage");
    
    var btnUpload_3   =   $('#img_photo_btn');
    var status_3      =   $('#img_photo_stat');
    startUp(btnUpload_3,status_3,"img_photo");
    
    $(".del_photo").click(function(){
        if (confirm("Удалить изображение?")){
            btn = $(this).parent().find(".load_buttons");
            img = $(this).parent().find("img");
            field = $(this).parent().find("input[type='text']");
            link = $(this);
            $.ajax({
                type:'POST',
                url:$(this).attr("href"),
                data:{url:$(this).attr("rel")}
            }).done(function(result){
                if (result == 1){
                    img.attr("src","/data/img/design/off.jpg");
                    field.val("");
                    alert("Файл успешно удален!");
                    link.hide();
                    btn.show();
                } else if (result==0){
                    alert("Не получилось удалить файл");
                    img.attr("src","/data/img/design/off.jpg");
                    field.val("");
                    link.hide();
                    btn.show();
                } else alert(result);
            })
            $("#changed").val("1");
        }
        
        return false;
    })
    
    $("#form_ae input,#form_ae textarea,#form_ae select").change(function(){
        $("#changed").val("1");
    })
    $("#form_ae").submit(function(){
        $("#changed").val("0");
    })
    
    } catch(err) {alert(err);}
});

function startUp(btnUpload,status,targ){
    var fname = '<{if $data.id!=-1}><{$data.id}><{else}><{$new_id}><{/if}>';
    var itype = '';
    var mk_icon = 0;
    if (targ == "img_icon") {fname = fname + "_icon"; itype='icon';}
    if (targ == "img_photo") {fname = fname + "_photo"; itype='photo';}
    if (targ == "img_bimage") {fname = fname + "_bimage"; itype='bimage';}
    
    new AjaxUpload(btnUpload, {
        action: '/admin/madmin/fm/upload_product',
        name: 'upload',
        data: {fname:fname, itype:itype, directory:'data/photo/'},
        onSubmit: function(file, ext){
            if (! (ext && /^(jpg|png|jpeg|gif)$/.test(ext))){
                status.text('Только JPG, PNG или GIF!');
                return false;
            }
            status.text('Загрузка...');
        },
        onComplete: function(file, data){
            status.text('');
            var response = JSON.parse(data);
            
            if(response.error==0){
                $("input[name='"+targ+"']").val(response.file);//alert(file)
                
                if (targ == "img_icon"){
                    $("#item_icon").attr("src","/"+response.file);
                    $(".del_sm").attr("rel",response.file).show();
                    $("#img_icon_btn").hide();
                }
                if (targ == "img_bimage"){
                    $("#item_bimage").attr("src","/"+response.file);
                    $(".del_bi").attr("rel",response.file).show();
                    $("#img_bimage_btn").hide();
                }
                if (targ == "img_photo"){
                    $("#item_photo").attr("src","/"+response.file);
                    $(".del_ph").attr("rel",response.file).show();
                    $("#img_photo_btn").hide();
                }
                $("#changed").val("1");
            } else {
                alert(response.msg)
            }
            
            startUp(btnUpload,status,targ)
        }
    });
}
window.onbeforeunload = function (evt){
    var message = "Документ не был сохранен после внесения изменений! Закрыв страницу, вы их потеряете...";
    if ($("#changed").val()==1){
        if (typeof evt == "undefined"){
            evt = window.event;
        }
        if (evt) {
            evt.returnValue = message;
        }
        return message;
    }
}
</script>

<input type="hidden" name="changed" id="changed" value="0" />
<{if $data.id == -1}>
<form action='/admin/madmin/@Module.UrlName/item_add_save' method='post' id='form_ae'>
<{else}>
<a href='' id='back'>&larr; Назад к категории</a><br>
<form action='/admin/madmin/@Module.UrlName/item_edit_save/<{$data.id}>' method='post' id='form_ae'>
<{/if}>
<{if $data.name == ""}><h2>Добавление товара</h2><{else}><h2>Редактирование товара</h2><{/if}>
<table width='900' id="items_results" class="admtable">    
 <tr>
  <th colspan="4">Общие параметры</th>
 </tr>
 <tr>
  <td width="175">Название:<input type="hidden" name="item_name" value="2"></td>
  <td colspan='3'><input type='text' name='item_name' size='60' maxlength='200' value='<{$data.name}>' style='margin-bottom:10px;'><br>
 </tr>
 <tr>
  <td>Вес:</td>
  <td><input type='text' id='item_weight' name='item_weight' size='35' value='<{$data.weight}>'></td>
  <td>Срок хранения:</td>
  <td><input type='text' id='item_time' name='item_time' size='35' value='<{$data.time}>'></td>
 </tr>
 <tr>
  <td>Тип:</td>
  <td><input type='text' id='item_type' name='item_type' size='35' value='<{$data.type}>'></td>
  <td>Упаковка:</td>
  <td><input type='text' id='item_pack' name='item_pack' size='35' value='<{$data.pack}>'></td>
 </tr>
 <tr>
  <td>Цена:</td>
  <td><input type='text' id='item_price' name='item_price' size='10' value='<{$data.price}>'> рублей</td>
  <td>Артикул/ГОСТ:<input type="hidden" name="item_article" value="2"></td>
  <td><input type='text' name='item_article' size='20' maxlength='200' value='<{$data.article}>' style='margin-bottom:10px;'><br>
 </tr>
 <tr>
  <td>Описание:<input type="hidden" name="item_descr" value="2"></td>
  <td colspan='3'><textarea name='item_descr' rows='4' cols='60' id="it_descr"><{$data.description}></textarea></td>
 </tr>
 <tr>
  <td>Описание внизу:<input type="hidden" name="item_descr_2" value="2"></td>
  <td colspan='3'><textarea name='item_descr_2' rows='5' cols='60' id="it_descr_2"><{$data.description_2}></textarea></td>
 </tr>
 <tr>
  <td>Фотография (иконка):</td>
  <td>
    <input type='text' name='img_icon' size='20' value="<{$data.photo.item_icon}>">
    <div id="img_icon_btn" class="load_buttons">Загрузить</div>
    <div id="img_icon_stat"></div>

    <br /><img src='/<{if $data.photo.item_icon|strlen>0}><{$data.photo.item_icon}><{else}>data/img/design/off.jpg<{/if}>' alt='' id='item_icon' /><br />
    <a href="/admin/madmin/@Module.UrlName/del_photo" rel="<{if $data.photo.item_icon|strlen>0}><{$data.photo.item_icon}><{/if}>" class="del_photo del_sm" title=""<{if $data.photo.item_icon|strlen==0}> style="display:none;"<{/if}>>Удалить файл</a>
  </td>
  <td>Фотография в &laquo;книжке&raquo; продукта:</td>
  <td>
    <input type='text' name='img_bimage' size='20' value="<{$data.photo.item_bimage}>">
    <div id="img_bimage_btn" class="load_buttons">Загрузить</div>
    <div id="img_bimage_stat"></div>

    <br /><img src='/<{if $data.photo.item_bimage|strlen>0}><{$data.photo.item_bimage}><{else}>data/img/design/off.jpg<{/if}>' alt='' id='item_bimage' />
    <a href="/admin/madmin/@Module.UrlName/del_photo" rel="<{if $data.photo.item_bimage|strlen>0}><{$data.photo.item_bimage}><{/if}>" class="del_photo del_bi" title=""<{if $data.photo.item_bimage|strlen==0}> style="display:none;"<{/if}>>Удалить файл</a>
  </td>
 </tr>
 <tr>
  <td>Большая фотография (при нажатии):</td>
  <td colspan="3">
    <input type='text' name='img_photo' size='20' value="<{$data.photo.item_photo}>">
    <div id="img_photo_btn" class="load_buttons">Загрузить</div>
    <div id="img_photo_stat"></div>

    <br /><img src='/<{if $data.photo.item_photo|strlen>0}><{$data.photo.item_photo}><{else}>data/img/design/off.jpg<{/if}>' alt='' id='item_photo' /><br />
    <a href="/admin/madmin/@Module.UrlName/del_photo" rel="<{if $data.photo.item_photo|strlen>0}><{$data.photo.item_photo}><{/if}>" class="del_photo del_ph" title=""<{if $data.photo.item_photo|strlen==0}> style="display:none;"<{/if}>>Удалить файл</a>
  </td>
 <tr>
  <td colspan="2">
  <{if $data.id == -1}>
  <input type='submit' id='save_func' value='Добавить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
  <{else}>
  <input type='submit' id='save_func' value='Сохранить'> <img src="/data/img/loading.gif" alt="" id="loading_img">
  <{/if}>
  </td>
 </tr>
 <tr>
  <td>Категория:<input type="hidden" name="item_cat" value="2"></td>
  <td colspan='3'>
<{if $data.id == -1}>
    <select disabled>
<{else}>
    <select id='changed_category'>
<{/if}>
<{foreach from=$cats_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.category}>selected<{/if}>><{$ad}></option>
<{/foreach}>
    </select>
    <input type='hidden' name='item_cat' value='<{$data.category}>'>
  </td>
 </tr>
 <tr>
  <td>Статус:<input type="hidden" name="item_status" value="2"></td>
  <td colspan='3'>
  <{if $data.id == -1}>
    <select name='item_status'>
     <option value='0'>Отключено</option>
     <option value='1' selected>Доступно для просмотра</option>
    </select>
  <{else}>
    <select name='item_status'>
     <option value='0' <{if $data.status == 0}>selected<{/if}>>Отключено</option>
     <option value='1' <{if $data.status == 1}>selected<{/if}>>Доступно для просмотра</option>
    </select>
   <{/if}>  
  </td>
 </tr>
 <tr style="display:none;">
  <td>Дополнительная категория:<input type="hidden" name="item_cat2" value="2"></td>
  <td colspan='3'>
    <select name='item_cat2'>
     <option value='-1' >игнорировать</option>
<{foreach from=$addcats_data item=ad key=id}>
     <option value='<{$id}>' <{if $id == $data.category2}>selected<{/if}>><{$ad}></option>
<{/foreach}>
    </select>
  </td>
 </tr>
 <tr style="display:none;">
  <td>Варианты:<input type="hidden" name="item_sizes_" value="2"></td>
  <td colspan='3'>
  <{assign var="minus" value='-1'}>
    <input type='checkbox' id='sizes_status' name='item_sizes[]' value='-1' <{if isset($data.sizes[$minus])}>checked<{else}><{if $data.id == -1}>checked<{/if}><{/if}>>&nbsp;Отключить<br>
<{foreach from=$sizes_data item=ad key=id}><input type='hidden' name='sizes_<{$id}>' value='<{if isset($data.sizes[$id])}><{$data.sizes[$id]}><{else}>0<{/if}>'><{/foreach}>
    <select id='item_sizes' name='item_sizes[]' multiple size=5>
<{foreach from=$sizes_data item=ad key=id}>
     <option value='<{$id}>' <{if isset($data.sizes[$id])}>selected<{/if}>><{$ad.name}></option>
<{/foreach}>
    </select>
    <div id='price_c'>
    <input type='hidden' id='price_editing' value=''>
    Стоимость: <input type='text' id='price' size='30'>&nbsp;<a href='' id='price_save'>Сохранить цену с этой модификацией</a><br />
    </div>
  </td>
 </tr>
 <tr class="seo">
  <td>SEO ключевики:</td>
  <td colspan='3'>
   <textarea name='item_kw' rows='7' cols='65' id="item_kw"><{$data.seo_kw}></textarea>
  </td>
 </tr>
 <tr class="seo">
  <td>SEO description:</td>
  <td colspan='3'>
   <textarea name='item_seo_descr' rows='7' cols='65' id="item_seo_descr"><{$data.seo_descr}></textarea>
  </td>
 </tr> 
</table>
<div id='item_result' style="padding:0 0 10px 10px;font-size:14px;"></div>
</form>