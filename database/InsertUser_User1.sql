USE [232127DelaCruzSTUDENTCRUD];
GO

-- Create or update user with USERID = 1 and password '123' (plain text)
IF EXISTS (SELECT 1 FROM dbo.[232127DelaCruzUSER] WHERE USERID = 1)
BEGIN
    UPDATE dbo.[232127DelaCruzUSER]
    SET FNAME = N'Test', LNAME = N'User', MNAME = N'', PASSWORD = N'123'
    WHERE USERID = 1;
END
ELSE
BEGIN
    SET IDENTITY_INSERT dbo.[232127DelaCruzUSER] ON;
    INSERT INTO dbo.[232127DelaCruzUSER] (USERID, FNAME, LNAME, MNAME, PASSWORD)
    VALUES (1, N'Test', N'User', N'', N'123');
    SET IDENTITY_INSERT dbo.[232127DelaCruzUSER] OFF;
END

-- Verify
SELECT USERID, FNAME, LNAME FROM dbo.[232127DelaCruzUSER] WHERE USERID = 1;

-- Optional: replace the plain password with a BCrypt hash for better security
-- Example hash for password '123' (generated externally):
-- $2a$11$wHh1H5h9v1wG9l7iYwqvJeQ5Y4bJxRK6zQe6YJdM6qQZW8q5x0c2G
-- UPDATE dbo.[232127DelaCruzUSER] SET PASSWORD = '$2a$11$wHh1H5h9v1wG9l7iYwqvJeQ5Y4bJxRK6zQe6YJdM6qQZW8q5x0c2G' WHERE USERID = 1;
