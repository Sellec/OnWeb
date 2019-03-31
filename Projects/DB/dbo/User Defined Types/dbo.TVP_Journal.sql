CREATE TYPE [dbo].[TVP_Journal] AS TABLE (
    [IdJournalData]     INT            DEFAULT ((0)) NOT NULL,
    [IdJournal]         INT            DEFAULT ((0)) NOT NULL,
    [EventType]         TINYINT        DEFAULT ((1)) NOT NULL,
    [EventInfo]         NVARCHAR (300) DEFAULT ('') NOT NULL,
    [EventInfoDetailed] NVARCHAR (MAX) NULL,
    [ExceptionDetailed] NVARCHAR (MAX) NULL,
    [DateEvent]         DATETIME       DEFAULT (getdate()) NOT NULL);

