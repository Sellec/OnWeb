CREATE TABLE [dbo].[config] (
    [IdConfig]     INT            IDENTITY (1, 1) NOT NULL,
    [name]         NVARCHAR (100) CONSTRAINT [DF__config__name__7A521F79] DEFAULT (NULL) NULL,
    [serialized]   NVARCHAR (MAX) NOT NULL,
    [DateChange]   INT            CONSTRAINT [DF__config__DateChan__7B4643B2] DEFAULT ((0)) NOT NULL,
    [IdUserChange] INT            CONSTRAINT [DF__config__IdUserCh__7C3A67EB] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_config] PRIMARY KEY CLUSTERED ([IdConfig] ASC),
    CONSTRAINT [config$name] UNIQUE NONCLUSTERED ([name] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.config', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'config';

