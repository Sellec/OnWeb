CREATE TABLE [dbo].[JournalName] (
    [IdJournal]     INT            IDENTITY (1, 1) NOT NULL,
    [IdJournalType] INT            NOT NULL,
    [JournalName]   NVARCHAR (100) NOT NULL,
    [UniqueKey]     NVARCHAR (100) NULL,
    CONSTRAINT [PK_JournalName] PRIMARY KEY CLUSTERED ([IdJournal] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex_20171216_173757]
    ON [dbo].[JournalName]([UniqueKey] ASC) WHERE ([UniqueKey] IS NOT NULL);

