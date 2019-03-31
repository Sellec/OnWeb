CREATE TYPE [dbo].[TVP_ItemType] AS TABLE (
    [IdItemType]   INT            DEFAULT ((0)) NOT NULL,
    [NameItemType] NVARCHAR (200) DEFAULT ('') NOT NULL,
    [UniqueKey]    NVARCHAR (200) DEFAULT (N'Имя типа данных .Net. Если не null, то используется для специфических типов в модулях (например, набор результатов поиска в модуле Goods будет зарегистрирован отдельно).') NOT NULL);

