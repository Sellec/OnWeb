-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertOnDuplicate_CreateQuery] 
	@TableName SYSNAME,
	@InsertData NVARCHAR(MAX),
	@UpdateData NVARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE	@TableName_ sysname = @TableName;
	DECLARE	@InsertData_ NVARCHAR(MAX) = @InsertData;
	DECLARE	@UpdateData_ NVARCHAR(MAX) = @UpdateData;

	DECLARE	@TypeName sysname = N'TVP_' + @TableName;

	----не убирать комментирование! ломается работа TransactionScope, если в нем есть вызов [InsertOnDuplicate_CreateQuery]
	--IF EXISTS(SELECT * FROM fn_my_permissions(NULL, 'DATABASE') t WHERE t.permission_name='CREATE TYPE')
	--BEGIN
	--   EXEC	[dbo].[InsertOnDuplicate_CreateTableUDT]
	--	  @TableName = @TableName_,
	--	  @TypeName = @TypeName OUTPUT
	--END
	
	--Генерация условия для проверки уникальности строки в MERGE
	DECLARE	@FilterStr nvarchar(max) = (SELECT t.FilterStr FROM dbo.InsertOnDuplicate_MergeFiltersCache t WHERE t.TableName=@TableName_)
	
	--Генерация INSERT полей.
	DECLARE @Fields TABLE (TableName SYSNAME, ColumnStr SYSNAME, IsIdentity bit, DefaultValue NVARCHAR(100))
	INSERT INTO @Fields (TableName, ColumnStr, IsIdentity, DefaultValue)
	SELECT t.name, col.name, col.is_identity, ISNULL('''' + def.Value + '''', 'NULL')
	FROM sys.tables t 
	INNER JOIN sys.columns col ON t.object_id = col.object_id
	INNER JOIN sys.types AS s ON col.system_type_id = s.system_type_id AND col.user_type_id = s.user_type_id
	LEFT JOIN dbo.InsertOnDuplicate_TypesDefaultValues() AS def ON def.TypeName = s.name
	
	DECLARE @FieldsInsertWithIdentity nvarchar(max) = (Select '(' + Left(Main.FilterStr, Len(Main.FilterStr)-1) + ')'
							From (
								Select distinct ST2.TableName, (
									Select '[' + ST1.ColumnStr + '], ' AS [text()]
									From @Fields ST1
									Where ST1.TableName = ST2.TableName 
									ORDER BY ST1.TableName
									For XML PATH ('')
								) FilterStr
							From @Fields ST2) [Main]     
						WHERE Main.TableName=@TableName_)
	DECLARE @FieldsInsertWithoutIdentity nvarchar(max) = (Select '(' + Left(Main.FilterStr, Len(Main.FilterStr)-1) + ')'
							From (
								Select distinct ST2.TableName, (
									Select '[' + ST1.ColumnStr + '], ' AS [text()]
									From @Fields ST1
									Where ST1.TableName = ST2.TableName AND ST1.IsIdentity = 0
									ORDER BY ST1.TableName
									For XML PATH ('')
								) FilterStr
							From @Fields ST2) [Main]     
						WHERE Main.TableName=@TableName_)
	DECLARE @FieldsUpdateWithIdentity nvarchar(max) = (Select '(' + Left(Main.FilterStr, Len(Main.FilterStr)-1) + ')'
							From (
								Select distinct ST2.TableName, (
									Select 'S.[' + ST1.ColumnStr + '], ' AS [text()]
									From @Fields ST1
									Where ST1.TableName = ST2.TableName 
									ORDER BY ST1.TableName
									For XML PATH ('')
								) FilterStr
							From @Fields ST2) [Main]     
						WHERE Main.TableName=@TableName_)
	DECLARE @FieldsUpdateWithoutIdentity nvarchar(max) = (Select '(' + Left(Main.FilterStr, Len(Main.FilterStr)-1) + ')'
							From (
								Select distinct ST2.TableName, (
									Select 'S.[' + ST1.ColumnStr + '], ' AS [text()]
									From @Fields ST1
									Where ST1.TableName = ST2.TableName AND ST1.IsIdentity = 0 
									ORDER BY ST1.TableName
									For XML PATH ('')
								) FilterStr
							From @Fields ST2) [Main]     
						WHERE Main.TableName=@TableName_)
						
	--Находим IDENTITY поле, если оно есть.
	DECLARE @FieldIdentity SYSNAME = NULL;
	DECLARE @FieldIdentityDefaultValue NVARCHAR(100) = NULL;
	SELECT @FieldIdentity = ColumnStr, @FieldIdentityDefaultValue = DefaultValue FROM @Fields WHERE IsIdentity <> 0 AND TableName=@TableName_;
	
	--Непосредственно запрос.	
	DECLARE @SQL NVARCHAR(MAX) = N'' + CHAR(13) + CHAR(10) + 
									'DECLARE @CountRows int = 0;' + CHAR(13) + CHAR(10) + 
									'DECLARE @ErrorMessage NVARCHAR(4000);' + CHAR(13) + CHAR(10) + 
									'DECLARE @ErrorSeverity INT;' + CHAR(13) + CHAR(10) + 
									'DECLARE @ErrorState INT;' + CHAR(13) + CHAR(10) + 
									'DECLARE @t ' + @TypeName + ';' + CHAR(13) + CHAR(10) + 
									'SET NOCOUNT ON;'  + CHAR(13) + CHAR(10) +
									@InsertData_ + CHAR(13) + CHAR(10) + 
									'SET NOCOUNT OFF;'  + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
									
									CASE WHEN @FieldIdentity IS NULL THEN '' ELSE 
										'SET IDENTITY_INSERT [' + @TableName_ + '] ON;' + CHAR(13) + CHAR(10) +
										'BEGIN TRY' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
										
										'	MERGE [' + @TableName_ + '] AS T' + CHAR(13) + CHAR(10) +
										'	USING (SELECT DISTINCT * FROM @t WHERE NOT [' + @FieldIdentity + '] = (' + @FieldIdentityDefaultValue + ')) AS S' + CHAR(13) + CHAR(10) +
										'		ON (' + @FilterStr + ')' + CHAR(13) + CHAR(10) +
										'	WHEN NOT MATCHED BY TARGET THEN INSERT ' + ISNULL(@FieldsInsertWithIdentity, '') + ' VALUES ' + ISNULL(@FieldsUpdateWithIdentity, '') + CHAR(13) + CHAR(10) + 
										'	WHEN MATCHED THEN UPDATE SET ' + @UpdateData_ + ';' + CHAR(13) + CHAR(10) +
										'	SET @CountRows = @CountRows + @@ROWCOUNT;' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
										
										'END TRY' + CHAR(13) + CHAR(10) +
										'BEGIN CATCH' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
										
										'	SET @ErrorMessage = ERROR_MESSAGE(); ' + CHAR(13) + CHAR(10) +
										'	SET @ErrorSeverity = ERROR_SEVERITY();' + CHAR(13) + CHAR(10) +
										'	SET @ErrorState = ERROR_STATE();' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
										
										'	SET IDENTITY_INSERT [' + @TableName_ + '] OFF;' + CHAR(13) + CHAR(10) +
										'	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
										
										'END CATCH' + CHAR(13) + CHAR(10) + 
										'SET IDENTITY_INSERT [' + @TableName_ + '] OFF;' + CHAR(13) + CHAR(10)	+ CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) 
									END +
									
									--Теперь добавляем блок для вставки данных БЕЗ identity столбца.
									'BEGIN TRY' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
									
									'	MERGE [' + @TableName_ + '] AS T' + CHAR(13) + CHAR(10) +
									'	USING (SELECT DISTINCT * FROM @t ' + CASE WHEN @FieldIdentity IS NULL THEN '' ELSE ' WHERE [' + @FieldIdentity + '] = (' + @FieldIdentityDefaultValue + ')' END + ') AS S' + CHAR(13) + CHAR(10) +
									'		ON (' + @FilterStr + ')' + CHAR(13) + CHAR(10) +
									'	WHEN NOT MATCHED BY TARGET THEN INSERT ' + ISNULL(@FieldsInsertWithoutIdentity, '') + ' VALUES ' + ISNULL(@FieldsUpdateWithoutIdentity, '') + CHAR(13) + CHAR(10) + 
									'	WHEN MATCHED THEN UPDATE SET ' + @UpdateData_ + ';' + CHAR(13) + CHAR(10) + 
									'	SET @CountRows = @CountRows + @@ROWCOUNT;' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
									
									'END TRY' + CHAR(13) + CHAR(10) +
									'BEGIN CATCH' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
									
									'	SET @ErrorMessage = ERROR_MESSAGE(); ' + CHAR(13) + CHAR(10) +
									'	SET @ErrorSeverity = ERROR_SEVERITY();' + CHAR(13) + CHAR(10) +
									'	SET @ErrorState = ERROR_STATE();' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
									
									'	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
									
									'END CATCH' + CHAR(13) + CHAR(10) 
									;
				
	PRINT @SQL;					
	SELECT @SQL AS Query;	
	
END
