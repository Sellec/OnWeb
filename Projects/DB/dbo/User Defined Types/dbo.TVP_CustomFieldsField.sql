CREATE TYPE [dbo].[TVP_CustomFieldsField] AS TABLE (
    [field_id]           INT            DEFAULT ((0)) NOT NULL,
    [field_module]       NVARCHAR (100) DEFAULT (N'') NULL,
    [field_module1]      INT            DEFAULT ((0)) NOT NULL,
    [field_name]         NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [NameFieldAlt]       NVARCHAR (200) NULL,
    [field_type]         INT            DEFAULT ((0)) NOT NULL,
    [field_alias]        NVARCHAR (100) NULL,
    [IdValueType]        INT            DEFAULT ((0)) NOT NULL,
    [field_size]         INT            DEFAULT ((0)) NOT NULL,
    [field_data]         NVARCHAR (MAX) NULL,
    [field_mustvalue]    BIT            DEFAULT ((0)) NOT NULL,
    [IsMultipleValues]   BIT            DEFAULT ((0)) NOT NULL,
    [status]             INT            DEFAULT ((1)) NOT NULL,
    [Block]              INT            DEFAULT ((0)) NOT NULL,
    [NameEnding]         NVARCHAR (100) DEFAULT (NULL) NULL,
    [FormatCheck]        NVARCHAR (200) DEFAULT (NULL) NULL,
    [IdSource]           INT            DEFAULT ((0)) NOT NULL,
    [ParameterNumeric01] REAL           DEFAULT ((0)) NOT NULL,
    [ParameterNumeric02] REAL           DEFAULT ((0)) NOT NULL,
    [DateChange]         INT            DEFAULT ((0)) NOT NULL,
    [UniqueKey]          NVARCHAR (100) NULL);





