CREATE TABLE [dbo].[search_engines] (
    [engine_id]     INT      IDENTITY (1, 1) NOT NULL,
    [engine_mid]    INT      DEFAULT ((0)) NOT NULL,
    [engine_status] SMALLINT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_search_engines_engine_id] PRIMARY KEY CLUSTERED ([engine_id] ASC),
    CONSTRAINT [search_engines$engine_mid] UNIQUE NONCLUSTERED ([engine_mid] ASC),
    CONSTRAINT [search_engines$engine_mid_2] UNIQUE NONCLUSTERED ([engine_mid] ASC, [engine_status] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.search_engines', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'search_engines';

