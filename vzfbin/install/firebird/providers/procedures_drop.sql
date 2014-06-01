﻿-- providers 

-- profile

EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_PROFILE_DELETEINACTIVE')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_PROFILE_DELETEINACTIVE;';
END

--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_PROFILE_DELETEPROFILE')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_PROFILE_DELETEPROFILE';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_PROFILE_GETPROFILES')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_PROFILE_GETPROFILES';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_PROFILE_GETNUMINACT')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_PROFILE_GETNUMINACT';
END
--GO


-- roles 

EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_ROLE_CREATEROLE')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_ROLE_CREATEROLE';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_ROLE_DELETEROLE')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_ROLE_DELETEROLE';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_ROLE_FINDUSERSINROLE')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_ROLE_FINDUSERSINROLE';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_ROLE_ISUSERINROLE')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_ROLE_ISUSERINROLE';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_ROLE_ADDUSERTOROLE')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_ROLE_ADDUSERTOROLE';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_ROLE_EXISTS')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_ROLE_EXISTS';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_ROLE_GETROLES')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_ROLE_GETROLES';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_ROLE_REMUSERFRROLE')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_ROLE_REMUSERFRROLE';
END
--GO


-- membership


EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_CHANGEPASSQUEANDANS')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_CHANGEPASSQUEANDANS';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_CHANGEPASSWORD')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_CHANGEPASSWORD';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_CREATEUSER')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_CREATEUSER';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_DELETEUSER')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_DELETEUSER';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_GETALLUSERS')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_GETALLUSERS';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_GETUSER')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_GETUSER';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_FINDUSERSBYEMAIL')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_FINDUSERSBYEMAIL';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_FINDUSERSBYNAME')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_FINDUSERSBYNAME';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_GETNUMBEROFUSERSONLINE')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_GETNUMBEROFUSERSONLINE';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_GETUSERNAMEBYEMAL')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_GETUSERNAMEBYEMAL';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_RESETPASSWORD')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_RESETPASSWORD';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_UNLOCKUSER')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_UNLOCKUSER';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_UPDATEUSER')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_UPDATEUSER';
END
--GO

-- providers extra
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}GET_VARCHAR64_UUID')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}GET_VARCHAR64_UUID';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}GET_HEX_UUID')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}GET_HEX_UUID';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}GET_VARCHAR64_CHAR16')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}GET_VARCHAR64_CHAR16';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_UPGRADE')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_UPGRADE';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (EXISTS(SELECT 1 
FROM RDB$PROCEDURES a WHERE a.RDB$PROCEDURE_NAME='{objectQualifier}P_CREATEAPPLICATION')) THEN
EXECUTE STATEMENT 'DROP PROCEDURE {objectQualifier}P_CREATEAPPLICATION';
END
--GO

