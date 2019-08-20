CREATE TYPE [dbo].[TVP_CustomFieldsValue] AS TABLE (
    [IdFieldValue] INT            DEFAULT ((0)) NOT NULL,
    [IdField]      INT            DEFAULT ((0)) NOT NULL,
    [FieldValue]   NVARCHAR (200) DEFAULT ('') NOT NULL,
    [Order]        INT            DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            DEFAULT ((0)) NOT NULL,
    [old_index]    INT            DEFAULT ((0)) NOT NULL);

