CREATE TYPE [dbo].[TVP_captcha] AS TABLE (
    [id]     INT           DEFAULT ((0)) NOT NULL,
    [code]   NVARCHAR (32) DEFAULT ('') NOT NULL,
    [number] INT           DEFAULT ((0)) NOT NULL,
    [dtime]  INT           DEFAULT ((0)) NOT NULL);

