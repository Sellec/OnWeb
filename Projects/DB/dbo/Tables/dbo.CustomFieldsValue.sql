CREATE TABLE [dbo].[CustomFieldsValue] (
    [IdFieldValue] INT            IDENTITY (1, 1) NOT NULL,
    [IdField]      INT            NOT NULL,
    [FieldValue]   NVARCHAR (200) NOT NULL,
    [Order]        INT            NOT NULL,
    [DateChange]   INT            NOT NULL,
    [old_index]    INT            CONSTRAINT [DF_CustomFieldsValue_old_index] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CustomFieldsValue] PRIMARY KEY CLUSTERED ([IdFieldValue] ASC)
);

