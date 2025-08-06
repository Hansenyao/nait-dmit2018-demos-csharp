-- 1. Create the roles "Admin" and "Manager" if they do not already exist

-- Check if the "Admin" role exists, if not, create it
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'ADMIN')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());
END

-- Check if the "Manager" role exists, if not, create it
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'MANAGER')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Manager', 'MANAGER', NEWID());
END

-- 2. Assign the "Admin" role to "admin@bb.cc" and the "Manager" role to "manager@bb.cc"

-- Fetch the user IDs for the new users
DECLARE @AdminUserId NVARCHAR(450);
DECLARE @ManagerUserId NVARCHAR(450);

SELECT @AdminUserId = Id FROM AspNetUsers WHERE UserName = 'admin@bb.cc';
SELECT @ManagerUserId = Id FROM AspNetUsers WHERE UserName = 'manager@bb.cc';

-- Fetch the role IDs for the roles
DECLARE @AdminRoleId NVARCHAR(450);
DECLARE @ManagerRoleId NVARCHAR(450);

SELECT @AdminRoleId = Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN';
SELECT @ManagerRoleId = Id FROM AspNetRoles WHERE NormalizedName = 'MANAGER';

-- Assign the "Admin" role to "admin@bb.cc"
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @AdminUserId AND RoleId = @AdminRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@AdminUserId, @AdminRoleId);
END

-- Assign the "Manager" role to "manager@bb.cc"
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @ManagerUserId AND RoleId = @ManagerRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@ManagerUserId, @ManagerRoleId);
END
