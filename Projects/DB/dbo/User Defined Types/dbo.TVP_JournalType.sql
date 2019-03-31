CREATE TYPE [dbo].[TVP_JournalType] AS TABLE (
    [IdJournalType]   INT            DEFAULT ((0)) NOT NULL,
    [NameJournalType] NVARCHAR (100) DEFAULT ('') NOT NULL);

