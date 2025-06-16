-- =============================================
-- Financial Project Version 3 - Database Script
-- Created by: dawnmoriaty
-- Date: 2025-06-13
-- Description: Complete database setup for Personal Finance Management System
-- =============================================

-- Create Database
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'FinancialProjectDB')
BEGIN
    ALTER DATABASE FinancialProjectDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE FinancialProjectDB;
END
GO

CREATE DATABASE FinancialProjectDB;
GO

USE FinancialProjectDB;
GO

-- =============================================
-- Table: Users
-- Description: Store user account information
-- =============================================
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NULL,
    AvatarPath NVARCHAR(255) NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'user',
    IsBlocked BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT CK_Users_Role CHECK (Role IN ('admin', 'user')),
    CONSTRAINT CK_Users_Email CHECK (Email LIKE '%@%.%')
);
GO

-- =============================================
-- Table: Categories
-- Description: Store income/expense categories created by users
-- =============================================
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Type NVARCHAR(10) NOT NULL,
    IconPath NVARCHAR(255) NULL,
    UserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- Foreign Key
    CONSTRAINT FK_Categories_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    
    -- Constraints
    CONSTRAINT CK_Categories_Type CHECK (Type IN ('income', 'expense')),
    CONSTRAINT UQ_Categories_UserName UNIQUE (UserId, Name)
);
GO

-- =============================================
-- Table: Transactions
-- Description: Store financial transactions (income/expense)
-- =============================================
CREATE TABLE Transactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Description NVARCHAR(200) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    TransactionDate DATE NOT NULL,
    CategoryId INT NOT NULL,
    UserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- Foreign Keys
    CONSTRAINT FK_Transactions_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Transactions_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    
    -- Constraints
    CONSTRAINT CK_Transactions_Amount CHECK (Amount > 0)
);
GO

-- =============================================
-- Indexes for Performance Optimization
-- =============================================

-- Users table indexes
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_Users_Role ON Users(Role);

-- Categories table indexes
CREATE NONCLUSTERED INDEX IX_Categories_UserId ON Categories(UserId);
CREATE NONCLUSTERED INDEX IX_Categories_Type ON Categories(Type);
CREATE NONCLUSTERED INDEX IX_Categories_UserId_Type ON Categories(UserId, Type);

-- Transactions table indexes
CREATE NONCLUSTERED INDEX IX_Transactions_UserId ON Transactions(UserId);
CREATE NONCLUSTERED INDEX IX_Transactions_CategoryId ON Transactions(CategoryId);
CREATE NONCLUSTERED INDEX IX_Transactions_TransactionDate ON Transactions(TransactionDate);
CREATE NONCLUSTERED INDEX IX_Transactions_UserId_TransactionDate ON Transactions(UserId, TransactionDate DESC);
CREATE NONCLUSTERED INDEX IX_Transactions_UserId_CategoryId ON Transactions(UserId, CategoryId);

-- =============================================
-- Triggers for Audit Trail
-- =============================================

-- Update trigger for Transactions table
CREATE TRIGGER TR_Transactions_UpdatedAt
ON Transactions
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE t
    SET UpdatedAt = GETDATE()
    FROM Transactions t
    INNER JOIN inserted i ON t.Id = i.Id;
END
GO

-- =============================================
-- Sample Data for Testing
-- =============================================

-- Insert Admin User (Password: Admin123!)
-- Note: In real application, this will be hashed using PBKDF2
INSERT INTO Users (Username, Email, PasswordHash, FullName, Role, CreatedAt)
VALUES (
    'admin',
    'admin@financialapp.com',
    'AQAAAAEAACcQAAAAEBr8nHJZ7VYd4Nfd8rR5dDhI0xKz9CJ2l1nFkPr3sM6e4vW7qY8tH9bC2dGxR6cP5w==', -- This should be actual PBKDF2 hash
    'System Administrator',
    'admin',
    GETDATE()
);

