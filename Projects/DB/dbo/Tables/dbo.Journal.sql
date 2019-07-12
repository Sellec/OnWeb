CREATE TABLE [dbo].[Journal] (
    [IdJournalData]     INT            IDENTITY (1, 1) NOT NULL,
    [IdJournal]         INT            NOT NULL,
    [EventType]         TINYINT        CONSTRAINT [DF_Journal_EventType] DEFAULT ((1)) NOT NULL,
    [EventInfo]         NVARCHAR (300) NOT NULL,
    [EventInfoDetailed] NVARCHAR (MAX) NULL,
    [ExceptionDetailed] NVARCHAR (MAX) NULL,
    [DateEvent]         DATETIME       NOT NULL,
    [IdRelatedItem]     INT            NULL,
    [IdRelatedItemType] NCHAR (10)     NULL,
    CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED ([IdJournalData] ASC)
);





