CREATE TYPE [dbo].[TVP_InsertOnDuplicate_Filters] AS TABLE (
    [TableName] NVARCHAR (128) DEFAULT ('') NOT NULL,
    [FilterStr] NVARCHAR (MAX) DEFAULT ('') NOT NULL);

