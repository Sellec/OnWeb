CREATE TYPE [dbo].[TVP_MessagesQueue] AS TABLE (
    [IdQueue]     INT        DEFAULT ((0)) NOT NULL,
    [MessageInfo] BINARY (1) NULL);

