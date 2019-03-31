<{if isset($fields) && is_array($fields)}>
<{foreach from=$fields item=ad key=id}>
 <{if $ad.field_type == 1 || $ad.field_type == 5}>      
 <li class="editable">
  <label><{$ad.field_name}>: <input type="hidden" value="1" /><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></label><br />
  <label><input type='text' name='field_value_<{$id}>[]' size='40' maxlength='200' value='<{$ad.field_value}>'> <{$ad.NameEnding}></label>
 </li>
 <{elseif $ad.field_type == 7}>
 <li class=" class="editable"">
  <label><{$ad.field_name}>: <input type="hidden" value="1" /><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></label><br />
  <label><textarea name='field_value_<{$id}>[]' rows='5' cols='40'><{$ad.field_value}></textarea> <{$ad.NameEnding}></label>
 </li>
 <{elseif $ad.field_type == 9}>
 <li class="editable">
  <label><{$ad.field_name}>: <input type="hidden" value="1" /><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></label><br />
  <label><textarea name='field_value_<{$id}>[]' rows='5' cols='40'><{$ad.field_value}></textarea> <{$ad.NameEnding}></label>
 </li>
 <{elseif $ad.field_type == 2}>
 <li class="editable">
  <label><{$ad.field_name}>: <input type="hidden" value="1" /><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></label><br />
  <label>
   <select name="field_value_<{$id}>[]">
   <{foreach from=$ad.field_data item=ad2 key=id2}>
     <option value='<{$ad2.value}>'<{if $ad2.value|in_array:$ad.field_value}> selected="selected"<{/if}>> <{$ad2.text}> <{$ad.NameEnding}></option>
    <{/foreach}>
   </select>
  </label>
 </li>
 <{elseif $ad.field_type == 3}>
 <li class="editable">
  <label><{$ad.field_name}>: <input type="hidden" value="1" /><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></label><br />
  <label>
   <select name="field_value_<{$id}>[]" size="10" multiple name="field_value_<{$id}>[]">
    <{foreach from=$ad.field_data item=ad2 key=id2}>
     <option value='<{$ad2.value}>'<{if $ad2.value|in_array:$ad.field_value}> selected="selected"<{/if}>> <{$ad2.text}> <{$ad.NameEnding}></option>
    <{/foreach}>
   </select>
  </label>
 </li>
 <{elseif $ad.field_type == 8}>
 <li class="editable">
  <label><{$ad.field_name}>: <input type="hidden" value="1" /><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></label><br />
  <label>
   <select name="field_value_<{$id}>[]" size="10" multiple name="field_value_<{$id}>[]">
    <{foreach from=$ad.field_data item=ad2 key=id2}>
     <option value='<{$ad2.value}>'<{if $ad2.value|in_array:$ad.field_value}> selected="selected"<{/if}>> <{$ad2.text}> <{$ad.NameEnding}></option>
    <{/foreach}>
   </select>
  </label>
 </li>
 <{elseif $ad.field_type == 4}>
 <li class="tcapt">
  <label><{$ad.field_name}> <input type="hidden" value="1" /><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></label><br />
  <label><input type="hidden" class='disable_elem' name="field_value_<{$ad.field_id}>" value="1" /></label>
 </li>
 <{elseif $ad.field_type == 6}>
 <li class='editable'>
  <label><{$ad.field_name}>: <input type="hidden" value="1" /><input type="hidden" class='disable_elem' name="field_<{$ad.field_id}>" value="-1"></label><br />
  <label>
   <select id="type_6_cats" name='field_value_<{$id}>[]' multiple size='10'>
    <{foreach from=$ad.field_data item=ad2 key=id2}>
     <option value='<{$id2}>' <{if $id2|in_array:$ad.field_value}>selected<{/if}>><{$ad2.name}> <{$ad.NameEnding}></option>
    <{/foreach}>
    </select>
    <p><a href="" id="deselect_accs">Снять выделение</a></p>
   </label>
  </li>
 <{/if}>
<{/foreach}>
<{/if}>