
/* Yet Another Forum.NET Firebird data layer by vzrus
 * Copyright (C) 2006-2013 Vladimir Zakharov
 * https://github.com/vzrus
 * http://sourceforge.net/projects/yaf-datalayers/
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2 only
 * General class structure is based on MS SQL Server code,
 * created by YAF developers
 *
 * http://www.yetanotherforum.net/
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation;version 2 only
 * of the License.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

-- The procedure converts all uuids to text
CREATE OR ALTER PROCEDURE  objQual_GET_HEX_UUID
returns(real_uuid char(16) character set OCTETS, hex_uuid varchar(32), varchar64_uuid varchar(64))
AS
declare variable i integer;
declare variable c integer;
BEGIN
real_uuid = GEN_UUID();
hex_uuid = '';
varchar64_uuid = '';
i = 0;
while (i < 16) do
begin
c = ascii_val(substring(real_uuid from i+1 for 1));
if (c < 0) then c = 256 + c;
if ((CHAR_LENGTH(varchar64_uuid)=8) 
OR (CHAR_LENGTH(varchar64_uuid)=12)
OR (CHAR_LENGTH(varchar64_uuid)=16)
OR (CHAR_LENGTH(varchar64_uuid)=20)) then varchar64_uuid = varchar64_uuid || '-';

hex_uuid = hex_uuid
|| substring('0123456789abcdef' from bin_shr(c, 4) + 1 for 1)
|| substring('0123456789abcdef' from bin_and(c, 15) + 1 for 1);
varchar64_uuid=varchar64_uuid
|| substring('0123456789abcdef' from bin_shr(c, 4) + 1 for 1)
|| substring('0123456789abcdef' from bin_and(c, 15) + 1 for 1);

i = i + 1;
end
suspend;
END;
--GO

CREATE OR ALTER PROCEDURE  objQual_GET_VARCHAR64_UUID
returns(varchar64_uuid varchar(64))
AS
declare variable i integer;
declare variable c integer;
declare variable real_uuid  char(16) character set OCTETS;
BEGIN
real_uuid = GEN_UUID();
varchar64_uuid = '';
i = 0;
while (i < 16) do
begin
c = ascii_val(substring(real_uuid from i+1 for 1));
if (c < 0) then c = 256 + c;
if (CHAR_LENGTH(varchar64_uuid)=8 
OR CHAR_LENGTH(varchar64_uuid)=13
OR CHAR_LENGTH(varchar64_uuid)=18 
OR CHAR_LENGTH(varchar64_uuid)=23) then varchar64_uuid = varchar64_uuid || '-';

varchar64_uuid=varchar64_uuid
|| substring('0123456789abcdef' from bin_shr(c, 4) + 1 for 1)
|| substring('0123456789abcdef' from bin_and(c, 15) + 1 for 1);

i = i + 1;
end
suspend;
END;

--GO

/*CREATE OR ALTER PROCEDURE  objQual_GET_CHAR16_VARCHAR64
returns(varchar64_uuid varchar(64))
AS
declare variable i integer;
declare variable c integer;
declare variable real_uuid  char(16) character set OCTETS;
BEGIN
real_uuid = '';
i = 0;
while (i < 16) do
begin
c = ascii_val(substring(real_uuid from i+1 for 1));
if (c < 0) then c = 256 + c;
if (CHAR_LENGTH(varchar64_uuid)=8 
OR CHAR_LENGTH(varchar64_uuid)=12
OR CHAR_LENGTH(varchar64_uuid)=16 
OR CHAR_LENGTH(varchar64_uuid)=20) then varchar64_uuid = varchar64_uuid || '-';
SUBSTRING( varchar64_uuid FROM 1 FOR 8) 
SUBSTRING( varchar64_uuid FROM 10 FOR 13) 
SUBSTRING( varchar64_uuid FROM 10 FOR 13) 
SUBSTRING( varchar64_uuid FROM 22 FOR 36) 
varchar64_uuid=varchar64_uuid
|| substring('0123456789abcdef' from bin_shr(c, 4) + 1 for 1)
|| substring('0123456789abcdef' from bin_and(c, 15) + 1 for 1);

i = i + 1;
end
suspend;
END;*/

-- GO

CREATE OR ALTER PROCEDURE  objQual_GET_VARCHAR64_CHAR16(real_uuid  char(16) character set OCTETS) 
returns(varchar64_uuid varchar(64))
AS
declare variable i integer;
declare variable c integer;
BEGIN
varchar64_uuid = '';
i = 0;
while (i < 16) do
begin
c = ascii_val(substring(real_uuid from i+1 for 1));
if (c < 0) then c = 256 + c;
if (CHAR_LENGTH(varchar64_uuid)=8 
OR CHAR_LENGTH(varchar64_uuid)=12
OR CHAR_LENGTH(varchar64_uuid)=16 
OR CHAR_LENGTH(varchar64_uuid)=20) 
then varchar64_uuid = varchar64_uuid || '-';

varchar64_uuid=varchar64_uuid
|| substring('0123456789abcdef' from bin_shr(c, 4) + 1 for 1)
|| substring('0123456789abcdef' from bin_and(c, 15) + 1 for 1);

i = i + 1;
end
suspend;
END;

--GO


 CREATE OR ALTER PROCEDURE  objQual_P_CREATEAPPLICATION
 (
 I_APPLICATIONNAME VARCHAR(128)
 )
RETURNS (I_APPLICATIONID  CHAR(16) CHARACTER SET OCTETS)
AS
 BEGIN
	SELECT APPLICATIONID FROM objQual_P_APPLICATION WHERE APPLICATIONNAMELWD=LOWER(:I_APPLICATIONNAME) INTO :I_APPLICATIONID;
	
	IF (I_APPLICATIONID IS NULL) THEN 
	begin
		I_APPLICATIONID = GEN_UUID(); 		  
		 INSERT  INTO objQual_P_APPLICATION (APPLICATIONID, APPLICATIONNAME, APPLICATIONNAMELWD)
			 VALUES  (:I_APPLICATIONID, :I_APPLICATIONNAME, LOWER(:I_APPLICATIONNAME));
	end    
 END;
 
--GO

CREATE OR ALTER PROCEDURE  objQual_P_UPGRADE
(
I_PREVIOUSVERSION INTEGER,
I_NEWVERSION INTEGER
)
AS
BEGIN
		IF (I_PREVIOUSVERSION = 32 OR I_PREVIOUSVERSION = 31) THEN
		BEGIN
			-- RESOLVE SALT ISSUE IN 193 RC2
			UPDATE objQual_P_MEMBERSHIP SET PASSWORDSALT='UwB5AHMAdABlAG0ALgBCAHkAdABlAFsAXQA=' WHERE PASSWORDSALT IS NOT NULL;
			UPDATE objQual_P_MEMBERSHIP SET JOINED=current_date WHERE JOINED IS NULL;
		END	
