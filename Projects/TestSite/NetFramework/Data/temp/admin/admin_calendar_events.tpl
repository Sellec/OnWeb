<script type='text/javascript'>
function calendar_apply_event_result(result_id,result_text,cae_d,cae_m,cae_y)
{
    alert(result_text);
}

$(document).ready(function() {
    $("#block").hide();
    try {
        changeTitle('Просмотр событий');

        var mDays = [];
<{foreach from=$data item=ad key=id}>
        mDays[mDays.length] = "<{$ad}>";
<{/foreach}>
        
    $(".sel_date").datepicker({ 
        showOtherMonths: true,
        selectOtherMonths: true,
        yearRange: '2009:2012',
        showMonthAfterYear: true,
        changeMonth: true,
        changeYear: true,
        onSelect: function(dateText, inst) { 
            try { 
            var ddr = "/admin/madmin/@Module.UrlName/events_date/"+inst.selectedDay+'.'+(parseInt(inst.selectedMonth)+1)+'.'+inst.selectedYear;
            aj = new ajaxRequest();
            aj.load(ddr,'day_events');
            } catch(err) {alert(err);}
        },
        firstDay: 1 , // первый день понедельник
        hightlight : { // подсвечиваем
            format:"dd/mm/yy", // формат в котором указаны даты, понимает все форматы которые понимает datepicker, по умолчанию равен $.datepicker._defaults.dateFormat или mm/dd/yy
            values:mDays, // список дат в том формате который мы тока что указали в прошлом параметре
            titles:[], // если хотите можно указать список всплывающих подсказок для дат
            settings:{} // дополнительные параметры для функции преобразования строк в дату можно посмотреть в комментариях к коду датапикера
        }
    });
    
    $('.sel_date').focus();
        
    } catch(err) { alert(err); }
    
});
</script>
<h2>Просмотр событий</h2>

<div class='sel_date1'></div>
<input type="text" name="date" class="sel_date"/>
<div id='day_events'></div>

