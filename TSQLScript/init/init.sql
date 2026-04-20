-- ============================================================
-- 初始化腳本：Myoffice_ACPD
-- 執行順序：資料庫 → 資料表 → SP → 測試資料
-- ============================================================

-- ────────────────────────────────────────────────────────────
-- 1. 建立資料庫
-- ────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Myoffice_ACPD')
BEGIN
    CREATE DATABASE Myoffice_ACPD;
END
GO

USE Myoffice_ACPD;
GO

-- ────────────────────────────────────────────────────────────
-- 2. 建立主資料表 MyOffice_ACPD
-- ────────────────────────────────────────────────────────────
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MyOffice_ACPD]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[MyOffice_ACPD](
        [ACPD_SID]         [char](20)      NOT NULL,
        [ACPD_Cname]       [nvarchar](60)  NULL,
        [ACPD_Ename]       [nvarchar](40)  NULL,
        [ACPD_Sname]       [nvarchar](40)  NULL,
        [ACPD_Email]       [nvarchar](60)  NULL,
        [ACPD_Status]      [tinyint]       NULL,
        [ACPD_Stop]        [bit]           NULL,
        [ACPD_StopMemo]    [nvarchar](60)  NULL,
        [ACPD_LoginID]     [nvarchar](30)  NULL,
        [ACPD_LoginPWD]    [nvarchar](60)  NULL,
        [ACPD_Memo]        [nvarchar](600) NULL,
        [ACPD_NowDateTime] [datetime]      NULL,
        [ACPD_NowID]       [nvarchar](20)  NULL,
        [ACPD_UPDDateTime] [datetime]      NULL,
        [ACPD_UPDID]       [nvarchar](20)  NULL,
        CONSTRAINT [PK_MyOffice_ACPD] PRIMARY KEY CLUSTERED ([ACPD_SID] ASC)
    );

    ALTER TABLE [dbo].[MyOffice_ACPD] ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_status]      DEFAULT ((0))        FOR [ACPD_Status];
    ALTER TABLE [dbo].[MyOffice_ACPD] ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_stop]        DEFAULT ((0))        FOR [ACPD_Stop];
    ALTER TABLE [dbo].[MyOffice_ACPD] ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_nowdatetime] DEFAULT (getdate())  FOR [ACPD_NowDateTime];
    ALTER TABLE [dbo].[MyOffice_ACPD] ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_upddatetime] DEFAULT (getdate())  FOR [ACPD_UPDDateTime];
END
GO

-- ────────────────────────────────────────────────────────────
-- 3. 建立執行日誌資料表 MyOffice_ExcuteionLog
-- ────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MyOffice_ExcuteionLog]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[MyOffice_ExcuteionLog](
        [DeLog_AutoID]           [bigint]           IDENTITY(1,1) NOT NULL,
        [DeLog_StoredPrograms]   [nvarchar](120)    NOT NULL,
        [DeLog_GroupID]          [uniqueidentifier] NOT NULL,
        [DeLog_isCustomDebug]    [bit]              NOT NULL,
        [DeLog_ExecutionProgram] [nvarchar](120)    NOT NULL,
        [DeLog_ExecutionInfo]    [nvarchar](max)    NULL,
        [DeLog_verifyNeeded]     [bit]              NULL,
        [DeLog_ExDateTime]       [datetime]         NOT NULL,
        CONSTRAINT [PK_MOTC_DataExchangeLog] PRIMARY KEY CLUSTERED ([DeLog_AutoID] ASC)
    );

    ALTER TABLE [dbo].[MyOffice_ExcuteionLog] ADD CONSTRAINT [DF_MOTC_DataExchangeLog_DeLog_isCustomDebug]  DEFAULT ((0))       FOR [DeLog_isCustomDebug];
    ALTER TABLE [dbo].[MyOffice_ExcuteionLog] ADD CONSTRAINT [DF_MyOffice_ExcuteionLog_DeLog_verifyNeeded]  DEFAULT ((0))       FOR [DeLog_verifyNeeded];
    ALTER TABLE [dbo].[MyOffice_ExcuteionLog] ADD CONSTRAINT [DF_MOTC_DataExchangeLog_DeLog_ExDateTime]     DEFAULT (getdate()) FOR [DeLog_ExDateTime];
