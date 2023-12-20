﻿IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Announce] (
    [ID] int NOT NULL IDENTITY,
    [Title] nvarchar(250) NULL,
    [Content] nvarchar(max) NULL,
    [Language] nvarchar(50) NULL,
    [Sort] int NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [CreatedAt] datetime2 NULL,
    [UpdateAt] datetime2 NULL,
    CONSTRAINT [PK_Announce] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Banner] (
    [ID] int NOT NULL IDENTITY,
    [Sort] int NULL,
    [Language] nvarchar(50) NULL,
    [ImagePath] nvarchar(500) NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Banner] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Group] (
    [ID] int NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    [ImagePath] nvarchar(500) NULL,
    [DefaultTab] bit NOT NULL,
    CONSTRAINT [PK_Group] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Macquee] (
    [ID] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NULL,
    [Content] nvarchar(200) NULL,
    [Sort] int NULL,
    [Language] nvarchar(50) NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [IsPeriod] bit NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Macquee] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Maintenance] (
    [ID] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NULL,
    [Content] nvarchar(200) NULL,
    [Sort] int NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [IsPeriod] bit NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Maintenance] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [News] (
    [ID] int NOT NULL IDENTITY,
    [Title] nvarchar(250) NULL,
    [Content] nvarchar(max) NULL,
    [Language] nvarchar(50) NULL,
    [ImagePath] nvarchar(500) NULL,
    [Sort] int NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_News] PRIMARY KEY ([ID])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230629022224_init', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Group] ADD [Language] nvarchar(50) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230629043014_groupadddefault', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Group] ADD [CreatedAt] datetime2 NULL;
GO

ALTER TABLE [Group] ADD [UpdatedAt] datetime2 NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230629043414_addGroupcolumns', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Maintenance] ADD [Language] nvarchar(50) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230629052509_addMaintenance', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Maintenance]') AND [c].[name] = N'Name');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Maintenance] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Maintenance] DROP COLUMN [Name];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Macquee]') AND [c].[name] = N'Name');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Macquee] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Macquee] DROP COLUMN [Name];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230629120121_removeName', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Group] ADD [grp] nvarchar(20) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230629145716_fixgroups', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230629153724_fixmarquee', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230629154129_add', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230629154612_reinit', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Marquee] (
    [ID] int NOT NULL IDENTITY,
    [Content] nvarchar(200) NULL,
    [Sort] int NULL,
    [Language] nvarchar(50) NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [IsPeriod] bit NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Marquee] PRIMARY KEY ([ID])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230629154741_addmarquee', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Group] ADD [Sort] int NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230630025737_addsort', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Logo] (
    [ID] int NOT NULL IDENTITY,
    [DesktopImgPath] nvarchar(500) NULL,
    [MobileImgPath] nvarchar(500) NULL,
    [PWAImgPath] nvarchar(500) NULL,
    CONSTRAINT [PK_Logo] PRIMARY KEY ([ID])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230701025119_addLogo', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Announce] ADD [IsPeriod] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230701175304_addannouceisperiod', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Group]') AND [c].[name] = N'DefaultTab');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Group] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Group] ALTER COLUMN [DefaultTab] bit NULL;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Announce]') AND [c].[name] = N'IsPeriod');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Announce] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Announce] ALTER COLUMN [IsPeriod] bit NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230701175445_addnullable', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [Macquee];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230709030257_addUserDetail', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [User] (
    [ID] int NOT NULL IDENTITY,
    [Email] nvarchar(250) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [PasswordSalt] nvarchar(max) NOT NULL,
    [Phone] nvarchar(50) NOT NULL,
    [Role] nvarchar(50) NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([ID])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230709064728_amendUser', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [User] ADD [Remark] nvarchar(50) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230709120858_addUserRemark', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Policy] (
    [ID] int NOT NULL IDENTITY,
    [Content] nvarchar(max) NULL,
    [Language] nvarchar(50) NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Policy] PRIMARY KEY ([ID])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230711053157_addPolicy', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Policy] ADD [ImagePath] nvarchar(500) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230711054036_updatePolicy', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [Policy];
GO

CREATE TABLE [DepositPolicy] (
    [ID] int NOT NULL IDENTITY,
    [Content] nvarchar(max) NULL,
    [Language] nvarchar(50) NULL,
    [ImagePath] nvarchar(500) NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_DepositPolicy] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [WithdrawPolicy] (
    [ID] int NOT NULL IDENTITY,
    [Content] nvarchar(max) NULL,
    [Language] nvarchar(50) NULL,
    [ImagePath] nvarchar(500) NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_WithdrawPolicy] PRIMARY KEY ([ID])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230718024429_addWithdrawDeposit', N'6.0.18');
GO

COMMIT;
GO
