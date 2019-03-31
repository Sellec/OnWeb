CREATE TYPE [dbo].[TVP_menus] AS TABLE (
    [id]   INT            DEFAULT ((0)) NOT NULL,
    [name] NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [code] NVARCHAR (MAX) DEFAULT ('') NOT NULL);

