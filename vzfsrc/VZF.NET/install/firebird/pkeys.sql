/* Yet Another Forum.NET Firebird data layer by vzrus
 * Copyright (C) 2006-2012 Vladimir Zakharov
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
EXECUTE STATEMENT 'ALTER TABLE objQual_ACCESSMASK ADD CONSTRAINT PRIMARY001 PRIMARY KEY (ACCESSMASKID)';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY002' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_ACTIVE ADD CONSTRAINT PRIMARY002 PRIMARY KEY (SESSIONID,BOARDID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY003' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_ATTACHMENT ADD CONSTRAINT PRIMARY003 PRIMARY KEY (ATTACHMENTID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY004' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_BANNEDIP ADD CONSTRAINT PRIMARY004 UNIQUE (ID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY005' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_BBCODE ADD CONSTRAINT PRIMARY005 PRIMARY KEY (BBCODEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_BOARDID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_BOARD ADD CONSTRAINT PK_objQual_BOARDID PRIMARY KEY (BOARDID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY007' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_CATEGORY ADD CONSTRAINT PRIMARY007 UNIQUE (CATEGORYID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY008' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_CHECKEMAIL ADD CONSTRAINT PRIMARY008 UNIQUE (CHECKEMAILID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY009' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_CHOICE ADD CONSTRAINT PRIMARY009 PRIMARY KEY (CHOICEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY010' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_EVENTLOG ADD CONSTRAINT PRIMARY010 PRIMARY KEY (EVENTLOGID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY011' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_EXTENSION ADD CONSTRAINT PRIMARY011 PRIMARY KEY (EXTENSIONID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY012' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_FORUM ADD CONSTRAINT PRIMARY012 UNIQUE (FORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY013' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_FORUMACCESS ADD CONSTRAINT PRIMARY013 PRIMARY KEY (GROUPID,FORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY014' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_GROUP ADD CONSTRAINT PRIMARY014 UNIQUE (GROUPID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY015' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_MAIL ADD CONSTRAINT PRIMARY015 PRIMARY KEY (MAILID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY016')) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_MEDAL ADD CONSTRAINT PRIMARY016 PRIMARY KEY (MEDALID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY017' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_MESSAGE ADD CONSTRAINT PRIMARY017 PRIMARY KEY (MESSAGEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_MSGEREPAUDIT' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_MESSAGEREPORTEDAUDIT ADD CONSTRAINT PK_objQual_MSGEREPAUDIT PRIMARY KEY (USERID,MESSAGEID,REPORTED);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY019' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_NNTPFORUM ADD CONSTRAINT PRIMARY019 PRIMARY KEY (NNTPFORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY020' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_NNTPSERVER ADD CONSTRAINT PRIMARY020 PRIMARY KEY (NNTPSERVERID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY021' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_NNTPTOPIC ADD CONSTRAINT PRIMARY021 PRIMARY KEY (NNTPTOPICID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY022' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_PMESSAGE ADD CONSTRAINT PRIMARY022 PRIMARY KEY (PMESSAGEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY023' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_POLL ADD CONSTRAINT PRIMARY023 PRIMARY KEY (POLLID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY024' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_POLLVOTE ADD CONSTRAINT PRIMARY024 PRIMARY KEY (POLLVOTEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY029' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_RANK ADD CONSTRAINT PRIMARY029 UNIQUE (RANKID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY030' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_REGISTRY ADD CONSTRAINT PRIMARY030 PRIMARY KEY (REGISTRYID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY031' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_REPLACE_WORDS ADD CONSTRAINT PRIMARY031 PRIMARY KEY (ID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PK_objQual_SHOUTBOXMESSAGE
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_SHOUTBOXMESSAGE' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_SHOUTBOXMESSAGE ADD CONSTRAINT PK_objQual_SHOUTBOXMESSAGE PRIMARY KEY (SHOUTBOXMESSAGEID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY032' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_SMILEY ADD CONSTRAINT PRIMARY032 UNIQUE (SMILEYID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_TOPICSTATUSID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_TOPICSTATUS ADD CONSTRAINT PK_TOPICSTATUSID PRIMARY KEY (TOPICSTATUSID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_TAGS_TAG' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_TAGS ADD CONSTRAINT PK_TAGS_TAG PRIMARY KEY (TAG);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY033' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_TOPIC ADD CONSTRAINT PRIMARY033 PRIMARY KEY (TOPICID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY034' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_USER ADD CONSTRAINT PRIMARY034 UNIQUE (USERID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY035' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_USERFORUM ADD CONSTRAINT PRIMARY035 PRIMARY KEY (USERID,FORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY036' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_USERGROUP ADD CONSTRAINT PRIMARY036 PRIMARY KEY (USERID,GROUPID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY037' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_USERPMESSAGE ADD CONSTRAINT PRIMARY037 PRIMARY KEY (USERPMESSAGEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY038' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_WATCHFORUM ADD CONSTRAINT PRIMARY038 UNIQUE (WATCHFORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PRIMARY039' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_WATCHTOPIC ADD CONSTRAINT PRIMARY039 UNIQUE (WATCHTOPICID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_IGNOREUSER_USERID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_IGNOREUSER ADD CONSTRAINT PK_objQual_IGNOREUSER_USERID PRIMARY KEY (USERID, IGNOREDUSERID);';
END
--GO


EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_THANKS_THANKSID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_THANKS ADD CONSTRAINT PK_objQual_THANKS_THANKSID PRIMARY KEY (THANKSID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_BUDDY_BUDDYID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_BUDDY ADD CONSTRAINT PK_objQual_BUDDY_BUDDYID PRIMARY KEY (ID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_FAVORITETOPIC_ID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_FAVORITETOPIC ADD CONSTRAINT PK_objQual_FAVORITETOPIC_ID PRIMARY KEY (ID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_USERALBUM_ALBUMID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_USERALBUM ADD CONSTRAINT PK_objQual_USERALBUM_ALBUMID PRIMARY KEY (ALBUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_USRALBIMG_IMAGEID' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_USERALBUMIMAGE ADD CONSTRAINT PK_objQual_USRALBIMG_IMAGEID PRIMARY KEY (IMAGEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_POLLGROUPCLUSTER' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_POLLGROUPCLUSTER ADD CONSTRAINT PK_objQual_POLLGROUPCLUSTER PRIMARY KEY (POLLGROUPID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT 1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_ACTIVEACCESS' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_ACTIVEACCESS ADD CONSTRAINT PK_objQual_ACTIVEACCESS PRIMARY KEY (USERID,FORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_FORUMREADTRACKING' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_FORUMREADTRACKING ADD CONSTRAINT PK_objQual_FORUMREADTRACKING PRIMARY KEY (USERID,FORUMID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_TOPICREADTRACKING' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_TOPICREADTRACKING ADD CONSTRAINT PK_objQual_TOPICREADTRACKING PRIMARY KEY (USERID,TOPICID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_GROUPMEDAL' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_GROUPMEDAL ADD CONSTRAINT PK_objQual_GROUPMEDAL PRIMARY KEY (MEDALID,GROUPID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_USERMEDAL' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_USERMEDAL ADD CONSTRAINT PK_objQual_USERMEDAL PRIMARY KEY (MEDALID,USERID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_USERPROFILE' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_USERPROFILE ADD CONSTRAINT PK_objQual_USERPROFILE PRIMARY KEY (USERID,APPLICATIONNAME);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_MESSAGEHISTORY' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_MESSAGEHISTORY ADD CONSTRAINT PK_objQual_MESSAGEHISTORY PRIMARY KEY (MESSAGEID,EDITED);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_MESSAGEREPORTED' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_MESSAGEREPORTED ADD CONSTRAINT PK_objQual_MESSAGEREPORTED PRIMARY KEY (MESSAGEID);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_ADMINPAGEUSERACCESS' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_ADMINPAGEUSERACCESS ADD CONSTRAINT PK_objQual_ADMINPAGEUSERACCESS PRIMARY KEY (USERID, PAGENAME);';
END
--GO

EXECUTE BLOCK
AS
BEGIN 
-- Source primary key: PRIMARY
IF (NOT EXISTS( SELECT  1
FROM RDB$INDICES a WHERE a.RDB$INDEX_NAME ='PK_objQual_EVENTLOGGROUPACCESS' ROWS 1)) THEN
EXECUTE STATEMENT 'ALTER TABLE objQual_EVENTLOGGROUPACCESS ADD CONSTRAINT PK_objQual_EVENTLOGGROUPACCESS PRIMARY KEY (GROUPID, EVENTTYPEID);';
END
--GO






