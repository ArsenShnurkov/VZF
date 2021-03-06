
/* Yet Another Forum.NET Firebird data layer by vzrus
 * Copyright (C) 2006-2016 Vladimir Zakharov
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

EXECUTE BLOCK
AS
BEGIN
-- Source table: {objectQualifier}ACCESSMASK
IF (NOT EXISTS(SELECT 1 
               FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}ACCESSMASK' 
               ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}ACCESSMASK
                       (
                       ACCESSMASKID         INTEGER DEFAULT 0 NOT NULL,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       NAME                 VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       FLAGS                INTEGER DEFAULT 0 NOT NULL,
                       SORTORDER            INTEGER DEFAULT 0 NOT NULL,
                       CREATEDBYUSERID      INTEGER,
                       CREATEDBYUSERNAME    VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       CREATEDBYUSERDISPLAYNAME  VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       CREATEDDATE          TIMESTAMP,
                       ISUSERMASK           SMALLINT  DEFAULT 0 NOT NULL,
                       ISADMINMASK          SMALLINT  DEFAULT 0 NOT NULL					   					    
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN
-- Source table: {objectQualifier}ACCESSMASKHISTORY
IF (NOT EXISTS(SELECT 1 
               FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}ACCESSMASKHISTORY' 
               ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}ACCESSMASKHISTORY
                       (
                       ACCESSMASKID         INTEGER DEFAULT 0 NOT NULL,
                       CHANGEDUSERID		    INTEGER,	
                       CHANGEDUSERNAME		VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       CHANGEDDISPLAYNAME	VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       CHANGEDDATE          TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL 
                       );';
END
--GO

/* EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}WARNING 
IF (NOT EXISTS(SELECT 1 FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}WARNING' ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}WARNING 
                       (
                       WARNINGID            INTEGER DEFAULT 0 NOT NULL,
					   FROMUSERID           INTEGER DEFAULT 0 NOT NULL,
					   TOUSERID             INTEGER DEFAULT 0 NOT NULL, 
					   DESCRIPTION          INTEGER DEFAULT 0 NOT NULL,  
					   SHARE100             INTEGER DEFAULT 0 NOT NULL,                                  
                       BANNEDUPTO           TIMESTAMP
                       );';
END
*/

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}ACTIVE
IF (NOT EXISTS(SELECT 1 FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}ACTIVE' ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}ACTIVE 
                       (
                       SESSIONID            VARCHAR(36) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       IP                   VARCHAR(39) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       LOGIN                TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL,
                       LASTACTIVE           TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL,
                       LOCATION             VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       FORUMID              INTEGER,
                       TOPICID              INTEGER,
                       BROWSER              VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       PLATFORM             VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       FORUMPAGE            VARCHAR(1024) CHARACTER SET UTF8 COLLATE UNICODE,
                       FLAGS                INTEGER NOT NULL, 
                       ISACTIVE             SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 1))),
                       ISGUEST              SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 2))),
                       ISREGISTERED         SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 4))),
                       ISCRAWLER            SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 8)))
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}ACTIVEACCESS
IF (NOT EXISTS(SELECT 1 FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}ACTIVEACCESS' 
               ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}ACTIVEACCESS
                       (
                       USERID			    INTEGER NOT NULL ,
                       BOARDID			    INTEGER NOT NULL ,			
                       FORUMID			    INTEGER NOT NULL,
                       ISADMIN				SMALLINT DEFAULT 0 NOT NULL,
                       ISFORUMMODERATOR	    SMALLINT DEFAULT 0 NOT NULL,
                       ISMODERATOR			SMALLINT DEFAULT 0 NOT NULL,
                       ISGUESTX			    SMALLINT DEFAULT 0 NOT NULL,
                       LASTACTIVE			TIMESTAMP,
                       READACCESS			SMALLINT NOT NULL ,
                       POSTACCESS			SMALLINT NOT NULL ,
                       REPLYACCESS		    SMALLINT NOT NULL,
                       PRIORITYACCESS		SMALLINT NOT NULL,
                       POLLACCESS			SMALLINT NOT NULL,
                       VOTEACCESS			SMALLINT NOT NULL,
                       MODERATORACCESS		SMALLINT NOT NULL,
                       EDITACCESS			SMALLINT NOT NULL,
                       DELETEACCESS		    SMALLINT NOT NULL,
                       UPLOADACCESS		    SMALLINT NOT NULL,		
                       DOWNLOADACCESS		SMALLINT NOT NULL,
                       USERFORUMACCESS      SMALLINT NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}ADMINPAGEUSERACCESS
