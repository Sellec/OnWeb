<style type="text/css">
#customsMapsDivError {
    font-size: 0.8em;

    text-align: center;

    width: 590px;
    padding: 5px;
    margin: 1em 0;

    border:solid 1px red;
    background: #fff1f1;
}
</style>
<script type='text/javascript'>
function f_customsMapsMess(err)
{
    $('#customsMapsDivError').show().text(err);
    setTimeout("$('#customsMapsDivError').hide();",3000);
}
</script>
<script src="http://api-maps.yandex.ru/1.1/index.xml?onerror=f_customsMapsMess&loadByRequire=1&modules=pmap&key=AG5XD0wBAAAAQXDhPAIATfNf4ymQfbK4U6JOM6wjsWOa13YAAAAAAAAAAADdt0U2jhermuGDU0AGAn6jRgwZzQ==" type="text/javascript"></script>
<script type='text/javascript'>
var v_customsMapsObject = null;
var v_customsMapsInitialOptions = {};
function f_customsMapsInit()
{
    try {
    YMaps.load("pmap");        
    
    $('#customsMapsDivYMap').show();
    
    v_customsMapsObject = new YMaps.Map(YMaps.jQuery("#customsMapsDivYMap")[0]);
    v_customsMapsObject.addControl(new YMaps.SmallZoom());

    v_customsMapsObject.enableHotKeys();
    v_customsMapsObject.enableScrollZoom();
    
    f_customsMapsEnablePYMap();

    } catch(err) {alert("4: "+err);}
}
function f_customsMapsUpdate()
{
    try {
    v_customsMapsObject.addControl(new YMaps.TypeControl([YMaps.MapType.MAP, YMaps.MapType.HYBRID]));
    
    if ( typeof(v_customsMapsInitialOptions) != 'undefined' && typeof(v_customsMapsInitialOptions['geocode']) != 'undefined' && v_customsMapsInitialOptions['geocode'] == 1 )
    v_customsMapsObject.addControl(new YMaps.SearchControl({geocodeOptions: {geocodeProvider: "yandex#pmap"}}));
    
    var center = new YMaps.GeoPoint(39.039379,55.380721);
    v_customsMapsObject.setCenter(center, 15, YMaps.MapType.PMAP);

    if ( typeof(v_customsMapsData) != 'undefined' )
    {
        var options = {
            //draggable: true,
            hasBalloon: true,
            hideIcon: true,
            style: "default#arrowDownRightIcon"
        };
        var center = new YMaps.GeoPoint(v_customsMapsData['c_x'],v_customsMapsData['c_y']);
        v_customsMapsObject.setCenter(center);
        v_customsMapsObject.setZoom(15);
        
        var placemark = new YMaps.Placemark(new YMaps.GeoPoint(v_customsMapsData['c_x'],v_customsMapsData['c_y']), options);
        placemark.setBalloonContent("<div><strong>"+$("h1").text()+"</strong><br /></div>");
        v_customsMapsObject.addOverlay(placemark);
    }
        
    if ( typeof(v_customsMapsInitialOptions) != 'undefined' && typeof(v_customsMapsInitialOptions['userFunction']) != 'undefined' ) 
    v_customsMapsInitialOptions['userFunction']();
    } catch(err) {alert("f_customsMapsUpdate: "+err);}
}

function f_customsMapsSetCenter(x,y)
{
    try{
    if ( typeof(v_customsMapsObject) != 'undefined' )
    {
        var center = new YMaps.GeoPoint(x,y);
        v_customsMapsObject.setCenter(center);
    } 
    } catch(err) {alert("f_customsMapsSetCenter: "+err);}  
}

function f_customsMapsEnableYMap()
{
    try {
        if ( typeof(YMaps) == 'undefined' ) setTimeout(f_customsMapsEnableYMap,100);
        else { 
            YMaps.load("pmap");
            YMaps.load(f_customsMapsInit);
        }
    } catch(err) {alert("2: "+err);}
}
function f_customsMapsEnablePYMap()
{
    try {
        if ( typeof(YMaps.MapType.HYBRID) == 'undefined' ) setTimeout(f_customsMapsEnablePYMap,100);
        else YMaps.load(f_customsMapsUpdate());
    } catch(err) {}
}
</script>
