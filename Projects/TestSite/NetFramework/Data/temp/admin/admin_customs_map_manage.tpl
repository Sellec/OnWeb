<div id='customsMapsDivContainer'>
<{include file="admin/admin_customs_map_inc.tpl"}>
<script type='text/javascript'>
<{if $data !== false}>
var v_customsMapsData = {"c_x":<{$data.obj_coord_x}>,"c_y":<{$data.obj_coord_y}>};
<{/if}>
$(document).ready(function(){
try {
    f_customsMapsEnableYMap();
    
    $('#customsMapsDivButton_autodetectplace').click(function(){
        try {
            if ( v_customsMapsObject != null )
            {
                if (YMaps.location) 
                {
                    center = new YMaps.GeoPoint(YMaps.location.longitude, YMaps.location.latitude);
                    v_customsMapsObject.setCenter(center, 16, YMaps.MapType.PMAP);
                } else $(this).attr('disabled','true');
            }
            
        } catch(err) {}
    });
    $('#customsMapsDivButton_setobjectcenter').click(function(){
        try {
            if ( v_customsMapsObject != null )
            {
                center = new YMaps.GeoPoint(v_customsMapsData['c_x'],v_customsMapsData['c_y']);
                v_customsMapsObject.setCenter(center);//, 14, YMaps.MapType.PMAP);
                v_customsMapsObject.setZoom(17);
            }
            
        } catch(err) {}
    });
    <{if $data === false}>
    $('#customsMapsDivButton_setobjectcenter').attr('disabled','true');
    $('#customsMapsDivButton_saveselectedplace').attr('disabled','true');
    <{else}>
    $('#customsMapsDivButton_setobjectcenter').removeAttr('disabled');
    $('#customsMapsDivButton_saveselectedplace').removeAttr('disabled');
    
    $('select[name=tags] option[value=<{$data.obj_tag}>]').attr("selected", true);
    <{/if}>
    $('#customsMapsDivButton_selectplacing').click(function(){
        try {
            if ( v_customsMapsObject != null )
            {
                $('#customsMapsDivYMap').toggleClass('elem_pointer');
                YMaps.Events.observe(v_customsMapsObject, v_customsMapsObject.Events.Click, function (map, mEvent) {
                    try {
                        v_customsMapsObject.removeOverlay(placemark);
                    var added = $('input[name="added"]').val();
                    if (added == 0){
                        _coords = mEvent.getCoordPoint();
                        $('input[name=coord_maps_x]').val(_coords.getX());
                        $('input[name=coord_maps_y]').val(_coords.getY());
                    
                        v_customsMapsObject.setCenter(_coords);
                    
                        var options = {
                            hasBalloon: false,
                            draggable: true
                        }
                        var placemark = new YMaps.Placemark(new YMaps.GeoPoint(_coords.getX(),_coords.getY()),options);
                        v_customsMapsObject.addOverlay(placemark); 
                        added = 1;
                        $('input[name="added"]').val(added)
                        
                        YMaps.Events.observe(placemark, placemark.Events.DragEnd, function (obj) {
                            var placenow = obj.getCoordPoint();
                            $('input[name=coord_maps_x]').val(placenow.getX());
                            $('input[name=coord_maps_y]').val(placenow.getY());
                            
                            v_customsMapsObject.setCenter(placenow);
                            v_customsMapsData = {"c_x":placenow.getX(),"c_y":placenow.getY()};
                        });
                        
                        v_customsMapsData = {"c_x":_coords.getX(),"c_y":_coords.getY()};
                    }
                    $('#customsMapsDivYMap').removeClass('elem_pointer');
                    $('#customsMapsDivButton_setobjectcenter').removeAttr('disabled');
                    $('#customsMapsDivButton_saveselectedplace').removeAttr('disabled');
                    }catch(err){alert(err);}
                });                
            }
            
        } catch(err) {}
    });
    $('#customsMapsDivButton_saveselectedplace').click(function(){
        try {
            if ( v_customsMapsObject != null )
            {
                if ( typeof(v_customsMapsData) == 'undefined' )
                {
                    $('#customsMapsDivButton_setobjectcenter').attr('disabled','true');
                    $('#customsMapsDivButton_saveselectedplace').attr('disabled','true');
                } else {
                    var aj = new ajaxRequest();
                    aj.load('/admin/madmin/@Module.UrlName/map_manage_save/<{$data_id}>/'+$('select[name=tags] option:selected').val()+'/'+v_customsMapsData['c_x']+'/'+v_customsMapsData['c_y'],'customsMapsDivError');
                    $('#customsMapsDivError').show();
                }
            }
            
        } catch(err) {}
    });
    $("#customsMapsDivButton_findplace").click(function(){
        try {
            //v_customsMapsObject.removeOverlay(geoResult);
            var geocoder = new YMaps.Geocoder("Егорьевск, " + $("input[name='field_value_8[]']").val(), {results: 1, boundedBy: v_customsMapsObject.getBounds(), hasBalloon: false});
            YMaps.Events.observe(geocoder, geocoder.Events.Load, function (){
                if (this.length()) {
                    geoResult = this.get(0);
                    //v_customsMapsObject.setBounds(geoResult.getBounds());
                    var newplace = geoResult.getGeoPoint();
                    v_customsMapsObject.panTo(newplace)
                    v_customsMapsObject.zoomBy(14)
                    
                    /*if ($('input[name="added"]').val() == 0) {
                        var options = {
                            draggable: false,
                            hasBalloon: false,
                            style: "default#arrowDownRightIcon"
                        };
                    
                        var geo = new YMaps.Placemark(new YMaps.GeoPoint(newplace.getX(),newplace.getY()), options);
                        v_customsMapsObject.addOverlay(geo)
                        $("#customsMapsDivYMap *").click(function(){
                            v_customsMapsObject.removeOverlay(geo);
                        })
                    }*/
                }else {
                    alert("Ничего не найдено")
                }
            });

            YMaps.Events.observe(geocoder, geocoder.Events.Fault, function (geocoder, error) {
                alert("Произошла ошибка: " + error);
            })
    
        } catch(err) {}
    })
} catch(err) {alert("0: "+err);}
});
</script>

<input type="hidden" name="added" value="0">
<input type="hidden" name="coord_maps_x" value=""><input type="hidden" name="coord_maps_y" value="">
<p>
 <button id='customsMapsDivButton_autodetectplace' style='display:none;'>Определить мое местоположение</button>&nbsp;
 <button id='customsMapsDivButton_selectplacing'>Выбрать место на карте</button>&nbsp;
 <button id='customsMapsDivButton_setobjectcenter'>Показать выбранную точку</button>&nbsp;
 <button id='customsMapsDivButton_saveselectedplace'>Сохранить выбранное местоположение</button>&nbsp;
 <button id='customsMapsDivButton_findplace'>Найти</button>&nbsp;
 <br />Тэг:&nbsp;<select name='tags'>
 <{foreach from=$data_tags item=ad key=id}>
  <option value='<{$id}>'><{$ad}></option>
 <{/foreach}>
 </select><br>
</p>
<div id="customsMapsDivError" style="display:none"></div>
<div id="customsMapsDivYMap" style="display:none;height:400px; width:600px;"></div>

</div>