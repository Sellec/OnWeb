





CREATE TRIGGER [DB_InsertOnDuplicate_TableUDT_Remove] ON DATABASE 
	FOR DROP_TABLE
AS 
	IF IS_MEMBER ('db_owner') = 0
	BEGIN
	   PRINT 'You must ask your DBA to drop or alter tables!' 
	   ROLLBACK TRANSACTION
	END

	DECLARE @TableName SYSNAME = EVENTDATA().value('(/EVENT_INSTANCE/ObjectName)[1]','SYSNAME');
	DECLARE @TypeName SYSNAME = N'TVP_' + @TableName;

	BEGIN TRY
		IF EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = @TypeName AND ss.name = N'dbo')
		BEGIN
			DECLARE @SQLDROP AS NVARCHAR(MAX) = N'DROP TYPE ' + @TypeName; 
			EXEC sp_executesql @SQLDROP
		END
	END TRY
	BEGIN CATCH
	END CATCH