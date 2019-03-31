CREATE TABLE [dbo].[AddressSearchHistory] (
    [IdAddressSearch]   INT            IDENTITY (1, 1) NOT NULL,
    [NameAddressSearch] NVARCHAR (500) NOT NULL,
    [DateSearch]        DATETIME       NOT NULL,
    [IsSuccess]         BIT            NOT NULL,
    [KodAddress]        NVARCHAR (32)  NULL,
    [AddressType]       TINYINT        NOT NULL,
    [ServiceFound]      NVARCHAR (50)  NULL,
    [ServiceAnswer]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AddressSearchHistory] PRIMARY KEY CLUSTERED ([IdAddressSearch] ASC)
);






GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex_20171215_083326]
    ON [dbo].[AddressSearchHistory]([NameAddressSearch] ASC, [ServiceFound] ASC)
    INCLUDE([DateSearch]);

