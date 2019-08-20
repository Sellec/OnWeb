CREATE TABLE [dbo].[UserLogHistory] (
    [IdRecord]    INT            IDENTITY (13849, 1) NOT NULL,
    [IdUser]      INT            CONSTRAINT [DF__userloghi__IdUse__70FDBF69] DEFAULT ((0)) NOT NULL,
    [DateEvent]   INT            CONSTRAINT [DF__userloghi__DateE__71F1E3A2] DEFAULT ((0)) NOT NULL,
    [IdEventType] INT            CONSTRAINT [DF__userloghi__IdEve__72E607DB] DEFAULT ((0)) NOT NULL,
    [IP]          NVARCHAR (50)  CONSTRAINT [DF__userloghisto__IP__73DA2C14] DEFAULT (N'') NULL,
    [Browser]     NVARCHAR (255) CONSTRAINT [DF__userloghi__Brows__74CE504D] DEFAULT (N'') NULL,
    [Place]       NVARCHAR (255) CONSTRAINT [DF__userloghi__Place__75C27486] DEFAULT (N'') NULL,
    [Comment]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_userloghistory_IdRecord] PRIMARY KEY CLUSTERED ([IdRecord] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IdUser]
    ON [dbo].[UserLogHistory]([IdUser] ASC);


GO
CREATE NONCLUSTERED INDEX [IdUser_2]
    ON [dbo].[UserLogHistory]([IdUser] ASC, [IdEventType] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.userloghistory', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserLogHistory';

