CREATE TABLE [dbo].[RoleUser] (
    [IdRole]       INT CONSTRAINT [DF__roleuser__IdRole__6F6A7CB2] DEFAULT ((0)) NOT NULL,
    [IdUser]       INT CONSTRAINT [DF__roleuser__IdUser__705EA0EB] DEFAULT ((0)) NOT NULL,
    [IdUserChange] INT CONSTRAINT [DF__roleuser__IdUser__7152C524] DEFAULT ((0)) NOT NULL,
    [DateChange]   INT CONSTRAINT [DF__roleuser__DateCh__7246E95D] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [roleuser$IdRole] UNIQUE CLUSTERED ([IdRole] ASC, [IdUser] ASC)
);






GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.roleuser', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RoleUser';


GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER RoleUser_Upsert
   ON  RoleUser
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
    INSERT INTO RoleUserHistory (IdRole, IdUser, IdUserChange, DateChange, [Action])
    SELECT IdRole, IdUser, IdUserChange, GETDATE(), 'INSERT/UPDATE'
    FROM inserted

END
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[RoleUser_Delete]
   ON  [dbo].[RoleUser]
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
    INSERT INTO RoleUserHistory (IdRole, IdUser, IdUserChange, DateChange, [Action])
    SELECT IdRole, IdUser, IdUserChange, GETDATE(), 'DELETE'
    FROM deleted

END