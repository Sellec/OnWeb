CREATE TYPE [dbo].[TVP_CurrencyToday] AS TABLE (
    [id]      INT            DEFAULT ((0)) NOT NULL,
    [usd]     NVARCHAR (100) DEFAULT ('') NOT NULL,
    [eur]     NVARCHAR (100) DEFAULT ('') NOT NULL,
    [usd_pos] NVARCHAR (10)  DEFAULT ('') NOT NULL,
    [eur_pos] NVARCHAR (10)  DEFAULT ('') NOT NULL,
    [usd_how] NVARCHAR (100) DEFAULT ('') NOT NULL,
    [eur_how] NVARCHAR (100) DEFAULT ('') NOT NULL);

