IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
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
    [IsPeriod] bit NULL,
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

CREATE TABLE [DepositPolicy] (
    [ID] int NOT NULL IDENTITY,
    [Content] nvarchar(max) NULL,
    [Language] nvarchar(50) NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_DepositPolicy] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Group] (
    [ID] int NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    [ImagePath] nvarchar(500) NULL,
    [Language] nvarchar(50) NULL,
    [grp] nvarchar(20) NULL,
    [DefaultTab] bit NULL,
    [Sort] int NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Group] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Logo] (
    [ID] int NOT NULL IDENTITY,
    [DesktopImgPath] nvarchar(500) NULL,
    [MobileImgPath] nvarchar(500) NULL,
    [PWAImgPath] nvarchar(500) NULL,
    CONSTRAINT [PK_Logo] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Maintenance] (
    [ID] int NOT NULL IDENTITY,
    [Content] nvarchar(200) NULL,
    [Language] nvarchar(50) NULL,
    [Sort] int NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [IsPeriod] bit NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Maintenance] PRIMARY KEY ([ID])
);
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

CREATE TABLE [User] (
    [ID] int NOT NULL IDENTITY,
    [Email] nvarchar(250) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [PasswordSalt] nvarchar(max) NOT NULL,
    [Phone] nvarchar(50) NOT NULL,
    [Role] nvarchar(50) NULL,
    [Remark] nvarchar(50) NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [WithdrawPolicy] (
    [ID] int NOT NULL IDENTITY,
    [Content] nvarchar(max) NULL,
    [Language] nvarchar(50) NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_WithdrawPolicy] PRIMARY KEY ([ID])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230806131252_addMyanmar', N'6.0.18');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230806132010_changeClass', N'6.0.18');
GO

COMMIT;
GO