END; 
--GO
 -- DROP PROCEDURE IF EXISTS objQual_P_CHANGEPASSWORD;

 CREATE OR ALTER PROCEDURE  objQual_P_CHANGEPASSWORD(
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128),
 I_PASSWORD VARCHAR(128),
 I_PASSWORDSALT VARCHAR(128),
 I_PASSWORDFORMAT VARCHAR(128),
 I_PASSWORDANSWER VARCHAR(128)
 ) AS 
 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN 	
	
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16;
			
	UPDATE objQual_P_MEMBERSHIP SET "PASSWORD"=:I_PASSWORD, PASSWORDSALT=:I_PASSWORDSALT,
		PASSWORDFORMAT=:I_PASSWORDFORMAT, PASSWORDANSWER=:I_PASSWORDANSWER
	WHERE USERNAMELWD=LOWER(:I_USERNAME) and APPLICATIONID=:ICI_APPLICATIONID16;
 
 END;
--GO


/* PROVIDER TABLE SCRIPT BY VZ_TEAM */
-- DROP PROCEDURE IF EXISTS objQual_P_deleteuser;
 -- GO 
 CREATE OR ALTER PROCEDURE  objQual_P_DELETEUSER
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128),
 I_DELETEALLRELATED BOOL
 ) AS     	
	DECLARE VARIABLE ICI_USERID16 CHAR(16); 
	DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN 
	
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16;
 
	/*get the userID*/
	SELECT   USERID  FROM objQual_P_MEMBERSHIP 
	WHERE APPLICATIONID = :ICI_APPLICATIONID16 
	AND USERNAMELWD = LOWER(:I_USERNAME) INTO  :ICI_USERID16;

		
	IF (ICI_USERID16 IS NOT NULL) THEN
	begin
		/*Delete records from membership*/
		DELETE FROM objQual_P_MEMBERSHIP WHERE USERID = :ICI_USERID16;
		/*Delete from Role table*/
		DELETE FROM objQual_P_ROLEMEMBERSHIP WHERE USERID = :ICI_USERID16;
		/*Delete from Profile table*/
		DELETE FROM objQual_P_PROFILE WHERE USERID = :ICI_USERID16;
	end	
END;
--GO



 


-- Implement here user name for temp table check column and so for all data layers!
-- DROP PROCEDURE IF EXISTS objQual_P_GETALLUSERS;
-- GO 
CREATE OR ALTER PROCEDURE  objQual_P_GETALLUSERS
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_PAGEINDEX INTEGER,
 I_PAGESIZE INTEGER 
 )
 RETURNS( "UserID" VARCHAR(64),
  "ApplicationID" VARCHAR(64),
  "Username" VARCHAR(128) ,
  "UsernameLwd" VARCHAR(128) ,
  "Password" VARCHAR(128),
  "PasswordSalt" VARCHAR(128),
  "PasswordFormat" VARCHAR(128),
  "Email" VARCHAR(128),
  "EmailLwd" VARCHAR(128),
  "PasswordQuestion" VARCHAR(128),
  "PasswordAnswer" VARCHAR(128),
  "IsApproved" SMALLINT,
  "IsLockedOut" SMALLINT,
  "LastLogin" TIMESTAMP,
  "LastActivity" TIMESTAMP,
  "LastPasswordChange" TIMESTAMP,
  "LastLockOut" TIMESTAMP,
  "FailedPasswordAttempts" INTEGER,
  "FailedAnswerAttempts" INTEGER,
  "FailedPasswordWindow" TIMESTAMP,
  "FailedAnswerWindow" TIMESTAMP,
  "Joined" TIMESTAMP,
  "Comment" BLOB SUB_TYPE 1,
  "RowNumber" INTEGER)
  AS   
   DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
   DECLARE VARIABLE ICI_PAGINGLOWERBOUNDARY INTEGER;
   DECLARE VARIABLE ICI_PAGINGUPPERBOUNDARY INTEGER;
   DECLARE VARIABLE ICI_TOTALRECORDS INTEGER;
 BEGIN

  EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
	/*Set the page bounds*/
	IF (I_PAGEINDEX < 0 ) THEN
	   begin
	 ICI_PAGINGLOWERBOUNDARY = ((I_PAGESIZE) * (I_PAGEINDEX))+1;
	 ICI_PAGINGUPPERBOUNDARY = ((I_PAGEINDEX +1)*(I_PAGESIZE))+1;
	   end
	 ELSE
	   begin
	 ICI_PAGINGLOWERBOUNDARY = I_PAGESIZE*I_PAGEINDEX;
	 ICI_PAGINGUPPERBOUNDARY = I_PAGESIZE -1 + ICI_PAGINGLOWERBOUNDARY;
	   end
  
   

	
   
	 FOR SELECT  FIRST(:ICI_PAGINGUPPERBOUNDARY) SKIP(:ICI_PAGINGLOWERBOUNDARY)
	 UUID_TO_CHAR(m.USERID) AS "UserID",
	 UUID_TO_CHAR(m.APPLICATIONID) AS "ApplicationID",
	 m.USERNAME,
	 m.USERNAMELWD,
	 m."PASSWORD",
	 m.PASSWORDSALT,
	 m.PASSWORDFORMAT,
	 m."EMAIL",
	 m.EMAILLWD,
	 m.PASSWORDQUESTION,
	 m.PASSWORDANSWER,
	 m.ISAPPROVED,
	 m.ISLOCKEDOUT,
	 m.LASTLOGIN, 	 
	 m.LASTACTIVITY, 	 
	 m.LASTPASSWORDCHANGE, 	 
	 m.LASTLOCKOUT, 	 
	 m.FAILEDPASSWORDATTEMPTS, 	
	 m.FAILEDANSWERATTEMPTS,
	 m.FAILEDPASSWORDWINDOW,
	 m.FAILEDANSWERWINDOW,
	 m.JOINED,
	 m."COMMENT", 	  
	 (SELECT :ICI_TOTALRECORDS FROM rdb$database) AS "RowNumber" 
	FROM objQual_P_MEMBERSHIP m  
	WHERE  m.APPLICATIONID = :ICI_APPLICATIONID16
	INTO 
  :"UserID" ,
  :"ApplicationID",
  :"Username" ,
  :"UsernameLwd",
  :"Password",
  :"PasswordSalt",
  :"PasswordFormat",
  :"Email",
  :"EmailLwd",
  :"PasswordQuestion",
  :"PasswordAnswer",
  :"IsApproved",
  :"IsLockedOut",
  :"LastLogin",
  :"LastActivity",
  :"LastPasswordChange",
  :"LastLockOut",
  :"FailedPasswordAttempts",
  :"FailedAnswerAttempts",
  :"FailedPasswordWindow",
  :"FailedAnswerWindow",
  :"Joined",
  :"Comment",
  :"RowNumber"
	DO
		begin 	
		ICI_TOTALRECORDS=ICI_TOTALRECORDS+1;
		SUSPEND; 	  	      
		end 
		   
 END;
--GO


