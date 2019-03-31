CREATE TYPE [dbo].[TVP_banners] AS TABLE (
    [id]           INT           DEFAULT ((0)) NOT NULL,
    [category]     INT           DEFAULT ((-1)) NULL,
    [banner_image] VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [status]       SMALLINT      DEFAULT ((1)) NOT NULL,
    [name]         VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [description]  VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [type]         SMALLINT      DEFAULT ((0)) NOT NULL,
    [url]          VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [photo]        VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [size_x]       INT           DEFAULT ((0)) NOT NULL,
    [size_y]       INT           DEFAULT ((0)) NOT NULL,
    [position]     INT           DEFAULT ((1)) NOT NULL);

