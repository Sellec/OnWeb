CREATE TABLE [dbo].[ItemParent] (
    [IdModule]     INT CONSTRAINT [DF__itemparen__IdMod__430CD787] DEFAULT ((0)) NOT NULL,
    [IdItem]       INT CONSTRAINT [DF__itemparen__IdIte__4400FBC0] DEFAULT ((0)) NOT NULL,
    [IdItemType]   INT CONSTRAINT [DF__itemparen__IdIte__44F51FF9] DEFAULT ((0)) NOT NULL,
    [IdParentItem] INT CONSTRAINT [DF__itemparen__IdPar__45E94432] DEFAULT ((0)) NOT NULL,
    [IdLevel]      INT CONSTRAINT [DF__itemparen__IdLev__46DD686B] DEFAULT ((0)) NOT NULL
);




GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.itemparent', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ItemParent';


GO
CREATE CLUSTERED INDEX [ItemParent_IdModule]
    ON [dbo].[ItemParent]([IdModule] ASC, [IdItem] ASC, [IdItemType] ASC, [IdParentItem] ASC);