CREATE OR ALTER PROCEDURE  objQual_P_RESETPASSWORD
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128),
 I_PASSWORD VARCHAR(128),
 I_PASSWORDSALT VARCHAR(128),
 I_PASSWORDFORMAT VARCHAR(128),
 I_MAXINVALIDATTEMPTS INTEGER,
 I_PASSWORDATTEMPTWINDOW INTEGER,
 I_CURRENTTIMEUTC TIMESTAMP
 )
 AS
 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN 	
	
	  EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
	
	UPDATE objQual_P_MEMBERSHIP SET
	"PASSWORD" = :I_PASSWORD,
	PASSWORDSALT = :I_PASSWORDSALT,
	PASSWORDFORMAT = :I_PASSWORDFORMAT,
	LASTPASSWORDCHANGE = :I_CURRENTTIMEUTC
	WHERE APPLICATIONID = :ICI_APPLICATIONID16 AND
	USERNAMELWD = LOWER(:I_USERNAME);
 
 END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_GETUSER
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128),
 I_USERKEY VARCHAR(64),
 I_USERISONLINE BOOL,
 I_UTCTIMESTAMP TIMESTAMP
 )
  RETURNS( "UserID" VARCHAR(64),
  "ApplicationID" VARCHAR(64),
  "Username" VARCHAR(128) ,
  "UsernameLwd" VARCHAR(128) ,
  "Password" VARCHAR(128),
  "PasswordSalt" VARCHAR(128),
  "PasswordFormat" VARCHAR(128),
  "Email" VARCHAR(128),
  "EmailLwd" VARCHAR(128),
  "PasswordQuestion" VARCHAR(128),
  "PasswordAnswer" VARCHAR(128),
  "IsApproved" SMALLINT,
  "IsLockedOut" SMALLINT,
  "LastLogin" TIMESTAMP,
  "LastActivity" TIMESTAMP,
  "LastPasswordChange" TIMESTAMP,
  "LastLockOut" TIMESTAMP,
  "FailedPasswordAttempts" INTEGER,
  "FailedAnswerAttempts" INTEGER,
  "FailedPasswordWindow" TIMESTAMP,
  "FailedAnswerWindow" TIMESTAMP,
  "Joined" TIMESTAMP,
  "Comment" BLOB SUB_TYPE 1)
 AS
 --DECLARE VARIABLE ICI_APPLICATIONID VARCHAR(64);
 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 DECLARE VARIABLE ICI_USERID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN 	
 
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
		
	-- (SELECT * FROM objQual_GET_VARCHAR64_CHAR16(m.USERID)) AS "UserID",
	-- (SELECT * FROM  objQual_GET_VARCHAR64_CHAR16(m.APPLICATIONID)) AS "ApplicationID",
	IF (I_USERKEY IS NULL) THEN 	
	SELECT
	UUID_TO_CHAR(m.USERID),
	UUID_TO_CHAR(m.APPLICATIONID), 	
	 m.USERNAME,
	 m.USERNAMELWD,
	 m."PASSWORD",
	 m.PASSWORDSALT,
	 m.PASSWORDFORMAT,
	 m."EMAIL",
	 m.EMAILLWD,
	 m.PASSWORDQUESTION,
	 m.PASSWORDANSWER,
	 m.ISAPPROVED,
	 m.ISLOCKEDOUT,
	 m.LASTLOGIN, 	 
	 m.LASTACTIVITY, 	 
	 m.LASTPASSWORDCHANGE, 	 
	 m.LASTLOCKOUT, 	 
	 m.FAILEDPASSWORDATTEMPTS, 	
	 m.FAILEDANSWERATTEMPTS,
	 m.FAILEDPASSWORDWINDOW,
	 m.FAILEDANSWERWINDOW,
	 m.JOINED,
	 m."COMMENT"
		 FROM objQual_P_MEMBERSHIP m 
				  WHERE m.USERNAMELWD = LOWER(:I_USERNAME) 
				  AND m.APPLICATIONID = :ICI_APPLICATIONID16
				  INTO 
  :"UserID" ,
  :"ApplicationID",
  :"Username" ,
  :"UsernameLwd",
  :"Password",
  :"PasswordSalt",
  :"PasswordFormat",
  :"Email",
  :"EmailLwd",
  :"PasswordQuestion",
  :"PasswordAnswer",
  :"IsApproved",
  :"IsLockedOut",
  :"LastLogin",
  :"LastActivity",
  :"LastPasswordChange",
  :"LastLockOut",
  :"FailedPasswordAttempts",
  :"FailedAnswerAttempts",
  :"FailedPasswordWindow",
  :"FailedAnswerWindow",
  :"Joined",
  :"Comment"; 	
	
	ELSE
	begin 	
	   
		SELECT 
	 UUID_TO_CHAR(m.USERID) AS "UserID",
	 UUID_TO_CHAR(m.APPLICATIONID)  AS "ApplicationID",
	 m.USERNAME,
	 m.USERNAMELWD,
	 m."PASSWORD",
	 m.PASSWORDSALT,
	 m.PASSWORDFORMAT,
	 m."EMAIL",
	 m.EMAILLWD,
	 m.PASSWORDQUESTION,
	 m.PASSWORDANSWER,
	 m.ISAPPROVED,
	 m.ISLOCKEDOUT,
	 m.LASTLOGIN, 	 
	 m.LASTACTIVITY, 	 
	 m.LASTPASSWORDCHANGE, 	 
	 m.LASTLOCKOUT, 	 
	 m.FAILEDPASSWORDATTEMPTS, 	
	 m.FAILEDANSWERATTEMPTS,
	 m.FAILEDPASSWORDWINDOW,
	 m.FAILEDANSWERWINDOW,
	 m.JOINED,
	 m."COMMENT" FROM objQual_P_MEMBERSHIP m 
				  WHERE m.USERID = CHAR_TO_UUID(:I_USERKEY )
				  AND m.APPLICATIONID = :ICI_APPLICATIONID16
		 INTO 
  :"UserID" ,
  :"ApplicationID",
  :"Username" ,
  :"UsernameLwd",
  :"Password",
  :"PasswordSalt",
  :"PasswordFormat",
  :"Email",
  :"EmailLwd",
  :"PasswordQuestion",
  :"PasswordAnswer",
  :"IsApproved",
  :"IsLockedOut",
  :"LastLogin",
  :"LastActivity",
  :"LastPasswordChange",
  :"LastLockOut",
  :"FailedPasswordAttempts",
  :"FailedAnswerAttempts",
  :"FailedPasswordWindow",
  :"FailedAnswerWindow",
  :"Joined",
  :"Comment"; 
  end         
		
	
	/*IF USER IS ONLINE DO AN UPDATE OF THE USER*/	
	IF (I_USERISONLINE = 1) THEN 	
		UPDATE objQual_P_MEMBERSHIP 
		SET LASTACTIVITY = :I_UTCTIMESTAMP 
		WHERE USERNAMELWD = LOWER(:I_USERNAME) 
		and APPLICATIONID = :ICI_APPLICATIONID16;
	SUSPEND;	
END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_UNLOCKUSER
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128)
 )
 AS 
 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN
	
 
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
	
	UPDATE objQual_P_MEMBERSHIP SET
	ISLOCKEDOUT = 0,
	FAILEDPASSWORDATTEMPTS = 0
	WHERE APPLICATIONID = :ICI_APPLICATIONID16 AND
	USERNAMELWD = LOWER(:I_USERNAME);
 
