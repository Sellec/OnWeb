CREATE TABLE [dbo].[Language] (
    [IdLanguage]    INT            NOT NULL,
    [NameLanguage]  NVARCHAR (200) NOT NULL,
    [ShortAlias]    NVARCHAR (20)  NOT NULL,
    [IsDefault]     INT            NOT NULL,
    [TemplatesPath] VARCHAR (200)  NOT NULL,
    [Culture]       VARCHAR (20)   CONSTRAINT [DF_Language_Culture] DEFAULT ('') NOT NULL
);

