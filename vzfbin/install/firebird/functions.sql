﻿/* Yet Another Forum.NET Firebird data layer by vzrus
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

CREATE OR ALTER PROCEDURE  {objectQualifier}forum_posts(I_FORUMID integer) 
returns (I_NumPosts integer) as
DECLARE VARIABLE I_tmp integer;
DECLARE VARIABLE I_tmpInter integer;
DECLARE C CURSOR FOR (select FORUMID from {objectQualifier}FORUM
		where PARENTID = :I_FORUMID);
begin
	
	select NUMPOSTS from {objectQualifier}FORUM where FORUMID=:I_FORUMID INTO :I_NumPosts;


	if (exists(select 1 from {objectQualifier}FORUM where PARENTID=:I_FORUMID))  THEN

	BEGIN
		OPEN C;
  WHILE (ROW_COUNT > 0)
  DO
  BEGIN
	FETCH C INTO :I_tmp;    
	IF(ROW_COUNT = 0)THEN
	  LEAVE;
	  EXECUTE PROCEDURE {objectQualifier}forum_posts :I_tmp 
	  RETURNING_VALUES :I_tmpInter; 
	 -- I_NumPosts=I_NumPosts+{objectQualifier}forum_posts(:I_tmp);  
	 I_NumPosts=I_NumPosts+I_tmpInter;
  END
  CLOSE C;
  END
  SUSPEND;
end;
--GO

CREATE OR ALTER PROCEDURE  {objectQualifier}FORUM_TOPICS(I_FORUMID integer)
 returns (I_NumTopics integer) as
	DECLARE VARIABLE I_tmp integer;
	DECLARE VARIABLE I_tmpInter integer;
		DECLARE ct cursor for
		(select FORUMID from {objectQualifier}FORUM
		where PARENTID = :I_FORUMID);
begin
	select NUMTOPICS from {objectQualifier}FORUM where FORUMID=:I_FORUMID INTO :I_NumTopics;
	if (exists(select 1 from {objectQualifier}FORUM where PARENTID=:I_FORUMID))  THEN

	begin	
		
		open ct;
		WHILE (ROW_COUNT > 0)
  DO
  BEGIN
	FETCH ct INTO :I_tmp;    
	IF(ROW_COUNT = 0) THEN
	  LEAVE;
	  EXECUTE PROCEDURE {objectQualifier}FORUM_TOPICS :I_tmp 
	  RETURNING_VALUES :I_tmpInter; 
	  -- I_NumTopics=I_NumTopics+{objectQualifier}FORUM_TOPICS(:I_tmp);
	 I_NumTopics=I_NumTopics+I_tmpInter;  
  END		
	close ct;		
	end	
	SUSPEND;
end;
--GO

CREATE PROCEDURE  {objectQualifier}FORUM_LASTPOSTED 

(	
	I_FORUMID integer,
	I_USERID integer,
	I_LASTTOPICID integer,
	I_LASTPOSTED timestamp
)
RETURNS 
(
	"LastTopicID" integer,
	"LastPosted" timestamp
)
AS
-- local variables for temporary values
	DECLARE VARIABLE L_SUBFORUMID integer;
	DECLARE VARIABLE I_TOPICID integer;
	DECLARE VARIABLE I_Posted timestamp;
			
BEGIN
	

	-- try to retrieve last direct topic posed in forums if not supplied as argument 
	if (I_LASTTOPICID is null or I_LASTPOSTED is null) THEN
	begin
		SELECT 
			a.LASTTOPICID,
			a.LASTPOSTED
		FROM
			{objectQualifier}FORUM a
			JOIN {objectQualifier}ACTIVEACCESS x 
			ON a.FORUMID=x.FORUMID
		WHERE
			(a.FORUMID=:I_FORUMID and
			(
				(:I_USERID is null and BIN_AND(a.FLAGS, 2)=0) or 
				(x.USERID=:I_USERID and (BIN_AND(a.FLAGS, 2)=0 
				or x.READACCESS<>0))
			))
			INTO :I_LASTTOPICID,:I_LASTPOSTED;
	

	-- look for newer topic/message in subforums
	if (exists(select 1 from {objectQualifier}FORUM where PARENTID=:I_FORUMID)) THEN
  BEGIN
  -- cycle through subforums
  FOR
  SELECT 
				a.FORUMID,
				a.LASTTOPICID,
				a.LASTPOSTED
			FROM
				{objectQualifier}FORUM a
				JOIN {objectQualifier}ACTIVEACCESS x ON a.FORUMID=x.FORUMID
			WHERE
				a.PARENTID=:I_FORUMID and
				(
					(:I_USERID is null and BIN_AND(a.FLAGS, 2)=0) or 
					(x.USERID=:I_USERID and (BIN_AND(a.FLAGS, 2)=0 
					or x.READACCESS<>0))
				)
				INTO :L_SUBFORUMID, :I_TOPICID, :I_POSTED
		DO
		BEGIN   
	 -- get last topic/message info for subforum		
			EXECUTE PROCEDURE {objectQualifier}FORUM_LASTPOSTED :L_SUBFORUMID, :I_USERID, :I_TOPICID, :I_POSTED
				RETURNING_VALUES
				:I_TOPICID,
				:I_Posted;


			-- if subforum has newer topic/message, make it last for parent forum
			if (I_TOPICID is not null and I_Posted is not null and I_LASTPOSTED < :I_POSTED) THEN
			begin
				I_LASTTOPICID = :I_TOPICID;
				I_LASTPOSTED = :I_POSTED;				
			end	   
   
  END
  SUSPEND;		
  END
end
	-- return vector
	-- INSERT I_LastPostInForum
	-- SELECT 
	--	I_LASTTOPICID,
	--	I_LASTPOSTED	
	  SELECT :I_LASTTOPICID, :I_LASTPOSTED FROM RDB$DATABASE
	  INTO 
	  :"LastTopicID",
	  :"LastPosted";
SUSPEND;
END;
--GO

CREATE OR ALTER PROCEDURE  {objectQualifier}FORUM_LASTTOPIC 

(	
	I_FORUMID integer,
	I_USERID integer,
	I_LASTTOPICID integer,
	I_LASTPOSTED timestamp
) RETURNS ("LastTopicID" integer) AS
-- local variables for temporary values
	DECLARE VARIABLE L_SUBFORUMID integer;	
	DECLARE VARIABLE I_POSTED TIMESTAMP;
	DECLARE VARIABLE I_TOPICID integer;		
BEGIN
	

	-- try to retrieve last direct topic posed in forums if not supplied as argument 
	if (:I_LASTTOPICID is null or :I_LASTPOSTED is null) THEN 	
		SELECT 
			a.LASTTOPICID,
			a.LASTPOSTED
		FROM
			{objectQualifier}FORUM a
			JOIN {objectQualifier}ACTIVEACCESS x ON a.FORUMID=x.FORUMID
		WHERE
			a.FORUMID=:I_FORUMID and
			(
				(:I_USERID is null and BIN_AND(a.FLAGS, 2)=0) or 
				(x.USERID=:I_USERID and (BIN_AND(a.FLAGS, 2)=0 
				or x.READACCESS<>0))
			)
			INTO :I_LASTTOPICID,:I_LASTPOSTED;


	-- look for newer topic/message in subforums
	if (exists(select FIRST 1 1 from {objectQualifier}FORUM 
	where PARENTID=:I_FORUMID)) THEN	
	BEGIN			
		FOR
		SELECT 
				a.FORUMID,
				a.LASTTOPICID,
				a.LASTPOSTED
			FROM
				{objectQualifier}FORUM a
				JOIN {objectQualifier}ACTIVEACCESS x ON a.FORUMID=x.FORUMID
			WHERE
				a.PARENTID=:I_FORUMID and
				(
					(:I_USERID is null and BIN_AND(a.FLAGS,2)=0) or 
					(x.USERID=:I_USERID and (BIN_AND(a.FLAGS, 2)=0 or x.READACCESS<>0))
				)		
	INTO :L_SUBFORUMID, :I_TOPICID, :I_POSTED	
	DO
  BEGIN
  -- cycle through subforums
	  -- get last topic/message info for subforum		
		EXECUTE PROCEDURE {objectQualifier}FORUM_LASTPOSTED :L_SUBFORUMID, :I_USERID, :I_TOPICID, :I_Posted
		RETURNING_VALUES :I_TOPICID,:I_Posted;

			-- if subforum has newer topic/message, make it last for parent forum
			if (:I_TOPICID is not null and :I_POSTED is not null and :I_LASTPOSTED < :I_POSTED)
			THEN begin
				I_LASTTOPICID = :I_TOPICID;
				I_LASTPOSTED = :I_POSTED;
				 end
			  -- workaround to avoid logical expressions with NULL possible differences through SQL server versions. 
			if (:I_TOPICID is not null and :I_POSTED is not null and :I_LASTPOSTED is null) 
			then begin
				I_LASTTOPICID = :I_TOPICID;
				I_LASTPOSTED = :I_POSTED;
				 end	   
  
  END	
END	
	-- return id of topic with last message in this forum or its subforums
  SELECT :I_LASTTOPICID FROM RDB$DATABASE
	INTO 
	  :"LastTopicID";
SUSPEND;	
END;
--GO

CREATE OR ALTER PROCEDURE  {objectQualifier}FORUM_NS_LASTTOPIC
(
	I_LK INTEGER,
	I_RK INTEGER,
    I_CATEGORYID INTEGER,
	I_USERID INTEGER
) RETURNS ("LastTopicID" INTEGER) AS
BEGIN
	IF (I_USERID IS NULL) THEN	
	BEGIN	
		    select  f.LastTopicID from {objectQualifier}FORUM f 
			        inner join {objectQualifier}ACTIVEACCESS x 
					 on f.FORUMID=x.FORUMID
		             where f.CATEGORYID = :I_CATEGORYID and f.left_key >= :I_LK  and f.right_key <= :I_RK and f.LASTPOSTED IS NOT NULL
					 and BIN_AND(f.FLAGS, 2)=0		 
                    order by f.LASTPOSTED desc ROWS 1
					INTO :"LastTopicID";
		END     
		else
		BEGIN
			select  f.LastTopicID from {objectQualifier}FORUM f 
			        inner join {objectQualifier}ACTIVEACCESS x  on f.FORUMID=x.FORUMID
		            where f.CATEGORYID = :I_CATEGORYID and f.left_key >= :I_LK and f.right_key <= :I_RK and f.LASTPOSTED IS NOT NULL 
		            and (BIN_AND(f.FLAGS, 2)=0  or x.READACCESS <> 0) 
		             and x.USERID = :I_USERID		 
                      order by f.LASTPOSTED desc ROWS 1
					  INTO :"LastTopicID"; 
        END
	 SUSPEND;
END;
--GO



CREATE OR ALTER PROCEDURE  {objectQualifier}MEDAL_GETRIBBONSETTINGS
(
	I_RIBBONURL VARCHAR(250),
	I_FLAGS integer,
	I_ONLYRIBBON BOOL
)
RETURNS (I_OnlyRibbonRet BOOL)
AS
BEGIN

	if ((I_RIBBONURL is null) or (BIN_AND(:I_FLAGS, 2) = 0)) THEN
	 I_ONLYRIBBON = 0;
	

END;
--GO

CREATE OR ALTER PROCEDURE  {objectQualifier}MEDAL_GETSORTORDER
(
	I_SORTORDER SMALLINT,
	I_DEFAULTSORTORDER SMALLINT,
	I_FLAGS integer
)
RETURNS (I_SortOrderRet SMALLINT)
AS
BEGIN

	if (BIN_AND(:I_FLAGS, 8) = 0)  THEN
	 I_SortOrderRet = I_DEFAULTSORTORDER;
	 ELSE
	 I_SortOrderRet =I_SORTORDER;
	

END;
--GO

CREATE OR ALTER PROCEDURE  {objectQualifier}MEDAL_HIDE
(
	I_HIDE BOOL,
	I_FLAGS integer
)
RETURNS (O_HIDERET BOOL)
AS
BEGIN

	if (BIN_AND(:I_FLAGS,4) = 0)  
	THEN O_HIDERET = 0;
	ELSE
	O_HIDERET=I_HIDE;	

END;
--GO

CREATE OR ALTER PROCEDURE  {objectQualifier}GET_USERSTYLE
(	
	I_USERID integer
)
RETURNS (O_STYLE VARCHAR(255))
AS
DECLARE VARIABLE ICI_STYLE varchar(255);
BEGIN


	 SELECT FIRST 1 c.STYLE FROM {objectQualifier}USER a 
						JOIN {objectQualifier}USERGROUP b
						  ON a.USERID = b.USERID
							JOIN {objectQualifier}GROUP c                         
							  ON b.GroupID = c.GroupID 
							  WHERE a.USERID = :I_USERID AND CHAR_LENGTH(c.STYLE) > 3 ORDER BY c.SORTORDER ASC
							  INTO :ICI_STYLE;
	   if ( ICI_STYLE is null OR CHAR_LENGTH(ICI_STYLE) < 4 ) THEN               
							  SELECT FIRST 1 c.STYLE FROM {objectQualifier}RANK c 
								JOIN {objectQualifier}USER d
								  ON c.RANKID = d.RANKID 
								  WHERE d.USERID = :I_USERID AND CHAR_LENGTH(c.STYLE) > 3 ORDER BY c.RANKID DESC
								  INTO :ICI_STYLE;                
	  
	  SELECT :ICI_STYLE FROM RDB$DATABASE INTO :O_STYLE;
	  SUSPEND;
END;
--GO

-- Gets the Thanks info which will be formatted and then placed in "dvThanksInfo" Div Tag in displaypost.ascx.
CREATE OR ALTER PROCEDURE   {objectQualifier}MESSAGE_GETTHANKSINFO
(
I_MESSAGEID  INT,
I_SHOWTHANKSDATE SMALLINT
) returns ( OUT_VALUE VARCHAR(4000) )
AS
DECLARE VARIABLE TMP_VALUE1 VARCHAR(4000);
DECLARE VARIABLE TMP_VALUE2 VARCHAR(4000);
DECLARE VARIABLE TMP_VALUE3 VARCHAR(4000);
BEGIN
	SELECT	
	(SELECT COALESCE((:OUT_VALUE || ','), '') FROM RDB$DATABASE),	      
	(CAST(i.THANKSFROMUSERID AS varchar(4000))), 
	(CASE :I_SHOWTHANKSDATE  WHEN 1 THEN (',' || CAST(i.THANKSDATE AS VARCHAR(4000)))  ELSE '' END)
			FROM	{objectQualifier}THANKS i
			WHERE	i.MESSAGEID = :I_MESSAGEID  ORDER BY i.THANKSDATE
INTO :TMP_VALUE1, :TMP_VALUE2, :TMP_VALUE3;
OUT_VALUE =:TMP_VALUE1 || :TMP_VALUE2 || :TMP_VALUE3;
	-- Add the last comma if OUT_VALUE  has data.
	IF (:OUT_VALUE <> '') THEN
	BEGIN
		OUT_VALUE = (:OUT_VALUE || ',');
	END
	SELECT :OUT_VALUE FROM RDB$DATABASE INTO :OUT_VALUE;
SUSPEND;
END;

--GO



CREATE OR ALTER PROCEDURE   {objectQualifier}FORUM_SAVE_PARENTSCHECKER(I_FORUMID INTEGER, I_PARENTID INTEGER)
RETURNS (O_DEPENDENCY INTEGER) 
as
DECLARE VARIABLE ICI_HASCHILDREN INTEGER DEFAULT 0;
DECLARE VARIABLE ICI_FRMTMP INTEGER;
DECLARE VARIABLE ICI_PRNTMP INTEGER;
begin
O_DEPENDENCY = 0;
-- Checks if the forum is already referenced as a parent    
	select FORUMID from {objectQualifier}FORUM where PARENTID = :I_FORUMID AND FORUMID = :I_PARENTID
	into :O_DEPENDENCY;
	if (:O_DEPENDENCY > 0) then 
	BEGIN
	  SELECT :I_PARENTID FROM RDB$DATABASE into :O_DEPENDENCY;
	  SUSPEND;
	  END
	  ELSE
	  BEGIN   

	if (exists(select 1 from {objectQualifier}FORUM where PARENTID=:I_FORUMID)) THEN
		begin        
		for
		select FORUMID,PARENTID from {objectQualifier}FORUM
		where PARENTID = :I_FORUMID        
		INTO :ICI_FRMTMP, :ICI_PRNTMP
		 DO
		 BEGIN     
		if (:ICI_FRMTMP > 0 AND :ICI_FRMTMP IS NOT NULL) THEN
		 begin 
				EXECUTE PROCEDURE {objectQualifier}FORUM_SAVE_PARENTSCHECKER :ICI_FRMTMP, :I_PARENTID
				RETURNING_VALUES :ICI_HASCHILDREN;  
		   
		   if  (:ICI_PRNTMP = :I_PARENTID) THEN
			begin
			O_DEPENDENCY= :I_PARENTID;
			end    
			else if (:ICI_HASCHILDREN > 0) THEN
			begin
			O_DEPENDENCY = :ICI_HASCHILDREN;            
			end  
			select :O_DEPENDENCY from RDB$DATABASE INTO :O_DEPENDENCY;  
		end 
	  end             
	end
	 SUSPEND;
	END   
end;
--GO

CREATE  OR ALTER PROCEDURE {objectQualifier}REGISTRY_VALUE (
	I_NAME VARCHAR(64)
	,I_BOARDID INTEGER
	)
RETURNS (ICI_RETURNVALUE BLOB)
AS
-- DECLARE ICI_RETURNVALUE BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET UTF8 COLLATE UNICODE;
BEGIN
	

	IF (:I_BOARDID IS NOT NULL AND EXISTS(SELECT 1 FROM {objectQualifier}REGISTRY WHERE LOWER("NAME") = LOWER(:I_NAME) AND BOARDID = :I_BOARDID)) THEN
	BEGIN
	   
			SELECT "VALUE"
			FROM {objectQualifier}REGISTRY
			WHERE LOWER("NAME") = LOWER(:I_NAME) AND BOARDID = :I_BOARDID
			INTO :ICI_RETURNVALUE;
	END
	ELSE
	BEGIN       
			SELECT "VALUE"
			FROM {objectQualifier}REGISTRY
			WHERE LOWER("NAME") = LOWER(:I_NAME) AND BOARDID IS NULL INTO :ICI_RETURNVALUE;
	END

	 SUSPEND;
END;
--GO


create procedure  {objectQualifier}TOPIC_TAGSAVE(I_TOPICID INTEGER, I_TAGS VARCHAR(1024))
as
declare variable LASTPOS integer;
declare variable NEXTPOS integer;
declare variable TEMPSTR varchar(4096);
declare variable THISID integer;
declare taglowered varchar(4096); 
begin

DELETE FROM {objectQualifier}TOPICTAGS WHERE TOPICID = :I_TOPICID;
  I_TAGS = :I_TAGS || ',';
  LASTPOS = 1;
  NEXTPOS = POSITION(',', :I_TAGS, LASTPOS);
  while (:NEXTPOS > 1) do
  begin
	TEMPSTR = SUBSTRING(:I_TAGS from :LASTPOS for :NEXTPOS - :LASTPOS);

	IF (EXISTS(SELECT FIRST 1 1 FROM {objectQualifier}TAGS where TAG = :TEMPSTR)) THEN
	BEGIN
	UPDATE {objectQualifier}TAGS 
	SET TAG = :TEMPSTR
	WHERE LOWER(TAG) = LOWER(:TEMPSTR);
	END
	ELSE
	BEGIN
	INSERT INTO  {objectQualifier}TAGS(TAG)
	VALUES (:TEMPSTR) 	
	RETURNING TAGID INTO :THISID;
	END	
	
	IF (NOT EXISTS(SELECT FIRST 1 1 FROM {objectQualifier}TOPICTAGS where TAGID = :THISID and TOPICID =  :I_TOPICID)) THEN
	BEGIN
	INSERT INTO  {objectQualifier}TOPICTAGS(TAGID,TOPICID)
	VALUES (:THISID,:I_TOPICID);	
	END	
	UPDATE {objectQualifier}TAGS SET TAGCOUNT = (SELECT COUNT(TAGID) FROM {objectQualifier}TOPICTAGS WHERE  TAGID = :THISID) WHERE  TAGID = :THISID;
	LASTPOS = :NEXTPOS + 1;
	NEXTPOS = POSITION(',', :I_TAGS, LASTPOS);
  end
  suspend;
end;
--GO
CREATE PROCEDURE  {objectQualifier}TOPIC_GETTAGS_STR(I_TOPICID INTEGER)
RETURNS ("TAGS" varchar(4000))
AS
DECLARE ICI_TAGSSTRING varchar(4000) DEFAULT '';
BEGIN
FOR	
   SELECT tg.TAG 
   FROM {objectQualifier}TAGS tg 
   JOIN {objectQualifier}TOPICTAGS tt ON tt.TAGID = tg.TAGID 
   WHERE tt.TOPICID = :I_TOPICID INTO :"TAGS" 
DO
BEGIN
IF (:ICI_TAGSSTRING <> '') THEN	
ICI_TAGSSTRING = :ICI_TAGSSTRING || ',' || :"TAGS" ;
ELSE
ICI_TAGSSTRING = :"TAGS" ;
END
SELECT :ICI_TAGSSTRING FROM RDB$DATABASE INTO :"TAGS";
SUSPEND;	
END;
--GO