-- Insert Test User (Password: User123!)
INSERT INTO Users (Username, Email, PasswordHash, FullName, Role, CreatedAt)
VALUES (
    'testuser',
    'test@example.com',
    'AQAAAAEAACcQAAAAEAb7mGJY6UXc3Mec7qQ4cCgH9wJy8BH1k0mEjOq2rL5d3uV6pX7sG8aB1cFwQ5bO4v==', -- This should be actual PBKDF2 hash
    'Test User',
    'user',
    GETDATE()
);

-- Get UserId for sample data
DECLARE @TestUserId INT = (SELECT Id FROM Users WHERE Username = 'testuser');

-- Insert Sample Categories
INSERT INTO Categories (Name, Type, UserId) VALUES
('Lương', 'income', @TestUserId),
('Thưởng', 'income', @TestUserId),
('Đầu tư', 'income', @TestUserId),
('Ăn uống', 'expense', @TestUserId),
('Xăng xe', 'expense', @TestUserId),
('Mua sắm', 'expense', @TestUserId),
('Giải trí', 'expense', @TestUserId),
('Học phí', 'expense', @TestUserId),
('Y tế', 'expense', @TestUserId),
('Hóa đơn', 'expense', @TestUserId);

-- Insert Sample Transactions
DECLARE @LuongId INT = (SELECT Id FROM Categories WHERE Name = 'Lương' AND UserId = @TestUserId);
DECLARE @AnUongId INT = (SELECT Id FROM Categories WHERE Name = 'Ăn uống' AND UserId = @TestUserId);
DECLARE @XangXeId INT = (SELECT Id FROM Categories WHERE Name = 'Xăng xe' AND UserId = @TestUserId);
DECLARE @MuaSamId INT = (SELECT Id FROM Categories WHERE Name = 'Mua sắm' AND UserId = @TestUserId);
DECLARE @GiaiTriId INT = (SELECT Id FROM Categories WHERE Name = 'Giải trí' AND UserId = @TestUserId);

INSERT INTO Transactions (Description, Amount, TransactionDate, CategoryId, UserId) VALUES
('Lương tháng 6/2025', 15000000.00, '2025-06-01', @LuongId, @TestUserId),
('Ăn trưa', 120000.00, '2025-06-13', @AnUongId, @TestUserId),
('Xăng xe máy', 150000.00, '2025-06-12', @XangXeId, @TestUserId),
('Mua áo', 500000.00, '2025-06-11', @MuaSamId, @TestUserId),
('Xem phim', 200000.00, '2025-06-10', @GiaiTriId, @TestUserId),
('Ăn sáng', 50000.00, '2025-06-13', @AnUongId, @TestUserId),
('Cà phê', 35000.00, '2025-06-12', @AnUongId, @TestUserId),
('Mua sách', 300000.00, '2025-06-09', @MuaSamId, @TestUserId);

-- =============================================
-- Views for Reporting
-- =============================================

-- View: User Transaction Summary
CREATE VIEW VW_UserTransactionSummary
AS
SELECT 
    u.Id AS UserId,
    u.Username,
    u.FullName,
    COUNT(t.Id) AS TotalTransactions,
    ISNULL(SUM(CASE WHEN c.Type = 'income' THEN t.Amount ELSE 0 END), 0) AS TotalIncome,
    ISNULL(SUM(CASE WHEN c.Type = 'expense' THEN t.Amount ELSE 0 END), 0) AS TotalExpense,
    ISNULL(SUM(CASE WHEN c.Type = 'income' THEN t.Amount ELSE -t.Amount END), 0) AS Balance
FROM Users u
LEFT JOIN Transactions t ON u.Id = t.UserId
LEFT JOIN Categories c ON t.CategoryId = c.Id
GROUP BY u.Id, u.Username, u.FullName;
GO

