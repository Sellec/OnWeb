//requestJSON
JsonResult = {
    OK: 0,
    FORMATERROR: 1,
    NETWORKERROR: 2,
    SCRIPTERROR: 3,
    JSERROR: 4,
};

function nl2br(text)
{
    if (text != undefined && text != null) return text.replace(/([^>])\n/g, '$1<br/>');
    return text;
}

/**
 * AJAX-методы.
 * */
$(function ()
{
    if ($.fn.uploadFile == undefined) $.getScript("/data/libs/jquery.uploadfile/jquery.uploadfile.js");

    var requestMethods = {
        prepareAnswer: function (answer, callback)
        {
            var success = JsonResult.OK;
            var message = "";
            var data = null;
            var modelState = null;

            try
            {
                if (!$.isPlainObject(answer)) 
                {
                    if ($.type(answer) != "string") throw "Параметр 'answer' должен быть строкой или объектом.";
                    else if (answer.length == 0) answer = { result: "", success: true };
                    else
                    {
                        try { answer = JSON.parse(answer); }
                        catch (err2) { answer = { result: answer, success: false }; }
                    }
                }

                if (answer == null || answer['success'] == undefined) throw "Неправильный формат ответа. Отсутствует параметр 'success'";
                else if (answer['result'] == undefined) throw "Неправильный формат ответа. Отсутствует параметр 'result'";
                else
                {
                    if (answer['success'] == true) success = JsonResult.OK; else success = JsonResult.SCRIPTERROR;
                    message = answer['result'];
                }

                if (answer != null && answer['data'] != undefined) data = answer['data'];

                $("*.field-validation-valid, *.field-validation-error").hide();
                if (answer != null && answer['modelState'] != undefined)
                {
                    if ($.isArray(answer['modelState']) || $.isPlainObject(answer['modelState']))
                    {
                        var scrolled = false;
                        $.each(answer['modelState'], function (index, value) {
                            if (index == "") index = "__entire_model__";

                            var validationMessageContainer = $("[data-valmsg-for='" + index + "'");
                            if (validationMessageContainer.length > 0) {
                                validationMessageContainer.removeClass('field-validation-valid field-validation-error').addClass('field-validation-error');
                                var validationMessage = value.join("\r\n").replace(/([^>])\n/g, '$1<br/>');
                                validationMessageContainer.html(validationMessage);
                                validationMessageContainer.show();

                                if (!scrolled) {
                                    jQuery("html:not(:animated),body:not(:animated)").animate({ scrollTop: validationMessageContainer.offset().top - 20 }, 800);
                                    scrolled = true;
                                }
                            }
                            else validationMessageContainer.removeClass('field-validation-valid field-validation-error').addClass('field-validation-valid');

                            var validationMessageContainer = $("span#" + index + "_validationMessage");
                            if (validationMessageContainer.length > 0) {
                                validationMessageContainer.removeClass('field-validation-valid field-validation-error').addClass('field-validation-error');
                                var validationMessage = value.join("\r\n").replace(/([^>])\n/g, '$1<br/>');
                                validationMessageContainer.html(validationMessage);
                                validationMessageContainer.show();

                                if (!scrolled) {
                                    jQuery("html:not(:animated),body:not(:animated)").animate({ scrollTop: validationMessageContainer.offset().top - 20 }, 800);
                                    scrolled = true;
                                }
                            }
                            else validationMessageContainer.removeClass('field-validation-valid field-validation-error').addClass('field-validation-valid');
                        });
                    }
                }
            }
            catch (err)
            {
                success = JsonResult.FORMATERROR;
                message = err;
            }

            if (message == undefined || message == null) message = "";
            if (success != JsonResult.OK)
                console.log("requestJSON result", success, message);

            //message = message.replace(/([^>])\n/g, '$1<br/>');

            return { Success: success, Message: message, Data: data };
        },

        func: function (answer, callback)
        {
            var result = requestMethods.prepareAnswer(answer);
            $.proxy(callback, this)(result.Success, result.Message, result.Data);
        },

        funcResponse: function (response, callback)
        {
            var message = "";
            if (response.ResponseText != undefined) message = response.ResponseText;
            if (response.responseText != undefined) message = response.responseText;

            if (message == undefined || message == null) message = "";

            var result = requestMethods.prepareAnswer(message);
            if (response.status != undefined && response.status == 0) {
                result.Success = JsonResult.NETWORKERROR;
                if (!result.Message) result.Message = "Возникла ошибка сети во время выполнения запроса.";
            }
            $.proxy(callback, this)(result.Success, result.Message, result.Data);
        },

        prepareUrl: function (url)
        {
            var param = "jsonrequestqueryf1F8Dz0";
            if (url.indexOf("?") >= 0) url += "&" + param;
            else url += "?" + param;

            return url;
        }
    };

    $.requestDirect = function (url, postData, callback)
    {
        try
        {
            // The rest of this code assumes you are not using a library.
            // It can be made less wordy if you use one.
            var form = document.createElement("form");
            form.setAttribute("method", "post");
            form.setAttribute("action", url != undefined && url != null && url.length > 0 ? url : document.URL);

            for (var key in postData)
            {
                if (postData.hasOwnProperty(key))
                {
                    var hiddenField = document.createElement("input");
                    hiddenField.setAttribute("type", "hidden");
                    hiddenField.setAttribute("name", key);
                    hiddenField.setAttribute("value", postData[key]);

                    form.appendChild(hiddenField);
                }
            }

            document.body.appendChild(form);
            form.submit();
        }
        catch (err) { callback(JsonResult.NETWORKERROR, err, null); }
    };

    $.requestJSON = function (url, postData, callback)
    {
        try
        {
            var getType = {};
            if (!callback || getType.toString.call(callback) != '[object Function]') throw new Error("callback должен быть функцией.");

            var func = function (answer)
            {
                $.proxy(requestMethods.func, this)(answer, callback);
            };

            var funcError = function (response)
            {
                $.proxy(requestMethods.funcResponse, this)(response, callback);
            }

            url = requestMethods.prepareUrl(url);

            if (postData == null || postData == undefined) $.getJSON(url, func).error(funcError);
            else
            {
                var _params = {
                    type: "POST",
                    url: url,
                    data: postData,
                    success: func,
                    dataType: "json",
                };
                if (postData instanceof FormData)
                {
                    _params['processData'] = false;
                    _params['contentType'] = false;
                }
                else
                {
                    _params['data'] = JSON.stringify(postData);
                    _params['contentType'] = "application/json; charset=utf-8";
                }

                $.ajax(_params).error(funcError);
                //$.post(url, postData, func, "json").error(funcError);
            }
        }
        catch (err) { callback(JsonResult.NETWORKERROR, err, null); }
    };

    $.fn.requestJSON = function (settings, settingsValue)
    {
        var defaults = {
            after: function () { },
            before: function () { },
        };

        if (this.length == 0) return;
        else if (this.length > 1) throw "requestJSON можно вызывать только для одного элемента.";

        if ($(this).is("form"))
        {
            var configSaved = $(this).data("requestJSON");
            var isSet = configSaved != null;

            if (settings == "destroy")
            {
                $(this).data("requestJSON", null);
                //if (isSet) $(this).unbind();
            }
            else
            {
                if (isSet)
                {
                    if (settings == "after")
                    {
                        var valueOld = configSaved.after;
                        if (settingsValue != undefined)
                        {
                            configSaved.after = settingsValue;
                            $(this).data("requestJSON", configSaved);
                        }
                        return valueOld;
                    }
                    else if (settings == "before")
                    {
                        var valueOld = configSaved.before;
                        if (settingsValue != undefined)
                        {
                            configSaved.before = settingsValue;
                            $(this).data("requestJSON", configSaved);
                        }
                        return valueOld;
                    }
                }
                else if (!isSet)
                {
                    var setts = {};
                    if ($.isFunction(settings)) setts = { callback: settings };
                    else setts = settings;

                    var configRJ = $.extend(this.config, defaults, setts);

                    $(this).submit(function (e)
                    {
                        e.preventDefault();

                        var thisObject = $(this),
                            isSubmittingRJ = $(this).data("requestJSON_submitting"),
                            config = $(this).data("requestJSON"),
                            elementsDisabled = new Array();

                        if (isSubmittingRJ == true) return;

                        thisObject.data("requestJSON_submitting", true);

                        try
                        {

                            var dataToSend = $(this).serializeArray();
                            try
                            {
                                var data2 = new FormData($(this)[0]);
                                dataToSend = data2;
                            }
                            catch (err) { }

                            if ($.proxy(config.before, this)(dataToSend) !== false)
                            {
                                $("input, button, textarea, select", this).each(function ()
                                {
                                    if (!$(this).is(":disabled"))
                                    {
                                        elementsDisabled[elementsDisabled.length] = $(this);
                                        $(this).prop("disabled", true);
                                    }
                                });

                                var form = this;
                                $.requestJSON($(this).attr('action'), dataToSend, function (result, message, data)
                                {
                                    thisObject.data("requestJSON_submitting", null);

                                    try
                                    {
                                        $.proxy(config.after, form)(result, message, data);
                                    }
                                    finally
                                    {
                                        $.each(elementsDisabled, function (index, element)
                                        {
                                            $(element).prop("disabled", false);
                                        });
                                    }
                                });
                            }
                        }
                        catch (err)
                        {
                            try
                            {
                                $.proxy(config.after, this)(JsonResult.JSERROR, err, null);

                                $.each(elementsDisabled, function (index, element)
                                {
                                    $(element).prop("disabled", false);
                                });
                            }
                            finally { thisObject.data("requestJSON_submitting", null); }
                        }
                    });

                    $(this).data("requestJSON", configRJ);
                }
            }
        }
        else throw new Error("requestJSON: элемент не является формой.");
    };

    $.fn.requestLoad = function (url, postData, callback)
    {
        var getType = {};
        if (!callback || getType.toString.call(callback) != '[object Function]') throw new Error("callback должен быть функцией.");

        url = requestMethods.prepareUrl(url);

        var func = function (result, status, xhr)
        {
            var message = result;
            if (status == 'error')
            {
                success = JsonResult.NETWORKERROR;
                if (xhr.ResponseText != undefined) message = xhr.ResponseText;
                if (xhr.responseText != undefined) message = xhr.responseText;

                if (message == undefined || message == null) message = "";

                try
                {
                    if (message.length == 0 && xhr.getAllResponseHeaders().length == 0) message = "Удаленный сервер не отвечает.";
                }
                catch (err) { }
            }
            else if (status == 'success') message = "";

            var result = requestMethods.prepareAnswer(message);
            $.proxy(callback, this)(result.Success, result.Message, result.Data);
        };

        return this.each(function ()
        {
            $(this).load(url, postData, $.proxy(func, this));
        });
    };

    $.fn.requestFileUploadSingle = function (settings)
    {
        if ($.fn.uploadFile == undefined) alert(100);

        var defaults = {
            after: function () { },
            before: function () { },

            uploadStr: "Загрузить",
            dragDropStr: "<span><b>Перетащите изображения сюда</b></span>",
            cancelStr: "Отмена",
            doneStr: "готово",
            multiDragErrorStr: "Загрузка папок не разрешена",
            extErrorStr: "не допускается. Разрешенные файлы:",
            sizeErrorStr: "не допускается. Разрешенный максимальный размер:",
            uploadErrorStr: "Загрузка не удалась",

            statusBarWidth: 'auto',
            dragdropWidth: 'auto',
            autoSubmit: true,
            showProgress: true,
        };

        var defaultsFileUploadHardCoded = {
            url: requestMethods.prepareUrl("/c7168ad3-1aa8-0b36-aa47-75163eed6383/UploadFile"),
            returnType: 'json',
        };

        if (this.length > 1) throw new Error("requestFileUploadSingle: можно применять только к одному элементу за вызов.");
        else if (this.length > 0)
        {

            if (!$(this).is("div")) throw new Error("requestFileUploadSingle: элемент не является div.");

            var element = $(this);

            var getConfig = function (settingsNew)
            {
                var cfg = $.extend(this.config, defaults, settingsNew, defaultsFileUploadHardCoded);
                cfg.onSubmit = function (files)
                {
                    $.proxy(cfg.before, element)(files);
                    $(element).trigger('requestFileUploadSingleBefore', files);
                };
                cfg.onSuccess = function (files, data, xhr, pd)
                {
                    var result = requestMethods.prepareAnswer(data);
                    $.proxy(cfg.after, element)(result.Success, result.Message, result.Data);
                    $(element).trigger('requestFileUploadSingleAfter', [result.Success, result.Message, result.Data]);
                };
                cfg.onError = function (files, status, errMsg, pd)
                {
                    $.proxy(cfg.after, element)(JsonResult.NETWORKERROR, "Ошибка загрузки файла. " + status + " " + errMsg, null);
                    $(element).trigger('requestFileUploadSingleAfter', [JsonResult.NETWORKERROR, "Ошибка загрузки файла. " + status + " " + errMsg, null]);
                };
                return cfg;
            }

            var config = getConfig(settings);

            var uploadObject = $(this).uploadFile(config);

            var uploadObjectUpdate = uploadObject.update;

            uploadObject.update = function (settingsNew) {
                var config = getConfig(settingsNew);
                uploadObjectUpdate(config);
            };

            uploadObject.getElement = function () {
                return element;
            };

            uploadObject.addFile = function (idFile) {
                $.proxy(config.after, element)(JsonResult.OK, "", idFile);
                $(element).trigger('requestFileUploadSingleAfter', [JsonResult.OK, "", idFile]);
            };

            return uploadObject;
        }
        else return null;
    };

});

