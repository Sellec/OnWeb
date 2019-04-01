<script type='text/javascript'>
var v_customExtensionsID = -1;

/*
Основная функция, подключающая расширения. id обозначает текущий объект. id=false означает что объект не выбран.
*/
function f_customExtensionsActivate(id) 
{ 
    if ( id == false ) v_customExtensionsID = -1;
    else v_customExtensionsID = id;
}

$(document).ready(function(){
    var aj = new ajaxRequest();
    aj.load('/admin/madmin/@Module.UrlName/extensionsGetData','admextens_content');
});
</script>
<div id='admextens_content'></div>