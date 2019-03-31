CREATE TABLE [dbo].[config_modules] (
    [m_id]       INT            IDENTITY (1, 1) NOT NULL,
    [m_name]     NVARCHAR (200) CONSTRAINT [DF__config_mo__m_nam__02E7657A] DEFAULT (N'') NOT NULL,
    [m_filename] NVARCHAR (200) CONSTRAINT [DF__config_mo__m_fil__03DB89B3] DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_config_modules] PRIMARY KEY CLUSTERED ([m_id] ASC),
    CONSTRAINT [config_modules$filename] UNIQUE NONCLUSTERED ([m_filename] ASC)
);


GO
CREATE NONCLUSTERED INDEX [name]
    ON [dbo].[config_modules]([m_name] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.config_modules', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'config_modules';

