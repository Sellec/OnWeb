CREATE TABLE [dbo].[Sessions] (
    [SessionId]   VARCHAR (24)    NOT NULL,
    [Created]     SMALLDATETIME   NOT NULL,
    [Expires]     SMALLDATETIME   NOT NULL,
    [LockDate]    SMALLDATETIME   NOT NULL,
    [LockId]      INT             NOT NULL,
    [Locked]      BIT             NOT NULL,
    [ItemContent] VARBINARY (MAX) NULL,
    [IdUser]      INT             NOT NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED ([SessionId] ASC)
);

