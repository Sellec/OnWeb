CREATE TYPE [dbo].[TVP_AddressSearchHistory] AS TABLE (
    [IdAddressSearch]   INT            DEFAULT ((0)) NOT NULL,
    [NameAddressSearch] NVARCHAR (500) DEFAULT ('') NOT NULL,
    [DateSearch]        DATETIME       DEFAULT (getdate()) NOT NULL,
    [IsSuccess]         BIT            DEFAULT ((0)) NOT NULL,
    [KodAddress]        NVARCHAR (32)  NULL,
    [AddressType]       TINYINT        DEFAULT ((0)) NOT NULL,
    [ServiceFound]      NVARCHAR (50)  NULL,
    [ServiceAnswer]     NVARCHAR (MAX) NULL);



