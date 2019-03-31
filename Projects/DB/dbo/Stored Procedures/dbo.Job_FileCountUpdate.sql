-- =============================================
-- Обновление количества ссылок для таблицы 
-- =============================================
CREATE PROCEDURE [dbo].[Job_FileCountUpdate]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @sql_tables nvarchar(max) =	null;

	SELECT @sql_tables = CASE WHEN @sql_tables IS NULL THEN '' ELSE @sql_tables + CHAR(13) + CHAR(10) + '		UNION ALL' + CHAR(13) + CHAR(10) END + '		SELECT ' + c.name + ' AS IdFile, count(*) AS FileCount FROM ' + t.name + ' GROUP BY ' + c.name
	FROM sys.foreign_key_columns AS fk
	INNER JOIN sys.tables AS t ON fk.parent_object_id = t.object_id
	INNER JOIN sys.columns AS c ON fk.parent_object_id = c.object_id AND fk.parent_column_id = c.column_id
	INNER JOIN sys.columns AS c2 ON fk.referenced_object_id = c2.object_id AND fk.referenced_column_id = c2.column_id
	WHERE fk.referenced_object_id = (SELECT object_id FROM sys.tables WHERE name = 'File') AND c2.name = 'IdFile';

	IF @sql_tables IS NOT NULL
	BEGIN
		DECLARE @sql nvarchar(max) =	'UPDATE dbo.[File]' + CHAR(13) + CHAR(10) + 
										'SET CountUsage = t2.FileCount' + CHAR(13) + CHAR(10) + 
										'FROM dbo.[File]' + CHAR(13) + CHAR(10) + 
										'INNER JOIN (' + CHAR(13) + CHAR(10) +
										'	SELECT IdFile, SUM(FileCount) AS FileCount ' + CHAR(13) + CHAR(10) + 
										'	FROM (' + CHAR(13) + CHAR(10) + 
											@sql_tables + CHAR(13) + CHAR(10) + 
											
											--Ниже - хак для CustomFields, т.к. нельзя создавать Foreign Key с условием.
										'		UNION ALL ' + CHAR(13) + CHAR(10) +
										'		SELECT IdFieldValue AS IdFile, count(*) AS FileCount ' + CHAR(13) + CHAR(10) +
										'		FROM CustomFieldsData ' + CHAR(13) + CHAR(10) +
										'		INNER JOIN CustomFieldsField ON CustomFieldsField.field_id = CustomFieldsData.IdField ' + CHAR(13) + CHAR(10) +
										'		WHERE CustomFieldsField.field_type in (10, 12) ' + CHAR(13) + CHAR(10) +
										'		GROUP BY IdFieldValue ' + CHAR(13) + CHAR(10) +
										'	) t' + CHAR(13) + CHAR(10) +
										'	GROUP BY IdFile' + CHAR(13) + CHAR(10) +
										') t2 ON t2.IdFile = dbo.[File].IdFile';

		print @sql;

		EXEC sp_executesql @sql;
	END;


END
