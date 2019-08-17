-- =============================================
-- Author:		Sellec
-- Description:	Файлы с истекшим сроком жизни помечаются на удаление. Все файлы, помеченные на удаление (по сроку жизни или вручную) помещаются в очередь на удаление.
-- =============================================
CREATE PROCEDURE [dbo].[FileManager_PlaceFileIntoQueue]
AS
BEGIN
	SET NOCOUNT ON;

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    --Файлы с истекшим сроком годности помечаются на удаление.
	UPDATE [File]
	SET IsRemoving = 1
	WHERE IsRemoved = 0 AND IsRemoving = 0 AND DateExpire IS NOT NULL AND DateExpire <= GETDATE()

	-- Файлы, помеченные на удаление, помещаются в очередь на удаление.
	INSERT INTO [FileRemoveQueue] (IdFile)
	SELECT f.IdFile 
	FROM [File] f
	LEFT JOIN [FileRemoveQueue] q ON f.IdFile = q.IdFile
	WHERE f.IsRemoved = 0 AND f.IsRemoving <> 0 AND q.IdFile IS NULL;

	PRINT 'В очередь на удаление помещено ' + convert(nvarchar(10), @@ROWCOUNT) + ' файлов.'

END