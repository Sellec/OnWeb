function cuf_dis_en_tr(id, status)
{
    var td = $('input[name=field_'+id+']').parent();

    if ($(td).find("input").val() != "2" & !$(td).parent().hasClass("tcapt")) 
    {
        if (status == 1 && $(td).parent().hasClass("off")) 
        {
            $(td).next().find("input,textarea,select").removeAttr("disabled");
            $(td).parent().removeClass("off");

            $(td).find('input.disable_elem').val(-1);
        } 
        else if (status == 0)
        {
            $(td).next().find("input,textarea,select").attr("disabled","disabled");
            $(td).parent().addClass("off");

            $(td).find('input.disable_elem').val(-2).removeAttr("disabled");
        }        
    }
    
}

var mCUFTableID = null;
function cuf_set_table(table_id)
{
    mCUFTableID = table_id;
    
    $("#"+table_id+" tr td:even").css({"text-align":"right","font-weight":"bold"});
    $("#"+table_id+" tr.editable td:even").each(function(){
        if ( $(this).find("input").val() != "2" & !$(this).parent().hasClass("tcapt") ) $(this).css({"cursor":"pointer"});        
    });
    $("#"+table_id+" tr.editable td:even").click(function(){
        if ( $(this).find("input").val() != "2" & !$(this).parent().hasClass("tcapt") ) {
            if ( $(this).parent().hasClass("off") ) {
                $(this).next().find("input,textarea,select").removeAttr("disabled");
                $(this).parent().removeClass("off");

                $(this).find('input.disable_elem').val(-1);
            } else {
                $(this).next().find("input,textarea,select").attr("disabled","disabled");
                $(this).parent().addClass("off");

                $(this).find('input.disable_elem').val(-2);
                $(this).find('input.disable_elem').removeAttr("disabled");
            }        
        } else { $(this).next().find("input,textarea,select").removeAttr("disabled");
            $(this).parent().removeClass("off");
            
            $(this).find('input.disable_elem').val(-1);
        }

    });
    
    $("#"+table_id+" tr.editable td:even").each(function (i) {
        if ( $(this).find("input").val() == "0") {
            $(this).next().find("input,textarea,select").attr("disabled","disabled");
            $(this).parent().addClass("off");
            
            $(this).find('input.disable_elem').val(-2);
            $(this).find('input.disable_elem').removeAttr("disabled");
        } else if ( $(this).find("input").val() == "1" ){
            $(this).next().find("input,textarea,select").removeAttr("disabled");
            $(this).parent().removeClass("off");
            
            $(this).find('input.disable_elem').val(-1);
        }
    });
   
    $(".all_turn_on").click(function(){
        $("#"+mCUFTableID+" tr.editable td:even").next().find("input,textarea,select").removeAttr("disabled");
        $("#"+mCUFTableID+" tr.editable td:even").parent().removeClass("off");

        $("#"+mCUFTableID+" tr.editable td:even").find('input.disable_elem').val(-1);
        return false;
    });
   
    $(".all_turn_off").click(function(){
        $("#"+mCUFTableID+" tr.editable td:even").each(function () {
            if ( $(this).find("input").val() != "2" & !$(this).parent().hasClass("tcapt") ){
                $(this).next().find("input,textarea,select").attr("disabled","disabled");
                $(this).parent().addClass("off");

                $(this).find('input.disable_elem').val(-2);
                $(this).find('input.disable_elem').removeAttr("disabled");
            }    
        });
        return false;
    }); 
   
}

function cuf_init(table_id,mode,data)
{
    mCUFTableID = table_id;
    
    var selector = '';
    if ( typeof(mode) == 'undefined' || mode == 'table' ) selector = 'tr.editable td:even';
    else if ( mode == 'li' ) selector = 'li.editable label:even';
    
    $("#"+table_id+" "+selector).each(function(){
        if ( $(this).find("input").val() != "2" & !$(this).parent().hasClass("tcapt") ) $(this).css({"cursor":"pointer"});        
    });
    $("#" + table_id + " " + selector).click(function () {
        if ($(this).find("input").val() != "2" & !$(this).parent().hasClass("tcapt")) {
            if ($(this).parent().hasClass("off")) {
                $(this).next().find("input,textarea,select").removeAttr("disabled");
                $(this).parent().removeClass("off");

                $(this).find('input.disable_elem').val(-1);
            } else {
                $(this).next().find("input,textarea,select").attr("disabled", "disabled");
                $(this).parent().addClass("off");

                $(this).find('input.disable_elem').val(-2);
                $(this).find('input.disable_elem').removeAttr("disabled");
            }
        } else {
            $(this).next().find("input,textarea,select").removeAttr("disabled");
            $(this).parent().removeClass("off");

            $(this).find('input.disable_elem').val(-1);
        }
    });
    
    $("#"+table_id+" "+selector).each(function (i) {
        if ( $(this).find("input").val() == "0") 
        {
            $(this).next().find("input,textarea,select").attr("disabled","disabled");
            $(this).parent().addClass("off");
            
            $(this).find('input.disable_elem').val(-2);
            $(this).find('input.disable_elem').removeAttr("disabled");
        } 
        else if ( $(this).find("input").val() == "1" )
        {
            $(this).next().find("input,textarea,select").removeAttr("disabled");
            $(this).parent().removeClass("off");
            
            $(this).find('input.disable_elem').val(-1);
        }
    });
   
    $(".all_turn_on").click(function(){
        $("#"+mCUFTableID+" "+selector).next().find("input,textarea,select").removeAttr("disabled");
        $("#"+mCUFTableID+" "+selector).parent().removeClass("off");

        $("#"+mCUFTableID+" "+selector).find('input.disable_elem').val(-1);
        return false;
    });
   
    $(".all_turn_off").click(function(){
        $("#"+mCUFTableID+" "+selector).each(function (){
            if ( $(this).find("input").val() != "2" & !$(this).parent().hasClass("tcapt") )
            {
                $(this).next().find("input,textarea,select").attr("disabled","disabled");
                $(this).parent().addClass("off");

                $(this).find('input.disable_elem').val(-2).removeAttr("disabled");
            }    
        });
        return false;
    });
    
    if ( typeof(data) != 'undefined' )
        for (var i in data) 
            cuf_dis_en_tr(i, 1);
   
}

