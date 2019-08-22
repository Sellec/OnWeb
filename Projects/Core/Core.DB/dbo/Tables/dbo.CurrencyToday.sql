CREATE TABLE [dbo].[CurrencyToday] (
    [id]      INT            IDENTITY (2, 1) NOT NULL,
    [usd]     NVARCHAR (100) NOT NULL,
    [eur]     NVARCHAR (100) NOT NULL,
    [usd_pos] NVARCHAR (10)  NOT NULL,
    [eur_pos] NVARCHAR (10)  NOT NULL,
    [usd_how] NVARCHAR (100) NOT NULL,
    [eur_how] NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_currency_today_id] PRIMARY KEY CLUSTERED ([id] ASC)
);

