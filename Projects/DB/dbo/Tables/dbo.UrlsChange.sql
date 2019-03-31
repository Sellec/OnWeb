CREATE TABLE [dbo].[UrlsChange] (
    [uc_id]          INT           IDENTITY (2, 1) NOT NULL,
    [uc_module]      VARCHAR (200) DEFAULT (N'') NOT NULL,
    [uc_expression]  VARCHAR (MAX) NOT NULL,
    [uc_replacement] VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_urlschange_uc_id] PRIMARY KEY CLUSTERED ([uc_id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.urlschange', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UrlsChange';

