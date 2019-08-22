CREATE TABLE [dbo].[CustomFieldsField] (
    [field_id]           INT            IDENTITY (1, 1) NOT NULL,
    [field_module]       NVARCHAR (100) CONSTRAINT [DF__customfie__field__294D0584] DEFAULT (N'') NULL,
    [field_module1]      INT            CONSTRAINT [DF__customfie__field__2A4129BD] DEFAULT ((0)) NOT NULL,
    [field_name]         NVARCHAR (200) CONSTRAINT [DF__customfie__field__2B354DF6] DEFAULT (N'') NOT NULL,
    [NameFieldAlt]       NVARCHAR (200) NULL,
    [field_type]         INT            CONSTRAINT [DF__customfie__field__2C29722F] DEFAULT ((0)) NOT NULL,
    [field_alias]        NVARCHAR (100) NULL,
    [IdValueType]        INT            CONSTRAINT [DF__customfie__IdVal__2D1D9668] DEFAULT ((0)) NOT NULL,
    [field_size]         INT            CONSTRAINT [DF__customfie__field__2E11BAA1] DEFAULT ((0)) NOT NULL,
    [field_data]         NVARCHAR (MAX) NULL,
    [field_mustvalue]    BIT            CONSTRAINT [DF__customfie__field__2F05DEDA] DEFAULT ((0)) NOT NULL,
    [IsMultipleValues]   BIT            CONSTRAINT [DF_CustomFieldsField_IsMultipleValues] DEFAULT ((0)) NOT NULL,
    [status]             INT            CONSTRAINT [DF__customfie__statu__2FFA0313] DEFAULT ((1)) NOT NULL,
    [Block]              INT            CONSTRAINT [DF__customfie__Block__30EE274C] DEFAULT ((0)) NOT NULL,
    [NameEnding]         NVARCHAR (100) CONSTRAINT [DF__customfie__NameE__31E24B85] DEFAULT (NULL) NULL,
    [FormatCheck]        NVARCHAR (200) CONSTRAINT [DF__customfie__Forma__32D66FBE] DEFAULT (NULL) NULL,
    [IdSource]           INT            CONSTRAINT [DF__customfie__IdSou__33CA93F7] DEFAULT ((0)) NOT NULL,
    [ParameterNumeric01] REAL           CONSTRAINT [DF__customfie__Param__34BEB830] DEFAULT ((0)) NOT NULL,
    [ParameterNumeric02] REAL           CONSTRAINT [DF__customfie__Param__35B2DC69] DEFAULT ((0)) NOT NULL,
    [DateChange]         INT            CONSTRAINT [DF__customfie__DateC__36A700A2] DEFAULT ((0)) NOT NULL,
    [UniqueKey]          NVARCHAR (100) NULL,
    CONSTRAINT [PK_CustomFieldsField] PRIMARY KEY CLUSTERED ([field_id] ASC)
);








GO
CREATE NONCLUSTERED INDEX [field_id]
    ON [dbo].[CustomFieldsField]([field_id] ASC, [field_module1] ASC);


GO
CREATE NONCLUSTERED INDEX [field_module1]
    ON [dbo].[CustomFieldsField]([field_module1] ASC);


GO
CREATE NONCLUSTERED INDEX [field_module1_2]
    ON [dbo].[CustomFieldsField]([field_module1] ASC, [field_name] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [CustomFieldsField_UniqueKey]
    ON [dbo].[CustomFieldsField]([UniqueKey] ASC) WHERE ([UniqueKey] IS NOT NULL);




GO





GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'fabrikae_fabrikanew.customfieldsfield', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CustomFieldsField';

