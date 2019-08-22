-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Maintenance_RebuildIndexes]
	@MinimumIndexFragmentstionToSearch int = 30
AS
BEGIN

	DECLARE @IndexesCursor CURSOR
	DECLARE @TableName nvarchar(200)
	DECLARE @IndexName nvarchar(200)
	DECLARE @Alter nvarchar(1000);


	SET @IndexesCursor = CURSOR FOR 
	SELECT DISTINCT dbschemas.name + '.[' + dbtables.name + ']' AS [Table], dbindexes.name AS [Index]
	FROM            sys.indexes AS dbindexes INNER JOIN
							 sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, NULL) AS indexstats ON dbindexes.object_id = indexstats.object_id AND 
							 indexstats.index_id = dbindexes.index_id INNER JOIN
							 sys.schemas AS dbschemas INNER JOIN
							 sys.tables AS dbtables ON dbschemas.schema_id = dbtables.schema_id ON dbindexes.object_id = dbtables.object_id INNER JOIN
							 sys.dm_db_partition_stats AS ddps ON dbindexes.object_id = ddps.object_id
	WHERE        (indexstats.database_id = DB_ID()) AND (dbindexes.index_id > 0) AND (indexstats.avg_fragmentation_in_percent >= @MinimumIndexFragmentstionToSearch)

	OPEN @IndexesCursor
	FETCH NEXT FROM @IndexesCursor INTO @TableName, @IndexName
	WHILE @@FETCH_STATUS = 0
	BEGIN

		SET @Alter = 'ALTER INDEX ' + @IndexName + ' ON ' + @TableName + ' REBUILD;';

		PRINT 'Rebuild index ''' + @IndexName + ''' on table ''' + @TableName + ''' with command ''' + @Alter + '''';

		EXEC(@Alter);

		FETCH NEXT FROM @IndexesCursor INTO @TableName, @IndexName;
	END
	CLOSE @IndexesCursor


END