CREATE TABLE [dbo].[config_extensions] (
    [id]       INT            IDENTITY (7, 1) NOT NULL,
    [name]     NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [filename] NVARCHAR (200) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_config_extensions_id] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.config_extensions', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'config_extensions';

