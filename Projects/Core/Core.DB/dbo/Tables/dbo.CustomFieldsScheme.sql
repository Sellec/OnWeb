CREATE TABLE [dbo].[CustomFieldsScheme] (
    [IdScheme]   INT            IDENTITY (1, 1) NOT NULL,
    [IdModule]   INT            NOT NULL,
    [NameScheme] NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_CustomFieldsScheme] PRIMARY KEY CLUSTERED ([IdScheme] ASC)
);

