CREATE TYPE [dbo].[TVP_CustomFieldsValueType] AS TABLE (
    [IdValueType]   INT            DEFAULT ((0)) NOT NULL,
    [NameValueType] NVARCHAR (100) DEFAULT ('') NOT NULL);

