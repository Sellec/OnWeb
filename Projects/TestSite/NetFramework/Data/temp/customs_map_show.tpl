<{extends 'baseCommon'}>

<{block 'body'}>
<script type='text/javascript'>
try {
var _LoadedPlaces = new Array();
var Datas = new Array();
<{foreach from=$data item=ad key=id}>
Datas[<{$id}>] = {"name":"<{$ad.name|java_string}>","fields":"<{$ad.Fields}>","description":"<{$ad.description|strip_tags|truncate:350|java_string}>","coord_x":<{$data_maps[$id].obj_coord_x}>,"coord_y":<{$data_maps[$id].obj_coord_y}>};
<{/foreach}>
var Tags = new Array();
<{foreach from=$data_tags item=ad key=id}>
Tags[<{$id}>] = {"name":"<{$ad.tag_name|java_string}>","style":"<{$ad.tag_style|java_string}>","image":"<{$ad.tag_icon|java_string}>"};
<{/foreach}>

function mapShowPlaces(tag_id,status)
{                   
    try {
    if ( typeof(_LoadedPlaces[tag_id]) == 'undefined' )
    {
        if ( status == true )
        {
            $('img#img_loading_'+tag_id).show();
            var aj = new ajaxRequest();
            aj.load('/@Module.UrlName/loadtagdata/'+tag_id, 'customsMapsDivError');
        }
    } else {
        if ( status == true ) v_customsMapsObject.addOverlay(_LoadedPlaces[tag_id]);
        else v_customsMapsObject.removeOverlay(_LoadedPlaces[tag_id]);        
    }
    } catch(err) {alert("mapShowPlaces: "+err);}
}

function mapShowPlacesLoadingComplete(tag_id,data)
{
    try{
    var style = '';
    if ( typeof(Tags[tag_id]['style']) != 'undefined' && Tags[tag_id]['style'].length > 0 ) style = Tags[tag_id]['style']; 
    else {
        var template = new YMaps.Template("<div>\
        <img alt=\"\" style=\"height:$[style.iconStyle.size.y];width:$[style.iconStyle.size.x];\" src=\"$[style.iconStyle.href]\"\/>\
        </div>");
        var s = new YMaps.Style();
        s.iconStyle = new YMaps.IconStyle(template);
        s.iconStyle.href = Tags[tag_id]['image'];
        YMaps.Styles.add("defined#top", s);
        style = 'defined#top';
    }
    var collection = new YMaps.GeoObjectCollection(style);
    for ( var i in data )
    {
        var placemark = new YMaps.Placemark(new YMaps.GeoPoint(data[i]['coord_x'],data[i]['coord_y']));
        var av_fields = '';
        if (typeof(data[i]['fields'][7]) != 'undefined') av_fields = av_fields + data[i]['fields'][7];
        if (typeof(data[i]['fields'][8]) != 'undefined') {
            if (av_fields.length > 0) av_fields = av_fields + "<br />";
            av_fields += data[i]['fields'][8];
        }
        if (typeof(data[i]['fields'][20]) != 'undefined'){
            if (av_fields.length > 0) av_fields = av_fields + "<br />";
            av_fields = av_fields + data[i]['fields'][20];
        } 
        
        if (typeof(data[i]['description']) != 'undefined' && data[i]['description'].length > 0) data[i]['description'] = "<p class='ytext'>" + data[i]['description'] + "</p>";
        else data[i]['description'] = data[i]['description'] + "<br />";
        placemark.setBalloonContent("<div><p class='name'>"+data[i]['name']+"</p>"+av_fields+"<br />"+data[i]['description']+"<a href='/company/"+i+"' title='' target='_blank'>подробнее →</a></div>"); //В содержимое балуна можно вставить название, адрес и телефон фирмы
        collection.add(placemark);
    }    
    
    $('img#img_loading_'+tag_id).hide();
    _LoadedPlaces[tag_id] = collection;
    v_customsMapsObject.addOverlay(_LoadedPlaces[tag_id]);
    } catch(err) {alert("mapShowPlacesLoadingComplete: "+err);}
}

function map_init()
{
    v_customsMapsObject.setCenter(new YMaps.GeoPoint(39.035506,55.378867), 14, YMaps.MapType.PMAP);
    
    var options = {
        hasBalloon: true,
        hideIcon: true,
        style: "default#arrowDownRightIcon"
    };
    
    for ( var i in Datas )
    {
        var placemark = new YMaps.Placemark(new YMaps.GeoPoint(Datas[i]['coord_x'],Datas[i]['coord_y']), options);
        placemark.setBalloonContent("<div><strong>"+Datas[i]['name']+"</strong><br />"+Datas[i]['description']+"</div>"); //В содержимое балуна можно вставить название, адрес и телефон фирмы
        v_customsMapsObject.addOverlay(placemark);
    }
}
} catch(err) {alert("0: "+err);}

$(function(){
    try {
        v_customsMapsInitialOptions['userFunction'] = map_init;
        f_customsMapsEnableYMap();
        
        $('input.tags_labels').change(function(){
            mapShowPlaces($(this).val(),$(this).attr('checked'));
        });
        $("#customsMapsDivTagContainer input").each(function(){
            $(this).attr("checked",false);
        })
        $("#customsMapsDivTagContainer a").click(function(e){
            e.preventDefault();
            var elem = $(this).prev("input");
            if (elem.attr("checked") == false){
                elem.attr("checked",true);
                mapShowPlaces(elem.val(),true);
                $(this).parent().stop().animate({backgroundPosition:"(0 0)"}, {duration:500});
            } else {                                    
                elem.attr("checked",false);
                mapShowPlaces(elem.val(),false);
                $(this).parent().stop().animate({backgroundPosition:"(250px 0)"}, {duration:500});
            }
        })
    } catch(err) {alert("0: "+err);}
});
</script>

 <div id="content_top"><img src="/data/img/content_tl.gif" alt="" /></div>
  <div id="content_left">
   <div id="content">
    <h1>Карта Егорьевска и Егорьевского района</h1>
    <ul id='customsMapsDivTagContainer'>
     <{foreach from=$data_tags item=ad key=id}>
     <li class='<{$id}>'>
     <!--<img src='/data/img/loading.gif' style='display:none;' id='img_loading_<{$id}>'>-->
     <img src='<{$ad.tag_icon}>' style='width:20px;'>
     <input type='checkbox' class='tags_labels' value='<{$id}>'>&nbsp;<a href="#" title=""><{$ad.tag_name}></a>
     </li>
     <{/foreach}>
    </ul>

    <div id='customsMapsDivContainer' class="index_map">
     <{include file="customs_map_inc.tpl"}>
     <div id="customsMapsDivError" style="display:none"></div>
     <div class="ymap">
      <div id="customsMapsDivYMap" class="yblock"></div>
     </div>
    </div>
    <p class="in_help">Уважаемые посетители! Вы можете управлять картой для более удобного просмотра: приближать и удалять кнопками (+) и (-), а также колесиком мыши.</p>
    <div class="wrapper"></div>
   </div>
  </div>
 <div id="content_bottom"><img src="/data/img/content_bl.gif" alt="" /></div>
<{/block}>