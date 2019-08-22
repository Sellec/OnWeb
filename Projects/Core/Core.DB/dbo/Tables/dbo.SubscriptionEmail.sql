CREATE TABLE [dbo].[SubscriptionEmail] (
    [id]           INT            IDENTITY (2603, 1) NOT NULL,
    [subscr_id]    INT            CONSTRAINT [DF__subscript__subsc__7BD05397] DEFAULT ((0)) NOT NULL,
    [email]        NVARCHAR (200) CONSTRAINT [DF__subscript__email__7CC477D0] DEFAULT (N'') NOT NULL,
    [password]     NVARCHAR (200) CONSTRAINT [DF__subscript__passw__7DB89C09] DEFAULT (N'') NOT NULL,
    [IdUserChange] INT            CONSTRAINT [DF__subscript__IdUse__7EACC042] DEFAULT ((0)) NOT NULL,
    [DateChange]   INT            CONSTRAINT [DF__subscript__DateC__7FA0E47B] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_subscriptionemail_id] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [subscriptionemail$subscr_id] UNIQUE NONCLUSTERED ([subscr_id] ASC, [email] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.subscriptionemail', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionEmail';

