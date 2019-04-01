<div id='calendar_panel'>
<script type='text/javascript'>
$(document).ready(function(){
    try{
        
        var mDays = [];
<{foreach from=$data item=ad key=id}>
        mDays[mDays.length] = "<{$ad}>";
<{/foreach}>
        
    $(".panel_sel_date").datepicker({ 
        showOtherMonths: true,
        selectOtherMonths: true,
        yearRange: '2009:2012',
        showMonthAfterYear: true,
        changeMonth: true,
        changeYear: true,
        onSelect: function(dateText, inst) {
            window.open('/@Module.UrlName/date/'+inst.selectedDay+'.'+(parseInt(inst.selectedMonth)+1)+'.'+inst.selectedYear,'window');
            return false;
        },
        firstDay: 1 , // первый день понедельник
        hightlight : { // подсвечиваем
            format:"dd/mm/yy", // формат в котором указаны даты, понимает все форматы которые понимает datepicker, по умолчанию равен $.datepicker._defaults.dateFormat или mm/dd/yy
            values:mDays, // список дат в том формате который мы тока что указали в прошлом параметре
            titles:[], // если хотите можно указать список всплывающих подсказок для дат
            settings:{} // дополнительные параметры для функции преобразования строк в дату можно посмотреть в комментариях к коду датапикера
        }
    });                           
    
    /*$('input[name=date]').focus();*/
    
    } catch(err) {alert(err);}
});
</script>

<br>--------<br>
<div class="panel_sel_date"></div>
<a href='' id='calendar_panel_link' target='_blank' style='display:;'>1</a>
<input type="text" name="date" class="sel_date" style='width:0;height:0;border:0;' />
<br>--------<br>
</div>