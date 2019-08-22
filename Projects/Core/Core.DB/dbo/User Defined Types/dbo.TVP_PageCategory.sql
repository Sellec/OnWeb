CREATE TYPE [dbo].[TVP_PageCategory] AS TABLE (
    [id]          INT           DEFAULT ((0)) NOT NULL,
    [sub_id]      INT           DEFAULT ((-1)) NOT NULL,
    [name]        VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [urlname]     VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [description] VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [language]    VARCHAR (20)  DEFAULT (N'') NOT NULL,
    [status]      SMALLINT      DEFAULT ((0)) NOT NULL);

