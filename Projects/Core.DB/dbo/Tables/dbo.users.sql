CREATE TABLE [dbo].[users] (
    [id]                    INT            IDENTITY (1, 1) NOT NULL,
    [email]                 NVARCHAR (128) NULL,
    [phone]                 NVARCHAR (100) NULL,
    [username]              NVARCHAR (100) NULL,
    [password]              NVARCHAR (64)  CONSTRAINT [DF__users__password__7993056A] DEFAULT (N'') NULL,
    [salt]                  NVARCHAR (5)   CONSTRAINT [DF__users__salt__7A8729A3] DEFAULT (N'') NULL,
    [Superuser]             TINYINT        CONSTRAINT [DF__users__Superuser__7C6F7215] DEFAULT ((0)) NOT NULL,
    [State]                 SMALLINT       CONSTRAINT [DF__users__State__7B7B4DDC] DEFAULT ((0)) NOT NULL,
    [StateConfirmation]     NVARCHAR (100) NULL,
    [AuthorizationAttempts] INT            CONSTRAINT [DF_users_AuthorizationAttempts] DEFAULT ((0)) NOT NULL,
    [Block]                 SMALLINT       CONSTRAINT [DF_users_Block] DEFAULT ((0)) NOT NULL,
    [BlockedUntil]          INT            CONSTRAINT [DF_users_BlockedUntil] DEFAULT ((0)) NOT NULL,
    [BlockedReason]         NVARCHAR (500) NULL,
    [IP_reg]                NVARCHAR (100) NULL,
    [DateReg]               INT            CONSTRAINT [DF__users__DateReg__0BB1B5A5] DEFAULT ((0)) NOT NULL,
    [DateChange]            INT            CONSTRAINT [DF__users__DateChang__7E57BA87] DEFAULT ((0)) NOT NULL,
    [IdUserChange]          INT            CONSTRAINT [DF__users__IdUserCha__7F4BDEC0] DEFAULT ((0)) NOT NULL,
    [Comment]               NVARCHAR (MAX) NULL,
    [CommentAdmin]          NVARCHAR (MAX) NULL,
    [name]                  NVARCHAR (200) NULL,
    [about]                 NVARCHAR (MAX) NULL,
    [IdPhoto]               INT            NULL,
    [UniqueKey]             NVARCHAR (200) NULL,
    CONSTRAINT [PK_users_id] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_users_File] FOREIGN KEY ([IdPhoto]) REFERENCES [dbo].[File] ([IdFile]) ON DELETE SET NULL ON UPDATE SET NULL
);
















GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.users', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'users';


GO
CREATE UNIQUE NONCLUSTERED INDEX [users_UniqueKey]
    ON [dbo].[users]([UniqueKey] ASC) WHERE ([UniqueKey] IS NOT NULL);


GO



GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[UsersUpsertHistory]
   ON  [dbo].[users]
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	INSERT INTO [dbo].[UserHistory] ([id], [email], [phone], [username], [password], [salt], [Superuser], [State], [StateConfirmation], [AuthorizationAttempts], [Block], [BlockedUntil], [BlockedReason], [IP_reg], [DateReg], [DateChange], [IdUserChange], [Comment], [CommentAdmin], [name], [about], [DateChangeHistory])
	select [id], [email], [phone], [username], [password], [salt], [Superuser], [State], [StateConfirmation], [AuthorizationAttempts], [Block], [BlockedUntil], [BlockedReason], [IP_reg], [DateReg], [DateChange], [IdUserChange], [Comment], [CommentAdmin], [name], [about], GETDATE()
	from inserted

END
GO
CREATE UNIQUE NONCLUSTERED INDEX [users_email]
    ON [dbo].[users]([email] ASC) WHERE ([email] IS NOT NULL);


GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[UsersUpsertOnUtilsApplication]
   ON  [dbo].[users]
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--BEGIN TRY
	--	SET IDENTITY_INSERT dbo.[UserBase] ON;
	--END TRY
	--BEGIN CATCH
	--END CATCH

	BEGIN TRY
		-- Insert statements for trigger here
		INSERT INTO [dbo].[UserBase] ([IdUser], [IsSuperuser], [DateChange], [IdUserChange], [UniqueKey])
		SELECT i.[id], CASE WHEN i.[Superuser] = 0 THEN 0 ELSE 1 END, dateadd(second, i.[DateChange], '1970-01-01'), i.[IdUserChange], i.[UniqueKey]
		FROM inserted i
		LEFT JOIN [UserBase] u ON i.id = u.IdUser
		WHERE u.IdUser IS NULL

		UPDATE dbo.[UserBase]
		SET 
			[IsSuperuser] = CASE WHEN i.[Superuser] = 0 THEN 0 ELSE 1 END, 
			[DateChange] = dateadd(second, i.[DateChange], '1970-01-01'), 
			[IdUserChange] = i.[IdUserChange],
			[UniqueKey] = i.[UniqueKey]
		FROM inserted i
		INNER JOIN dbo.[UserBase] u ON i.id = u.IdUser
	END TRY
	BEGIN CATCH
	END CATCH

	--BEGIN TRY
	--	SET IDENTITY_INSERT dbo.[User] OFF;
	--END TRY
	--BEGIN CATCH
	--END CATCH

END
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[UsersDeleteOnUtilsApplication]
   ON  [dbo].[users]
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		DELETE FROM [dbo].[UserBase]
		FROM [dbo].[UserBase]
		INNER JOIN deleted d ON [dbo].[UserBase].IdUser = d.id
	END TRY
	BEGIN CATCH
	END CATCH

END