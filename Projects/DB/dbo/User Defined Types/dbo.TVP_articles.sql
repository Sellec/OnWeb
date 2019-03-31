CREATE TYPE [dbo].[TVP_articles] AS TABLE (
    [id]             INT            DEFAULT ((0)) NOT NULL,
    [category]       INT            DEFAULT ((-1)) NULL,
    [status]         SMALLINT       DEFAULT ((1)) NOT NULL,
    [name]           NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [urlname]        NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [text]           NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [date]           INT            DEFAULT ((0)) NOT NULL,
    [comments_count] INT            DEFAULT ((0)) NOT NULL,
    [user]           INT            DEFAULT ((0)) NOT NULL,
    [news_id]        INT            DEFAULT ((0)) NOT NULL,
    [photo]          NVARCHAR (250) DEFAULT ('') NOT NULL);

