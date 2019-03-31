CREATE TABLE [dbo].[Page] (
    [id]             INT           IDENTITY (28, 1) NOT NULL,
    [category]       INT           DEFAULT ((-1)) NULL,
    [subs_id]        VARCHAR (MAX) NOT NULL,
    [subs_order]     VARCHAR (MAX) NOT NULL,
    [status]         SMALLINT      DEFAULT ((0)) NOT NULL,
    [language]       VARCHAR (20)  DEFAULT (N'') NOT NULL,
    [name]           VARCHAR (MAX) NOT NULL,
    [urlname]        VARCHAR (MAX) NOT NULL,
    [body]           VARCHAR (MAX) NOT NULL,
    [parent]         SMALLINT      DEFAULT ((-1)) NOT NULL,
    [order]          INT           DEFAULT ((0)) NOT NULL,
    [photo]          VARCHAR (MAX) NOT NULL,
    [count_views]    INT           DEFAULT ((0)) NOT NULL,
    [comments_count] INT           DEFAULT ((0)) NOT NULL,
    [pages_gallery]  INT           DEFAULT ((-1)) NOT NULL,
    [news_id]        INT           DEFAULT ((0)) NOT NULL,
    [seo_title]      VARCHAR (255) DEFAULT (N'') NOT NULL,
    [seo_descr]      VARCHAR (MAX) NOT NULL,
    [seo_kw]         VARCHAR (MAX) NOT NULL,
    [ajax_name]      VARCHAR (255) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_pages_id] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.pages', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Page';

