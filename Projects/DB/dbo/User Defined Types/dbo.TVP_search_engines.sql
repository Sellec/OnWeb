CREATE TYPE [dbo].[TVP_search_engines] AS TABLE (
    [engine_id]     INT      DEFAULT ((0)) NOT NULL,
    [engine_mid]    INT      DEFAULT ((0)) NOT NULL,
    [engine_status] SMALLINT DEFAULT ((0)) NOT NULL);

