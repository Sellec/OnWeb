function fileQueued(file) {
    try {
    } catch (ex) {
        this.customSettings.mainUploadObject.debug(ex);
        this.debug(ex);
    }
}

function fileQueueError(file, errorCode, message) {
    try {
        if (errorCode === SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED) {
            alert("Вы попытались выбрать слишком много файлов.\n" + (message === 0 ? "Вы достигли предела загрузки файлов." : "Вы можете выбрать " + (message > 1 ? "не более " + message + " файлов." : "один файл.")));
            return;
        }

        switch (errorCode) {
        case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
            this.customSettings.mainUploadObject.setStatus("Файл слишком большой.");
            break;
        case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
            this.customSettings.mainUploadObject.setStatus("Нельзя загружать файлы с нулевым размером.");
            break;
        case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
            this.customSettings.mainUploadObject.setStatus("Неправильный тип файла.");
            break;
        default:
            if (file !== null) {
                this.customSettings.mainUploadObject.setStatus("Неизвестная ошибка");
            }
            break;
        }
    } catch (ex) {
        this.customSettings.mainUploadObject.debug(ex);
        this.debug(ex);
    }
}

function fileDialogComplete(numFilesSelected, numFilesQueued) {
    try {
        this.startUpload();
    } catch (ex)  {
        this.customSettings.mainUploadObject.debug(ex);
        this.debug(ex);
    }
}

function uploadStart(file) {
    try {
        this.customSettings.mainUploadObject.setStatus("Загрузка...");
    } catch (ex) {
        this.customSettings.mainUploadObject.debug(ex);
        this.debug(ex);        
    }
    
    return true;
}

function uploadProgress(file, bytesLoaded, bytesTotal) {
    try {
        var percent = Math.ceil((bytesLoaded / bytesTotal) * 100);
        this.customSettings.mainUploadObject.setStatus("Загрузка..."+percent+"%");
    } catch (ex) {
        this.customSettings.mainUploadObject.debug(ex);
        this.debug(ex);
    }
}

function uploadSuccess(file, serverData) {
    try {
        this.customSettings.mainUploadObject.setStatus("Загрузка завершена.");
        this.customSettings.mainUploadObject.success(file,serverData);        
    } catch (ex) {
        this.customSettings.mainUploadObject.debug(ex);
        this.debug(ex);
    }
}

function uploadError(file, errorCode, message) {
    try {
        switch (errorCode) {
        case SWFUpload.UPLOAD_ERROR.HTTP_ERROR:
            this.customSettings.mainUploadObject.setStatus("Ошибка загрузки: " + message);
            break;
        case SWFUpload.UPLOAD_ERROR.UPLOAD_FAILED:
            this.customSettings.mainUploadObject.setStatus("Загрузка оборвалась.");
            break;
        case SWFUpload.UPLOAD_ERROR.IO_ERROR:
            this.customSettings.mainUploadObject.setStatus("Ошибка ввода/вывода сервера");
            break;
        case SWFUpload.UPLOAD_ERROR.SECURITY_ERROR:
            this.customSettings.mainUploadObject.setStatus("Ошибка безопасности");
            break;
        case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
            this.customSettings.mainUploadObject.setStatus("Достигнут лимит загрузки.");
            break;
        case SWFUpload.UPLOAD_ERROR.FILE_VALIDATION_FAILED:
            this.customSettings.mainUploadObject.setStatus("Ошибка проверки. Загрузка отменена.");
            break;
        case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
            this.customSettings.mainUploadObject.setStatus("Закрыто");
            break;
        case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
            this.customSettings.mainUploadObject.setStatus("Остановлено");
            break;
        default:
            this.customSettings.mainUploadObject.setStatus("Неизвестная ошибка: " + errorCode);
            break;
        }
    } catch (ex) {
        this.customSettings.mainUploadObject.debug(ex);
        this.debug(ex);
    }
}

function uploadComplete(file) {
    if (this.getStats().files_queued === 0) {
    }
}

function queueComplete(numFilesUploaded) {
    this.customSettings.mainUploadObject.setStatus(numFilesUploaded + " файлов загружено.");
}

