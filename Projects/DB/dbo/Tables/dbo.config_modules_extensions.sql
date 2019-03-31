CREATE TABLE [dbo].[config_modules_extensions] (
    [m_id]       INT      DEFAULT ((0)) NOT NULL,
    [ext_id]     INT      DEFAULT ((0)) NOT NULL,
    [use_admin]  SMALLINT DEFAULT ((0)) NOT NULL,
    [use_common] SMALLINT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_config_modules_extensions_ext_id] PRIMARY KEY CLUSTERED ([ext_id] ASC, [m_id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [m_id]
    ON [dbo].[config_modules_extensions]([m_id] ASC, [use_admin] ASC);


GO
CREATE NONCLUSTERED INDEX [m_id_2]
    ON [dbo].[config_modules_extensions]([m_id] ASC, [use_common] ASC);


GO
CREATE NONCLUSTERED INDEX [use_admin]
    ON [dbo].[config_modules_extensions]([m_id] ASC, [ext_id] ASC, [use_admin] ASC);


GO
CREATE NONCLUSTERED INDEX [use_common]
    ON [dbo].[config_modules_extensions]([m_id] ASC, [ext_id] ASC, [use_common] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.config_modules_extensions', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'config_modules_extensions';

