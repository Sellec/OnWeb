CREATE TYPE [dbo].[TVP_config_extensions] AS TABLE (
    [id]       INT            DEFAULT ((0)) NOT NULL,
    [name]     NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [filename] NVARCHAR (200) DEFAULT (N'') NOT NULL);

