-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[InsertOnDuplicate_MergeFilters]()
RETURNS @Result TABLE (TableName SYSNAME, FilterStr nvarchar(MAX))
AS
BEGIN
	/**
	 * Вот здесь вместо ебучего MS, не поддерживающего ANSI-92, создаем условие, которое будет срабатывать в MERGE IF, чтобы определить строки, которые уже существуют в базе.
	 * */

	DECLARE @Fields TABLE (TableName SYSNAME, IndexName SYSNAME, ColumnStr nvarchar(MAX))
	DECLARE @Fields2 TABLE (TableName SYSNAME, FilterStr nvarchar(MAX))

	INSERT INTO @Fields (TableName, IndexName, ColumnStr)
	SELECT t.name, ind.name, '(T.[' + col.name + '] = S.[' + col.name + ']' + CASE WHEN col.is_nullable <> 0 THEN ' AND S.[' + col.name + '] IS NOT NULL' ELSE '' END + ')'
	FROM sys.indexes ind 
	INNER JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
	INNER JOIN sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
	INNER JOIN sys.tables t ON ind.object_id = t.object_id 
	WHERE (ind.is_primary_key <> 0 OR ind.is_unique = 1 OR ind.is_unique_constraint = 1)-- AND t.name = 'File'
     
	INSERT INTO @Fields2 (TableName, FilterStr)
	Select Main.TableName,
		   '(' + Left(Main.FilterStr, Len(Main.FilterStr)-4) + ')' AS Filter
	From
		(
			Select distinct ST2.TableName, ST2.IndexName, 
				(
					Select ST1.ColumnStr + ' AND ' AS [text()]
					From @Fields ST1
					Where ST1.TableName = ST2.TableName AND ST1.IndexName = ST2.IndexName
					ORDER BY ST1.TableName, ST1.IndexName
					For XML PATH ('')
				) FilterStr
			From @Fields ST2
		) [Main]     
    
	INSERT INTO @Result (TableName, FilterStr)
	Select Main.TableName,
		   '(' + Left(Main.FilterStr, Len(Main.FilterStr)-3) + ')' AS Filter
	From
		(
			Select distinct ST2.TableName, 
				(
					Select ST1.FilterStr + ' OR ' AS [text()]
					From @Fields2 ST1
					Where ST1.TableName = ST2.TableName 
					ORDER BY ST1.TableName
					For XML PATH ('')
				) FilterStr
			From @Fields ST2
		) [Main]   	
	
	RETURN 
END
