CREATE TABLE [dbo].[CustomFieldsValueType] (
    [IdValueType]   INT            IDENTITY (1, 1) NOT NULL,
    [NameValueType] NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_CustomFieldsValueType] PRIMARY KEY CLUSTERED ([IdValueType] ASC)
);