var uploadRequest = function (_settings_,container)
{
    if ( typeof(SWFUpload) == 'undefined' ) return false;
    
    var _settings = this.getDefSettings();
    for ( var i in _settings_ ) 
    {
        _settings[i] = _settings_[i];
    }
    
    var settings = {};
    settings.debug = false;
    
    settings.flash_url = "/data/swfupload/swfupload.swf";
    settings.upload_url = _settings.upload_url;
    settings.post_params = _settings.post_params;
    settings.file_size_limit = _settings.file_size_limit;
    settings.file_types = _settings.file_types;
    settings.file_types_description = _settings.file_types_description;
    settings.file_upload_limit = _settings.file_upload_limit;
    settings.file_queue_limit = _settings.file_queue_limit;
    settings.file_post_name = _settings.file_post_name;
    
    settings.custom_settings  = {
        mainUploadObject : this
    };

    settings.button_image_url = _settings.button_image_url;
    settings.button_width = _settings.button_width;
    settings.button_height = _settings.button_height;
    settings.button_placeholder_id = _settings.button_placeholder_id;
    settings.button_text = _settings.button_text;
    settings.button_text_style = _settings.button_text_style;
    settings.button_text_left_padding = _settings.button_text_left_padding;
    settings.button_text_top_padding = _settings.button_text_top_padding;
        
    // The event handler functions are defined in handlers.js
    settings.file_queued_handler = _settings.file_queued_handler;
    settings.file_queue_error_handler = _settings.file_queue_error_handler;
    settings.file_dialog_complete_handler = _settings.file_dialog_complete_handler;
    settings.upload_start_handler = _settings.upload_start_handler;
    settings.upload_progress_handler = _settings.upload_progress_handler;
    settings.upload_error_handler = _settings.upload_error_handler;
    settings.upload_success_handler = _settings.upload_success_handler;
    settings.upload_complete_handler = _settings.upload_complete_handler;
    settings.queue_complete_handler = _settings.queue_complete_handler;
    
    this.swfUploader = new SWFUpload(settings);
    this.statusElement = _settings.statusElement;
    this.successFunc = _settings.successFunc;
    this.container = container;
};

//Функция выдает базовый набор настроек, которые заменяются переданными
uploadRequest.prototype.getDefSettings = function()
{
    return {
        file_size_limit : "10 MB",
        file_types : "*.*",
        file_types_description : "All Files",
        file_upload_limit : 1,
        file_queue_limit : 1000,
        file_post_name : 'upload',

        button_image_url: "/data/swfupload/buttons/TestImageNoText_65x29.png",
        button_width: "65",
        button_height: "29",
        button_placeholder_id: "spanButtonPlaceHolder",
        button_text: 'Загрузить',
        button_text_style: ".theFont { font-size: 16; }",
        button_text_left_padding: 3,
        button_text_top_padding: 3,
        
        statusElement: $('#divStatus'),
        
        successFunc : function(filename){},

        file_queued_handler : fileQueued,
        file_queue_error_handler : fileQueueError,
        file_dialog_complete_handler : fileDialogComplete,
        upload_start_handler : uploadStart,
        upload_progress_handler : uploadProgress,
        upload_error_handler : uploadError,
        upload_success_handler : uploadSuccess,
        upload_complete_handler : uploadComplete,
        queue_complete_handler : queueComplete,
    };
}

//начать загрузку!
uploadRequest.prototype.startUpload = function() 
{
    try {
        this.swfUploader.startUpload();
    } catch (err) { alert(err); }
}
//функция для записи текста в нужный элемент
uploadRequest.prototype.setStatus = function(text) 
{
    try {
        $(this.statusElement).text(text);
    } catch (err) { alert(err); }
}
//добавляем post параметр в массив отправки
uploadRequest.prototype.setPostParameter = function(name,value) 
{
    try {
        this.swfUploader.setPostParams({name:value});
        this.swfUploader.settings.post_params[name] = value;
    } catch (err) { alert(err); }
}
//Отладка ошибок
uploadRequest.prototype.debug = function(error) 
{
    try {
        alert(error);
    } catch (err) { alert(err); }
}
//Успешный результат загрузки
uploadRequest.prototype.success = function(file,serverData) 
{
    try {
        if ( typeof(this.successFunc) != 'undefined' ) this.successFunc(serverData);
        if ( typeof(this.container) != 'undefined' ) this.workLoadedText(serverData,this.container);
    } catch (err) { alert(err); }
}

uploadRequest.prototype.workLoadedText = function(text,container)
{
    try {
        var element = document.getElementById(container);
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
                        /*window.eval(matched[1]);*/
                        window.eval.call(window,matched[1]);/*window.eval(matched[1]);*/
                    } catch(err) { alert('Java eval error ('+err.number+'): '+err.description); }
                }
            };
            
            element.innerHTML = text;
            extractScripts(text);
        } else alert('Элемент с id='+container+' не был найден в теле документа.');
    } catch (err) { alert(err); }
}

