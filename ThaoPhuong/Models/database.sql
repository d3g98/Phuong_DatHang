SET QUOTED_IDENTIFIER OFF;
GO
USE [THAOPHUONG];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__DANH__TDONHANGID__3A81B327]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DANH] DROP CONSTRAINT [FK__DANH__TDONHANGID__3A81B327];
GO
IF OBJECT_ID(N'[dbo].[FK__TDONHANG__DKHACH__37A5467C]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TDONHANG] DROP CONSTRAINT [FK__TDONHANG__DKHACH__37A5467C];
GO
IF OBJECT_ID(N'[dbo].[FK__TDONHANG__DQUAYI__36B12243]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TDONHANG] DROP CONSTRAINT [FK__TDONHANG__DQUAYI__36B12243];
GO
IF OBJECT_ID(N'[dbo].[FK__TDONHANG__DTRANG__29221CFB]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TDONHANG] DROP CONSTRAINT [FK__TDONHANG__DTRANG__29221CFB];
GO
IF OBJECT_ID(N'[dbo].[FK__TDONHANGC__TDONH__3D5E1FD2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TDONHANGCHITIET] DROP CONSTRAINT [FK__TDONHANGC__TDONH__3D5E1FD2];
GO
IF OBJECT_ID(N'[dbo].[FK__TTHANHTOA__DKHAC__7F2BE32F]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TTHANHTOAN] DROP CONSTRAINT [FK__TTHANHTOA__DKHAC__7F2BE32F];
GO
IF OBJECT_ID(N'[dbo].[FK__TTHANHTOA__TDONH__17036CC0]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TTHANHTOANCHITIET] DROP CONSTRAINT [FK__TTHANHTOA__TDONH__17036CC0];
GO
IF OBJECT_ID(N'[dbo].[FK__TTHANHTOA__TTHAN__160F4887]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TTHANHTOANCHITIET] DROP CONSTRAINT [FK__TTHANHTOA__TTHAN__160F4887];
GO
IF OBJECT_ID(N'[dbo].[FK__TTHUCHI__DKHACHH__5CD6CB2B]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TTHUCHI] DROP CONSTRAINT [FK__TTHUCHI__DKHACHH__5CD6CB2B];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[DANH]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DANH];
GO
IF OBJECT_ID(N'[dbo].[DKHACHHANG]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DKHACHHANG];
GO
IF OBJECT_ID(N'[dbo].[DQUAY]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DQUAY];
GO
IF OBJECT_ID(N'[dbo].[DTRANGTHAI]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DTRANGTHAI];
GO
IF OBJECT_ID(N'[dbo].[TDONHANG]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TDONHANG];
GO
IF OBJECT_ID(N'[dbo].[TDONHANGCHITIET]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TDONHANGCHITIET];
GO
IF OBJECT_ID(N'[dbo].[TTHANHTOAN]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TTHANHTOAN];
GO
IF OBJECT_ID(N'[dbo].[TTHANHTOANCHITIET]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TTHANHTOANCHITIET];
GO
IF OBJECT_ID(N'[dbo].[TTHUCHI]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TTHUCHI];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'DANH'
CREATE TABLE [dbo].[DANH] (
    [ID] varchar(36)  NOT NULL,
    [PATH] nvarchar(255)  NOT NULL,
    [TDONHANGID] varchar(36)  NULL
);
GO

-- Creating table 'DKHACHHANG'
CREATE TABLE [dbo].[DKHACHHANG] (
    [ID] varchar(36)  NOT NULL,
    [NAME] nvarchar(255)  NOT NULL,
    [USERNAME] varchar(255)  NULL,
    [PASSWORD] varchar(255)  NULL,
    [DIENTHOAI] varchar(255)  NULL,
    [DIACHI] nvarchar(255)  NULL,
    [ISADMIN] int  NULL,
    [ISACTIVE] int  NULL
);
GO

-- Creating table 'DQUAY'
CREATE TABLE [dbo].[DQUAY] (
    [ID] varchar(36)  NOT NULL,
    [NAME] nvarchar(255)  NOT NULL,
    [POSITION] int  NULL,
    [GIAODICH] int  NULL
);
GO

-- Creating table 'DTRANGTHAI'
CREATE TABLE [dbo].[DTRANGTHAI] (
    [ID] varchar(36)  NOT NULL,
    [NAME] nvarchar(255)  NULL,
    [BASIC] int  NULL
);
GO

-- Creating table 'TDONHANG'
CREATE TABLE [dbo].[TDONHANG] (
    [ID] varchar(36)  NOT NULL,
    [NAME] nvarchar(255)  NULL,
    [LOAI] int  NULL,
    [DQUAYID] varchar(36)  NULL,
    [DKHACHHANGID] varchar(36)  NULL,
    [TIMECREATED] datetime  NULL,
    [TIMEUPDATED] datetime  NULL,
    [TIENDUKIEN] decimal(18,2)  NULL,
    [TIENDANHAT] decimal(18,2)  NULL,
    [TIENCONG] decimal(18,2)  NULL,
    [TONGCONG] decimal(18,2)  NULL,
    [NOTE] nvarchar(255)  NULL,
    [TENSP] nvarchar(255)  NULL,
    [DTRANGTHAIID] varchar(36)  NULL,
    [SLDANHAT] int  NULL
);
GO

-- Creating table 'TDONHANGCHITIET'
CREATE TABLE [dbo].[TDONHANGCHITIET] (
    [ID] varchar(36)  NOT NULL,
    [TDONHANGID] varchar(36)  NULL,
    [TENHANG] nvarchar(255)  NULL,
    [SOLUONG] int  NULL,
    [SIZE] nvarchar(255)  NULL,
    [NOTE] nvarchar(max)  NULL,
    [SOLUONGDANHAT] int  NULL
);
GO

-- Creating table 'TTHANHTOAN'
CREATE TABLE [dbo].[TTHANHTOAN] (
    [ID] varchar(36)  NOT NULL,
    [NAME] nvarchar(255)  NULL,
    [DKHACHHANGID] varchar(36)  NULL,
    [TIENHANG] decimal(18,2)  NULL,
    [PHUPHI] decimal(18,2)  NULL,
    [TONGCONG] decimal(18,2)  NULL,
    [TIENTHANHTOAN] decimal(18,2)  NULL,
    [KETTHUC] int  NULL,
    [NOTE] nvarchar(255)  NULL,
    [TIMECREATED] datetime  NULL,
    [TIMEUPDATED] datetime  NULL
);
GO

-- Creating table 'TTHANHTOANCHITIET'
CREATE TABLE [dbo].[TTHANHTOANCHITIET] (
    [ID] varchar(36)  NOT NULL,
    [TTHANHTOANID] varchar(36)  NULL,
    [TDONHANGID] varchar(36)  NULL,
    [NOTE] nvarchar(max)  NULL
);
GO

-- Creating table 'TTHUCHI'
CREATE TABLE [dbo].[TTHUCHI] (
    [ID] varchar(36)  NOT NULL,
    [LOAI] int  NULL,
    [DKHACHHANGID] varchar(36)  NULL,
    [THU] decimal(18,2)  NULL,
    [CHI] decimal(18,2)  NULL,
    [NOTE] nvarchar(255)  NULL,
    [TIMECREATED] datetime  NULL,
    [TIMEUPDATED] datetime  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'DANH'
ALTER TABLE [dbo].[DANH]
ADD CONSTRAINT [PK_DANH]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'DKHACHHANG'
ALTER TABLE [dbo].[DKHACHHANG]
ADD CONSTRAINT [PK_DKHACHHANG]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'DQUAY'
ALTER TABLE [dbo].[DQUAY]
ADD CONSTRAINT [PK_DQUAY]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'DTRANGTHAI'
ALTER TABLE [dbo].[DTRANGTHAI]
ADD CONSTRAINT [PK_DTRANGTHAI]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'TDONHANG'
ALTER TABLE [dbo].[TDONHANG]
ADD CONSTRAINT [PK_TDONHANG]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'TDONHANGCHITIET'
ALTER TABLE [dbo].[TDONHANGCHITIET]
ADD CONSTRAINT [PK_TDONHANGCHITIET]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'TTHANHTOAN'
ALTER TABLE [dbo].[TTHANHTOAN]
ADD CONSTRAINT [PK_TTHANHTOAN]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'TTHANHTOANCHITIET'
ALTER TABLE [dbo].[TTHANHTOANCHITIET]
ADD CONSTRAINT [PK_TTHANHTOANCHITIET]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'TTHUCHI'
ALTER TABLE [dbo].[TTHUCHI]
ADD CONSTRAINT [PK_TTHUCHI]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TDONHANGID] in table 'DANHs'
ALTER TABLE [dbo].[DANH]
ADD CONSTRAINT [FK__DANH__TDONHANGID__22AA2996]
    FOREIGN KEY ([TDONHANGID])
    REFERENCES [dbo].[TDONHANG]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__DANH__TDONHANGID__22AA2996'
CREATE INDEX [IX_FK__DANH__TDONHANGID__22AA2996]
ON [dbo].[DANH]
    ([TDONHANGID]);
GO

-- Creating foreign key on [DKHACHHANGID] in table 'TDONHANGs'
ALTER TABLE [dbo].[TDONHANG]
ADD CONSTRAINT [FK__TDONHANG__DKHACH__239E4DCF]
    FOREIGN KEY ([DKHACHHANGID])
    REFERENCES [dbo].[DKHACHHANG]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__TDONHANG__DKHACH__239E4DCF'
CREATE INDEX [IX_FK__TDONHANG__DKHACH__239E4DCF]
ON [dbo].[TDONHANG]
    ([DKHACHHANGID]);
GO

-- Creating foreign key on [DKHACHHANGID] in table 'TTHANHTOANs'
ALTER TABLE [dbo].[TTHANHTOAN]
ADD CONSTRAINT [FK__TTHANHTOA__DKHAC__276EDEB3]
    FOREIGN KEY ([DKHACHHANGID])
    REFERENCES [dbo].[DKHACHHANG]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__TTHANHTOA__DKHAC__276EDEB3'
CREATE INDEX [IX_FK__TTHANHTOA__DKHAC__276EDEB3]
ON [dbo].[TTHANHTOAN]
    ([DKHACHHANGID]);
GO

-- Creating foreign key on [DKHACHHANGID] in table 'TTHUCHIs'
ALTER TABLE [dbo].[TTHUCHI]
ADD CONSTRAINT [FK__TTHUCHI__DKHACHH__2A4B4B5E]
    FOREIGN KEY ([DKHACHHANGID])
    REFERENCES [dbo].[DKHACHHANG]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__TTHUCHI__DKHACHH__2A4B4B5E'
CREATE INDEX [IX_FK__TTHUCHI__DKHACHH__2A4B4B5E]
ON [dbo].[TTHUCHI]
    ([DKHACHHANGID]);
GO

-- Creating foreign key on [DQUAYID] in table 'TDONHANGs'
ALTER TABLE [dbo].[TDONHANG]
ADD CONSTRAINT [FK__TDONHANG__DQUAYI__24927208]
    FOREIGN KEY ([DQUAYID])
    REFERENCES [dbo].[DQUAY]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__TDONHANG__DQUAYI__24927208'
CREATE INDEX [IX_FK__TDONHANG__DQUAYI__24927208]
ON [dbo].[TDONHANG]
    ([DQUAYID]);
GO

-- Creating foreign key on [DTRANGTHAIID] in table 'TDONHANGs'
ALTER TABLE [dbo].[TDONHANG]
ADD CONSTRAINT [FK__TDONHANG__DTRANG__25869641]
    FOREIGN KEY ([DTRANGTHAIID])
    REFERENCES [dbo].[DTRANGTHAI]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__TDONHANG__DTRANG__25869641'
CREATE INDEX [IX_FK__TDONHANG__DTRANG__25869641]
ON [dbo].[TDONHANG]
    ([DTRANGTHAIID]);
GO

-- Creating foreign key on [TDONHANGID] in table 'TDONHANGCHITIETs'
ALTER TABLE [dbo].[TDONHANGCHITIET]
ADD CONSTRAINT [FK__TDONHANGC__TDONH__267ABA7A]
    FOREIGN KEY ([TDONHANGID])
    REFERENCES [dbo].[TDONHANG]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__TDONHANGC__TDONH__267ABA7A'
CREATE INDEX [IX_FK__TDONHANGC__TDONH__267ABA7A]
ON [dbo].[TDONHANGCHITIET]
    ([TDONHANGID]);
GO

-- Creating foreign key on [TDONHANGID] in table 'TTHANHTOANCHITIETs'
ALTER TABLE [dbo].[TTHANHTOANCHITIET]
ADD CONSTRAINT [FK__TTHANHTOA__TDONH__286302EC]
    FOREIGN KEY ([TDONHANGID])
    REFERENCES [dbo].[TDONHANG]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__TTHANHTOA__TDONH__286302EC'
CREATE INDEX [IX_FK__TTHANHTOA__TDONH__286302EC]
ON [dbo].[TTHANHTOANCHITIET]
    ([TDONHANGID]);
GO

-- Creating foreign key on [TTHANHTOANID] in table 'TTHANHTOANCHITIETs'
ALTER TABLE [dbo].[TTHANHTOANCHITIET]
ADD CONSTRAINT [FK__TTHANHTOA__TTHAN__29572725]
    FOREIGN KEY ([TTHANHTOANID])
    REFERENCES [dbo].[TTHANHTOAN]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__TTHANHTOA__TTHAN__29572725'
CREATE INDEX [IX_FK__TTHANHTOA__TTHAN__29572725]
ON [dbo].[TTHANHTOANCHITIET]
    ([TTHANHTOANID]);
GO

CREATE TABLE SCONFIG
(
 THONGBAOKHACHHANG NVARCHAR(MAX)
)

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------