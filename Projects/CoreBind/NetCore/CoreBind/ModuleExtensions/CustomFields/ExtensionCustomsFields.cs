namespace OnWeb.Core.ModuleExtensions.CustomFields
{
    using Modules;
    using Modules.Extensions;

    /// <summary>
    /// Класс расширения пользовательских полей. 
    /// </summary>
    [ModuleExtension("CustomFields")]
    public class ExtensionCustomsFields : ExtensionCustomsFieldsBase
    {
        private object SyncRoot = new object();

        public ExtensionCustomsFields(ModuleCore moduleObject)
            : base(moduleObject)
        {
        }

        //    /*
        //    Выводит данные полей. 
        //    $schemeID - номер схемы, для которой следует искать поля.
        //    $schemeItemID - номер объекта, к которому привязаны поля и схема. Например, это может быть номер категории.
        //    $itemID - номер объекта, для которого следует искать данные полей.
        //    */
        //    public function displayItem($schemeID, $schemeItemID, $itemID, $IdItemType = null, $templateType = null)
        //    {
        //        $schemeItemID = (int)$schemeItemID;
        //        $schemeID = (int)$schemeID;
        //        $itemID = (int)$itemID;

        //        $data_items = array($itemID => array());
        //        $this.prepareItems($schemeItemID, ModuleCore::CategoryType, $schemeID, $data_items, true, $IdItemType);

        //        foreach ($data_items[$itemID]['fields"] as $k=>$v)
        //        {
        //            //echo "$k=>".$v['name"].'<br>';
        //        }

        //        $this.assign('fields', $data_items[$itemID]['fields"]);

        //        if ($templateType != null) $this.display('customs_fields_view_'.$templateType.'.cshtml");
        //        else $this.display('customs_fields_view.cshtml");
        //    }

        //    /*
        //    Выводит данные полей. 
        //    $schemeItemID - номер объекта, к которому привязаны поля и схемы. Например, это может быть номер категории.
        //    $itemID - номер объекта, для которого следует искать данные полей.
        //    */
        //    public function displayEdit($schemeItemID, $itemID = 0, $schemeID = null, $IdItemType = null)
        //    {
        //        $schemeItemID = (int)$schemeItemID;
        //        $itemID = (int)$itemID;

        //        if (!($schemeItemID >= 0)) return;

        //        $data_items = array($itemID => array());
        //        $this.prepareItems($schemeItemID, ModuleCore::CategoryType, $schemeID, $data_items, true, $IdItemType);
        //        $this.assign('fields', isset($data_items[$itemID]['fields"]) ? $data_items[$itemID]['fields"] : array());

        //        $this.display('customs_fields_edit.cshtml");
        //    }
    }
}
