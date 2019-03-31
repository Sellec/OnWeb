CREATE TYPE [dbo].[TVP_config_modules] AS TABLE (
    [m_id]       INT            DEFAULT ((0)) NOT NULL,
    [m_name]     NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [m_filename] NVARCHAR (200) DEFAULT (N'') NOT NULL);

