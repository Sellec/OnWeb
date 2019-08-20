CREATE TYPE [dbo].[TVP_CustomFieldsScheme] AS TABLE (
    [IdScheme]   INT            DEFAULT ((0)) NOT NULL,
    [IdModule]   INT            DEFAULT ((0)) NOT NULL,
    [NameScheme] NVARCHAR (200) DEFAULT ('') NOT NULL);

