-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertOnDuplicate_CreateTableUDT]
	@TableName SYSNAME,
	@TypeName SYSNAME OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @TypeName = N'TVP_' + @TableName;

	DECLARE @sql NVARCHAR(MAX) = N'';
	
	SELECT @sql = @sql + N',' + CHAR(13) + CHAR(10) + CHAR(9) 
		+ QUOTENAME(c.name) + ' '
		+ s.name + 
		CASE WHEN LOWER(s.name) LIKE '%char' THEN 
			'(' + CASE WHEN c.max_length = -1 THEN 'MAX' ELSE
				CONVERT(VARCHAR(12), 
											c.max_length/(CASE LOWER(LEFT(s.name, 1)) WHEN N'n' THEN 2 ELSE 1 END)
										) 
				END +
			+ ')' 
			ELSE
				CASE WHEN s.name = 'decimal' OR s.name = 'numeric' THEN 
					CASE WHEN c.[precision] > 0 AND c.[scale] > 0 THEN '(' + convert(nvarchar(10), c.[precision]) + ', ' + convert(nvarchar(10), c.[scale]) + ')'
						 WHEN c.[precision] > 0 THEN '(' + convert(nvarchar(10), c.[precision]) + ')'
						 ELSE '' 
					END
				ELSE '' END
			END + 
			' ' + CASE WHEN c.is_nullable = 1 THEN 'NULL' ELSE 'NOT NULL' END +
			' ' + CASE WHEN c.default_object_id = 0 THEN 
					CASE WHEN c.is_nullable = 1 THEN '' ELSE 'DEFAULT (' + ISNULL((SELECT Value FROM dbo.InsertOnDuplicate_TypesDefaultValues() WHERE TypeName = s.name), 'NULL')  + ')' END
				  ELSE 
					'DEFAULT ' + object_definition(c.default_object_id) 
				  END
			-- need much more conditionals here for other data types
		FROM sys.columns AS c
		INNER JOIN sys.types AS s
		ON c.system_type_id = s.system_type_id
		AND c.user_type_id = s.user_type_id
		WHERE c.[object_id] = OBJECT_ID(@TableName);
	    
	IF EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = @TypeName AND ss.name = N'dbo')
	BEGIN
		DECLARE @SQLDROP AS NVARCHAR(MAX) = N'DROP TYPE ' + @TypeName; 
		EXEC sp_executesql @SQLDROP
	END

	SET @sql = N'CREATE TYPE ' + @TypeName
		+ ' AS TABLE ' + CHAR(13) + CHAR(10) + '(' + STUFF(@sql, 1, 1, '')
		+ CHAR(13) + CHAR(10) + ');';

	PRINT @SQL;
	EXEC sp_executesql @sql;
END
