CREATE TABLE [dbo].[articles_category] (
    [id]          INT            IDENTITY (6, 1) NOT NULL,
    [sub_id]      INT            DEFAULT ((-1)) NOT NULL,
    [name]        NVARCHAR (MAX) NOT NULL,
    [urlname]     NVARCHAR (MAX) NOT NULL,
    [description] NVARCHAR (MAX) NOT NULL,
    [status]      SMALLINT       DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_articles_category_id] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.articles_category', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'articles_category';