END;
 --GO 
 
  CREATE OR ALTER PROCEDURE  objQual_P_CHANGEPASSQUEANDANS
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128),
 I_PASSWORDQUESTION VARCHAR(128),
 I_PASSWORDANSWER VARCHAR(128)
 )
 AS
 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN 	
	
		EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
	
	UPDATE objQual_P_MEMBERSHIP SET PASSWORDQUESTION=:I_PASSWORDQUESTION, PASSWORDANSWER=:I_PASSWORDANSWER
	WHERE USERNAMELWD=LOWER(:I_USERNAME) and APPLICATIONID=:ICI_APPLICATIONID16;
 
END;
--GO

CREATE OR ALTER PROCEDURE  objQual_P_CREATEUSER
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128),
 I_PASSWORD VARCHAR(128),
 I_PASSWORDSALT VARCHAR(128),
 I_PASSWORDFORMAT VARCHAR(128),
 I_EMAIL VARCHAR(128),
 I_PASSWORDQUESTION VARCHAR(128),
 I_PASSWORDANSWER VARCHAR(128),
 I_ISAPPROVED BOOL,
 I_USERKEY VARCHAR(64),
 I_UTCTIMESTAMP TIMESTAMP  
 ) 
  RETURNS (OUT_USERKEY VARCHAR(64))
 AS
 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 DECLARE VARIABLE ICI_USERKEY16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN 	
		
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
		
	IF (:I_USERKEY IS NULL) THEN
	begin
	ICI_USERKEY16 = GEN_UUID();
	OUT_USERKEY=UUID_TO_CHAR(:ICI_USERKEY16);
	-- SELECT * FROM objQual_GET_VARCHAR64_CHAR16(:ICI_USERKEY16) INTO :OUT_USERKEY;
	end	
	INSERT INTO objQual_P_MEMBERSHIP (USERID,APPLICATIONID,USERNAME,USERNAMELWD,"PASSWORD",PASSWORDSALT,PASSWORDFORMAT,"EMAIL",EMAILLWD,PASSWORDQUESTION,PASSWORDANSWER,ISAPPROVED,JOINED)
		VALUES (:ICI_USERKEY16, :ICI_APPLICATIONID16 ,:I_USERNAME, LOWER(:I_USERNAME), :I_PASSWORD, :I_PASSWORDSALT, :I_PASSWORDFORMAT, :I_EMAIL, LOWER(:I_EMAIL), :I_PASSWORDQUESTION, :I_PASSWORDANSWER, :I_ISAPPROVED,:I_UTCTIMESTAMP);
SUSPEND;
END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_FINDUSERSBYEMAIL
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_EMAILADDRESS VARCHAR(128),
 I_PAGEINDEX INTEGER,
 I_PAGESIZE INTEGER
 )
 RETURNS( "UserID" VARCHAR(64),
  "ApplicationID" VARCHAR(64),
  "Username" VARCHAR(128) ,
  "UsernameLwd" VARCHAR(128) ,
  "Password" VARCHAR(128),
  "PasswordSalt" VARCHAR(128),
  "PasswordFormat" VARCHAR(128),
  "Email" VARCHAR(128),
  "EmailLwd" VARCHAR(128),
  "PasswordQuestion" VARCHAR(128),
  "PasswordAnswer" VARCHAR(128),
  "IsApproved" SMALLINT,
  "IsLockedOut" SMALLINT,
  "LastLogin" TIMESTAMP,
  "LastActivity" TIMESTAMP,
  "LastPasswordChange" TIMESTAMP,
  "LastLockOut" TIMESTAMP,
  "FailedPasswordAttempts" INTEGER,
  "FailedAnswerAttempts" INTEGER,
  "FailedPasswordWindow" TIMESTAMP,
  "FailedAnswerWindow" TIMESTAMP,
  "Joined" TIMESTAMP,
  "Comment" BLOB SUB_TYPE 1,
  "RowNumber" INTEGER)
 AS
	 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
	 DECLARE VARIABLE ICI_PAGINGLOWERBOUNDARY INTEGER;
	 DECLARE VARIABLE ICI_PAGINGUPPERBOUNDARY INTEGER;
	 DECLARE VARIABLE ICI_ID INTEGER;
	  
 BEGIN 
	 /*Set the page bounds*/  
 
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 


	   IF (I_PAGEINDEX < 0 ) THEN
	   begin
	 ICI_PAGINGLOWERBOUNDARY = ((I_PAGESIZE) * (I_PAGEINDEX))+1;
	 ICI_PAGINGUPPERBOUNDARY = ((I_PAGEINDEX +1)*(I_PAGESIZE))+1;
	   end
	 ELSE
	   begin
	 ICI_PAGINGLOWERBOUNDARY = I_PAGESIZE*I_PAGEINDEX;
	 ICI_PAGINGUPPERBOUNDARY = I_PAGESIZE -1 + ICI_PAGINGLOWERBOUNDARY;
	   end
  


-- SET GENERATOR PFUBM TO 0;
	 FOR SELECT  FIRST(:ICI_PAGINGUPPERBOUNDARY) SKIP(:ICI_PAGINGLOWERBOUNDARY)
	 UUID_TO_CHAR(m.USERID)  AS "UserID",
	 UUID_TO_CHAR(m.APPLICATIONID) AS "ApplicationID",
	 m.USERNAME,
	 m.USERNAMELWD,
	 m."PASSWORD",
	 m.PASSWORDSALT,
	 m.PASSWORDFORMAT,
	 m."EMAIL",
	 m.EMAILLWD,
	 m.PASSWORDQUESTION,
	 m.PASSWORDANSWER,
	 m.ISAPPROVED,
	 m.ISLOCKEDOUT,
	 m.LASTLOGIN, 	 
	 m.LASTACTIVITY, 	 
	 m.LASTPASSWORDCHANGE, 	 
	 m.LASTLOCKOUT, 	 
	 m.FAILEDPASSWORDATTEMPTS, 	
	 m.FAILEDANSWERATTEMPTS,
	 m.FAILEDPASSWORDWINDOW,
	 m.FAILEDANSWERWINDOW,
	 m.JOINED,
	 m."COMMENT", 
	 (SELECT :ICI_ID FROM rdb$database) AS "RowNumber" FROM objQual_P_MEMBERSHIP m  
		WHERE m.EMAILLWD = LOWER(:I_EMAILADDRESS)       
		 INTO 
  :"UserID" ,
  :"ApplicationID",
  :"Username" ,
  :"UsernameLwd",
  :"Password",
  :"PasswordSalt",
  :"PasswordFormat",
  :"Email",
  :"EmailLwd",
  :"PasswordQuestion",
  :"PasswordAnswer",
  :"IsApproved",
  :"IsLockedOut",
  :"LastLogin",
  :"LastActivity",
  :"LastPasswordChange",
  :"LastLockOut",
  :"FailedPasswordAttempts",
  :"FailedAnswerAttempts",
  :"FailedPasswordWindow",
  :"FailedAnswerWindow",
  :"Joined",
  :"Comment",
  :"RowNumber"
  DO
  begin
  ICI_ID = ICI_ID+1;
   SUSPEND;
  end 
   
END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_FINDUSERSBYNAME
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128),
 I_PAGEINDEX INTEGER,
 I_PAGESIZE INTEGER
 )