/**
 * AJAX-методы.
 * */
ShowDialogButtons = {
    OK: 0,
    YESNO: 1,
    OKCANCEL: 2,
    SAVECANCEL: 3,
    NOBUTTONS: 4,
};

$(function ()
{
    $.fn.showDialog = function (settings)
    {
        var defaults = {
            success: function () { },
            cancel: function () { },
            show: function () { },
            buttons: ShowDialogButtons.OK,
            closeOnPressEscape: false,
            closeOnClickOutOfForm: false,
        };

        return this.each(function(option)
        {
            if ($(this).is("div"))
            {
                //Формируем настройки.
                var setts = {};
                if ($.isFunction(settings)) setts = { success: settings };
                else setts = settings;

                var thisObject = $(this),
                    target = $("#popup-dialog"),
                    targetContent = target.find(".popup__content:eq(0)"),
                    targetButtonsPanel = target.find(".popup__buttons:eq(0)"),
                    config = $.extend(this.config, defaults, setts),
                    isSet = $(this).data("showDialog") === true;

                if (settings === "destroy" || settings === "close")
                {
                    thisObject.data("showDialog", null);
                    if (isSet)
                    {
                        //Вызываем callback закрытия формы без сохранения.
                        if ($.isFunction(config.cancel)) $.proxy(config.cancel, thisObject)();

                        //Закрытие arcticmodal
                        thisObject.arcticmodal("close");
                    }
                }
                else
                {
                    if (!isSet)
                    {
                        var buttonsType = ShowDialogButtons.OK;
                        switch (config.buttons) {
                            case ShowDialogButtons.OK:
                            case ShowDialogButtons.YESNO:
                            case ShowDialogButtons.OKCANCEL:
                            case ShowDialogButtons.SAVECANCEL:
                            case ShowDialogButtons.NOBUTTONS:
                                buttonsType = config.buttons;
                                break;

                            default:
                                buttonsType = ShowDialogButtons.OK;
                                break;
                        }

                        //Ищем основной контейнер, внутрь которого будет вставлен целевой див.
                        if (target.length === 0) throw "Не найден диалоговый элемент.";
                        else if (target.length > 1) throw "Диалоговых элементов больше одного.";
                        
                        //Две кнопки, которые должны высветиться на форме. Либо одна из двух. Короче, какие кнопки высветили - такие значения дали переменным. Нет кнопки отмены - buttonCancel сделать null.
                        var buttonOK = null;
                        var buttonCancel = null;

                        //Обрабатываем типы кнопок.
                        if (buttonsType === ShowDialogButtons.OK) {
                            //Только кнопка ОК.
                            //TODO - скрыть остальные кнопки.
                            buttonOK = $('<span class="popup__buttons-element popup__button-OK">ОК</span>'); //TODO тут должна присваиваться кнопка ОК.
                            buttonCancel = null;
                        }
                        else if (buttonsType === ShowDialogButtons.YESNO) {
                            //Кнопки "Да" и "Нет". 
                            //TODO - скрыть остальные кнопки.
                            buttonOK = $('<span class="popup__buttons-element popup__button-YES">Да</span>');     //TODO тут должна присваиваться кнопка Да.
                            buttonCancel = $('<span class="popup__buttons-element popup__button-NO">Нет</span>'); //TODO тут должна присваиваться кнопка Нет.
                        }
                        else if (buttonsType === ShowDialogButtons.OKCANCEL) {
                            //Кнопки "ОК" и "Отменить". 
                            //TODO - скрыть остальные кнопки.
                            buttonOK = $('<span class="popup__buttons-element popup__button-OK">ОК</span>');     //TODO тут должна присваиваться кнопка ОК.
                            buttonCancel = $('<span class="popup__buttons-element popup__button-OK">Отменить</span>'); //TODO тут должна присваиваться кнопка Отменить.
                        }
                        else if (buttonsType === ShowDialogButtons.SAVECANCEL) {
                            //Кнопки "Сохранить" и "Отменить". 
                            //TODO - скрыть остальные кнопки.
                            buttonOK = $('<span class="popup__buttons-element popup__button-SAVE">Сохранить</span>');     //TODO тут должна присваиваться кнопка Сохранить.
                            buttonCancel = $('<span class="popup__buttons-element popup__button-CANCEL">Отменить</span>'); //TODO тут должна присваиваться кнопка Отменить.
                        }
                        else if (buttonsType === ShowDialogButtons.NOBUTTONS) {
                            //Без кнопок
                            buttonOK = null;
                            buttonCancel = null;
                        }

                        //Очищаем панель кнопок
                        targetButtonsPanel.html("");
                        
                        //Размещаем кнопки и привязываем события к ним
                        if (buttonOK !== null && $(buttonOK).length > 0)
						{
                            targetButtonsPanel.append(buttonOK);
                            $(buttonOK).click(function ()
                            {
                                if ($.isFunction(config.success))
                                    if ($.proxy(config.success, thisObject)() === false)
                                        return;

								//Закрытие окна
                                thisObject.data("showDialog", null);
                                thisObject.arcticmodal("close");

							});
						}

                        if (buttonCancel !== null && $(buttonCancel).length > 0)
						{
                            targetButtonsPanel.append(buttonCancel);
							$(buttonCancel).click(function () {
                                if ($.isFunction(config.cancel))
                                    if ($.proxy(config.cancel, thisObject)() === false)
										return;
							
								//Закрытие окна
								thisObject.data("showDialog", null);
								thisObject.arcticmodal("close");
							});
						}

                        var copiedContent = thisObject;//.clone(true);
                        targetContent.append(copiedContent.show());

                        thisObject.data("showDialog", true);
						
                        if ($.isFunction(config.show)) $.proxy(config.show, thisObject)();
						
                        // TODO собственно открытие arcticmodal с div внутри. Кнопки уже привязаны.
                        target.arcticmodal({
                            closeOnEsc: config.closeOnPressEscape === true,
                            closeOnOverlayClick: config.closeOnClickOutOfForm === true,
                            overlay: {
                                css: {
                                    backgroundColor: "#222",
                                    opacity: .85
                                }
                            },
                            closeEffect: { type: "none" },
                            beforeClose: function () {
                                if ($.isFunction(config.cancel))
                                    if ($.proxy(config.cancel, thisObject)() === false)
                                        return false;

                                $("body").append(copiedContent.hide());

                                //Закрытие окна
                                thisObject.data("showDialog", null);
                            }
                        });
                    }
                }
            }
            else throw new Error("showDialog: элемент не является div.");
        });
    };

    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

    $.fn.elementsDisabler = function (makeDisabled) {
        return this.each(function () {
            var isDisabled = $(this).data("ElementsDisablerState");
            if (makeDisabled === true) {
                if ($(this).is('input') || $(this).is('button') || $(this).is('textarea') || $(this).is('select')) {
                    if (isDisabled !== true && !$($(this)).is(":disabled")) {
                        $(this).data("ElementsDisablerState", true);
                        $(this).prop("disabled", true);
                    }
                }
            }
            else if (makeDisabled === false) {
                if (isDisabled === true) {
                    $(this).data("ElementsDisablerState", false);
                    $(this).prop("disabled", false);
                }
            }
        });
    };
});

