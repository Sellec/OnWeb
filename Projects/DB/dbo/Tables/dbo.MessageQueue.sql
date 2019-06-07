CREATE TABLE [dbo].[MessageQueue] (
    [IdQueue]       INT            IDENTITY (1, 1) NOT NULL,
    [IdMessageType] TINYINT        CONSTRAINT [DF_MessagesQueue_IdMessageType] DEFAULT ((0)) NOT NULL,
    [MessageInfo]   BINARY (8000)  NULL,
    [DateCreate]    DATETIME       CONSTRAINT [DF_MessageQueue_DateCreate] DEFAULT (getdate()) NOT NULL,
    [IsHandled]     BIT            CONSTRAINT [DF_MessageQueue_IsHandled] DEFAULT ((0)) NOT NULL,
    [IsSent]        BIT            CONSTRAINT [DF_MessageQueue_IsSent] DEFAULT ((0)) NOT NULL,
    [DateSent]      DATETIME       NULL,
    [ExternalID]    NVARCHAR (100) NULL,
    CONSTRAINT [PK_MessageQueue] PRIMARY KEY CLUSTERED ([IdQueue] ASC)
);






GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex_1]
    ON [dbo].[MessageQueue]([IdMessageType] ASC, [IsSent] ASC)
    INCLUDE([IdQueue], [MessageInfo], [DateCreate], [DateSent], [ExternalID]);

