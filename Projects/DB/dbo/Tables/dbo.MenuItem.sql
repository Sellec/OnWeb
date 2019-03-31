CREATE TABLE [dbo].[MenuItem] (
    [IdItem]         INT            IDENTITY (1, 1) NOT NULL,
    [IdItemParent]   INT            DEFAULT ((0)) NOT NULL,
    [IdMenu]         INT            DEFAULT ((0)) NOT NULL,
    [IdModule]       INT            DEFAULT ((0)) NOT NULL,
    [IdMaterial]     INT            DEFAULT ((0)) NOT NULL,
    [IdMaterialType] INT            DEFAULT ((0)) NOT NULL,
    [ItemName]       NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [ItemRule]       NVARCHAR (MAX) NOT NULL,
    [Order]          INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_menuitem_IdItem] PRIMARY KEY CLUSTERED ([IdItem] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.menuitem', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MenuItem';

