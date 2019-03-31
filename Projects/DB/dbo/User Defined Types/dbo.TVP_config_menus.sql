CREATE TYPE [dbo].[TVP_config_menus] AS TABLE (
    [id]         BIGINT         DEFAULT ((0)) NOT NULL,
    [name]       NVARCHAR (100) DEFAULT ('') NOT NULL,
    [code]       NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [DateChange] BIGINT         DEFAULT ((0)) NOT NULL);

