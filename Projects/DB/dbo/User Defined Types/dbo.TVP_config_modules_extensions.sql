CREATE TYPE [dbo].[TVP_config_modules_extensions] AS TABLE (
    [m_id]       INT      DEFAULT ((0)) NOT NULL,
    [ext_id]     INT      DEFAULT ((0)) NOT NULL,
    [use_admin]  SMALLINT DEFAULT ((0)) NOT NULL,
    [use_common] SMALLINT DEFAULT ((0)) NOT NULL);

