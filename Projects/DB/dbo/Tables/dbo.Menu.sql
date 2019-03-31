CREATE TABLE [dbo].[Menu] (
    [IdMenu]       INT            IDENTITY (1, 1) NOT NULL,
    [NameMenu]     NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [IdUserChange] INT            DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_menu_IdMenu] PRIMARY KEY CLUSTERED ([IdMenu] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.menu', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Menu';

