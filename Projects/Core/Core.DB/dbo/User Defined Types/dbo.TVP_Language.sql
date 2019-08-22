CREATE TYPE [dbo].[TVP_Language] AS TABLE (
    [IdLanguage]    INT            DEFAULT ((0)) NOT NULL,
    [NameLanguage]  NVARCHAR (200) DEFAULT ('') NOT NULL,
    [ShortAlias]    NVARCHAR (20)  DEFAULT ('') NOT NULL,
    [IsDefault]     INT            DEFAULT ((0)) NOT NULL,
    [TemplatesPath] VARCHAR (200)  DEFAULT ('') NOT NULL,
    [Culture]       VARCHAR (20)   DEFAULT ('') NOT NULL);

