-- This scripts for PostgreSQL Yet Another Forum https://github.com/vzrus/YetAnotherForumExtraDataLayers http://sourceforge.net/projects/yafdotnet/
-- were created by vzrus from vz-team  https://github.com/vzrus
-- They are distributed under terms of GPLv2 licence only as in http://www.fsf.org/licensing/licenses/gpl.html
-- Copyright vzrus(c) 2009-2012

CREATE OR REPLACE FUNCTION databaseSchema.objectQualifier_create_or_check_tables()
                  RETURNS void AS
$BODY$
BEGIN

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_accessmask' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_accessmask 
            (
             accessmaskid              serial NOT NULL,
             boardid                   integer NOT NULL,
             name                      varchar(128) NOT NULL CHECK (name <> ''),
             flags                     integer DEFAULT 0 NOT NULL,
             sortorder                 integer DEFAULT 0 NOT NULL,
             createdbyuserid           integer,
             createdbyusername         varchar(255),
             createdbyuserdisplayname  varchar(255),
             createddate               timestamp,
             isusermask                boolean  DEFAULT false NOT NULL,
             isadminmask               boolean  DEFAULT false NOT NULL
            ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_accessmaskhistory' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_accessmaskhistory 
            (
             accessmaskid              integer NOT NULL,
             changeduserid             integer,
             changedusername           varchar(255) NOT NULL,
             changeddisplayname        varchar(255) NOT NULL,		
             changeddate               timestamp
            ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_active' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_active 
            (
             sessionid                 varchar(24) NOT NULL,
             boardid                   integer NOT NULL,
             userid                    integer NOT NULL,
             ip                        varchar(39) NOT NULL CHECK (ip <> ''),
             login                     timestamp  NOT NULL,
             lastactive                timestamp  NOT NULL,
             location                  varchar(1024) NOT NULL,
             forumid                   integer,
             topicid                   integer,
             browser                   varchar(128),
             platform                  varchar(128),
             forumpage                 varchar(1024) NOT NULL,
             flags                     integer NOT NULL default 0
            ) 
        WITH (OIDS=withOIDs,fillfactor=10,autovacuum_enabled=true);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_activeaccess' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_activeaccess
             (
             userid                    integer NOT NULL,
             boardid                   integer NOT NULL,
             forumid                   integer NOT NULL,
             isadmin                   boolean NOT NULL default false,
             isforummoderator	       boolean NOT NULL default false,
             ismoderator    	       boolean NOT NULL default false,
             isguestx			       boolean NOT NULL default false,
             lastactive			       timestamp ,
             readaccess			       boolean NOT NULL default false,
             postaccess			       boolean NOT NULL default false,
             replyaccess		       boolean NOT NULL default false,
             priorityaccess		       boolean NOT NULL default false,
             pollaccess			       boolean NOT NULL default false,
             voteaccess			       boolean NOT NULL default false,
             moderatoraccess	       boolean NOT NULL default false,
             editaccess			       boolean NOT NULL default false,
             deleteaccess		       boolean NOT NULL default false,
             uploadaccess		       boolean NOT NULL default false,
             downloadaccess		       boolean NOT NULL default false,
             userforumaccess		   boolean NOT NULL default false			
             ) 
       WITH (OIDS=withOIDs,fillfactor=10,autovacuum_enabled=true);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
               AND tablename='objectQualifier_adminpageuseraccess' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_adminpageuseraccess
             (
             userid                    integer NOT NULL,
             pagename                  varchar(128) NOT NULL CHECK (pagename <> '')
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
               AND tablename='objectQualifier_eventloggroupaccess' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_eventloggroupaccess(
        groupid		    integer NOT NULL,	
        eventtypeid     integer NOT NULL,  	
        eventtypename	varchar(128) NOT NULL CHECK (eventtypename <> ''),
        deleteaccess    boolean NOT NULL default false
    )
WITH (OIDS=withOIDs,autovacuum_enabled=true);
END IF;


IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
               AND tablename='objectQualifier_attachment' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_attachment
             (
             attachmentid              serial NOT NULL,
             messageid                 integer NOT NULL,
             filename                  varchar(255) NOT NULL CHECK (filename <> ''),
             bytes                     integer NOT NULL,
             fileid                    integer,
             contenttype               varchar(128),
             downloads                 integer NOT NULL,
             filedata                  bytea
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
               AND tablename='objectQualifier_bannedip' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_bannedip
             (
             id                        serial NOT NULL,
             boardid                   integer NOT NULL,
             mask                      varchar(57) NOT NULL,
             since                     timestamp  NOT NULL,
             reason                    varchar(128),
             userid	                   integer
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_bbcode' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_bbcode
            (
             bbcodeid                  serial NOT NULL,
             boardid                   integer NOT NULL,
             name                      varchar(255) NOT NULL CHECK (name <> ''),
             description               varchar(4000),
             onclickjs                 varchar(1000),
             displayjs                 text,
             editjs                    text,
             displaycss                text,
             searchregex               text,
             replaceregex              text,
             variables                 varchar(1000),
             usemodule                 boolean,
             moduleclass               varchar(255),
             execorder                 integer NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema'
                 AND tablename='objectQualifier_board' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_board
             (
             boardid                   serial NOT NULL,
             name                      varchar(128) NOT NULL CHECK (name <> ''),
             allowthreaded             boolean NOT NULL,
             membershipappname         varchar(255),
             rolesappname              varchar(255)
             ) 
        WITH (OIDS=withOIDs);
END IF;
IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_category' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_category
             (
             categoryid                serial NOT NULL,
             boardid                   integer NOT NULL,
             name                      varchar(128) NOT NULL CHECK (name <> ''),
             categoryimage             varchar(255),
             sortorder                 smallint NOT NULL,
             pollgroupid               int,
             canhavepersforums         boolean  DEFAULT false NOT NULL
             ) 			 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_checkemail' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_checkemail
             (
             checkemailid              serial NOT NULL,
             userid                    integer NOT NULL,
             email                     varchar(128) NOT NULL CHECK (email <> ''),
             created                   timestamp  NOT NULL,
             hash                      varchar(32) NOT NULL CHECK (hash <> '') 
             ) 
        WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_choice' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_choice
             (
             choiceid                  serial NOT NULL,
             pollid                    integer NOT NULL,
             choice                    varchar(128) NOT NULL CHECK (choice <> ''),
             votes                     integer NOT NULL,
             objectpath                varchar(255),
             mimetype                  varchar(50)
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_eventlog' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_eventlog
             (
             eventlogid                serial NOT NULL,
             eventtime                 timestamp  NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
             userid                    integer,
             source                    varchar(128) NOT NULL CHECK (source <> ''),
             description               text NOT NULL CHECK (description <> ''),
             type                      integer DEFAULT 0 NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_extension' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_extension
             (
             extensionid               serial NOT NULL,
             boardid                   integer DEFAULT 1 NOT NULL,
             extension                 varchar(10) NOT NULL CHECK (extension <> '')
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables
               where schemaname='databaseSchema' 
               AND tablename='objectQualifier_forum' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_forum
             (
             forumid                   serial NOT NULL,
             categoryid                integer NOT NULL,
             parentid                  integer,
             name                      varchar(128) NOT NULL CHECK (name <> ''),
             description               varchar(255),
             sortorder                 integer NOT NULL CHECK (sortorder >= 0),
             lastposted                timestamp ,
             lasttopicid               integer,
             lastmessageid             integer,
             lastuserid                integer,
             lastusername              varchar(128),
             lastuserdisplayname       varchar(128),
             numtopics                 integer NOT NULL CHECK (numtopics >= 0),
             numposts                  integer NOT NULL CHECK (numposts >= 0),
             remoteurl                 varchar(255),
             flags                     integer DEFAULT 0 NOT NULL CHECK (flags >= 0),
             themeurl                  varchar(255),
             imageurl                  varchar(255),
             styles                    varchar(255),
             pollgroupid               integer,
             createdbyuserid           integer,
             createdbyusername         varchar(255),
             createdbyuserdisplayname  varchar(255),
             createddate               timestamp,
             isuserforum               boolean DEFAULT false NOT NULL,
             canhavepersforums         boolean DEFAULT false NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_forumhistory' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_forumhistory 
            (
             forumid                   integer  NOT NULL,
             changeduserid             integer,
             changedusername           varchar(255) NOT NULL,
             changeddisplayname        varchar(255) NOT NULL,		
             changeddate               timestamp
            ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_forumaccess' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_forumaccess
             (
             groupid                   integer NOT NULL,
             forumid                   integer NOT NULL,
             accessmaskid              integer NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_group' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_group
             (
             groupid                   serial NOT NULL,
             boardid                   integer NOT NULL,
             name                      varchar(128) NOT NULL,
             flags                     integer DEFAULT 0 NOT NULL CHECK (flags >= 0),
             pmlimit                   integer DEFAULT 0 NOT NULL,
             style                     varchar(255),
             sortorder                 integer DEFAULT 0 NOT NULL,
             description               varchar(128),
             usrsigchars               integer  DEFAULT 0 NOT NULL,
             usrsigbbcodes	           varchar(255),
             usrsightmltags            varchar(255),
             usralbums                 integer  DEFAULT 0 NOT NULL,
             usralbumimages            integer  DEFAULT 0 NOT NULL,
             createdbyuserid           integer,
             createdbyusername         varchar(255),
             createdbyuserdisplayname  varchar(255),
             createddate               timestamp,			
             isusergroup               boolean  DEFAULT false NOT NULL,
             ishidden                  boolean  DEFAULT false NOT NULL,
             usrpersonalforums         integer  DEFAULT 0 NOT NULL,
             usrpersonalmasks          integer  DEFAULT 0 NOT NULL,
             usrpersonalgroups         integer  DEFAULT 0 NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_grouphistory' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_grouphistory 
            (
             groupid              integer NOT NULL,
             changeduserid             integer,
             changedusername           varchar(255) NOT NULL,
             changeddisplayname        varchar(255) NOT NULL,		
             changeddate               timestamp
            ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_groupmedal' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_groupmedal
             (
             groupid                   integer NOT NULL,
             medalid                   integer NOT NULL,
             message                   varchar(128),
             hide                      boolean DEFAULT false NOT NULL,
             onlyribbon                boolean DEFAULT false NOT NULL,
             sortorder                 smallint DEFAULT 255 NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
               AND tablename='objectQualifier_mail' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_mail
             (
             mailid                    serial NOT NULL,
             fromuser                  varchar(128) NOT NULL,
             fromusername              varchar(128),
             touser                    varchar(128) NOT NULL,
             tousername                varchar(128),
             created                   timestamp  NOT NULL,
             subject                   varchar(128) NOT NULL,
             body                      text NOT NULL,
             bodyhtml                  text,
             sendtries                 integer DEFAULT 0 NOT NULL,
             sendattempt               timestamp ,
             processid                 integer
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
              where schemaname='databaseSchema' 
              AND tablename='objectQualifier_medal' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_medal
            (
             medalid                   serial NOT NULL,
             boardid                   integer NOT NULL,
             name                      varchar(128) NOT NULL,
             description               text NOT NULL,
             message                   varchar(128) NOT NULL,
             category                  varchar(128),
             medalurl                  varchar(255) NOT NULL,
             ribbonurl                 varchar(255),
             smallmedalurl             varchar(255) NOT NULL,
             smallribbonurl            varchar(255),
             smallmedalwidth           smallint NOT NULL,
             smallmedalheight          smallint NOT NULL,
             smallribbonwidth          smallint,
             smallribbonheight         smallint,
             sortorder                 smallint DEFAULT 255 NOT NULL,
             flags                     integer DEFAULT 0 NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_message' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_message
             (
             messageid                 serial NOT NULL,
             topicid                   integer NOT NULL,
             replyto                   integer,
             position                  integer NOT NULL,
             indent                    integer NOT NULL,
             userid                    integer NOT NULL,
             username                  varchar(128),
             userdisplayname           varchar(128),
             posted                    timestamp  NOT NULL,
             message                   text NOT NULL,
			 description               varchar(255),
             ip                        varchar(39) NOT NULL,
             edited                    timestamp ,
             flags                     integer DEFAULT 23 NOT NULL,
             editreason                varchar(128),
             ismoderatorchanged        boolean DEFAULT false NOT NULL,
             deletereason              varchar(128),
             blogpostid                varchar(128),
             editedby                  integer,
             externalmessageid         varchar(255),
             referencemessageid	       varchar(255),
             isdeleted                 boolean DEFAULT false NOT NULL,
             isapproved                boolean DEFAULT false NOT NULL,
             ts_message                text
             )
        WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_messagereported' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_messagereported
             (
             messageid                 integer NOT NULL,
             message                   text,
             resolved                  boolean,
             resolvedby                integer,
             resolveddate              timestamp
             )
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
              where schemaname='databaseSchema' 
                AND tablename='objectQualifier_messagereportedaudit' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_messagereportedaudit
             (
             logid                     serial NOT NULL,
             userid                    integer NOT NULL,
             messageid                 integer NOT NULL,
             reported                  timestamp NOT NULL,
             reportednumber            integer,
             reporttext                varchar(4000)
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_nntpforum' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_nntpforum
            (
             nntpforumid               serial NOT NULL,
             nntpserverid              integer NOT NULL,
             groupname                 varchar(128) NOT NULL,
             forumid                   integer NOT NULL,
             lastmessageno             integer NOT NULL,
             lastupdate                timestamp  NOT NULL,
             active                    boolean NOT NULL,
             datecutoff	               timestamp 
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_nntpserver' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_nntpserver
             (
             nntpserverid              serial NOT NULL,
             boardid                   integer NOT NULL,
             name                      varchar(128) NOT NULL,
             address                   varchar(100) NOT NULL,
             port                      integer,
             username                  varchar(128),
             userpass                  varchar(128)
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_nntptopic' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_nntptopic
             (
             nntptopicid               serial NOT NULL,
             nntpforumid               integer NOT NULL,
             thread                    varchar(64) NOT NULL,
             topicid                   integer NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_pmessage' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_pmessage
             (
             pmessageid                serial NOT NULL,
             replyto                   integer,
             fromuserid                integer NOT NULL,
             created                   timestamp  NOT NULL,
             subject                   varchar(128) NOT NULL,
             body                      text NOT NULL,
             flags                     integer DEFAULT 0 NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;
IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_pollgroupcluster' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_pollgroupcluster
             (
             pollgroupid               serial NOT NULL,
             userid                    integer NOT NULL,
             flags                     integer NOT NULL DEFAULT 0
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_poll' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_poll
             (
             pollid                    serial NOT NULL,
             question                  varchar(128) NOT NULL,
             closes                    timestamp ,
             pollgroupid               integer,
             userid                    integer not null,
             objectpath                varchar(255),
             mimetype                  varchar(50),
             flags                     integer  NOT NULL DEFAULT 0
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_pollvote' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_pollvote
             (
             pollvoteid                serial NOT NULL,
             pollid                    integer NOT NULL,
             userid                    integer,
             remoteip                  varchar(39),
             choiceid                  integer
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_pollvoterefuse' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_pollvoterefuse
             (
             refuseid                  serial NOT NULL,
             pollid                    integer NOT NULL,
             userid                    integer,
             remoteip                  varchar(57)
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
               AND tablename='objectQualifier_rank' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_rank
             (
             rankid                    serial NOT NULL,
             boardid                   integer NOT NULL,
             name                      varchar(128) NOT NULL,
             minposts                  integer,
             rankimage                 varchar(128),
             flags                     integer DEFAULT 0 NOT NULL,
             pmlimit                   integer DEFAULT 0 NOT NULL,
             style                     varchar(255),
             sortorder                 smallint DEFAULT 0 NOT NULL,
             description               varchar(128),
             usrsigchars               integer DEFAULT 0 NOT NULL,
             usrsigbbcodes	           varchar(255),
             usrsightmltags            varchar(255),
             usralbums                 integer DEFAULT 0 NOT NULL,
             usralbumimages            integer DEFAULT 0 NOT NULL
             ) 
        WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
               AND tablename='objectQualifier_registry' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_registry
             (
             registryid                serial NOT NULL,
             name                      varchar(128) NOT NULL,
             value                     text,
             boardid                   integer
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
               AND tablename='objectQualifier_replace_words' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_replace_words
             (
             id                        serial NOT NULL,
             boardid                   integer NOT NULL,
             badword                   varchar(255),
             goodword                  varchar(255)
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_smiley' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_smiley
             (
             smileyid                  serial NOT NULL,
             boardid                   integer NOT NULL,
             code                      varchar(10) NOT NULL,
             icon                      varchar(128) NOT NULL,
             emoticon                  varchar(128),
             sortorder                 smallint DEFAULT 0 NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_topic' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_topic
             (
             topicid                   serial NOT NULL,
             forumid                   integer NOT NULL,
             userid                    integer NOT NULL,
             username                  varchar(128),
             userdisplayname           varchar(128),
             posted                    timestamp  NOT NULL,
             topic                     varchar(128) NOT NULL,
             views                     integer NOT NULL,
             priority                  smallint NOT NULL,
             pollid                    integer,
             topicmovedid              integer,
             lastposted                timestamp ,
             lastmessageid             integer,
             lastuserid                integer,
             lastusername              varchar(128),
             lastuserdisplayname       varchar(128),
             numposts                  integer NOT NULL,
             flags                     integer DEFAULT 0 NOT NULL,
             answermessageid           integer NULL,
             lastmessageflags          integer NOT null default 22,
             description               varchar(255),
             status                    varchar(255),
             styles                    varchar(255),
             islocked                  boolean DEFAULT FALSE NOT NULL,
             isdeleted                 boolean DEFAULT FALSE NOT NULL,
             ispersistent              boolean DEFAULT FALSE NOT NULL,
             isquestion                boolean DEFAULT FALSE NOT NULL,
             linkdate                  timestamp,
             topicimage                varchar(255),  
             topicimagetype            varchar(50),  
             topicimagebin             bytea  
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_user' LIMIT 1) THEN
CREATE TABLE databaseSchema.objectQualifier_user
             (
             userid                    serial NOT NULL,
             boardid                   integer NOT NULL,
             provideruserkey           varchar(64),
             name                      varchar(128) NOT NULL,
             password                  varchar(32) NOT NULL,
             email                     varchar(128),
             joined                    timestamp  NOT NULL,
             lastvisit                 timestamp  NOT NULL,
             ip                        varchar(39),
             numposts                  integer NOT NULL,
             timezone                  integer NOT NULL,
             avatar                    varchar(255),
             signature                 text,
             avatarimage               bytea,
             avatarimagetype           varchar(128),
             rankid                    integer NOT NULL,
             suspended                 timestamp ,
             languagefile              varchar(128),
             themefile                 varchar(128),
             overridedefaultthemes     boolean DEFAULT false NOT NULL,
             pmnotification            boolean DEFAULT true NOT NULL,
             flags                     integer DEFAULT 0 NOT NULL,
             points                    integer DEFAULT 1 NOT NULL,
             autowatchtopics           boolean NOT NULL DEFAULT FALSE,
             displayname               varchar(128) NOT NULL,
             culture                   varchar(10),
             dailydigest               boolean NOT NULL DEFAULT FALSE,
             notificationtype          integer,
             texteditor	               varchar(50) NULL,
             usesinglesignon           boolean NOT NULL  DEFAULT FALSE,
             isfacebookuser            boolean NOT NULL  DEFAULT FALSE,
             istwitteruser             boolean NOT NULL  DEFAULT FALSE,
             ishostadmin               boolean NOT NULL  DEFAULT FALSE,
             isapproved                boolean NOT NULL  DEFAULT FALSE,
             isguest                   boolean NOT NULL  DEFAULT TRUE,
             iscaptchaexcluded         boolean NOT NULL  DEFAULT FALSE,
             isactiveexcluded          boolean NOT NULL  DEFAULT FALSE,
             isdst                     boolean NOT NULL  DEFAULT FALSE,
             isdirty                   boolean NOT NULL  DEFAULT FALSE,
             userstyle                 varchar(255),
             styleflags                integer DEFAULT 0 NOT NULL,
             isuserstyle               boolean NOT NULL  DEFAULT FALSE, 
             isgroupstyle              boolean NOT NULL  DEFAULT FALSE, 
             isrankstyle               boolean NOT NULL  DEFAULT FALSE, 
             commonviewtype            integer NOT NULL DEFAULT 0,
             postsperpage              integer NOT NULL DEFAULT 10, 
             topicsperpage              integer NOT NULL DEFAULT 20  
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_userprofile' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_userprofile
             (
             userid                    integer NOT NULL,
             lastupdateddate           timestamp  NOT NULL,			
             lastactivity              timestamp ,
             applicationname           varchar(255) NOT NULL,
             isanonymous               boolean DEFAULT false NOT NULL,
             username                  varchar(255) NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_userpmessage' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_userpmessage
             (
             userpmessageid            serial NOT NULL,
             userid                    integer NOT NULL,
             pmessageid                integer NOT NULL,
             flags                     integer DEFAULT 0 NOT NULL,
             isread                    boolean DEFAULT false NOT NULL,
             isinoutbox                boolean DEFAULT false NOT NULL,
             isarchived                boolean DEFAULT false NOT NULL,
             isdeleted                 boolean DEFAULT false NOT NULL,
             isreply                   boolean DEFAULT false NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_userforum' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_userforum
             (
             userid                    serial NOT NULL,
             forumid                   integer NOT NULL,
             accessmaskid              integer NOT NULL,
             invited                   timestamp  NOT NULL,
             accepted                  boolean NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_usergroup' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_usergroup
             (
             userid                    integer NOT NULL,
             groupid                   integer NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_usermedal' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_usermedal
             (
             userid                    integer NOT NULL,
             medalid                   integer NOT NULL,
             message                   varchar(128),
             hide                      boolean DEFAULT false NOT NULL,
             onlyribbon                boolean DEFAULT false NOT NULL,
             sortorder                 smallint DEFAULT 255 NOT NULL,
             dateawarded               timestamp  NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_ignoreuser' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_ignoreuser
            (
             userid                    integer NOT NULL,
             ignoreduserid             integer NOT NULL
            )
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_watchforum' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_watchforum
            (
             watchforumid              serial NOT NULL,
             forumid                   integer NOT NULL,
             userid                    integer NOT NULL,
             created timestamp  NOT NULL,
             lastmail timestamp
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_watchtopic' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_watchtopic
             (
             watchtopicid              serial NOT NULL,
             topicid                   integer NOT NULL,
             userid                    integer NOT NULL,
             created                   timestamp  NOT NULL,
             lastmail                  timestamp
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_shoutboxmessage' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_shoutboxmessage
             (
             shoutboxmessageid         serial NOT NULL,
             userid                    integer,
             username                  varchar(128) NOT NULL,
             userdisplayname           varchar(128) NOT NULL,
             message                   text,
             "date"                    timestamp  NOT NULL,
             "ip"                      varchar(128) NOT NULL,
             boardid integer
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_thanks' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_thanks
             (
             thanksid                  serial NOT NULL,
             thanksfromuserid          integer NOT NULL,
             thankstouserid            integer NOT NULL,
             messageid                 integer NOT NULL,
             thanksdate                timestamp  NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

-- YAF Buddy Table

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                  AND tablename='objectQualifier_buddy' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_buddy
             (
             id                        serial NOT NULL,
             fromuserid                integer NOT NULL,
             touserid                  integer NOT NULL,
             approved                  boolean NOT NULL,
             requested                 timestamp  NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

-- FavoriteTopic Table

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_favoritetopic' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_favoritetopic
             (
             id                        serial NOT NULL,		
             userid                    integer NOT NULL,
             topicid                   integer NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

-- UserAlbum Table
IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_useralbum' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_useralbum
             (
             albumid                   serial NOT NULL,		
             userid                    integer NOT NULL,
             title                     varchar(255),	
             coverimageid              integer,
             updated                   timestamp  NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables where schemaname='databaseSchema' AND tablename='objectQualifier_useralbumimage' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_useralbumimage
             (
             imageid                   serial NOT NULL,
             albumid                   integer NOT NULL,
             caption                   varchar(255),	
             filename                  varchar(255) NOT NULL,
             bytes                     integer NOT NULL,
             contenttype               varchar(50),
             uploaded                  timestamp  NOT NULL,
             downloads                 integer NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_messagehistory' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_messagehistory
             (
             messageid		           integer NOT NULL,
             message		           text NOT NULL,
             ip				           varchar(15) NOT NULL,
             edited			           timestamp  NULL,
             editreason                varchar(100) NULL,
             ismoderatorchanged        boolean DEFAULT false NOT NULL,
             flags                     integer NOT NULL DEFAULT 0,
             editedby		           int NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;


IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_topicreadtracking' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_topicreadtracking
             (
             userid			           int NOT NULL,
             topicid			       int NOT NULL ,
             lastaccessdate	           timestamp  NOT NULL
             )
       WITH (OIDS=withOIDs);
END IF;

-- Create Forum Read Tracking Table

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_forumreadtracking' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_forumreadtracking
             (
             userid			           int NOT NULL,
             forumid			       int NOT NULL,
             lastaccessdate	           timestamp  NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_topicstatus' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_topicstatus
             (
             topicstatusid             serial NOT NULL,
             topicstatusname           varchar(100) NOT NULL,
             boardid                   integer NOT NULL,
             defaultdescription        varchar(100) NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;


IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_tags' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_tags
             (
             tagid                     serial NOT NULL,          
             tag                       varchar(1024) NOT NULL,
             tagcount			       integer NOT NULL DEFAULT 0
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_topictags' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_topictags
             (
             tagid                     integer NOT NULL,          
             topicid                   integer NOT NULL			 
             ) 
       WITH (OIDS=withOIDs);
END IF;

IF NOT EXISTS (select 1 from pg_tables 
               where schemaname='databaseSchema' 
                 AND tablename='objectQualifier_reputationvote' limit 1) THEN
CREATE TABLE databaseSchema.objectQualifier_reputationvote
             (        
             reputationfromuserid      integer NOT NULL,
             reputationtouserid        integer NOT NULL,
             votedate                  timestamp NOT NULL
             ) 
       WITH (OIDS=withOIDs);
END IF;

END;
$BODY$
  LANGUAGE 'plpgsql' VOLATILE SECURITY DEFINER STRICT
  COST 100;
    -- GRANT EXECUTE ON FUNCTION databaseSchema.objectQualifier_create_or_check_tables() TO public;
    -- GO
    SELECT databaseSchema.objectQualifier_create_or_check_tables();
    --GO
    DROP FUNCTION databaseSchema.objectQualifier_create_or_check_tables();
--GO

-- add missing columns or change them

CREATE OR REPLACE FUNCTION databaseSchema.objectQualifier_add_or_change_columns()
RETURNS void AS
$BODY$
BEGIN
     -- drop
     /* IF (column_exists('databaseSchema.objectQualifier_watchforum','watchforumid')) THEN
         ALTER TABLE databaseSchema.objectQualifier_watchforum DROP COLUMN watchforumid CASCADE;
     END IF;
     IF (column_exists('databaseSchema.objectQualifier_watchtopic','watchtopicid')) THEN
        ALTER TABLE databaseSchema.objectQualifier_watchtopic DROP COLUMN watchtopicid CASCADE;
     END IF; */
     -- create
     IF (NOT column_exists('databaseSchema.objectQualifier_forum','lastuserdisplayname')) THEN
         ALTER TABLE databaseSchema.objectQualifier_forum ADD COLUMN lastuserdisplayname  varchar(128);
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_topic','userdisplayname')) THEN
         ALTER TABLE databaseSchema.objectQualifier_topic ADD COLUMN userdisplayname   varchar(128);
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_topic','lastuserdisplayname')) THEN
         ALTER TABLE databaseSchema.objectQualifier_topic ADD COLUMN lastuserdisplayname   varchar(128);
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_message','userdisplayname')) THEN
         ALTER TABLE databaseSchema.objectQualifier_message ADD COLUMN userdisplayname   varchar(128);
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_user','userstyle')) THEN
         ALTER TABLE databaseSchema.objectQualifier_user ADD COLUMN userstyle  varchar(255);
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_user','styleflags')) THEN
         ALTER TABLE databaseSchema.objectQualifier_user ADD COLUMN styleflags  integer DEFAULT 0 NOT NULL;
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_user','isuserstyle')) THEN
         ALTER TABLE databaseSchema.objectQualifier_user ADD COLUMN isuserstyle  boolean DEFAULT FALSE NOT NULL;
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_user','isgroupstyle')) THEN
         ALTER TABLE databaseSchema.objectQualifier_user ADD COLUMN isgroupstyle  boolean DEFAULT FALSE NOT NULL;
     END IF;
      IF (NOT column_exists('databaseSchema.objectQualifier_user','isrankstyle')) THEN
         ALTER TABLE databaseSchema.objectQualifier_user ADD COLUMN isrankstyle  boolean DEFAULT FALSE NOT NULL;
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_user','commonviewtype')) THEN
         ALTER TABLE databaseSchema.objectQualifier_user ADD COLUMN commonviewtype  integer NOT NULL DEFAULT 0;
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_user','postsperpage')) THEN
         ALTER TABLE databaseSchema.objectQualifier_user ADD COLUMN postsperpage integer NOT NULL DEFAULT 10;
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_user','topicsperpage')) THEN
         ALTER TABLE databaseSchema.objectQualifier_user ADD COLUMN topicsperpage integer NOT NULL DEFAULT 20;
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_shoutboxmessage','userdisplayname')) THEN
         ALTER TABLE databaseSchema.objectQualifier_shoutboxmessage ADD COLUMN userdisplayname  varchar(128);
     END IF;
     IF (column_exists('databaseSchema.objectQualifier_topic','linkdate') IS FALSE) THEN
         ALTER TABLE databaseSchema.objectQualifier_topic ADD COLUMN linkdate  timestamp;
     END IF;
     IF (column_exists('databaseSchema.objectQualifier_topic','topicimage') IS FALSE) THEN
         ALTER TABLE databaseSchema.objectQualifier_topic ADD COLUMN topicimage varchar(255);
     END IF;
     IF (column_exists('databaseSchema.objectQualifier_topic','topicimagetype') IS FALSE) THEN
         ALTER TABLE databaseSchema.objectQualifier_topic ADD COLUMN topicimagetype varchar(50);
     END IF;
     IF (column_exists('databaseSchema.objectQualifier_topic','topicimagebin') IS FALSE) THEN
         ALTER TABLE databaseSchema.objectQualifier_topic ADD COLUMN  topicimagebin bytea ;
     END IF;        
             
     IF (NOT column_exists('databaseSchema.objectQualifier_message','ts_message')) THEN
         ALTER TABLE databaseSchema.objectQualifier_message ADD COLUMN ts_message  text;
     END IF;

	 IF (NOT column_exists('databaseSchema.objectQualifier_message','description')) THEN
         ALTER TABLE databaseSchema.objectQualifier_message ADD COLUMN description text;
     END IF;	 

     IF (NOT column_exists('databaseSchema.objectQualifier_pmessage','replyto')) THEN
         ALTER TABLE databaseSchema.objectQualifier_pmessage ADD COLUMN replyto  integer;
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_userpmessage','isreply')) THEN
         ALTER TABLE databaseSchema.objectQualifier_userpmessage ADD COLUMN isreply  boolean DEFAULT FALSE NOT NULL ;
     END IF;
     
     IF (NOT column_exists('databaseSchema.objectQualifier_tags','tagcount')) THEN
         ALTER TABLE databaseSchema.objectQualifier_tags ADD COLUMN tagcount  integer DEFAULT 0 NOT NULL ;
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_forum','createdbyuserid')) THEN
         ALTER TABLE databaseSchema.objectQualifier_forum ADD COLUMN createdbyuserid  integer;
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_forum','createdbyusername')) THEN
         ALTER TABLE databaseSchema.objectQualifier_forum ADD COLUMN createdbyusername  varchar(255);
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_forum','createdbyuserdisplayname')) THEN
         ALTER TABLE databaseSchema.objectQualifier_forum ADD COLUMN createdbyuserdisplayname  varchar(255);
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_forum','createddate')) THEN
         ALTER TABLE databaseSchema.objectQualifier_forum ADD COLUMN createddate  timestamp;
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_forum','isuserforum')) THEN
         ALTER TABLE databaseSchema.objectQualifier_forum ADD COLUMN isuserforum  boolean  DEFAULT false NOT NULL ;
     END IF;

         IF (NOT column_exists('databaseSchema.objectQualifier_forum','canhavepersforums')) THEN
         ALTER TABLE databaseSchema.objectQualifier_forum ADD COLUMN canhavepersforums  boolean  DEFAULT false NOT NULL ;
     END IF;     

        if exists (select 1 from information_schema.columns where table_name='objectQualifier_forum' 
        and table_schema='databaseSchema' and column_name ='description' and is_nullable='NO') then
         ALTER TABLE databaseSchema.objectQualifier_forum ALTER COLUMN description DROP NOT NULL;		 
     END IF;		
     
     if exists (select 1 from information_schema.columns where table_name='objectQualifier_forum' 
        and table_schema='databaseSchema' and column_name ='sortorder' and data_type !='integer') then
         ALTER TABLE databaseSchema.objectQualifier_forum ALTER COLUMN sortorder TYPE integer;		
     END IF;
     
      if exists (select 1 from information_schema.columns where table_name='objectQualifier_group' 
        and table_schema='databaseSchema' and column_name ='sortorder' and data_type !='integer') then
         ALTER TABLE databaseSchema.objectQualifier_group ALTER COLUMN sortorder TYPE integer;		
     END IF;

	  if exists (select 1 from information_schema.columns where table_name='objectQualifier_accessmask' 
        and table_schema='databaseSchema' and column_name ='sortorder' and data_type !='integer') then
         ALTER TABLE databaseSchema.objectQualifier_accessmask ALTER COLUMN sortorder TYPE integer;		
     END IF;

        if exists (select 1 from information_schema.columns where table_name='objectQualifier_group' 
        and table_schema='databaseSchema' and column_name ='sortorder' and data_type !='integer') then
         ALTER TABLE databaseSchema.objectQualifier_group ALTER COLUMN sortorder TYPE integer;		
     END IF;

    END;	
$BODY$
  LANGUAGE 'plpgsql' VOLATILE SECURITY DEFINER STRICT
  COST 100;   
--GO
    SELECT databaseSchema.objectQualifier_add_or_change_columns();
--GO
    DROP FUNCTION databaseSchema.objectQualifier_add_or_change_columns();
--GO

CREATE OR REPLACE FUNCTION databaseSchema.objectQualifier_add_or_change_columns1()
RETURNS void AS
$BODY$
BEGIN   
      IF (NOT column_exists('databaseSchema.objectQualifier_group','createdbyuserid')) THEN
         ALTER TABLE databaseSchema.objectQualifier_group ADD COLUMN createdbyuserid  integer;
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_group','createdbyusername')) THEN
         ALTER TABLE databaseSchema.objectQualifier_group ADD COLUMN createdbyusername  varchar(255);
     END IF;     
   

     IF (NOT column_exists('databaseSchema.objectQualifier_group','createdbyuserdisplayname')) THEN
         ALTER TABLE databaseSchema.objectQualifier_group ADD COLUMN createdbyuserdisplayname  varchar(255);
     END IF;

  IF (NOT column_exists('databaseSchema.objectQualifier_group','createddate')) THEN
         ALTER TABLE databaseSchema.objectQualifier_group ADD COLUMN createddate  timestamp;
     END IF;

	 IF (NOT column_exists('databaseSchema.objectQualifier_group','isadmingroup')) THEN
         ALTER TABLE databaseSchema.objectQualifier_group ADD COLUMN isadmingroup  boolean  DEFAULT false NOT NULL ;
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_group','isusergroup')) THEN
         ALTER TABLE databaseSchema.objectQualifier_group ADD COLUMN isusergroup  boolean  DEFAULT false NOT NULL ;
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_group','ishidden')) THEN
         ALTER TABLE databaseSchema.objectQualifier_group ADD COLUMN ishidden  boolean  DEFAULT false NOT NULL;
     END IF;
     
     IF (NOT column_exists('databaseSchema.objectQualifier_group','usrpersonalforums')) THEN
         ALTER TABLE databaseSchema.objectQualifier_group ADD COLUMN  usrpersonalforums  integer  DEFAULT 0 NOT NULL;
     END IF;	
    IF (NOT column_exists('databaseSchema.objectQualifier_group','usrpersonalmasks')) THEN
         ALTER TABLE databaseSchema.objectQualifier_group ADD COLUMN  usrpersonalmasks  integer  DEFAULT 0 NOT NULL;
     END IF; 

     IF (NOT column_exists('databaseSchema.objectQualifier_group','usrpersonalgroups')) THEN
         ALTER TABLE databaseSchema.objectQualifier_group ADD COLUMN  usrpersonalgroups  integer  DEFAULT 0 NOT NULL;
     END IF;

         IF (NOT column_exists('databaseSchema.objectQualifier_accessmask','createdbyuserid')) THEN
         ALTER TABLE databaseSchema.objectQualifier_accessmask ADD COLUMN createdbyuserid  integer;
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_accessmask','createdbyusername')) THEN
         ALTER TABLE databaseSchema.objectQualifier_accessmask ADD COLUMN createdbyusername  varchar(255);
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_accessmask','createdbyuserdisplayname')) THEN
         ALTER TABLE databaseSchema.objectQualifier_accessmask ADD COLUMN createdbyuserdisplayname  varchar(255);
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_accessmask','createddate')) THEN
         ALTER TABLE databaseSchema.objectQualifier_accessmask ADD COLUMN createddate  timestamp;
     END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_accessmask','isusermask')) THEN
         ALTER TABLE databaseSchema.objectQualifier_accessmask ADD COLUMN isusermask  boolean  DEFAULT false NOT NULL ;
     END IF;

     IF (NOT column_exists('databaseSchema.objectQualifier_accessmask','isadminmask')) THEN
         ALTER TABLE databaseSchema.objectQualifier_accessmask ADD COLUMN isadminmask  boolean  DEFAULT false NOT NULL ;
     END IF;

      IF (NOT column_exists('databaseSchema.objectQualifier_category','canhavepersforums')) THEN
         ALTER TABLE databaseSchema.objectQualifier_category ADD COLUMN canhavepersforums  boolean  DEFAULT false NOT NULL ;
     END IF;

	 IF (NOT column_exists('databaseSchema.objectQualifier_userprofile','birthday')) THEN
         ALTER TABLE databaseSchema.objectQualifier_userprofile ADD COLUMN birthday  timestamp;
     END IF;
     /* IF (EXISTS (SELECT 1 FROM pg_indexes WHERE tablename='objectQualifier_forumreadtracking' 
                                               AND indexname='ix_objectQualifier_forumreadtracking_userid_forumid')) THEN
         DROP INDEX  ix_objectQualifier_forumreadtracking_userid_forumid;
     END IF; 
     IF (EXISTS (SELECT 1 FROM pg_indexes WHERE tablename='objectQualifier_topicreadtracking' 
                                               AND indexname='ix_objectQualifier_topicreadtracking_userid_topicid')) THEN
         DROP INDEX  ix_objectQualifier_topicreadtracking_userid_topicid CASCADE;
     END IF; 

     IF (EXISTS (SELECT 1 FROM pg_indexes WHERE tablename='objectQualifier_messagehistory' 
                                               AND indexname='ix_objectQualifier_messagehistory_edited_messageid')) THEN
       DROP INDEX  ix_objectQualifier_messagehistory_edited_messageid CASCADE;
     END IF; */
     IF (EXISTS (SELECT 1 FROM pg_constraint where contype='p' and conname ='pk_databaseSchema_objectQualifier_logid')) THEN
       ALTER TABLE  databaseSchema.objectQualifier_messagereportedaudit DROP CONSTRAINT pk_databaseSchema_objectQualifier_logid CASCADE;
     END IF;
    IF (EXISTS (SELECT 1 FROM pg_attribute where  attrelid = 'databaseSchema.objectQualifier_messagereportedaudit'::regclass and attname='messageid' and attnotnull ='FALSE')) THEN
     ALTER TABLE databaseSchema.objectQualifier_messagereportedaudit ALTER COLUMN messageid SET NOT NULL;
    END IF;
        
        IF (EXISTS (SELECT 1 FROM pg_attribute where  attrelid = 'databaseSchema.objectQualifier_messagereportedaudit'::regclass and attname='userid' and attnotnull ='FALSE')) THEN
     ALTER TABLE databaseSchema.objectQualifier_messagereportedaudit ALTER COLUMN userid SET NOT NULL;
    END IF;	


    IF (EXISTS (SELECT 1 FROM pg_attribute where  attrelid = 'databaseSchema.objectQualifier_accessmask'::regclass and attname='sortorder')) THEN
     ALTER TABLE databaseSchema.objectQualifier_accessmask ALTER COLUMN sortorder TYPE integer;
    END IF;	

        IF (EXISTS (SELECT 1 FROM pg_attribute where  attrelid = 'databaseSchema.objectQualifier_forum'::regclass and attname='sortorder')) THEN
     ALTER TABLE databaseSchema.objectQualifier_forum ALTER COLUMN sortorder TYPE integer;
    END IF;	

        IF (EXISTS (SELECT 1 FROM pg_attribute where  attrelid = 'databaseSchema.objectQualifier_group'::regclass and attname='sortorder')) THEN
     ALTER TABLE databaseSchema.objectQualifier_group ALTER COLUMN sortorder TYPE integer;
    END IF;	


     IF (EXISTS (SELECT 1 FROM pg_attribute where  attrelid = 'databaseSchema.objectQualifier_messagereportedaudit'::regclass and attname='reported' and not attnotnull)) THEN
     ALTER TABLE databaseSchema.objectQualifier_messagereportedaudit ALTER COLUMN reported SET NOT NULL;
    END IF;
     IF (NOT column_exists('databaseSchema.objectQualifier_activeaccess','userforumaccess')) THEN
         ALTER TABLE databaseSchema.objectQualifier_activeaccess ADD COLUMN userforumaccess  boolean  DEFAULT false NOT NULL ;
     END IF;

     UPDATE databaseSchema.objectQualifier_group set style = null where style is not null and char_length(style) <= 2;
     UPDATE databaseSchema.objectQualifier_rank set style = null where style is not null and char_length(style) <= 2;
     UPDATE databaseSchema.objectQualifier_rank set flags = flags | 4 where name like 'Guest';
    END;	
$BODY$
  LANGUAGE 'plpgsql' VOLATILE SECURITY DEFINER STRICT
  COST 100;   
--GO
    SELECT databaseSchema.objectQualifier_add_or_change_columns1();
--GO
    DROP FUNCTION databaseSchema.objectQualifier_add_or_change_columns1();
--GO

CREATE OR REPLACE FUNCTION databaseSchema.objectQualifier_forum_initdisplayname()
RETURNS void AS
$BODY$
declare _rec1 RECORD;
        _rec2 RECORD;
        _rec3 RECORD;
        _rec4 RECORD;
        _userdisplayname varchar(255);
        _lastuserdisplayname varchar(255);
BEGIN		
        for _rec1 IN select forumid, lastuserid from databaseSchema.objectQualifier_forum
        where lastuserdisplayname IS NULL and 	lastuserid is not null		
        FOR UPDATE 
        LOOP
        update databaseSchema.objectQualifier_forum set lastuserdisplayname = (select u.displayname FROM databaseSchema.objectQualifier_user u WHERE u.userid = _rec1.lastuserid) where  databaseSchema.objectQualifier_forum.forumid = _rec1.forumid;	
        update databaseSchema.objectQualifier_forum set lastusername = (select u.name FROM databaseSchema.objectQualifier_user u WHERE u.userid = _rec1.lastuserid) where  databaseSchema.objectQualifier_forum.forumid = _rec1.forumid;	
        END LOOP;		
    
        for _rec2 IN select shoutboxmessageid,userid from databaseSchema.objectQualifier_shoutboxmessage
        where userdisplayname IS NULL
        FOR UPDATE	
        LOOP		
        update databaseSchema.objectQualifier_shoutboxmessage set userdisplayname = (select u.displayname FROM databaseSchema.objectQualifier_user u WHERE u.userid = _rec2.userid) where databaseSchema.objectQualifier_shoutboxmessage.shoutboxmessageid = _rec2.shoutboxmessageid;
        END LOOP;		
        
        for _rec3 IN select messageid,userid from databaseSchema.objectQualifier_message
        where userdisplayname IS NULL
        FOR UPDATE
        LOOP	
        update databaseSchema.objectQualifier_message  set userdisplayname = (select u.displayname FROM databaseSchema.objectQualifier_user u WHERE u.userid = _rec3.userid) where messageid = _rec3.messageid;
        update databaseSchema.objectQualifier_message  set username = (select u.name FROM databaseSchema.objectQualifier_user u WHERE u.userid = _rec3.userid) where messageid = _rec3.messageid;
        END LOOP;		
        
        for _rec4 IN select topicid,userid,lastuserid from databaseSchema.objectQualifier_topic
        where userdisplayname IS NULL OR lastuserdisplayname IS NULL and lastuserid is not null		
        FOR UPDATE 
        LOOP    
        update databaseSchema.objectQualifier_topic set username = (select u.name FROM databaseSchema.objectQualifier_user u WHERE u.userid = _rec4.userid) where topicid = _rec4.topicid;
        update databaseSchema.objectQualifier_topic set userdisplayname = (select u.displayname FROM databaseSchema.objectQualifier_user u WHERE u.userid = _rec4.userid) where topicid = _rec4.topicid;
        update databaseSchema.objectQualifier_topic set lastuserdisplayname = (select u.displayname FROM databaseSchema.objectQualifier_user u WHERE u.userid = _rec4.lastuserid) where topicid = _rec4.topicid;
        update databaseSchema.objectQualifier_topic set lastusername = (select u.name FROM databaseSchema.objectQualifier_user u WHERE u.userid = _rec4.lastuserid) where topicid = _rec4.topicid;
        END LOOP;
 END;	 	
$BODY$
  LANGUAGE 'plpgsql' VOLATILE SECURITY DEFINER STRICT
  COST 100;   
--GO  

CREATE OR REPLACE FUNCTION databaseSchema.objectQualifier_init_dsiplayname_launcher()
RETURNS void AS
$BODY$
BEGIN
if exists (select 1 from databaseSchema.objectQualifier_message where userdisplayname IS NULL LIMIT 1) then
 PERFORM databaseSchema.objectQualifier_forum_initdisplayname();
 end if;
END;
$BODY$
  LANGUAGE 'plpgsql' VOLATILE SECURITY DEFINER STRICT
  COST 100;   
--GO 
    SELECT databaseSchema.objectQualifier_init_dsiplayname_launcher();
--GO

DO $$DECLARE r record; notnulll varchar(10);
BEGIN
    FOR r IN SELECT t.table_name, c.column_name, c.is_nullable 
	FROM information_schema.tables t
	         JOIN 
			 information_schema.columns c
			 ON t.table_name = c.table_name     
			 WHERE t.table_type = 'BASE TABLE' 
			 AND t.table_schema = 'databaseSchema'
			 and c.data_type = 'timestamp with time zone'
    LOOP		
		 notnulll := r.is_nullable;
         EXECUTE 'ALTER TABLE databaseSchema' || '.' ||  r.table_name || ' ALTER COLUMN ' ||  r.column_name  || ' TYPE timestamp without time zone;';
		IF (notnulll NOT LIKE 'YES') THEN	
		 EXECUTE 'ALTER TABLE databaseSchema' || '.' ||  r.table_name || ' ALTER COLUMN ' ||  r.column_name || ' SET NOT NULL;';
		END IF;		
    END LOOP;
END$$;
--GO