RETURNS( "UserID" VARCHAR(64),
  "ApplicationID" VARCHAR(64),
  "Username" VARCHAR(128) ,
  "UsernameLwd" VARCHAR(128) ,
  "Password" VARCHAR(128),
  "PasswordSalt" VARCHAR(128),
  "PasswordFormat" VARCHAR(128),
  "Email" VARCHAR(128),
  "EmailLwd" VARCHAR(128),
  "PasswordQuestion" VARCHAR(128),
  "PasswordAnswer" VARCHAR(128),
  "IsApproved" SMALLINT,
  "IsLockedOut" SMALLINT,
  "LastLogin" TIMESTAMP,
  "LastActivity" TIMESTAMP,
  "LastPasswordChange" TIMESTAMP,
  "LastLockOut" TIMESTAMP,
  "FailedPasswordAttempts" INTEGER,
  "FailedAnswerAttempts" INTEGER,
  "FailedPasswordWindow" TIMESTAMP,
  "FailedAnswerWindow" TIMESTAMP,
  "Joined" TIMESTAMP,
  "Comment" BLOB SUB_TYPE 1,
  "RowNumber" INTEGER)
 AS
	 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
	 DECLARE VARIABLE ICI_PAGINGLOWERBOUNDARY INTEGER;
	 DECLARE VARIABLE ICI_PAGINGUPPERBOUNDARY INTEGER;
	 DECLARE VARIABLE ICI_ID INTEGER;
 BEGIN    
	
 
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
 
	  /* Set the page bounds*/
	  
	 IF (I_PAGEINDEX < 0 ) THEN
	  begin
	 ICI_PAGINGLOWERBOUNDARY = ((I_PAGESIZE) * (I_PAGEINDEX))+1;
	 ICI_PAGINGUPPERBOUNDARY= ((I_PAGEINDEX +1)*(I_PAGESIZE))+1;
	  end
	 ELSE
	  begin
	 ICI_PAGINGLOWERBOUNDARY = I_PAGESIZE*I_PAGEINDEX;
	 ICI_PAGINGUPPERBOUNDARY= I_PAGESIZE -1 + ICI_PAGINGLOWERBOUNDARY ;
	  end


  --  SET GENERATOR PFUBN TO 0;  
  --  SELECT m.*, GEN_ID(PFUBN, 1) AS "RowNumber" 
 
		 FOR SELECT  FIRST(:ICI_PAGINGUPPERBOUNDARY) SKIP(:ICI_PAGINGLOWERBOUNDARY)
	 UUID_TO_CHAR(m.USERID) AS "UserID",
	 UUID_TO_CHAR(m.APPLICATIONID)  AS "ApplicationID",
	 m.USERNAME,
	 m.USERNAMELWD,
	 m."PASSWORD",
	 m.PASSWORDSALT,
	 m.PASSWORDFORMAT,
	 m."EMAIL",
	 m.EMAILLWD,
	 m.PASSWORDQUESTION,
	 m.PASSWORDANSWER,
	 m.ISAPPROVED,
	 m.ISLOCKEDOUT,
	 m.LASTLOGIN, 	 
	 m.LASTACTIVITY, 	 
	 m.LASTPASSWORDCHANGE, 	 
	 m.LASTLOCKOUT, 	 
	 m.FAILEDPASSWORDATTEMPTS, 	
	 m.FAILEDANSWERATTEMPTS,
	 m.FAILEDPASSWORDWINDOW,
	 m.FAILEDANSWERWINDOW,
	 m.JOINED,
	 m."COMMENT", 
	 (SELECT :ICI_ID FROM rdb$database) AS "RowNumber" 
	 FROM objQual_P_MEMBERSHIP m  
	   WHERE m.USERNAMELWD = LOWER(:I_USERNAME)       
		 INTO 
  :"UserID" ,
  :"ApplicationID",
  :"Username" ,
  :"UsernameLwd",
  :"Password",
  :"PasswordSalt",
  :"PasswordFormat",
  :"Email",
  :"EmailLwd",
  :"PasswordQuestion",
  :"PasswordAnswer",
  :"IsApproved",
  :"IsLockedOut",
  :"LastLogin",
  :"LastActivity",
  :"LastPasswordChange",
  :"LastLockOut",
  :"FailedPasswordAttempts",
  :"FailedAnswerAttempts",
  :"FailedPasswordWindow",
  :"FailedAnswerWindow",
  :"Joined",
  :"Comment",
  :"RowNumber" 
  DO
  begin
  ICI_ID = ICI_ID+1;
   SUSPEND;
  end 
   
END;
--GO 

 CREATE OR ALTER PROCEDURE  objQual_P_GETUSERNAMEBYEMAL
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_EMAIL VARCHAR(128)
 )
 RETURNS ("Username" VARCHAR(128))
 AS
 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN
	
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
 
	FOR SELECT m.USERNAME FROM objQual_P_MEMBERSHIP m 
	INNER JOIN objQual_P_APPLICATION a 
	ON m.APPLICATIONID = a.APPLICATIONID  
	WHERE a.APPLICATIONID = :ICI_APPLICATIONID16 
	AND m.EMAILLWD = LOWER(:I_EMAIL)
	INTO :"Username" 	
	DO
	SUSPEND;
END;
--GO 

 CREATE OR ALTER PROCEDURE  objQual_P_GETNUMBEROFUSERSONLINE
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_TIMEWINDOW INTEGER,
 I_CURRENTTIMEUTC TIMESTAMP
 )
 RETURNS (ici_NumberActive  INTEGER)
 AS
	DECLARE VARIABLE ici_ActivityDate TIMESTAMP;
	DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN 

	  -- ici_ActivityDate = DATEDIFF(i_CurrentTimeUTC, INTERVAL I_TIMEWINDOW DAY);

	ici_ActivityDate =DATEDIFF(DAY, current_date,(cast(:i_CurrentTimeUTC as date) -5)) ;
	
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
	
	
	 SELECT COUNT(1) FROM objQual_P_MEMBERSHIP m 
	 INNER JOIN objQual_P_APPLICATION a 
	 ON m.APPLICATIONID = a.APPLICATIONID  
	 WHERE a.APPLICATIONID = :ICI_APPLICATIONID16 
	 AND m.LASTLOGIN >= :ici_ActivityDate 
	 INTO :ici_NumberActive;    
	 -- SUSPEND; 
