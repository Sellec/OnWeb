CREATE TYPE [dbo].[TVP_SubscriptionEmail] AS TABLE (
    [id]           INT            DEFAULT ((0)) NOT NULL,
    [subscr_id]    INT            DEFAULT ((0)) NOT NULL,
    [email]        NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [password]     NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [IdUserChange] INT            DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            DEFAULT ((0)) NOT NULL);

