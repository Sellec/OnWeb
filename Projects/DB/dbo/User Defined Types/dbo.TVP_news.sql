CREATE TYPE [dbo].[TVP_News] AS TABLE (
    [id]             INT           DEFAULT ((0)) NOT NULL,
    [category]       INT           DEFAULT ((0)) NOT NULL,
    [status]         BIT           DEFAULT ((0)) NOT NULL,
    [name]           VARCHAR (300) DEFAULT ('') NOT NULL,
    [short_text]     VARCHAR (MAX) NULL,
    [text]           VARCHAR (MAX) NULL,
    [date]           DATETIME      DEFAULT (getdate()) NOT NULL,
    [comments_count] INT           DEFAULT ((0)) NOT NULL,
    [user]           INT           DEFAULT ((-1)) NOT NULL,
    [image]          VARCHAR (255) NULL,
    [Block]          BIT           DEFAULT ((0)) NOT NULL);



