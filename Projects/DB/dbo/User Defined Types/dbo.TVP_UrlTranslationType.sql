CREATE TYPE [dbo].[TVP_UrlTranslationType] AS TABLE (
    [IdTranslationType]          INT            DEFAULT ((0)) NOT NULL,
    [NameTranslationType]        NVARCHAR (100) DEFAULT ('') NOT NULL,
    [DescriptionTranslationType] NVARCHAR (200) DEFAULT ('') NOT NULL);

