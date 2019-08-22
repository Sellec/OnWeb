CREATE TABLE [dbo].[UserLogHistoryEventType] (
    [IdEventType]   INT            IDENTITY (5, 1) NOT NULL,
    [NameEventType] NVARCHAR (200) CONSTRAINT [DF__userloghi__NameE__76B698BF] DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_userloghistoryeventtype_IdEventType] PRIMARY KEY CLUSTERED ([IdEventType] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.userloghistoryeventtype', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UserLogHistoryEventType';

