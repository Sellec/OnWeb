CREATE TABLE [dbo].[articles] (
    [id]             INT            IDENTITY (18, 1) NOT NULL,
    [category]       INT            DEFAULT ((-1)) NULL,
    [status]         SMALLINT       DEFAULT ((1)) NOT NULL,
    [name]           NVARCHAR (MAX) NOT NULL,
    [urlname]        NVARCHAR (MAX) NOT NULL,
    [text]           NVARCHAR (MAX) NOT NULL,
    [date]           INT            DEFAULT ((0)) NOT NULL,
    [comments_count] INT            DEFAULT ((0)) NOT NULL,
    [user]           INT            DEFAULT ((0)) NOT NULL,
    [news_id]        INT            DEFAULT ((0)) NOT NULL,
    [photo]          NVARCHAR (250) CONSTRAINT [DF_articles_photo] DEFAULT ('') NOT NULL,
    CONSTRAINT [PK_articles_id] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.articles', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'articles';

