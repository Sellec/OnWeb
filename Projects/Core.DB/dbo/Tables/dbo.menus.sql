CREATE TABLE [dbo].[menus] (
    [id]   INT            IDENTITY (8, 1) NOT NULL,
    [name] NVARCHAR (MAX) NOT NULL,
    [code] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_menus_id] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.menus', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'menus';

