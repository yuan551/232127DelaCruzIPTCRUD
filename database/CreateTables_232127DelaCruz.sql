-- Create database if it doesn't exist and create tables used by the ASP.NET MVC app
-- Change the database name below if you prefer a different name.
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'IPTCrudACT')
BEGIN
    CREATE DATABASE [IPTCrudACT];
END

USE [IPTCrudACT];

-- Course table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[232127DelaCruzCOURSE]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[232127DelaCruzCOURSE]
    (
        COURSEID INT IDENTITY(1,1) PRIMARY KEY,
        DESCRIPTION NVARCHAR(250) NOT NULL
    );
END

-- Student table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[232127DelaCruzSTUDENT]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[232127DelaCruzSTUDENT]
    (
        STUDENTID INT IDENTITY(1,1) PRIMARY KEY,
        FNAME NVARCHAR(100) NOT NULL,
        LNAME NVARCHAR(100) NOT NULL,
        MNAME NVARCHAR(100) NULL,
        BDAY DATE NOT NULL,
        GENDER NVARCHAR(20) NOT NULL,
        COURSEID INT NULL,
        CONSTRAINT FK_232127DelaCruzSTUDENT_COURSE FOREIGN KEY (COURSEID) REFERENCES [dbo].[232127DelaCruzCOURSE](COURSEID)
    );
END

-- User table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[232127DelaCruzUSER]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[232127DelaCruzUSER]
    (
        USERID INT IDENTITY(1,1) PRIMARY KEY,
        FNAME NVARCHAR(100) NOT NULL,
        LNAME NVARCHAR(100) NOT NULL,
        MNAME NVARCHAR(100) NULL,
        PASSWORD NVARCHAR(500) NULL
    );
END

-- Optional: create a sample user (user id will be generated)
IF NOT EXISTS (SELECT * FROM [dbo].[232127DelaCruzUSER])
BEGIN
    INSERT INTO [dbo].[232127DelaCruzUSER] (FNAME, LNAME, MNAME, PASSWORD)
    VALUES (N'Admin', N'User', N'', N'admin'); -- plaintext password for quick testing (the app will accept plain text during verification)
END
