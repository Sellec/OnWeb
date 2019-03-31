CREATE TYPE [dbo].[TVP_banners_category] AS TABLE (
    [id]          INT           DEFAULT ((0)) NOT NULL,
    [sub_id]      INT           DEFAULT ((-1)) NOT NULL,
    [name]        VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [description] VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [status]      SMALLINT      DEFAULT ((0)) NOT NULL);

