<{extends "baseCustomer.tpl"}>

<{block 'head'}>
<link rel="stylesheet" type="text/css" href="/data/css/jquery-ui-clear.css" />
<script type="text/javascript">
$(function(){
    $( "#tabs" ).tabs();
})
</script>
<{/block}>
<{block 'body'}>
             <h2>Данные аккаунта</h2>
             <div id="customer_index_data">
                 <div>
                  <label for="email">Эл. почта: </label>
                  <span><{$data.email}></span>
                 </div>
                 
                 <{getCustom type=getLegalEntities assign=legal}>
                 <h2>Ваши юридические лица (<a href="/tender/legalEntities">управление</a>)</h2>
                 <{if $legal|@count > 0}>
                 <div id="tabs">
                    <ul>
                        <{foreach from=$legal item=ad key=id name=legals}>
                        <li><a href="#tabs-<{$smarty.foreach.legals.iteration}>"><{$ad.NameLegalEntity}></a></li>
                        <{/foreach}>
                    </ul>
                      
                    <{foreach from=$legal item=ad key=id name=legals_com}>
                      <div id="tabs-<{$smarty.foreach.legals_com.iteration}>">
                      <fieldset><legend>Данные компании</legend>
                         <div class="padleft">
                          <label for="name">Наименование: </label>
                          <span><{$ad.NameLegalEntity}></span>
                         </div>
                         <div class="padleft">
                          <label>Форма организации: </label>
                          <span><{$ad.company_form}></span>
                         </div>
                         <div class="padleft">
                          <label>ИНН: </label>
                          <span><{$ad.company_inn}></span>
                         </div>
                         <div class="padleft">
                          <label>КПП: </label>
                          <span><{$ad.company_kpp}></span>
                         </div>
                         <div class="padleft">
                          <label>ОГРН: </label>
                          <span><{$ad.company_ogrn}></span>
                         </div>
                         <div class="wrapper"></div>
                     
                         <div class="padleft">
                          <label>Юр. адрес: </label>
                          <span><{$ad.company_address}></span>
                         </div>
                         <div class="padleft">
                          <label>Реальный адрес: </label>
                          <span><{if strlen($ad.company_address_real)>0}><{$ad.company_address_real}><{else}><{$ad.company_address}><{/if}></span>
                         </div>
                         <div class="padleft">
                          <label>Телефоны: </label>
                          <span><{$ad.company_phones}></span>
                         </div>
                         <div class="wrapper"></div>
                         
                         <div class="padleft">
                          <label>Генеральный директор: </label>
                          <span><{$ad.company_gendir}></span>
                         </div>
                         <div class="wrapper"></div>
                       </fieldset>
                       <fieldset><legend>Контактные данные</legend>                 
                         <div class="padleft">
                          <label>Контактное лицо: </label>
                          <span><{$ad.company_contname}></span>
                         </div>
                         <div class="padleft">
                          <label>Телефон для связи: </label>
                          <span><{$ad.company_contphone}></span>
                         </div>
                         <div class="padleft">
                          <label>Должность: </label>
                          <span><{$ad.company_contpos}></span>
                         </div>
                       </fieldset>
                       <fieldset><legend>Состояние</legend>
                        <span id='state_<{$id}>'><{$ad.StateName}></span>
                        <{if $ad.State == UserState::WaitForAcceptEmail}>
                        <div id='buttons_<{$id}>'><input type='button' class='le_accept' rel='<{$id}>' value='Принять'>&nbsp;<input type='button' class='le_decline' rel='<{$id}>' value='Отклонить'></div>
                        <{elseif $ad.State == UserState::WaitForAcceptAdmin}>
                        <div id='buttons_<{$id}>'><input type='button' class='le_accept' rel='<{$id}>' value='Принять принудительно'></div>
                        <{/if}>
                       </fieldset>
                       </div>
                    <{/foreach}>
                 </div>
                 <{else}>
                    Нет добавленных юридических лиц в этом аккаунте.<br /><a href="/tender/legalEntities">Перейти</a> к добавлению и управлению.<br /><br />
                 <{/if}>
                 <{*<fieldset><legend>Данные компании</legend>
                     <div class="form_500">
                      <label for="name">Наименование: </label>
                      <span><{$data.name}></span>
                     </div>
                     <div>
                      <label>Форма организации: </label>
                      <span><{$data.company_form}></span>
                     </div>
                     <div class="form_300">
                      <label>ИНН: </label>
                      <span><{$data.company_inn}></span>
                     </div>
                     <div class="form_300">
                      <label>КПП: </label>
                      <span><{$data.company_kpp}></span>
                     </div>
                     <div class="form_300">
                      <label>ОГРН: </label>
                      <span><{$data.company_ogrn}></span>
                     </div>
                     <div class="wrapper"></div>
                 
                     <div>
                      <label>Юр. адрес: </label>
                      <span><{$data.company_address}></span>
                     </div>
                     <div>
                      <label>Реальный адрес: </label>
                      <span><{if strlen($data.company_address_real)>0}><{$data.company_address_real}><{else}><{$data.company_address}><{/if}></span>
                     </div>
                     <div class="form_half">
                      <label>Телефоны: </label>
                      <span><{$data.company_phones}></span>
                     </div>
                     <div class="wrapper"></div>
                     
                     <div class="form_half">
                      <label>Генеральный директор: </label>
                      <span><{$data.company_gendir}></span>
                     </div>
                     <div class="wrapper"></div>
                 </fieldset>
                 <fieldset><legend>Контактные данные</legend>                 
                     <div class="form_half">
                      <label>Контактное лицо: </label>
                      <span><{$data.company_contname}></span>
                     </div>
                     <div class="form_half_r">
                      <label>Телефон для связи: </label>
                      <span><{$data.company_contphone}></span>
                     </div>
                     <div class="form_half_r">
                      <label>Должность: </label>
                      <span><{$data.company_contpos}></span>
                     </div>
                 </fieldset> *}>
                 
                 <div>
                  <label>Комментарий: </label>
                  <span><{$data.Comment}></span>
                 </div>
             </div>
<{/block}>