IF (NOT EXISTS(SELECT 1 FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}ADMINPAGEUSERACCESS' ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}ADMINPAGEUSERACCESS 
                       (
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       PAGENAME             VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}EVENTLOGGROUPACCESS
IF (NOT EXISTS(SELECT 1 FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}EVENTLOGGROUPACCESS' ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}EVENTLOGGROUPACCESS 
                       (
                       GROUPID              INTEGER DEFAULT 0 NOT NULL,
                       EVENTTYPEID          INTEGER DEFAULT 0 NOT NULL,
                       EVENTTYPENAME        VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       DELETEACCESS         SMALLINT DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}ATTACHMENT
IF (NOT EXISTS(SELECT 1 FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}ATTACHMENT' ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}ATTACHMENT 
                       (
                       ATTACHMENTID         INTEGER DEFAULT 0 NOT NULL,
                       MESSAGEID            INTEGER DEFAULT 0 NOT NULL,
                       FILENAME             VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       BYTES                INTEGER DEFAULT 0 NOT NULL,
                       FILEID               INTEGER,
                       CONTENTTYPE          VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       DOWNLOADS            INTEGER DEFAULT 0 NOT NULL,
                       FILEDATA             BLOB SUB_TYPE 0 SEGMENT SIZE 80
                       );';
END
--GO

/* EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}ANNOUNCEMENTPOSITION
IF (NOT EXISTS(SELECT 1 FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}ANNOUNCEMENTPOSITION' ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}ANNOUNCEMENTPOSITION 
                       (
                       ANNOUNCEMENTID       INTEGER DEFAULT 0 NOT NULL,
					   MESSAGEID            INTEGER DEFAULT 0 NOT NULL,
					   BOARDID              INTEGER DEFAULT 0 NOT NULL, 
					   CATEGORYID           INTEGER DEFAULT 0 NOT NULL,  
					   FORUMID              INTEGER DEFAULT 0 NOT NULL,                                  
                       SHOWDESCRIPTION      SMALLINT DEFAULT 0 NOT NULL,
					   DESCRIPTIONLIMIT     INTEGER DEFAULT 0 NOT NULL,
                       SHOWCONTENT          SMALLINT DEFAULT 0 NOT NULL,
					   CONTENTLIMIT         INTEGER DEFAULT 0 NOT NULL,
                       FIRSTPOSITION        INTEGER DEFAULT 0 NOT NULL,
                       REPEATEIN            INTEGER DEFAULT 0 NOT NULL,					 
					   REPEATENUMBER        INTEGER DEFAULT 0 NOT NULL,
					   ALLCATEGORIES        SMALLINT DEFAULT 0 NOT NULL,
					   ALLFORUMS            SMALLINT DEFAULT 0 NOT NULL,
					   ALLTOPICS            SMALLINT DEFAULT 0 NOT NULL,
					   BEGINIT              TIMESTAMP,
					   ENDIT                TIMESTAMP,
					   ANNOUNCEMENTGROUPID  INTEGER DEFAULT 0 NOT NULL
                       );';
END
*/



EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}BANNEDIP
IF (NOT EXISTS(SELECT FIRST 1 1 FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}BANNEDIP')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}BANNEDIP
                       (
                       ID                   INTEGER DEFAULT 0,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       MASK                 VARCHAR(57) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       SINCE                TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL,
                       REASON               VARCHAR(255) CHARACTER SET  UTF8 COLLATE UNICODE,
                       USERID	            INTEGER
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}BBCODE
IF (NOT EXISTS(SELECT FIRST 1 1 FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}BBCODE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}BBCODE
                       (
                       BBCODEID             INTEGER DEFAULT 0 NOT NULL,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       NAME                 VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       DESCRIPTION          BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET UTF8 COLLATE UNICODE,
                       ONCLICKJS            BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET UTF8 COLLATE UNICODE,
                       DISPLAYJS            BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET UTF8 COLLATE UNICODE,
                       EDITJS               BLOB SUB_TYPE 1 SEGMENT SIZE 20 CHARACTER SET UTF8 COLLATE UNICODE,
                       DISPLAYCSS           BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET UTF8 COLLATE UNICODE,
                       SEARCHREGEX          BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET UTF8 COLLATE UNICODE,
                       REPLACEREGEX         BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET UTF8 COLLATE UNICODE,
                       VARIABLES            BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET UTF8 COLLATE UNICODE,
                       USEMODULE            SMALLINT,
                       MODULECLASS          VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       EXECORDER            INTEGER DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}BOARD
IF (NOT EXISTS(SELECT FIRST 1 1 FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}BOARD')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}BOARD
                       (
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       NAME                 VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       ALLOWTHREADED        SMALLINT DEFAULT 0 NOT NULL,
                       MEMBERSHIPAPPNAME    VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       ROLESAPPNAME         VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}CATEGORY
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}CATEGORY')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}CATEGORY 
                       (
                       CATEGORYID           INTEGER DEFAULT 0,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       NAME                 VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       CATEGORYIMAGE        VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       SORTORDER            INTEGER DEFAULT 0 NOT NULL,
                       POLLGROUPID          INTEGER,
					   CANHAVEPERSFORUMS    SMALLINT DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}CHECKEMAIL
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}CHECKEMAIL')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}CHECKEMAIL 
                       (
                       CHECKEMAILID         INTEGER DEFAULT 0,
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       "EMAIL"              VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       CREATED              TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL,
                       "HASH"               VARCHAR(32) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE 
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}CHOICE
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}CHOICE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}CHOICE
                       (
                       CHOICEID             INTEGER DEFAULT 0 NOT NULL,
                       POLLID               INTEGER DEFAULT 0 NOT NULL,
                       CHOICE               VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       VOTES                INTEGER DEFAULT 0 NOT NULL,
                       OBJECTPATH           VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       MIMETYPE             VARCHAR(50) CHARACTER SET UTF8 COLLATE UNICODE
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}EVENTLOG
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}EVENTLOG')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}EVENTLOG 
                       (
                       EVENTLOGID           INTEGER DEFAULT 0 NOT NULL,
                       EVENTTIME            TIMESTAMP DEFAULT current_timestamp,
                       USERID               INTEGER,
                       SOURCE               VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       DESCRIPTION          BLOB SUB_TYPE 1 SEGMENT SIZE 80 NOT NULL,
                       "TYPE"               INTEGER DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}EXTENSION
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}EXTENSION')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}EXTENSION 
                       (
                       EXTENSIONID          INTEGER DEFAULT 0 NOT NULL,
                       BOARDID              INTEGER DEFAULT 1 NOT NULL,
                       EXTENSION            VARCHAR(10) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE 
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}FORUM
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}FORUM')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}FORUM
                       (
                       FORUMID              INTEGER DEFAULT 0,
                       CATEGORYID           INTEGER DEFAULT 0 NOT NULL,
                       PARENTID             INTEGER,
                       NAME                 VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       DESCRIPTION          VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       IMAGEURL             VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       STYLES               VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       SORTORDER            INTEGER DEFAULT 0 NOT NULL,
                       LASTPOSTED           TIMESTAMP,
                       LASTTOPICID          INTEGER,
                       LASTMESSAGEID        INTEGER,
                       LASTUSERID           INTEGER,
                       LASTUSERNAME         VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       LASTUSERDISPLAYNAME  VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       NUMTOPICS            INTEGER DEFAULT 0 NOT NULL,
                       NUMPOSTS             INTEGER DEFAULT 0 NOT NULL,
                       REMOTEURL            VARCHAR(512) CHARACTER SET UTF8 COLLATE UNICODE,
                       FLAGS                INTEGER DEFAULT 0 NOT NULL,
                       THEMEURL             VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       POLLGROUPID          INTEGER,
                       USERID               INTEGER,
                       ISLOCKED  	        SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 1))),
                       ISHIDDEN             SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 2))),
                       ISNOCOUNT	        SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 4))),
                       ISMODERATED          SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 8))),
                       CREATEDBYUSERID      INTEGER,
                       CREATEDBYUSERNAME    VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       CREATEDBYUSERDISPLAYNAME  VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       CREATEDDATE  TIMESTAMP,
                       ISUSERFORUM SMALLINT DEFAULT 0 NOT NULL,
					   CANHAVEPERSFORUMS  SMALLINT DEFAULT 0 NOT NULL
                       );';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}FORUMHISTORY
