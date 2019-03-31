-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE RemoveHangfire
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
    SET NOCOUNT ON;

    DROP TABLE [HangFire].[AggregatedCounter];
    DROP TABLE [HangFire].[Counter];
    DROP TABLE [HangFire].[Hash];
    DROP TABLE [HangFire].[JobQueue];
    DROP TABLE [HangFire].[List];
    DROP TABLE [HangFire].[Schema];
    DROP TABLE [HangFire].[Server];
    DROP TABLE [HangFire].[Set];
    DROP TABLE [HangFire].[State];
    DROP TABLE [HangFire].[Job];
    DROP TABLE [HangFire].[JobParameter];

END