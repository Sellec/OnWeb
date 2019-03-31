CREATE TABLE [dbo].[news_category] (
    [id]          INT           IDENTITY (4, 1) NOT NULL,
    [sub_id]      INT           DEFAULT ((-1)) NOT NULL,
    [name]        VARCHAR (MAX) NOT NULL,
    [urlname]     VARCHAR (MAX) NOT NULL,
    [description] VARCHAR (MAX) NOT NULL,
    [status]      SMALLINT      DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_news_category_id] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.news_category', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'news_category';

