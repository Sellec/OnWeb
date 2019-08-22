/**
 * Расширение jQuery, позволяющее заполнять таблицы динамическими данными на основе переданного массива. 
 * Ключевой момент записей, передаваемых в таблицу - в записях должны быть поля c именем id.
 * 
 *
 * @name tableFill
 * @type jQuery
 * @option array Data                   Массив данных, на основе которых необходимо заполнить таблицу. 
 * 
 * @option function RowHandler          Функция, обрабатывающая каждую строку в таблице. Получает два аргумента: @option object new_row,@option array Data
 * 
 * @option function Filter              Функция-фильтр, обрабатывающая каждую строку в таблице и определяющая, показывать строку или нет. Получает один аргумент: @option array Data. Возвращает true/false
 * 
 * @option function AfterLoadingHandler Функция, вызываемая после окончания обработки таблицы.
 * 
 */
jQuery.fn.tablefill = function (Data, RowHandler, Filter, AfterLoadingHandler) {
    function update_each_row(obj, RowData, RowHandler) {
        try {
            var cloned = $(obj).find('tr#obraz').clone(true);
            cloned.show().attr('id', 'tr_res_' + RowData["id"]).data('BoundedItem', RowData);
            if (RowHandler != null) RowHandler(cloned, RowData);

            if ($(obj).find('tbody') != undefined && $(obj).find('tbody').length > 0)
                $(obj).find('tbody').first().append(cloned);
            else
                $(obj).append(cloned);

        }
        catch (err) { console.log('update_each_row: ' + err, obj, RowData); }
    };

    return this.each(function () {
        var ser = /tr_res_/i;
        var elems = $(this).find('tr').each(function (id, elem) {
            if ($(elem).data('BoundedItem') != undefined) $(elem).remove();
            else if ($(elem).attr('id') != undefined && $(elem).attr('id').length > 0 && $(elem).attr('id').search(ser) != -1) $(elem).remove();
        });

        var _RowHandler = $.proxy(RowHandler, this);
        var _Filter = $.proxy(Filter, this);
        var _AfterLoadingHandler = $.proxy(AfterLoadingHandler, this);

        $(this).data('BoundedItem', Data);
        $(this).data('BoundedRowHandler', _RowHandler);
        $(this).data('BoundedFilter', _Filter);
        $(this).data('BoundedAfterLoadingHandler', _AfterLoadingHandler);

        var filt = false;
        var count = 0;
        if (typeof (_Filter) == 'function') filt = true;

        if (typeof (Data) != 'undefined' && Data != null) {
            var t = this;
            $.each(Data, function (key, value) {
                if (value != null && value != 'undefined' && value != undefined && (filt == false || _Filter(value) != false)) {
                    update_each_row(t, value, _RowHandler);
                    count++;
                }
            });
        }

        if (count == 0) $(this).find('tr#notfounded').show();
        else $(this).find('tr#notfounded').hide();

        $(this).find('tr#obraz').hide();

        if (typeof ($.fn.tablesorter) != 'undefined')
        {
            var resort = true;
            $(this).tablesorter().trigger('updateAll', [resort]);
        }
        //if ( typeof($.fn.columnFilters) != 'undefined' ) $(this).columnFilters();

        if (typeof (_AfterLoadingHandler) == 'function') _AfterLoadingHandler(this);
    });

};

jQuery.fn.tablefillAdd = function (RowData) {
    function update_each_row(obj, RowData, RowHandler) {
        try {
            var cloned = $(obj).find('tr#obraz').clone(true);
            cloned.show().attr('id', 'tr_res_' + RowData["id"]).data('BoundedItem', RowData);
            if (RowHandler != null) RowHandler(cloned, RowData);

            if ($(obj).find('tbody') != undefined && $(obj).find('tbody').length > 0) $(obj).find('tbody').first().append(cloned);
            else $(obj).append(cloned);
        }
        catch (err) { console.log('update_each_row: ' + err, obj, RowData); }
    };

    return this.each(function () {
        RowHandler = $(this).data('BoundedRowHandler');
        Filter = $(this).data('BoundedFilter');
        AfterLoadingHandler = $(this).data('BoundedAfterLoadingHandler');

        var filt = false;
        var count = 0;
        if (typeof (Filter) == 'function') filt = true;

        if (typeof (RowData) != 'undefined') {
            if (filt == false || Filter(RowData) != false) {
                update_each_row(this, RowData, RowHandler);
                count++;
            }
        }

        if (count == 0) $(this).find('tr#notfounded').show();
        else $(this).find('tr#notfounded').hide();

        $(this).find('tr#obraz').hide();

        if (typeof ($.fn.tablesorter) != 'undefined') $(this).tablesorter();
        //if ( typeof($.fn.columnFilters) != 'undefined' ) $(this).columnFilters();

        if (typeof (AfterLoadingHandler) == 'function') AfterLoadingHandler(this);
    });

};

/**
 * Расширение jQuery, позволяющее немедленно отфильтровать таблицу.
 * 
 *
 * @name tablefillFilter
 * @type jQuery
 * @option array Data                   Массив данных, на основе которых заполнялась таблицу. 
 * 
 * @option function Filter         Функция-фильтр, обрабатывающая каждую строку в таблице и определяющая, показывать строку или нет. Получает один аргумент: @option array Data. Возвращает true/false
 * 
 */
jQuery.fn.tablefillFilter = function (Data, Filter) {
    return this.each(function () {
        if (typeof (Data) == 'undefined' || typeof (Filter) != 'function') return;

        var ser = /tr_res_/i;
        var count = 0;
        var elems = $(this).find('tr').each(function (id, elem) {
            var attrid = $(elem).attr('id');
            if (attrid.length > 0 && attrid.search(ser) != -1) {
                var _data = attrid.match(/tr_res_(\d+)/i);
                if (_data == null) return;

                if (typeof (data[_data[1]]) == 'undefined') return;
                if (Filter(data[_data[1]])) {
                    $(elem).show();
                    count++;
                } else $(elem).hide();
            }

        });

        if (count == 0) $(this).find('tr#notfounded').show();
        else $(this).find('tr#notfounded').hide();
    });

};


jQuery.fn.tableFillView = function () {
    return this.each(function () {
        if (typeof ($.fn.tablesorter) != 'undefined') $(this).tablesorter();
        if (typeof ($.fn.columnFilters) != 'undefined') $(this).columnFilters();
    });

};
