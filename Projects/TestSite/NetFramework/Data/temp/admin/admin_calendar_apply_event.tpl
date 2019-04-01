<div id='calendar_apply_event_all'>
<script type='text/javascript'>
function calendar_apply_event_result(result_id,result_text,cae_d,cae_m,cae_y)
{
    if ( result_id == 0 ) alert(result_text);
}

function calendar_apply_event_changeaddr(id)
{
    if ( typeof(caeUploader) != 'undefined' )
    {
        mCaeUploaderAddr = "/admin/madmin/@Module.UrlName/calendar_apply_event2/"+id+"/<{$data_header}>";
    }
}

var mCaeUploaderAddr = "/admin/madmin/@Module.UrlName/calendar_apply_event2/<{$data_id}>/<{$data_header}>";
$(document).ready(function(){
    try{
        var mDays = [];
<{foreach from=$datas item=ad key=id}>
        mDays[mDays.length] = "<{$ad}>";
<{/foreach}>
        
    $(".sel_date").datepicker({ 
        showOtherMonths: false,
        selectOtherMonths: true,
        yearRange: '2009:2012',
        altField: '.sel_date',
        showMonthAfterYear: true,
        changeMonth: true,
        changeYear: true,
        onSelect: function(dateText, inst) { 
            try { 
            var ddr = mCaeUploaderAddr+'/'+inst.selectedDay+'/'+(parseInt(inst.selectedMonth)+1)+'/'+inst.selectedYear;
            aj = new ajaxRequest();
            aj.load(ddr,$('div#calendar_apply_event_all').parent().attr('id'));
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
    
    
    $('a.calendar_apply_event_close').click(function(){
        try {       
            $('div#calendar_apply_event_all').hide();
        } catch(err) {alert(err);}
        return false; 
    });
    
    $('input[name=date]').focus();
    
    } catch(err) {alert(err);}
});
</script>

<div id='calendar_apply_event_result'></div>
<div class="panel_sel_date"></div>
<a href='' class='calendar_apply_event_close'>Закрыть календарь</a>
<input type="text" name="date" class="sel_date" style='width:0;height:0;border:0;' />
</div>