-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[InsertOnDuplicate_TypesDefaultValues]()
RETURNS @TypesDefaultValues TABLE (TypeName SYSNAME, Value NVARCHAR(100))
AS
BEGIN
	-- Fill the table variable with the rows for your result set
	INSERT INTO @TypesDefaultValues (TypeName, Value) VALUES
	('bit', '0'), ('tinyint', '0'), ('smallint', '0'), ('int', '0'), ('bigint', '0'), ('numeric', '0'), ('decimal', '0'), ('smallmoney', '0'), ('money', '0'),
	('float', '0'), ('real', '0'),
	('datetime', 'GETDATE()'), ('smalldatetime', 'GETDATE()'), ('date', 'GETDATE()'), ('time', 'GETDATE()'), ('datetimeoffset', '0'), ('datetime2', 'GETDATE()'),
	('char', ''''''), ('varchar', ''''''), ('text', ''''''), ('nchar', ''''''), ('nvarchar', ''''''), ('ntext', ''''''),
	('binary', ''), ('varbinary', ''), ('image', '')
	
	RETURN 
END
