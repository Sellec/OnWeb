CREATE TABLE [dbo].[CustomFieldsData] (
    [IdFieldData]  INT            IDENTITY (1, 1) NOT NULL,
    [IdField]      INT            NOT NULL,
    [IdItem]       INT            NOT NULL,
    [IdItemType]   INT            NOT NULL,
    [IdFieldValue] INT            NOT NULL,
    [FieldValue]   NVARCHAR (MAX) NOT NULL,
    [DateChange]   INT            NOT NULL,
    CONSTRAINT [PK_CustomFieldsData] PRIMARY KEY CLUSTERED ([IdFieldData] ASC)
);








GO
CREATE NONCLUSTERED INDEX [IdItem_IdItemType]
    ON [dbo].[CustomFieldsData]([IdItem] DESC, [IdItemType] ASC);


GO
CREATE NONCLUSTERED INDEX [IdField_with_IdItem_IdFieldValue]
    ON [dbo].[CustomFieldsData]([IdField] ASC)
    INCLUDE([IdItem], [IdFieldValue]);


GO
CREATE NONCLUSTERED INDEX [IdField_IdItem_with_FieldValue]
    ON [dbo].[CustomFieldsData]([IdField] ASC, [IdItem] ASC)
    INCLUDE([FieldValue]);


GO
ALTER INDEX [IdField_IdItem_with_FieldValue]
    ON [dbo].[CustomFieldsData] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IdField_IdItem_IdItemType_with_FieldValue]
    ON [dbo].[CustomFieldsData]([IdField] ASC, [IdItem] ASC, [IdItemType] ASC)
    INCLUDE([FieldValue]);

