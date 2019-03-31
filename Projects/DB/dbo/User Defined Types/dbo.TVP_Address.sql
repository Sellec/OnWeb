CREATE TYPE [dbo].[TVP_Address] AS TABLE (
    [IdAddress]         INT              DEFAULT ((0)) NOT NULL,
    [KodAddress]        NVARCHAR (32)    DEFAULT (N'') NOT NULL,
    [NameAddress]       NVARCHAR (200)   DEFAULT (N'') NOT NULL,
    [NameAddressShort]  NVARCHAR (100)   DEFAULT (N'') NOT NULL,
    [NameAddressFull]   NVARCHAR (200)   DEFAULT (N'') NOT NULL,
    [KodFias]           UNIQUEIDENTIFIER NULL,
    [ZipCode]           NVARCHAR (8)     NULL,
    [DateChange]        DATETIME         DEFAULT (getdate()) NOT NULL,
    [KodRegion]         NVARCHAR (32)    DEFAULT (N'') NOT NULL,
    [KodDistrict]       NVARCHAR (32)    DEFAULT (N'') NOT NULL,
    [KodCity]           NVARCHAR (32)    DEFAULT (N'') NOT NULL,
    [KodStreet]         NVARCHAR (32)    DEFAULT (N'') NOT NULL,
    [KodBuildingCommon] NVARCHAR (32)    DEFAULT (N'') NOT NULL,
    [KodBuilding]       NVARCHAR (32)    DEFAULT (N'') NOT NULL,
    [AddressType]       TINYINT          DEFAULT ((0)) NOT NULL,
    [CoordinateX]       DECIMAL (9, 6)   DEFAULT ((0)) NOT NULL,
    [CoordinateY]       DECIMAL (9, 6)   DEFAULT ((0)) NOT NULL,
    [IsRegion]          BIT              DEFAULT ((0)) NOT NULL,
    [IsRegionCenter]    BIT              DEFAULT ((0)) NOT NULL,
    [IsDistrict]        BIT              DEFAULT ((0)) NOT NULL,
    [IsDistrictCenter]  BIT              DEFAULT ((0)) NOT NULL,
    [IsCity]            BIT              DEFAULT ((0)) NOT NULL);









