<div id='customsMapsDivContainer'>
<{include file="customs_map_inc.tpl"}>
<script type='text/javascript'>
<{if $data !== false}>
var v_customsMapsData = {"c_x":<{$data.obj_coord_x}>,"c_y":<{$data.obj_coord_y}>};
<{/if}>
$(document).ready(function(){
try {
    f_customsMapsEnableYMap();
} catch(err) {alert("0: "+err);}
});
</script>

<div id="customsMapsDivError" style="display:none"></div>
<div id="customsMapsDivYMap" class="firm_map"></div>

</div>
