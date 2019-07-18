CREATE TYPE [dbo].[TVP_JournalName] AS TABLE (
    [IdJournal]     INT            DEFAULT ((0)) NOT NULL,
    [IdJournalType] INT            DEFAULT ((0)) NOT NULL,
    [JournalName]   NVARCHAR (150) DEFAULT ('') NOT NULL,
    [UniqueKey]     NVARCHAR (600) NULL);



