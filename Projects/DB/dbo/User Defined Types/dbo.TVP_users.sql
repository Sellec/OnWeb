CREATE TYPE [dbo].[TVP_users] AS TABLE (
    [id]                    INT            DEFAULT ((0)) NOT NULL,
    [email]                 NVARCHAR (128) NULL,
    [phone]                 NVARCHAR (100) NULL,
    [username]              NVARCHAR (100) NULL,
    [password]              NVARCHAR (64)  DEFAULT (N'') NULL,
    [salt]                  NVARCHAR (5)   DEFAULT (N'') NULL,
    [Superuser]             TINYINT        DEFAULT ((0)) NOT NULL,
    [State]                 SMALLINT       DEFAULT ((0)) NOT NULL,
    [StateConfirmation]     NVARCHAR (100) NULL,
    [AuthorizationAttempts] INT            DEFAULT ((0)) NOT NULL,
    [Block]                 SMALLINT       DEFAULT ((0)) NOT NULL,
    [BlockedUntil]          INT            DEFAULT ((0)) NOT NULL,
    [BlockedReason]         NVARCHAR (500) NULL,
    [IP_reg]                NVARCHAR (100) NULL,
    [DateReg]               INT            DEFAULT ((0)) NOT NULL,
    [DateChange]            INT            DEFAULT ((0)) NOT NULL,
    [IdUserChange]          INT            DEFAULT ((0)) NOT NULL,
    [Comment]               NVARCHAR (MAX) NULL,
    [CommentAdmin]          NVARCHAR (MAX) NULL,
    [name]                  NVARCHAR (200) NULL,
    [about]                 NVARCHAR (MAX) NULL,
    [IdPhoto]               INT            NULL,
    [UniqueKey]             NVARCHAR (200) NULL);





