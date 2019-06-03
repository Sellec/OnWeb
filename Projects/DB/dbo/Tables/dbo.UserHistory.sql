CREATE TABLE [dbo].[UserHistory] (
    [IdUserHistory]         INT            IDENTITY (1, 1) NOT NULL,
    [id]                    INT            NOT NULL,
    [email]                 NVARCHAR (128) CONSTRAINT [DF__UserHistory__email__77AABCF8] DEFAULT (N'') NULL,
    [phone]                 NVARCHAR (100) CONSTRAINT [DF__UserHistory__phone__789EE131] DEFAULT (N'') NULL,
    [username]              NVARCHAR (100) NULL,
    [password]              NVARCHAR (64)  CONSTRAINT [DF__UserHistory__password__7993056A] DEFAULT (N'') NULL,
    [salt]                  NVARCHAR (5)   CONSTRAINT [DF__UserHistory__salt__7A8729A3] DEFAULT (N'') NULL,
    [Superuser]             TINYINT        CONSTRAINT [DF__UserHistory__Superuser__7C6F7215] DEFAULT ((0)) NOT NULL,
    [State]                 SMALLINT       CONSTRAINT [DF__UserHistory__State__7B7B4DDC] DEFAULT ((0)) NOT NULL,
    [StateConfirmation]     NVARCHAR (100) NULL,
    [AuthorizationAttempts] INT            CONSTRAINT [DF_UserHistory_AuthorizationAttempts] DEFAULT ((0)) NOT NULL,
    [Block]                 SMALLINT       CONSTRAINT [DF_UserHistory_Block] DEFAULT ((0)) NOT NULL,
    [BlockedUntil]          INT            CONSTRAINT [DF_UserHistory_BlockedUntil] DEFAULT ((0)) NOT NULL,
    [BlockedReason]         NVARCHAR (500) NULL,
    [IP_reg]                NVARCHAR (100) CONSTRAINT [DF__UserHistory__IP_reg__0ABD916C] DEFAULT (N'') NULL,
    [DateReg]               INT            CONSTRAINT [DF__UserHistory__DateReg__0BB1B5A5] DEFAULT ((0)) NOT NULL,
    [DateChange]            INT            CONSTRAINT [DF__UserHistory__DateChang__7E57BA87] DEFAULT ((0)) NOT NULL,
    [IdUserChange]          INT            CONSTRAINT [DF__UserHistory__IdUserCha__7F4BDEC0] DEFAULT ((0)) NOT NULL,
    [Comment]               NVARCHAR (MAX) NULL,
    [CommentAdmin]          NVARCHAR (MAX) NULL,
    [name]                  NVARCHAR (200) CONSTRAINT [DF__UserHistory__name__7D63964E] DEFAULT (N'') NULL,
    [about]                 NVARCHAR (MAX) NULL,
    [DateChangeHistory]     DATETIME       CONSTRAINT [DF_UserHistory_DateChangeHistory] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_UserHistory_IdUserHistory] PRIMARY KEY CLUSTERED ([IdUserHistory] ASC)
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.UserHistory', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserHistory';

