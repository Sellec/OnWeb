CREATE TABLE [dbo].[UrlTranslation] (
    [IdTranslation]     INT             IDENTITY (1, 1) NOT NULL,
    [IdTranslationType] INT             CONSTRAINT [DF_UrlTranslation_IdTranslationType] DEFAULT ((0)) NOT NULL,
    [IdModule]          INT             CONSTRAINT [DF__urltransl__IdMod__6A50C1DA] DEFAULT ((0)) NOT NULL,
    [IdItem]            INT             CONSTRAINT [DF__urltransl__IdIte__6B44E613] DEFAULT ((0)) NOT NULL,
    [IdItemType]        INT             CONSTRAINT [DF__urltransl__IdIte__6C390A4C] DEFAULT ((0)) NOT NULL,
    [Action]            NVARCHAR (200)  CONSTRAINT [DF__urltransl__Actio__6D2D2E85] DEFAULT (N'') NOT NULL,
    [Arguments]         NVARCHAR (4000) NULL,
    [UrlFull]           NVARCHAR (500)  CONSTRAINT [DF__urltransl__UrlFu__6E2152BE] DEFAULT (N'') NOT NULL,
    [DateChange]        INT             CONSTRAINT [DF__urltransl__DateC__6F1576F7] DEFAULT ((0)) NOT NULL,
    [IdUserChange]      INT             CONSTRAINT [DF__urltransl__IdUse__70099B30] DEFAULT ((0)) NOT NULL,
    [IsFixedLength]     BIT             CONSTRAINT [DF_UrlTranslation_IsFixedLength] DEFAULT ((0)) NOT NULL,
    [UniqueKey]         NVARCHAR (200)  NULL,
    CONSTRAINT [PK_urltranslation_IdTranslation] PRIMARY KEY CLUSTERED ([IdTranslation] ASC)
);








GO
CREATE NONCLUSTERED INDEX [urltranslation$IdModule_2]
    ON [dbo].[UrlTranslation]([IdModule] ASC, [IdItem] ASC, [IdItemType] ASC, [Action] ASC, [IdTranslationType] ASC);


GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.urltranslation', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UrlTranslation';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Название Action из контроллера модуля', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UrlTranslation', @level2type = N'COLUMN', @level2name = N'Action';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Аргументы для Action, сериализованные в json', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UrlTranslation', @level2type = N'COLUMN', @level2name = N'Arguments';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Адрес в запросе к сайту, для которого определяется данная строка таблицы адресов.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UrlTranslation', @level2type = N'COLUMN', @level2name = N'UrlFull';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Адрес фиксированной длины означает, что при наличии в базе адреса "/коляски/коляска№123" и переходе пользователя по адресу "/коляски/коляска№123/123/46" он будет принудительно перенаправлен на "/коляски/коляска№123"', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UrlTranslation', @level2type = N'COLUMN', @level2name = N'IsFixedLength';


GO
CREATE UNIQUE NONCLUSTERED INDEX [urltranslationUniqueKey]
    ON [dbo].[UrlTranslation]([UniqueKey] ASC, [IdModule] ASC, [IdItem] ASC, [IdItemType] ASC, [Action] ASC) WHERE ([UniqueKey] IS NOT NULL AND [IdTranslationType]<>(2));


GO



GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex_IdTranslationType]
    ON [dbo].[UrlTranslation]([IdTranslationType] ASC)
    INCLUDE([IdTranslation], [UrlFull], [UniqueKey]);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex_20180302_114957]
    ON [dbo].[UrlTranslation]([IdModule] ASC, [IdItem] ASC, [IdItemType] ASC, [UniqueKey] ASC)
    INCLUDE([UrlFull]);


GO
CREATE NONCLUSTERED INDEX [HistoryIndex_ОтключенВеситМного]
    ON [dbo].[UrlTranslation]([IdTranslationType] ASC, [IdModule] ASC, [IdItem] DESC, [IdItemType] ASC, [Action] ASC, [Arguments] ASC, [UrlFull] ASC, [IsFixedLength] ASC)
    INCLUDE([IdTranslation]);


GO
ALTER INDEX [HistoryIndex_ОтключенВеситМного]
    ON [dbo].[UrlTranslation] DISABLE;


