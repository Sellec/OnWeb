CREATE TABLE [dbo].[UrlTranslationType] (
    [IdTranslationType]          INT            NOT NULL,
    [NameTranslationType]        NVARCHAR (100) NOT NULL,
    [DescriptionTranslationType] NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_UrlTranslationType] PRIMARY KEY CLUSTERED ([IdTranslationType] ASC)
);