var InvisibleRecaptchaInitCalled = false;
/**
 * Инициализация собственной рекапчи с коллбеком.
 * */
function InvisibleRecaptchaInit(container)
{
    if (container == undefined || container == null) container = $("body");

    $(".captchaInvisible", container).each(function (k, elem)
    {
        var that = $(elem);
        //that.removeClass("g-recaptcha");

        var id = grecaptcha.render(that[0], {
            sitekey: that.data('sitekey'),
            callback: function (token)
            {
                console.log("callback", token, that);
                that.closest("form").submit();
                grecaptcha.reset(id);
            },
            'expired-callback': function (e1, e2)
            {
                console.log("expired-callback", e1, e2);
            },
            'error-callback': function (e1, e2)
            {
                console.log("error-callback", e1, e2);
            }
        });
        console.log(id);
    });

    InvisibleRecaptchaInitCalled = true;
}

/**
 * Здесь инициализация конкретных элементов сайта. 
 * */
$(function ()
{
    $(document).ajaxComplete(function ()
    {
        if (InvisibleRecaptchaInitCalled)
        {

        }
        console.log("ajax complete", $(this));
    });

    /**
     * Применяем фильтр к таблицам, если указано.
     * */
    $("table.tablesorter").each(function (e, table)
    {
        if ($("th.filter", table).length > 0)
        {
            //Инициализируем tablesorter с фильтрацией.
            $("th", table).each(function (e, column)
            {
                if (!$(column).hasClass("filter")) $(column).addClass('filter-false');
            });

            $(table).tablesorter({
                // hidden filter input/selects will resize the columns, so try to minimize the change
                //widthFixed : true,
                widthFixed: false,

                // initialize zebra striping and filter widgets
                widgets: ["filter"],

                ignoreCase: true,

                widgetOptions: {
                },
            });
        }
        else
        {
            //Если фильтров не надо, то просто инициализируем tablesorter.
            $(table).tablesorter();
        }
    });

    /**
     * Инициализация собственной рекапчи с коллбеком.
     * */
    try
    {
        var d = grecaptcha;
        InvisibleRecaptchaInit();
    }
    catch (err)
    {
        $.getScript("https://www.google.com/recaptcha/api.js?onload=InvisibleRecaptchaInit&render=explicit");
    }

    /**
     * Привязка к ссылке "Сообщить об ошибке".
     * */
    $(".js-show-dialog__send-error").click(function (e)
    {
        e.preventDefault();
        return;

        var dialogElement = $("div#show-dialog__send-error:first");
        if ($(this).data("element") != null && $(this).data("element").length > 0) dialogElement = $("div#" + $(this).data("element")).first();

        if (dialogElement.length == 0)
        {
            dialogElement = $("<div></div>").attr("id", "show-dialog__send-error");
            $("body").append(dialogElement);
        }

        dialogElement.showDialog({
            buttons: ShowDialogButtons.SAVECANCEL,
            show: function ()
            {
                var parent = $(this);
                $(this).show().html("<img src='/data/img/loading.gif'>").requestLoad('/support/TicketNewJson', null, function (result, message)
                {
                    if (result != JsonResult.OK) $(this).html(message);
                });
            },
            success: function ()
            {
                var form = $("form", $(this));
                if (form.length > 0)
                {
                    $("form", $(this)).submit();
                    console.log("form submit");
                    return false;
                }
            },
            cancel: function ()
            {
                console.log("cancel");
            }
        });
    });

    /**
     * Для всех полей с type=url добавляем обработку значения с обрезанием пробельных символов.
     * */
    $("[type='url']").on('change keyup', function ()
    {
        var valueTrimmed = $(this).val().trim();
        if (valueTrimmed != $(this).val()) $(this).val(valueTrimmed).change();
    });
});

