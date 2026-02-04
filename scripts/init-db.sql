-- Initialize Redarbor Database
-- This script creates the database and initial schema

USE master;
GO

-- Create database if not exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RedarborDb')
BEGIN
    CREATE DATABASE RedarborDb;
END
GO

USE RedarborDb;
GO

-- Create Employees table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employees]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Employees] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [Email] NVARCHAR(256) NOT NULL,
        [Fax] NVARCHAR(20) NULL,
        [Name] NVARCHAR(100) NULL,
        [LastLogin] DATETIME2 NULL,
        [Password] NVARCHAR(500) NOT NULL,
        [PortalId] INT NOT NULL,
        [RoleId] INT NOT NULL,
        [StatusId] INT NOT NULL,
        [Telephone] NVARCHAR(20) NULL,
        [Username] NVARCHAR(50) NOT NULL,
        [CreatedOn] DATETIME2 NOT NULL,
        [UpdatedOn] DATETIME2 NULL,
        [DeletedOn] DATETIME2 NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    -- Create indexes
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Employees_Email] ON [dbo].[Employees]([Email] ASC);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Employees_Username] ON [dbo].[Employees]([Username] ASC);
    CREATE NONCLUSTERED INDEX [IX_Employees_CompanyId] ON [dbo].[Employees]([CompanyId] ASC);
    CREATE NONCLUSTERED INDEX [IX_Employees_StatusId] ON [dbo].[Employees]([StatusId] ASC);
    CREATE NONCLUSTERED INDEX [IX_Employees_DeletedOn] ON [dbo].[Employees]([DeletedOn] ASC);
END
GO

-- Insert test admin user (password: Admin@123456)
-- Hash is generated using PBKDF2 with SHA256
IF NOT EXISTS (SELECT * FROM [dbo].[Employees] WHERE [Username] = 'admin')
BEGIN
    INSERT INTO [dbo].[Employees]
        ([CompanyId], [Email], [Fax], [Name], [Password], [PortalId], [RoleId], [StatusId], [Telephone], [Username], [CreatedOn])
    VALUES
        (1, 'admin@redarbor.com', NULL, 'System Administrator',
         'YWRtaW4=' -- Note: This is a placeholder, actual hash should be generated
        , 1, 1, 1, NULL, 'admin', GETUTCDATE());
END
GO

PRINT 'Database initialization completed successfully.';
GO
