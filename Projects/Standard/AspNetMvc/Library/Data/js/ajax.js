fileUploadResult = null;
// Инициализируем таблицу перевода
var trans = [];
for (var i = 0x410; i <= 0x44F; i++) trans[i] = i - 0x350; // А-Яа-я
trans[0x401] = 0xA8;    // Ё
trans[0x451] = 0xB8;    // ё

function reloc_addr(addr) { document.location = addr; }

// Переопределяем функцию escape()
function my_escape(str)
{
    try {
        var ret = [];
        // Составляем массив кодов символов, попутно переводим кириллицу
        for (var i = 0; i < str.length; i++)
        {
            var n = str.charCodeAt(i);
            if (typeof trans[n] != 'undefined') n = trans[n];
            if (n <= 0xFF) ret.push(n);
        }
        return window.escape(String.fromCharCode.apply(null, ret));
    } catch(err) { alert(err); }
}

function serialize( mixed_value ) 
{
    try {
        var _getType = function( inp ) 
        {
            var type = typeof inp, match;
            if (type == 'object' && !inp) { return 'null'; }
            if (type == "object") 
            {
                if (!inp.constructor) { return 'object'; }
                var cons = inp.constructor.toString();
                if (match = cons.match(/(\w+)\(/)) { cons = match[1].toLowerCase(); }
                var types = ["boolean", "number", "string", "array"];
                for (key in types) 
                {
                    if (cons == types[key]) 
                    { 
                        type = types[key];
                        break;
                    }
                }
            }
            return type;
        };
        var type = _getType(mixed_value);
        var val, ktype = '';
        switch (type) {
            case "function":
                val = "";
            break;
                case "undefined":
                val = "N";
            break;
            case "boolean":
                val = "b:" + (mixed_value ? "1" : "0");
            break;
            case "number":
                val = (Math.round(mixed_value) == mixed_value ? "i" : "d") + ":" + mixed_value;
            break;
            case "string":
                val = "s:" + mixed_value.length + ":\"" + mixed_value + "\"";
            break;
            case "array":
            case "object":
                val = "a";
                var count = 0;
                var vals = "";
                var okey;
                for (key in mixed_value) 
                {
                    ktype = _getType(mixed_value[key]);
                    if (ktype == "function" || ktype == "object") { continue; }
                    if (mixed_value[key]== "reduce" || mixed_value[key] == "reduceRight") { continue; }
                    okey = (key.match(/^[0-9]+$/) ? parseInt(key) : key);
                    vals += serialize(okey) +
                    serialize(mixed_value[key]);
                    count++;
                }
                val += ":" + count + ":{" + vals + "}";
            break;
        }
        if (type != "object" && type != "array") val += ";";
        return val;
    } catch (err) { alert(err); }
}


var ajaxRequest = function ()
{
    this.userOnLoad = null;
    this.userOnErrLoad = null;

    this.GET  = new Array();
    this.POST = new Array();
    this.POSTARR = new Array();
    this.POST_N = false;

    this.text = null;
    this.xml  = null;

    this.errtext = null;

    this.mContainerName = null;

    this.TIMEOUT = null;
    
    this.HTTP = null;

    if(window.XMLHttpRequest) 
    {
        this.HTTP = new XMLHttpRequest();
    } else if(window.ActiveXObject)  { this.HTTP = new ActiveXObject("Msxml2.XMLHTTP"); }
};

//Две функции для добавления GET, POST параметров
ajaxRequest.prototype.nullData = function() 
{
    try {
        this.GET  = new Array();
        
        this.POST = new Array();
        this.POSTARR = new Array();
        this.POST_N = false;
    } catch (err) { alert(err); }
}
//Две функции для добавления GET, POST параметров
ajaxRequest.prototype.setGET = function(vname, value) 
{
    try {
        this.GET[vname] = value;
    } catch (err) { alert(err); }
}
ajaxRequest.prototype.setPOST = function(vname, value) 
{
    try {
        for ( var i=0; i < this.POST.length; i++ )
        {
            if ( this.POST[i]['name'] == vname )
            {
                this.POST[i]['value'] = value;
                return ;
            }
        }
        
        this.POST[this.POST.length] = {'name':vname,'value':value};
        this.POST_N = true;
    } catch (err) { alert(err); }
}
ajaxRequest.prototype.setPOSTARR = function(vname, value) 
{
    try {
        for ( var i=0; i < this.POSTARR.length; i++ )
        {
            if ( this.POSTARR[i]['name'] == vname )
            {
                this.POSTARR[i]['value'] = value;
                return ;
            }
        }
        
        this.POSTARR[this.POSTARR.length] = {'name':vname,'value':value};
        this.POST_N = true;
    } catch (err) { alert(err); }
}

//Функция для отправки запроса серверу, в параметре передается путь и имя файла
ajaxRequest.prototype.load_form = function(form_id,before_func,container,url_new)
{
    try {
        if ( typeof(container) == 'undefined' || container == null )
        {
            alert('Не указано имя элемента, который будет принимать данные.');
            return ;
        }
        this.mContainerName = container;
        if ( typeof($('#'+form_id).ajaxForm) == 'undefined' )
        {
            alert('Не загружен модуль для работы с формами!');
            return ;
        }
        var befor = function (){};
        var _this = this;
        if ( typeof(before_func) != 'undefined' && before_func != null ) befor = before_func;

        var options = { success: function(responseText, statusText){
            try {   
                _this.workLoadedText(responseText,_this.mContainerName);
                if(_this.userOnLoad !== null) {_this.userOnLoad();}
            } catch (err) { alert(err); alert(responseText); }
        }, beforeSubmit: befor };
        if ( typeof(url_new) != 'undefined' && url_new != null ) options['url'] = url_new;
        
        $('#'+form_id).ajaxForm(options);
    } catch (err) { alert(err); }
} 

//Функция для отправки запроса серверу, в параметре передается адрес, куда следует слать запрос, и имя контейнера, куда следует вставить данные.
ajaxRequest.prototype.load = function(file,container) 
{   
    try {
        if ( typeof(container) == 'undefined' || container == null )
        {
            alert('Не указано имя элемента, который будет принимать данные.');
            return ;
        }
        this.mContainerName = container;
        var v;
        var post;
        if ( typeof(this) != 'undefined' && this != null && typeof(this.HTTP) != 'undefined' && this.HTTP != null ) this.HTTP.abort(); //Закрываем предыдущие соеденение
        var url = file;
        if(this.GET.length !== 0) 
        {
            url += "?";
            for(v in this.GET) 
            {
                url += v + "=" + encodeURIComponent(this.GET[v]) + "&";
            }
        }
        try { if (url.indexOf('?') == -1) url = url + '?'; } catch (err) { }
        url += "&eryiyhkfgdksbv&"+Math.random();
        if(this.POST_N == false) 
        {
            this.HTTP.open("GET", url, true);
            post = null;
        } else {
            this.HTTP.open("POST", url, true);
            post = "eryiyhkfgdksbv&" + Math.random() + "&"; 
            for ( var i=0; i < this.POST.length; i++ )
            {
                post += this.POST[i]['name'] + "=" + encodeURIComponent(this.POST[i]['value']) + "&";
            }
            for ( var i=0; i < this.POSTARR.length; i++ )
            {
                post += this.POSTARR[i]['name'] + "=" + encodeURIComponent(serialize(this.POSTARR[i]['value'])) + "&";
            }

            this.HTTP.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            this.HTTP.setRequestHeader("Content-Length", post.length);
        }
        
        var _this = this;
        
        this.HTTP.onreadystatechange = function()
        {
            try {
                if(_this.HTTP.readyState == 4) 
                {
                    clearTimeout(_this.TIMEOUT);
                    _this.TIMEOUT = null;
                    if(_this.HTTP.status == 200) 
                    {
                        _this.text = _this.HTTP.responseText;
                        _this.xml = _this.HTTP.responseXML;
                        //Если событие onload отслеживается, то сгенерируем его
                        if(_this.onLoaded !== null) {_this.onLoaded();}
                        if(_this.userOnLoad !== null) {_this.userOnLoad();}
                    } else {
                        //Ошибка!!! Если событие onerror отслеживается, то сгенерируем его
                        _this.errtext = _this.HTTP.statusText;
                        if(_this.onerror !== null) {_this.onerror();}
                        if(_this.userOnErrLoad !== null) {_this.userOnErrLoad();}
                    }

                    _this.userOnLoad = null;
                    _this.userOnErrLoad = null;
                }
            } catch (err) { alert(err); }
        };

        this.HTTP.send(post);
        this.nullData();
        this.TIMEOUT = setTimeout(function(){_this.etimeout();}, 300000);
    } catch (err) { alert(err); }
}
//Создадим функцию для обработки таймаута
ajaxRequest.prototype.etimeout = function() 
{
    try {
        if ( typeof(this) != 'undefined' && this != null )
        {
            if ( typeof(this.HTTP) != 'undefined' && this.HTTP != null ) this.HTTP.abort(); //Закрываем предыдущие соеденение
            this.errtext = "Timeout";
            if(this.onerror !== null) { this.onerror(); }
        }
    } catch (err) { alert(err); }
}

ajaxRequest.prototype.onLoaded = function() 
{
    try {
        this.workLoadedText(this.text,this.mContainerName);
    } catch (err) { alert(err+'\r\n-----------\r\n'+this.text); }
}
ajaxRequest.prototype.workLoadedText = function(text,container)
{
    try {
        var element = null;
        if ( typeof(container) == 'string' ) element = document.getElementById(container);
        else if ( typeof(container) == 'object' && typeof(container.innerHTML) != 'undefined' ) element = container;
        
        if ( element != null )
        {
            var matchAllInline = new RegExp('<script[^>]*>([\\S\\s]*?)<\/script>', 'img');
            var matchAllSrc = new RegExp('<script src=[\'|"]([^\'"]*?)[\'|"][^>]*>([\\S\\s]*?)<\/script>', 'img');
            
            var stripScripts = function(text) 
            {
                while ( (matched = matchAllInline.exec(text)) != null ) 
                {
                    try {
                        text = text.replace(matched[1],'');
                    } catch(err) { alert('Java eval error ('+err.number+'): '+err.description); }
                }
            };
            
            var extractScripts = function(text) 
            {
                while ( (matched = matchAllSrc.exec(text)) != null ) $.getScript(matched[1]);
                while ( (matched = matchAllInline.exec(text)) != null ) 
                {
                    try {
                        if ( typeof(window.execScript) != 'undefined' ) window.execScript(matched[1],'javascript');
                        else window.eval.call(window,matched[1]);
                    } catch(err) { alert('Java eval error ('+err.number+'): '+err.description); }
                }
            };
            
            element.innerHTML = text;
            extractScripts(text);
        } else alert('Элемент с id='+container+' не был найден в теле документа.');
    } catch (err) { alert(err); }
}
ajaxRequest.prototype.onerror = function() 
{
    var text = 'Ошибка запроса данных! '+this.HTTP.status+'\r\n'+this.HTTP.responseText;
    if ( typeof(this.mContainerName) != 'undefined' || this.mContainerName != null )
    {
        this.workLoadedText(text,this.mContainerName);
    } else alert(text);
}

JsonResult = {
    OK : 0,
    NETWORKERROR : 1,
    FORMATERROR : 2,
    SCRIPTERROR : 3
};