END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_UPDATEUSER
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERKEY VARCHAR(64),
 I_USERNAME VARCHAR(128),
 I_EMAIL VARCHAR(128),
 I_COMMENT BLOB SUB_TYPE 1 SEGMENT SIZE 80,
 I_ISAPPROVED BOOL,
 I_LASTLOGIN TIMESTAMP,
 I_LASTACTIVITY TIMESTAMP,
 I_UNIQUEEMAIL BOOL
 )
 AS
	DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
	DECLARE VARIABLE i_Return INTEGER DEFAULT 1;
 BEGIN 	
 
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
	
		/* Check UserKey */

		IF (I_USERKEY IS NULL) THEN
		begin
		 i_Return = 1 ; EXIT; 
		end
 
	/* Check for UniqueEmail */
	IF (I_UNIQUEEMAIL = 1) THEN 	
		IF (EXISTS (SELECT 1 FROM objQual_P_MEMBERSHIP m 
		WHERE m.USERID != CHAR_TO_UUID(:I_USERKEY) 
		AND m.EMAILLWD=LOWER(:I_EMAIL) 
		AND m.APPLICATIONID=:ICI_APPLICATIONID16) ) THEN
		begin
	   i_Return = 2; EXIT; 			
		end
	
	UPDATE objQual_P_MEMBERSHIP SET
	USERNAME = :I_USERNAME,
	USERNAMELWD = LOWER(:I_USERNAME),
	"EMAIL" = :I_EMAIL,
	EMAILLWD = LOWER(:I_EMAIL),
	ISAPPROVED = :I_ISAPPROVED,
	LASTLOGIN = :I_LASTLOGIN,
	LASTACTIVITY = :I_LASTACTIVITY,
	COMMENT =:I_COMMENT
	WHERE APPLICATIONID = :ICI_APPLICATIONID16 AND
	USERID = CHAR_TO_UUID(:I_USERKEY);
 
	/* Return successful */
	 i_Return = 0;        
END;
--GO

/* ROLE PROVIDER */

CREATE OR ALTER PROCEDURE  objQual_P_ROLE_DELETEROLE
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_ROLENAME VARCHAR(128),
 I_DELETEONLYIFROLEISEMPTY BOOL
 )
 RETURNS (ICI_ERRORCODE INTEGER)
 AS
   DECLARE VARIABLE ICI_ROLEID16 CHAR(16) CHARACTER SET OCTETS;
   DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN 	
	
	  EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16;
	
	ICI_ERRORCODE = 0;
	SELECT r.ROLEID FROM objQual_P_ROLE r 
	WHERE r.ROLENAMELWD=LOWER(:I_ROLENAME) 
	AND r.APPLICATIONID = :ICI_APPLICATIONID16 INTO :ICI_ROLEID16;
	
	IF (I_DELETEONLYIFROLEISEMPTY <> 0) THEN
			begin
			  IF (EXISTS (SELECT 1 FROM objQual_P_ROLEMEMBERSHIP rm WHERE rm.ROLEID=:ICI_ROLEID16)) THEN
				  ICI_ERRORCODE = 2; 
			end
	ELSE
		DELETE FROM objQual_P_ROLEMEMBERSHIP WHERE ROLEID = :ICI_ROLEID16;
	
 
	IF (ICI_ERRORCODE = 0) THEN
		DELETE FROM objQual_P_ROLE WHERE ROLEID =:ICI_ROLEID16;    
 END;

--GO

CREATE OR ALTER PROCEDURE  objQual_P_ROLE_CREATEROLE
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_ROLENAME VARCHAR(128)
 )
 AS
  DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN 	
	
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
	
	IF (NOT EXISTS(SELECT 1 FROM objQual_P_ROLE r WHERE r.APPLICATIONID = :ICI_APPLICATIONID16 
	AND r.ROLENAMELWD = LOWER(:I_ROLENAME))) THEN
		INSERT INTO objQual_P_ROLE (ROLEID, APPLICATIONID, ROLENAME, ROLENAMELWD) 
		VALUES (GEN_UUID(),:ICI_APPLICATIONID16, :I_ROLENAME,LOWER(:I_ROLENAME));
				
END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_ROLE_FINDUSERSINROLE
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_ROLENAME VARCHAR(128)
 )
 RETURNS( "UserID" VARCHAR(64),
  "ApplicationID" VARCHAR(64),
  "Username" VARCHAR(128) ,
  "UsernameLwd" VARCHAR(128) ,
  "Password" VARCHAR(128),
  "PasswordSalt" VARCHAR(128),
  "PasswordFormat" VARCHAR(128),
  "Email" VARCHAR(128),
  "EmailLwd" VARCHAR(128),
  "PasswordQuestion" VARCHAR(128),
  "PasswordAnswer" VARCHAR(128),
  "IsApproved" SMALLINT,
  "IsLockedOut" SMALLINT,
  "LastLogin" TIMESTAMP,
  "LastActivity" TIMESTAMP,
  "LastPasswordChange" TIMESTAMP,
  "LastLockOut" TIMESTAMP,
  "FailedPasswordAttempts" INTEGER,
  "FailedAnswerAttempts" INTEGER,
  "FailedPasswordWindow" TIMESTAMP,
  "FailedAnswerWindow" TIMESTAMP,
  "Joined" TIMESTAMP,
  "Comment" BLOB SUB_TYPE 1)
 AS
	DECLARE VARIABLE ICI_ROLEID16 CHAR(16) CHARACTER SET OCTETS;
	DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN
	
	
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
 
	SELECT r.ROLEID FROM objQual_P_ROLE r 
	INNER JOIN objQual_P_APPLICATION a 
	ON r.APPLICATIONID = a.APPLICATIONID 
	WHERE r.ROLENAMELWD=LOWER(:I_ROLENAME) 
	AND a.APPLICATIONID = :ICI_APPLICATIONID16 
	INTO :ici_RoleID16;
 
	FOR SELECT 
	UUID_TO_CHAR(m.USERID)   AS "UserID",
	UUID_TO_CHAR(m.APPLICATIONID)  AS "ApplicationID",
	 m.USERNAME,
	 m.USERNAMELWD,
	 m."PASSWORD",
	 m.PASSWORDSALT,
	 m.PASSWORDFORMAT,
	 m."EMAIL",
	 m.EMAILLWD,
	 m.PASSWORDQUESTION,
	 m.PASSWORDANSWER,
	 m.ISAPPROVED,
	 m.ISLOCKEDOUT,
	 m.LASTLOGIN, 	 
	 m.LASTACTIVITY, 	 
	 m.LASTPASSWORDCHANGE, 	 
	 m.LASTLOCKOUT, 	 
	 m.FAILEDPASSWORDATTEMPTS, 	
	 m.FAILEDANSWERATTEMPTS,
	 m.FAILEDPASSWORDWINDOW,
	 m.FAILEDANSWERWINDOW,
	 m.JOINED,
	 m."COMMENT"   FROM objQual_P_MEMBERSHIP m 
	 INNER JOIN objQual_P_ROLEMEMBERSHIP rm 
	 ON m.USERID = rm.USERID 
	 WHERE rm.ROLEID = :ici_RoleID16
	 INTO
	   :"UserID" ,
  :"ApplicationID",
  :"Username" ,
  :"UsernameLwd",
  :"Password",
  :"PasswordSalt",
  :"PasswordFormat",
  :"Email",
  :"EmailLwd",
  :"PasswordQuestion",
  :"PasswordAnswer",
  :"IsApproved",
  :"IsLockedOut",
  :"LastLogin",
  :"LastActivity",
  :"LastPasswordChange",
  :"LastLockOut",
  :"FailedPasswordAttempts",
  :"FailedAnswerAttempts",
  :"FailedPasswordWindow",
  :"FailedAnswerWindow",
  :"Joined",
  :"Comment"
		DO
		begin
		SUSPEND;
		end
