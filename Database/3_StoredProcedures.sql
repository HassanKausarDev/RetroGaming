USE RetroGamingDB;
GO

-- MANUFACTURER STORED PROCEDURES
CREATE OR ALTER PROCEDURE sp_GetAllManufacturers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        m.Id,
        m.Name,
        m.Country,
        m.City,
        m.FoundedYear,
        m.Latitude,
        m.Longitude,
        m.CreatedAt,
        m.UpdatedAt,
        COUNT(c.Id) AS ConsoleCount
    FROM Manufacturers m
    LEFT JOIN Consoles c ON c.ManufacturerId = m.Id
    GROUP BY
        m.Id, m.Name, m.Country, m.City,
        m.FoundedYear, m.Latitude, m.Longitude,
        m.CreatedAt, m.UpdatedAt
    ORDER BY m.Name;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetManufacturerById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        m.Id,
        m.Name,
        m.Country,
        m.City,
        m.FoundedYear,
        m.Latitude,
        m.Longitude,
        m.CreatedAt,
        m.UpdatedAt,
        COUNT(c.Id) AS ConsoleCount
    FROM Manufacturers m
    LEFT JOIN Consoles c ON c.ManufacturerId = m.Id
    WHERE m.Id = @Id
    GROUP BY
        m.Id, m.Name, m.Country, m.City,
        m.FoundedYear, m.Latitude, m.Longitude,
        m.CreatedAt, m.UpdatedAt;
END;
GO

CREATE OR ALTER PROCEDURE sp_CreateManufacturer
    @Name        NVARCHAR(100),
    @Country     NVARCHAR(100),
    @City        NVARCHAR(100),
    @FoundedYear INT,
    @Latitude    DECIMAL(9,6) = NULL,
    @Longitude   DECIMAL(9,6) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Manufacturers
        (Name, Country, City, FoundedYear, Latitude, Longitude, CreatedAt, UpdatedAt)
    VALUES
        (@Name, @Country, @City, @FoundedYear, @Latitude, @Longitude, GETUTCDATE(), GETUTCDATE());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END;
GO

CREATE OR ALTER PROCEDURE sp_UpdateManufacturer
    @Id          INT,
    @Name        NVARCHAR(100),
    @Country     NVARCHAR(100),
    @City        NVARCHAR(100),
    @FoundedYear INT,
    @Latitude    DECIMAL(9,6) = NULL,
    @Longitude   DECIMAL(9,6) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Manufacturers
    SET
        Name        = @Name,
        Country     = @Country,
        City        = @City,
        FoundedYear = @FoundedYear,
        Latitude    = @Latitude,
        Longitude   = @Longitude,
        UpdatedAt   = GETUTCDATE()
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE sp_DeleteManufacturer
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Manufacturers WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE sp_ManufacturerNameExists
    @Name      NVARCHAR(100),
    @ExcludeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CAST(
        CASE WHEN EXISTS (
            SELECT 1 FROM Manufacturers
            WHERE Name = @Name
            AND (@ExcludeId IS NULL OR Id != @ExcludeId)
        ) THEN 1 ELSE 0 END
    AS BIT) AS Result;
END;
GO

CREATE OR ALTER PROCEDURE sp_ManufacturerHasConsoles
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CAST(
        CASE WHEN EXISTS (
            SELECT 1 FROM Consoles WHERE ManufacturerId = @Id
        ) THEN 1 ELSE 0 END
    AS BIT) AS Result;
END;
GO

-- CONSOLE STORED PROCEDURES

CREATE OR ALTER PROCEDURE sp_GetAllConsoles
    @Search NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        c.Id,
        c.ManufacturerId,
        c.Name,
        c.ReleaseYear,
        c.Generation,
        c.UnitsSoldMillions,
        c.IsDiscontinued,
        c.CreatedAt,
        c.UpdatedAt,
        m.Name AS ManufacturerName
    FROM Consoles c
    INNER JOIN Manufacturers m ON m.Id = c.ManufacturerId
    WHERE
        @Search IS NULL
        OR c.Name LIKE '%' + @Search + '%'
        OR m.Name LIKE '%' + @Search + '%'
    ORDER BY c.Name;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetConsoleById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        c.Id,
        c.ManufacturerId,
        c.Name,
        c.ReleaseYear,
        c.Generation,
        c.UnitsSoldMillions,
        c.IsDiscontinued,
        c.CreatedAt,
        c.UpdatedAt,
        m.Name AS ManufacturerName
    FROM Consoles c
    INNER JOIN Manufacturers m ON m.Id = c.ManufacturerId
    WHERE c.Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE sp_CreateConsole
    @ManufacturerId    INT,
    @Name              NVARCHAR(100),
    @ReleaseYear       INT,
    @Generation        INT,
    @UnitsSoldMillions DECIMAL(10,2) = NULL,
    @IsDiscontinued    BIT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Consoles
        (ManufacturerId, Name, ReleaseYear, Generation, UnitsSoldMillions, IsDiscontinued, CreatedAt, UpdatedAt)
    VALUES
        (@ManufacturerId, @Name, @ReleaseYear, @Generation, @UnitsSoldMillions, @IsDiscontinued, GETUTCDATE(), GETUTCDATE());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END;
GO

CREATE OR ALTER PROCEDURE sp_UpdateConsole
    @Id                INT,
    @ManufacturerId    INT,
    @Name              NVARCHAR(100),
    @ReleaseYear       INT,
    @Generation        INT,
    @UnitsSoldMillions DECIMAL(10,2) = NULL,
    @IsDiscontinued    BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Consoles
    SET
        ManufacturerId    = @ManufacturerId,
        Name              = @Name,
        ReleaseYear       = @ReleaseYear,
        Generation        = @Generation,
        UnitsSoldMillions = @UnitsSoldMillions,
        IsDiscontinued    = @IsDiscontinued,
        UpdatedAt         = GETUTCDATE()
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE sp_DeleteConsole
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Consoles WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE sp_ConsoleNameExistsForManufacturer
    @ManufacturerId INT,
    @Name           NVARCHAR(100),
    @ExcludeId      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CAST(
        CASE WHEN EXISTS (
            SELECT 1 FROM Consoles
            WHERE ManufacturerId = @ManufacturerId
            AND   Name           = @Name
            AND   (@ExcludeId IS NULL OR Id != @ExcludeId)
        ) THEN 1 ELSE 0 END
    AS BIT) AS Result;
END;
GO
