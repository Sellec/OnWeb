CREATE TABLE [dbo].[itemsextensionsdata] (
    [IdData]      INT            IDENTITY (1, 1) NOT NULL,
    [IdItem]      INT            DEFAULT ((0)) NOT NULL,
    [ValueStr1]   NVARCHAR (MAX) NOT NULL,
    [ValueStr2]   NVARCHAR (MAX) NOT NULL,
    [ValueInt1]   INT            DEFAULT ((0)) NOT NULL,
    [ValueFloat1] REAL           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_itemsextensionsdata_IdData] PRIMARY KEY CLUSTERED ([IdData] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IdList]
    ON [dbo].[itemsextensionsdata]([IdItem] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.itemsextensionsdata', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'itemsextensionsdata';

