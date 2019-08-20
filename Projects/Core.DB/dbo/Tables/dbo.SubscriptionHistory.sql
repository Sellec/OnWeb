CREATE TABLE [dbo].[SubscriptionHistory] (
    [id]        INT            IDENTITY (2737, 1) NOT NULL,
    [subscr_id] INT            DEFAULT ((0)) NOT NULL,
    [email]     NVARCHAR (200) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_subscriptionhistory_id] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [subscriptionhistory$subscr_id] UNIQUE NONCLUSTERED ([subscr_id] ASC, [email] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.subscriptionhistory', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionHistory';

