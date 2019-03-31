CREATE TYPE [dbo].[TVP_SubscriptionRole] AS TABLE (
    [IdSubscription] INT DEFAULT ((0)) NOT NULL,
    [IdRole]         INT DEFAULT ((0)) NOT NULL,
    [IdUserChange]   INT DEFAULT ((0)) NOT NULL,
    [DateChange]     INT DEFAULT ((0)) NOT NULL);

