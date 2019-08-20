CREATE TYPE [dbo].[TVP_CurrencyCoefficient] AS TABLE (
    [IdCurrencyFrom]   INT              DEFAULT ((0)) NOT NULL,
    [IdCurrencyTo]     INT              DEFAULT ((0)) NOT NULL,
    [PriceCoefficient] DECIMAL (30, 15) DEFAULT ((0)) NOT NULL,
    [DateChange]       INT              DEFAULT ((0)) NOT NULL);

