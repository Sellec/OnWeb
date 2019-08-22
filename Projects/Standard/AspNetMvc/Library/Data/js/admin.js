/*$(function(){       

    $("#items_results tr td:even").css({"text-align":"right","font-weight":"bold"});
    $("#items_results tr td:even").each(function(){
        if ( $(this).parent().hasClass("editable") ) $(this).css({"cursor":"pointer"});        
    });
    $("#items_results tr td:even").click(function(){
        if ( $(this).find("input").val() != "2" && !$(this).parent().hasClass("tcapt") ) {
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
    $(".tcapt td").css({"text-align":"left","padding":"2px 11px"});
    
      $("#items_results tr td:even").each(function (i) {
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
            $("#items_results tr td:even").next().find("input,textarea,select").removeAttr("disabled");
            $("#items_results tr td:even").parent().removeClass("off");
            
            $("#items_results tr td:even").find('input.disable_elem').val(-1);
            return false;
   });
   
   $(".all_turn_off").click(function(){
        $("#items_results tr td:even").each(function () {
            if ( $(this).find("input").val() != "2" & !$(this).parent().hasClass("tcapt") ){
                $(this).next().find("input,textarea,select").attr("disabled","disabled");
                $(this).parent().addClass("off");

                $(this).find('input.disable_elem').val(-2);
                $(this).find('input.disable_elem').removeAttr("disabled");
            }    
        });
        return false;
   }); 
    $(".gettop").click(function(){scroll(0,0);return false});
    
});
*/