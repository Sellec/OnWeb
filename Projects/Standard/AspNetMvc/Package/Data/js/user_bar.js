function updateUserPanel(new_status, adm)
{
    var authorized = 'undefined' != typeof(new_status) && (new_status == "1" || new_status == 1);
    
    if ('undefined' != typeof(new_status) && (new_status == "1" || new_status == 1))
    {
        $(".unauthorized").hide();
        $(".authorized").show();

        if (typeof(adm) != 'undefined' && adm == 1 ) {$("#admin_link").parent().show();}
        else {$("#admin_link").parent().hide();}
    } 
    else
    {
        $(".authorized").hide();
        $(".unauthorized").show();
    }    
}

function log_end(res,text,login,acc,adm)
{    
    try 
    {
        $("span.ress").hide();
        $(this).oneTime("1s", function(){
            if ( res == 4 )
            {
                $("span.ress").css({'color':'black'});
                $("form").fadeOut("slow");
                $("p.form_caption").fadeOut("slow");
                $("a.forget").fadeOut();
                $(".authorized").fadeIn("slow");
                $(".unauthorized").fadeOut("slow");
                updateUserPanel(1,adm);
                $("#user_panel li:first a").text(login);
                $("#user_panel li:first a").attr("href",$("#user_panel li:first a").attr("href") + login);
            } 
            else if ( res != 4 && res != 5 )
            {
                $(".unauthorized").show();
                $(".authorized").hide();
                $("span.ress").css({'color':'red'});
                
                updateUserPanel(2);
            } 
            else if ( res == 5 )
            {
                $(".authorized").fadeOut("slow");
                $(".unauthorized").fadeIn("slow");
                $(".unauthorized").show();
                $(".authorized").hide();
                updateUserPanel(2);
            }
            
            if (res != 5) $(this).oneTime(500, function(){$("span.ress").fadeIn("slow");$("span.ress").text(text);});
        }, 1);
    } 
    catch (err) { alert(err); }
}
   
function log_call(login, pass)
{
    try 
    {
        $("span.ress").show();
        $("span.ress").text("Подождите, идет проверка данных...");
        
        var aj = new ajaxRequest();
        aj.setPOST("login",login.value);
        aj.setPOST("pass",pass.value);
        aj.load("/login/inajax",'result_login');
    } 
    catch (err) { alert(err); }
}

function log_exit()
{
    try {
        var aj = new ajaxRequest();
        aj.load("/login/outajax",'result_login');
    } catch (err) { alert(err); }
}