END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_ROLE_ISUSERINROLE
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128),
 I_ROLENAME VARCHAR(128)
 )
  RETURNS( "UserID" VARCHAR(64),
  "ApplicationID" VARCHAR(64),
  "Username" VARCHAR(128) ,
  "UsernameLwd" VARCHAR(128) ,
  "Password" VARCHAR(128),
  "PasswordSalt" VARCHAR(128),
  "PasswordFormat" VARCHAR(128),
  "Email" VARCHAR(128),
  "EmailLwd" VARCHAR(128),
  "PasswordQuestion" VARCHAR(128),
  "PasswordAnswer" VARCHAR(128),
  "IsApproved" SMALLINT,
  "IsLockedOut" SMALLINT,
  "LastLogin" TIMESTAMP,
  "LastActivity" TIMESTAMP,
  "LastPasswordChange" TIMESTAMP,
  "LastLockOut" TIMESTAMP,
  "FailedPasswordAttempts" INTEGER,
  "FailedAnswerAttempts" INTEGER,
  "FailedPasswordWindow" TIMESTAMP,
  "FailedAnswerWindow" TIMESTAMP,
  "Joined" TIMESTAMP,
  "Comment" BLOB SUB_TYPE 1)
 AS
 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN 	
 
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
 
 FOR SELECT UUID_TO_CHAR(m.USERID)  AS "UserID",
	UUID_TO_CHAR(m.APPLICATIONID) AS "ApplicationID",
	 m.USERNAME,
	 m.USERNAMELWD,
	 m."PASSWORD",
	 m.PASSWORDSALT,
	 m.PASSWORDFORMAT,
	 m."EMAIL",
	 m.EMAILLWD,
	 m.PASSWORDQUESTION,
	 m.PASSWORDANSWER,
	 m.ISAPPROVED,
	 m.ISLOCKEDOUT,
	 m.LASTLOGIN, 	 
	 m.LASTACTIVITY, 	 
	 m.LASTPASSWORDCHANGE, 	 
	 m.LASTLOCKOUT, 	 
	 m.FAILEDPASSWORDATTEMPTS, 	
	 m.FAILEDANSWERATTEMPTS,
	 m.FAILEDPASSWORDWINDOW,
	 m.FAILEDANSWERWINDOW,
	 m.JOINED,
	 m."COMMENT" FROM objQual_P_ROLEMEMBERSHIP rm 
		INNER JOIN objQual_P_MEMBERSHIP m ON rm.USERID = m.USERID
		INNER JOIN objQual_P_ROLE r ON rm.ROLEID = r.ROLEID
		WHERE m.USERNAMELWD=LOWER(:I_USERNAME) 
		AND r.ROLENAMELWD =LOWER(:I_ROLENAME) 
		AND r.APPLICATIONID = :ICI_APPLICATIONID16
		INTO
		  :"UserID" ,
  :"ApplicationID",
  :"Username" ,
  :"UsernameLwd",
  :"Password",
  :"PasswordSalt",
  :"PasswordFormat",
  :"Email",
  :"EmailLwd",
  :"PasswordQuestion",
  :"PasswordAnswer",
  :"IsApproved",
  :"IsLockedOut",
  :"LastLogin",
  :"LastActivity",
  :"LastPasswordChange",
  :"LastLockOut",
  :"FailedPasswordAttempts",
  :"FailedAnswerAttempts",
  :"FailedPasswordWindow",
  :"FailedAnswerWindow",
  :"Joined",
  :"Comment"
  DO
		begin
		SUSPEND;
		end
END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_ROLE_ADDUSERTOROLE
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128),
 I_ROLENAME VARCHAR(128)
 )
 AS 
	DECLARE VARIABLE ICI_USERID16 CHAR(16) CHARACTER SET OCTETS;
	DECLARE VARIABLE ICI_ROLEID16 CHAR(16) CHARACTER SET OCTETS;
	DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN
 
	
	EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
 
	SELECT m.USERID FROM objQual_P_MEMBERSHIP m 
	WHERE m.USERNAMELWD=LOWER(:I_USERNAME) 
	AND m.APPLICATIONID = :ICI_APPLICATIONID16 
	INTO :ici_UserID16;
	
	SELECT r.ROLEID FROM objQual_P_ROLE r 
	WHERE r.ROLENAMELWD=LOWER(:I_ROLENAME) 
	AND r.APPLICATIONID = :ICI_APPLICATIONID16 
	INTO :ici_RoleID16;
	
	IF (NOT EXISTS(SELECT 1 FROM objQual_P_ROLEMEMBERSHIP rm 
	WHERE rm.USERID=:ici_UserID16 
	AND rm.ROLEID=:ici_RoleID16)) 
	THEN 
	INSERT INTO objQual_P_ROLEMEMBERSHIP (ROLEID, USERID)
	VALUES (:ici_RoleID16, :ici_UserID16);
END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_ROLE_EXISTS
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_ROLENAME VARCHAR(128)
 )
 RETURNS (OUT_EXISTS INTEGER) 
 AS
 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN	
 
EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
			
	SELECT COUNT(1) FROM objQual_P_ROLE
		WHERE ROLENAMELWD = LOWER(:I_ROLENAME) 
		AND APPLICATIONID = :ICI_APPLICATIONID16 INTO :OUT_EXISTS;
END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_ROLE_GETROLES
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128)
 )
 RETURNS
 ("RoleID" VARCHAR(64),
  "ApplicationID" VARCHAR(64),
  "RoleName" VARCHAR(128),
  "RoleNameLwd"VARCHAR(128))
 AS
DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN
	
	
EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16;  
	IF (I_USERNAME is null) THEN
	FOR	SELECT 
		UUID_TO_CHAR(r.ROLEID) AS "RoleID",
		UUID_TO_CHAR(r.APPLICATIONID) AS "ApplicationID",
			r.ROLENAME,
			r.ROLENAMELWD FROM objQual_P_ROLE r 
		WHERE r.APPLICATIONID = :ICI_APPLICATIONID16
		INTO 
		:"RoleID",
		:"ApplicationID",
		:"RoleName",
		:"RoleNameLwd"
		DO SUSPEND;
	ELSE
		FOR SELECT
		UUID_TO_CHAR(r.ROLEID) AS "RoleID",
		UUID_TO_CHAR(r.APPLICATIONID) AS "ApplicationID",
			r.ROLENAME,
			r.ROLENAMELWD
		FROM
			objQual_P_ROLE r
		INNER JOIN
			objQual_P_ROLEMEMBERSHIP rm ON r.ROLEID = rm.ROLEID
		INNER JOIN
			objQual_P_MEMBERSHIP m ON m.USERID = rm.USERID
		WHERE
			r.APPLICATIONID  = :ICI_APPLICATIONID16
			AND m.USERNAMELWD = LOWER(:I_USERNAME)
			INTO 
		:"RoleID",
		:"ApplicationID",
		:"RoleName",
		:"RoleNameLwd"
		DO SUSPEND;
END;
--GO 

CREATE OR ALTER PROCEDURE  objQual_P_ROLE_REMUSERFRROLE
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128),
 I_ROLENAME VARCHAR(128)
 )
 AS  
	DECLARE VARIABLE ICI_USERID16 CHAR(16) CHARACTER SET OCTETS;
	DECLARE VARIABLE ICI_ROLEID16 CHAR(16) CHARACTER SET OCTETS;
	DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN
	
 
EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
			
	SELECT r.ROLEID FROM objQual_P_ROLE r 
	WHERE r.ROLENAMELWD = LOWER(:I_ROLENAME) 
	AND r.APPLICATIONID = :ICI_APPLICATIONID16 
	INTO :ICI_ROLEID16;
	
	SELECT m.USERID FROM objQual_P_MEMBERSHIP m 
	WHERE m.USERNAMELWD=LOWER(:I_USERNAME) 
	AND m.APPLICATIONID = :ICI_APPLICATIONID16 
	INTO :ICI_USERID16;
	
	DELETE FROM objQual_P_ROLEMEMBERSHIP 
	WHERE ROLEID = :ICI_ROLEID16 
	AND USERID=:ICI_USERID16;
	
END;
--GO 


/* PROFILES PROVIDER */

CREATE OR ALTER PROCEDURE  objQual_P_PROFILE_DELETEINACTIVE
 (
 I_APPLICATIONNAME VARCHAR(128),
 I_INACTIVESINCEDATE TIMESTAMP
 )
 RETURNS ("RowCount" INTEGER)
 AS
 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN
	
 
EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16;  
	 DELETE
	 FROM    objQual_P_PROFILE
	 WHERE   USERID IN
			 (   SELECT  m.USERID
				 FROM    objQual_P_MEMBERSHIP m
				 WHERE   m.APPLICATIONID = :ICI_APPLICATIONID16
						 AND (m.LASTACTIVITY <= :I_INACTIVESINCEDATE)
			 );
 
	 "RowCount"=ROW_COUNT;
END;
--GO 

--TODO

 -- DROP PROCEDURE IF EXISTS objQual_P_profile_getnumberinactiveprofiles;
 CREATE OR ALTER PROCEDURE  objQual_P_PROFILE_GETNUMINACT
	( I_APPLICATIONNAME        VARCHAR(128),
	 I_INACTIVESINCEDATE      TIMESTAMP)
	 RETURNS ("RowCount" INTEGER)
AS
DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN
	
 
EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
		 
	 SELECT  COUNT(*)
	 FROM    objQual_P_MEMBERSHIP m, objQual_P_PROFILE p
	 WHERE   APPLICATIONID = :ICI_APPLICATIONID16
		 AND m.USERID = p.USERID
		 AND (LASTACTIVITY <= :I_INACTIVESINCEDATE) INTO :"RowCount";
END;
--GO 
CREATE OR ALTER PROCEDURE  objQual_P_PROFILE_DELETEPROFILE
(
 I_APPLICATIONNAME VARCHAR(128),
 I_USERNAME VARCHAR(128)
 )
 RETURNS(ici_NumDeleted INTEGER)
 AS 
	 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
 BEGIN
 EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16; 
			DELETE FROM objQual_P_PROFILE 
			WHERE USERID IN (SELECT USERID 
			FROM objQual_P_MEMBERSHIP 
			WHERE USERNAMELWD = LOWER(:I_USERNAME) 
			AND APPLICATIONID = :ICI_APPLICATIONID16); 
			/* IF( ici_ici_ERROR <> 0 ) THEN           
				 SET ici_ErrorCode = -1
				 GOTO Error
			 END IF;*/
			 IF (ROW_COUNT <> 0) THEN
			 ici_NumDeleted = ici_NumDeleted + 1;      
 END;
--GO


CREATE OR ALTER PROCEDURE  objQual_P_PROFILE_GETPROFILES
(
	I_APPLICATIONNAME VARCHAR(128) CHARACTER SET UTF8,
	I_PROFILEAUTHOPTIONS INTEGER,
	I_USERNAMETOMATCH VARCHAR(128) CHARACTER SET UTF8,
	I_INACTIVESINCEDATE TIMESTAMP,
	I_PAGEINDEX INTEGER,
	I_PAGESIZE INTEGER)    
   RETURNS 
	(
  "TotalCount" INTEGER, 
  "UserName" VARCHAR(255) CHARACTER SET UTF8,
  "LastActivity" TIMESTAMP,
  "UserID" VARCHAR(64), 
  VALUEINDEX BLOB SUB_TYPE 1,
  STRINGDATA BLOB SUB_TYPE 1,
  BINARYDATA BLOB SUB_TYPE 0,
  "LastUpdatedDate" TIMESTAMP  
	)
 AS
	 DECLARE VARIABLE ICI_APPLICATIONID16 CHAR(16) CHARACTER SET OCTETS;
	 DECLARE VARIABLE pkid char(36) character set octets;
	 DECLARE VARIABLE spkid char(16) character set octets;
	 DECLARE VARIABLE I_PAGELOWERBOUND INTEGER;
	 DECLARE VARIABLE I_PAGEUPPERBOUND INTEGER;
	 DECLARE VARIABLE I_TOTALRECORDS   INTEGER;
 BEGIN 	
 
	 /*Set the page bounds*/    
   

EXECUTE PROCEDURE objQual_P_CREATEAPPLICATION :I_APPLICATIONNAME
		RETURNING_VALUES :ICI_APPLICATIONID16;         


	I_PAGELOWERBOUND = I_PAGESIZE*I_PAGEINDEX;
	I_PAGEUPPERBOUND= I_PAGESIZE + I_PAGELOWERBOUND;
	
   SELECT COUNT(m.USERNAME) FROM    objQual_P_MEMBERSHIP m, 
	 objQual_P_PROFILE p
	 WHERE   m.USERID = p.USERID 
	  AND (:I_INACTIVESINCEDATE IS NULL 
	  OR LASTACTIVITY <= :I_INACTIVESINCEDATE)
			 AND (:I_USERNAMETOMATCH IS NULL 
			 OR m.USERNAMELWD LIKE LOWER(:I_USERNAMETOMATCH)) 
   INTO :I_TOTALRECORDS;
 
   FOR SELECT  FIRST(:I_PAGEUPPERBOUND) SKIP(:I_PAGELOWERBOUND) 
   (SELECT  :I_TOTALRECORDS FROM RDB$DATABASE),
   m.USERNAME,
   m.LASTACTIVITY, 
   UUID_TO_CHAR(p.USERID) AS "UserID",
   p.VALUEINDEX,
   p.STRINGDATA,
   p.BINARYDATA,
   p.LASTUPDATEDDATE
	FROM    objQual_P_MEMBERSHIP m, 
	 objQual_P_PROFILE p
	 WHERE   m.USERID = p.USERID 
	  AND (:I_INACTIVESINCEDATE IS NULL 
	  OR LASTACTIVITY <= :I_INACTIVESINCEDATE)
			 AND (:I_USERNAMETOMATCH IS NULL 
			 OR m.USERNAMELWD LIKE LOWER(:I_USERNAMETOMATCH))
		 ORDER BY m.USERNAME 
   INTO 
   :"TotalCount",
   :"UserName", 
   :"LastActivity", 
   :"UserID",
   :VALUEINDEX,
   :STRINGDATA,
   :BINARYDATA,
   :"LastUpdatedDate"
  DO          
	SUSPEND;
	
  END; 
 
--GO 
