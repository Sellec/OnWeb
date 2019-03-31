CREATE TABLE [dbo].[ItemType] (
    [IdItemType]   INT            IDENTITY (1, 1) NOT NULL,
    [NameItemType] NVARCHAR (200) NOT NULL,
    [UniqueKey]    NVARCHAR (200) CONSTRAINT [DF_ItemType_FullNameItem] DEFAULT (N'Имя типа данных .Net. Если не null, то используется для специфических типов в модулях (например, набор результатов поиска в модуле Goods будет зарегистрирован отдельно).') NOT NULL,
    CONSTRAINT [PK_ItemType] PRIMARY KEY CLUSTERED ([IdItemType] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [ItemType_UniqueKey]
    ON [dbo].[ItemType]([UniqueKey] ASC);

