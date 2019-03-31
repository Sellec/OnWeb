CREATE TABLE [dbo].[captcha] (
    [id]     INT           IDENTITY (17459, 1) NOT NULL,
    [code]   NVARCHAR (32) NOT NULL,
    [number] INT           DEFAULT ((0)) NOT NULL,
    [dtime]  INT           NOT NULL,
    CONSTRAINT [PK_captcha_id] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'capitalrent.captcha', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'captcha';

