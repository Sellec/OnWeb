CREATE TABLE [dbo].[JournalName] (
    [IdJournal]     INT            IDENTITY (1, 1) NOT NULL,
    [IdJournalType] INT            NOT NULL,
    [JournalName]   NVARCHAR (150) NOT NULL,
    [UniqueKey]     NVARCHAR (600) NULL,
    CONSTRAINT [PK_JournalName] PRIMARY KEY CLUSTERED ([IdJournal] ASC)
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [UniqueKey]
    ON [dbo].[JournalName]([UniqueKey] ASC) WHERE ([UniqueKey] IS NOT NULL);

