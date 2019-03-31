CREATE TABLE [dbo].[MessageQueueHistory] (
    [IdQueueHistory] INT            IDENTITY (1, 1) NOT NULL,
    [IdQueue]        INT            CONSTRAINT [DF_MessageQueueHistory_IdQueue] DEFAULT ((0)) NOT NULL,
    [DateEvent]      DATETIME       NOT NULL,
    [EventText]      NVARCHAR (500) NULL,
    [IsSuccess]      BIT            CONSTRAINT [DF_MessageQueueHistory_IsSuccess] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_MessageQueueHistory] PRIMARY KEY CLUSTERED ([IdQueueHistory] ASC)
);

