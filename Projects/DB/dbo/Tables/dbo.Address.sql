CREATE TABLE [dbo].[Address] (
    [IdAddress]         INT              IDENTITY (1, 1) NOT NULL,
    [KodAddress]        NVARCHAR (32)    CONSTRAINT [DF__address__KodAddr__4F67C174] DEFAULT (N'') NOT NULL,
    [NameAddress]       NVARCHAR (200)   CONSTRAINT [DF__address__NameAdd__505BE5AD] DEFAULT (N'') NOT NULL,
    [NameAddressShort]  NVARCHAR (100)   CONSTRAINT [DF__address__NameAdd__515009E6] DEFAULT (N'') NOT NULL,
    [NameAddressFull]   NVARCHAR (200)   CONSTRAINT [DF__address__NameAdd__52442E1F] DEFAULT (N'') NOT NULL,
    [KodFias]           UNIQUEIDENTIFIER NULL,
    [ZipCode]           NVARCHAR (8)     NULL,
    [DateChange]        DATETIME         CONSTRAINT [DF_Address_DateChange] DEFAULT (getdate()) NOT NULL,
    [KodRegion]         NVARCHAR (32)    CONSTRAINT [DF__address__KodRegi__542C7691] DEFAULT (N'') NOT NULL,
    [KodDistrict]       NVARCHAR (32)    CONSTRAINT [DF__address__KodDist__55209ACA] DEFAULT (N'') NOT NULL,
    [KodCity]           NVARCHAR (32)    CONSTRAINT [DF__address__KodCity__5614BF03] DEFAULT (N'') NOT NULL,
    [KodStreet]         NVARCHAR (32)    CONSTRAINT [DF__address__KodStre__5708E33C] DEFAULT (N'') NOT NULL,
    [KodBuildingCommon] NVARCHAR (32)    CONSTRAINT [DF__address__KodBuil__57FD0775] DEFAULT (N'') NOT NULL,
    [KodBuilding]       NVARCHAR (32)    CONSTRAINT [DF__address__KodBuil__58F12BAE] DEFAULT (N'') NOT NULL,
    [AddressType]       TINYINT          CONSTRAINT [DF_Address_AddressType] DEFAULT ((0)) NOT NULL,
    [CoordinateX]       DECIMAL (9, 6)   CONSTRAINT [DF_Address_CoordinateX] DEFAULT ((0)) NOT NULL,
    [CoordinateY]       DECIMAL (9, 6)   CONSTRAINT [DF_Address_CoordinateY] DEFAULT ((0)) NOT NULL,
    [IsRegion]          BIT              CONSTRAINT [DF_Address_IsRegion] DEFAULT ((0)) NOT NULL,
    [IsRegionCenter]    BIT              CONSTRAINT [DF_Address_IsRegionCenter] DEFAULT ((0)) NOT NULL,
    [IsDistrict]        BIT              CONSTRAINT [DF_Address_IsDistrict] DEFAULT ((0)) NOT NULL,
    [IsDistrictCenter]  BIT              CONSTRAINT [DF_Address_IsDistrictCenter] DEFAULT ((0)) NOT NULL,
    [IsCity]            BIT              CONSTRAINT [DF_Address_IsCity] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_address_IdAddress] PRIMARY KEY CLUSTERED ([KodAddress] ASC)
);










GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.address', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Address';


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex_KodCity]
    ON [dbo].[Address]([KodCity] ASC)
    INCLUDE([KodAddress]);


GO
CREATE NONCLUSTERED INDEX [NameAddressShort_IsDistrictCenter_with_KodAddress_KodDistrict]
    ON [dbo].[Address]([NameAddressShort] ASC, [IsDistrictCenter] ASC)
    INCLUDE([KodAddress], [KodDistrict]);


GO
CREATE NONCLUSTERED INDEX [KodDistrict_with_KodAddress_KodCity]
    ON [dbo].[Address]([KodDistrict] ASC)
    INCLUDE([KodAddress], [KodCity]);