END
GO

-- ────────────────────────────────────────────────────────────
-- 4. 建立 SP：NEWSID（產生 20 碼唯一主鍵）
--    格式：2碼年份代碼 + 3碼年內第幾天 + 5碼當天秒數 + 10碼亂數
-- ────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.NEWSID') IS NOT NULL
    DROP PROCEDURE [dbo].[NEWSID]
GO

CREATE PROCEDURE [dbo].[NEWSID]
(
    @TableName  nvarchar(128),
    @ReturnSID  nvarchar(20) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @SIDRowName   nvarchar(20)
    DECLARE @currentYear  int
    DECLARE @dayOfYear    int
    DECLARE @secondOfDay  int
    DECLARE @alphabets    char(36)
    DECLARE @firstDigit   char(1)
    DECLARE @secondDigit  char(1)
    DECLARE @prefix       char(2)
    DECLARE @dayCode      char(3)
    DECLARE @secondCode   char(5)
    DECLARE @sql          nvarchar(MAX)
    DECLARE @randomValue  char(10)
    DECLARE @ParmDefinition nvarchar(500)

    SET @currentYear  = YEAR(GETDATE()) - 2000;
    SET @dayOfYear    = DATEPART(DAYOFYEAR, GETDATE());
    SET @secondOfDay  = DATEPART(SECOND, GETDATE())
                      + (60   * DATEPART(MINUTE, GETDATE()))
                      + (3600 * DATEPART(HOUR,   GETDATE()));

    IF (@currentYear > 1295) SET @currentYear = 1295;

    SET @alphabets    = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    SET @firstDigit   = SUBSTRING(@alphabets, (@currentYear / 36) % 36 + 1, 1);
    SET @secondDigit  = SUBSTRING(@alphabets, @currentYear % 36 + 1, 1);
    SET @prefix       = @firstDigit + @secondDigit;
    SET @dayCode      = RIGHT('000'   + CONVERT(VARCHAR, @dayOfYear),   3);
    SET @secondCode   = RIGHT('00000' + CONVERT(VARCHAR, @secondOfDay), 5);

    -- 取得該資料表的主鍵欄位名稱
    SELECT TOP 1
        @SIDRowName = STUFF((
            SELECT ', ' + c.name
            FROM sys.index_columns ic
            JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
            ORDER BY ic.key_ordinal
            FOR XML PATH('')
        ), 1, 2, '')
    FROM sys.indexes i
    WHERE i.object_id = OBJECT_ID(@TableName) AND i.index_id > 0;

    -- 確保不重複
    WHILE 1 = 1
    BEGIN
        SET @randomValue  = RIGHT('0000000000' + CAST(ABS(CAST(CAST(NEWID() AS BINARY(5)) AS BIGINT)) % 10000000000 AS VARCHAR(10)), 10);
        SET @ReturnSID    = @prefix + @dayCode + @secondCode + @randomValue;
        SET @sql          = N'SELECT @SIDRowNameOUT = ' + QUOTENAME(@SIDRowName)
                          + N' FROM '  + QUOTENAME(@TableName)
                          + N' WHERE ' + QUOTENAME(@SIDRowName) + N' = @ReturnSIDIN';
        SET @ParmDefinition = N'@ReturnSIDIN nvarchar(20), @SIDRowNameOUT nvarchar(20) OUTPUT';

        DECLARE @SIDRowNameOUT nvarchar(20);
        EXEC sp_executesql @sql, @ParmDefinition,
             @ReturnSIDIN    = @ReturnSID,
             @SIDRowNameOUT  = @SIDRowNameOUT OUTPUT;

        IF @SIDRowNameOUT IS NULL BREAK;
    END;
END
GO

-- ────────────────────────────────────────────────────────────
-- 5. 建立 SP：usp_AddLog（寫入/查詢執行日誌）
--    @_InBox_ReadID = 0：寫入並回傳該 GroupID 的所有日誌
-- ────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.usp_AddLog') IS NOT NULL
    DROP PROCEDURE [dbo].[usp_AddLog]
GO

CREATE PROCEDURE [dbo].[usp_AddLog]
(
    @_InBox_ReadID       tinyint,            -- 操作模式（0 = 寫入）
    @_InBox_SPNAME       nvarchar(120),       -- 呼叫端 SP 名稱
    @_InBox_GroupID      uniqueidentifier,    -- 同批次 GUID
    @_InBox_ExProgram    nvarchar(40),         -- 動作名稱
    @_InBox_ActionJSON   nvarchar(MAX),        -- 執行內容 JSON
    @_OutBox_ReturnValues nvarchar(MAX) OUTPUT -- 回傳 JSON
)
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @_StoredProgramsNAME nvarchar(100) = 'usp_AddLog';

    IF (@_InBox_ReadID = 0)
    BEGIN
        INSERT INTO MyOffice_ExcuteionLog
        (
            DeLog_StoredPrograms,
            DeLog_GroupID,
            DeLog_ExecutionProgram,
            DeLog_ExecutionInfo
        )
        VALUES
        (
            @_InBox_SPNAME,
            @_InBox_GroupID,
            @_InBox_ExProgram,
            @_InBox_ActionJSON
        );

        SET @_OutBox_ReturnValues =
        (
            SELECT TOP 100
                DeLog_AutoID           AS 'AutoID',
                DeLog_ExecutionProgram AS 'NAME',
                DeLog_ExecutionInfo    AS 'Action',
                DeLog_ExDateTime       AS 'DateTime'
            FROM MyOffice_ExcuteionLog WITH(NOLOCK)
            WHERE DeLog_GroupID = @_InBox_GroupID
            ORDER BY DeLog_AutoID
            FOR JSON PATH, ROOT('ProgramLog'), INCLUDE_NULL_VALUES
        );

        RETURN;
    END
END
GO

-- ────────────────────────────────────────────────────────────
-- 6. 插入測試資料（只在資料表為空時執行）
-- ────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM [dbo].[MyOffice_ACPD])
BEGIN
    DECLARE @sid1 nvarchar(20), @sid2 nvarchar(20), @sid3 nvarchar(20);
    EXEC dbo.NEWSID 'MyOffice_ACPD', @sid1 OUTPUT;
    WAITFOR DELAY '00:00:01';  -- 確保時間戳不同，SID 不重複
    EXEC dbo.NEWSID 'MyOffice_ACPD', @sid2 OUTPUT;
    WAITFOR DELAY '00:00:01';
    EXEC dbo.NEWSID 'MyOffice_ACPD', @sid3 OUTPUT;

    INSERT INTO [dbo].[MyOffice_ACPD]
        ([ACPD_SID], [ACPD_Cname], [ACPD_Ename], [ACPD_Sname], [ACPD_Email],
         [ACPD_Status], [ACPD_Stop], [ACPD_LoginID], [ACPD_LoginPWD], [ACPD_Memo],
         [ACPD_NowID], [ACPD_UPDID])
    VALUES
        (@sid1, N'張小明', 'Chang Xiao Ming', N'小明', 'ming@example.com',   1, 0, 'ming001',   'Pass@1234', N'測試帳號1', 'SYS', 'SYS'),
        (@sid2, N'李小華', 'Lee Xiao Hua',    N'小華', 'hua@example.com',    1, 0, 'hua002',    'Pass@1234', N'測試帳號2', 'SYS', 'SYS'),
        (@sid3, N'王大同', 'Wang Da Tong',    N'大同', 'datong@example.com', 0, 1, 'datong003', 'Pass@1234', N'已停用帳號', 'SYS', 'SYS');
END
GO