IF (NOT EXISTS(SELECT 1 
               FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}FORUMHISTORY' 
               ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}FORUMHISTORY
                       (
                       FORUMID         INTEGER DEFAULT 0 NOT NULL,
                       CHANGEDUSERID		    INTEGER,	
                       CHANGEDUSERNAME		VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       CHANGEDDISPLAYNAME	VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       CHANGEDDATE          TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL 
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}FORUMACCESS
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}FORUMACCESS')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}FORUMACCESS 
                       (
                       GROUPID              INTEGER DEFAULT 0 NOT NULL,
                       FORUMID              INTEGER DEFAULT 0 NOT NULL,
                       ACCESSMASKID         INTEGER DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}GROUP
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}GROUP')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}GROUP 
                       (
                       GROUPID              INTEGER DEFAULT 0 NOT NULL,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       NAME                 VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       FLAGS                INTEGER DEFAULT 0 NOT NULL,
                       PMLIMIT              INTEGER DEFAULT 0 NOT NULL,
                       STYLE                VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       SORTORDER            INTEGER DEFAULT 0 NOT NULL,
                       DESCRIPTION          VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       USRSIGCHARS          INTEGER DEFAULT 0 NOT NULL,
                       USRSIGBBCODES        VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       USRSIGHTMLTAGS       VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       USRALBUMS            INTEGER DEFAULT 0 NOT NULL,
                       USRALBUMIMAGES       INTEGER DEFAULT 0 NOT NULL,
                       CREATEDBYUSERID      INTEGER,
                       CREATEDBYUSERNAME    VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       CREATEDBYUSERDISPLAYNAME  VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       CREATEDDATE          TIMESTAMP,
                       ISUSERGROUP          SMALLINT DEFAULT 0 NOT NULL,
                       ISHIDDEN             SMALLINT DEFAULT 0 NOT NULL,
					   USRPERSONALFORUMS    INTEGER DEFAULT 0 NOT NULL,
					   USRPERSONALMASKS     INTEGER DEFAULT 0 NOT NULL,
					   USRPERSONALGROUPS    INTEGER DEFAULT 0 NOT NULL
                       ) ;';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}GROUPHISTORY
