CREATE TABLE [dbo].[News] (
    [id]             INT           IDENTITY (102, 1) NOT NULL,
    [category]       INT           CONSTRAINT [DF__news__category__10CB707D] DEFAULT ((0)) NOT NULL,
    [status]         BIT           CONSTRAINT [DF__news__status__11BF94B6] DEFAULT ((0)) NOT NULL,
    [name]           VARCHAR (300) NOT NULL,
    [short_text]     VARCHAR (MAX) NULL,
    [text]           VARCHAR (MAX) NULL,
    [date]           DATETIME      CONSTRAINT [DF__news__date__12B3B8EF] DEFAULT (getdate()) NOT NULL,
    [comments_count] INT           CONSTRAINT [DF__news__comments_c__13A7DD28] DEFAULT ((0)) NOT NULL,
    [user]           INT           CONSTRAINT [DF__news__user__168449D3] DEFAULT ((-1)) NOT NULL,
    [image]          VARCHAR (255) NULL,
    [Block]          BIT           CONSTRAINT [DF_News_Block] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_news_id] PRIMARY KEY CLUSTERED ([id] ASC)
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.news', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'news';

