CREATE TYPE [dbo].[TVP_articles_category] AS TABLE (
    [id]          INT            DEFAULT ((0)) NOT NULL,
    [sub_id]      INT            DEFAULT ((-1)) NOT NULL,
    [name]        NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [urlname]     NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [description] NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [status]      SMALLINT       DEFAULT ((1)) NOT NULL);

