/* Yet Another Forum.NET Firebird data layer by vzrus
 * Copyright (C) 2006-2014 Vladimir Zakharov
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
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY001' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ACCESSMASK ADD CONSTRAINT PRIMARY001 PRIMARY KEY (ACCESSMASKID)';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY002' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ACTIVE ADD CONSTRAINT PRIMARY002 PRIMARY KEY (SESSIONID,BOARDID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY003' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ATTACHMENT ADD CONSTRAINT PRIMARY003 PRIMARY KEY (ATTACHMENTID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY004' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}BANNEDIP ADD CONSTRAINT PRIMARY004 UNIQUE (ID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY005' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}BBCODE ADD CONSTRAINT PRIMARY005 PRIMARY KEY (BBCODEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}BOARDID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}BOARD ADD CONSTRAINT PK_{objectQualifier}BOARDID PRIMARY KEY (BOARDID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY007' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}CATEGORY ADD CONSTRAINT PRIMARY007 UNIQUE (CATEGORYID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY008' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}CHECKEMAIL ADD CONSTRAINT PRIMARY008 UNIQUE (CHECKEMAILID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY009' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}CHOICE ADD CONSTRAINT PRIMARY009 PRIMARY KEY (CHOICEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY010' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}EVENTLOG ADD CONSTRAINT PRIMARY010 PRIMARY KEY (EVENTLOGID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY011' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}EXTENSION ADD CONSTRAINT PRIMARY011 PRIMARY KEY (EXTENSIONID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY012' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUM ADD CONSTRAINT PRIMARY012 UNIQUE (FORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY013' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUMACCESS ADD CONSTRAINT PRIMARY013 PRIMARY KEY (GROUPID,FORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY014' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUP ADD CONSTRAINT PRIMARY014 UNIQUE (GROUPID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY015' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}MAIL ADD CONSTRAINT PRIMARY015 PRIMARY KEY (MAILID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY016')) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}MEDAL ADD CONSTRAINT PRIMARY016 PRIMARY KEY (MEDALID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY017' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}MESSAGE ADD CONSTRAINT PRIMARY017 PRIMARY KEY (MESSAGEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}MSGEREPAUDIT' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}MESSAGEREPORTEDAUDIT ADD CONSTRAINT PK_{objectQualifier}MSGEREPAUDIT PRIMARY KEY (USERID,MESSAGEID,REPORTED);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY019' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}NNTPFORUM ADD CONSTRAINT PRIMARY019 PRIMARY KEY (NNTPFORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY020' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}NNTPSERVER ADD CONSTRAINT PRIMARY020 PRIMARY KEY (NNTPSERVERID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY021' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}NNTPTOPIC ADD CONSTRAINT PRIMARY021 PRIMARY KEY (NNTPTOPICID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY022' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}PMESSAGE ADD CONSTRAINT PRIMARY022 PRIMARY KEY (PMESSAGEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY023' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}POLL ADD CONSTRAINT PRIMARY023 PRIMARY KEY (POLLID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY024' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}POLLVOTE ADD CONSTRAINT PRIMARY024 PRIMARY KEY (POLLVOTEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY029' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}RANK ADD CONSTRAINT PRIMARY029 UNIQUE (RANKID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY030' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}REGISTRY ADD CONSTRAINT PRIMARY030 PRIMARY KEY (REGISTRYID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY031' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}REPLACE_WORDS ADD CONSTRAINT PRIMARY031 PRIMARY KEY (ID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PK_{objectQualifier}SHOUTBOXMESSAGE
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}SHOUTBOXMESSAGE' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}SHOUTBOXMESSAGE ADD CONSTRAINT PK_{objectQualifier}SHOUTBOXMESSAGE PRIMARY KEY (SHOUTBOXMESSAGEID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY032' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}SMILEY ADD CONSTRAINT PRIMARY032 UNIQUE (SMILEYID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_TOPICSTATUSID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPICSTATUS ADD CONSTRAINT PK_TOPICSTATUSID PRIMARY KEY (TOPICSTATUSID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_TAGS_TAG' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TAGS ADD CONSTRAINT PK_TAGS_TAG PRIMARY KEY (TAGID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_TOPICTAGS_TAGIDTOPICID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPICTAGS ADD CONSTRAINT PK_TOPICTAGS_TAGIDTOPICID PRIMARY KEY (TAGID,TOPICID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY033' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPIC ADD CONSTRAINT PRIMARY033 PRIMARY KEY (TOPICID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY034' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USER ADD CONSTRAINT PRIMARY034 UNIQUE (USERID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY035' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USERFORUM ADD CONSTRAINT PRIMARY035 PRIMARY KEY (USERID,FORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY036' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USERGROUP ADD CONSTRAINT PRIMARY036 PRIMARY KEY (USERID,GROUPID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY037' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USERPMESSAGE ADD CONSTRAINT PRIMARY037 PRIMARY KEY (USERPMESSAGEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY038' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}WATCHFORUM ADD CONSTRAINT PRIMARY038 UNIQUE (WATCHFORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY039' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}WATCHTOPIC ADD CONSTRAINT PRIMARY039 UNIQUE (WATCHTOPICID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}IGNOREUSER_USERID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}IGNOREUSER ADD CONSTRAINT PK_{objectQualifier}IGNOREUSER_USERID PRIMARY KEY (USERID, IGNOREDUSERID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}THANKS_THANKSID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}THANKS ADD CONSTRAINT PK_{objectQualifier}THANKS_THANKSID PRIMARY KEY (THANKSID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}BUDDY_BUDDYID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}BUDDY ADD CONSTRAINT PK_{objectQualifier}BUDDY_BUDDYID PRIMARY KEY (ID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}FAVORITETOPIC_ID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FAVORITETOPIC ADD CONSTRAINT PK_{objectQualifier}FAVORITETOPIC_ID PRIMARY KEY (ID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}USERALBUM_ALBUMID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USERALBUM ADD CONSTRAINT PK_{objectQualifier}USERALBUM_ALBUMID PRIMARY KEY (ALBUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}USRALBIMG_IMAGEID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USERALBUMIMAGE ADD CONSTRAINT PK_{objectQualifier}USRALBIMG_IMAGEID PRIMARY KEY (IMAGEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}POLLGROUPCLUSTER' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}POLLGROUPCLUSTER ADD CONSTRAINT PK_{objectQualifier}POLLGROUPCLUSTER PRIMARY KEY (POLLGROUPID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}ACTIVEACCESS' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ACTIVEACCESS ADD CONSTRAINT PK_{objectQualifier}ACTIVEACCESS PRIMARY KEY (USERID,FORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}FORUMREADTRACKING' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}FORUMREADTRACKING ADD CONSTRAINT PK_{objectQualifier}FORUMREADTRACKING PRIMARY KEY (USERID,FORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}TOPICREADTRACKING' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}TOPICREADTRACKING ADD CONSTRAINT PK_{objectQualifier}TOPICREADTRACKING PRIMARY KEY (USERID,TOPICID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}GROUPMEDAL' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}GROUPMEDAL ADD CONSTRAINT PK_{objectQualifier}GROUPMEDAL PRIMARY KEY (MEDALID,GROUPID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}USERMEDAL' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USERMEDAL ADD CONSTRAINT PK_{objectQualifier}USERMEDAL PRIMARY KEY (MEDALID,USERID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}USERPROFILE' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}USERPROFILE ADD CONSTRAINT PK_{objectQualifier}USERPROFILE PRIMARY KEY (USERID,APPLICATIONNAME);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}MESSAGEHISTORY' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}MESSAGEHISTORY ADD CONSTRAINT PK_{objectQualifier}MESSAGEHISTORY PRIMARY KEY (MESSAGEID,EDITED);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}MESSAGEREPORTED' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}MESSAGEREPORTED ADD CONSTRAINT PK_{objectQualifier}MESSAGEREPORTED PRIMARY KEY (MESSAGEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}ADMINPAGEUSERACCESS' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}ADMINPAGEUSERACCESS ADD CONSTRAINT PK_{objectQualifier}ADMINPAGEUSERACCESS PRIMARY KEY (USERID, PAGENAME);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_{objectQualifier}EVENTLOGGROUPACCESS' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE {objectQualifier}EVENTLOGGROUPACCESS ADD CONSTRAINT PK_{objectQualifier}EVENTLOGGROUPACCESS PRIMARY KEY (GROUPID, EVENTTYPEID);';
END
--GO






