CREATE TABLE [dbo].[Theme] (
    [IdTheme]      INT            IDENTITY (2, 1) NOT NULL,
    [NameTheme]    NVARCHAR (200) CONSTRAINT [DF__theme__NameTheme__6497E884] DEFAULT (N'') NOT NULL,
    [FolderName]   NVARCHAR (200) CONSTRAINT [DF__theme__FolderNam__658C0CBD] DEFAULT (N'') NOT NULL,
    [Active]       SMALLINT       CONSTRAINT [DF__theme__Active__668030F6] DEFAULT ((0)) NOT NULL,
    [IdUserChange] INT            CONSTRAINT [DF__theme__IdUserCha__6774552F] DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            CONSTRAINT [DF__theme__DateChang__68687968] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_theme_IdTheme] PRIMARY KEY CLUSTERED ([IdTheme] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.theme', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Theme';

