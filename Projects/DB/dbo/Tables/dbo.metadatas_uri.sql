CREATE TABLE [dbo].[metadatas_uri] (
    [md_id]          INT             IDENTITY (1, 1) NOT NULL,
    [md_uri]         NVARCHAR (300)  NOT NULL,
    [md_keywords]    NVARCHAR (500)  NOT NULL,
    [md_description] NVARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_metadatas_uri_md_id] PRIMARY KEY CLUSTERED ([md_id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'capitalrent.metadatas_uri', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'metadatas_uri';

