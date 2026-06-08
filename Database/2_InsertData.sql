USE RetroGamingDB;
GO

-- Clear existing data
DELETE FROM Consoles;
DELETE FROM Manufacturers;
DBCC CHECKIDENT ('Consoles',      RESEED, 0);
DBCC CHECKIDENT ('Manufacturers', RESEED, 0);
GO

-- Manufacturers
INSERT INTO Manufacturers (Name, Country, City, FoundedYear, Latitude, Longitude)
VALUES
    ('Nintendo',  'Japan',         'Kyoto',          1889,  35.011635,  135.768150),
    ('Sega',      'Japan',         'Shinagawa',       1960,  35.609333,  139.730408),
    ('Atari',     'United States', 'Sunnyvale',       1972,  37.368830, -122.036346),
    ('Sony',      'Japan',         'Minato',          1946,  35.658200,  139.745400),
    ('Microsoft', 'United States', 'Redmond',         1975,  47.673988, -122.121513);
GO

-- Consoles
INSERT INTO Consoles (ManufacturerId, Name, ReleaseYear, Generation, UnitsSoldMillions, IsDiscontinued)
VALUES
    -- Nintendo
    (1, 'NES',            1983, 3,  61.91,  1),
    (1, 'Super Nintendo', 1990, 4,  49.10,  1),
    (1, 'Nintendo 64',    1996, 5,  32.93,  1),
    (1, 'GameCube',       2001, 6,  21.74,  1),
    (1, 'Wii',            2006, 7, 101.63,  1),
    (1, 'Wii U',          2012, 8,  13.56,  1),
    (1, 'Switch',         2017, 9, 140.00,  0),

    -- Sega
    (2, 'Mega Drive',     1988, 4,  30.75,  1),
    (2, 'Sega Saturn',    1994, 5,   9.26,  1),
    (2, 'Dreamcast',      1998, 6,   9.13,  1),

    -- Atari
    (3, 'Atari 2600',     1977, 2,  30.00,  1),
    (3, 'Atari 5200',     1982, 3,   1.00,  1),
    (3, 'Atari 7800',     1986, 3,   3.77,  1),

    -- Sony
    (4, 'PlayStation',    1994, 5, 102.49,  1),
    (4, 'PlayStation 2',  2000, 6, 155.00,  1),
    (4, 'PlayStation 3',  2006, 7,  87.40,  1),
    (4, 'PlayStation 4',  2013, 8, 117.20,  1),
    (4, 'PlayStation 5',  2020, 9,  50.00,  0),

    -- Microsoft
    (5, 'Xbox',           2001, 6,  24.00,  1),
    (5, 'Xbox 360',       2005, 7,  84.00,  1),
    (5, 'Xbox One',       2013, 8,  51.00,  1),
    (5, 'Xbox Series X',  2020, 9,  19.00,  0);
GO