-- View: Monthly Transaction Summary
CREATE VIEW VW_MonthlyTransactionSummary
AS
SELECT 
    t.UserId,
    YEAR(t.TransactionDate) AS Year,
    MONTH(t.TransactionDate) AS Month,
    DATENAME(MONTH, t.TransactionDate) + ' ' + CAST(YEAR(t.TransactionDate) AS VARCHAR(4)) AS MonthYear,
    COUNT(t.Id) AS TransactionCount,
    ISNULL(SUM(CASE WHEN c.Type = 'income' THEN t.Amount ELSE 0 END), 0) AS MonthlyIncome,
    ISNULL(SUM(CASE WHEN c.Type = 'expense' THEN t.Amount ELSE 0 END), 0) AS MonthlyExpense,
    ISNULL(SUM(CASE WHEN c.Type = 'income' THEN t.Amount ELSE -t.Amount END), 0) AS MonthlyBalance
FROM Transactions t
INNER JOIN Categories c ON t.CategoryId = c.Id
GROUP BY t.UserId, YEAR(t.TransactionDate), MONTH(t.TransactionDate), DATENAME(MONTH, t.TransactionDate);
GO

-- View: Category Summary
CREATE VIEW VW_CategorySummary
AS
SELECT 
    c.Id AS CategoryId,
    c.Name AS CategoryName,
    c.Type AS CategoryType,
    c.UserId,
    COUNT(t.Id) AS TransactionCount,
    ISNULL(SUM(t.Amount), 0) AS TotalAmount,
    ISNULL(AVG(t.Amount), 0) AS AverageAmount,
    MAX(t.TransactionDate) AS LastTransactionDate
FROM Categories c
LEFT JOIN Transactions t ON c.Id = t.CategoryId
GROUP BY c.Id, c.Name, c.Type, c.UserId;
GO

-- =============================================
-- Stored Procedures for Common Operations
-- =============================================

-- Procedure: Get User Dashboard Data
CREATE PROCEDURE SP_GetUserDashboard
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Overall Summary
    SELECT 
        ISNULL(SUM(CASE WHEN c.Type = 'income' THEN t.Amount ELSE 0 END), 0) AS TotalIncome,
        ISNULL(SUM(CASE WHEN c.Type = 'expense' THEN t.Amount ELSE 0 END), 0) AS TotalExpense,
        ISNULL(SUM(CASE WHEN c.Type = 'income' THEN t.Amount ELSE -t.Amount END), 0) AS Balance
    FROM Transactions t
    INNER JOIN Categories c ON t.CategoryId = c.Id
    WHERE t.UserId = @UserId;
    
    -- Monthly Summary (Current Month)
    DECLARE @StartOfMonth DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
    DECLARE @EndOfMonth DATE = EOMONTH(GETDATE());
    
    SELECT 
        ISNULL(SUM(CASE WHEN c.Type = 'income' THEN t.Amount ELSE 0 END), 0) AS MonthlyIncome,
        ISNULL(SUM(CASE WHEN c.Type = 'expense' THEN t.Amount ELSE 0 END), 0) AS MonthlyExpense
    FROM Transactions t
    INNER JOIN Categories c ON t.CategoryId = c.Id
    WHERE t.UserId = @UserId 
      AND t.TransactionDate >= @StartOfMonth 
      AND t.TransactionDate <= @EndOfMonth;
    
    -- Recent Transactions (Last 8)
    SELECT TOP 8
        t.Id,
        t.Description,
        t.Amount,
        t.TransactionDate,
        c.Name AS CategoryName,
        c.Type AS CategoryType
    FROM Transactions t
    INNER JOIN Categories c ON t.CategoryId = c.Id
    WHERE t.UserId = @UserId
    ORDER BY t.TransactionDate DESC, t.CreatedAt DESC;
END
GO

