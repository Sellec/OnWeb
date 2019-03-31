CREATE TYPE [dbo].[TVP_CustomFieldsData] AS TABLE (
    [IdFieldData]  INT            DEFAULT ((0)) NOT NULL,
    [IdField]      INT            DEFAULT ((0)) NOT NULL,
    [IdItem]       INT            DEFAULT ((0)) NOT NULL,
    [IdItemType]   INT            DEFAULT ((0)) NOT NULL,
    [IdFieldValue] INT            DEFAULT ((0)) NOT NULL,
    [FieldValue]   NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [DateChange]   INT            DEFAULT ((0)) NOT NULL);