IF (NOT EXISTS(SELECT 1 
               FROM RDB$RELATIONS a 
               WHERE a.RDB$RELATION_NAME='{objectQualifier}GROUPHISTORY' 
               ROWS 1)) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}GROUPHISTORY
                       (
                       GROUPID              INTEGER DEFAULT 0 NOT NULL,
                       CHANGEDUSERID		    INTEGER,	
                       CHANGEDUSERNAME		VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       CHANGEDDISPLAYNAME	VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       CHANGEDDATE          TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL 
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}GROUPMEDAL
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}GROUPMEDAL')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}GROUPMEDAL 
                       (
                       GROUPID              INTEGER DEFAULT 0 NOT NULL,
                       MEDALID              INTEGER DEFAULT 0 NOT NULL,
                       MESSAGE              VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       HIDE                 SMALLINT DEFAULT 0 NOT NULL,
                       ONLYRIBBON           SMALLINT DEFAULT 0 NOT NULL,
                       SORTORDER            INT DEFAULT 255 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}MAIL
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}MAIL')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}MAIL
                       (
                       MAILID               INTEGER DEFAULT 0 NOT NULL,
                       FROMUSER             VARCHAR(255) CHARACTER SET  UTF8 NOT NULL COLLATE UNICODE,
                       FROMUSERNAME         VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       TOUSER               VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       TOUSERNAME           VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       CREATED              TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL,
                       SUBJECT              VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       BODY                 BLOB SUB_TYPE 1 SEGMENT SIZE 80 NOT NULL,
                       BODYHTML             BLOB SUB_TYPE 1 SEGMENT SIZE 80,
                       SENDTRIES            INTEGER DEFAULT 0 NOT NULL,
                       SENDATTEMPT          TIMESTAMP,
                       PROCESSID            INTEGER
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}MEDAL
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}MEDAL')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}MEDAL
                       (
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       MEDALID              INTEGER DEFAULT 0 NOT NULL,
                       NAME                 VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       DESCRIPTION          BLOB SUB_TYPE 1 SEGMENT SIZE 80 NOT NULL,
                       MESSAGE              VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       CATEGORY             VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       MEDALURL             VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       RIBBONURL            VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       SMALLMEDALURL        VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       SMALLRIBBONURL       VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       SMALLMEDALWIDTH      INTEGER DEFAULT 0 NOT NULL,
                       SMALLMEDALHEIGHT     INTEGER DEFAULT 0 NOT NULL,
                       SMALLRIBBONWIDTH     INTEGER,
                       SMALLRIBBONHEIGHT    INTEGER,
                       SORTORDER INTEGER    DEFAULT 255 NOT NULL,
                       FLAGS INTEGER        DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}MESSAGE
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}MESSAGE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}MESSAGE
                       (
                       MESSAGEID            INTEGER DEFAULT 0 NOT NULL,
                       TOPICID              INTEGER DEFAULT 0 NOT NULL,
                       REPLYTO              INTEGER,
                       "POSITION"           INTEGER DEFAULT 0 NOT NULL,
                       INDENT               INTEGER DEFAULT 0 NOT NULL,
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       USERNAME             VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       USERDISPLAYNAME      VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       POSTED               TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL,
                       MESSAGE              BLOB SUB_TYPE 1 NOT NULL,
					   DESCRIPTION          VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       IP                   VARCHAR(39),
                       EDITED               TIMESTAMP,
                       FLAGS                INTEGER DEFAULT 23 NOT NULL,
                       EDITREASON           VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       DELETEREASON         VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       ISMODERATORCHANGED   SMALLINT DEFAULT 0,
                       BLOGPOSTID           VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       EXTERNALMESSAGEID    VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       REFERENCEMESSAGEID   VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       EDITEDBY             INTEGER,
					   ISDELETED	        SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 8))),
                       ISAPPROVED           SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 16)))
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}MESSAGEREPORTED
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}MESSAGEREPORTED')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}MESSAGEREPORTED
                       (
                       MESSAGEID INTEGER DEFAULT 0 NOT NULL,
                       MESSAGE BLOB SUB_TYPE 1 SEGMENT SIZE 80,
                       RESOLVED SMALLINT,
                       RESOLVEDBY INTEGER,
                       RESOLVEDDATE TIMESTAMP
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}MESSAGEREPORTEDAUDIT
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}MESSAGEREPORTEDAUDIT')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}MESSAGEREPORTEDAUDIT
                       (
                       LOGID                INTEGER DEFAULT 0 NOT NULL,
                       USERID               INTEGER NOT NULL,
                       MESSAGEID            INTEGER NOT NULL,
                       REPORTED             TIMESTAMP NOT NULL,
                       REPORTEDNUMBER       INTEGER  DEFAULT 1 NOT NULL,
                       REPORTTEXT           VARCHAR(4000)
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}NNTPFORUM
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}NNTPFORUM')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}NNTPFORUM
                       (
                       NNTPFORUMID          INTEGER DEFAULT 0 NOT NULL,
                       NNTPSERVERID         INTEGER DEFAULT 0 NOT NULL,
                       GROUPNAME            VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       FORUMID              INTEGER DEFAULT 0 NOT NULL,
                       LASTMESSAGENO        INTEGER DEFAULT 0 NOT NULL,
                       LASTUPDATE           TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL,
                       "ACTIVE"             SMALLINT DEFAULT 0 NOT NULL,
                       DATECUTOFF           TIMESTAMP
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}NNTPSERVER
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}NNTPSERVER')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}NNTPSERVER
                       (
                       NNTPSERVERID         INTEGER DEFAULT 0 NOT NULL,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       NAME                 VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       ADDRESS              VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       "PORT"               INTEGER,
                       USERNAME             VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       USERPASS             VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE 
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}NNTPTOPIC
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}NNTPTOPIC')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}NNTPTOPIC
                       (
                       NNTPTOPICID          INTEGER DEFAULT 0 NOT NULL,
                       NNTPFORUMID          INTEGER DEFAULT 0 NOT NULL,
                       "THREAD"             VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       TOPICID              INTEGER DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}PMESSAGE
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}PMESSAGE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}PMESSAGE
                       (
                       PMESSAGEID           INTEGER DEFAULT 0 NOT NULL,
                       FROMUSERID           INTEGER DEFAULT 0 NOT NULL,
                       CREATED              TIMESTAMP DEFAULT ''current_timestamp''NOT NULL,
                       SUBJECT              VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       BODY                 BLOB SUB_TYPE 1 SEGMENT SIZE 80,
                       FLAGS                INTEGER DEFAULT 0 NOT NULL,
                       REPLYTO              INTEGER
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}PMESSAGE
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}POLLGROUPCLUSTER')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}POLLGROUPCLUSTER
                       (
                       POLLGROUPID          INTEGER NOT NULL,
                       USERID               INTEGER NOT NULL,
                       FLAGS                INTEGER DEFAULT 0 NOT NULL,
                       ISBOUND              SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 2)))
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}POLL
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}POLL')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}POLL
                       (
                       POLLID               INTEGER DEFAULT 0 NOT NULL,
                       QUESTION             VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       CLOSES               TIMESTAMP,
                       POLLGROUPID          INTEGER,
                       USERID               INTEGER NOT NULL,
                       OBJECTPATH           VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       MIMETYPE             VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       FLAGS                INTEGER DEFAULT 0 NOT NULL,
                       ISCLOSEDBOUND	    SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 4))),
                       ALLOWMULTIPLECHOICES SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 8))),
                       SHOWVOTERS	        SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 16))),
                       ALLOWSKIPVOTE        SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 32)))
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}POLLVOTEREFUSE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}POLLVOTEREFUSE
                       (
                       REFUSEID             INTEGER NOT NULL,
                       POLLID               INTEGER,
                       USERID               INTEGER,
                       REMOTEIP             VARCHAR(57) CHARACTER SET UTF8 COLLATE UNICODE
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}POLLVOTE
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}POLLVOTE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}POLLVOTE
                       (
                       POLLVOTEID           INTEGER DEFAULT 0 NOT NULL,
                       POLLID               INTEGER DEFAULT 0 NOT NULL,
                       USERID               INTEGER,
                       REMOTEIP             VARCHAR(39) CHARACTER SET UTF8 COLLATE UNICODE,
                       CHOICEID             INTEGER
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}RANK
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
    WHERE a.RDB$RELATION_NAME='{objectQualifier}RANK')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}RANK
                       (
                       RANKID               INTEGER DEFAULT 0,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       NAME                 VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       DESCRIPTION          VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
					   MINPOSTS             INTEGER,
                       RANKIMAGE            VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       FLAGS                INTEGER DEFAULT 0 NOT NULL,
                       PMLIMIT              INTEGER DEFAULT 0 NOT NULL,
                       STYLE                VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       SORTORDER            INTEGER DEFAULT 0 NOT NULL,
                       USRSIGCHARS          INTEGER DEFAULT 0 NOT NULL,
                       USRSIGBBCODES        VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       USRSIGHTMLTAGS       VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       USRALBUMS            INTEGER DEFAULT 0 NOT NULL,
                       USRALBUMIMAGES       INTEGER DEFAULT 0 NOT NULL
                       ) ;';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}REGISTRY
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}REGISTRY')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}REGISTRY
                       (
                       REGISTRYID           INTEGER DEFAULT 0 NOT NULL,
                       NAME                 VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       "VALUE"              BLOB SUB_TYPE 1 SEGMENT SIZE 80,
                       BOARDID              INTEGER
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}REPLACE_WORDS
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}REPLACE_WORDS')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}REPLACE_WORDS
                       (
                       ID                   INTEGER DEFAULT 0 NOT NULL,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       BADWORD              VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       GOODWORD             VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE 
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}SHOUTBOXMESSAGE
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}SHOUTBOXMESSAGE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}SHOUTBOXMESSAGE
                       (
                       SHOUTBOXMESSAGEID    INTEGER NOT NULL,
                       BOARDID              INTEGER,
                       GROUPID              INTEGER,
                       USERID               INTEGER,
                       USERNAME             VARCHAR(255)  CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       USERDISPLAYNAME      VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       "MESSAGE"            BLOB SUB_TYPE 1 SEGMENT SIZE 80,
                       "DATE"               TIMESTAMP DEFAULT current_timestamp NOT NULL,
                       IP                   VARCHAR(39) NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}SMILEY
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}SMILEY')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}SMILEY
                       (
                       SMILEYID             INTEGER DEFAULT 0,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       "CODE"               VARCHAR(10) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       ICON                 VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       EMOTICON             VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       SORTORDER            INT DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}TOPIC
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}TOPIC')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}TOPIC 
                       (
                       TOPICID             INTEGER DEFAULT 0 NOT NULL,
                       FORUMID             INTEGER DEFAULT 0 NOT NULL,
                       USERID              INTEGER DEFAULT 0 NOT NULL,
                       USERNAME            VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       USERDISPLAYNAME     VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       POSTED              TIMESTAMP  DEFAULT current_timestamp NOT NULL,
                       TOPIC               VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       STATUS     	       VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       DESCRIPTION	       VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       VIEWS               INTEGER DEFAULT 0 NOT NULL,
                       "PRIORITY"          INTEGER DEFAULT 0 NOT NULL,
                       POLLID              INTEGER,
                       TOPICMOVEDID        INTEGER,
                       LASTPOSTED          TIMESTAMP,
                       LASTMESSAGEID       INTEGER,
                       LASTUSERID          INTEGER,
                       LASTUSERNAME        VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       LASTUSERDISPLAYNAME VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       NUMPOSTS            INTEGER DEFAULT 0 NOT NULL,
                       FLAGS               INTEGER DEFAULT 0 NOT NULL,
                       ISDELETED           SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 8))),
                       ISQUESTION          SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 1024))),
                       ANSWERMESSAGEID     INTEGER,
                       LASTMESSAGEFLAGS    INTEGER,
                       TOPICIMAGE          VARCHAR(255),
					   TOPICIMAGETYPE      VARCHAR(50),
					   TOPICIMAGEBIN       BLOB SUB_TYPE 0,
                       STYLES    	       VARCHAR(255),
                       LINKDATE    	       TIMESTAMP);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}USER
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}USER')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}USER
                       (
                       USERID               INTEGER DEFAULT 0,
                       BOARDID              INTEGER DEFAULT 0 NOT NULL,
                       PROVIDERUSERKEY      CHAR(16) CHARACTER SET OCTETS,
                       NAME                 VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       DISPLAYNAME          VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       "PASSWORD"           VARCHAR(32) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       "EMAIL"              VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       JOINED               TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
                       LASTVISIT            TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
                       IP                   VARCHAR(39) CHARACTER SET UTF8 COLLATE UNICODE,
                       NUMPOSTS             INTEGER DEFAULT 0 NOT NULL,
                       TIMEZONE             INTEGER DEFAULT 0 NOT NULL,
                       AVATAR               VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE,
                       SIGNATURE            BLOB SUB_TYPE 1 SEGMENT SIZE 80,
                       AVATARIMAGE          BLOB SUB_TYPE 0,
                       AVATARIMAGETYPE      VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       RANKID               INTEGER NOT NULL,
                       SUSPENDED            TIMESTAMP,
                       LANGUAGEFILE         VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       THEMEFILE            VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       USESINGLESIGNON      INTEGER DEFAULT 0 NOT NULL,
                       TEXTEDITOR           VARCHAR(50) CHARACTER SET UTF8 COLLATE UNICODE,
                       OVERRIDEDEFAULTTHEMES SMALLINT DEFAULT 0 NOT NULL,
                       PMNOTIFICATION       SMALLINT DEFAULT 1 NOT NULL,
                       NOTIFICATIONTYPE     INTEGER DEFAULT 10 NOT NULL,
                       FLAGS                INTEGER DEFAULT 0 NOT NULL,
                       POINTS               INTEGER DEFAULT 1 NOT NULL,
                       ISAPPROVED 	        SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 2))),
                       ISGUEST              SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 4))),
                       ISCAPTCHAEXCLUDED    SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 8))),
                       ISACTIVEEXCLUDED     SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 16))),
                       ISDST                SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 32))),
                       ISDIRTY              SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 64))),
                       AUTOWATCHTOPICS      SMALLINT DEFAULT 0 NOT NULL,
                       CULTURE              VARCHAR(10),
                       DAILYDIGEST          SMALLINT DEFAULT 0 NOT NULL,
                       ISFACEBOOKUSER       SMALLINT DEFAULT 0 NOT NULL,
                       ISTWITTERUSER        SMALLINT DEFAULT 0 NOT NULL,
					   COMMONVIEWTYPE       INTEGER DEFAULT 0 NOT NULL,
					   POSTSPERPAGE         INTEGER DEFAULT 10 NOT NULL, 
					   TOPICSPERPAGE        INTEGER DEFAULT 20 NOT NULL, 
                       USERSTYLE            VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       STYLEFLAGS           INTEGER DEFAULT 0 NOT NULL,
                       ISUSERSTYLE          SMALLINT COMPUTED BY (SIGN(BIN_AND(STYLEFLAGS, 1))),
                       ISGROUPSTYLE         SMALLINT COMPUTED BY (SIGN(BIN_AND(STYLEFLAGS, 2))),
                       ISRANKSTYLE          SMALLINT COMPUTED BY (SIGN(BIN_AND(STYLEFLAGS, 4)))
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}USERFORUM
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}USERFORUM')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}USERFORUM
                       (
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       FORUMID              INTEGER DEFAULT 0 NOT NULL,
                       ACCESSMASKID         INTEGER DEFAULT 0 NOT NULL,
                       INVITED              TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL,
                       ACCEPTED             SMALLINT DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}USERGROUP
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}USERGROUP')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}USERGROUP
                       (
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       GROUPID              INTEGER DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}USERMEDAL
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}USERMEDAL')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}USERMEDAL
                       (
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       MEDALID              INTEGER DEFAULT 0 NOT NULL,
                       MESSAGE              VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       HIDE                 SMALLINT DEFAULT 0 NOT NULL,
                       ONLYRIBBON           SMALLINT DEFAULT 0 NOT NULL,
                       SORTORDER            INT DEFAULT 255 NOT NULL,
                       DATEAWARDED          TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL
                       );';
 END