GO
-- =============================================
-- Триггер разбит на 2 части.
--
-- 1. [UrlTranslationSaveOldAddresses]
-- При добавлении адресов любого типа, кроме 2 (2 - история адресов) дублирует эти адреса с типом 2, сохраняя историю адресов для конкретного объекта. 
-- При этом [UniqueKey] для записей с типом 2 отбрасывается.
--
-- 2. [UrlTranslationKeepMainAddressesUnique]
-- При добавлении адресов типа 1 (1 - основной адрес сущности) ищет адреса с аналогичным [UrlFull] для других сущностей и удаляет их. 
-- Т.о. один [UrlFull] с типом 1 может соответствовать только одной сущности. При этом другие сущности теряют свой основной адрес, но, т.к. адрес дублируется с типом 2 (см. триггер [UrlTranslationSaveOldAddresses]), то в случае коллизий по прежнему можно обратиться к сущности по указанному адресу.
-- =============================================
CREATE TRIGGER [dbo].[UrlTranslation_SaveOldAddresses_AND_KeepMainAddressesUnique]
   ON  [dbo].[UrlTranslation]
   AFTER INSERT, UPDATE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- 1. [UrlTranslationSaveOldAddresses]
    SET IDENTITY_INSERT dbo.[UrlTranslation] off;

    INSERT INTO UrlTranslation (
	   [IdTranslationType],
	   [IdModule],
	   [IdItem],
	   [IdItemType],
	   [Action],
	   [Arguments],
	   [UrlFull],
	   [DateChange],
	   [IdUserChange],
	   [IsFixedLength],
	   [UniqueKey])
    SELECT 
	   2, 
	   i.[IdModule],
	   i.[IdItem],
	   i.[IdItemType],
	   i.[Action],
	   i.[Arguments],
	   i.[UrlFull],
	   i.[DateChange],
	   i.[IdUserChange],
	   i.[IsFixedLength],
	   NULL
    FROM inserted i
    LEFT JOIN dbo.[UrlTranslation] u ON 2 = u.[IdTranslationType] AND 
						  i.[IdModule] = u.[IdModule] AND 	
						  i.[IdItem] = u.[IdItem] AND 
						  i.[IdItemType] = u.[IdItemType] AND 
						  i.[Action] = u.[Action] AND 
						  i.[Arguments] = u.[Arguments] AND 
						  i.[UrlFull] = u.[UrlFull] AND 
						  i.[IsFixedLength] = u.[IsFixedLength]
    WHERE u.IdTranslation IS NULL

    -- 2. [UrlTranslationKeepMainAddressesUnique]
    DECLARE @Result VARCHAR(MAX);

    --SELECT 
    --@Result =	CASE
				--WHEN @Result IS NULL
				--THEN ''
				--ELSE @Result + ';' + CHAR(13) + CHAR(10)
			 --END + 'remove ' + convert(nvarchar(10), u.IdTranslation) + ' with ''' + u.[UrlFull] + ''' becouse i.[IdTranslation](' + convert(nvarchar(10), i.IdTranslation) + ') <> u.[IdTranslation](' + convert(nvarchar(10), u.IdTranslation) + ')'
    --FROM [UrlTranslation] u
    --INNER JOIN inserted i ON i.[UrlFull] = u.[UrlFull] AND i.[IdTranslation] <> u.[IdTranslation]
    --WHERE 
	   --i.[IdTranslationType] = 1 AND 
	   --u.[IdTranslationType] = 1 AND 
	   --u.[IdTranslation] NOT IN (
		  --SELECT MAX(i.[IdTranslation]) AS IdTranslationMax
		  --FROM [UrlTranslation] u
		  --INNER JOIN inserted i ON i.[UrlFull] = u.[UrlFull] AND i.[IdTranslation] <> u.[IdTranslation]
		  --WHERE i.[IdTranslationType] = 1 AND u.[IdTranslationType] = 1
    --)

    --print @Result

	--это убрал. пусь будут одинаковые urlfull у разных итемов, система всё равно будет брать последний зарегистрированный.
    --DELETE FROM [UrlTranslation]
    --FROM [UrlTranslation] u
    --INNER JOIN inserted i ON i.[UrlFull] = u.[UrlFull] AND i.[IdTranslation] <> u.[IdTranslation]
    --WHERE 
	   --i.[IdTranslationType] = 1 AND 
	   --u.[IdTranslationType] = 1 AND 
	   --u.[IdTranslation] NOT IN (
		  --SELECT MAX(i.[IdTranslation]) AS IdTranslationMax
		  --FROM [UrlTranslation] u
		  --INNER JOIN inserted i ON i.[UrlFull] = u.[UrlFull] AND i.[IdTranslation] <> u.[IdTranslation]
		  --WHERE i.[IdTranslationType] = 1 AND u.[IdTranslationType] = 1
    --)
	   
END
GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex_RoutingByUrlFull]
    ON [dbo].[UrlTranslation]([UrlFull] ASC, [IsFixedLength] ASC)
    INCLUDE([IdTranslation], [IdTranslationType], [DateChange]);

