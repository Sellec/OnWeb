-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION FROM_UNIXTIME
(
	@timestamp int
)
RETURNS datetime
AS
BEGIN
	RETURN dateadd(s, @timestamp, '1/1/1970')
END