--GO

EXECUTE BLOCK
AS
BEGIN  
-- Source table: {objectQualifier}IGNOREUSER
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}IGNOREUSER')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}IGNOREUSER
                       (
                       USERID               INTEGER NOT NULL,
                       IGNOREDUSERID        INTEGER NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 	
-- Source table: {objectQualifier}USERPMESSAGE
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}USERPMESSAGE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}USERPMESSAGE
                       (
                       USERPMESSAGEID       INTEGER DEFAULT 0 NOT NULL,
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       PMESSAGEID           INTEGER DEFAULT 0 NOT NULL,
                       FLAGS                INTEGER DEFAULT 0 NOT NULL,
                       ISREAD		        SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 1))),
                       ISINOUTBOX	        SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 2))),
                       ISARCHIVED	        SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 4))),
                       ISDELETED	        SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 8))),
                       ISREPLY              SMALLINT DEFAULT 0 NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}WATCHFORUM
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}WATCHFORUM')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}WATCHFORUM
                       (
                       WATCHFORUMID         INTEGER DEFAULT 0,
                       FORUMID              INTEGER DEFAULT 0 NOT NULL,
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       CREATED              TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL,
                       LASTMAIL             TIMESTAMP
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source table: {objectQualifier}WATCHTOPIC
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}WATCHTOPIC')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}WATCHTOPIC
                       (
                       WATCHTOPICID         INTEGER DEFAULT 0,
                       TOPICID              INTEGER DEFAULT 0 NOT NULL,
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       CREATED              TIMESTAMP DEFAULT ''current_timestamp'' NOT NULL,
                       LASTMAIL             TIMESTAMP
                       );';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
            WHERE a.RDB$RELATION_NAME='{objectQualifier}TMP_FLR')) THEN
    EXECUTE STATEMENT 'DROP TABLE {objectQualifier}TMP_FLR;';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}TMP_FLR')) THEN
    EXECUTE STATEMENT 'CREATE GLOBAL TEMPORARY TABLE {objectQualifier}TMP_FLR
                       (
                       CATEGORYID           INTEGER,
                       CATEGORY             VARCHAR(128),
                       FORUMID              INTEGER,
                       FORUM                VARCHAR(128),
                       DESCRIPTION          VARCHAR(128),
                       IMAGEURL             VARCHAR(128),
                       STYLES               VARCHAR(255),
                       POLLGROUPID          INTEGER,
                       TOPICS               INTEGER,
                       POSTS                INTEGER,
                       SUBFORUMS            INTEGER,
                       FLAGS                INTEGER,
                       VIEWING              INTEGER,
                       REMOTEURL            VARCHAR(255),
                       READACCESS           INTEGER,
                       LASTTOPICID          INTEGER,
                       LASTPOSTED           TIMESTAMP
                       ) ON COMMIT DELETE ROWS';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}THANKS')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}THANKS
                       (
                       THANKSID             INTEGER NOT NULL,
                       THANKSFROMUSERID     INTEGER NOT NULL,
                       THANKSTOUSERID       INTEGER NOT NULL,
                       MESSAGEID            INTEGER NOT NULL,
                       THANKSDATE           TIMESTAMP NOT NULL
                       );';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}BUDDY')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}BUDDY
                       (
                       ID                   INTEGER NOT NULL,
                       FROMUSERID           INTEGER NOT NULL,
                       TOUSERID             INTEGER NOT NULL,
                       APPROVED             SMALLINT NOT NULL,
                       REQUESTED            TIMESTAMP  NOT NULL
                       );';
  END
