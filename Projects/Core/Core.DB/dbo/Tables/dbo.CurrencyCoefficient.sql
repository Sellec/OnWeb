CREATE TABLE [dbo].[CurrencyCoefficient] (
    [IdCurrencyFrom]   INT              NOT NULL,
    [IdCurrencyTo]     INT              NOT NULL,
    [PriceCoefficient] DECIMAL (30, 15) NOT NULL,
    [DateChange]       INT              NOT NULL,
    CONSTRAINT [PK_currencycoefficient_IdCurrencyFrom] PRIMARY KEY CLUSTERED ([IdCurrencyFrom] ASC, [IdCurrencyTo] ASC)
);

