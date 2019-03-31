CREATE TABLE [dbo].[InsertOnDuplicate_MergeFiltersCache] (
    [TableName] NVARCHAR (128) NOT NULL,
    [FilterStr] NVARCHAR (MAX) NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [TableName]
    ON [dbo].[InsertOnDuplicate_MergeFiltersCache]([TableName] ASC);