-- Procedure: Get Monthly Chart Data
CREATE PROCEDURE SP_GetMonthlyChartData
    @UserId INT,
    @MonthsBack INT = 6
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH MonthSeries AS (
        SELECT 
            DATEADD(MONTH, -n.Number, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS MonthStart
        FROM (
            SELECT 0 AS Number UNION SELECT 1 UNION SELECT 2 UNION 
            SELECT 3 UNION SELECT 4 UNION SELECT 5 UNION 
            SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION 
            SELECT 9 UNION SELECT 10 UNION SELECT 11
        ) n
        WHERE n.Number < @MonthsBack
    )
    SELECT 
        CONCAT('T', MONTH(ms.MonthStart), '/', YEAR(ms.MonthStart)) AS Label,
        ISNULL(SUM(CASE WHEN c.Type = 'income' THEN t.Amount ELSE 0 END), 0) AS Income,
        ISNULL(SUM(CASE WHEN c.Type = 'expense' THEN t.Amount ELSE 0 END), 0) AS Expense
    FROM MonthSeries ms
    LEFT JOIN Transactions t ON t.UserId = @UserId 
        AND t.TransactionDate >= ms.MonthStart 
        AND t.TransactionDate < DATEADD(MONTH, 1, ms.MonthStart)
    LEFT JOIN Categories c ON t.CategoryId = c.Id
    GROUP BY ms.MonthStart
    ORDER BY ms.MonthStart;
END
GO

-- Procedure: Get Category Chart Data
CREATE PROCEDURE SP_GetCategoryChartData
    @UserId INT,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @StartDate IS NULL SET @StartDate = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
    IF @EndDate IS NULL SET @EndDate = EOMONTH(GETDATE());
    
    SELECT TOP 5
        c.Name,
        SUM(t.Amount) AS Amount
    FROM Transactions t
    INNER JOIN Categories c ON t.CategoryId = c.Id
    WHERE t.UserId = @UserId 
      AND c.Type = 'expense'
      AND t.TransactionDate >= @StartDate 
      AND t.TransactionDate <= @EndDate
    GROUP BY c.Id, c.Name
    ORDER BY SUM(t.Amount) DESC;
END
GO

-- =============================================
-- Security: Create Application User
-- =============================================

-- Create login for application
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'FinancialAppUser')
BEGIN
    CREATE LOGIN FinancialAppUser WITH PASSWORD = 'FinancialApp2025!@#';
END
GO

-- Create database user
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'FinancialAppUser')
BEGIN
    CREATE USER FinancialAppUser FOR LOGIN FinancialAppUser;
END
GO

-- Grant permissions
ALTER ROLE db_datareader ADD MEMBER FinancialAppUser;
ALTER ROLE db_datawriter ADD MEMBER FinancialAppUser;
ALTER ROLE db_ddladmin ADD MEMBER FinancialAppUser;
GO

-- Grant execute permissions on stored procedures
GRANT EXECUTE ON SP_GetUserDashboard TO FinancialAppUser;
GRANT EXECUTE ON SP_GetMonthlyChartData TO FinancialAppUser;
GRANT EXECUTE ON SP_GetCategoryChartData TO FinancialAppUser;
GO

-- Grant select permissions on views
GRANT SELECT ON VW_UserTransactionSummary TO FinancialAppUser;
GRANT SELECT ON VW_MonthlyTransactionSummary TO FinancialAppUser;
GRANT SELECT ON VW_CategorySummary TO FinancialAppUser;
GO

-- =============================================
-- Final Status Check
-- =============================================

PRINT 'Database setup completed successfully!';
PRINT '';
PRINT 'Tables created:';
PRINT '- Users (' + CAST((SELECT COUNT(*) FROM Users) AS VARCHAR) + ' records)';
PRINT '- Categories (' + CAST((SELECT COUNT(*) FROM Categories) AS VARCHAR) + ' records)';
PRINT '- Transactions (' + CAST((SELECT COUNT(*) FROM Transactions) AS VARCHAR) + ' records)';
PRINT '';
PRINT 'Views created: 3';
PRINT 'Stored Procedures created: 3';
PRINT 'Indexes created: 12';
PRINT '';
PRINT 'Test admin account: admin / Admin123!';
PRINT 'Test user account: testuser / User123!';
PRINT '';
PRINT 'Database is ready for use!';
GO

