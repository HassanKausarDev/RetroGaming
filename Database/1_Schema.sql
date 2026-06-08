IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'RetroGamingDB')
BEGIN
    CREATE DATABASE RetroGamingDB;
END
GO

USE RetroGamingDB;
GO

-- Manufacturers Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Manufacturers')
BEGIN
    CREATE TABLE Manufacturers (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        Name         NVARCHAR(100) NOT NULL,
        Country      NVARCHAR(100) NOT NULL,
        City         NVARCHAR(100) NOT NULL,
        FoundedYear  INT           NOT NULL
            CONSTRAINT CHK_Manufacturers_FoundedYear CHECK (FoundedYear >= 1800 AND FoundedYear <= 2100),
        Latitude     DECIMAL(9,6)  NULL
            CONSTRAINT CHK_Manufacturers_Latitude    CHECK (Latitude  BETWEEN -90  AND 90),
        Longitude    DECIMAL(9,6)  NULL
            CONSTRAINT CHK_Manufacturers_Longitude   CHECK (Longitude BETWEEN -180 AND 180),
        CreatedAt    DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt    DATETIME2     NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT UQ_Manufacturers_Name UNIQUE (Name)
    );
END
GO

-- Consoles Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Consoles')
BEGIN
    CREATE TABLE Consoles (
        Id                INT IDENTITY(1,1) PRIMARY KEY,
        ManufacturerId    INT            NOT NULL,
        Name              NVARCHAR(100)  NOT NULL,
        ReleaseYear       INT            NOT NULL
            CONSTRAINT CHK_Consoles_ReleaseYear  CHECK (ReleaseYear >= 1800 AND ReleaseYear <= 2100),
        Generation        INT            NOT NULL
            CONSTRAINT CHK_Consoles_Generation   CHECK (Generation >= 1),
        UnitsSoldMillions DECIMAL(10,2)  NULL
            CONSTRAINT CHK_Consoles_UnitsSold    CHECK (UnitsSoldMillions >= 0),
        IsDiscontinued    BIT            NOT NULL DEFAULT 1,
        CreatedAt         DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt         DATETIME2      NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT FK_Consoles_Manufacturer
            FOREIGN KEY (ManufacturerId) REFERENCES Manufacturers(Id),

        CONSTRAINT UQ_Console_Name_Per_Manufacturer UNIQUE (ManufacturerId, Name)
    );
END
GO


-- Auto-update UpdatedAt on Manufacturers
CREATE OR ALTER TRIGGER trg_Manufacturers_UpdatedAt
ON Manufacturers
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Manufacturers
    SET    UpdatedAt = GETUTCDATE()
    FROM   Manufacturers m
    INNER JOIN inserted i ON m.Id = i.Id;
END;
GO

-- Auto-update UpdatedAt on Consoles
CREATE OR ALTER TRIGGER trg_Consoles_UpdatedAt
ON Consoles
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Consoles
    SET    UpdatedAt = GETUTCDATE()
    FROM   Consoles c
    INNER JOIN inserted i ON c.Id = i.Id;
END;
GO