--GO


/* YAF FavoriteTopic Table */

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}FAVORITETOPIC')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}FAVORITETOPIC
                       (
                       ID                   INTEGER NOT NULL,
                       USERID               INTEGER NOT NULL,
                       TOPICID              INTEGER NOT NULL
                       );';
  END
--GO

/* YAF Album Tables */
EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}USERALBUM')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}USERALBUM
                       (
                       ALBUMID              INTEGER NOT NULL,
                       USERID               INTEGER NOT NULL,
                       TITLE                VARCHAR(255),
                       COVERIMAGEID         INTEGER,
                       UPDATED              TIMESTAMP NOT NULL
                       );';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}USERALBUMIMAGE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}USERALBUMIMAGE
                       (
                       IMAGEID INTEGER NOT NULL,
                       ALBUMID INTEGER NOT NULL,
                       CAPTION VARCHAR(255),
                       FILENAME VARCHAR(255) NOT NULL,
                       BYTES INTEGER NOT NULL,
                       CONTENTTYPE VARCHAR(50),
                       UPLOADED TIMESTAMP  NOT NULL,
                       DOWNLOADS INTEGER NOT NULL
                       );';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}MESSAGEHISTORY')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}MESSAGEHISTORY
                       (
                       MESSAGEID            INTEGER DEFAULT 0 NOT NULL,
                       MESSAGE              BLOB SUB_TYPE 1 NOT NULL,
                       IP VARCHAR(39)       CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       EDITED               TIMESTAMP NOT NULL,
                       EDITEDBY             INTEGER,
                       EDITREASON           VARCHAR(128) CHARACTER SET UTF8 COLLATE UNICODE,
                       ISMODERATORCHANGED   SMALLINT DEFAULT 0,
                       FLAGS                INTEGER DEFAULT 23 NOT NULL
                       );';
  END

--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}TOPICREADTRACKING')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}TOPICREADTRACKING
                       (
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       TOPICID              INTEGER DEFAULT 0 NOT NULL,
                       LASTACCESSDATE       TIMESTAMP  NOT NULL
                       );';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}FORUMREADTRACKING')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}FORUMREADTRACKING
                       (
                       USERID               INTEGER DEFAULT 0 NOT NULL,
                       FORUMID              INTEGER DEFAULT 0 NOT NULL,
                       LASTACCESSDATE       TIMESTAMP  NOT NULL
                       );';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}USERPROFILE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}USERPROFILE
                       (
                       USERID               INTEGER NOT NULL,
                       LASTUPDATEDDATE      TIMESTAMP NOT NULL,
                       LASTACTIVITY         TIMESTAMP,
                       APPLICATIONNAME      VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE,
                       ISANONYMOUS          SMALLINT DEFAULT 0 NOT NULL,
                       USERNAME             VARCHAR(255) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE
                       );';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}TOPICSTATUS')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}TOPICSTATUS
                       (
                       TOPICSTATUSID        INTEGER NOT NULL,
                       TOPICSTATUSNAME      VARCHAR(128) CHARACTER SET  UTF8 NOT NULL COLLATE UNICODE,
                       BOARDID              INTEGER NOT NULL,
                       DEFAULTDESCRIPTION   VARCHAR(128) CHARACTER SET UTF8 NOT NULL COLLATE UNICODE
                       );';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}TAGS')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}TAGS
                       (
                       TAGID       INTEGER NOT NULL,		  
                       TAG      VARCHAR(1024) CHARACTER SET  UTF8 NOT NULL COLLATE UNICODE,
                       TAGCOUNT  INTEGER DEFAULT 0 	NOT NULL			  
                       );';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}TOPICTAGS')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}TOPICTAGS
                       (			  
                       TAGID      INTEGER NOT NULL,
                       TOPICID              INTEGER NOT NULL
                       );';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}REPUTATIONVOTE')) THEN
    EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}REPUTATIONVOTE
                       (
                       REPUTATIONFROMUSERID INTEGER NOT NULL,
                       REPUTATIONTOUSERID   INTEGER NOT NULL,
                       VOTEDATE             TIMESTAMP NOT NULL
                       );';
  END
