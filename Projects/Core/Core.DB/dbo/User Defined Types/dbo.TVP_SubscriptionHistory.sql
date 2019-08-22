CREATE TYPE [dbo].[TVP_SubscriptionHistory] AS TABLE (
    [id]        INT            DEFAULT ((0)) NOT NULL,
    [subscr_id] INT            DEFAULT ((0)) NOT NULL,
    [email]     NVARCHAR (200) DEFAULT (N'') NOT NULL);

