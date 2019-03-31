<{if isset($fields) && is_array($fields)}>
<{foreach from=$fields item=ad key=id}>
 <{if $ad.field_type == 1 || $ad.field_type == 5}>      
 <tr class="editable">
  <td width="175"><{$ad.field_name}>:<input type="hidden" value="1"><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></td>
  <td>
     <input type='text' name='field_value_<{$id}>[]' size='40' maxlength='200' value='<{$ad.field_value}>' style='margin-bottom:10px;'> <{$ad.NameEnding}>
  </td>
 </tr>
 <{elseif $ad.field_type == 7}>
 <tr class="editable">
  <td width="175"><{$ad.field_name}>:<input type="hidden" value="1"><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></td>
  <td>
     <textarea name='field_value_<{$id}>[]' rows='5' cols='40'><{$ad.field_value}></textarea>
  </td>
 </tr>
 <{elseif $ad.field_type == 9}>
 <tr class="editable">
  <td width="175"><{$ad.field_name}>:<input type="hidden" value="1"><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></td>
  <td>
     <textarea name='field_value_<{$id}>[]' rows='5' cols='40'><{$ad.field_value}></textarea> <{$ad.NameEnding}>
  </td>
 </tr>
 <{elseif $ad.field_type == 2}>
 <tr class="editable">
  <td width="175"><{$ad.field_name}>:<input type="hidden" value="1"><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></td>
  <td>
   <{foreach from=$ad.field_data item=ad2 key=id2}>
    <input type='radio' name='field_value_<{$id}>[]' value='<{$ad2.value}>' <{if $ad2.value|in_array:$ad.field_value}>checked<{/if}>> <{$ad2.text}> <{$ad.NameEnding}> &nbsp;
   <{/foreach}>
  </td>
 </tr>
 <{elseif $ad.field_type == 3}>
 <tr class="editable">
  <td width="175"><{$ad.field_name}>:<input type="hidden" value="1"><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></td>
  <td>
   <{foreach from=$ad.field_data item=ad2 key=id2}>
    <input type='checkbox' name='field_value_<{$id}>[]' value='<{$ad2.value}>' <{if $ad2.value|in_array:$ad.field_value}>checked<{/if}>> <{$ad2.text}> <{$ad.NameEnding}> &nbsp;
   <{/foreach}>
  </td>
 </tr>
 <{elseif $ad.field_type == 8}>
 <tr class="editable">
  <td width="175"><{$ad.field_name}>:<input type="hidden" value="1"><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></td>
  <td>
   <{foreach from=$ad.field_data item=ad2 key=id2}>
    <input type='checkbox' name='field_value_<{$id}>[]' value='<{$ad2.value}>' <{if $ad2.value|in_array:$ad.field_value}>checked<{/if}>> <{$ad2.text}> <{$ad.NameEnding}> &nbsp;
   <{/foreach}>
  </td>
 </tr>
 <{elseif $ad.field_type == 4}>
 <tr class="tcapt">
  <td><{$ad.field_name}><input type="hidden" value="1"><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></td>
  <td><input type="hidden" class='disable_elem' name="field_value_<{$ad.field_id}>" value="1"></td>
 </tr>
 <{elseif $ad.field_type == 6}>
 <tr class="editable">
  <td width="175"><{$ad.field_name}>:<input type="hidden" value="1"><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></td>
  <td>
   <select id='type_6_cats' name='field_value_<{$id}>[]' multiple size='10'>
   <{foreach from=$ad.field_data item=ad2 key=id2}>
    <option value='<{$id2}>' <{if $id2|in_array:$ad.field_value}>selected<{/if}>><{$ad2.name}> <{$ad.NameEnding}></option>
   <{/foreach}>
   </select>
   <p><a href="" id="deselect_accs">Снять выделение</a></p>
  </td>
 </tr>
 <{/if}>
<{/foreach}>
<{/if}>