--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}TBL')) THEN
    EXECUTE STATEMENT 'CREATE global TEMPORARY TABLE {objectQualifier}TBL
                       (
                       FORUMID              INTEGER, 
                       PARENTID             INTEGER
                       ) ON COMMIT DELETE ROWS;';
  END

--GO

EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}TBL1')) THEN
    EXECUTE STATEMENT 'CREATE global TEMPORARY TABLE {objectQualifier}TBL1
                       (
                       FORUMID              INTEGER, 
                       PARENTID             INTEGER
                       ) ON COMMIT DELETE ROWS;';
  END
--GO
EXECUTE BLOCK
AS
BEGIN 
IF (NOT EXISTS( SELECT FIRST 1 1 FROM RDB$RELATIONS a 
                WHERE a.RDB$RELATION_NAME='{objectQualifier}TMPEVLOGIDS')) THEN
    EXECUTE STATEMENT 'CREATE global TEMPORARY TABLE {objectQualifier}TMPEVLOGIDS
                       (
                       EVENTTYPEID              INTEGER
                       ) ON COMMIT DELETE ROWS;';
  END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS(SELECT 1 
			   FROM RDB$RELATIONS a 
			   WHERE a.RDB$RELATION_NAME='{objectQualifier}FORUM_NS' 
			   ROWS 1)) THEN
	EXECUTE STATEMENT 'CREATE TABLE {objectQualifier}FORUM_NS
(
  NID INTEGER NOT NULL,
  BOARDID INTEGER NOT NULL,
  CATEGORYID INTEGER NOT NULL,
  FORUMID INTEGER NOT NULL,
  LEFT_KEY INTEGER NOT NULL,
  RIGHT_KEY INTEGER NOT NULL,
  "LEVEL" INTEGER DEFAULT 0 NOT NULL,
  TREE INTEGER DEFAULT 0 NOT NULL,
  PARENTID INTEGER DEFAULT 0 NOT NULL,  
  TRIGGER_LOCK_UPDATE SMALLINT DEFAULT 0 NOT NULL,
  TRIGGER_FOR_DELETE SMALLINT DEFAULT 0 NOT NULL, 
  SORTORDER INTEGER DEFAULT 0 NOT NULL,
  PATH_CACHE varchar(255)); ';
  END;
--GO

-- add missing fields
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
            WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME =UPPER('{objectQualifier}ACTIVEACCESS')
            AND RDB$NULL_FLAG IS NULL  AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = 'FORUMID' ROWS 1)) THEN
    EXECUTE STATEMENT 'UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 1
                       WHERE RDB$FIELD_NAME = ''FORUMID'' AND RDB$RELATION_NAME = UPPER(''{objectQualifier}ACTIVEACCESS'')';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
            WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME =UPPER('{objectQualifier}MESSAGEREPORTEDAUDIT')
            AND RDB$NULL_FLAG IS NULL  AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = 'MESSAGEID' ROWS 1)) THEN
    EXECUTE STATEMENT 'UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 1
                       WHERE RDB$FIELD_NAME = ''MESSAGEID'' AND RDB$RELATION_NAME = UPPER(''{objectQualifier}MESSAGEREPORTEDAUDIT'')';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
            WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME =UPPER('{objectQualifier}MESSAGEREPORTEDAUDIT')
            AND RDB$NULL_FLAG IS NULL  AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = 'USERID' ROWS 1)) THEN
    EXECUTE STATEMENT 'UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 1
                       WHERE RDB$FIELD_NAME = ''USERID'' AND RDB$RELATION_NAME = UPPER(''{objectQualifier}MESSAGEREPORTEDAUDIT'')';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
            WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME =UPPER('{objectQualifier}MESSAGEREPORTEDAUDIT')
            AND RDB$NULL_FLAG IS NULL  AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = 'REPORTED' ROWS 1)) THEN
    EXECUTE STATEMENT 'UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 1
                       WHERE RDB$FIELD_NAME = ''REPORTED'' AND RDB$RELATION_NAME = UPPER(''{objectQualifier}MESSAGEREPORTEDAUDIT'')';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
            WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME =UPPER('{objectQualifier}MESSAGEHISTORY')
            AND RDB$NULL_FLAG IS NULL  AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = 'EDITED' ROWS 1)) THEN
    EXECUTE STATEMENT 'UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 1
                       WHERE RDB$FIELD_NAME = ''EDITED'' AND RDB$RELATION_NAME = UPPER(''{objectQualifier}MESSAGEHISTORY'')';
END
--GO


EXECUTE BLOCK
AS
BEGIN				
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}TOPIC')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('TOPICIMAGETYPE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPIC ADD TOPICIMAGETYPE  VARCHAR(50);';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}TOPIC')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('TOPICIMAGEBIN') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPIC ADD TOPICIMAGEBIN  BLOB SUB_TYPE 0;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}TOPIC')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('LINKDATE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPIC ADD LINKDATE TIMESTAMP;';

	IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}MESSAGE')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('DESCRIPTION') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}MESSAGE ADD DESCRIPTION  VARCHAR(255) CHARACTER SET UTF8  COLLATE UNICODE;';	

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}USER')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('COMMONVIEWTYPE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USER ADD COMMONVIEWTYPE  INTEGER DEFAULT 0 NOT NULL;';	

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}USER')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('POSTSPERPAGE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USER ADD POSTSPERPAGE INTEGER DEFAULT 10 NOT NULL;';					    

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}USER')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('TOPICSPERPAGE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USER ADD TOPICSPERPAGE INTEGER DEFAULT 20 NOT NULL;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}USER')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('USERSTYLE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USER ADD USERSTYLE  VARCHAR(255) CHARACTER SET UTF8  COLLATE UNICODE;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}USER')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('STYLEFLAGS') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USER ADD STYLEFLAGS INTEGER DEFAULT 0 NOT NULL;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}USER')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('ISUSERSTYLE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USER ADD ISUSERSTYLE SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 1)));';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}USER')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('ISGROUPSTYLE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USER ADD ISGROUPSTYLE SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 2)));';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT  1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}USER')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('ISRANKSTYLE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USER ADD ISRANKSTYLE SMALLINT COMPUTED BY (SIGN(BIN_AND(FLAGS, 4)));';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('LASTUSERDISPLAYNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD LASTUSERDISPLAYNAME VARCHAR(128) CHARACTER SET UTF8  NULL COLLATE UNICODE;';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('LEFT_KEY') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD LEFT_KEY INT;';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('RIGHT_KEY') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD RIGHT_KEY INT;';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('LEVEL') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD "LEVEL" INT;';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('TRIGGER_FOR_DELETE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD TRIGGER_FOR_DELETE INT DEFAULT 0 NOT NULL;';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('TRIGGER_LOCK_UPDATE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD TRIGGER_LOCK_UPDATE INT DEFAULT 0 NOT NULL;';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}MESSAGE')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('USERDISPLAYNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}MESSAGE ADD USERDISPLAYNAME VARCHAR(128) CHARACTER SET UTF8  NULL COLLATE UNICODE;';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}TOPIC')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('USERDISPLAYNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPIC ADD USERDISPLAYNAME VARCHAR(128) CHARACTER SET UTF8  NULL COLLATE UNICODE;';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}TOPIC')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('LASTUSERDISPLAYNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPIC ADD LASTUSERDISPLAYNAME VARCHAR(255) CHARACTER SET UTF8  NULL COLLATE UNICODE;';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}SHOUTBOXMESSAGE')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('USERDISPLAYNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}SHOUTBOXMESSAGE ADD USERDISPLAYNAME VARCHAR(255) CHARACTER SET UTF8  NULL COLLATE UNICODE;';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}PMESSAGE')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('REPLYTO') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}PMESSAGE ADD REPLYTO INT;';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}USERPMESSAGE')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('ISREPLY') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USERPMESSAGE ADD ISREPLY SMALLINT DEFAULT 0 NOT NULL;';
END
--GO
EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}TOPIC')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('LINKDATE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPIC ADD LINKDATE TIMESTAMP;';
END
--GO
EXECUTE BLOCK
AS
BEGIN
	IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}GROUP')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDBYUSERID') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD CREATEDBYUSERID INTEGER;';  
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}GROUP')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDBYUSERNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD CREATEDBYUSERNAME  VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE;';
END
--GO

