// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Vladimir Zakharov" file="Db.cs">
//   VZF by vzrus
//   Copyright (C) 2006-2014 Vladimir Zakharov
//   https://github.com/vzrus
//   http://sourceforge.net/projects/yaf-datalayers/
//    This program is free software; you can redistribute it and/or
//   modify it under the terms of the GNU General Public License
//   as published by the Free Software Foundation; version 2 only 
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//    
//    You should have received a copy of the GNU General Public License
//   along with this program; if not, write to the Free Software
//   Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. 
// </copyright>
// <summary>
//   The common db.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace VZF.Data.Postgre
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Hosting;
    using System.Web.Security;

    using Npgsql;

    using NpgsqlTypes;

    using VZF.Utils;
    using VZF.Utils.Helpers;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Handlers;
    using YAF.Types.Objects;

    /// <summary>
    /// All the Database functions for YAF
    /// </summary>
    public static class Db
    {
        // added vzrus
        #region ConnectionStringOptions

        /// <summary>
        /// Gets the provider assembly name.
        /// </summary>
        public static string ProviderAssemblyName
        {
            get
            {
                return "Npgsql";
            }
        }

        /// <summary>
        /// Gets a value indicating whether password placeholder visible.
        /// </summary>
        public static bool PasswordPlaceholderVisible
        {
            get
            {
                return true;
            }
        }

        #endregion        

        public static bool FullTextSupported = false;

        public static string FullTextScript = "postgre/fulltext.sql";

        #region Forum

        public static DataTable forum_ns_getchildren_anyuser(
            [NotNull] string connectionString,
            int boardid,
            int categoryid,
            int forumid,
            int userid,
            bool notincluded,
            bool immediateonly,
            string indentchars)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_ns_getchildren_anyuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardid;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryid;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumid;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userid;
                cmd.Parameters.Add(new NpgsqlParameter("i_notincluded", NpgsqlDbType.Boolean)).Value = notincluded;
                cmd.Parameters.Add(new NpgsqlParameter("i_immediateonly", NpgsqlDbType.Boolean)).Value = immediateonly;

                DataTable dt = PostgreDbAccess.GetData(cmd, connectionString);
                DataTable sorted = dt.Clone();
                bool forumRow = false;
                foreach (DataRow row in dt.Rows)
                {
                    DataRow newRow = sorted.NewRow();
                    newRow.ItemArray = row.ItemArray;
                    newRow = row;

                    int currentIndent = (int)row["Level"];
                    string sIndent = string.Empty;

                    for (int j = 0; j < currentIndent; j++) sIndent += "--";
                    if (currentIndent == 1 && !forumRow)
                    {
                        newRow["ForumID"] = currentIndent;
                        newRow["Title"] = string.Format(" -{0} {1}", sIndent, row["CategoryName"]);
                        forumRow = true;
                    }
                    else
                    {
                        newRow["ForumID"] = currentIndent;
                        newRow["Title"] = string.Format(" -{0} {1}", sIndent, row["Title"]);
                        forumRow = false;
                    }


                    // import the row into the destination




                    sorted.Rows.Add(newRow);
                }

                return sorted;
            }
        }

        public static DataTable forum_ns_getchildren(
            [NotNull] string connectionString,
            int? boardid,
            int? categoryid,
            int? forumid,
            bool notincluded,
            bool immediateonly,
            string indentchars)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_ns_getchildren"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardid;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryid;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumid;
                cmd.Parameters.Add(new NpgsqlParameter("i_notincluded", NpgsqlDbType.Boolean)).Value = notincluded;
                cmd.Parameters.Add(new NpgsqlParameter("i_immediateonly", NpgsqlDbType.Boolean)).Value = immediateonly;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable forum_ns_getchildren_activeuser(
            [NotNull] string connectionString,
            int? boardid,
            int? categoryid,
            int? forumid,
            int userid,
            bool notincluded,
            bool immediateonly,
            string indentchars)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_ns_getchildren_activeuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardid;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryid;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumid;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userid;
                cmd.Parameters.Add(new NpgsqlParameter("i_notincluded", NpgsqlDbType.Boolean)).Value = notincluded;
                cmd.Parameters.Add(new NpgsqlParameter("i_immediateonly", NpgsqlDbType.Boolean)).Value = immediateonly;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable forum_listall_sorted(
            [NotNull] string connectionString, object boardId, object userId, int[] forumidExclusions)
        {
            var d = new List<int>();
            d.Add(0);
            return forum_listall_sorted(connectionString, boardId, userId, null, false, d);
        }

        //Here
        public static DataTable forum_listall_sorted(
            [NotNull] string connectionString,
            object boardId,
            object userId,
            int[] forumidExclusions,
            bool emptyFirstRow,
            List<int> startAt)
        {
            using (DataTable dataTable = forum_listall(connectionString, boardId, userId, startAt, false))
            {
                int baseForumId = 0;
                int baseCategoryId = 0;

                if (startAt.Any())
                {
                    // find the base ids..
                    foreach (DataRow dataRow in
                        dataTable.Rows.Cast<DataRow>()
                                 .Where(dataRow => Convert.ToInt32(dataRow["ForumID"]) == startAt.First(f => f > -1)))
                    {
                        baseForumId = dataRow["ParentID"] != DBNull.Value ? Convert.ToInt32(dataRow["ParentID"]) : 0;
                        baseCategoryId = Convert.ToInt32(dataRow["CategoryID"]);
                        break;
                    }
                }

                return forum_sort_list(dataTable, baseForumId, baseCategoryId, 0, forumidExclusions, emptyFirstRow);
            }
        }
       
        public static DataRow pageload(
            [NotNull] string connectionString,
            object sessionID,
            object boardId,
            object userKey,
            object ip,
            object location,
            object forumPage,
            object browser,
            object platform,
            object categoryID,
            object forumID,
            object topicID,
            object messageID,
            object isCrawler,
            object isMobileDevice,
            object donttrack)
        {
            int nTries = 0;
            while (true)
            {
                try
                {
                    // var dd = PostgreDbAccess.GetConnectionParams();
                    DataTable dt1 = null;
                    using (var cmd = PostgreDbAccess.GetCommand("pageload"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new NpgsqlParameter("i_sessionid", NpgsqlDbType.Varchar)).Value = sessionID;
                        cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                        cmd.Parameters.Add(new NpgsqlParameter("i_userkey", NpgsqlDbType.Varchar)).Value = userKey
                                                                                                           ?? DBNull
                                                                                                                  .Value;
                        cmd.Parameters.Add(new NpgsqlParameter("i_ip", NpgsqlDbType.Varchar)).Value = ip;
                        cmd.Parameters.Add(new NpgsqlParameter("i_location", NpgsqlDbType.Varchar)).Value = location;
                        cmd.Parameters.Add(new NpgsqlParameter("i_forumpage", NpgsqlDbType.Varchar)).Value = forumPage;
                        cmd.Parameters.Add(new NpgsqlParameter("i_browser", NpgsqlDbType.Varchar)).Value = browser;
                        cmd.Parameters.Add(new NpgsqlParameter("i_platform", NpgsqlDbType.Varchar)).Value = platform;
                        cmd.Parameters.Add(new NpgsqlParameter("ii_categoryid", NpgsqlDbType.Integer)).Value =
                            categoryID;
                        cmd.Parameters.Add(new NpgsqlParameter("ii_forumid", NpgsqlDbType.Integer)).Value = forumID;
                        cmd.Parameters.Add(new NpgsqlParameter("ii_topicid", NpgsqlDbType.Integer)).Value = topicID;
                        cmd.Parameters.Add(new NpgsqlParameter("ii_messageid", NpgsqlDbType.Integer)).Value = messageID;
                        cmd.Parameters.Add(new NpgsqlParameter("i_iscrawler", NpgsqlDbType.Boolean)).Value = isCrawler;
                        cmd.Parameters.Add(new NpgsqlParameter("i_ismobiledevice", NpgsqlDbType.Boolean)).Value =
                            isMobileDevice;
                        cmd.Parameters.Add(new NpgsqlParameter("i_donttrack", NpgsqlDbType.Boolean)).Value = donttrack;
                        cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                            DateTime.UtcNow;

                        dt1 = PostgreDbAccess.GetData(cmd, false, connectionString);
                        return dt1.Rows[0];

                        /*   if (dt1.Columns.Count == 0) throw new ArgumentOutOfRangeException();
                        using (var cmd1 = PostgreDbAccess.GetCommand("vaccess_combo"))
                        {
                            cmd1.CommandType = CommandType.StoredProcedure;
                            cmd1.Parameters.Add("i_userid", NpgsqlDbType.Integer).Value = dt1.Rows[0]["UserID"];
                            cmd1.Parameters.Add("i_forumid", NpgsqlDbType.Integer).Value = dt1.Rows[0]["ForumID"];
                            //We  trigger AcceptChanges() right now as we don't need to return more rows
                            return PostgreDbAccess.Current.AddValuesToDataTableFromReader(cmd1, dt1, false, true, dt1.Columns.Count).Rows[0];
                        } */


                    }
                }
                catch (NpgsqlException x)
                {
                    if (x.ErrorCode == 1205 && nTries < 3)
                    {
                        /// Transaction (Process ID XXX) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
                    }
                    else
                        throw new ApplicationException(
                            string.Format("Sql Exception with error number {0} (Tries={1})", x.Code, nTries), x);
                }
                ++nTries;
            }
        }

        

        #endregion
        #region DataSets

        public static void forum_list_sort_basic(DataTable listsource, DataTable list, int parentid, int currentLvl)
        {
            for (int i = 0; i < listsource.Rows.Count; i++)
            {
                DataRow row = listsource.Rows[i];
                if ((row["ParentID"]) == DBNull.Value) row["ParentID"] = 0;

                if ((int)row["ParentID"] == parentid)
                {
                    string sIndent = string.Empty;
                    int iIndent = Convert.ToInt32(currentLvl);
                    for (int j = 0; j < iIndent; j++) sIndent += "--";
                    row["Name"] = string.Format(" -{0} {1}", sIndent, row["Name"]);
                    list.Rows.Add(row.ItemArray);
                    forum_list_sort_basic(listsource, list, (int)row["ForumID"], currentLvl + 1);
                }
            }
        }

        /// <summary>
        /// Gets a list of categories????
        /// </summary>
        /// <param name="boardId">BoardID</param>
        /// <returns>DataSet with categories</returns>
        public static DataSet ds_forumadmin([NotNull] string connectionString, object boardId, object pageUserID, object isUserForum)
        {
            using (var connMan = new PostgreDbConnectionManager(connectionString))
            {
                using (var ds = new DataSet())
                {
                    using (var trans = connMan.OpenDBConnection(connectionString).BeginTransaction(PostgreDbAccess.IsolationLevel))
                    {
                        using (var da = new NpgsqlDataAdapter(PostgreDbAccess.GetObjectName("category_list"), connMan.DBConnection))
                        {
                            da.SelectCommand.Transaction = trans;

                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer))
                              .Value = boardId;
                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer))
                              .Value = DBNull.Value;

                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.Fill(ds, PostgreDbAccess.GetObjectName("Category"));
                            da.SelectCommand.Parameters.Clear();
                            da.SelectCommand.CommandText = PostgreDbAccess.GetObjectName("forum_list");
                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer))
                            .Value = boardId;
                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = DBNull.Value;
                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = pageUserID;
                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_isuserforum", NpgsqlDbType.Boolean)).Value = isUserForum;
                            da.Fill(ds, PostgreDbAccess.GetObjectName("ForumUnsorted"));
                            DataTable dtForumListSorted =
                                ds.Tables[PostgreDbAccess.GetObjectName("ForumUnsorted")].Clone();
                            dtForumListSorted.TableName = PostgreDbAccess.GetObjectName("Forum");
                            ds.Tables.Add(dtForumListSorted);
                            dtForumListSorted.Dispose();
                            Db.forum_list_sort_basic(
                                ds.Tables[PostgreDbAccess.GetObjectName("ForumUnsorted")],
                                ds.Tables[PostgreDbAccess.GetObjectName("Forum")],
                                0,
                                0);
                            ds.Tables.Remove(PostgreDbAccess.GetObjectName("ForumUnsorted"));
                            ds.Relations.Add(
                                "FK_Forum_Category",
                                ds.Tables[PostgreDbAccess.GetObjectName("Category")].Columns["CategoryID"],
                                ds.Tables[PostgreDbAccess.GetObjectName("Forum")].Columns["CategoryID"]);
                            trans.Commit();
                        }

                        return ds;
                    }
                }
            }
        }

        #endregion         



        public static void eventlog_create([NotNull] string connectionString, object userId, object source, object description)
        {
            eventlog_create(connectionString, userId, (object)source.GetType().ToString(), description, (object)0);
        }

        public static void eventlog_create(
            [NotNull] string connectionString, object userId, object source, object description, object type)
        {
            try
            {
                if (userId == null) userId = DBNull.Value;
                using (var cmd = PostgreDbAccess.GetCommand("eventlog_create"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;



                    cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                    cmd.Parameters.Add(new NpgsqlParameter("i_source", NpgsqlDbType.Varchar)).Value = source.ToString();
                    cmd.Parameters.Add(new NpgsqlParameter("i_description", NpgsqlDbType.Text)).Value =
                        description.ToString();
                    cmd.Parameters.Add(new NpgsqlParameter("i_type", NpgsqlDbType.Integer)).Value = type;
                    cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                        DateTime.UtcNow;

                    PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
                }
            }
            catch
            {
                // Ignore any errors in this method
            }
        }
    

        #region yaf_PollVote

        /// <summary>
        /// Checks for a vote in the database 
        /// </summary>
        /// <param name="pollGroupId">
        /// The pollGroupid.
        /// </param>
        /// <param name="userId">
        /// The userid.
        /// </param>
        /// <param name="remoteIp">
        /// The remoteip.
        /// </param>
        public static DataTable pollgroup_votecheck(
            [NotNull] string connectionString, object pollGroupId, object userId, object remoteIp)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pollgroup_votecheck"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("i_pollgroupid", pollGroupId);
                cmd.Parameters.AddWithValue("i_userid", userId);
                cmd.Parameters.AddWithValue("i_remoteip", remoteIp);
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Checks for a vote in the database
        /// </summary>
        /// <param name="choiceID">Choice of the vote</param>
        public static DataTable pollvote_check([NotNull] string connectionString, object pollid, object userid, object remoteip)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pollvote_check"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_pollid", NpgsqlDbType.Integer)).Value = pollid;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userid;
                cmd.Parameters.Add(new NpgsqlParameter("i_remoteip", NpgsqlDbType.Varchar)).Value = remoteip;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_Forum

        /// <summary>
        /// Get the list of recently logged in users.
        /// </summary>
        /// <param name="boardID">
        /// The board ID.
        /// </param>
        /// <param name="timeSinceLastLogin">
        /// The time since last login in minutes.
        /// </param>
        /// <param name="styledNicks">
        /// The styled Nicks.
        /// </param>
        /// <returns>
        /// The list of users in Datatable format.
        /// </returns>
        public static DataTable recent_users(
            [NotNull] string connectionString, object boardID, int timeSinceLastLogin, object styledNicks)
        {
            using (var cmd = PostgreDbAccess.GetCommand("recent_users"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardID;
                cmd.Parameters.Add(new NpgsqlParameter("i_timesincelastlogin", NpgsqlDbType.Integer)).Value =
                    timeSinceLastLogin;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = styledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// List of categories accessible for an active user
        /// </summary>
        /// <param name="boardId">The board id.</param>
        /// <param name="userId">The user Id.</param>
        /// <returns>A <see cref="T:System.Data.DataTable"/> of categories.</returns>
        public static DataTable forum_categoryaccess_activeuser([NotNull] string connectionString, object boardId, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_categoryaccess_activeuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        //ABOT NEW 16.04.04
        /// <summary>
        /// Deletes attachments out of a entire forum
        /// </summary>
        /// <param name="ForumID">ID of forum to delete all attachemnts out of</param>
        private static void forum_deleteAttachments([NotNull] string connectionString, object forumID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_listtopics"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;

                using (var dt = PostgreDbAccess.GetData(cmd, connectionString))
                {
                    foreach (DataRow row in
                        dt.Rows.Cast<DataRow>().Where(row => row != null && row["TopicID"] != DBNull.Value))
                    {
                        topic_delete(connectionString, row["TopicID"], true);
                    }
                }
            }
        }

        //END ABOT NEW 16.04.04
        //ABOT CHANGE 16.04.04
        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="ForumID">forum to delete</param>
        /// <returns>bool to indicate that forum has been deleted</returns>
        public static bool forum_delete([NotNull] string connectionString, object forumID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_listSubForums"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;

                if (!(PostgreDbAccess.ExecuteScalar(cmd, connectionString) is DBNull)) return false;
                else
                {
                    forum_deleteAttachments(connectionString, forumID);
                    using (var cmd_new = PostgreDbAccess.GetCommand("forum_delete"))
                    {
                        cmd_new.CommandType = CommandType.StoredProcedure;
                        cmd_new.CommandTimeout = 99999;
                        cmd_new.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;

                        PostgreDbAccess.ExecuteNonQuery(cmd_new, connectionString);
                    }

                    forum_ns_recreate(connectionString);
                    return true;
                }
            }
        }

        /// <summary>
        /// The forum_move.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="forumOldID">
        /// The forum old id.
        /// </param>
        /// <param name="forumNewID">
        /// The forum new id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool forum_move([NotNull] string connectionString, [NotNull] object forumOldID, [NotNull] object forumNewID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_listSubForums"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumOldID;

                if (!(PostgreDbAccess.ExecuteScalar(cmd, connectionString) is DBNull))
                {
                    return false;
                }

                using (var cmd_new = PostgreDbAccess.GetCommand("forum_move"))
                {
                    cmd_new.CommandType = CommandType.StoredProcedure;
                    cmd_new.CommandTimeout = 99999;
                    cmd.Parameters.Add(new NpgsqlParameter("i_forumoldid", NpgsqlDbType.Integer)).Value = forumOldID;
                    cmd.Parameters.Add(new NpgsqlParameter("i_forumnewid", NpgsqlDbType.Integer)).Value = forumNewID;

                    PostgreDbAccess.ExecuteNonQuery(cmd_new, connectionString);
                }

                return true;
            }
        }

        /// <summary>
        /// Lists all moderated forums for a user
        /// </summary>
        /// <param name="boardId">board if of moderators</param>
        /// <param name="userId">user id</param>
        /// <returns>DataTable of moderated forums</returns>
        public static DataTable forum_listallMyModerated([NotNull] string connectionString, object boardId, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_listallmymoderated"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        //END ABOT NEW 16.04.04
        /// <summary>
        /// Gets a list of topics in a forum
        /// </summary>
        /// <param name="boardId">boardId</param>
        /// <param name="ForumID">forumID</param>
        /// <returns>DataTable with list of topics from a forum</returns>
        public static DataTable forum_list([NotNull] string connectionString, object boardId, object forumID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_list"))
            {
                if (forumID == null)
                {
                    forumID = DBNull.Value;
                }
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_isuserforum", NpgsqlDbType.Boolean)).Value = false;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable forum_byflags(string connectionString, object forumId, object flags)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_byflags"))
            {

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumId
                                                                                                   ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_flags", NpgsqlDbType.Integer)).Value = flags;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static
            DataTable forum_byuserlist([NotNull] string connectionString, object boardId, object forumID, object userId, object isUserForum)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_byuserlist"))
            {
                if (forumID == null)
                {
                    forumID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_isuserforum", NpgsqlDbType.Boolean)).Value = isUserForum;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Gets a max id of forums.
        /// </summary>
        /// <param name="boardID">
        /// boardID
        /// </param>
        /// <returns>
        /// Max forum id for a board
        /// </returns>
        public static int forum_maxid([NotNull] string connectionString, [NotNull] object boardID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_maxid"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardID;
                return Convert.ToInt32(PostgreDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        /// <summary>
        /// Lists all forums accessible to a user
        /// </summary>
        /// <param name="boardId">BoardID</param>
        /// <param name="userId">ID of user</param>
        /// <param name="startAt">startAt ID</param>
        /// <returns>DataTable of all accessible forums</returns>
        public static DataTable forum_listall(
            [NotNull] string connectionString, object boardId, object userId, object startAt, bool returnAll)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_listall"))
            {
                if (startAt == null)
                {
                    startAt = 0;
                }
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_root", NpgsqlDbType.Integer)).Value = startAt;
                cmd.Parameters.Add(new NpgsqlParameter("i_returnall", NpgsqlDbType.Boolean)).Value = returnAll;
                var dd = PostgreDbAccess.GetData(cmd, connectionString);
                return dd;
            }
        }


        public static IEnumerable<TypedForumListAll> ForumListAll([NotNull] string connectionString, int boardId, int userId)
        {
            return
                forum_listall(connectionString, boardId, userId, 0, false)
                    .AsEnumerable()
                    .Select(r => new TypedForumListAll(r));
        }

        public static IEnumerable<TypedForumListAll> ForumListAll(
            [NotNull] string connectionString, int boardId, int userId, List<int> startForumId)
        {
            var allForums = ForumListAll(connectionString, boardId, userId);

            var forumIds = new List<int>();
            var tempForumIds = new List<int>();

            int ff = 0;
            if (startForumId.Any())
            {
                ff = startForumId.First(s => s > -1);
            }
            forumIds.Add(ff);
            tempForumIds.Add(ff);

            IEnumerable<TypedForumListAll> typedForumListAlls = allForums as List<TypedForumListAll>
                                                                ?? allForums.ToList();
            while (true)
            {
                var ids = tempForumIds;
                var temp = typedForumListAlls.Where(f => ids.Contains(f.ParentID ?? 0));

                var forumListAlls = temp as List<TypedForumListAll> ?? temp.ToList();
                if (!forumListAlls.Any())
                {
                    break;
                }

                // replace temp forum ids with these..
                tempForumIds = forumListAlls.Select(f => f.ForumID ?? 0).Distinct().ToList();

                // add them..
                forumIds.AddRange(tempForumIds);
            }

            // return filtered forums..
            return typedForumListAlls.Where(f => forumIds.Contains(f.ForumID ?? 0)).Distinct();
        }


        /// <summary>
        /// Lists forums very simply (for URL rewriting)
        /// </summary>
        /// <param name="StartID"></param>
        /// <param name="Limit"></param>
        /// <returns></returns>
        public static DataTable forum_simplelist([NotNull] string connectionString, int startID, int limit)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_simplelist"))
            {
                if (startID <= 0)
                {
                    startID = 0;
                }
                if (limit <= 0)
                {
                    limit = 500;
                }
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_startid", NpgsqlDbType.Integer)).Value = startID;
                cmd.Parameters.Add(new NpgsqlParameter("i_limit", NpgsqlDbType.Integer)).Value = limit;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void forum_sort_list_recursive(
            DataTable listSource, DataTable listDestination, int parentID, int categoryID, int currentIndent)
        {
            foreach (DataRow row in listSource.Rows)
            {
                // see if this is a root-forum
                if (row["ParentID"] == DBNull.Value) row["ParentID"] = 0;

                if ((int)row["ParentID"] != parentID) continue;

                DataRow newRow;
                if ((int)row["CategoryID"] != categoryID)
                {
                    categoryID = (int)row["CategoryID"];

                    newRow = listDestination.NewRow();
                    newRow["ForumID"] = -categoryID; // Ederon : 9/4/2007
                    newRow["Title"] = string.Format("{0}", row["Category"]);
                    newRow["CanHavePersForums"] = row["CanHavePersForums"].ToType<bool>();
                    listDestination.Rows.Add(newRow);
                }

                string sIndent = string.Empty;

                for (int j = 0; j < currentIndent; j++) sIndent += "--";

                // import the row into the destination
                newRow = listDestination.NewRow();

                newRow["ForumID"] = row["ForumID"];
                newRow["Title"] = string.Format(" -{0} {1}", sIndent, row["Forum"]);
                newRow["CanHavePersForums"] = row["CanHavePersForums"].ToType<bool>();

                listDestination.Rows.Add(newRow);

                // recurse through the list..
                forum_sort_list_recursive(
                    listSource, listDestination, (int)row["ForumID"], categoryID, currentIndent + 1);
            }
        }

        public static DataTable forum_sort_list(
            DataTable listSource,
            int parentID,
            int categoryID,
            int startingIndent,
            int[] forumidExclusions,
            bool emptyFirstRow)
        {
            var listDestination = new DataTable { TableName = "forum_sort_list" };
            listDestination.Columns.Add("ForumID", typeof(String));
            listDestination.Columns.Add("Title", typeof(String));
            listDestination.Columns.Add("CanHavePersForums", typeof(bool));

            if (emptyFirstRow)
            {
                DataRow blankRow = listDestination.NewRow();
                blankRow["ForumID"] = string.Empty;
                blankRow["Title"] = string.Empty;
                blankRow["CanHavePersForums"] = false;
                listDestination.Rows.Add(blankRow);
            }
            // filter the forum list -- not sure if this code actually works
            DataView dv = listSource.DefaultView;

            if (forumidExclusions != null && forumidExclusions.Length > 0)
            {
                string strExclusions = string.Empty;
                bool bFirst = true;

                foreach (int forumId in forumidExclusions)
                {
                    if (bFirst) bFirst = false;
                    else strExclusions += ",";

                    strExclusions += forumId.ToString();
                }

                dv.RowFilter = string.Format("ForumID NOT IN ({0})", strExclusions);
                dv.ApplyDefaultSort = true;
            }

            forum_sort_list_recursive(dv.ToTable(), listDestination, parentID, categoryID, startingIndent);

            return listDestination;
        }

        /// <summary>
        /// Lists all forums within a given subcategory
        /// </summary>
        /// <param name="boardId">BoardID</param>
        /// <param name="CategoryID">CategoryID</param>
        /// <param name="EmptyFirstRow">EmptyFirstRow</param>
        /// <returns>DataTable with list</returns>
        public static DataTable forum_listall_fromCat(
            [NotNull] string connectionString, object boardId, object categoryID, bool emptyFirstRow, bool allowUserForumsOnly)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_listall_fromCat"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryID;
                cmd.Parameters.Add(new NpgsqlParameter("i_allowuseforumsonly", NpgsqlDbType.Boolean)).Value = allowUserForumsOnly;
                int intCategoryId = Convert.ToInt32(categoryID.ToString());

                using (DataTable dt = PostgreDbAccess.GetData(cmd, connectionString))
                {
                    return Db.forum_sort_list(dt, 0, intCategoryId, 0, null, emptyFirstRow);
                }
            }
        }

        /// <summary>
        /// Sorry no idea what this does
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public static DataTable forum_listpath([NotNull] string connectionString, object forumID)
        {
            if (!Config.LargeForumTree)
            {

                using (var cmd = PostgreDbAccess.GetCommand("forum_listpath"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;

                    return PostgreDbAccess.GetData(cmd, connectionString);
                }
            }
            else
            {
                using (var cmd = PostgreDbAccess.GetCommand("forum_ns_listpath"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;

                    return PostgreDbAccess.GetData(cmd, connectionString);
                }
            }
        }

        /// <summary>
        /// The forum_listread.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="parentId">
        /// The parent id.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use styled nicks.
        /// </param>
        /// <param name="findLastRead">
        /// The find last read.
        /// </param>
        /// <param name="showCommonForums">
        /// The show common forums.
        /// </param>
        /// <param name="showPersonalForums">
        /// The show personal forums.
        /// </param>
        /// <param name="forumCreatedByUserId">
        /// The forum created by user id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable forum_listread(
            [NotNull] string connectionString,
            object boardId,
            object userId,
            object categoryId,
            object parentId,
            object useStyledNicks,
            [CanBeNull] bool findLastRead, 
            [NotNull] bool showCommonForums, 
            [NotNull]bool showPersonalForums, 
            [CanBeNull] int? forumCreatedByUserId)
        {
            if (!Config.LargeForumTree)
            {
                using (var cmd = PostgreDbAccess.GetCommand("forum_listread"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("i_boardid", NpgsqlDbType.Integer).Value = boardId;
                    cmd.Parameters.Add("i_userid", NpgsqlDbType.Integer).Value = userId;
                    cmd.Parameters.Add("i_categoryid", NpgsqlDbType.Integer).Value = categoryId ?? DBNull.Value;
                    cmd.Parameters.Add("i_parentid", NpgsqlDbType.Integer).Value = parentId ?? DBNull.Value;
                    cmd.Parameters.Add("i_stylednicks", NpgsqlDbType.Boolean).Value = useStyledNicks;
                    cmd.Parameters.Add("i_findlastunread", NpgsqlDbType.Boolean).Value = findLastRead;
                    cmd.Parameters.Add("i_showcommonforums", NpgsqlDbType.Boolean).Value = showCommonForums;
                    cmd.Parameters.Add("i_showpersonalforums", NpgsqlDbType.Boolean).Value = showPersonalForums;
                    cmd.Parameters.Add("i_forumcreatedbyuserid", NpgsqlDbType.Integer).Value = forumCreatedByUserId;
                    cmd.Parameters.Add("i_UTCTIMESTAMP", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;
                    
                    return PostgreDbAccess.GetData(cmd, false, connectionString);
                }
            }
            using (var cmd = PostgreDbAccess.GetCommand("forum_ns_listread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_boardid", NpgsqlDbType.Integer).Value = boardId;
                cmd.Parameters.Add("i_userid", NpgsqlDbType.Integer).Value = userId;
                cmd.Parameters.Add("i_categoryid", NpgsqlDbType.Integer).Value = categoryId;
                cmd.Parameters.Add("i_parentid", NpgsqlDbType.Integer).Value = parentId;
                cmd.Parameters.Add("i_stylednicks", NpgsqlDbType.Boolean).Value = useStyledNicks;
                cmd.Parameters.Add("i_findlastunread", NpgsqlDbType.Boolean).Value = findLastRead;
                cmd.Parameters.Add("i_ShowCommonForums", NpgsqlDbType.Boolean).Value = showCommonForums;
                cmd.Parameters.Add("i_ShowPersonalForums", NpgsqlDbType.Boolean).Value = showPersonalForums;
                cmd.Parameters.Add("i_ForumCreatedByUserId", NpgsqlDbType.Integer).Value = forumCreatedByUserId;
                cmd.Parameters.Add("i_UTCTIMESTAMP", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;
                  
                return PostgreDbAccess.GetData(cmd, false, connectionString);
            }
        }

        /// <summary>
        /// The forum_listreadpersonal.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="parentId">
        /// The parent id.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use styled nicks.
        /// </param>
        /// <param name="findLastRead">
        /// The find last read.
        /// </param>
        /// <param name="showCommonForums">
        /// The show common forums.
        /// </param>
        /// <param name="showPersonalForums">
        /// The show personal forums.
        /// </param>
        /// <param name="forumCreatedByUserId">
        /// The forum created by user id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable forum_listreadpersonal(
           [NotNull] string connectionString,
           object boardId,
           object userId,
           object categoryId,
           object parentId,
           object useStyledNicks,
           [CanBeNull] bool findLastRead,
           [NotNull] bool showCommonForums,
           [NotNull]bool showPersonalForums,
           [CanBeNull] int? forumCreatedByUserId)
        {
            if (!Config.LargeForumTree)
            {
                using (var cmd = PostgreDbAccess.GetCommand("forum_listreadpersonal"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("i_boardid", NpgsqlDbType.Integer).Value = boardId;
                    cmd.Parameters.Add("i_userid", NpgsqlDbType.Integer).Value = userId;
                    cmd.Parameters.Add("i_categoryid", NpgsqlDbType.Integer).Value = categoryId ?? DBNull.Value;
                    cmd.Parameters.Add("i_parentid", NpgsqlDbType.Integer).Value = parentId ?? DBNull.Value;
                    cmd.Parameters.Add("i_stylednicks", NpgsqlDbType.Boolean).Value = useStyledNicks;
                    cmd.Parameters.Add("i_findlastunread", NpgsqlDbType.Boolean).Value = findLastRead;
                    cmd.Parameters.Add("i_showcommonforums", NpgsqlDbType.Boolean).Value = showCommonForums;
                    cmd.Parameters.Add("i_showpersonalforums", NpgsqlDbType.Boolean).Value = showPersonalForums;
                    cmd.Parameters.Add("i_forumcreatedbyuserid", NpgsqlDbType.Integer).Value = forumCreatedByUserId;
                    cmd.Parameters.Add("i_UTCTIMESTAMP", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;

                    return PostgreDbAccess.GetData(cmd, false, connectionString);
                }
            }

            using (var cmd = PostgreDbAccess.GetCommand("forum_ns_listreadpersonal"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_boardid", NpgsqlDbType.Integer).Value = boardId;
                cmd.Parameters.Add("i_userid", NpgsqlDbType.Integer).Value = userId;
                cmd.Parameters.Add("i_categoryid", NpgsqlDbType.Integer).Value = categoryId;
                cmd.Parameters.Add("i_parentid", NpgsqlDbType.Integer).Value = parentId;
                cmd.Parameters.Add("i_stylednicks", NpgsqlDbType.Boolean).Value = useStyledNicks;
                cmd.Parameters.Add("i_findlastunread", NpgsqlDbType.Boolean).Value = findLastRead;
                cmd.Parameters.Add("i_ShowCommonForums", NpgsqlDbType.Boolean).Value = showCommonForums;
                cmd.Parameters.Add("i_ShowPersonalForums", NpgsqlDbType.Boolean).Value = showPersonalForums;
                cmd.Parameters.Add("i_ForumCreatedByUserId", NpgsqlDbType.Integer).Value = forumCreatedByUserId;
                cmd.Parameters.Add("i_UTCTIMESTAMP", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, false, connectionString);
            }
        }

        /// <summary>
        /// Return admin view of Categories with Forums/Subforums ordered accordingly.
        /// </summary>
        /// <param name="boardId">BoardID</param>
        /// <param name="userId">UserID</param>
        /// <returns>DataSet with categories</returns>
        public static DataSet forum_moderatelist([NotNull] string connectionString, object userId, object boardId)
        {
            using (var connMan = new PostgreDbConnectionManager(connectionString))
            {
                using (var ds = new DataSet())
                {
                    using (var da = new NpgsqlDataAdapter(PostgreDbAccess.GetObjectName("category_list"), connMan.OpenDBConnection(connectionString)))
                    {
                        using (var trans = da.SelectCommand.Connection.BeginTransaction(PostgreDbAccess.IsolationLevel))
                        {
                            da.SelectCommand.Transaction = trans;
                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer))
                              .Value = boardId;
                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer))
                              .Value = DBNull.Value;

                            da.SelectCommand.CommandType = CommandType.StoredProcedure;


                            da.Fill(ds, PostgreDbAccess.GetObjectName("Category"));
                            da.SelectCommand.CommandText = PostgreDbAccess.GetObjectName("forum_moderatelist");
                            da.SelectCommand.Parameters.Clear();
                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer))
                              .Value = boardId;
                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value
                                = userId;
                            da.SelectCommand.Parameters.Add(new NpgsqlParameter("i_isuserforum", NpgsqlDbType.Boolean)).Value
                               = false;
                            da.Fill(ds, PostgreDbAccess.GetObjectName("ForumUnsorted"));
                            DataTable dtForumListSorted =
                                ds.Tables[PostgreDbAccess.GetObjectName("ForumUnsorted")].Clone();
                            dtForumListSorted.TableName = PostgreDbAccess.GetObjectName("Forum");
                            ds.Tables.Add(dtForumListSorted);
                            dtForumListSorted.Dispose();
                            Db.forum_list_sort_basic(
                                ds.Tables[PostgreDbAccess.GetObjectName("ForumUnsorted")],
                                ds.Tables[PostgreDbAccess.GetObjectName("Forum")],
                                0,
                                0);
                            ds.Tables.Remove(PostgreDbAccess.GetObjectName("ForumUnsorted"));

                            // vzrus: Remove here all forums with no reports. Would be better to do it in query..
                            // Array to write categories numbers
                            var categories = new int[ds.Tables[PostgreDbAccess.GetObjectName("Forum")].Rows.Count];
                            int cntr = 0;

                            // We should make it before too as the colection was changed
                            ds.Tables[PostgreDbAccess.GetObjectName("Forum")].AcceptChanges();
                            foreach (DataRow dr in ds.Tables[PostgreDbAccess.GetObjectName("Forum")].Rows)
                            {
                                categories[cntr] = Convert.ToInt32(dr["CategoryID"]);
                                if (Convert.ToInt32(dr["ReportedCount"]) == 0
                                    && Convert.ToInt32(dr["MessageCount"]) == 0)
                                {
                                    dr.Delete();
                                    categories[cntr] = 0;
                                }

                                cntr++;
                            }

                            ds.Tables[PostgreDbAccess.GetObjectName("Forum")].AcceptChanges();

                            foreach (DataRow dr in ds.Tables[PostgreDbAccess.GetObjectName("Category")].Rows)
                            {
                                bool deleteMe = true;
                                for (int i = 0; i < categories.Length; i++)
                                {
                                    // We check here if the Category is missing in the array where 
                                    // we've written categories number for each forum
                                    if (categories[i] == Convert.ToInt32(dr["CategoryID"]))
                                    {
                                        deleteMe = false;
                                    }
                                }

                                if (deleteMe)
                                {
                                    dr.Delete();
                                }
                            }

                            ds.Tables[PostgreDbAccess.GetObjectName("Category")].AcceptChanges();

                            ds.Relations.Add(
                                "FK_Forum_Category",
                                ds.Tables[PostgreDbAccess.GetObjectName("Category")].Columns["CategoryID"],
                                ds.Tables[PostgreDbAccess.GetObjectName("Forum")].Columns["CategoryID"]);
                            trans.Commit();
                        }

                        return ds;
                    }
                }
            }
        }

        public static DataTable forum_moderators([NotNull] string connectionString, bool useStyledNicks)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_moderators"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The moderators_team_list
        /// </summary>
        /// <param name="useStyledNicks">
        /// The use Styled Nicks.
        /// </param>
        /// <returns>
        ///  Returns Data Table with all Mods
        /// </returns>
        public static DataTable moderators_team_list([NotNull] string connectionString, bool useStyledNicks)
        {
            using (var cmd = PostgreDbAccess.GetCommand("moderators_team_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }
      
        public static long forum_save(
            [NotNull] string connectionString,
            object forumID,
            object categoryID,
            object parentID,
            object name,
            object description,
            object sortOrder,
            object locked,
            object hidden,
            object isTest,
            object moderated,
            object accessMaskID,
            object remoteURL,
            object themeURL,
            object imageURL,
            object styles,
            bool dummy, 
            object userId,
            bool isUserForum,
            bool canhavepersforums)
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_save"))
            {
                int sortOrderOut = 0;
                bool result = int.TryParse(sortOrder.ToString(), out sortOrderOut);
                if (result)
                {
                    if (sortOrderOut >= 255)
                    {
                        sortOrderOut = 0;
                    }
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryID;
                cmd.Parameters.Add(new NpgsqlParameter("i_parentid", NpgsqlDbType.Integer)).Value = parentID ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_name", NpgsqlDbType.Varchar)).Value = name;
                cmd.Parameters.Add(new NpgsqlParameter("i_description", NpgsqlDbType.Varchar)).Value = description;
                cmd.Parameters.Add(new NpgsqlParameter("i_sortorder", NpgsqlDbType.Smallint)).Value = sortOrderOut;
                cmd.Parameters.Add(new NpgsqlParameter("i_locked", NpgsqlDbType.Boolean)).Value = locked;
                cmd.Parameters.Add(new NpgsqlParameter("i_hidden", NpgsqlDbType.Boolean)).Value = hidden;
                cmd.Parameters.Add(new NpgsqlParameter("i_istest", NpgsqlDbType.Boolean)).Value = isTest;
                cmd.Parameters.Add(new NpgsqlParameter("i_moderated", NpgsqlDbType.Boolean)).Value = moderated;
                cmd.Parameters.Add(new NpgsqlParameter("i_remoteurl", NpgsqlDbType.Varchar)).Value = remoteURL ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_themeurl", NpgsqlDbType.Varchar)).Value = themeURL ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_imageurl", NpgsqlDbType.Varchar)).Value = imageURL ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_styles", NpgsqlDbType.Varchar)).Value = styles;
                cmd.Parameters.Add(new NpgsqlParameter("i_accessmaskid", NpgsqlDbType.Integer)).Value = accessMaskID ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_isuserforum", NpgsqlDbType.Boolean)).Value = isUserForum;
                cmd.Parameters.Add(new NpgsqlParameter("i_canhavepersforums", NpgsqlDbType.Boolean)).Value = canhavepersforums;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                   DateTime.UtcNow;
                String resultop = PostgreDbAccess.ExecuteScalar(cmd, connectionString).ToString();
                if (String.IsNullOrEmpty(resultop))
                {
                    return 0;
                }
                else
                {
                    forum_ns_recreate(connectionString);
                    return long.Parse(resultop);
                }
            }
        }

        /// <summary>
        /// The method returns an integer value for a  found parent forum 
        /// if a forum is a parent of an existing child to avoid circular dependency
        /// while creating a new forum
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="parentID"></param>
        /// <returns>Integer value for a found dependency</returns>
        public static int forum_save_parentschecker([NotNull] string connectionString, object forumID, object parentID)
        {
            using (
                var cmd =
                    PostgreDbAccess.GetCommand(
                        "SELECT " + PostgreDbAccess.GetObjectName("forum_save_parentschecker") + "(@ForumID,@ParentID)",
                        true))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new NpgsqlParameter("@ForumID", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("@ParentID", NpgsqlDbType.Integer)).Value = parentID;
                return Convert.ToInt32(PostgreDbAccess.ExecuteScalar(cmd, connectionString));
            }

        }

        /// <summary>
        /// Recreate tree.
        /// </summary>
        public static void forum_ns_recreate([NotNull] string connectionString )
        {
            using (var cmd = PostgreDbAccess.GetCommand("forum_ns_recreate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }


        #endregion     

        #region yaf_Message

        public static DataTable post_list(
            [NotNull] string connectionString,
            object topicId,
            object currentUserID,
            object authorUserID,
            object updateViewCount,
            bool showDeleted,
            bool styledNicks,
            bool showReputation,
            DateTime sincePostedDate,
            DateTime toPostedDate,
            DateTime sinceEditedDate,
            DateTime toEditedDate,
            int pageIndex,
            int pageSize,
            int sortPosted,
            int sortEdited,
            int sortPosition,
            bool showThanks,
            int messagePosition,
            int messageId, 
            DateTime lastRead)
        {
            using (var cmd = PostgreDbAccess.GetCommand("post_list"))
            {
                if (updateViewCount == null)
                {
                    updateViewCount = 1;
                }
                //if (showDeleted != false || showDeleted != true) { showDeleted = true; }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicId;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = currentUserID;
                cmd.Parameters.Add(new NpgsqlParameter("i_authoruserid", NpgsqlDbType.Integer)).Value = authorUserID;
                cmd.Parameters.Add(new NpgsqlParameter("i_updateviewcount", NpgsqlDbType.Smallint)).Value =
                    updateViewCount;
                cmd.Parameters.Add(new NpgsqlParameter("i_showdeleted", NpgsqlDbType.Boolean)).Value = showDeleted;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = styledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_showreputation", NpgsqlDbType.Boolean)).Value = showReputation;
                cmd.Parameters.Add(new NpgsqlParameter("i_sinceposteddate", NpgsqlDbType.Timestamp)).Value =
                    sincePostedDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_toposteddate", NpgsqlDbType.Timestamp)).Value = toPostedDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_sinceediteddate", NpgsqlDbType.Timestamp)).Value =
                    sinceEditedDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_toediteddate", NpgsqlDbType.Timestamp)).Value = toEditedDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize;
                cmd.Parameters.Add(new NpgsqlParameter("i_sortposted", NpgsqlDbType.Integer)).Value = sortPosted;
                cmd.Parameters.Add(new NpgsqlParameter("i_sortedited", NpgsqlDbType.Integer)).Value = sortEdited;
                cmd.Parameters.Add(new NpgsqlParameter("i_sortposition", NpgsqlDbType.Integer)).Value = sortPosition;
                cmd.Parameters.Add(new NpgsqlParameter("i_showthanks", NpgsqlDbType.Boolean)).Value = showThanks;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageposition", NpgsqlDbType.Integer)).Value =
                    messagePosition;
                cmd.Parameters.Add("i_messsageid", NpgsqlDbType.Integer).Value = messageId;
                cmd.Parameters.Add("i_lastread", NpgsqlDbType.Timestamp).Value = lastRead;
                cmd.Parameters.Add("i_utctimestamp", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);

            }
        }

        public static DataTable post_list_reverse10([NotNull] string connectionString, object topicID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("post_list_reverse10"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }


        /// <summary>
        /// Gets all the post by a user.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="pageUserID">
        /// The page user id.
        /// </param>
        /// <param name="topCount">
        /// Top count to return. Null is all.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable post_alluser(
            [NotNull] string connectionString, object boardid, object userid, object pageUserID, object topCount)
        {
            using (var cmd = PostgreDbAccess.GetCommand("post_alluser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardid;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userid;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserID;
                cmd.Parameters.Add(new NpgsqlParameter("i_topcount", NpgsqlDbType.Integer)).Value = topCount;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }


        // gets list of replies to message
        public static DataTable message_getRepliesList([NotNull] string connectionString, object messageID)
        {
            DataTable list = new DataTable();

            list.Columns.Add("MessageID", typeof(int));
            list.Columns.Add("Posted", typeof(DateTime));
            list.Columns.Add("Subject", typeof(string));
            list.Columns.Add("Message", typeof(string));
            list.Columns.Add("UserID", typeof(int));
            list.Columns.Add("Flags", typeof(int));
            list.Columns.Add("UserName", typeof(string));
            list.Columns.Add("Signature", typeof(string));

            using (var cmd = PostgreDbAccess.GetCommand("message_reply_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;

                DataTable dtr = PostgreDbAccess.GetData(cmd, connectionString);
                for (int i = 0; i < dtr.Rows.Count; i++)
                {
                    DataRow newRow = list.NewRow();
                    DataRow row = dtr.Rows[i];
                    newRow["MessageID"] = row["MessageID"];
                    newRow["Posted"] = row["Posted"];
                    newRow["Subject"] = row["Subject"];
                    newRow["Message"] = row["Message"];
                    newRow["UserID"] = row["UserID"];
                    newRow["Flags"] = row["Flags"];
                    newRow["UserName"] = row["UserName"];
                    newRow["Signature"] = row["Signature"];
                    list.Rows.Add(newRow);
                    message_getRepliesList_populate(connectionString, dtr, list, (int)row["MessageId"]);
                }
                return list;
            }
        }

        // gets list of nested replies to message
        private static void message_getRepliesList_populate(
            [NotNull] string connectionString, DataTable listsource, DataTable list, int messageID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_reply_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer));
                cmd.Parameters[0].Value = messageID;


                DataTable dtr = PostgreDbAccess.GetData(cmd, connectionString);

                for (int i = 0; i < dtr.Rows.Count; i++)
                {
                    DataRow row = dtr.Rows[i];
                    DataRow newRow = list.NewRow();
                    newRow["MessageID"] = row["MessageID"];
                    newRow["Posted"] = row["Posted"];
                    newRow["Subject"] = row["Subject"];
                    newRow["Message"] = row["Message"];
                    newRow["UserID"] = row["UserID"];
                    newRow["Flags"] = row["Flags"];
                    newRow["UserName"] = row["UserName"];
                    newRow["Signature"] = row["Signature"];
                    list.Rows.Add(newRow);
                    message_getRepliesList_populate(connectionString, dtr, list, (int)row["MessageId"]);
                }
            }

        }

        //creates new topic, using some parameters from message itself
        public static long topic_create_by_message(
            [NotNull] string connectionString, object messageID, object forumId, object newTopicSubj)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_create_by_message"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumId;
                cmd.Parameters.Add(new NpgsqlParameter("i_subject", NpgsqlDbType.Varchar)).Value = newTopicSubj;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return Convert.ToInt32(PostgreDbAccess.ExecuteScalar(cmd, connectionString));
                //return long.Parse(dt.Rows[0]["TopicID"].ToString());
            }
        }

        /// <summary>
        /// The message_list.
        /// </summary>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <returns>
        /// </returns>
        public static IEnumerable<TypedMessageList> MessageList([NotNull] string connectionString, int messageID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;

                var dd = PostgreDbAccess.GetData(cmd, connectionString)
                                      .AsEnumerable()
                                      .Select(t => new TypedMessageList(t));
                return dd;

            }
        }

        public static void message_delete(
            [NotNull] string connectionString,
            object messageID,
            bool isModeratorChanged,
            string deleteReason,
            int isDeleteAction,
            bool DeleteLinked)
        {
            message_delete(
                connectionString, messageID, isModeratorChanged, deleteReason, isDeleteAction, DeleteLinked, false);
        }

        public static void message_delete(
            [NotNull] string connectionString,
            object messageID,
            bool isModeratorChanged,
            string deleteReason,
            int isDeleteAction,
            bool DeleteLinked,
            bool eraseMessage)
        {
            message_deleteRecursively(
                connectionString,
                messageID,
                isModeratorChanged,
                deleteReason,
                isDeleteAction,
                DeleteLinked,
                false,
                eraseMessage);
        }

        // <summary> Retrieve all reported messages with the correct forumID argument. </summary>
        public static DataTable message_listreported([NotNull] string connectionString, object forumID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_listreported"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Here we get reporters list for a reported message
        /// </summary>       
        /// <param name="MessageID">Should not be NULL</param>
        /// <returns>Returns reporters DataTable for a reported message.</returns>
        public static DataTable message_listreporters([NotNull] string connectionString, int messageID)
        {

            return message_listreporters(connectionString, messageID, null);

        }

        public static DataTable message_listreporters([NotNull] string connectionString, int messageID, object userID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_listreporters"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        // <summary> Save reported message back to the database. </summary>
        public static void message_report(
            [NotNull] string connectionString, object messageID, object userId, object reportedDateTime, object reportText)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_report"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_reporterid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_reporteddate", NpgsqlDbType.Timestamp)).Value =
                    reportedDateTime;
                cmd.Parameters.Add(new NpgsqlParameter("i_reporttext", NpgsqlDbType.Varchar)).Value = reportText;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        // <summary> Copy current Message text over reported Message text. </summary>
        public static void message_reportcopyover([NotNull] string connectionString, object messageID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_reportcopyover"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        // <summary> Copy current Message text over reported Message text. </summary>
        public static void message_reportresolve(
            [NotNull] string connectionString, object messageFlag, object messageID, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_reportresolve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageflag", NpgsqlDbType.Integer)).Value = messageFlag;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        //BAI ADDED 30.01.2004
        // <summary> Delete message and all subsequent releated messages to that ID </summary>
        private static void message_deleteRecursively(
            [NotNull] string connectionString,
            object messageID,
            bool isModeratorChanged,
            string deleteReason,
            int isDeleteAction,
            bool DeleteLinked,
            bool isLinked)
        {
            message_deleteRecursively(
                connectionString,
                messageID,
                isModeratorChanged,
                deleteReason,
                isDeleteAction,
                DeleteLinked,
                isLinked,
                false);
        }

        private static void message_deleteRecursively(
            [NotNull] string connectionString,
            object messageID,
            bool isModeratorChanged,
            string deleteReason,
            int isDeleteAction,
            bool DeleteLinked,
            bool isLinked,
            bool eraseMessages)
        {
            bool UseFileTable = GetBooleanRegistryValue(connectionString, "UseFileTable");


            if (DeleteLinked)
            {
                //Delete replies
                using (var cmd = PostgreDbAccess.GetCommand("message_getReplies"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;

                    DataTable tbReplies = PostgreDbAccess.GetData(cmd, connectionString);

                    foreach (DataRow row in tbReplies.Rows)
                        message_deleteRecursively(
                            connectionString,
                            row["MessageID"],
                            isModeratorChanged,
                            deleteReason,
                            isDeleteAction,
                            DeleteLinked,
                            true,
                            eraseMessages);
                }
            }

            // If the files are actually saved in the Hard Drive.
            if (!UseFileTable)
            {
                using (var cmd = PostgreDbAccess.GetCommand("attachment_list"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                    cmd.Parameters.Add(new NpgsqlParameter("i_attachmentid", NpgsqlDbType.Integer)).Value = null;
                    cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = null;
                    cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = 0;
                    cmd.Parameters.Add(new NpgsqlParameter("i_pageisize", NpgsqlDbType.Integer)).Value = 1000000;
                    DataTable tbAttachments = PostgreDbAccess.GetData(cmd, connectionString);
                    string uploadDir =
                        HostingEnvironment.MapPath(
                            String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));


                    foreach (DataRow row in tbAttachments.Rows)
                    {
                        try
                        {
                            string fileName = String.Format(
                                "{0}/{1}.{2}.yafupload", uploadDir, messageID, row["FileName"]);

                            if (File.Exists(fileName))
                            {
                                File.Delete(fileName);
                            }
                        }
                        catch
                        {
                            // error deleting that file.. 
                        }
                    }
                }
            }

            // Ederon : erase message for good
            if (eraseMessages)
            {
                using (var cmd = PostgreDbAccess.GetCommand("message_delete"))
                {
                    //if (eraseMessages == null) { eraseMessages = false; }
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                    cmd.Parameters.Add(new NpgsqlParameter("i_erasemessage", NpgsqlDbType.Boolean)).Value =
                        eraseMessages;

                    PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
                }
            }
            else
            {
                //Delete Message
                // undelete function added
                using (var cmd = PostgreDbAccess.GetCommand("message_deleteundelete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                    cmd.Parameters.Add(new NpgsqlParameter("i_ismoderatorchanged", NpgsqlDbType.Boolean)).Value =
                        isModeratorChanged;
                    cmd.Parameters.Add(new NpgsqlParameter("i_deletereason", NpgsqlDbType.Varchar)).Value = deleteReason;
                    cmd.Parameters.Add(new NpgsqlParameter("i_isdeleteaction", NpgsqlDbType.Integer)).Value =
                        isDeleteAction;

                    PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
                }
            }
        }

        // <summary> Set flag on message to approved and store in DB </summary>
        public static void message_approve([NotNull] string connectionString, object messageID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_approve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Get message topic IDs (for URL rewriting)
        /// </summary>
        /// <param name="StartID"></param>
        /// <param name="Limit"></param>
        /// <returns></returns>
        public static DataTable message_simplelist([NotNull] string connectionString, int StartID, int Limit)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_simplelist"))
            {
                if (StartID <= 0)
                {
                    StartID = 0;
                }
                if (Limit <= 0)
                {
                    Limit = 1000;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_startid", NpgsqlDbType.Integer)).Value = StartID;
                cmd.Parameters.Add(new NpgsqlParameter("i_limit", NpgsqlDbType.Integer)).Value = Limit;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }


        public static void message_update(
            [NotNull] string connectionString,
            object messageID,
            object priority,
            object message,
            object description,
            object status,
            object styles,
            object subject,
            object flags,
            object reasonOfEdit,
            object isModeratorChanged,
            object overrideApproval,
            object origMessage,
            object editedBy,
            object messageDescription,
            string tags)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_priority", NpgsqlDbType.Integer)).Value = priority;
                cmd.Parameters.Add(new NpgsqlParameter("i_subject", NpgsqlDbType.Varchar)).Value = subject;
                cmd.Parameters.Add(new NpgsqlParameter("i_description", NpgsqlDbType.Varchar)).Value = description;
                cmd.Parameters.Add(new NpgsqlParameter("i_status", NpgsqlDbType.Varchar)).Value = status;
                cmd.Parameters.Add(new NpgsqlParameter("i_styles", NpgsqlDbType.Varchar)).Value = styles;
                cmd.Parameters.Add(new NpgsqlParameter("i_flags", NpgsqlDbType.Integer)).Value = flags;
                cmd.Parameters.Add(new NpgsqlParameter("i_message", NpgsqlDbType.Text)).Value = message;
                cmd.Parameters.Add(new NpgsqlParameter("i_reason", NpgsqlDbType.Varchar)).Value = reasonOfEdit;
                cmd.Parameters.Add(new NpgsqlParameter("i_editedby", NpgsqlDbType.Integer)).Value = editedBy;
                cmd.Parameters.Add(new NpgsqlParameter("i_ismoderatorchanged", NpgsqlDbType.Boolean)).Value =
                    isModeratorChanged;
                cmd.Parameters.Add(new NpgsqlParameter("i_overrideapproval", NpgsqlDbType.Boolean)).Value =
                    overrideApproval ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_originalmessage", NpgsqlDbType.Text)).Value = origMessage;
                cmd.Parameters.Add(new NpgsqlParameter("i_newguid", NpgsqlDbType.Uuid)).Value = Guid.NewGuid();
                cmd.Parameters.Add(new NpgsqlParameter("i_messagedescription", NpgsqlDbType.Varchar)).Value = messageDescription;
                cmd.Parameters.Add(new NpgsqlParameter("i_tags", NpgsqlDbType.Text)).Value = tags;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        // <summary> Save message to Db. </summary>
        public static bool message_save(
            [NotNull] string connectionString,
            [NotNull] object topicId,
            [NotNull] object userId,
            [NotNull] object message,
            [NotNull] object userName,
            [NotNull] object ip,
            [NotNull] object posted,
            [NotNull] object replyTo,
            [NotNull] object flags,
            [CanBeNull] object messageDescription,
            ref long messageId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_save"))
            {
                if (userName == null)
                {
                    userName = DBNull.Value;
                }
                if (posted == null)
                {
                    posted = DBNull.Value;
                }


                NpgsqlParameter paramMessageID = new NpgsqlParameter("i_messageid", messageId);
                paramMessageID.Direction = ParameterDirection.Output;

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_message", NpgsqlDbType.Text)).Value = message;
                cmd.Parameters.Add(new NpgsqlParameter("i_username", NpgsqlDbType.Varchar)).Value = userName;
                cmd.Parameters.Add(new NpgsqlParameter("i_ip", NpgsqlDbType.Varchar)).Value = ip;
                cmd.Parameters.Add(new NpgsqlParameter("i_posted", NpgsqlDbType.Timestamp)).Value = posted;
                cmd.Parameters.Add(new NpgsqlParameter("i_replyto", NpgsqlDbType.Integer)).Value = replyTo;
                cmd.Parameters.Add(new NpgsqlParameter("i_blogpostid", NpgsqlDbType.Varchar)).Value = DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_externalmessageid", NpgsqlDbType.Varchar)).Value =
                    DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_referencemessageid", NpgsqlDbType.Varchar)).Value =
                    DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_flags", NpgsqlDbType.Integer)).Value = flags;
                cmd.Parameters.Add(new NpgsqlParameter("i_messagedescription", NpgsqlDbType.Varchar)).Value = messageDescription;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;
                cmd.Parameters.Add(paramMessageID);
                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
                messageId = Convert.ToInt64(paramMessageID.Value);
                return true;
            }
        }

        public static DataTable message_unapproved([NotNull] string connectionString, object forumID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_unapproved"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_forumID", NpgsqlDbType.Integer)).Value = forumID;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable message_findunread(
            [NotNull] string connectionString,
            object topicID,
            object messageId,
            object lastRead,
            object showDeleted,
            object authorUserID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_findunread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageId;
                cmd.Parameters.Add(new NpgsqlParameter("i_lastread", NpgsqlDbType.Timestamp)).Value = lastRead;
                cmd.Parameters.Add(new NpgsqlParameter("i_showdeleted", NpgsqlDbType.Boolean)).Value = showDeleted;
                cmd.Parameters.Add(new NpgsqlParameter("i_authoruserid", NpgsqlDbType.Integer)).Value = authorUserID;
                DataTable dt = PostgreDbAccess.GetData(cmd, connectionString);
                return dt;
            }
        }

        // message movind function
        public static void message_move([NotNull] string connectionString, object messageID, object moveToTopic, bool moveAll)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_move"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_movetotopic", NpgsqlDbType.Integer)).Value = moveToTopic;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
            //moveAll=true anyway
            // it's in charge of moving answers of moved post
            if (moveAll)
            {
                using (var cmd = PostgreDbAccess.GetCommand("message_getReplies"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;

                    DataTable tbReplies = PostgreDbAccess.GetData(cmd, connectionString);
                    foreach (DataRow row in tbReplies.Rows)
                    {
                        message_moveRecursively(connectionString, row["MessageID"], moveToTopic);
                    }

                }
            }
        }

        //moves answers of moved post
        private static void message_moveRecursively([NotNull] string connectionString, object messageID, object moveToTopic)
        {
            bool UseFileTable = GetBooleanRegistryValue(connectionString, "UseFileTable");

            //Delete replies
            using (var cmd = PostgreDbAccess.GetCommand("message_getReplies"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;

                DataTable tbReplies = PostgreDbAccess.GetData(cmd, connectionString);
                foreach (DataRow row in tbReplies.Rows)
                {
                    message_moveRecursively(connectionString, row["messageID"], moveToTopic);
                }
                using (NpgsqlCommand innercmd = PostgreDbAccess.GetCommand("message_move"))
                {
                    innercmd.CommandType = CommandType.StoredProcedure;

                    innercmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                    innercmd.Parameters.Add(new NpgsqlParameter("i_movetotopic", NpgsqlDbType.Integer)).Value =
                        moveToTopic;

                    PostgreDbAccess.ExecuteNonQuery(innercmd, connectionString);
                }
            }
        }



        // functions for Thanks feature

        // <summary> Checks if the message with the provided messageID is thanked 
        //           by the user with the provided UserID. if so, returns true,
        //           otherwise returns false. </summary>
        public static bool message_isThankedByUser([NotNull] string connectionString, object userID, object messageID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_isthankedbyuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;

                return Convert.ToBoolean(PostgreDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        /// <summary>
        /// Is User Thanked the current Message
        /// </summary>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// If the User Thanked the the Current Message
        /// </returns>
        public static bool user_ThankedMessage([NotNull] string connectionString, object messageId, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_thankedmessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                int thankCount = (int)PostgreDbAccess.ExecuteScalar(cmd, connectionString);

                return thankCount > 0;
            }
        }

        public static int user_ThankFromCount([NotNull] string connectionString, [NotNull] object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_thankfromcount"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                var thankCount = (int)PostgreDbAccess.ExecuteScalar(cmd, connectionString);

                return thankCount;
            }
        }

        // <summary> Return the number of times the message with the provided messageID
        //           has been thanked. </summary>
        public static int message_ThanksNumber([NotNull] string connectionString, object messageID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_thanksnumber"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;

                return (int)PostgreDbAccess.ExecuteScalar(cmd, connectionString);
            }
        }

        // <summary> Returns the UserIDs and UserNames who have thanked the message
        //           with the provided messageID. </summary>
        public static DataTable message_GetThanks([NotNull] string connectionString, object messageID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_getthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        // <summary> Retuns All the Thanks for the Message IDs which are in the 
        //           delimited string variable MessageIDs </summary>
        public static DataTable message_GetAllThanks([NotNull] string connectionString, object messageIDs)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_getallthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageids", NpgsqlDbType.Text)).Value = messageIDs;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Retuns All the message text for the Message IDs which are in the 
        /// delimited string variable MessageIDs
        /// </summary>
        /// <param name="messageIDs">
        /// The message ids.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable message_GetTextByIds([NotNull] string connectionString, string messageIDs)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_gettextbyids"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageids", NpgsqlDbType.Text)).Value = messageIDs;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Retuns All the Thanks for the Message IDs which are in the 
        /// delimited string variable MessageIDs
        /// </summary>
        /// <param name="messageIdsSeparatedWithColon">
        /// The message i ds.
        /// </param>
        /// <returns>
        /// </returns>
        public static IEnumerable<TypedAllThanks> MessageGetAllThanks(
            [NotNull] string connectionString, string messageIdsSeparatedWithColon)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_getallthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageids", NpgsqlDbType.Text)).Value =
                    messageIdsSeparatedWithColon;

                return PostgreDbAccess.GetData(cmd, connectionString).AsEnumerable().Select(t => new TypedAllThanks(t));
            }
        }

        public static string message_AddThanks(
            [NotNull] string connectionString, object fromUserID, object messageID, bool useDisplayName)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_addthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                // var paramOutput = new NpgsqlParameter("paramOutput", NpgsqlDbType.Varchar);
                // paramOutput.Direction = ParameterDirection.Output;                  
                cmd.Parameters.Add(new NpgsqlParameter("i_fromuserid", NpgsqlDbType.Integer)).Value = fromUserID;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;
                cmd.Parameters.Add(new NpgsqlParameter("i_usedisplayname", NpgsqlDbType.Boolean)).Value = useDisplayName;

                return PostgreDbAccess.ExecuteScalar(cmd, connectionString).ToString();
                //return paramOutput.Value.ToString();                
            }
        }

        public static string message_RemoveThanks(
            [NotNull] string connectionString, object fromUserID, object messageID, bool useDisplayName)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_removethanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_fromuserid", NpgsqlDbType.Integer)).Value = fromUserID;
                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_usedisplayname", NpgsqlDbType.Boolean)).Value = useDisplayName;

                PostgreDbAccess.ExecuteScalar(cmd, connectionString);
                return PostgreDbAccess.ExecuteScalar(cmd, connectionString).ToString();
            }
        }

        /// <summary>
        /// The messagehistory_list.
        /// </summary>
        /// <param name="messageID">
        /// The Message ID.
        /// </param>
        /// <param name="daysToClean">
        /// Days to clean.
        /// </param>
        /// <param name="showAll">
        /// The Show All.
        /// </param>
        /// <returns>
        /// List of all message changes. 
        /// </returns>
        public static DataTable messagehistory_list([NotNull] string connectionString, int messageID, int daysToClean)
        {
            using (var cmd = PostgreDbAccess.GetCommand("messagehistory_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_daystoclean", NpgsqlDbType.Integer)).Value = daysToClean;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Returns message data based on user access rights
        /// </summary>
        /// <param name="MessageID">The Message Id.</param>
        /// <param name="UserID">The UserId.</param>
        /// <returns></returns>
        public static DataTable message_secdata([NotNull] string connectionString, int MessageID, object pageUserId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("message_secdata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = MessageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;

                return PostgreDbAccess.GetData(cmd, connectionString);

            }
        }

        #endregion        

        #region yaf_NntpForum

        public static IEnumerable<TypedNntpForum> NntpForumList(
            [NotNull] string connectionString, int boardId, int? minutes, int? nntpForumID, bool? active)
        {
            using (var cmd = PostgreDbAccess.GetCommand("nntpforum_list"))
            {

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_minutes", NpgsqlDbType.Integer)).Value = minutes;
                cmd.Parameters.Add(new NpgsqlParameter("i_nntpforumid", NpgsqlDbType.Integer)).Value = nntpForumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_active", NpgsqlDbType.Boolean)).Value = active;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString).AsEnumerable().Select(r => new TypedNntpForum(r));
            }
        }

        public static DataTable nntpforum_list(
            [NotNull] string connectionString, object boardId, object minutes, object nntpForumID, object active)
        {
            using (var cmd = PostgreDbAccess.GetCommand("nntpforum_list"))
            {
                if (minutes == null)
                {
                    minutes = DBNull.Value;
                }
                if (nntpForumID == null)
                {
                    nntpForumID = DBNull.Value;
                }
                if (active == null)
                {
                    active = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_minutes", NpgsqlDbType.Integer)).Value = minutes;
                cmd.Parameters.Add(new NpgsqlParameter("i_nntpforumid", NpgsqlDbType.Integer)).Value = nntpForumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_active", NpgsqlDbType.Boolean)).Value = active;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void nntpforum_update(
            [NotNull] string connectionString, object nntpForumID, object lastMessageNo, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("nntpforum_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_nntpforumid", NpgsqlDbType.Integer)).Value = nntpForumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_lastmessageno", NpgsqlDbType.Integer)).Value = lastMessageNo;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void nntpforum_save(
            [NotNull] string connectionString,
            object nntpForumID,
            object nntpServerID,
            object groupName,
            object forumID,
            object active,
            object cutoffdate)
        {
            using (var cmd = PostgreDbAccess.GetCommand("nntpforum_save"))
            {
                if (nntpForumID == null)
                {
                    nntpForumID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_nntpforumid", NpgsqlDbType.Integer)).Value = nntpForumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_nntpserverid", NpgsqlDbType.Integer)).Value = nntpServerID;
                cmd.Parameters.Add(new NpgsqlParameter("i_groupname", NpgsqlDbType.Varchar)).Value = groupName;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_active", NpgsqlDbType.Boolean)).Value = active;
                cmd.Parameters.Add(new NpgsqlParameter("i_datecutoff", NpgsqlDbType.Timestamp)).Value = cutoffdate;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void nntpforum_delete([NotNull] string connectionString, object nntpForumID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("nntpforum_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_nntpforumid", NpgsqlDbType.Integer)).Value = nntpForumID;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_NntpServer

        public static DataTable nntpserver_list([NotNull] string connectionString, object boardId, object nntpServerID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("nntpserver_list"))
            {
                if (boardId == null)
                {
                    boardId = DBNull.Value;
                }
                if (nntpServerID == null)
                {
                    nntpServerID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_nntpserverid", NpgsqlDbType.Integer)).Value = nntpServerID;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void nntpserver_save(
            [NotNull] string connectionString,
            object nntpServerID,
            object boardId,
            object name,
            object address,
            object port,
            object userName,
            object userPass)
        {
            using (var cmd = PostgreDbAccess.GetCommand("nntpserver_save"))
            {
                if (nntpServerID == null)
                {
                    nntpServerID = DBNull.Value;
                }
                if (userName == null)
                {
                    userName = DBNull.Value;
                }
                if (userPass == null)
                {
                    userPass = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_nntpserverid", NpgsqlDbType.Integer)).Value = nntpServerID;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_name", NpgsqlDbType.Varchar)).Value = name;
                cmd.Parameters.Add(new NpgsqlParameter("i_address", NpgsqlDbType.Varchar)).Value = address;
                cmd.Parameters.Add(new NpgsqlParameter("i_port", NpgsqlDbType.Integer)).Value = port;
                cmd.Parameters.Add(new NpgsqlParameter("i_username", NpgsqlDbType.Varchar)).Value = userName;
                cmd.Parameters.Add(new NpgsqlParameter("i_userpass", NpgsqlDbType.Varchar)).Value = userPass;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void nntpserver_delete([NotNull] string connectionString, object nntpServerID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("nntpserver_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_nntpserverid", NpgsqlDbType.Integer)).Value = nntpServerID;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_NntpTopic

        public static DataTable nntptopic_list([NotNull] string connectionString, object thread)
        {
            using (var cmd = PostgreDbAccess.GetCommand("nntptopic_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_thread", NpgsqlDbType.Varchar)).Value = thread;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void nntptopic_savemessage(
            [NotNull] string connectionString,
            object nntpForumID,
            object topic,
            object body,
            object userId,
            object userName,
            object ip,
            object posted,
            object externalMessageId,
            object referenceMessageId)
        {
            // string newbody = body.ToString().Replace(@"&lt;br&gt;", "> ").Replace(@"&amp;lt;", "<").Replace(@"&lt;hr&gt;", "> ").Replace(@"&amp;quot;", @"""").Replace(@"&lt;", @"<").Replace(@"br&gt;", @"> ").Replace(@"&amp;gt;", @"> ").Replace(@"&gt;", "> ").Replace("&quot;", @"""").Replace("[-snip-]","(SNIP)").Replace(@"@","[dog]").Replace("_.","");
            using (var cmd = PostgreDbAccess.GetCommand("nntptopic_savemessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_nntpforumid", NpgsqlDbType.Integer)).Value = nntpForumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_topic", NpgsqlDbType.Varchar)).Value = topic;
                cmd.Parameters.Add(new NpgsqlParameter("i_body", NpgsqlDbType.Text)).Value = body;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_username", NpgsqlDbType.Varchar)).Value = userName;
                cmd.Parameters.Add(new NpgsqlParameter("i_ip", NpgsqlDbType.Varchar)).Value = ip;
                cmd.Parameters.Add(new NpgsqlParameter("i_posted", NpgsqlDbType.Timestamp)).Value = posted;
                cmd.Parameters.Add(new NpgsqlParameter("i_externalmessageid", NpgsqlDbType.Varchar)).Value =
                    externalMessageId;
                cmd.Parameters.Add(new NpgsqlParameter("i_referencemessageid", NpgsqlDbType.Varchar)).Value =
                    referenceMessageId;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_PMessage

        /// <summary>
        /// The pmessage_list.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="userPMessageID">
        /// The user p message id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable pmessage_list([NotNull] string connectionString, object userPMessageID)
        {
            return pmessage_list(connectionString, null, null, userPMessageID);
        }

        /// <summary>
        /// Returns a list of private messages based on the arguments specified.
        /// If pMessageID != null, returns the PM of id pMessageId.
        /// If toUserID != null, returns the list of PMs sent to the user with the given ID.
        /// If fromUserID != null, returns the list of PMs sent by the user of the given ID.
        /// </summary>
        /// <param name="toUserID"></param>
        /// <param name="fromUserID"></param>
        /// <param name="pMessageID">The id of the private message</param>
        /// <returns></returns>
        public static DataTable pmessage_list(
            [NotNull] string connectionString, object toUserID, object fromUserID, object userPMessageID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pmessage_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_touserid", NpgsqlDbType.Integer)).Value = toUserID ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_fromuserid", NpgsqlDbType.Integer)).Value = fromUserID ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_userpmessageid", NpgsqlDbType.Integer)).Value = userPMessageID ?? DBNull.Value;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Deletes the private message from the database as per the given parameter.  If fromOutbox is true,
        /// the message is only deleted from the user's outbox.  Otherwise, it is completely delete from the database.
        /// </summary>
        /// <param name="userPMessageID"></param>
        public static void pmessage_delete([NotNull] string connectionString, object userPMessageID)
        {
            pmessage_delete(connectionString, userPMessageID, false);
        }

        /// <summary>
        /// Deletes the private message from the database as per the given parameter.  If <paramref name="fromOutbox"/> is true,
        /// the message is only removed from the user's outbox.  Otherwise, it is completely delete from the database.
        /// </summary>
        /// <param name="pMessageID"></param>
        /// <param name="fromOutbox">If true, removes the message from the outbox.  Otherwise deletes the message completely.</param>
        public static void pmessage_delete([NotNull] string connectionString, object userPMessageID, bool fromOutbox)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pmessage_delete"))
            {
                // if (fromOutbox != false || fromOutbox != true) { fromOutbox = false; }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userpmessageid", NpgsqlDbType.Integer)).Value = userPMessageID;
                cmd.Parameters.Add(new NpgsqlParameter("i_fromoutbox", NpgsqlDbType.Boolean)).Value = fromOutbox;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Archives the private message of the given id.  Archiving moves the message from the user's inbox to his message archive.
        /// </summary>
        /// <param name="pMessageID">The ID of the private message</param>
        public static void pmessage_archive([NotNull] string connectionString, object userPMessageID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pmessage_archive"))
            {
                if (userPMessageID == null)
                {
                    userPMessageID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userpmessageid", NpgsqlDbType.Integer)).Value = userPMessageID;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void pmessage_save(
            [NotNull] string connectionString,
            object fromUserID,
            object toUserID,
            object subject,
            object body,
            object Flags,
            object replyTo)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pmessage_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_fromuserid", NpgsqlDbType.Integer)).Value = fromUserID;
                cmd.Parameters.Add(new NpgsqlParameter("i_touserid", NpgsqlDbType.Integer)).Value = toUserID;
                cmd.Parameters.Add(new NpgsqlParameter("i_subject", NpgsqlDbType.Varchar)).Value = subject;
                cmd.Parameters.Add(new NpgsqlParameter("i_body", NpgsqlDbType.Text)).Value = body;
                cmd.Parameters.Add(new NpgsqlParameter("i_flags", NpgsqlDbType.Integer)).Value = Flags;
                cmd.Parameters.Add(new NpgsqlParameter("i_replyto", NpgsqlDbType.Integer)).Value = replyTo;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void pmessage_markread([NotNull] string connectionString, object userPMessageID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pmessage_markread"))
            {
                if (userPMessageID == null)
                {
                    userPMessageID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userpmessageid", NpgsqlDbType.Integer)).Value = userPMessageID;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static DataTable pmessage_info([NotNull] string connectionString )
        {
            using (var cmd = PostgreDbAccess.GetCommand("pmessage_info"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void pmessage_prune([NotNull] string connectionString, object daysRead, object daysUnread)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pmessage_prune"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_daysread", NpgsqlDbType.Integer)).Value = daysRead;
                cmd.Parameters.Add(new NpgsqlParameter("i_daysunread", NpgsqlDbType.Integer)).Value = daysUnread;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_Poll

        /// <summary>
        /// The pollgroup_stats.
        /// </summary>
        /// <param name="pollGroupId">
        /// The poll group id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable pollgroup_stats([NotNull] string connectionString, int? pollGroupId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pollgroup_stats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("i_pollgroupid", pollGroupId);
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The pollgroup_attach.
        /// </summary>
        /// <param name="pollGroupId">
        /// The poll group id.
        /// </param>
        /// <returns>
        /// </returns>
        public static int pollgroup_attach(
            [NotNull] string connectionString, int? pollGroupId, int? topicId, int? forumId, int? categoryId, int? boardId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pollgroup_attach"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("i_pollgroupid", pollGroupId);
                cmd.Parameters.AddWithValue("i_topicid", topicId);
                cmd.Parameters.AddWithValue("i_forumid", forumId);
                cmd.Parameters.AddWithValue("i_categoryid", categoryId);
                cmd.Parameters.AddWithValue("i_boardid", boardId);
                return (int)PostgreDbAccess.ExecuteScalar(cmd, connectionString);
            }
        }

        public static DataTable poll_stats([NotNull] string connectionString, int? pollId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("poll_stats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_pollid", NpgsqlDbType.Integer)).Value = pollId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The method saves many questions and answers to them in a single transaction 
        /// </summary>
        /// <param name="pollList">List to hold all polls data</param>
        /// <returns>Last saved poll id.</returns>
        public static int? poll_save([NotNull] string connectionString, List<PollSaveList> pollList)
        {
            int? currPoll;
            int? pollGroup = null;
            foreach (PollSaveList question in pollList)
            {
                StringBuilder pgStr = new StringBuilder();
                // Check if the group already exists
                if (question.TopicId > 0)
                {
                    pgStr.Append("select pollid  from ");
                    pgStr.Append(PostgreDbAccess.GetObjectName("topic"));
                    pgStr.Append(" WHERE topicid = :i_topicid; ");
                }
                else if (question.ForumId > 0)
                {
                    pgStr.Append("select  pollgroupid  from ");
                    pgStr.Append(PostgreDbAccess.GetObjectName("forum"));
                    pgStr.Append(" WHERE forumid =:i_forumid");
                }
                else if (question.CategoryId > 0)
                {
                    pgStr.Append("select pollgroupid  from ");
                    pgStr.Append(PostgreDbAccess.GetObjectName("category"));
                    pgStr.Append(" WHERE categoryid = :i_categoryid");
                }

                using (var cmdPg = PostgreDbAccess.GetCommand(pgStr.ToString(), true))
                {
                    // Add parameters
                    if (question.TopicId > 0)
                    {
                        cmdPg.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value =
                            question.TopicId;
                    }
                    else if (question.ForumId > 0)
                    {
                        cmdPg.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value =
                            question.ForumId;
                    }
                    else if (question.CategoryId > 0)
                    {
                        cmdPg.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value =
                            question.CategoryId;
                    }

                    object pgexist = PostgreDbAccess.ExecuteScalar(cmdPg, true, connectionString);
                    if (pgexist != DBNull.Value)
                    {
                        pollGroup = Convert.ToInt32(pgexist);
                    }

                }
                // buck stops here
                // the group doesn't exists, create a new one
                if (pollGroup == null)
                {
                    pgStr = new StringBuilder();
                    pgStr.Append("INSERT INTO ");
                    pgStr.Append(PostgreDbAccess.GetObjectName("pollgroupcluster"));
                    pgStr.Append("(userid,flags ) VALUES(:i_userid, :i_flags) RETURNING pollgroupid; ");
                    using (var cmdPgIns = PostgreDbAccess.GetCommand(pgStr.ToString(), true))
                    {
                        // set poll group flags
                        int pollGroupFlags = 0;
                        if (question.IsBound)
                        {
                            pollGroupFlags = pollGroupFlags | 2;
                        }

                        // Add parameters
                        cmdPgIns.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value =
                            question.UserId;
                        cmdPgIns.Parameters.Add(new NpgsqlParameter("i_flags", NpgsqlDbType.Integer)).Value =
                            pollGroupFlags;
                        pollGroup = (int?)PostgreDbAccess.ExecuteScalar(cmdPgIns, true, connectionString);
                    }
                }




                using (var cmd = PostgreDbAccess.GetCommand("poll_save"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new NpgsqlParameter("i_question", NpgsqlDbType.Varchar)).Value =
                        question.Question;

                    if (question.Closes > DateTimeHelper.SqlDbMinTime())
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("i_closes", NpgsqlDbType.Timestamp)).Value =
                            question.Closes;
                    }
                    else
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("i_closes", NpgsqlDbType.Timestamp)).Value =
                            DBNull.Value;
                    }
                    int pollFlags = question.IsClosedBound ? 0 | 4 : 0;
                    pollFlags = question.AllowMultipleChoices ? pollFlags | 8 : pollFlags;
                    pollFlags = question.ShowVoters ? pollFlags | 16 : pollFlags;
                    pollFlags = question.AllowSkipVote ? pollFlags | 32 : pollFlags;
                    cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = question.UserId;
                    cmd.Parameters.Add(new NpgsqlParameter("i_pollgroupid", NpgsqlDbType.Integer)).Value = pollGroup;
                    cmd.Parameters.Add(new NpgsqlParameter("i_objectpath", NpgsqlDbType.Varchar)).Value =
                        question.QuestionObjectPath;
                    cmd.Parameters.Add(new NpgsqlParameter("i_mimetype", NpgsqlDbType.Varchar)).Value =
                        question.QuestionMimeType;
                    cmd.Parameters.Add(new NpgsqlParameter("i_flags", NpgsqlDbType.Integer)).Value = pollFlags;


                    currPoll = (int?)PostgreDbAccess.ExecuteScalar(cmd, connectionString);
                }


                // The cycle through question reply choices  
                int chl = question.Choice.GetUpperBound(1) + 1;
                for (uint choiceCount = 0; choiceCount < chl; choiceCount++)
                {
                    if (question.Choice[0, choiceCount].Trim().Length > 0)
                    {
                        StringBuilder sbChoice = new StringBuilder();
                        sbChoice.Append("INSERT INTO ");
                        sbChoice.Append(PostgreDbAccess.GetObjectName("choice"));
                        sbChoice.AppendFormat(
                            "(pollid,choice,votes,objectpath,mimetype) VALUES (:pollid{0}, :choice{0}, :votes{0}, :objectpath{0}, :mimetype{0}); ",
                            choiceCount);
                        using (var cmdChoice = PostgreDbAccess.GetCommand(sbChoice.ToString(), true))
                        {
                            cmdChoice.Parameters.Add(
                                new NpgsqlParameter(String.Format("pollid{0}", choiceCount), NpgsqlDbType.Integer))
                                     .Value = currPoll;
                            cmdChoice.Parameters.Add(
                                new NpgsqlParameter(String.Format("choice{0}", choiceCount), NpgsqlDbType.Varchar))
                                     .Value = question.Choice[0, choiceCount];
                            cmdChoice.Parameters.Add(
                                new NpgsqlParameter(String.Format("votes{0}", choiceCount), NpgsqlDbType.Integer)).Value
                                = 0;
                            cmdChoice.Parameters.Add(
                                new NpgsqlParameter(String.Format("objectpath{0}", choiceCount), NpgsqlDbType.Varchar))
                                     .Value = question.Choice[1, choiceCount].IsNotSet()
                                                  ? String.Empty
                                                  : question.Choice[1, choiceCount];
                            cmdChoice.Parameters.Add(
                                new NpgsqlParameter(String.Format("mimetype{0}", choiceCount), NpgsqlDbType.Varchar))
                                     .Value = question.Choice[2, choiceCount].IsNotSet()
                                                  ? String.Empty
                                                  : question.Choice[2, choiceCount];
                            PostgreDbAccess.ExecuteNonQuery(cmdChoice, true, connectionString);
                        }

                    }

                }
                //   var cmd = new NpgsqlCommand();
                //  cmd.CommandText = paramSb.ToString() + ")" + sb.ToString();
                //   NpgsqlConnection con = PostgreDbAccess.Current.GetConnectionManager().DBConnection;
                // con.Open();
                //  cmd.Connection = con;
                //   IDbTransaction trans = cmd.Connection.BeginTransaction();
                //    cmd.Transaction = trans;
                //    cmd.CommandText = sb.ToString();


                /* using (var cmd1 = PostgreDbAccess.GetCommand(sb.ToString(), true))
                {


                    // Add parameters
                     cmd1.Parameters.Add(new NpgsqlParameter("question", NpgsqlDbType.Varchar));

                    if (question.Closes > DateTimeHelper.SqlDbMinTime())
                    {
                        cmd1.Parameters.Add(new NpgsqlParameter("closes", NpgsqlDbType.Timestamp));
                    } 
                    for (uint choiceCount1 = 0; choiceCount1 < question.Choice.GetLength(0); choiceCount1++)
                    {
                        if (question.Choice[0, choiceCount1].Trim().Length > 0)
                        {
                            cmd1.Parameters.Add(new NpgsqlParameter(String.Format("choice{0}", choiceCount1),
                                                                    NpgsqlDbType.Varchar)).Value =
                                question.Choice[0, choiceCount1];
                            cmd1.Parameters.Add(new NpgsqlParameter(String.Format("votes{0}", choiceCount1),
                                                                    NpgsqlDbType.Integer)).Value = 0;
                            cmd1.Parameters.Add(new NpgsqlParameter(String.Format("objectpath{0}", choiceCount1),
                                                                    NpgsqlDbType.Varchar)).Value =
                                question.Choice[1, choiceCount1].IsNotSet() ? String.Empty : question.Choice[1, choiceCount1];
                            cmd1.Parameters.Add(new NpgsqlParameter(String.Format("mimetype{0}", choiceCount1),
                                                                    NpgsqlDbType.Varchar)).Value =
                                question.Choice[2, choiceCount1].IsNotSet() ? String.Empty : question.Choice[2, choiceCount1];
                        }
                    }
                     int? result = (int?)PostgreDbAccess.ExecuteNonQueryInt(cmd1, true);
                }
            */

                // Add pollgroup id to an object
                StringBuilder Usb = new StringBuilder();
                //cmd2.Parameters.Add(new NpgsqlParameter(":i_pollgroupid", NpgsqlDbType.Integer)).Value = pollGroup;
                if (question.TopicId > 0)
                {
                    Usb.Append("UPDATE ");
                    Usb.Append(PostgreDbAccess.GetObjectName("topic"));
                    Usb.Append(" SET pollid = :i_pollid WHERE topicid= :i_topicid; ");
                }
                else if (question.ForumId > 0)
                {
                    Usb.Append("UPDATE ");
                    Usb.Append(PostgreDbAccess.GetObjectName("forum"));
                    Usb.Append(" SET pollgroupid = :i_pollgroupid where forumid = :i_forumid; ");

                }
                else if (question.CategoryId > 0)
                {
                    Usb.Append("UPDATE ");
                    Usb.Append(PostgreDbAccess.GetObjectName("category"));
                    Usb.Append(" SET pollgroupid = :i_pollgroupid where categoryid = :i_categoryid; ");
                }


                using (var cmd2 = PostgreDbAccess.GetCommand(Usb.ToString(), true))
                {
                    cmd2.Parameters.Add(new NpgsqlParameter("i_pollid", NpgsqlDbType.Integer)).Value = pollGroup;
                    //cmd2.Parameters.Add(new NpgsqlParameter(":i_pollgroupid", NpgsqlDbType.Integer)).Value = pollGroup;
                    if (question.TopicId > 0)
                    {
                        cmd2.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value =
                            question.TopicId;
                    }
                    else if (question.ForumId > 0)
                    {
                        cmd2.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value =
                            question.ForumId;
                    }
                    else if (question.CategoryId > 0)
                    {
                        cmd2.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value =
                            question.CategoryId;
                    }
                    PostgreDbAccess.ExecuteNonQuery(cmd2, connectionString);
                }


                /* if (ret.Value != DBNull.Value)
                     {
                         return (int?)ret.Value;
                     }*/

                //  }
                //   trans.Commit();
                //   con.Close();

            }

            return pollGroup;
        }


        public static void poll_update(
            [NotNull] string connectionString,
            object pollID,
            object question,
            object closes,
            object isBounded,
            bool isClosedBounded,
            bool allowMultipleChoices,
            bool showVoters,
            bool allowSkipVote,
            object questionPath,
            object questionMime)
        {
            using (var cmd = PostgreDbAccess.GetCommand("poll_update"))
            {
                if (closes == null)
                {
                    closes = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_pollid", NpgsqlDbType.Integer)).Value = pollID;
                cmd.Parameters.Add(new NpgsqlParameter("i_question", NpgsqlDbType.Varchar)).Value = question;
                cmd.Parameters.Add(new NpgsqlParameter("i_closes", NpgsqlDbType.Timestamp)).Value = closes;
                cmd.Parameters.Add(new NpgsqlParameter("i_questionobjectpath", NpgsqlDbType.Varchar)).Value =
                    questionPath;
                cmd.Parameters.Add(new NpgsqlParameter("i_questionmimetype", NpgsqlDbType.Varchar)).Value = questionMime;
                cmd.Parameters.Add(new NpgsqlParameter("i_isbounded", NpgsqlDbType.Boolean)).Value = isBounded;
                cmd.Parameters.Add(new NpgsqlParameter("i_isclosedbounded", NpgsqlDbType.Boolean)).Value =
                    isClosedBounded;
                cmd.Parameters.Add(new NpgsqlParameter("i_allowmultiplechoices", NpgsqlDbType.Boolean)).Value =
                    allowMultipleChoices;
                cmd.Parameters.Add(new NpgsqlParameter("i_showvoters", NpgsqlDbType.Boolean)).Value = showVoters;
                cmd.Parameters.Add(new NpgsqlParameter("i_allowskipvote", NpgsqlDbType.Boolean)).Value = allowSkipVote;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void poll_remove(
            [NotNull] string connectionString,
            object pollGroupID,
            object pollID,
            object boardId,
            bool removeCompletely,
            bool removeEverywhere)
        {
            using (var cmd = PostgreDbAccess.GetCommand("poll_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_pollgroupid", NpgsqlDbType.Integer)).Value = pollGroupID;
                cmd.Parameters.Add(new NpgsqlParameter("i_pollid", NpgsqlDbType.Integer)).Value = pollID;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_removecompletely", NpgsqlDbType.Boolean)).Value =
                    removeCompletely;
                cmd.Parameters.Add(new NpgsqlParameter("i_removeeverywhere", NpgsqlDbType.Boolean)).Value =
                    removeEverywhere;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Gets a typed poll group list.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// </returns>
        public static IEnumerable<TypedPollGroup> PollGroupList(
            [NotNull] string connectionString, int userID, int? forumId, int boardId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pollgroup_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumId;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;

                return PostgreDbAccess.GetData(cmd, connectionString).AsEnumerable().Select(r => new TypedPollGroup(r));
            }
        }

        /// <summary>
        /// The poll_remove.
        /// </summary>
        /// <param name="pollGroupID">
        /// The poll group id. The parameter should always be present. 
        /// </param>
        /// <param name="topicId">
        /// The poll id. If null all polls in a group a deleted. 
        /// </param>
        /// <param name="boardId">
        /// The BoardID id. 
        /// </param>
        /// <param name="removeCompletely">
        /// The RemoveCompletely. If true and pollID is null , all polls in a group are deleted completely, 
        /// else only one poll is deleted completely. 
        /// </param>
        /// <param name="forumId"></param>
        /// <param name="removeEverywhere"></param>
        public static void pollgroup_remove(
            [NotNull] string connectionString,
            object pollGroupID,
            object topicId,
            object forumId,
            object categoryId,
            object boardId,
            bool removeCompletely,
            bool removeEverywhere)
        {
            using (var cmd = PostgreDbAccess.GetCommand("pollgroup_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_pollgroupid", NpgsqlDbType.Integer)).Value = pollGroupID;
                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicId;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumId;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryId;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_removecompletely", NpgsqlDbType.Boolean)).Value =
                    removeCompletely;
                cmd.Parameters.Add(new NpgsqlParameter("i_removeeverywhere", NpgsqlDbType.Boolean)).Value =
                    removeEverywhere;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }     

        #endregion

        #region yaf_Rank

        public static DataTable rank_list([NotNull] string connectionString, object boardId, object rankID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("rank_list"))
            {
                if (rankID == null)
                {
                    rankID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_rankid", NpgsqlDbType.Integer)).Value = rankID;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void rank_save(
            [NotNull] string connectionString,
            object rankID,
            object boardId,
            object name,
            object isStart,
            object isLadder,
            [NotNull] object isGuest,
            object minPosts,
            object rankImage,
            object pmLimit,
            object style,
            object sortOrder,
            object description,
            object usrSigChars,
            object usrSigBBCodes,
            object usrSigHTMLTags,
            object usrAlbums,
            object usrAlbumImages)
        {
            using (var cmd = PostgreDbAccess.GetCommand("rank_save"))
            {
                if (rankImage == null)
                {
                    rankImage = DBNull.Value;
                }
                if (minPosts.ToString() == string.Empty)
                {
                    minPosts = 0;
                }
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_rankid", NpgsqlDbType.Integer)).Value = rankID;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_name", NpgsqlDbType.Varchar)).Value = name;
                cmd.Parameters.Add(new NpgsqlParameter("i_isstart", NpgsqlDbType.Boolean)).Value = isStart;
                cmd.Parameters.Add(new NpgsqlParameter("i_isladder", NpgsqlDbType.Boolean)).Value = isLadder;
                cmd.Parameters.Add(new NpgsqlParameter("i_isguest", NpgsqlDbType.Boolean)).Value = isGuest;
                cmd.Parameters.Add(new NpgsqlParameter("i_minposts", NpgsqlDbType.Integer)).Value = minPosts;
                cmd.Parameters.Add(new NpgsqlParameter("i_rankimage", NpgsqlDbType.Varchar)).Value = rankImage;
                cmd.Parameters.Add(new NpgsqlParameter("i_pmlimit", NpgsqlDbType.Integer)).Value = pmLimit;
                cmd.Parameters.Add(new NpgsqlParameter("i_style", NpgsqlDbType.Varchar)).Value = style;
                cmd.Parameters.Add(new NpgsqlParameter("i_sortorder", NpgsqlDbType.Smallint)).Value = sortOrder;
                cmd.Parameters.Add(new NpgsqlParameter("i_description", NpgsqlDbType.Varchar)).Value = description;
                cmd.Parameters.Add(new NpgsqlParameter("i_usrsigchars", NpgsqlDbType.Integer)).Value = usrSigChars;
                cmd.Parameters.Add(new NpgsqlParameter("i_usrsigbbcodes", NpgsqlDbType.Varchar)).Value = usrSigBBCodes;
                cmd.Parameters.Add(new NpgsqlParameter("i_usrsightmltags", NpgsqlDbType.Varchar)).Value = usrSigHTMLTags;
                cmd.Parameters.Add(new NpgsqlParameter("i_usralbums", NpgsqlDbType.Integer)).Value = usrAlbums;
                cmd.Parameters.Add(new NpgsqlParameter("i_usralbumimages", NpgsqlDbType.Integer)).Value = usrAlbumImages;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void rank_delete([NotNull] string connectionString, object rankID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("rank_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_rankid", NpgsqlDbType.Integer)).Value = rankID;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_Smiley

        public static DataTable smiley_list([NotNull] string connectionString, object boardId, object smileyID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("smiley_list"))
            {
                if (smileyID == null)
                {
                    smileyID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_smileyid", NpgsqlDbType.Integer)).Value = smileyID;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The smiley_list.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="smileyID">
        /// The smiley id.
        /// </param>
        /// <returns>
        /// </returns>
        public static IEnumerable<TypedSmileyList> SmileyList([NotNull] string connectionString, int boardId, int? smileyID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("smiley_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_smileyid", NpgsqlDbType.Integer)).Value = smileyID;

                return PostgreDbAccess.GetData(cmd, connectionString).AsEnumerable().Select(r => new TypedSmileyList(r));
            }
        }

        public static DataTable smiley_listunique([NotNull] string connectionString, object boardId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("smiley_listunique"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void smiley_delete([NotNull] string connectionString, object smileyID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("smiley_delete"))
            {
                if (smileyID == null)
                {
                    smileyID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_smileyid", NpgsqlDbType.Integer)).Value = smileyID;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void smiley_save(
            [NotNull] string connectionString,
            object smileyID,
            object boardId,
            object code,
            object icon,
            object emoticon,
            object sortOrder,
            object replace)
        {
            using (var cmd = PostgreDbAccess.GetCommand("smiley_save"))
            {
                if (smileyID == null)
                {
                    smileyID = DBNull.Value;
                }

                if (replace == null)
                {
                    replace = 0;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_smileyid", NpgsqlDbType.Integer)).Value = smileyID;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_code", NpgsqlDbType.Varchar)).Value = code;
                cmd.Parameters.Add(new NpgsqlParameter("i_icon", NpgsqlDbType.Varchar)).Value = icon;
                cmd.Parameters.Add(new NpgsqlParameter("i_emoticon", NpgsqlDbType.Varchar)).Value = emoticon;
                cmd.Parameters.Add(new NpgsqlParameter("i_sortorder", NpgsqlDbType.Smallint)).Value = sortOrder;
                cmd.Parameters.Add(new NpgsqlParameter("i_replace", NpgsqlDbType.Smallint)).Value = replace;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void smiley_resort([NotNull] string connectionString, object boardId, object smileyID, int move)
        {
            using (var cmd = PostgreDbAccess.GetCommand("smiley_resort"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_smileyid", NpgsqlDbType.Integer)).Value = smileyID;
                cmd.Parameters.Add(new NpgsqlParameter("i_move", NpgsqlDbType.Integer)).Value = move;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion        

        #region yaf_Registry

        /// <summary>
        /// Retrieves entries in the board settings registry
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="name">Use to specify return of specific entry only. Setting this to null returns all entries.
        /// </param>
        /// <returns>DataTable filled will registry entries</returns>
        public static DataTable registry_list([NotNull] string connectionString, object name, object boardId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("registry_list"))
            {
                if (name == null)
                {
                    name = DBNull.Value;
                }

                if (boardId == null)
                {
                    boardId = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_name", NpgsqlDbType.Varchar)).Value = name;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Retrieves entries in the board settings registry
        /// </summary>
        /// <param name="name">Use to specify return of specific entry only. Setting this to null returns all entries.</param>
        /// <returns>DataTable filled will registry entries</returns>
        public static DataTable registry_list([NotNull] string connectionString, [NotNull] object name)
        {
            return registry_list(connectionString, name, null);
        }

        /// <summary>
        /// Retrieves all the entries in the board settings registry
        /// </summary>
        /// <returns>DataTable filled will all registry entries</returns>
        public static DataTable registry_list([NotNull] string connectionString )
        {
            return registry_list(connectionString, null, null);
        }

        /// <summary>
        /// Saves a single registry entry pair to the database.
        /// </summary>
        /// <param name="Name">Unique name associated with this entry</param>
        /// <param name="Value">Value associated with this entry which can be null</param>
        public static void registry_save([NotNull] string connectionString, object name, object value)
        {

            registry_save(connectionString, name, value, DBNull.Value);

        }

        /// <summary>
        /// Saves a single registry entry pair to the database.
        /// </summary>
        /// <param name="Name">Unique name associated with this entry</param>
        /// <param name="Value">Value associated with this entry which can be null</param>
        /// <param name="BoardID">The BoardID for this entry</param>
        public static void registry_save([NotNull] string connectionString, object name, object value, object boardId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("registry_save"))
            {
                if (value == null)
                {
                    value = DBNull.Value;
                }
                if (boardId == null)
                {
                    boardId = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_name", NpgsqlDbType.Varchar)).Value = name;
                cmd.Parameters.Add(new NpgsqlParameter("i_value", NpgsqlDbType.Text)).Value = value;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_System

        /// <summary>
        /// Not in use anymore. Only required for old database versions.
        /// </summary>
        /// <returns></returns>
        public static DataTable system_list([NotNull] string connectionString )
        {
            using (var cmd = PostgreDbAccess.GetCommand("system_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_Topic

        public static DataTable topic_tags([NotNull] string connectionString, object boardId, object pageUserId, object topicId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_tags"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }

        }

        public static DataTable topic_bytags(
            [NotNull] string connectionString,
            object boardId,
            int forumId,
            object pageUserId,
            object tags,
            object sinceDate,
            int pageIndex,
            int pageSize)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_bytags"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumId;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                cmd.Parameters.Add(new NpgsqlParameter("i_tags", NpgsqlDbType.Varchar)).Value = tags;
                cmd.Parameters.Add(new NpgsqlParameter("i_sincedate", NpgsqlDbType.Timestamp)).Value = sinceDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }

        }


        public static void topic_updatetopic([NotNull] string connectionString, int topicId, string topic)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_updatetopic"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicId;
                cmd.Parameters.Add(new NpgsqlParameter("i_topic", NpgsqlDbType.Varchar)).Value = topic;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static int topic_prune(
            [NotNull] string connectionString, object boardId, object forumID, object days, object permDelete)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_prune"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_days", NpgsqlDbType.Integer)).Value = days;
                cmd.Parameters.Add(new NpgsqlParameter("i_permdelete", NpgsqlDbType.Boolean)).Value =
                    Convert.ToBoolean(permDelete);

                return (int)PostgreDbAccess.ExecuteScalar(cmd, connectionString);
            }
        }

        public static DataTable topic_list(
            [NotNull] string connectionString,
            object forumID,
            [NotNull] object userId,
            [NotNull] object sinceDate,
            [NotNull] object toDate,
            [NotNull] object pageIndex,
            [NotNull] object pageSize,
            [NotNull] object useStyledNicks,
            [NotNull] object showMoved,
            [CanBeNull] bool findLastRead, 
            [NotNull] bool getTags)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_sincedate", NpgsqlDbType.Timestamp)).Value = sinceDate ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_todate", NpgsqlDbType.Timestamp)).Value = toDate ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_showmoved", NpgsqlDbType.Boolean)).Value = showMoved;
                cmd.Parameters.Add(new NpgsqlParameter("i_findlastread", NpgsqlDbType.Boolean)).Value = findLastRead;
                cmd.Parameters.Add(new NpgsqlParameter("i_gettags", NpgsqlDbType.Boolean)).Value = getTags;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable announcements_list(
            [NotNull] string connectionString,
            [NotNull] object forumID,
            [NotNull] object userId,
            [NotNull] object sinceDate,
            [NotNull] object toDate,
            [NotNull] object pageIndex,
            [NotNull] object pageSize,
            [NotNull] object useStyledNicks,
            [NotNull] object showMoved,
            [CanBeNull] bool findLastRead, 
            [NotNull]bool getTags)
        {
            using (var cmd = PostgreDbAccess.GetCommand("announcements_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_sincedate", NpgsqlDbType.Timestamp)).Value = sinceDate ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_todate", NpgsqlDbType.Timestamp)).Value = toDate ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_showmoved", NpgsqlDbType.Boolean)).Value = showMoved;
                cmd.Parameters.Add(new NpgsqlParameter("i_findlastread", NpgsqlDbType.Boolean)).Value = findLastRead;
                cmd.Parameters.Add(new NpgsqlParameter("i_gettags", NpgsqlDbType.Boolean)).Value = getTags;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Lists topics very simply (for URL rewriting)
        /// </summary>
        /// <param name="StartID"></param>
        /// <param name="Limit"></param>
        /// <returns></returns>
        public static DataTable topic_simplelist([NotNull] string connectionString, int StartID, int Limit)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_simplelist"))
            {
                if (StartID <= 0)
                {
                    StartID = 0;
                }
                if (Limit <= 0)
                {
                    Limit = 500;
                }


                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_startid", NpgsqlDbType.Integer)).Value = StartID;
                cmd.Parameters.Add(new NpgsqlParameter("i_limit", NpgsqlDbType.Integer)).Value = Limit;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void topic_move(
            [NotNull] string connectionString, object topicID, object forumID, object showMoved, object linkDays)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_move"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_showmoved", NpgsqlDbType.Boolean)).Value = showMoved;
                cmd.Parameters.Add(new NpgsqlParameter("i_linkdays", NpgsqlDbType.Integer)).Value = linkDays;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;
                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static DataTable topic_announcements(
            [NotNull] string connectionString, object boardId, object numOfPostsToRetrieve, object pageUserId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_announcements"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_numposts", NpgsqlDbType.Integer)).Value = numOfPostsToRetrieve;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable topic_latest(
            [NotNull] string connectionString,
            object boardID,
            object numOfPostsToRetrieve,
            object pageUserId,
            bool useStyledNicks,
            bool showNoCountPosts,
            bool findLastRead)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_latest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardID;
                cmd.Parameters.Add(new NpgsqlParameter("i_numposts", NpgsqlDbType.Integer)).Value = numOfPostsToRetrieve;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_shownocountposts", NpgsqlDbType.Boolean)).Value =
                    showNoCountPosts;
                cmd.Parameters.Add(new NpgsqlParameter("i_findlastread", NpgsqlDbType.Boolean)).Value = findLastRead;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The rss_topic_latest.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="numOfPostsToRetrieve">
        /// The num of posts to retrieve.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="useStyledNicks">
        /// If true returns string for userID style.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable rss_topic_latest(
            [NotNull] string connectionString,
            object boardId,
            object numOfPostsToRetrieve,
            object pageUserId,
            bool useStyledNicks,
            bool showNoCountPosts)
        {
            using (var cmd = PostgreDbAccess.GetCommand("rss_topic_latest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_numposts", NpgsqlDbType.Integer)).Value = numOfPostsToRetrieve;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_shownocountposts", NpgsqlDbType.Boolean)).Value =
                    showNoCountPosts;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable topic_active(
            [NotNull] string connectionString,
            [NotNull] object boardId,
            [CanBeNull] object categoryId,
            [NotNull] object pageUserId,
            [NotNull] object sinceDate,
            [NotNull] object toDate,
            [NotNull] object pageIndex,
            [NotNull] object pageSize,
            [NotNull] object useStyledNicks,
            [CanBeNull] bool findLastRead)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_active"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryId;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                cmd.Parameters.Add(new NpgsqlParameter("i_sincedate", NpgsqlDbType.Timestamp)).Value = sinceDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_todate", NpgsqlDbType.Timestamp)).Value = toDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_findlastread", NpgsqlDbType.Boolean)).Value = findLastRead;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        private static void topic_deleteAttachments([NotNull] string connectionString, object topicID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_listmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;

                using (DataTable dt = PostgreDbAccess.GetData(cmd, connectionString))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        message_deleteRecursively(
                            connectionString, row["MessageID"], true, string.Empty, 0, true, false);
                    }
                }
            }
        }


        private static void topic_deleteimages([NotNull] string connectionString, int topicID)
        {

            string uploadDir = HostingEnvironment.MapPath(String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads, "/", YafBoardFolders.Current.Topics));

            try
            {
                string topicImage = string.Empty;
                var dt = topic_info(
                 connectionString, topicID, false);
                if (dt != null)
                {
                    topicImage = dt["TopicImage"].ToString();
                }

                string fileName = string.Format("{0}/{1}.{2}.yafupload", uploadDir, topicID, topicImage);
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }
                string fileNameThumb = string.Format("{0}/{1}.thumb.{2}.yafupload", uploadDir, topicID, topicImage);
                if (System.IO.File.Exists(fileNameThumb))
                {
                    System.IO.File.Delete(fileNameThumb);
                }
            }
            catch
            {
                // error deleting that file... 
            }
        }
        public static void topic_delete([NotNull] string connectionString, object topicID)
        {
            topic_delete(connectionString, topicID, false);
        }

        public static void topic_delete([NotNull] string connectionString, object topicID, object eraseTopic)
        {
            if (eraseTopic == null)
            {
                eraseTopic = false;
            }
           

            if (eraseTopic.ToType<bool>())
            {
                topic_deleteAttachments(connectionString, topicID);

                topic_deleteimages(connectionString, (int)topicID);
            }

            using (var cmd = PostgreDbAccess.GetCommand("topic_delete"))
            {
               
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;
                cmd.Parameters.Add(new NpgsqlParameter("i_updatelastpost", NpgsqlDbType.Boolean)).Value = true;
                cmd.Parameters.Add(new NpgsqlParameter("i_erasetopic", NpgsqlDbType.Boolean)).Value = eraseTopic.ToType<bool>();

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static DataTable topic_findprev([NotNull] string connectionString, object topicID)
        {
            DataTable dt;
            DataRow dr;
            using (var cmd = PostgreDbAccess.GetCommand("topic_findprev"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;

                dt = PostgreDbAccess.GetData(cmd, connectionString);
                dr = dt.Rows[0];
                if (dt.Rows[0][0] == DBNull.Value)
                {
                    return new DataTable();
                }

                return dt;
            }
        }

        public static DataTable topic_findnext([NotNull] string connectionString, object topicID)
        {
            DataTable dt;
            DataRow dr;
            using (var cmd = PostgreDbAccess.GetCommand("topic_findnext"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;

                dt = PostgreDbAccess.GetData(cmd, connectionString);
                dr = dt.Rows[0];
                if (dt.Rows[0][0] == DBNull.Value)
                {

                    return new DataTable();
                }

                return dt;
            }
        }

        public static void topic_lock([NotNull] string connectionString, object topicID, object locked)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_lock"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;
                cmd.Parameters.Add(new NpgsqlParameter("i_locked", NpgsqlDbType.Boolean)).Value = locked;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static long topic_save(
            [NotNull] string connectionString,
            object forumID,
            object subject,
            object status,
            object styles,
            object description,
            object message,
             [CanBeNull] object messageDescription,
            object userId,
            object priority,
            object userName,
            object ip,
            object posted,
            object blogPostID,
            object flags,
            out long messageID,
            string tags)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_subject", NpgsqlDbType.Varchar)).Value = subject;
                cmd.Parameters.Add(new NpgsqlParameter("i_status", NpgsqlDbType.Varchar)).Value = status;
                cmd.Parameters.Add(new NpgsqlParameter("i_styles", NpgsqlDbType.Varchar)).Value = styles;
                cmd.Parameters.Add(new NpgsqlParameter("i_description", NpgsqlDbType.Varchar)).Value = description;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_message", NpgsqlDbType.Text)).Value = message;
                cmd.Parameters.Add(new NpgsqlParameter("i_priority", NpgsqlDbType.Smallint)).Value = priority;
                cmd.Parameters.Add(new NpgsqlParameter("i_username", NpgsqlDbType.Varchar)).Value = userName;
                cmd.Parameters.Add(new NpgsqlParameter("i_ip", NpgsqlDbType.Varchar)).Value = ip;
                cmd.Parameters.Add(new NpgsqlParameter("i_posted", NpgsqlDbType.Timestamp)).Value = posted;
                cmd.Parameters.Add(new NpgsqlParameter("i_blogpostid", NpgsqlDbType.Varchar)).Value = blogPostID;
                cmd.Parameters.Add(new NpgsqlParameter("i_flags", NpgsqlDbType.Integer)).Value = flags;
                cmd.Parameters.Add(new NpgsqlParameter("i_messagedescription", NpgsqlDbType.Varchar)).Value = messageDescription;
                cmd.Parameters.Add(new NpgsqlParameter("i_tags", NpgsqlDbType.Text)).Value = tags;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                DataTable dt = PostgreDbAccess.GetData(cmd, connectionString);
                messageID = long.Parse(dt.Rows[0]["MessageID"].ToString());
                return long.Parse(dt.Rows[0]["TopicID"].ToString());
            }
        }

        public static DataRow topic_info([NotNull] string connectionString, object topicID, [NotNull] bool getTags)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_info"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_showdeleted", NpgsqlDbType.Boolean)).Value = false;
                cmd.Parameters.Add(new NpgsqlParameter("i_gettags", NpgsqlDbType.Boolean)).Value = getTags;

                using (var dt = PostgreDbAccess.GetData(cmd, connectionString))
                {
                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
        }

        public static void topic_imagesave([NotNull] string connectionString, object topicID, [NotNull] object imageUrl, Stream stream, object avatarImageType)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_imagesave"))
            {
                byte[] data = null;
                if (stream != null)
                {
                    data = new byte[stream.Length];
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    stream.Read(data, 0, (int)stream.Length);
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;
                cmd.Parameters.Add(new NpgsqlParameter("i_imageurl", NpgsqlDbType.Varchar)).Value = imageUrl;
                cmd.Parameters.Add(new NpgsqlParameter("i_stream", NpgsqlDbType.Bytea)).Value = data;
                cmd.Parameters.Add(new NpgsqlParameter("i_avatarimagetype", NpgsqlDbType.Varchar)).Value = avatarImageType;
                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }
      
        public static int topic_findduplicate([NotNull] string connectionString, object topicName)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_findduplicate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_topicname", NpgsqlDbType.Varchar)).Value = topicName;
                return (int)PostgreDbAccess.ExecuteScalar(cmd, connectionString);
            }
        }

        public static DataTable topic_favorite_details(
            [NotNull] string connectionString,
            [NotNull] object boardId,
            [CanBeNull] object categoryId,
            [NotNull] object pageUserId,
            [NotNull] object sinceDate,
            [NotNull] object toDate,
            [NotNull] object pageIndex,
            [NotNull] object pageSize,
            [NotNull] object useStyledNicks,
            [CanBeNull] bool findLastRead)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_favorite_details"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryId;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                cmd.Parameters.Add(new NpgsqlParameter("i_sincedate", NpgsqlDbType.Timestamp)).Value = sinceDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_todate", NpgsqlDbType.Timestamp)).Value = toDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_findlastread", NpgsqlDbType.Boolean)).Value = findLastRead;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The topic_favorite_list.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable topic_favorite_list([NotNull] string connectionString, object userID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_favorite_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The topic_favorite_remove.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        public static void topic_favorite_remove([NotNull] string connectionString, object userID, object topicID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_favorite_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;
                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// The topic_favorite_add.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        public static void topic_favorite_add([NotNull] string connectionString, object userID, object topicID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_favorite_add"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// The topic_unanswered
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        ///  </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the Active Topics
        /// </returns>
        public static DataTable topic_unanswered(
            [NotNull] string connectionString,
            [NotNull] object boardId,
            [CanBeNull] object categoryId,
            [NotNull] object pageUserId,
            [NotNull] object sinceDate,
            [NotNull] object toDate,
            [NotNull] object pageIndex,
            [NotNull] object pageSize,
            [NotNull] object useStyledNicks,
            [CanBeNull] bool findLastRead)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_unanswered"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryId;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                cmd.Parameters.Add(new NpgsqlParameter("i_sincedate", NpgsqlDbType.Timestamp)).Value = sinceDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_todate", NpgsqlDbType.Timestamp)).Value = toDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_findlastread", NpgsqlDbType.Boolean)).Value = findLastRead;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable topic_unread(
            [NotNull] string connectionString,
            [NotNull] object boardId,
            [CanBeNull] object categoryId,
            [NotNull] object pageUserId,
            [NotNull] object sinceDate,
            [NotNull] object toDate,
            [NotNull] object pageIndex,
            [NotNull] object pageSize,
            [NotNull] object useStyledNicks,
            [CanBeNull] bool findLastRead)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topic_unread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryId;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                cmd.Parameters.Add(new NpgsqlParameter("i_sincedate", NpgsqlDbType.Timestamp)).Value = sinceDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_todate", NpgsqlDbType.Timestamp)).Value = toDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_findlastread", NpgsqlDbType.Boolean)).Value = findLastRead;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Gets all topics where the pageUserid has posted
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        ///  </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param> 
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the User Topics
        /// </returns>
        public static DataTable Topics_ByUser(
            [NotNull] string connectionString,
            [NotNull] object boardId,
            [CanBeNull] object categoryId,
            [NotNull] object pageUserId,
            [NotNull] object sinceDate,
            [NotNull] object toDate,
            [NotNull] object pageIndex,
            [NotNull] object pageSize,
            [NotNull] object useStyledNicks,
            [CanBeNull] bool findLastRead)
        {
            using (var cmd = PostgreDbAccess.GetCommand("topics_byuser"))
            {
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_categoryid", NpgsqlDbType.Integer)).Value = categoryId;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                cmd.Parameters.Add(new NpgsqlParameter("i_sincedate", NpgsqlDbType.Timestamp)).Value = sinceDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_todate", NpgsqlDbType.Timestamp)).Value = toDate;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_findlastread", NpgsqlDbType.Boolean)).Value = findLastRead;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Delete a topic status.
        /// </summary>
        /// <param name="topicStatusID">The topic status ID.</param>
        public static void TopicStatus_Delete([NotNull] string connectionString, [NotNull] object topicStatusID)
        {
            try
            {
                using (var cmd = PostgreDbAccess.GetCommand("TopicStatus_Delete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("i_TopicStatusID", NpgsqlDbType.Integer).Value = topicStatusID;
                    PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
                }
            }
            catch
            {
                // Ignore any errors in this method
            }
        }

        /// <summary>
        /// Get a Topic Status by topicStatusID
        /// </summary>
        /// <param name="topicStatusID">The topic status ID.</param>
        /// <returns></returns>
        public static DataTable TopicStatus_Edit([NotNull] string connectionString, [NotNull] object topicStatusID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("TopicStatus_Edit"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_TopicStatusID", NpgsqlDbType.Integer).Value = topicStatusID;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// List all Topics of the Current Board
        /// </summary>
        /// <param name="boardID">The board ID.</param>
        /// <returns></returns>
        public static DataTable TopicStatus_List([NotNull] string connectionString, [NotNull] object boardID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("TopicStatus_List"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", NpgsqlDbType.Integer).Value = boardID;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Saves a topic status
        /// </summary>
        /// <param name="topicStatusID">The topic status ID.</param>
        /// <param name="boardID">The board ID.</param>
        /// <param name="topicStatusName">Name of the topic status.</param>
        /// <param name="defaultDescription">The default description.</param>
        public static void TopicStatus_Save(
            [NotNull] string connectionString,
            [NotNull] object topicStatusID,
            [NotNull] object boardID,
            [NotNull] object topicStatusName,
            [NotNull] object defaultDescription)
        {
            try
            {
                using (var cmd = PostgreDbAccess.GetCommand("TopicStatus_Save"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("i_TopicStatusID", NpgsqlDbType.Integer).Value = topicStatusID;
                    cmd.Parameters.Add("i_BoardID", NpgsqlDbType.Integer).Value = boardID;
                    cmd.Parameters.Add("i_TopicStatusName", NpgsqlDbType.Varchar).Value = topicStatusName;
                    cmd.Parameters.Add("i_DefaultDescription", NpgsqlDbType.Varchar).Value = defaultDescription;

                    PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
                }
            }
            catch
            {
                // Ignore any errors in this method
            }
        }

        #endregion

        #region yaf_ReplaceWords

        // rico : replace words / begin
        /// <summary>
        /// Gets a list of replace words
        /// </summary>
        /// <returns>DataTable with replace words</returns>
        public static DataTable replace_words_list([NotNull] string connectionString, object boardId, object id)
        {
            using (var cmd = PostgreDbAccess.GetCommand("replace_words_list"))
            {
                if (id == null)
                {
                    id = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_id", NpgsqlDbType.Integer)).Value = id;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Saves changs to a words
        /// </summary>
        /// <param name="id">ID of bad/good word</param>
        /// <param name="badword">bad word</param>
        /// <param name="goodword">good word</param>
        public static void replace_words_save(
            [NotNull] string connectionString, object boardId, object id, object badword, object goodword)
        {
            using (var cmd = PostgreDbAccess.GetCommand("replace_words_save"))
            {
                if (id == null)
                {
                    id = DBNull.Value;
                }
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_id", NpgsqlDbType.Integer)).Value = id;
                cmd.Parameters.Add(new NpgsqlParameter("i_badword", NpgsqlDbType.Varchar)).Value = badword;
                cmd.Parameters.Add(new NpgsqlParameter("i_goodword", NpgsqlDbType.Varchar)).Value = goodword;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Deletes a bad/good word
        /// </summary>
        /// <param name="ID">ID of bad/good word to delete</param>
        public static void replace_words_delete([NotNull] string connectionString, object id)
        {
            using (var cmd = PostgreDbAccess.GetCommand("replace_words_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_id", NpgsqlDbType.Integer)).Value = id;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_IgnoreUser

        public static void user_addignoreduser([NotNull] string connectionString, object userId, object ignoredUserId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_addignoreduser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_ignoreduserid", NpgsqlDbType.Integer)).Value = ignoredUserId;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void user_removeignoreduser([NotNull] string connectionString, object userId, object ignoredUserId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_removeignoreduser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_ignoreduserid", NpgsqlDbType.Integer)).Value = ignoredUserId;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static bool user_isuserignored([NotNull] string connectionString, object userId, object ignoredUserId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_isuserignored"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_ignoreduserid", NpgsqlDbType.Integer)).Value = ignoredUserId;
                cmd.Parameters.Add("result", NpgsqlDbType.Boolean);
                cmd.Parameters["result"].Direction = ParameterDirection.ReturnValue;

                return Convert.ToBoolean(PostgreDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        public static DataTable user_ignoredlist([NotNull] string connectionString, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_ignoredlist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_User

        /// <summary>
        /// To return a rather rarely updated active user data
        /// </summary>
        /// <param name="userID">The UserID. It is always should have a positive > 0 value.</param>
        /// <param name="styledNicks">If styles should be returned.</param>
        /// <returns>A DataRow, it should never return a null value.</returns>
        public static DataRow user_lazydata(
            [NotNull] string connectionString,
            object userID,
            object boardID,
            bool showPendingMails,
            bool showPendingBuddies,
            bool showUnreadPMs,
            bool showUserAlbums,
            bool styledNicks)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_lazydata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardID;
                cmd.Parameters.Add(new NpgsqlParameter("i_showpendingmails", NpgsqlDbType.Boolean)).Value =
                    showPendingMails;
                cmd.Parameters.Add(new NpgsqlParameter("i_showpendingbuddies", NpgsqlDbType.Boolean)).Value =
                    showPendingBuddies;
                cmd.Parameters.Add(new NpgsqlParameter("i_showunreadpms", NpgsqlDbType.Boolean)).Value = showUnreadPMs;
                cmd.Parameters.Add(new NpgsqlParameter("i_showuseralbums", NpgsqlDbType.Boolean)).Value = showUserAlbums;
                cmd.Parameters.Add(new NpgsqlParameter("i_showuserstyle", NpgsqlDbType.Boolean)).Value = styledNicks;
                return PostgreDbAccess.GetData(cmd, connectionString).Rows[0];
            }
        }

        /// <summary>
        /// The user_list.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="approved">
        /// The approved.
        /// </param>
        /// <param name="groupID">
        /// The group id.
        /// </param>
        /// <param name="rankID">
        /// The rank id.
        /// </param>
        /// <param name="useStyledNicks">
        /// Return style info.
        /// </param> 
        /// <returns>
        /// </returns>
        public static DataTable user_list(
            [NotNull] string connectionString,
            object boardId,
            object userId,
            object approved,
            object groupID,
            object rankID,
            object useStyledNicks)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId
                                                                                                   ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_approved", NpgsqlDbType.Boolean)).Value = approved
                                                                                                    ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_groupid", NpgsqlDbType.Integer)).Value = groupID
                                                                                                   ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_rankid", NpgsqlDbType.Integer)).Value = rankID ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks
                                                                                                       ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The user_pagedlist.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="approved">
        /// The approved.
        /// </param>
        /// <param name="groupID">
        /// The group id.
        /// </param>
        /// <param name="rankID">
        /// The rank id.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use styled nicks.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable user_pagedlist(
            [NotNull] string connectionString,
            object boardId,
            object userId,
            object approved,
            object groupID,
            object rankID,
            object useStyledNicks,
            object pageIndex,
            object pageSize)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_pagedlist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId
                                                                                                   ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_approved", NpgsqlDbType.Boolean)).Value = approved
                                                                                                    ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_groupid", NpgsqlDbType.Integer)).Value = groupID
                                                                                                   ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_rankid", NpgsqlDbType.Integer)).Value = rankID ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks ?? DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize; 
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }
        
        

        /// <summary>
        /// The user_list20members.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="approved">
        /// The approved.
        /// </param>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <param name="rankId">
        /// The rank id.
        /// </param>
        /// <param name="useStyledNicks">
        /// Return style info.
        /// </param>
        /// <param name="lastUserId">
        /// The last user Id.
        /// </param>
        /// <param name="literals">
        /// The literals.
        /// </param>
        /// <param name="exclude">
        /// The exclude.
        /// </param>
        /// <param name="beginsWith">
        /// The begins with.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable user_listmembers(
            [NotNull] string connectionString,
            object boardId,
            object userId,
            object approved,
            object groupId,
            object rankId,
            object useStyledNicks,
            object lastUserId,
            object literals,
            object exclude,
            object beginsWith,
            object pageIndex,
            object pageSize,
            object sortName,
            object sortRank,
            object sortJoined,
            object sortPosts,
            object sortLastVisit,
            object numPosts,
            object numPostCompare)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_listmembers"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_approved", NpgsqlDbType.Boolean)).Value = approved;
                cmd.Parameters.Add(new NpgsqlParameter("i_groupid", NpgsqlDbType.Integer)).Value = groupId;
                cmd.Parameters.Add(new NpgsqlParameter("i_rankid", NpgsqlDbType.Integer)).Value = rankId;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;
                cmd.Parameters.Add(new NpgsqlParameter("i_literals", NpgsqlDbType.Varchar)).Value = literals.ToString()
                                                                                                    != "\0"
                                                                                                    && literals.ToString
                                                                                                           ().IsSet()
                                                                                                        ? literals
                                                                                                        : string.Empty;
                cmd.Parameters.Add(new NpgsqlParameter("i_exclude", NpgsqlDbType.Boolean)).Value = exclude;
                cmd.Parameters.Add(new NpgsqlParameter("i_beginswith", NpgsqlDbType.Boolean)).Value = beginsWith;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageindex", NpgsqlDbType.Integer)).Value = pageIndex;
                cmd.Parameters.Add(new NpgsqlParameter("i_pagesize", NpgsqlDbType.Integer)).Value = pageSize;
                cmd.Parameters.Add(new NpgsqlParameter("i_sortname", NpgsqlDbType.Integer)).Value =
                    sortName.ToType<int>();
                cmd.Parameters.Add(new NpgsqlParameter("i_sortrank", NpgsqlDbType.Integer)).Value =
                    sortRank.ToType<int>();
                cmd.Parameters.Add(new NpgsqlParameter("i_sortjoined", NpgsqlDbType.Integer)).Value =
                    sortJoined.ToType<int>();
                cmd.Parameters.Add(new NpgsqlParameter("i_sortposts", NpgsqlDbType.Integer)).Value =
                    sortPosts.ToType<int>();
                cmd.Parameters.Add(new NpgsqlParameter("i_sortlastvisit", NpgsqlDbType.Integer)).Value =
                    sortLastVisit.ToType<int>();
                cmd.Parameters.Add(new NpgsqlParameter("i_numposts", NpgsqlDbType.Integer)).Value =
                    numPosts.ToType<int>();
                cmd.Parameters.Add(new NpgsqlParameter("i_numpostcopmate", NpgsqlDbType.Integer)).Value =
                    numPostCompare.ToType<int>();

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }     

        public static void user_delete([NotNull] string connectionString, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void user_setrole([NotNull] string connectionString, int boardId, object providerUserKey, object role)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_setrole"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_provideruserkey", NpgsqlDbType.Varchar)).Value =
                    providerUserKey;
                cmd.Parameters.Add(new NpgsqlParameter("i_role", NpgsqlDbType.Varchar)).Value = role;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        // TODO: is not used anywhere? 
        public static void user_setinfo([NotNull] string connectionString, int boardId, System.Web.Security.MembershipUser user)
        {
            using (
                var cmd =
                    PostgreDbAccess.GetCommand(
                        "update {databaseOwner}.{objectQualifier}User set Name=i_UserName,Email=i_Email where BoardID=i_BoardID and ProviderUserKey=i_ProviderUserKey",
                        true))
            {
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new NpgsqlParameter("i_UserName", NpgsqlDbType.Varchar)).Value = user.UserName;
                cmd.Parameters.Add(new NpgsqlParameter("i_Email", NpgsqlDbType.Varchar)).Value = user.Email;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_ProviderUserKey", NpgsqlDbType.Varchar)).Value =
                    user.ProviderUserKey;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }            

        public static void user_migrate(
            [NotNull] string connectionString, object userId, object providerUserKey, object updateProvider)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_migrate"))
            {
                if (providerUserKey == null)
                {
                    providerUserKey = DBNull.Value;
                }
                if (updateProvider == null)
                {
                    updateProvider = DBNull.Value;
                }
                //if (date == null) { date = DBNull.Value; }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_provideruserkey", NpgsqlDbType.Varchar)).Value =
                    providerUserKey;
                cmd.Parameters.Add(new NpgsqlParameter("i_updateprovider", NpgsqlDbType.Boolean)).Value = updateProvider;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void user_deleteold([NotNull] string connectionString, object boardId, object days)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_deleteold"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_days", NpgsqlDbType.Integer)).Value = days;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void user_approve([NotNull] string connectionString, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_approve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void user_approveall([NotNull] string connectionString, object boardId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_approveall"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Returns data about allowed signature tags and character limits
        /// </summary>
        /// <param name="userID">The userID</param>
        /// <param name="boardID">The boardID</param>
        /// <returns>Data Table</returns>
        public static DataTable user_getsignaturedata([NotNull] string connectionString, object userID, object boardID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_getsignaturedata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardID;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Returns data about albums: allowed number of images and albums
        /// </summary>
        /// <param name="userID">The userID</param>
        /// <param name="boardID">The boardID</param>  
        public static DataTable user_getalbumsdata([NotNull] string connectionString, object userID, object boardID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_getalbumsdata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardID;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                DataTable dt = PostgreDbAccess.GetData(cmd, connectionString);
                return dt;
            }
        }

        public static bool user_changepassword(
            [NotNull] string connectionString, object userId, object oldPassword, object newPassword)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_changepassword"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_oldpassword", NpgsqlDbType.Varchar)).Value = oldPassword;
                cmd.Parameters.Add(new NpgsqlParameter("i_newpassword", NpgsqlDbType.Varchar)).Value = newPassword;

                return (bool)PostgreDbAccess.ExecuteScalar(cmd, connectionString);
            }
        }

        public static DataTable user_pmcount([NotNull] string connectionString, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_pmcount"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Checks if the User has replied tho the specifc topic.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// Returns if true or not
        /// </returns>
        public static bool user_RepliedTopic(
            [NotNull] string connectionString, [NotNull] object messageId, [NotNull] object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_repliedtopic"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_messageid", NpgsqlDbType.Integer)).Value = messageId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                var messageCount = (int)PostgreDbAccess.ExecuteScalar(cmd, connectionString);

                return messageCount > 0;
            }
        }

        public static void user_save(
            [NotNull] string connectionString,
            object userId,
            object boardId,
            object userName,
            object displayName,
            object email,
            object timeZone,
            object languageFile,
            object culture,
            object themeFile,
            object useSingleSignOn,
            object textEditor,
            object overrideDefaultThemes,
            object approved,
            object pmNotification,
            object autoWatchTopics,
            object dSTUser,
            object isHidden,
            object notificationType, 
            object topicsPerPage, 
            object postsPerPage)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_save"))
            {
                if (email == null)
                {
                    email = DBNull.Value;
                }
                if (languageFile == null)
                {
                    languageFile = DBNull.Value;
                }
                if (themeFile == null)
                {
                    themeFile = DBNull.Value;
                }
                if (overrideDefaultThemes == null)
                {
                    overrideDefaultThemes = DBNull.Value;
                }
                if (approved == null)
                {
                    approved = DBNull.Value;
                }
                if (pmNotification == null)
                {
                    pmNotification = DBNull.Value;
                }
                if (culture == null)
                {
                    culture = DBNull.Value;
                }
                if (dSTUser == null)
                {
                    dSTUser = DBNull.Value;
                }
                if (isHidden == null)
                {
                    isHidden = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_username", NpgsqlDbType.Varchar)).Value = userName ?? DBNull.Value; 
                cmd.Parameters.Add(new NpgsqlParameter("i_displayname", NpgsqlDbType.Varchar)).Value = displayName;
                cmd.Parameters.Add(new NpgsqlParameter("i_email", NpgsqlDbType.Varchar)).Value = email;
                cmd.Parameters.Add(new NpgsqlParameter("i_timezone", NpgsqlDbType.Integer)).Value = timeZone;
                cmd.Parameters.Add(new NpgsqlParameter("i_languagefile", NpgsqlDbType.Varchar)).Value = languageFile;
                cmd.Parameters.Add(new NpgsqlParameter("i_culture", NpgsqlDbType.Varchar)).Value = culture;
                cmd.Parameters.Add(new NpgsqlParameter("i_themefile", NpgsqlDbType.Varchar)).Value = themeFile;
                cmd.Parameters.Add(new NpgsqlParameter("i_usesinglesignon", NpgsqlDbType.Boolean)).Value =
                    useSingleSignOn;
                cmd.Parameters.Add(new NpgsqlParameter("i_texteditor", NpgsqlDbType.Varchar)).Value = textEditor;
                cmd.Parameters.Add(new NpgsqlParameter("i_overridedefaulttheme", NpgsqlDbType.Boolean)).Value =
                    overrideDefaultThemes;
                cmd.Parameters.Add(new NpgsqlParameter("i_approved", NpgsqlDbType.Boolean)).Value = approved;
                cmd.Parameters.Add(new NpgsqlParameter("i_pmnotification", NpgsqlDbType.Boolean)).Value = pmNotification;
                cmd.Parameters.Add(new NpgsqlParameter("i_notificationtype", NpgsqlDbType.Integer)).Value =
                    notificationType;
                cmd.Parameters.Add(new NpgsqlParameter("i_provideruserkey", NpgsqlDbType.Varchar)).Value = DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_autowatchtopics", NpgsqlDbType.Boolean)).Value =
                    autoWatchTopics;
                cmd.Parameters.Add(new NpgsqlParameter("i_dstuser", NpgsqlDbType.Boolean)).Value = dSTUser;
                cmd.Parameters.Add(new NpgsqlParameter("i_hideuser", NpgsqlDbType.Boolean)).Value = isHidden;
                cmd.Parameters.Add(new NpgsqlParameter("i_topicsperpage", NpgsqlDbType.Integer)).Value = topicsPerPage;
                cmd.Parameters.Add(new NpgsqlParameter("i_postsperpage", NpgsqlDbType.Integer)).Value = postsPerPage;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value = DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);

            }
        }

        /// <summary>
        /// Saves the notification type for a user
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        public static void user_savenotification(
            [NotNull] string connectionString,
            object userId,
            object pmNotification,
            object autoWatchTopics,
            object notificationType,
            object dailyDigest)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_savenotification"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_pmnotification", NpgsqlDbType.Boolean)).Value = pmNotification;
                cmd.Parameters.Add(new NpgsqlParameter("i_autowatchtopics", NpgsqlDbType.Boolean)).Value =
                    autoWatchTopics;
                cmd.Parameters.Add(new NpgsqlParameter("i_notificationtype", NpgsqlDbType.Integer)).Value =
                    notificationType;
                cmd.Parameters.Add(new NpgsqlParameter("i_dailydigest", NpgsqlDbType.Boolean)).Value = dailyDigest;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void user_adminsave(
            [NotNull] string connectionString,
            object boardId,
            object userId,
            object name,
            object displayName,
            object email,
            object flags,
            object rankID)
        {

            using (var cmd = PostgreDbAccess.GetCommand("user_adminsave"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;
                cmd.Parameters.Add(new NpgsqlParameter("i_name", NpgsqlDbType.Varchar)).Value = name;
                cmd.Parameters.Add(new NpgsqlParameter("i_displayname", NpgsqlDbType.Varchar)).Value = displayName;
                cmd.Parameters.Add(new NpgsqlParameter("i_email", NpgsqlDbType.Varchar)).Value = email;
                cmd.Parameters.Add(new NpgsqlParameter("i_flags", NpgsqlDbType.Integer)).Value = flags;
                cmd.Parameters.Add(new NpgsqlParameter("i_rankid", NpgsqlDbType.Integer)).Value = rankID;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static DataTable user_emails([NotNull] string connectionString, object boardId, object groupID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_emails"))
            {

                if (groupID == null)
                {
                    groupID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_groupid", NpgsqlDbType.Integer)).Value = groupID;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable user_accessmasks([NotNull] string connectionString, object boardId, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_accessmasks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                return userforumaccess_sort_list(PostgreDbAccess.GetData(cmd, connectionString), 0, 0, 0);
            }
        }

        public static DataTable user_accessmasksbyforum([NotNull] string connectionString, object boardId, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_accessmasksbyforum"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable user_accessmasksbygroup([NotNull] string connectionString, object boardId, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_accessmasksbygroup"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        //adds some convenience while editing group's access rights (indent forums)
        private static DataTable userforumaccess_sort_list(
            DataTable listSource, int parentID, int categoryID, int startingIndent)
        {

            DataTable listDestination = new DataTable();

            listDestination.Columns.Add("ForumID", typeof(String));
            listDestination.Columns.Add("ForumName", typeof(String));
            //it is uset in two different procedures with different tables, 
            //so, we must add correct columns
            if (listSource.Columns.IndexOf("AccessMaskName") >= 0) listDestination.Columns.Add("AccessMaskName", typeof(String));
            else
            {
                listDestination.Columns.Add("BoardName", typeof(String));
                listDestination.Columns.Add("CategoryName", typeof(String));
                listDestination.Columns.Add("AccessMaskId", typeof(Int32));
            }
            DataView dv = listSource.DefaultView;
            userforumaccess_sort_list_recursive(dv.ToTable(), listDestination, parentID, categoryID, startingIndent);
            return listDestination;
        }

        private static void userforumaccess_sort_list_recursive(
            DataTable listSource, DataTable listDestination, int parentID, int categoryID, int currentIndent)
        {
            DataRow newRow;

            foreach (DataRow row in listSource.Rows)
            {
                // see if this is a root-forum
                if (row["ParentID"] == DBNull.Value) row["ParentID"] = 0;

                if ((int)row["ParentID"] == parentID)
                {
                    string sIndent = string.Empty;

                    for (int j = 0; j < currentIndent; j++) sIndent += "--";

                    // import the row into the destination
                    newRow = listDestination.NewRow();

                    newRow["ForumID"] = row["ForumID"];
                    newRow["ForumName"] = string.Format("{0} {1}", sIndent, row["ForumName"]);
                    if (listDestination.Columns.IndexOf("AccessMaskName") >= 0) newRow["AccessMaskName"] = row["AccessMaskName"];
                    else
                    {
                        newRow["BoardName"] = row["BoardName"];
                        newRow["CategoryName"] = row["CategoryName"];
                        newRow["AccessMaskId"] = row["AccessMaskId"];
                    }


                    listDestination.Rows.Add(newRow);

                    // recurse through the list..
                    userforumaccess_sort_list_recursive(
                        listSource, listDestination, (int)row["ForumID"], categoryID, currentIndent + 1);
                }
            }
        }
      
        public static DataTable user_avatarimage([NotNull] string connectionString, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_avatarimage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static int user_get([NotNull] string connectionString, int boardId, object providerUserKey)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_get"))
            {
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_provideruserkey", NpgsqlDbType.Varchar)).Value =
                    providerUserKey;

                return PostgreDbAccess.ExecuteScalar(cmd, connectionString).ToType<int>();
                //return PostgreDbAccess.GetData(cmd,connectionString);
            }
        }
        
        public static string user_getsignature([NotNull] string connectionString, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_getsignature"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                return PostgreDbAccess.ExecuteScalar(cmd, connectionString).ToString();
            }
        }  

        public static void user_deleteavatar([NotNull] string connectionString, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_deleteavatar"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

       public static int user_aspnet(
            [NotNull] string connectionString,
            int boardId,
            string userName,
            string displayName,
            string email,
            object providerUserKey,
            object isApproved)
        {
            try
            {
                using (var cmd = PostgreDbAccess.GetCommand("user_aspnet"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                    cmd.Parameters.Add(new NpgsqlParameter("i_username", NpgsqlDbType.Varchar)).Value = userName;
                    cmd.Parameters.Add(new NpgsqlParameter("i_displayname", NpgsqlDbType.Varchar)).Value = displayName;
                    // ?? userName;                    
                    cmd.Parameters.Add(new NpgsqlParameter("i_email", NpgsqlDbType.Varchar)).Value = email;
                    cmd.Parameters.Add(new NpgsqlParameter("i_provideruserkey", NpgsqlDbType.Varchar)).Value =
                        providerUserKey;
                    cmd.Parameters.Add(new NpgsqlParameter("i_isapproved", NpgsqlDbType.Boolean)).Value = isApproved;
                    cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                        DateTime.UtcNow;

                    return (int)PostgreDbAccess.ExecuteScalar(cmd, connectionString);
                }
            }
            catch (Exception x)
            {
                Db.eventlog_create(null, "user_aspnet in VZF.Classes.Data.Db.cs", x, EventLogTypes.Error);
                return 0;
            }
        }

        /// <summary>
        /// The user_guest.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <returns>
        /// The user_guest.
        /// </returns>
        public static int? user_guest([NotNull] string connectionString, object boardId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_guest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;

                return Convert.ToInt32(PostgreDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        public static DataTable user_activity_rank(
            [NotNull] string connectionString, object boardId, object startDate, object displayNumber)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_activity_rank"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_displaynumber", NpgsqlDbType.Integer)).Value = displayNumber;
                cmd.Parameters.Add(new NpgsqlParameter("i_startdate", NpgsqlDbType.Timestamp)).Value = startDate;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        public static int user_nntp(
            [NotNull] string connectionString, object boardId, object userName, object email, int? timeZone)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_nntp"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_username", NpgsqlDbType.Varchar)).Value = userName;
                cmd.Parameters.Add(new NpgsqlParameter("i_email", NpgsqlDbType.Varchar)).Value = email;
                cmd.Parameters.Add(new NpgsqlParameter("i_timezone", NpgsqlDbType.Integer)).Value = timeZone;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                object o = PostgreDbAccess.ExecuteScalar(cmd, connectionString);
                //  if ( o != DBNull.Value)
                //  {
                return Convert.ToInt32(o);
                //  }
                //  else
                //     return -1;

            }
        }

        /// <summary>
        /// Add Reputation Points to the specified user id.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="fromUserID">From user ID.</param>
        /// <param name="points">The points.</param>
        public static void user_addpoints(
            [NotNull] string connectionString, [NotNull] object userID, [CanBeNull] object fromUserID, [NotNull] object points)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_addpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add("i_fromuserid", NpgsqlDbType.Integer).Value = fromUserID;
                cmd.Parameters.Add("i_utctimestamp", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;
                cmd.Parameters.Add(new NpgsqlParameter("i_points", NpgsqlDbType.Integer)).Value = points;


                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }


        /// <summary>
        /// Remove Repuatation Points from the specified user id.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="fromUserID">From user ID.</param>
        /// <param name="points">The points.</param>
        public static void user_removepoints(
            [NotNull] string connectionString, [NotNull] object userID, [CanBeNull] object fromUserID, [NotNull] object points)
        {

            using (var cmd = PostgreDbAccess.GetCommand("user_removepoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add("i_fromuserid", NpgsqlDbType.Integer).Value = fromUserID;
                cmd.Parameters.Add("i_utctimestamp", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;
                cmd.Parameters.Add(new NpgsqlParameter("i_points", NpgsqlDbType.Integer)).Value = points;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static int user_getpoints([NotNull] string connectionString, object userId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_getpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userId;

                return (int)PostgreDbAccess.ExecuteScalar(cmd, connectionString);
            }
        }


        public static int user_getthanks_from([NotNull] string connectionString, object userID, object pageUserId)
        {

            using (var cmd = PostgreDbAccess.GetCommand("user_getthanks_from"))
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                return Convert.ToInt32(PostgreDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        //<summary> Returns the number of times and posts that other users have thanked the 
        // user with the provided userID.
        public static int[] user_getthanks_to([NotNull] string connectionString, object userID, object pageUserId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_getthanks_to"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                NpgsqlParameter paramThanksToNumber = new NpgsqlParameter("i_thankstonumber", NpgsqlDbType.Integer);
                paramThanksToNumber.Direction = ParameterDirection.Output;
                NpgsqlParameter paramThanksToPostsNumber = new NpgsqlParameter(
                    "i_thankstopostsnumber", NpgsqlDbType.Integer);
                paramThanksToPostsNumber.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_pageuserid", NpgsqlDbType.Integer)).Value = pageUserId;
                cmd.Parameters.Add(paramThanksToNumber);
                cmd.Parameters.Add(paramThanksToPostsNumber);
                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);

                int ThanksToPostsNumber, ThanksToNumber;
                if (paramThanksToNumber.Value == DBNull.Value)
                {
                    ThanksToNumber = 0;
                    ThanksToPostsNumber = 0;
                }
                else
                {
                    ThanksToPostsNumber = Convert.ToInt32(paramThanksToPostsNumber.Value);
                    ThanksToNumber = Convert.ToInt32(paramThanksToNumber.Value);
                }
                return new int[] { ThanksToNumber, ThanksToPostsNumber };
            }
        }  

        #endregion    

        /// <summary>
        /// Add Or Update Read Tracking for the Current User and Topic
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        public static void Readtopic_AddOrUpdate(
            [NotNull] string connectionString, [NotNull] object userID, [NotNull] object topicID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("readtopic_addorupdate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;
                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Delete the Read Tracking
        /// </summary>
        /// <param name="trackingID">
        /// The tracking id.
        /// </param>
        /* public static void Readtopic_delete([NotNull] object userID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("readtopic_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                PostgreDbAccess.ExecuteNonQuery(cmd,connectionString);
            }
        } */

        /// <summary>
        /// Get the Global Last Read DateTime User
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="lastVisitDate">
        /// The last Visit Date of the User
        /// </param>
        /// <returns>
        /// Returns the Global Last Read DateTime
        /// </returns>
        public static DateTime? User_LastRead([NotNull] string connectionString, [NotNull] object userID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("user_lastread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;

                var tableLastRead = PostgreDbAccess.ExecuteScalar(cmd, connectionString);

                return tableLastRead.ToType<DateTime?>();
            }
        }

        /// <summary>
        /// Get the Last Read DateTime for the Current Topic and User
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="topicID">
        /// The topic ID.
        /// </param>
        /// <returns>
        /// Returns the Last Read DateTime
        /// </returns>
        public static DateTime? Readtopic_lastread(
            [NotNull] string connectionString, [NotNull] object userID, [NotNull] object topicID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("readtopic_lastread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_topicid", NpgsqlDbType.Integer)).Value = topicID;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                var tableLastRead = PostgreDbAccess.ExecuteScalar(cmd, connectionString);

                return tableLastRead.ToType<DateTime?>();
            }
        }

        /// <summary>
        /// Add Or Update Read Tracking for the forum and Topic
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        public static void ReadForum_AddOrUpdate(
            [NotNull] string connectionString, [NotNull] object userID, [NotNull] object forumID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("readforum_addorupdate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;
                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Delete the Read Tracking
        /// </summary>
        /// <param name="trackingID">
        /// The tracking id.
        /// </param>
        /* public static void ReadForum_delete([NotNull] object userID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("readforum_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                PostgreDbAccess.ExecuteNonQuery(cmd,connectionString);
            }
        } */

        /// <summary>
        /// Get the Last Read DateTime for the Forum and User
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="forumID">
        /// The forum ID.
        /// </param>
        /// <returns>
        /// Returns the Last Read DateTime
        /// </returns>
        public static DateTime? ReadForum_lastread(
            [NotNull] string connectionString, [NotNull] object userID, [NotNull] object forumID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("readforum_lastread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;

                var tableLastRead = PostgreDbAccess.ExecuteScalar(cmd, connectionString);

                return tableLastRead != null && tableLastRead != DBNull.Value
                           ? (DateTime)tableLastRead
                           : DateTimeHelper.SqlDbMinTime();
            }
        }        

        #region Miscelaneous vzrus addons

        #region reindex page controls

        // DB Maintenance page buttons name    

        /// <summary>
        /// Gets a value indicating whether panel get stats.
        /// </summary>
        public static bool PanelGetStats
        {
            get
            {
                return true;
            }
        }

        public static bool PanelRecoveryMode
        {
            get
            {
                return true;
            }
        }

        public static bool PanelReindex
        {
            get
            {
                return true;
            }
        }

        public static bool PanelShrink
        {
            get
            {
                return true;
            }
        }


        public static bool btnReindexVisible
        {
            get
            {
                return true;
            }
        }

        #endregion

        /// <summary>
        /// The rsstopic_list.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="topicStart">
        /// The topic start.
        /// </param>
        /// <param name="topicCount">
        /// The topic count.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable rsstopic_list([NotNull] string connectionString, int forumID, int topicStart, int topicCount)
        {
            using (var cmd = PostgreDbAccess.GetCommand("rsstopic_list"))
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumid", NpgsqlDbType.Integer)).Value = forumID;
                cmd.Parameters.Add(new NpgsqlParameter("i_start", NpgsqlDbType.Integer)).Value = topicStart;
                cmd.Parameters.Add(new NpgsqlParameter("i_count", NpgsqlDbType.Integer)).Value = topicCount;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The get stats message.
        /// </summary>
        private static string getStatsMessage;

        /// <summary>
        /// The db_getstats_new.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_getstats_new([NotNull] string connectionString )
        {
            try
            {
                using (var connMan = new PostgreDbConnectionManager(connectionString))
                {
                    connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(getStats_InfoMessage);
                    using (var cmd = new NpgsqlCommand("VACUUM ANALYZE VERBOSE;", connMan.OpenDBConnection(connectionString)))
                    {
                        cmd.CommandType = CommandType.Text;

                        // up the command timeout..
                        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                        // run it..
                        cmd.ExecuteNonQuery();
                        return getStatsMessage;
                    }
                }
            }
            finally
            {
                getStatsMessage = string.Empty;
            }
        }

        /// <summary>
        /// The reindexDb_InfoMessage.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void getStats_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            getStatsMessage += "\r\n{0}".FormatWith(e.Message);
        }

        /// <summary>
        /// The reindex db message.
        /// </summary>
        private static string reindexDbMessage;

        /// <summary>
        /// The db_reindex_new.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_reindex_new([NotNull] string connectionString )
        {
            // VACUUM ANALYZE VERBOSE;VACUUM cannot be implemeneted within function or multiline command line string 
            try
            {
                using (var connMan = new PostgreDbConnectionManager(connectionString))
                {
                    connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(reindexDb_InfoMessage);

                    using (
                        var cmd =
                            new NpgsqlCommand(
                                String.Format(
                                    @"REINDEX DATABASE {0};",
                                    Config.DatabaseSchemaName.IsSet() ? Config.DatabaseSchemaName : "public"),
                                connMan.OpenDBConnection(connectionString)))
                    {
                        cmd.CommandType = CommandType.Text;
                        
                        // up the command timeout..
                        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                        // run it..                   
                        cmd.ExecuteNonQuery();
                        return reindexDbMessage;
                    }
                }
            }
            finally
            {
                reindexDbMessage = string.Empty;
            }
        }

        /// <summary>
        /// The reindexDb_InfoMessage.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void reindexDb_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            reindexDbMessage += "\r\n{0}".FormatWith(e.Message);
        }


        public static string db_reindex_warning()
        {
            return "Operation completed. Database tables reindexing can take a lot of time.";
        }

        public static string db_getstats_warning()
        {
            return
                "Operation completed. Vacuum operation blocks all database objects! If there is a lot of indexes, the database can be blocked for a long time. Choose a right timing!";
        }

        /// <summary>
        /// The db_runsql.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="connMan">
        /// The conn man.
        /// </param>
        /// <returns>
        /// The db_runsql.
        /// </returns>
        /*  public static string db_runsql(string sql, IDbConnectionManager connMan, bool useTransaction)
        {
            using (var command = new SqlCommand(sql, connMan.OpenDBConnection))
            {
                command.CommandTimeout = 9999;
                command.Connection = connMan.OpenDBConnection;

                return InnerRunSqlExecuteReader(command, useTransaction);
            }
        } */

        /*   public static string db_runsql( string sql, bool useTransaction)
        {
            string txtResult = "";
            var results = new System.Text.StringBuilder();

            using (var cmd = new NpgsqlCommand(sql, connMan.OpenDBConnection))
            {
                cmd.CommandTimeout = 9999;
                NpgsqlDataReader reader = null;

                using (NpgsqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(PostgreDbAccess.IsolationLevel))
                {
                    try
                    {
                        cmd.Connection = connMan.DBConnection;
                        cmd.Transaction = trans;
                        reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            int rowIndex = 1;

                            results.Append("RowNumber");
                            int gg = 0;
                            var columnNames = new string[reader.GetSchemaTable().Rows.Count-1];
                            foreach (DataRow drd in reader.GetSchemaTable().Rows)
                            {
                                  columnNames[gg] = drd["ColumnName"].ToString();
                                  results.Append(",");
                                  results.Append(drd["ColumnName"].ToString());
                                  gg++;
                                
                            }
                         //   var columnNames = reader.GetSchemaTable().Rows.Cast<DataRow>().Select(r => r["ColumnName"].ToString()).ToList();

                            
                           

                            results.AppendLine();

                            while (reader.Read())
                            {
                                results.AppendFormat(@"""{0}""", rowIndex++);

                                // dump all columns..
                                foreach (var col in columnNames)
                                {
                                    results.AppendFormat(@",""{0}""", reader[col].ToString().Replace("\"", "\"\""));
                                }

                                results.AppendLine();
                            }
                        }
                        else if (reader.RecordsAffected > 0)
                        {
                            results.AppendFormat("{0} Record(s) Affected", reader.RecordsAffected);
                            results.AppendLine();
                        }
                        else
                        {
                            results.AppendLine("No Results Returned.");
                        }


                        reader.Close();
                        trans.Commit();
                    }
                    catch (Exception x)
          {
            if (reader != null)
            {
              reader.Close();
            }

            // rollback..
            trans.Rollback();
            results.AppendLine();
            results.AppendFormat("SQL ERROR: {0}", x);
          }

          return results.ToString();
        }
               
            }

        } 
       */

        private static string messageRunSql;

        /// <summary>
        /// The db_runsql.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="connectionManager">
        /// The conn man.
        /// </param>
        /// <param name="useTransaction">
        /// The use Transaction.
        /// </param>
        /// <returns>
        /// The db_runsql.
        /// </returns>
        public static string db_runsql_new([NotNull] string connectionString, [NotNull] string sql, bool useTransaction)
        {

            try
            {
                using (var connMan = new PostgreDbConnectionManager(connectionString))
                {
                    connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(runSql_InfoMessage);
                    sql = PostgreDbAccess.GetCommandTextReplaced(sql.Trim());

                    var results = new System.Text.StringBuilder();

                    using (var cmd = new NpgsqlCommand(sql, connMan.OpenDBConnection(connectionString)))
                    {
                        cmd.CommandTimeout = 9999;
                        NpgsqlDataReader reader = null;
                        var trans = useTransaction ? cmd.Connection.BeginTransaction(PostgreDbAccess.IsolationLevel)
                                                                   : null;
                        try
                        {
                            cmd.Connection = connMan.DBConnection;
                            cmd.Transaction = trans;
                            reader = cmd.ExecuteReader();

                            if (reader.HasRows)
                            {
                                int rowIndex = 1;

                                results.Append("RowNumber");
                                int gg = 0;
                                var columnNames = new string[reader.GetSchemaTable().Rows.Count];
                                foreach (DataRow drd in reader.GetSchemaTable().Rows)
                                {
                                    columnNames[gg] = drd["ColumnName"].ToString();
                                    results.Append(",");
                                    results.Append(drd["ColumnName"]);
                                    gg++;
                                }
                                
                                //   var columnNames = reader.GetSchemaTable().Rows.Cast<DataRow>().Select(r => r["ColumnName"].ToString()).ToList();

                                results.AppendLine();

                                while (reader.Read())
                                {
                                    results.AppendFormat(@"""{0}""", rowIndex++);

                                    // dump all columns..
                                    foreach (var col in columnNames)
                                    {
                                        results.AppendFormat(@",""{0}""", reader[col].ToString().Replace("\"", "\"\""));
                                    }

                                    results.AppendLine();
                                }
                            }
                            else if (reader.RecordsAffected > 0)
                            {
                                results.AppendFormat("{0} Record(s) Affected", reader.RecordsAffected);
                                results.AppendLine();
                            }
                            else
                            {
                                if (messageRunSql.IsSet())
                                {
                                    results.AppendLine(messageRunSql);
                                    results.AppendLine();
                                }
                                results.AppendLine("No Results Returned.");
                            }


                            reader.Close();
                            trans.Commit();
                        }
                        catch (Exception x)
                        {
                            if (reader != null)
                            {
                                reader.Close();
                            }

                            // rollback..
                            trans.Rollback();
                            results.AppendLine();
                            results.AppendFormat("SQL ERROR: {0}", x);
                        }

                        return results.ToString();


                    }
                }
            }
            finally
            {
                messageRunSql = string.Empty;
            }


        }

        /// <summary>
        /// The runSql_InfoMessage.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void runSql_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            messageRunSql = "\r\n" + e.Message;
        }      
     

        public static readonly string[] _scriptList =
            {
                "pgsql/preinstall.sql", "pgsql/domains.sql", "pgsql/tables.sql",
                "pgsql/sequences.sql", "pgsql/pkeys.sql", "pgsql/indexes.sql",
                "pgsql/fkeys.sql", "pgsql/triggers.sql", "pgsql/rules.sql",
                "pgsql/views.sql", "pgsql/types.sql", "pgsql/procedures.sql",
                "pgsql/procedures1.sql", "pgsql/functions.sql",
                "pgsql/providers/tables.sql", "pgsql/providers/pkeys.sql",
                "pgsql/providers/indexes.sql", "pgsql/providers/types.sql",
                "pgsql/providers/procedures.sql", "pgsql/postinstall.sql",
                "pgsql/nestedsets.sql", "pgsql/nestedsets_sp.sql",
                "pgsql/fulltext_ru.sql"
            };

        public static string[] ScriptList
        {
            get
            {
                return _scriptList;
            }
        }

        private static bool GetBooleanRegistryValue([NotNull] string connectionString, string name)
        {
            using (DataTable dt = Db.registry_list(connectionString, name))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int i;
                    return int.TryParse(dr["Value"].ToString(), out i)
                               ? Convert.ToBoolean(i)
                               : Convert.ToBoolean(dr["Value"]);
                }
            }
            return false;
        }

        public static void system_deleteinstallobjects([NotNull] string connectionString )
        {
            string tSQL = "DROP FUNCTION" + PostgreDbAccess.GetObjectName("system_initialize");
            using (var cmd = PostgreDbAccess.GetCommand(tSQL, true))
            {
                cmd.CommandType = CommandType.Text;
                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void system_initialize_executescripts(
            [NotNull] string connectionString, string script, string scriptFile, bool useTransactions)
        {
            script = PostgreDbAccess.GetCommandTextReplaced(script);


            //Scripts separation regexp
            var statements = System.Text.RegularExpressions.Regex.Split(script, "(?:--GO)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            using (var connMan = new PostgreDbConnectionManager(connectionString))
            {              
                // use transactions..
                if (useTransactions)
                {
                    using (NpgsqlTransaction trans = connMan.OpenDBConnection(connectionString).BeginTransaction(PostgreDbAccess.IsolationLevel))
                    {
                        foreach (var sql0 in statements)
                        {
                            string sql = sql0.Trim();

                            try
                            {
                                if (sql.ToLower().IndexOf("setuser", System.StringComparison.Ordinal) >= 0)
                                {
                                    continue;
                                }

                                if (sql.Length > 0)
                                {
                                    using (var cmd = new NpgsqlCommand())
                                    {
                                        cmd.Transaction = trans;
                                        cmd.Connection = connMan.DBConnection;
                                        cmd.CommandType = CommandType.Text;
                                        cmd.CommandText = sql.Trim();

                                        // added so command won't timeout anymore..
                                        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            catch (Exception x)
                            {
                                sql = sql0;
                                trans.Rollback();
                                throw new Exception(
                                    String.Format(
                                        "FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}", scriptFile, sql, x.Message));
                            }
                        }

                        trans.Commit();
                    }
                }
                else
                {
                    // don't use transactions
                    foreach (string sql in statements.Select(sql0 => sql0.Trim()))
                    {
                        try
                        {
                            if (sql.ToLower().IndexOf("setuser", System.StringComparison.Ordinal) >= 0)
                            {
                                continue;
                            }

                            if (sql.Length > 0)
                            {
                                using (var cmd = new NpgsqlCommand())
                                {
                                    cmd.Connection = connMan.OpenDBConnection(connectionString);
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = sql.Trim();
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (Exception x)
                        {
                            throw new Exception(
                                String.Format(
                                    "FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}", scriptFile, sql, x.Message));
                        }
                    }
                }
            }


        }

        public static void system_initialize_fixaccess([NotNull] string connectionString, bool bGrant)
        {
            /*   using (VZF.Classes.Data.IDbConnectionManager connMan = new IDbConnectionManager())
            {
                using (IDbTransaction trans = connMan.OpenDBConnection.BeginTransaction(VZF.Classes.Data.PostgreDbAccess.IsolationLevel))
                {
                    // select * from  pg_tables where schemaname tableowner
                  //  select * from  pg_views where schemaname viewowner
                    //  select * from  pg_proc where proname like {objectQualifier} prowner(oid)
                   // select * from pg_user usesysoid <-oid
                    // REVIEW : Ederon - would "{databaseOwner}.{objectQualifier}" work, might need only "{objectQualifier}"
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter("select Name, OBJECTPROPERTY(id, N'IsUserTable') AS IsUserTable,IsScalarFunction = OBJECTPROPERTY(id, N'IsScalarFunction'),IsProcedure = OBJECTPROPERTY(id, N'IsProcedure'),IsView = OBJECTPROPERTY(id, N'IsView') from dbo.sysobjects where Name like '{databaseOwner}.{objectQualifier}%'", connMan.OpenDBConnection))
                    {
                        da.SelectCommand.Transaction = trans;
                        using (DataTable dt = new DataTable("sysobjects"))
                        {
                            da.Fill(dt);
                            using (var cmd = connMan.DBConnection.CreateCommand())
                            {
                                cmd.Transaction = trans;
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "select current_user";
                                string userName = (string)cmd.ExecuteScalar();

                                if (bGrant)
                                {
                                    cmd.CommandType = CommandType.Text;
                                    foreach (DataRow row in dt.Select("IsProcedure=1 or IsScalarFunction=1"))
                                    {
                                        cmd.CommandText = string.Format("grant execute on \"{0}\" to \"{1}\"", row["Name"], userName);
                                        cmd.ExecuteNonQuery();
                                    }
                                    foreach (DataRow row in dt.Select("IsUserTable=1 or IsView=1"))
                                    {
                                        cmd.CommandText = string.Format("grant select,update on \"{0}\" to \"{1}\"", row["Name"], userName);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    cmd.CommandText = "sp_changeobjectowner";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    foreach (DataRow row in dt.Select("IsUserTable=1"))
                                    {
                                        cmd.Parameters.Clear();
                                        cmd.Parameters.Add(new NpgsqlParameter("i_objname", NpgsqlDbType.Varchar));
                                        cmd.Parameters[0].Value = row["Name"];
                                        cmd.Parameters.Add(new NpgsqlParameter("i_newowner", NpgsqlDbType.Varchar));
                                        cmd.Parameters[1].Value = PostgreDbAccess.SchemaName;                                        
                                        
                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                        }
                                        catch (NpgsqlException)
                                        {
                                        }
                                    }
                                    foreach (DataRow row in dt.Select("IsView=1"))
                                    {
                                        cmd.Parameters.Clear();
                                      
                                        cmd.Parameters.Add(new NpgsqlParameter("i_objname", NpgsqlDbType.Varchar));
                                        cmd.Parameters[0].Value = row["Name"];
                                        cmd.Parameters.Add(new NpgsqlParameter("i_newowner", NpgsqlDbType.Varchar));
                                        cmd.Parameters[1].Value = PostgreDbAccess.SchemaName;  
                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                        }
                                        catch (NpgsqlException)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                    trans.Commit();
                }
            }*/

        }

        public static void system_initialize(
            [NotNull] string connectionString,
            string forumName,
            string timeZone,
            string culture,
            string languageFile,
            string forumEmail,
            string smtpServer,
            string userName,
            string userEmail,
            object providerUserKey,
            string rolePrefix)
        {
            using (var cmd = PostgreDbAccess.GetCommand("system_initialize"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_name", NpgsqlDbType.Varchar)).Value = forumName;
                cmd.Parameters.Add(new NpgsqlParameter("i_timezone", NpgsqlDbType.Integer)).Value = timeZone;
                cmd.Parameters.Add(new NpgsqlParameter("i_languagefile", NpgsqlDbType.Varchar)).Value = languageFile;
                cmd.Parameters.Add(new NpgsqlParameter("i_culture", NpgsqlDbType.Varchar)).Value = culture;
                cmd.Parameters.Add(new NpgsqlParameter("i_forumemail", NpgsqlDbType.Varchar)).Value = forumEmail;
                cmd.Parameters.Add(new NpgsqlParameter("i_smtpserver", NpgsqlDbType.Varchar)).Value = smtpServer;
                cmd.Parameters.Add(new NpgsqlParameter("i_user", NpgsqlDbType.Varchar)).Value = userName;
                // vzrus:The input parameter should be implemented in the system initialize and board_create procedures, else there will be an error in create watch because the user email is missing
                cmd.Parameters.Add(new NpgsqlParameter("i_useremail", NpgsqlDbType.Varchar)).Value = userEmail;
                cmd.Parameters.Add(new NpgsqlParameter("i_userkey", NpgsqlDbType.Uuid)).Value = providerUserKey;
                cmd.Parameters.Add(new NpgsqlParameter("i_newboardguid", NpgsqlDbType.Uuid)).Value = Guid.NewGuid();
                cmd.Parameters.Add(new NpgsqlParameter("i_roleprefix", NpgsqlDbType.Varchar)).Value = rolePrefix;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);

            }

        }

        public static void system_updateversion([NotNull] string connectionString, int version, string name)
        {
            using (var cmd = PostgreDbAccess.GetCommand("system_updateversion"))
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_version", NpgsqlDbType.Integer)).Value = version;
                cmd.Parameters.Add(new NpgsqlParameter("i_versionname", NpgsqlDbType.Varchar)).Value = name;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Returns info about all Groups and Rank styles. 
        /// Used in GroupRankStyles cache.
        /// Usage: LegendID = 1 - Select Groups, LegendID = 2 - select Ranks by Name 
        /// </summary>
        public static DataTable group_rank_style([NotNull] string connectionString, object boardID)
        {
            using (var cmd = PostgreDbAccess.GetCommand("group_rank_style"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardID;
                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        #endregion

        #region DLESKTECH_ShoutBox

        /// <summary>
        /// The shoutbox_getmessages.
        /// </summary>
        /// <param name="numberOfMessages">
        /// The number of messages.
        /// </param>
        /// <param name="useStyledNicks">
        /// Use style for user nicks in ShoutBox.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable shoutbox_getmessages(
            [NotNull] string connectionString, int boardId, int numberOfMessages, object useStyledNicks)
        {
            using (var cmd = PostgreDbAccess.GetCommand("shoutbox_getmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_numberofmessages", NpgsqlDbType.Integer)).Value =
                    numberOfMessages;
                cmd.Parameters.Add(new NpgsqlParameter("i_stylednicks", NpgsqlDbType.Boolean)).Value = useStyledNicks;

                return PostgreDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The shoutbox_savemessage.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool shoutbox_savemessage(
            [NotNull] string connectionString, int boardId, string message, string userName, int userID, object ip)
        {
            using (var cmd = PostgreDbAccess.GetCommand("shoutbox_savemessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new NpgsqlParameter("i_userid", NpgsqlDbType.Integer)).Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("i_username", NpgsqlDbType.Varchar)).Value = userName;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_message", NpgsqlDbType.Text)).Value = message;
                cmd.Parameters.Add(new NpgsqlParameter("i_date", NpgsqlDbType.Timestamp)).Value = DBNull.Value;
                cmd.Parameters.Add(new NpgsqlParameter("i_ip", NpgsqlDbType.Varchar)).Value = ip;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;

                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);

                return true;
            }
        }

        public static Boolean shoutbox_clearmessages([NotNull] string connectionString, int boardId)
        {
            using (var cmd = PostgreDbAccess.GetCommand("shoutbox_clearmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("i_boardid", NpgsqlDbType.Integer)).Value = boardId;
                cmd.Parameters.Add(new NpgsqlParameter("i_utctimestamp", NpgsqlDbType.Timestamp)).Value =
                    DateTime.UtcNow;
                PostgreDbAccess.ExecuteNonQuery(cmd, connectionString);
                return true;
            }
        }

        #endregion

        #region Touradg Mods

        // Shinking Operation

        /// <summary>
        /// The db_shrink_warning.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_shrink_warning([NotNull] string connectionString )
        {
            return string.Empty;
        }

        /// <summary>
        /// The db_shrink.
        /// </summary>
        public static void db_shrink()
        {
            /*  String ShrinkSql = "DBCC SHRINKDATABASE(N'" + DBName.DBConnection.Database + "')";
            SqlConnection ShrinkConn = new SqlConnection(VZF.Classes.Config.ConnectionString);
            SqlCommand ShrinkCmd = new SqlCommand(ShrinkSql, ShrinkConn);
            ShrinkConn.Open();
            ShrinkCmd.ExecuteNonQuery();
            ShrinkConn.Close();
            using (SqlCommand cmd = new SqlCommand(ShrinkSql.ToString(), DBName.OpenDBConnection))
            {
                cmd.Connection = DBName.DBConnection;
                cmd.CommandTimeout = 9999;
                cmd.ExecuteNonQuery();
            }*/
        }

        /// <summary>
        /// The db shink message.
        /// </summary>
        private static string dbShinkMessage;

        /// <summary>
        /// The db_shrink.
        /// </summary>
        /// <param name="DBName">
        /// The db name.
        /// </param>
        public static string db_shrink_new([NotNull] string connectionString )
        {
            /* try
             {
                 using (var conn = new PostgreDbConnectionManager(connectionString))
                 {
                     conn.InfoMessage += new YafDBConnInfoMessageEventHandler(dbShink_InfoMessage);
                   
                     string ShrinkSql = "DBCC SHRINKDATABASE(N'" + conn.DBConnection.Database + "')";
                     var ShrinkConn = new SqlConnection(Config.ConnectionString);
                     var ShrinkCmd = new SqlCommand(ShrinkSql, ShrinkConn);
                     ShrinkConn.Open();
                     ShrinkCmd.ExecuteNonQuery();
                     ShrinkConn.Close();
                     using (var cmd = new SqlCommand(ShrinkSql, conn.OpenDBConnection(connectionString)))
                     {
                         cmd.Connection = conn.DBConnection;
                         cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                         cmd.ExecuteNonQuery();
                     }
                 }
                 return dbShinkMessage;
             }
             finally
             {
                 dbShinkMessage = string.Empty;
             } */
            return string.Empty;
        }

        /// <summary>
        /// The runSql_InfoMessage.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void dbShink_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            dbShinkMessage = "\r\n" + e.Message;
        }

        /// <summary>
        /// The db_recovery_mode_warning.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_recovery_mode_warning()
        {
            return string.Empty;
        }

        /// <summary>
        /// The db_recovery_mode_new.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="dbRecoveryMode">
        /// The db recovery mode.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_recovery_mode_new([NotNull] string connectionString, string dbRecoveryMode)
        {
            /* String RecoveryMode = "ALTER DATABASE " + DBName.DBConnection.Database + " SET RECOVERY " + dbRecoveryMode;
            SqlConnection RecoveryModeConn = new SqlConnection(VZF.Classes.Config.ConnectionString);
            SqlCommand RecoveryModeCmd = new SqlCommand(RecoveryMode, RecoveryModeConn);
            RecoveryModeConn.Open();
            RecoveryModeCmd.ExecuteNonQuery();
            RecoveryModeConn.Close();
            using (SqlCommand cmd = new SqlCommand(RecoveryMode.ToString(), DBName.OpenDBConnection))
            {
                cmd.Connection = DBName.DBConnection;
                cmd.CommandTimeout = 9999;
                cmd.ExecuteNonQuery();
            }*/
            return string.Empty;
        }

        #endregion        
    }
}
