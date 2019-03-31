


CREATE TRIGGER [DB_InsertOnDuplicate_TableUDT] ON DATABASE 
	FOR ALTER_TABLE, CREATE_TABLE
AS 
	IF IS_MEMBER ('db_owner') = 0
	BEGIN
	   PRINT 'You must ask your DBA to drop or alter tables!' 
	   ROLLBACK TRANSACTION
	END
	
	DECLARE @TableName SYSNAME = EVENTDATA().value('(/EVENT_INSTANCE/ObjectName)[1]','SYSNAME');
	DECLARE @TypeName SYSNAME;

	BEGIN TRY
		EXEC	[dbo].[InsertOnDuplicate_CreateTableUDT]
			@TableName = @TableName,
			@TypeName = @TypeName OUTPUT
	END TRY
	BEGIN CATCH
	END CATCH

	BEGIN TRY
	   DELETE FROM InsertOnDuplicate_MergeFiltersCache

	   INSERT INTO InsertOnDuplicate_MergeFiltersCache (TableName, FilterStr)
	   SELECT d.TableName, d.FilterStr FROM dbo.InsertOnDuplicate_MergeFilters() d
	END TRY
	BEGIN CATCH
	END CATCH	