EXECUTE BLOCK
AS
BEGIN
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}GROUP')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDBYUSERDISLAYNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD CREATEDBYUSERDISLAYNAME  VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE;';
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}GROUP')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDDATE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD CREATEDDATE TIMESTAMP';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}GROUP')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('ISUSERGROUP') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD ISUSERGROUP SMALLINT DEFAULT 0 NOT NULL;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}GROUP')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('ISADMINGROUP') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD ISADMINGROUP SMALLINT DEFAULT 0 NOT NULL;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}GROUP')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('ISHIDDEN') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD ISHIDDEN SMALLINT DEFAULT 0 NOT NULL;';
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}GROUP')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('USRPERSONALFORUMS') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD USRPERSONALFORUMS INTEGER DEFAULT 0 NOT NULL;';
	
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}GROUP')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('USRPERSONALMASKS') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD USRPERSONALMASKS INTEGER DEFAULT 0 NOT NULL;';	
	
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}GROUP')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('USRPERSONALGROUPS') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD USRPERSONALGROUPS INTEGER DEFAULT 0 NOT NULL;';					

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDBYUSERID') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD CREATEDBYUSERID INTEGER;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDBYUSERNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD CREATEDBYUSERNAME  VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDBYUSERDISLAYNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD CREATEDBYUSERDISLAYNAME  VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDDATE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD CREATEDDATE  TIMESTAMP';
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('ISUSERFORUM') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD ISUSERFORUM SMALLINT DEFAULT 0 NOT NULL;';	 

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}FORUM')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CANHAVEPERSFORUMS') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD CANHAVEPERSFORUMS  SMALLINT DEFAULT 0 NOT NULL;';		  
	
IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}ACCESSMASK')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDBYUSERID') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ACCESSMASK ADD CREATEDBYUSERID INTEGER;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}ACCESSMASK')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDBYUSERNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ACCESSMASK ADD CREATEDBYUSERNAME  VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}ACCESSMASK')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDBYUSERDISLAYNAME') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ACCESSMASK ADD CREATEDBYUSERDISLAYNAME  VARCHAR(255) CHARACTER SET UTF8 COLLATE UNICODE;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}ACCESSMASK')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CREATEDDATE') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ACCESSMASK ADD CREATEDDATE  TIMESTAMP';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}ACCESSMASK')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('ISUSERMASK') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ACCESSMASK ADD ISUSERMASK SMALLINT DEFAULT 0 NOT NULL;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}ACCESSMASK')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('ISADMINMASK') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ACCESSMASK ADD ISADMINMASK SMALLINT DEFAULT 0 NOT NULL;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}ACTIVEACCESS')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('USERFORUMACCESS') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ACTIVEACCESS ADD USERFORUMACCESS INT DEFAULT 0 NOT NULL;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}CATEGORY')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CANHAVEPERSFORUMS') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}CATEGORY ADD CANHAVEPERSFORUMS  SMALLINT DEFAULT 0 NOT NULL;';		

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}USERPROFILE')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('BIRTHDAY') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USERPROFILE ADD BIRTHDAY TIMESTAMP;';

IF (NOT EXISTS( SELECT 1 FROM RDB$RELATION_FIELDS
                WHERE RDB$RELATION_FIELDS.RDB$RELATION_NAME = UPPER('{objectQualifier}CATEGORY')
                AND RDB$RELATION_FIELDS.RDB$FIELD_NAME = UPPER('CANHAVEPERSFORUMS') ROWS 1)) THEN
    EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}CATEGORY ADD CANHAVEPERSFORUMS  SMALLINT DEFAULT 0 NOT NULL;';		
END
--GO

-- Source unique key: IX_{objectQualifier}TOPREADTR
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='IX_{objectQualifier}TOPREADTR' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPICREADTRACKING DROP CONSTRAINT IX_{objectQualifier}TOPREADTR;';
END
--GO

-- Source unique key: PRIMARY018
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY018' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}MESSAGEREPORTEDAUDIT DROP CONSTRAINT PRIMARY018;';
END
--GO

-- Source unique key: IX_{objectQualifier}FORREADTR
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='IX_{objectQualifier}FORREADTR' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUMREADTRACKING DROP CONSTRAINT IX_{objectQualifier}FORREADTR;';
END
--GO
-- Source unique key: IX_{objectQualifier}MSGHIST_ED_ID
EXECUTE BLOCK
AS
BEGIN
IF (EXISTS( SELECT 1 
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='IX_{objectQualifier}MSGHIST_ED_ID' ROWS 1 )) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}MESSAGEHISTORY DROP CONSTRAINT IX_{objectQualifier}MSGHIST_ED_ID;';
END
--GO
