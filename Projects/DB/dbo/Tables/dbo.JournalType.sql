CREATE TABLE [dbo].[JournalType] (
    [IdJournalType]   INT            IDENTITY (1, 1) NOT NULL,
    [NameJournalType] NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_JournalType] PRIMARY KEY CLUSTERED ([IdJournalType] ASC)
);

