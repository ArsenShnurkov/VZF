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
//   The MySQL Db Access.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace VZF.Data.Mysql
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

    using MySql.Data.MySqlClient;

    using VZF.Utils;
    using VZF.Utils.Helpers;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Handlers;
    using YAF.Types.Objects;
    using VZF.Data.Common;

    /// <summary>
    /// The Db.
    /// </summary>
    public static class Db
    {
        // added by vzrus
        #region ConnectionStringOptions

        /// <summary>
        /// Gets the provider assembly name.
        /// </summary>
        public static string ProviderAssemblyName
        {
            get
            {
                return "MySql.Data.MySqlClient";
            }
        }

        public static bool PasswordPlaceholderVisible
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Common

        /// <summary>
        /// The db size old.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DbSizeOld([NotNull] string connectionString)
        {
            string version;
            using (var cmd1 = new MySqlCommand("SELECT VERSION()"))
            {
                cmd1.CommandType = CommandType.Text;
                version = MySqlDbAccess.ExecuteScalar(cmd1, false, connectionString).ToString();
            }

            var sb = new StringBuilder();
            sb.Append("SELECT s.schema_name, ");
            sb.Append(
                "(SELECT COUNT(table_name) FROM  INFORMATION_SCHEMA.SCHEMATA s LEFT JOIN INFORMATION_SCHEMA.TABLES t ON s.schema_name = t.table_schema WHERE t.engine ='InnoDB'  AND t.TABLE_TYPE='BASE TABLE') total_tables, ");
            sb.Append(
                "(SELECT COUNT(table_name) FROM  INFORMATION_SCHEMA.SCHEMATA s LEFT JOIN INFORMATION_SCHEMA.TABLES t ON s.schema_name = t.table_schema WHERE t.TABLE_TYPE='VIEW') total_views, ");
            sb.Append(
                "(SELECT COUNT(ROUTINE_NAME) FROM INFORMATION_SCHEMA.SCHEMATA s LEFT JOIN INFORMATION_SCHEMA.ROUTINES r ON s.schema_name = r.ROUTINE_SCHEMA WHERE r.ROUTINE_TYPE='PROCEDURE') total_procedures, ");
            sb.Append(
                "(SELECT COUNT(ROUTINE_NAME) FROM INFORMATION_SCHEMA.SCHEMATA s LEFT JOIN INFORMATION_SCHEMA.ROUTINES r ON s.schema_name = r.ROUTINE_SCHEMA WHERE r.ROUTINE_TYPE='FUNCTION') total_functions, ");
            sb.Append("CAST(CONCAT(IFNULL(ROUND((SUM(t.data_length)+SUM(t.index_length))/1024/1024,2),0.00),");
            sb.Append("'Mb'");
            sb.Append(") AS CHAR) total_size, ");
            sb.Append("CAST(CONCAT(IFNULL(ROUND(SUM(t.index_length)/1024/1024,2),0.00),");
            sb.Append("'Mb'");
            sb.Append(") AS CHAR) index_size, ");
            sb.Append(
                " CAST(CONCAT(IFNULL(ROUND(((SUM(t.data_length)+SUM(t.index_length))-SUM(t.data_free))/1024/1024,2),0.00),");
            sb.Append("'Mb'");
            sb.Append(") AS CHAR) data_used, ");
            sb.Append(" CAST(CONCAT(IFNULL(ROUND(SUM(data_free)/1024/1024,2),0.00),");
            sb.Append("'Mb'");
            sb.Append(") AS CHAR) data_free, ");
            if (version.Contains("5.1"))
            {
                sb.Append(
                    " CAST(CONCAT(IFNULL(ROUND((((SUM(t.data_length)+SUM(t.index_length))-SUM(t.data_free))/((SUM(t.data_length)+SUM(t.index_length)))*100),2),0.00),");
                sb.Append(@"'Mb'");
                sb.Append(") AS CHAR) pct_used ");
            }
            else
            {
                sb.Append(
                    " CAST(CONCAT(IFNULL(ROUND((((SUM(t.data_length)+SUM(t.index_length))-SUM(t.data_free))/((SUM(t.data_length)+SUM(t.index_length)))*100),2),0.00),");
                sb.Append(@"'%'");
                sb.Append(") AS CHAR) pct_used ");
            }

            sb.Append(
                "FROM INFORMATION_SCHEMA.SCHEMATA s LEFT JOIN INFORMATION_SCHEMA.TABLES t ON s.schema_name = t.table_schema WHERE t.engine = ");
            sb.Append("'InnoDB' ");
            sb.Append("GROUP BY s.schema_name ORDER BY pct_used DESC");
            sb.Append(";");
            using (var cmd = new MySqlCommand(sb.ToString()))
            {
                var sb1 = new StringBuilder();
                cmd.CommandType = CommandType.Text;
                DataTable dt = MySqlDbAccess.GetData(cmd, connectionString);
                int cnt = dt.Columns.Count - 1;
                DataRow dr = dt.Rows[0];

                for (int i = 0; i <= cnt; i++)
                {
                    sb1.Append(dt.Columns[i].ColumnName);
                    sb1.Append(" = ");
                    sb1.Append(dr[i]);
                    sb1.Append(" | ");
                }

                return sb1.ToString();
            }
        }

        
        // MySQL InnoDB engine currently don't support fulltext....

        /// <summary>
        /// The _full text supported.
        /// </summary>
        private static bool _fullTextSupported;

        /// <summary>
        /// Gets a value indicating whether full text supported.
        /// </summary>
        public static bool FullTextSupported
        {
            get
            {
                return _fullTextSupported;
            }
        }

        /// <summary>
        /// The _full text script.
        /// </summary>
        private const string _fullTextScript = "mysql/fulltext.sql";

        /// <summary>
        /// Gets the full text script.
        /// </summary>
        public static string FullTextScript
        {
            get
            {
                return _fullTextScript;
            }
        }

        /// <summary>
        /// The _script list.
        /// </summary>
        private static readonly string[] _scriptList =
            {
                "mysql/tables.sql", "mysql/pkeys.sql", "mysql/indexes.sql",
                "mysql/fkeys.sql", "mysql/constraints.sql",
                "mysql/triggers.sql", "mysql/types.sql", "mysql/views.sql",
                "mysql/procedures.sql", "mysql/functions.sql",
                "mysql/providers/tables.sql", "mysql/providers/pkeys.sql",
                "mysql/providers/indexes.sql", "mysql/providers/procedures.sql"
            };

        /// <summary>
        /// Gets the script list.
        /// </summary>
        public static string[] ScriptList
        {
            get
            {
                return _scriptList;
            }
        }

        /// <summary>
        /// The get boolean registry value.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool GetBooleanRegistryValue([NotNull] string connectionString, string name)
        {
            using (DataTable dt = Db.registry_list(connectionString, name, DBNull.Value))
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

        #endregion

        #region Forum

        /// <summary>
        /// Lists all forums accessible to a user sorted by order.
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
        /// <param name="forumidExclusions">
        /// The forumid exclusions.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty first row.
        /// </param>
        /// <param name="startAt">
        /// The start at.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> of all accessible forums.
        /// </returns>
        public static DataTable forum_listall_sorted(
            [NotNull] string connectionString,
            object boardId,
            object userId,
            int[] forumidExclusions,
            bool emptyFirstRow,
            List<int> startAt)
        {
            using (var dataTable = forum_listall(connectionString, boardId, userId, startAt, false))
            {
                int baseForumId = 0;
                int baseCategoryId = 0;

                if (startAt.Any())
                {
                    // find the base ids...
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        if (Convert.ToInt32(dataRow["ForumID"]) == startAt.First(f => f > -1))
                        {
                            baseForumId = Convert.ToInt32(dataRow["ParentID"]);
                            baseCategoryId = Convert.ToInt32(dataRow["CategoryID"]);
                            break;
                        }
                    }
                }

                return forum_sort_list(
                    connectionString, dataTable, baseForumId, baseCategoryId, 0, forumidExclusions, emptyFirstRow);
            }
        }

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
            using (var cmd = MySqlDbAccess.GetCommand("recent_users"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_BoardID", MySqlDbType.Int32).Value = boardID;
                cmd.Parameters.Add("I_TimeSinceLastLogin", MySqlDbType.Int32).Value = timeSinceLastLogin;
                cmd.Parameters.Add("I_StyledNicks", MySqlDbType.Byte).Value = styledNicks;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }
        
        /// <summary>
        /// The pageload.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <param name="boardId">
        /// The boardId.
        /// </param>
        /// <param name="userKey">
        /// The user key.
        /// </param>
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <param name="location">
        /// The location.
        /// </param>
        /// <param name="forumPage">
        /// The forum page name.   
        /// </param>
        /// <param name="browser">
        /// The browser.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="isCrawler">
        /// The browser is a crawler.
        /// </param>
        /// <param name="isMobileDevice">
        /// The browser is a mobile device.
        /// </param> 
        /// <param name="donttrack">
        /// The donttrack.
        /// </param> 
        /// <returns>
        /// Common User Info DataRow
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public static DataRow pageload(
            [NotNull] string connectionString,
            object sessionId,
            object boardId,
            object userKey,
            object ip,
            object location,
            object forumPage,
            object browser,
            object platform,
            object categoryId,
            object forumId,
            object topicId,
            object messageId,
            object isCrawler,
            object isMobileDevice,
            object donttrack)
        {
            int nTries = 0;
            while (true)
            {
                try
                {
                    // var dd = MySqlDbAccess.GetConnectionParams();
                    //  DataTable dt1=null;                   

                    using (var cmd = MySqlDbAccess.GetCommand("pageload"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("i_SessionID", MySqlDbType.VarChar).Value = sessionId;
                        cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                        cmd.Parameters.Add("i_UserKey", MySqlDbType.VarChar).Value = userKey ?? DBNull.Value;
                        cmd.Parameters.Add("i_IP", MySqlDbType.VarChar).Value = ip;
                        cmd.Parameters.Add("i_Location", MySqlDbType.VarChar, 128).Value = location;
                        cmd.Parameters.Add("i_ForumPage", MySqlDbType.VarChar, 128).Value = forumPage;
                        cmd.Parameters.Add("i_Browser", MySqlDbType.VarChar).Value = browser;
                        cmd.Parameters.Add("i_Platform", MySqlDbType.VarChar).Value = platform;
                        cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                        cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumId ?? DBNull.Value;
                        cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicId ?? DBNull.Value;
                        cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageId ?? DBNull.Value;
                        cmd.Parameters.Add("i_IsCrawler", MySqlDbType.Byte).Value = isCrawler ?? 0;
                        cmd.Parameters.Add("i_IsMobileDevice", MySqlDbType.Byte).Value = isMobileDevice ?? 0;
                        cmd.Parameters.Add("i_DontTrack", MySqlDbType.Byte).Value = donttrack ?? 0;
                        cmd.Parameters.Add("i_CurrentTime", MySqlDbType.Timestamp).Value = DateTime.UtcNow;

                        using (DataTable dt = MySqlDbAccess.GetData(cmd, connectionString))
                        {
                            if (dt.Rows.Count > 0)
                            {
                                return dt.Rows[0];
                            }
                            else
                            {
                                return null;
                            }
                        }

                        //We don't trigger AcceptChanges() as we need to return more 
                        /*
                                                dt1 = MySqlDbAccess.GetDataTableFromReader( cmd, false, false, connectionString);
                        */
                    }
                    //int firstColumnIndex = dt1.Columns.Count;
                    /*
                                        if (dt1.Columns.Count == 0) throw new ArgumentOutOfRangeException();
                                        using ( var cmd1 = MySqlDbAccess.GetCommand( "vaccess_combo" ))
                                        { 
                                            cmd1.CommandType = CommandType.StoredProcedure;                    
                                            cmd1.Parameters.Add( "i_UserID", MySqlDbType.Int32 ).Value = dt1.Rows[0]["UserID"];
                                            cmd1.Parameters.Add( "i_ForumID", MySqlDbType.Int32 ).Value = dt1.Rows[0]["ForumID"];
                                            //We  trigger AcceptChanges() right now as we don't need to return more rows
                                            return MySqlDbAccess.AddValuesToDataTableFromReader(cmd1, dt1, false, true, dt1.Columns.Count,connectionString).Rows[0];                        
                                        }
                    */

                }
                catch (ArgumentOutOfRangeException xx)
                {
                    if (nTries < 3)
                    {
                        /// Transaction (Process ID XXX) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
                    }
                    else
                        throw new ArgumentOutOfRangeException(
                            string.Format(
                                "Number of DataTable columns from DataReader cannot be null. Trys -{0}", nTries),
                            xx);
                }
                catch (MySqlException x)
                {
                    if (x.Number == 1213 && nTries < 3)
                    {
                        /// Transaction (Process ID XXX) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
                    }
                    else
                        throw new ApplicationException(
                            string.Format("Sql Exception with error number {0} (Tries={1})", x.Number, nTries), x);
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
        public static DataSet ds_forumadmin(
            [NotNull] string connectionString, object boardId, object pageUserID, object isUserForum)
        {
            using (var connMan = new MySqlDbConnectionManager(connectionString))
            {
                using (var ds = new DataSet())
                {
                    using (
                        MySqlTransaction trans =
                            connMan.OpenDBConnection(connectionString).BeginTransaction(MySqlDbAccess.IsolationLevel))
                    {
                        using (
                            var da = new MySqlDataAdapter(
                                MySqlDbAccess.GetObjectName("category_list"), new MySqlConnection(connectionString)))
                        {
                            da.SelectCommand.Transaction = trans;
                            da.SelectCommand.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                            da.SelectCommand.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = DBNull.Value;
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.Fill(ds, MySqlDbAccess.GetObjectName("Category"));
                            da.SelectCommand.Parameters.RemoveAt("i_CategoryID");
                            da.SelectCommand.CommandText = MySqlDbAccess.GetObjectName("forum_list");
                            da.SelectCommand.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = DBNull.Value;
                            da.SelectCommand.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = pageUserID;
                            da.SelectCommand.Parameters.Add("i_IsUserForum", MySqlDbType.Byte).Value = isUserForum;
                            da.Fill(ds, MySqlDbAccess.GetObjectName("ForumUnsorted"));

                            DataTable dtForumListSorted =
                                ds.Tables[MySqlDbAccess.GetObjectName("ForumUnsorted")].Clone();
                            dtForumListSorted.TableName = MySqlDbAccess.GetObjectName("Forum");
                            ds.Tables.Add(dtForumListSorted);
                            dtForumListSorted.Dispose();
                            Db.forum_list_sort_basic(
                                ds.Tables[MySqlDbAccess.GetObjectName("ForumUnsorted")],
                                ds.Tables[MySqlDbAccess.GetObjectName("Forum")],
                                0,
                                0);
                            ds.Tables.Remove(MySqlDbAccess.GetObjectName("ForumUnsorted"));
                            ds.Relations.Add(
                                "FK_Forum_Category",
                                ds.Tables[MySqlDbAccess.GetObjectName("Category")].Columns["CategoryID"],
                                ds.Tables[MySqlDbAccess.GetObjectName("Forum")].Columns["CategoryID"]);
                            trans.Commit();
                        }

                        return ds;
                    }
                }
            }
        }

        #endregion                                        

       
        public static void eventlog_create(
            [NotNull] string connectionString, object userID, object source, object description, object type)
        {
            try
            {
                if (userID == null) userID = DBNull.Value;
                using (var cmd = MySqlDbAccess.GetCommand("eventlog_create"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                    cmd.Parameters.Add("i_Source", MySqlDbType.VarChar).Value = source.ToString();
                    cmd.Parameters.Add("i_Description", MySqlDbType.Text).Value = description.ToString();
                    cmd.Parameters.Add("i_Type", MySqlDbType.Int32).Value = type;
                    cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;
                    MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
        /// <param name="choiceID">Choice of the vote</param>
        public static DataTable pollvote_check([NotNull] string connectionString, object pollid, object userid, object remoteip)
        {
            using (var cmd = MySqlDbAccess.GetCommand("pollvote_check"))
            {
                if (userid == null)
                {
                    userid = DBNull.Value;
                }
                if (remoteip == null)
                {
                    remoteip = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_PollID", MySqlDbType.Int32).Value = pollid;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userid;
                cmd.Parameters.Add("i_RemoteIP", MySqlDbType.Int32).Value = remoteip;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

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
            using (var cmd = MySqlDbAccess.GetCommand("pollgroup_votecheck"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_PollGroupID", MySqlDbType.Int32).Value = pollGroupId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("i_RemoteIP", MySqlDbType.Int32).Value = remoteIp;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_Forum

        /// <summary>
        /// List of categories accessible for an active user
        /// </summary>
        /// <param name="boardId">The board id.</param>
        /// <param name="userId">The user Id.</param>
        /// <returns>A <see cref="T:System.Data.DataTable"/> of categories.</returns>
        public static DataTable forum_categoryaccess_activeuser([NotNull] string connectionString, object boardId, object userId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_categoryaccess_activeuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

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
            using (var cmd = MySqlDbAccess.GetCommand("forum_ns_getchildren_anyuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new MySqlParameter("i_boardid", MySqlDbType.Int32)).Value = boardid;
                cmd.Parameters.Add(new MySqlParameter("i_categoryid", MySqlDbType.Int32)).Value = categoryid;
                cmd.Parameters.Add(new MySqlParameter("i_forumid", MySqlDbType.Int32)).Value = forumid;
                cmd.Parameters.Add(new MySqlParameter("i_userid", MySqlDbType.Int32)).Value = userid;
                cmd.Parameters.Add(new MySqlParameter("i_notincluded", MySqlDbType.Byte)).Value = notincluded;
                cmd.Parameters.Add(new MySqlParameter("i_immediateonly", MySqlDbType.Byte)).Value = immediateonly;

                DataTable dt = MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("forum_ns_getchildren"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new MySqlParameter("i_boardid", MySqlDbType.Int32)).Value = boardid;
                cmd.Parameters.Add(new MySqlParameter("i_categoryid", MySqlDbType.Int32)).Value = categoryid;
                cmd.Parameters.Add(new MySqlParameter("i_forumid", MySqlDbType.Int32)).Value = forumid;
                cmd.Parameters.Add(new MySqlParameter("i_notincluded", MySqlDbType.Byte)).Value = notincluded;
                cmd.Parameters.Add(new MySqlParameter("i_immediateonly", MySqlDbType.Byte)).Value = immediateonly;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("forum_ns_getchildren_activeuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new MySqlParameter("i_boardid", MySqlDbType.Int32)).Value = boardid;
                cmd.Parameters.Add(new MySqlParameter("i_categoryid", MySqlDbType.Int32)).Value = categoryid;
                cmd.Parameters.Add(new MySqlParameter("i_forumid", MySqlDbType.Int32)).Value = forumid;
                cmd.Parameters.Add(new MySqlParameter("i_userid", MySqlDbType.Int32)).Value = userid;
                cmd.Parameters.Add(new MySqlParameter("i_notincluded", MySqlDbType.Byte)).Value = notincluded;
                cmd.Parameters.Add(new MySqlParameter("i_immediateonly", MySqlDbType.Byte)).Value = immediateonly;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Deletes attachments out of a entire forum
        /// </summary>
        /// <param name="ForumID">
        /// ID of forum to delete all attachemnts out of
        /// </param>
        private static void forum_deleteAttachments([NotNull] string connectionString, object forumID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_listtopics"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;
                DataTable dt = MySqlDbAccess.GetData(cmd, connectionString);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row != null && row["TopicID"] != DBNull.Value)
                        {
                            topic_delete(connectionString, row["TopicID"], true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="ForumID">forum to delete</param>
        /// <returns>bool to indicate that forum has been deleted</returns>
        public static bool forum_delete([NotNull] string connectionString, object forumID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_listSubForums"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;

                if (!(MySqlDbAccess.ExecuteScalar(cmd, connectionString) is DBNull))
                {
                    return false;
                }

                forum_deleteAttachments(connectionString, forumID);
                using (var cmd_new = MySqlDbAccess.GetCommand("forum_delete"))
                {
                    cmd_new.CommandType = CommandType.StoredProcedure;
                    cmd_new.CommandTimeout = 99999;
                    cmd_new.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;
                    MySqlDbAccess.ExecuteNonQuery(cmd_new, connectionString);
                }
                return true;
            }
        }

        /// <summary>
        /// The forum_tags.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
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
        public static DataTable forum_tags(
            [NotNull] string connectionString,
            int boardId,
            int pageUserId,
            int forumId,
            int pageIndex,
            int pageSize,
            string searchText,
            bool beginsWith)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_tags"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_boardid", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_pageuserid", MySqlDbType.Int32).Value = pageUserId;
                cmd.Parameters.Add("i_forumid", MySqlDbType.Int32).Value = forumId;
                cmd.Parameters.Add("i_pageindex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_pagesize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("i_searchtext", MySqlDbType.VarChar).Value = searchText;
                cmd.Parameters.Add("i_beginswith", MySqlDbType.Byte).Value = beginsWith;

                return MySqlDbAccess.GetData(cmd, true, connectionString);
            }
        }

        public static bool forum_move([NotNull] string connectionString, [NotNull] object forumOldID, [NotNull] object forumNewID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_listSubForums"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumOldID;

                if (!(MySqlDbAccess.ExecuteScalar(cmd, connectionString) is DBNull))
                {
                    return false;
                }

                using (var cmd_new = MySqlDbAccess.GetCommand("forum_move"))
                {
                    cmd_new.CommandType = CommandType.StoredProcedure;
                    cmd_new.CommandTimeout = 99999;
                    cmd.Parameters.Add("i_ForumOldID", MySqlDbType.Int32).Value = forumOldID;
                    cmd.Parameters.Add("i_ForumNewID", MySqlDbType.Int32).Value = forumNewID;

                    MySqlDbAccess.ExecuteNonQuery(cmd_new, connectionString);
                }

                return true;
            }
        }

        /// <summary>
        /// Lists all moderated forums for a user
        /// </summary>
        /// <param name="boardId">board if of moderators</param>
        /// <param name="userID">user id</param>
        /// <returns>DataTable of moderated forums</returns>
        public static DataTable forum_listallMyModerated([NotNull] string connectionString, object boardId, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_listallmymoderated"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("forum_list"))
            {
                if (forumID == null)
                {
                    forumID = DBNull.Value;
                }
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = DBNull.Value;
                cmd.Parameters.Add("i_IsUserForum", MySqlDbType.Byte).Value = false;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }   

        /// <summary>
        /// Gets a max id of forums.
        /// </summary>
        /// <param name="boardID">
        /// boardID
        /// </param>
        /// <returns>
        /// A max forum id for a board
        /// </returns>
        public static int forum_maxid([NotNull] string connectionString, [NotNull] object boardID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_maxid"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardID;
                return Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        public static IEnumerable<TypedForumListAll> ForumListAll([NotNull] string connectionString, int boardId, int userId)
        {
            return
                forum_listall(connectionString, boardId, userId, 0, false)
                    .AsEnumerable()
                    .Select(r => new TypedForumListAll(r));
        }

        /// <summary>
        /// Lists all forums accessible to a user
        /// </summary>
        /// <param name="boardId">BoardID</param>
        /// <param name="userID">ID of user</param>
        /// <param name="startAt">startAt ID</param>
        /// <returns>DataTable of all accessible forums</returns>
        public static DataTable forum_listall(
            [NotNull] string connectionString, object boardId, object userID, object startAt, bool returnAll)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_listall"))
            {
                if (startAt == null)
                {
                    startAt = 0;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_Root", MySqlDbType.Int32).Value = startAt;
                cmd.Parameters.Add("i_ReturnAll", MySqlDbType.Byte).Value = returnAll;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
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

            while (true)
            {
                var ids = tempForumIds;
                var temp = allForums.Where(f => ids.Contains(f.ParentID ?? 0));

                if (!temp.Any())
                {
                    break;
                }

                // replace temp forum ids with these...
                tempForumIds = temp.Select(f => f.ForumID ?? 0).Distinct().ToList();

                // add them...
                forumIds.AddRange(tempForumIds);
            }

            // return filtered forums...
            return allForums.Where(f => forumIds.Contains(f.ForumID ?? 0)).Distinct();
        }

        /// <summary>
        /// Lists forums very simply (for URL rewriting)
        /// </summary>
        /// <param name="StartID"></param>
        /// <param name="Limit"></param>
        /// <returns></returns>
        public static DataTable forum_simplelist([NotNull] string connectionString, int StartID, int Limit)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_simplelist"))
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

                cmd.Parameters.Add("i_StartID", MySqlDbType.Int32).Value = StartID;
                cmd.Parameters.Add("i_Limit", MySqlDbType.Int32).Value = Limit;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable forum_sort_list(
            [NotNull] string connectionString,
            DataTable listSource,
            int parentID,
            int categoryID,
            int startingIndent,
            int[] forumidExclusions,
            bool emptyFirstRow)
        {
            DataTable listDestination = new DataTable();
            listDestination.TableName = "forum_sort_list";
            listDestination.Columns.Add("ForumID", typeof(String));
            listDestination.Columns.Add("Title", typeof(String));
            listDestination.Columns.Add("CanHavePersForums", typeof(bool));

            if (emptyFirstRow)
            {
                DataRow blankRow = listDestination.NewRow();
                blankRow["ForumID"] = 0;
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

                foreach (int forumID in forumidExclusions)
                {
                    if (bFirst)
                    {
                        bFirst = false;
                    }
                    else
                    {
                        strExclusions += ",";
                    }

                    strExclusions += forumID.ToString();
                }

                dv.RowFilter = string.Format("ForumID NOT IN ({0})", strExclusions);
                dv.ApplyDefaultSort = true;
            }

            forum_sort_list_recursive(
                connectionString, dv.ToTable(), listDestination, parentID, categoryID, startingIndent);

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
            using (var cmd = MySqlDbAccess.GetCommand("forum_listall_fromCat"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryID;
                cmd.Parameters.Add("i_AllowUserForumsOnly", MySqlDbType.Byte).Value = allowUserForumsOnly;

                int intCategoryID = Convert.ToInt32(categoryID.ToString());

                using (DataTable dt = MySqlDbAccess.GetData(cmd, connectionString))
                {
                    return Db.forum_sort_list(connectionString, dt, 0, intCategoryID, 0, null, emptyFirstRow);
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

                using (var cmd = MySqlDbAccess.GetCommand("forum_listpath"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("i_ForumID", MySqlDbType.Int32)).Value = forumID;

                    return MySqlDbAccess.GetData(cmd, connectionString);
                }
            }
            else
            {
                using (var cmd = MySqlDbAccess.GetCommand("forum_ns_listpath"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("i_ForumID", MySqlDbType.Int32)).Value = forumID;

                    return MySqlDbAccess.GetData(cmd, connectionString);
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
            object findLastRead, 
            [NotNull] bool showCommonForums, 
            [NotNull]bool showPersonalForums, 
            [CanBeNull] int? forumCreatedByUserId)
        {
            if (!Config.LargeForumTree)
            {
                using (var cmd = MySqlDbAccess.GetCommand("forum_listread"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                    cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId;
                    cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                    cmd.Parameters.Add("i_ParentID", MySqlDbType.Int32).Value = parentId;
                    cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                    cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;
                    cmd.Parameters.Add("i_ShowCommonForums", MySqlDbType.Byte).Value = showCommonForums;
                    cmd.Parameters.Add("i_ShowPersonalForums", MySqlDbType.Byte).Value = showPersonalForums;
                    cmd.Parameters.Add("i_ForumCreatedByUserId", MySqlDbType.Int32).Value = forumCreatedByUserId;
                    cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                    return MySqlDbAccess.GetData(cmd, connectionString);
                }
            }

            using (var cmd = MySqlDbAccess.GetCommand("forum_ns_listread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                cmd.Parameters.Add("i_ParentID", MySqlDbType.Int32).Value = parentId;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;
                cmd.Parameters.Add("i_ShowCommonForums", MySqlDbType.Byte).Value = showCommonForums;
                cmd.Parameters.Add("i_ShowPersonalForums", MySqlDbType.Byte).Value = showPersonalForums;
                cmd.Parameters.Add("i_ForumCreatedByUserId", MySqlDbType.Int32).Value = forumCreatedByUserId;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;
                return MySqlDbAccess.GetData(cmd, connectionString);
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
            object findLastRead,
            [NotNull] bool showCommonForums,
            [NotNull]bool showPersonalForums,
            [CanBeNull] int? forumCreatedByUserId)
        {
            if (!Config.LargeForumTree)
            {
                using (var cmd = MySqlDbAccess.GetCommand("forum_listreadpersonal"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                    cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId;
                    cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                    cmd.Parameters.Add("i_ParentID", MySqlDbType.Int32).Value = parentId;
                    cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                    cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;
                    cmd.Parameters.Add("i_ShowCommonForums", MySqlDbType.Byte).Value = showCommonForums;
                    cmd.Parameters.Add("i_ShowPersonalForums", MySqlDbType.Byte).Value = showPersonalForums;
                    cmd.Parameters.Add("i_ForumCreatedByUserId", MySqlDbType.Int32).Value = forumCreatedByUserId;
                    cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                    return MySqlDbAccess.GetData(cmd, connectionString);
                }
            }

            using (var cmd = MySqlDbAccess.GetCommand("forum_ns_listreadpersonal"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                cmd.Parameters.Add("i_ParentID", MySqlDbType.Int32).Value = parentId;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;
                cmd.Parameters.Add("i_ShowCommonForums", MySqlDbType.Byte).Value = showCommonForums;
                cmd.Parameters.Add("i_ShowPersonalForums", MySqlDbType.Byte).Value = showPersonalForums;
                cmd.Parameters.Add("i_ForumCreatedByUserId", MySqlDbType.Int32).Value = forumCreatedByUserId;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Return admin view of Categories with Forums/Subforums ordered accordingly.
        /// </summary>
        /// <param name="boardId">BoardID</param>
        /// <param name="userID">UserID</param>
        /// <returns>DataSet with categories</returns>
        public static DataSet forum_moderatelist([NotNull] string connectionString, object userID, object boardId)
        {
            using (var connMan = new MySqlDbConnectionManager(connectionString))
            {
                using (var ds = new DataSet())
                {
                    using (
                        var da = new MySqlDataAdapter(
                            MySqlDbAccess.GetObjectName("category_list"), connMan.OpenDBConnection(connectionString)))
                    {
                        using (
                            MySqlTransaction trans =
                                da.SelectCommand.Connection.BeginTransaction(MySqlDbAccess.IsolationLevel))
                        {
                            da.SelectCommand.Transaction = trans;
                            da.SelectCommand.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                            da.SelectCommand.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = DBNull.Value;
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.Fill(ds, MySqlDbAccess.GetObjectName("Category"));
                            da.SelectCommand.Parameters.RemoveAt("i_CategoryID");
                            da.SelectCommand.CommandText = MySqlDbAccess.GetObjectName("forum_moderatelist");
                            da.SelectCommand.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                            da.SelectCommand.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;

                            da.Fill(ds, MySqlDbAccess.GetObjectName("ForumUnsorted"));
                            DataTable dtForumListSorted =
                                ds.Tables[MySqlDbAccess.GetObjectName("ForumUnsorted")].Clone();
                            dtForumListSorted.TableName = MySqlDbAccess.GetObjectName("Forum");
                            ds.Tables.Add(dtForumListSorted);
                            dtForumListSorted.Dispose();
                            Db.forum_list_sort_basic(
                                ds.Tables[MySqlDbAccess.GetObjectName("ForumUnsorted")],
                                ds.Tables[MySqlDbAccess.GetObjectName("Forum")],
                                0,
                                0);
                            ds.Tables.Remove(MySqlDbAccess.GetObjectName("ForumUnsorted"));
                            // vzrus: Remove here all forums with no reports. Would be better to do it in query...
                            // Array to write categories numbers
                            int[] categories = new int[ds.Tables[MySqlDbAccess.GetObjectName("Forum")].Rows.Count];
                            int cntr = 0;
                            //We should make it before too as the colection was changed
                            ds.Tables[MySqlDbAccess.GetObjectName("Forum")].AcceptChanges();
                            foreach (DataRow dr in ds.Tables[MySqlDbAccess.GetObjectName("Forum")].Rows)
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

                            ds.Tables[MySqlDbAccess.GetObjectName("Forum")].AcceptChanges();

                            foreach (DataRow dr in ds.Tables[MySqlDbAccess.GetObjectName("Category")].Rows)
                            {
                                bool deleteMe = true;
                                foreach (int t in categories)
                                {
                                    // We check here if the Category is missing in the array where 
                                    // we've written categories number for each forum
                                    if (t == Convert.ToInt32(dr["CategoryID"]))
                                    {
                                        deleteMe = false;
                                    }
                                }

                                if (deleteMe)
                                {
                                    dr.Delete();
                                }
                            }

                            ds.Tables[MySqlDbAccess.GetObjectName("Category")].AcceptChanges();

                            ds.Relations.Add(
                                "FK_Forum_Category",
                                ds.Tables[MySqlDbAccess.GetObjectName("Category")].Columns["CategoryID"],
                                ds.Tables[MySqlDbAccess.GetObjectName("Forum")].Columns["CategoryID"]);
                            trans.Commit();
                        }

                        return ds;
                    }
                }
            }
        }

        /// <summary>
        /// The forum_moderators.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="styledNicks">
        /// The styled nicks.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable forum_moderators([NotNull] string connectionString, object styledNicks)
        {
            /*  using (var cmd = MySqlDbAccess.GetCommand("forum_moderators"))   
              {
                  cmd.CommandType = CommandType.StoredProcedure;
                 return MySqlDbAccess.GetData(cmd,connectionString);

              }*/
            DataTable dt1 = null;
            using (var cmd1 = MySqlDbAccess.GetCommand("forum_moderators_1"))
            {
                cmd1.CommandType = CommandType.StoredProcedure;

                dt1 = MySqlDbAccess.GetData(cmd1, connectionString);

            }

            using (var cmd2 = MySqlDbAccess.GetCommand("forum_moderators_2"))
            {
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("i_StyledNicks", MySqlDbType.Int32).Value = styledNicks;
                DataTable dt2 = MySqlDbAccess.GetData(cmd2, connectionString);
                if (dt2.Rows.Count > 0 && dt2.Rows[0]["ForumID"] != DBNull.Value)
                {
                    dt1.Merge(dt2);
                }

                return dt1;
            }

            //  }
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
        public static DataTable moderators_team_list([NotNull] string connectionString, bool styledNicks)
        {
            using (var cmd = MySqlDbAccess.GetCommand("moderators_team_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Int32).Value = styledNicks;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }


        /// <summary>
        /// Updates topic and post count and last topic for specified forum
        /// </summary>
        /// <param name="boardId">BoardID</param>
        /// <param name="forumID">If null, all forums in board are updated</param>
        public static void forum_resync([NotNull] string connectionString, object boardId, object forumID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_resync"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID ?? DBNull.Value;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// The forum_save.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="parentId">
        /// The parent id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="sortOrder">
        /// The sort order.
        /// </param>
        /// <param name="locked">
        /// The locked.
        /// </param>
        /// <param name="hidden">
        /// The hidden.
        /// </param>
        /// <param name="isTest">
        /// The is test.
        /// </param>
        /// <param name="moderated">
        /// The moderated.
        /// </param>
        /// <param name="accessMaskId">
        /// The access mask id.
        /// </param>
        /// <param name="remoteURL">
        /// The remote url.
        /// </param>
        /// <param name="themeURL">
        /// The theme url.
        /// </param>
        /// <param name="imageURL">
        /// The image url.
        /// </param>
        /// <param name="styles">
        /// The styles.
        /// </param>
        /// <param name="dummy">
        /// The dummy.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="isUserForum">
        /// The is user forum.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public static long forum_save(
            [NotNull] string connectionString,
            object forumId,
            object categoryId,
            object parentId,
            object name,
            object description,
            object sortOrder,
            object locked,
            object hidden,
            object isTest,
            object moderated,
            object accessMaskId,
            object remoteURL,
            object themeURL,
            object imageURL,
            object styles,
            bool dummy,
            object userId,
            bool isUserForum,
            bool canhavepersforums)
        {
            using (var cmd = MySqlDbAccess.GetCommand("forum_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                cmd.Parameters.Add("i_ParentID", MySqlDbType.Int32).Value = parentId ?? DBNull.Value;
                cmd.Parameters.Add("i_Name", MySqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("i_Description", MySqlDbType.VarChar).Value = description;
                cmd.Parameters.Add("i_SortOrder", MySqlDbType.Int32).Value = sortOrder;
                cmd.Parameters.Add("i_Locked", MySqlDbType.Byte).Value = locked;
                cmd.Parameters.Add("i_Hidden", MySqlDbType.Byte).Value = hidden;
                cmd.Parameters.Add("i_IsTest", MySqlDbType.Byte).Value = isTest;
                cmd.Parameters.Add("i_Moderated", MySqlDbType.Byte).Value = moderated;
                cmd.Parameters.Add("i_RemoteURL", MySqlDbType.VarChar).Value = remoteURL ?? DBNull.Value;
                cmd.Parameters.Add("i_ThemeURL", MySqlDbType.VarChar).Value = themeURL ?? DBNull.Value;
                cmd.Parameters.Add("i_ImageURL", MySqlDbType.VarChar).Value = imageURL ?? DBNull.Value;
                cmd.Parameters.Add("i_Styles", MySqlDbType.VarChar).Value = styles;
                cmd.Parameters.Add("i_AccessMaskID", MySqlDbType.Int32).Value = accessMaskId ?? DBNull.Value;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId ?? DBNull.Value;
                cmd.Parameters.Add("i_IsUserForum", MySqlDbType.Byte).Value = isUserForum;
                cmd.Parameters.Add("i_CanHavePersForums", MySqlDbType.Byte).Value = canhavepersforums;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;
                return long.Parse(MySqlDbAccess.ExecuteScalar(cmd, connectionString).ToString());
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
                    MySqlDbAccess.GetCommand(
                        "SELECT " + MySqlDbAccess.GetObjectName("forum_save_parentschecker") + "(@ForumID,@ParentID)",
                        true))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@ForumID", MySqlDbType.Int32).Value = forumID;
                cmd.Parameters.Add("@ParentID", MySqlDbType.Int32).Value = parentID;
                return Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
            }

        }

        private static void forum_sort_list_recursive(
            [NotNull] string connectionString,
            DataTable listSource,
            DataTable listDestination,
            int parentID,
            int categoryID,
            int currentIndent)
        {
            DataRow newRow;

            foreach (DataRow row in listSource.Rows)
            {
                // see if this is a root-forum
                if (row["ParentID"] == DBNull.Value) row["ParentID"] = 0;

                if ((int)row["ParentID"] == parentID)
                {
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

                    // recurse through the list...
                    forum_sort_list_recursive(
                        connectionString,
                        listSource,
                        listDestination,
                        (int)row["ForumID"],
                        categoryID,
                        currentIndent + 1);
                }
            }
        }


        #endregion        

        #region yaf_Message

        public static DataTable post_list(
            [NotNull] string connectionString,
            [NotNull] object topicId,
            object currentUserID,
            [NotNull] object authorUserID,
            [NotNull] object updateViewCount,
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

            using (var cmd = MySqlDbAccess.GetCommand("post_list"))
            {

                if (updateViewCount == null)
                {
                    updateViewCount = 1;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicId;
                cmd.Parameters.Add("i_AuthorUserID", MySqlDbType.Int32).Value = authorUserID;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = currentUserID;
                cmd.Parameters.Add("i_UpdateViewCount", MySqlDbType.Int16).Value = updateViewCount;
                cmd.Parameters.Add("i_ShowDeleted", MySqlDbType.Byte).Value = showDeleted;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = styledNicks;
                cmd.Parameters.Add("i_SincePostedDate", MySqlDbType.DateTime).Value = sincePostedDate;
                cmd.Parameters.Add("i_ToPostedDate", MySqlDbType.DateTime).Value = toPostedDate;
                cmd.Parameters.Add("i_SinceEditedDate", MySqlDbType.DateTime).Value = sinceEditedDate;
                cmd.Parameters.Add("i_ToEditedDate", MySqlDbType.DateTime).Value = toEditedDate;
                cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("i_SortPosted", MySqlDbType.Int32).Value = sortPosted;
                cmd.Parameters.Add("i_SortEdited", MySqlDbType.Int32).Value = sortEdited;
                cmd.Parameters.Add("i_SortPosition", MySqlDbType.Int32).Value = sortPosition;
                cmd.Parameters.Add("i_ShowThanks", MySqlDbType.Byte).Value = showThanks;
                cmd.Parameters.Add("i_ShowReputation", MySqlDbType.Byte).Value = showReputation;
                cmd.Parameters.Add("i_MessagePosition", MySqlDbType.Int32).Value = messagePosition;
                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageId;
                cmd.Parameters.Add("i_LastRead", MySqlDbType.DateTime).Value = lastRead;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                return MySqlDbAccess.GetData(cmd, connectionString);

            }

            /*  if (dtt != null && dtt.Columns.Count > 0)
            {
                if (dtt.Rows.Count > 0)
                {
                    rowNumber = 0;
                    using (var cmd = MySqlDbAccess.GetCommand("post_list_result"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = currentUserID;
                        cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = styledNicks;
                        cmd.Parameters.Add("i_ShowReputation", MySqlDbType.Byte).Value = showReputation;
                        cmd.Parameters.Add("i_post_totalrowsnumber", MySqlDbType.Int32).Value =
                            dtt.Rows[rowNumber]["PostTotalRowsNumber"];
                        cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = dtt.Rows[rowNumber]["PageIndex"];
                        cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                        cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicId;
                        cmd.Parameters.Add("i_AuthorUserID", MySqlDbType.Int32).Value = authorUserID;
                        cmd.Parameters.Add("i_SortPosted", MySqlDbType.Int32).Value = sortPosted;
                        cmd.Parameters.Add("i_SortEdited", MySqlDbType.Int32).Value = sortEdited;
                        cmd.Parameters.Add("i_SortPosition", MySqlDbType.Int32).Value = sortPosition;
                        cmd.Parameters.Add("i_SincePostedDate", MySqlDbType.DateTime).Value = sincePostedDate;
                        cmd.Parameters.Add("i_ToPostedDate", MySqlDbType.DateTime).Value = toPostedDate;
                        cmd.Parameters.Add("i_SinceEditedDate", MySqlDbType.DateTime).Value = sinceEditedDate;
                        cmd.Parameters.Add("i_ToEditedDate", MySqlDbType.DateTime).Value = toEditedDate;
                        cmd.Parameters.Add("i_FirstSelectPosted", MySqlDbType.DateTime).Value =
                            dtt.Rows[rowNumber]["i_FirstSelectPosted"];
                        cmd.Parameters.Add("i_FirstSelectEdited", MySqlDbType.DateTime).Value =
                            dtt.Rows[rowNumber]["i_FirstSelectEdited"];
                        cmd.Parameters.Add("i_ShowDeleted", MySqlDbType.Byte).Value = showDeleted;
                        cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;


                        return MySqlDbAccess.GetData(cmd,connectionString);
                    }
                }
                else
                {
                    rowNumber = 0;
                    using (var cmd = MySqlDbAccess.GetCommand("post_list_result"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = currentUserID;
                        cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = styledNicks;
                        cmd.Parameters.Add("i_ShowReputation", MySqlDbType.Byte).Value = showReputation;
                        cmd.Parameters.Add("i_post_totalrowsnumber", MySqlDbType.Int32).Value = 1;
                        cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = (int)pageIndex + 1;
                        cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                        cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicId;
                        cmd.Parameters.Add("i_AuthorUserID", MySqlDbType.Int32).Value = authorUserID;
                        cmd.Parameters.Add("i_SortPosted", MySqlDbType.Int32).Value = sortPosted;
                        cmd.Parameters.Add("i_SortEdited", MySqlDbType.Int32).Value = sortEdited;
                        cmd.Parameters.Add("i_SortPosition", MySqlDbType.Int32).Value = sortPosition;
                        cmd.Parameters.Add("i_SincePostedDate", MySqlDbType.DateTime).Value = sincePostedDate;
                        cmd.Parameters.Add("i_ToPostedDate", MySqlDbType.DateTime).Value = toPostedDate;
                        cmd.Parameters.Add("i_SinceEditedDate", MySqlDbType.DateTime).Value = sinceEditedDate;
                        cmd.Parameters.Add("i_ToEditedDate", MySqlDbType.DateTime).Value = toEditedDate;
                        cmd.Parameters.Add("i_FirstSelectPosted", MySqlDbType.DateTime).Value =
                           DateTime.UtcNow;
                        cmd.Parameters.Add("i_FirstSelectEdited", MySqlDbType.DateTime).Value =
                             DateTime.UtcNow;
                        cmd.Parameters.Add("i_ShowDeleted", MySqlDbType.Byte).Value = showDeleted;
                        cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                        return MySqlDbAccess.GetData(cmd,connectionString);
                    }
                }
            }
            return null;*/
        }

        public static DataTable post_list_reverse10([NotNull] string connectionString, object topicID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("post_list_reverse10"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable post_alluser(
            [NotNull] string connectionString, object boardId, object userID, object pageUserID, object numberOfMessages)
        {
            DataTable dt1 = null;

            using (var cmd = MySqlDbAccess.GetCommand("post_alluser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserID;
                cmd.Parameters.Add("I_NumberOfMessages", MySqlDbType.Int32).Value = numberOfMessages;

                //With transaction
                dt1 = MySqlDbAccess.GetData(cmd, false, connectionString);

                foreach (DataRow dr in dt1.Rows)
                {

                    //  using (var cmd1 = MySqlDbAccess.GetCommand(String.Format("SELECT {0}(@i_UserID, @i_ForumID);", MySqlDbAccess.GetObjectName("vaccess_s_readaccess_combo")), true))
                    //   {

                    // cmd1.Parameters.Add( "@i_UserID", MySqlDbType.Int32 ).Value = userID;
                    //  cmd1.Parameters.Add( "@i_ForumID", MySqlDbType.Int32 ).Value = dr["ForumID"];

                    // if ( Convert.ToInt32( MySqlDbAccess.ExecuteScalar( cmd1 ) ) == 0 )
                    if (dr["ReadAccess"].ToString() == "0")
                    {
                        dr.Delete();
                    }
                    //   }

                }
                dt1.AcceptChanges();
            }
            return dt1;

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

            using (var cmd = MySqlDbAccess.GetCommand("message_reply_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;

                DataTable dtr = MySqlDbAccess.GetData(cmd, connectionString);

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
            using (var cmd = MySqlDbAccess.GetCommand("message_reply_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;

                DataTable dtr = MySqlDbAccess.GetData(cmd, connectionString);

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
            }

        }

        //creates new topic, using some parameters from message itself
        public static long topic_create_by_message(
            [NotNull] string connectionString, object messageID, object forumId, object newTopicSubj)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_create_by_message"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumId;
                cmd.Parameters.Add("i_Subject", MySqlDbType.VarChar).Value = newTopicSubj;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                DataTable dt = MySqlDbAccess.GetData(cmd, connectionString);
                return long.Parse(dt.Rows[0]["TopicID"].ToString());
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
            using (var cmd = MySqlDbAccess.GetCommand("message_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;

                return MySqlDbAccess.GetData(cmd, connectionString).AsEnumerable().Select(t => new TypedMessageList(t));
            }
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
            using (var cmd = MySqlDbAccess.GetCommand("message_listreported"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("message_listreporters"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        // <summary> Save reported message back to the database. </summary>
        public static void message_report(
            [NotNull] string connectionString, object messageID, object userID, object reportedDateTime, object reportText)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_report"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                cmd.Parameters.Add("i_ReporterID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_ReportedDate", MySqlDbType.DateTime).Value = reportedDateTime;
                cmd.Parameters.Add("i_ReportText", MySqlDbType.VarChar).Value = reportText;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        // <summary> Copy current Message text over reported Message text. </summary>
        public static void message_reportcopyover([NotNull] string connectionString, object messageID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_reportcopyover"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        // <summary> Copy current Message text over reported Message text. </summary>
        public static void message_reportresolve(
            [NotNull] string connectionString, object messageFlag, object messageID, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_reportresolve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageFlag", MySqlDbType.Int32).Value = messageFlag;
                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
                using (var cmd = MySqlDbAccess.GetCommand("message_getReplies"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;

                    DataTable tbReplies = MySqlDbAccess.GetData(cmd, connectionString);

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

            //If the files are actually saved in the Hard Drive
            if (!UseFileTable)
            {
                using (var cmd = MySqlDbAccess.GetCommand("attachment_list"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                    cmd.Parameters.Add("i_AttachmentID", MySqlDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = DBNull.Value;

                    DataTable tbAttachments = MySqlDbAccess.GetData(cmd, connectionString);
                    string uploadDir =
                        HostingEnvironment.MapPath(
                            String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

                    foreach (DataRow row in tbAttachments.Rows)
                    {
                        try
                        {
                            string fileName = String.Format(
                                "{0}/{1}.{2}.yafupload", uploadDir, row["MessageID"], row["FileName"]);

                            if (File.Exists(fileName))
                            {
                                File.Delete(fileName);
                            }
                        }
                        catch
                        {
                            // error deleting that file... 
                        }
                    }
                }
            }

            // Ederon : erase message for good
            if (eraseMessages)
            {
                using (var cmd = MySqlDbAccess.GetCommand("message_delete"))
                {
                    //if (eraseMessages == null) { eraseMessages = false; }                   

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                    cmd.Parameters.Add("i_EraseMessage", MySqlDbType.Byte).Value = eraseMessages;

                    MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
                }
            }
            else
            {
                //Delete Message
                // undelete function added
                using (var cmd = MySqlDbAccess.GetCommand("message_deleteundelete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                    cmd.Parameters.Add("i_isModeratorChanged", MySqlDbType.Byte).Value = isModeratorChanged;
                    cmd.Parameters.Add("i_DeleteReason", MySqlDbType.VarChar).Value = deleteReason;
                    cmd.Parameters.Add("i_isDeleteAction", MySqlDbType.Byte).Value = isDeleteAction;

                    MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
                }
            }
        }

        // <summary> Set flag on message to approved and store in DB </summary>
        public static void message_approve([NotNull] string connectionString, object messageID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_approve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("message_simplelist"))
            {

                if (Limit == 0)
                {
                    Limit = 1000;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_StartID", MySqlDbType.Int32).Value = StartID;
                cmd.Parameters.Add("i_Limit", MySqlDbType.Int32).Value = Limit;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
            object originalMessage,
            object editedBy,
            object messageDescription,
            string tags)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_update"))
            {
               cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                cmd.Parameters.Add("i_Priority", MySqlDbType.Int32).Value = priority;
                cmd.Parameters.Add("i_Subject", MySqlDbType.VarChar).Value = subject;
                cmd.Parameters.Add("i_status", MySqlDbType.VarChar).Value = status;
                cmd.Parameters.Add("i_Styles", MySqlDbType.VarChar).Value = styles;
                cmd.Parameters.Add("i_Description", MySqlDbType.Text).Value = description;
                cmd.Parameters.Add("i_Flags", MySqlDbType.Int32).Value = flags;
                cmd.Parameters.Add("i_Message", MySqlDbType.Text).Value = message;
                cmd.Parameters.Add("i_Reason", MySqlDbType.VarChar).Value = reasonOfEdit;
                cmd.Parameters.Add("i_EditedBy", MySqlDbType.Int32).Value = editedBy;
                cmd.Parameters.Add("i_IsModeratorChanged", MySqlDbType.Byte).Value = isModeratorChanged;
                cmd.Parameters.Add("i_OverrideApproval", MySqlDbType.Byte).Value = overrideApproval ?? DBNull.Value; 
                cmd.Parameters.Add("i_OriginalMessage", MySqlDbType.Text).Value = originalMessage;
                cmd.Parameters.Add("i_MessageDescription", MySqlDbType.VarChar).Value = messageDescription;
                cmd.Parameters.Add("i_Tags", MySqlDbType.VarChar).Value = tags;
                cmd.Parameters.Add("i_UtcTimeStamp", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        // <summary> Save message to DB. </summary>
        public static bool message_save(
            [NotNull] string connectionString,
            object topicID,
            object userID,
            object message,
            object userName,
            object ip,
            object posted,
            object replyTo,
            object flags,
            [CanBeNull] object messageDescription,
            ref long messageID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_save"))
            {
                if (userName == null)
                {
                    userName = DBNull.Value;
                }
                if (posted == null)
                {
                    posted = DBNull.Value;
                }


                MySqlParameter paramMessageID = new MySqlParameter("i_MessageID", messageID);
                paramMessageID.MySqlDbType = MySqlDbType.Int32;
                paramMessageID.Direction = ParameterDirection.Output;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_Message", MySqlDbType.Text).Value = message;
                cmd.Parameters.Add("i_UserName", MySqlDbType.VarChar).Value = userName;
                cmd.Parameters.Add("i_IP", MySqlDbType.VarChar).Value = ip;
                cmd.Parameters.Add("i_Posted", MySqlDbType.DateTime).Value = posted;
                cmd.Parameters.Add("i_ReplyTo", MySqlDbType.Int32).Value = replyTo;
                cmd.Parameters.Add("i_BlogPostID", MySqlDbType.VarChar).Value = DBNull.Value; // Ederon : 6/16/2007
                cmd.Parameters.Add("i_ExternalMessageID", MySqlDbType.VarChar).Value = DBNull.Value;
                cmd.Parameters.Add("i_ReferenceMessageID", MySqlDbType.VarChar).Value = DBNull.Value;
                cmd.Parameters.Add("i_MessageDescription", MySqlDbType.VarChar).Value = messageDescription;
                cmd.Parameters.Add("i_Flags", MySqlDbType.Int32).Value = flags;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                cmd.Parameters.Add(paramMessageID);
                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
                messageID = Convert.ToInt64(paramMessageID.Value);
                return true;
            }
        }

        public static DataTable message_unapproved([NotNull] string connectionString, object forumID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_unapproved"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("message_findunread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;
                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageId;
                cmd.Parameters.Add("i_LastRead", MySqlDbType.DateTime).Value = lastRead;
                cmd.Parameters.Add("i_ShowDeleted", MySqlDbType.Byte).Value = showDeleted;
                cmd.Parameters.Add("i_AuthorUserID", MySqlDbType.Int32).Value = authorUserID;



                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        // message movind function
        public static void message_move([NotNull] string connectionString, object messageID, object moveToTopic, bool moveAll)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_move"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                cmd.Parameters.Add("i_MoveToTopic", MySqlDbType.Int32).Value = moveToTopic;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
            //moveAll=true anyway
            // it's in charge of moving answers of moved post
            if (moveAll)
            {
                using (var cmd = MySqlDbAccess.GetCommand("message_getReplies"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;

                    DataTable tbReplies = MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("message_getReplies"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;

                DataTable tbReplies = MySqlDbAccess.GetData(cmd, connectionString);
                foreach (DataRow row in tbReplies.Rows)
                {
                    message_moveRecursively(connectionString, row["messageID"], moveToTopic);
                }
                using (MySqlCommand innercmd = MySqlDbAccess.GetCommand("message_move"))
                {
                    innercmd.CommandType = CommandType.StoredProcedure;

                    innercmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                    innercmd.Parameters.Add("i_MoveToTopic", MySqlDbType.Int32).Value = moveToTopic;

                    MySqlDbAccess.ExecuteNonQuery(innercmd, connectionString);
                }
            }
        }

        // functions for Thanks feature
        //TODO: to remove
        // <summary> Checks if the message with the provided messageID is thanked 
        //           by the user with the provided UserID. if so, returns true,
        //           otherwise returns false. </summary>
        public static bool message_isThankedByUser([NotNull] string connectionString, object userID, object messageID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_isthankedbyuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("I_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("I_MessageID", MySqlDbType.Int32).Value = messageID;

                return Convert.ToBoolean(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        // functions for Thanks feature
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
            using (var cmd = MySqlDbAccess.GetCommand("user_thankedmessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("I_MessageID", MySqlDbType.Int32).Value = messageId;
                cmd.Parameters.Add("I_UserID", MySqlDbType.Int32).Value = userId;

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                int thankCount = (int)MySqlDbAccess.ExecuteScalar(cmd, connectionString);

                return thankCount > 0;
            }
        }

        // <summary> Return the number of times the message with the provided messageID
        //           has been thanked. </summary>
        public static int message_ThanksNumber([NotNull] string connectionString, object messageID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_thanksnumber"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("I_MessageID", MySqlDbType.Int32).Value = messageID;

                return Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        // <summary> Returns the UserIDs and UserNames who have thanked the message
        //           with the provided messageID. </summary>
        public static DataTable message_GetThanks([NotNull] string connectionString, object messageID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_getthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("I_MessageID", MySqlDbType.Int32).Value = messageID;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("message_gettextbyids"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_MessageIDs", MySqlDbType.VarChar).Value = messageIDs;
                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("message_getallthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_MessageIDs", MySqlDbType.VarChar).Value = messageIdsSeparatedWithColon;

                return MySqlDbAccess.GetData(cmd, connectionString).AsEnumerable().Select(t => new TypedAllThanks(t));
            }
        }

        // <summary> Retuns All the Thanks for the Message IDs which are in the 
        //           delimited string variable MessageIDs </summary>
        public static DataTable message_GetAllThanks([NotNull] string connectionString, object MessageIDs)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_getallthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_MessageIDs", MySqlDbType.VarChar).Value = MessageIDs;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static string message_AddThanks(
            [NotNull] string connectionString, object fromUserID, object messageID, bool useDisplayName)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_addthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("I_FromUserID", MySqlDbType.Int32).Value = fromUserID;
                cmd.Parameters.Add("I_MessageID", MySqlDbType.Int32).Value = messageID;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;
                cmd.Parameters.Add("I_UseDisplayName", MySqlDbType.Byte).Value = useDisplayName;

                return MySqlDbAccess.ExecuteScalar(cmd, connectionString).ToString();
            }
        }

        public static string message_RemoveThanks(
            [NotNull] string connectionString, object fromUserID, object messageID, bool useDisplayName)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_removethanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("I_FromUserID", MySqlDbType.Int32).Value = fromUserID;
                cmd.Parameters.Add("I_MessageID", MySqlDbType.Int32).Value = messageID;
                cmd.Parameters.Add("I_UseDisplayName", MySqlDbType.Byte).Value = useDisplayName;
                return MySqlDbAccess.ExecuteScalar(cmd, connectionString).ToString();
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
        public static DataTable messagehistory_list([NotNull] string connectionString, int messageID, int? daysToClean)
        {
            using (var cmd = MySqlDbAccess.GetCommand("messagehistory_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_MessageID", MySqlDbType.Int32).Value = messageID;
                cmd.Parameters.Add("i_DaysToClean", MySqlDbType.Int32).Value = daysToClean;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Returns message data based on user access rights
        /// </summary>
        /// <param name="messageID">The Message Id.</param>
        /// <param name="userID">The UserId.</param>
        /// <returns></returns>
        public static DataTable message_secdata([NotNull] string connectionString, int messageID, object pageUserId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("message_secdata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("I_MessageID", MySqlDbType.Int32).Value = messageID;
                cmd.Parameters.Add("I_PageUserID", MySqlDbType.Int32).Value = pageUserId;

                return MySqlDbAccess.GetData(cmd, connectionString);

            }
        }

        #endregion        

        #region yaf_NntpForum

        public static IEnumerable<TypedNntpForum> NntpForumList(
            [NotNull] string connectionString, int boardId, int? minutes, int? nntpForumID, bool? active)
        {
            using (var cmd = MySqlDbAccess.GetCommand("nntpforum_list"))
            {

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_Minutes", MySqlDbType.Int32).Value = minutes;
                cmd.Parameters.Add("i_NntpForumID", MySqlDbType.Int32).Value = nntpForumID;
                cmd.Parameters.Add("i_Active", MySqlDbType.Byte).Value = active;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                return MySqlDbAccess.GetData(cmd, connectionString).AsEnumerable().Select(r => new TypedNntpForum(r));
            }
        }

        public static DataTable nntpforum_list(
            [NotNull] string connectionString, object boardId, object minutes, object nntpForumID, object active)
        {
            using (var cmd = MySqlDbAccess.GetCommand("nntpforum_list"))
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

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_Minutes", MySqlDbType.Int32).Value = minutes;
                cmd.Parameters.Add("i_NntpForumID", MySqlDbType.Int32).Value = nntpForumID;
                cmd.Parameters.Add("i_Active", MySqlDbType.Byte).Value = active;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void nntpforum_update(
            [NotNull] string connectionString, object nntpForumID, object lastMessageNo, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("nntpforum_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_NntpForumID", MySqlDbType.Int32).Value = nntpForumID;
                cmd.Parameters.Add("i_LastMessageNo", MySqlDbType.Int32).Value = lastMessageNo;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void nntpforum_save(
            [NotNull] string connectionString,
            object nntpForumID,
            object nntpServerID,
            object groupName,
            object forumID,
            object active,
            object dateCutOff)
        {
            using (var cmd = MySqlDbAccess.GetCommand("nntpforum_save"))
            {
                if (nntpForumID == null)
                {
                    nntpForumID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_NntpForumID", MySqlDbType.Int32).Value = nntpForumID;
                cmd.Parameters.Add("i_NntpServerID", MySqlDbType.Int32).Value = nntpServerID;
                cmd.Parameters.Add("i_GroupName", MySqlDbType.VarChar).Value = groupName;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;
                cmd.Parameters.Add("i_Active", MySqlDbType.Byte).Value = active;
                cmd.Parameters.Add("i_DateCutoff", MySqlDbType.DateTime).Value = dateCutOff;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void nntpforum_delete([NotNull] string connectionString, object nntpForumID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("nntpforum_delete"))
            {

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_NntpForumID", MySqlDbType.Int32).Value = nntpForumID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_NntpServer

        public static DataTable nntpserver_list([NotNull] string connectionString, object boardId, object nntpServerID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("nntpserver_list"))
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

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_NntpServerID", MySqlDbType.Int32).Value = nntpServerID;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("nntpserver_save"))
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

                cmd.Parameters.Add("i_NntpServerID", MySqlDbType.Int32).Value = nntpServerID;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_Name", MySqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("i_Address", MySqlDbType.VarChar).Value = address;
                cmd.Parameters.Add("i_Port", MySqlDbType.Int32).Value = port;
                cmd.Parameters.Add("i_UserName", MySqlDbType.VarChar).Value = userName;
                cmd.Parameters.Add("i_UserPass", MySqlDbType.VarChar).Value = userPass;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void nntpserver_delete([NotNull] string connectionString, object nntpServerID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("nntpserver_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_NntpServerID", MySqlDbType.Int32).Value = nntpServerID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_NntpTopic

        public static DataTable nntptopic_list([NotNull] string connectionString, object thread)
        {
            using (var cmd = MySqlDbAccess.GetCommand("nntptopic_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_Thread", MySqlDbType.String).Value = thread;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void nntptopic_savemessage(
            [NotNull] string connectionString,
            object nntpForumID,
            object topic,
            object body,
            object userID,
            object userName,
            object ip,
            object posted,
            object externalMessageId,
            object referenceMessageId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("nntptopic_savemessage"))
            {


                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_NntpForumID", MySqlDbType.Int32).Value = nntpForumID;
                cmd.Parameters.Add("i_Topic", MySqlDbType.VarChar).Value = topic;
                cmd.Parameters.Add("i_Body", MySqlDbType.Text).Value = body;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_UserName", MySqlDbType.VarChar).Value = userName;
                cmd.Parameters.Add("i_IP", MySqlDbType.VarChar).Value = ip;
                cmd.Parameters.Add("i_Posted", MySqlDbType.DateTime).Value = posted;
                cmd.Parameters.Add("i_ExternalMessageId", MySqlDbType.VarChar).Value = externalMessageId;
                cmd.Parameters.Add("i_ReferenceMessageId", MySqlDbType.VarChar).Value = referenceMessageId;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;


                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_PMessage

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
            using (var cmd = MySqlDbAccess.GetCommand("pmessage_list"))
            {
                if (fromUserID == null)
                {
                    fromUserID = DBNull.Value;
                }
                if (toUserID == null)
                {
                    toUserID = DBNull.Value;
                }
                if (userPMessageID == null)
                {
                    userPMessageID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_FromUserID", MySqlDbType.Int32).Value = fromUserID;
                cmd.Parameters.Add("i_ToUserID", MySqlDbType.Int32).Value = toUserID;
                cmd.Parameters.Add("i_UserPMessageID", MySqlDbType.Int32).Value = userPMessageID;


                DataTable dt = MySqlDbAccess.GetData(cmd, connectionString);
                return dt;
                // return MySqlDbAccess.GetData(cmd,connectionString);

            }
        }

        /// <summary>
        /// Deletes the private message from the database as per the given parameter.  If <paramref name="fromOutbox"/> is true,
        /// the message is only removed from the user's outbox.  Otherwise, it is completely delete from the database.
        /// </summary>
        /// <param name="pMessageID"></param>
        /// <param name="fromOutbox">If true, removes the message from the outbox.  Otherwise deletes the message completely.</param>
        public static void pmessage_delete([NotNull] string connectionString, object userPMessageID, bool fromOutbox)
        {
            using (var cmd = MySqlDbAccess.GetCommand("pmessage_delete"))
            {
                // if (fromOutbox != false || fromOutbox != true) { fromOutbox = false; }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_UserPMessageID", MySqlDbType.Int32).Value = userPMessageID;
                cmd.Parameters.Add("i_FromOutbox", MySqlDbType.Byte).Value = fromOutbox;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
            /*using (var cmd = MySqlDbAccess.GetCommand("pmessage_delete_1"))
            {
                // if (fromOutbox != false || fromOutbox != true) { fromOutbox = false; }


                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add( "i_UserPMessageID", MySqlDbType.Int32 ).Value = userPMessageID;
                cmd.Parameters.Add( "i_FromOutbox", MySqlDbType.Byte ).Value = fromOutbox;
               
                MySqlDbAccess.ExecuteNonQuery(cmd,connectionString);
            }*/
        }

        /// <summary>
        /// Archives the private message of the given id.  Archiving moves the message from the user's inbox to his message archive.
        /// </summary>
        /// <param name="pMessageID">The ID of the private message</param>
        public static void pmessage_archive([NotNull] string connectionString, object userPMessageID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("pmessage_archive"))
            {
                if (userPMessageID == null)
                {
                    userPMessageID = DBNull.Value;
                }


                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserPMessageID", MySqlDbType.Int32).Value = userPMessageID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("pmessage_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_FromUserID", MySqlDbType.Int32).Value = fromUserID;
                cmd.Parameters.Add("i_ToUserID", MySqlDbType.Int32).Value = toUserID;
                cmd.Parameters.Add("i_Subject", MySqlDbType.VarChar).Value = subject;
                cmd.Parameters.Add("i_Body", MySqlDbType.Text).Value = body;
                cmd.Parameters.Add("i_Flags", MySqlDbType.Int32).Value = Flags;
                cmd.Parameters.Add("i_ReplyTo", MySqlDbType.Int32).Value = replyTo;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void pmessage_markread([NotNull] string connectionString, object userPMessageID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("pmessage_markread"))
            {
                if (userPMessageID == null)
                {
                    userPMessageID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserPMessageID", MySqlDbType.Int32).Value = userPMessageID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static DataTable pmessage_info([NotNull] string connectionString)
        {
            using (var cmd = MySqlDbAccess.GetCommand("pmessage_info"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void pmessage_prune([NotNull] string connectionString, object daysRead, object daysUnread)
        {
            using (var cmd = MySqlDbAccess.GetCommand("pmessage_prune"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_DaysRead", MySqlDbType.Int32).Value = daysRead;
                cmd.Parameters.Add("i_DaysUnread", MySqlDbType.Int32).Value = daysUnread;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("pollgroup_stats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_PollGroupID", MySqlDbType.Int32).Value = pollGroupId;
                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("pollgroup_attach"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_PollGroupID", MySqlDbType.Int32).Value = pollGroupId;
                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicId;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                return Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }


        public static DataTable poll_stats([NotNull] string connectionString, int? pollID)
        {
            /*Workaround for /pages/posts.ascx.cs (int)row["Stats"]*/
            using (var cmd = MySqlDbAccess.GetCommand("poll_stats"))
            {

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_PollID", MySqlDbType.Int32).Value = pollID;

                DataTable dt = MySqlDbAccess.GetData(cmd, false, connectionString);
                DataTable dt_ret = new DataTable();
                foreach (DataColumn dc in dt.Columns)
                {
                    DataColumn dc_ret;
                    if (dc.DataType == typeof(decimal)) dc_ret = new DataColumn(dc.ColumnName, typeof(System.Int32));
                    else dc_ret = new DataColumn(dc.ColumnName, dc.DataType);

                    dt_ret.Columns.Add(dc_ret);
                }
                dt_ret.AcceptChanges();
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow dr_ret = dt_ret.NewRow();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        dr_ret[dc.ColumnName] = dr[dc];

                    }
                    dt_ret.Rows.Add(dr_ret);
                }
                dt_ret.AcceptChanges();
                return dt_ret;
            }

            /*using (var cmd = MySqlDbAccess.GetCommand( "poll_stats" ) )
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue( "i_PollID", pollID );
                return MySqlDbAccess.GetData(cmd,connectionString);
            }*/
        }

        /// <summary>
        /// The method saves many questions and answers to them in a single transaction 
        /// </summary>
        /// <param name="pollList">List to hold all polls data</param>
        /// <returns>Last saved poll id.</returns>
        public static int? poll_save([NotNull] string connectionString, List<PollSaveList> pollList)
        {


            foreach (PollSaveList question in pollList)
            {
                using (MySqlDbConnectionManager connMan = new MySqlDbConnectionManager(connectionString))
                {
                    using (
                        MySqlTransaction trans =
                            connMan.OpenDBConnection(connectionString).BeginTransaction(MySqlDbAccess.IsolationLevel))
                    {
                        try
                        {
                            int? myPollID = null;

                            StringBuilder sb = new StringBuilder();

                            // Check if the group already exists
                            if (question.TopicId > 0)
                            {
                                sb.Append("select PollID  from ");
                                sb.Append(MySqlDbAccess.GetObjectName("Topic"));
                                sb.Append(" WHERE TopicID = ?TopicID; ");
                            }
                            else if (question.ForumId > 0)
                            {

                                sb.Append("select PollGroupID  from ");
                                sb.Append(MySqlDbAccess.GetObjectName("Forum"));
                                sb.Append(" WHERE ForumID = ?ForumID;");
                            }
                            else if (question.CategoryId > 0)
                            {

                                sb.Append("select PollGroupID  from ");
                                sb.Append(MySqlDbAccess.GetObjectName("Category"));
                                sb.Append(" WHERE CategoryID = ?CategoryID;");
                            }
                            int? pollGroupId = null;
                            object pollGroupIdObj = null;
                            using (var cmdPoll = MySqlDbAccess.GetCommand(sb.ToString(), true))
                            {
                                cmdPoll.Transaction = trans;
                                if (question.TopicId > 0)
                                {
                                    cmdPoll.Parameters.Add("?TopicID", MySqlDbType.Int32).Value = question.TopicId;
                                }
                                else if (question.ForumId > 0)
                                {
                                    cmdPoll.Parameters.Add("?ForumID", MySqlDbType.Int32).Value = question.ForumId;
                                }
                                else if (question.CategoryId > 0)
                                {
                                    cmdPoll.Parameters.Add("?CategoryID", MySqlDbType.Int32).Value = question.CategoryId;
                                }
                                pollGroupIdObj = MySqlDbAccess.ExecuteScalar(cmdPoll, false, connectionString);

                            }
                            sb = new StringBuilder();
                            // the group doesn't exists, create a new one
                            int pgIdcheck = 0;
                            if (!int.TryParse(pollGroupIdObj.ToString(), out pgIdcheck))
                            {
                                sb.Append(
                                    string.Format(
                                        "INSERT INTO {0}(UserID,Flags ) VALUES(?UserID, ?Flags); ",
                                        MySqlDbAccess.GetObjectName("PollGroupCluster")));
                                //  sb.Append("SELECT PollGroupID FROM ");
                                sb.Append("SELECT LAST_INSERT_ID(); ");
                                //  sb.Append(MySqlDbAccess.GetObjectName("PollGroupCluster"));
                                //   sb.Append(" WHERE PollGroupID = LAST_INSERT_ID(); ");
                                using (var cmdPoll = MySqlDbAccess.GetCommand(sb.ToString(), true))
                                {
                                    cmdPoll.Transaction = trans;
                                    cmdPoll.Parameters.Add("?UserID", MySqlDbType.Int32).Value = question.UserId;
                                    // set poll group flags
                                    int groupFlags = 0;
                                    if (question.IsBound)
                                    {
                                        groupFlags = groupFlags | 2;
                                    }
                                    cmdPoll.Parameters.Add("?Flags", MySqlDbType.Int32).Value = groupFlags;

                                    pollGroupId =
                                        Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmdPoll, false, connectionString));

                                }
                            }
                            else
                            {

                                sb.Append(
                                    String.Format(
                                        "UPDATE {0} SET Flags = (CASE WHEN Flags <> 0 AND (?Flags & 2) = 2 THEN Flags = Flags | 2 ELSE ?Flags END)  WHERE PollGroupID = ?PollGroupID; ",
                                        MySqlDbAccess.GetObjectName("PollGroupCluster")));
                                using (var cmdPollUpdate = MySqlDbAccess.GetCommand(sb.ToString(), true))
                                {
                                    cmdPollUpdate.Transaction = trans;
                                    cmdPollUpdate.Parameters.Add("?UserID", MySqlDbType.Int32).Value = question.UserId;
                                    // set poll group flags
                                    int groupFlags = 0;
                                    if (question.IsBound)
                                    {
                                        groupFlags = groupFlags | 2;
                                    }
                                    cmdPollUpdate.Parameters.Add("?Flags", MySqlDbType.Int32).Value = groupFlags;
                                    cmdPollUpdate.Parameters.Add("?PollGroupID", MySqlDbType.Int32).Value = pollGroupId;
                                    MySqlDbAccess.ExecuteNonQuery(cmdPollUpdate, false, connectionString);

                                }
                                pollGroupId = (int?)pollGroupIdObj;
                            }



                            sb =
                                new System.Text.StringBuilder(
                                    string.Format("INSERT INTO {0}", MySqlDbAccess.GetObjectName("Poll")));

                            if (question.Closes > DateTimeHelper.SqlDbMinTime())
                            {
                                sb.Append("(Question,Closes, UserID,PollGroupID,ObjectPath,MimeType,Flags) ");
                            }
                            else
                            {
                                sb.Append("(Question,UserID,PollGroupID,ObjectPath,MimeType,Flags) ");
                            }

                            sb.Append(" VALUES(");
                            sb.Append("?Question");

                            if (question.Closes > DateTimeHelper.SqlDbMinTime())
                            {
                                sb.Append(",?Closes");
                            }
                            sb.Append(",?UserID,?PollGroupID,?QuestionObjectPath,?QuestionMimeType,?PollFlags");
                            sb.Append("); ");

                            sb.Append("SELECT ");
                            sb.Append(" LAST_INSERT_ID(); ");
                            using (var cmdPoll = MySqlDbAccess.GetCommand(sb.ToString(), true))
                            {
                                cmdPoll.Transaction = trans;
                                cmdPoll.CommandType = CommandType.Text;
                                cmdPoll.Parameters.Add("?Question", MySqlDbType.VarChar).Value = question.Question;

                                if (question.Closes > DateTimeHelper.SqlDbMinTime())
                                {
                                    cmdPoll.Parameters.Add("?Closes", MySqlDbType.DateTime).Value = question.Closes;
                                }
                                cmdPoll.Parameters.Add("?UserID", MySqlDbType.VarChar).Value = question.UserId;
                                cmdPoll.Parameters.Add("?PollGroupID", MySqlDbType.VarChar).Value = pollGroupId;
                                cmdPoll.Parameters.Add("?QuestionObjectPath", MySqlDbType.VarChar).Value =
                                    question.QuestionObjectPath;
                                cmdPoll.Parameters.Add("?QuestionMimeType", MySqlDbType.VarChar).Value =
                                    question.QuestionMimeType;
                                int pollFlags = question.IsClosedBound ? 0 | 4 : 0;
                                pollFlags = question.AllowMultipleChoices ? pollFlags | 8 : pollFlags;
                                pollFlags = question.ShowVoters ? pollFlags | 16 : pollFlags;
                                pollFlags = question.AllowSkipVote ? pollFlags | 32 : pollFlags;

                                cmdPoll.Parameters.Add("?PollFlags", MySqlDbType.VarChar).Value = pollFlags;
                                object dd = MySqlDbAccess.ExecuteScalar(cmdPoll, false, connectionString);
                                myPollID = Convert.ToInt32(dd);

                            }

                            sb = new System.Text.StringBuilder();

                            // The cycle through question reply choices            
                            for (uint choiceCount = 0;
                                 choiceCount < question.Choice.GetUpperBound(1) + 1;
                                 choiceCount++)
                            {
                                if (!string.IsNullOrEmpty(question.Choice[0, choiceCount]))
                                {
                                    // sb.Append(string.Format(" INSERT INTO  "));
                                    //  sb.Append(MySqlDbAccess.GetObjectName("Choice"));
                                    //   sb.Append(string.Format("("));
                                    // sb.Append(string.Format(" INSERT INTO {0}(",MySqlDbAccess.GetObjectName("Choice")));
                                    //  sb.AppendFormat("PollID,Choice,Votes) VALUES(?PollID{0},?Choice{0},?Votes{0}); ",
                                    //                  choiceCount);
                                    sb.AppendFormat(
                                        "INSERT INTO  {0}(PollID,Choice,Votes,ObjectPath,MimeType) VALUES(?PollID{1},?Choice{1},?Votes{1},?ChoiceObjectPath{1},?ChoiceMimeType{1}); ",
                                        MySqlDbAccess.GetObjectName("Choice"),
                                        choiceCount);
                                }
                            }
                            using (var cmd = MySqlDbAccess.GetCommand(sb.ToString(), true))
                            {
                                /* MySqlParameter ret = new MySqlParameter();
                        ret.ParameterName = "?PollIDOut";
                        ret.MySqlDbType = MySqlDbType.Int32;
                        ret.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ret); 
                        cmd.Parameters.Add("?Question", MySqlDbType.VarChar ).Value = question.Question;

                        if (question.Closes > DateTimeHelper.SqlDbMinTime())
                        {
                            cmd.Parameters.Add("?Closes", MySqlDbType.DateTime ).Value = question.Closes;
                        }
                       */
                                cmd.Transaction = trans;
                                for (uint choiceCount1 = 0;
                                     choiceCount1 < question.Choice.GetUpperBound(1) + 1;
                                     choiceCount1++)
                                {
                                    if (!string.IsNullOrEmpty(question.Choice[0, choiceCount1]))
                                    {

                                        cmd.Parameters.Add("?PollID{0}".FormatWith(choiceCount1), MySqlDbType.Int32)
                                           .Value = myPollID;
                                        cmd.Parameters.Add(
                                            String.Format("?Choice{0}", choiceCount1), MySqlDbType.VarChar).Value =
                                            question.Choice[0, choiceCount1];
                                        cmd.Parameters.Add(String.Format("?Votes{0}", choiceCount1), MySqlDbType.Int32)
                                           .Value = 0;
                                        cmd.Parameters.Add(
                                            String.Format("?ChoiceObjectPath{0}", choiceCount1), MySqlDbType.VarChar)
                                           .Value = question.Choice[1, choiceCount1].IsNotSet()
                                                        ? String.Empty
                                                        : question.Choice[1, choiceCount1];
                                        cmd.Parameters.Add(
                                            String.Format("?ChoiceMimeType{0}", choiceCount1), MySqlDbType.VarChar)
                                           .Value = question.Choice[2, choiceCount1].IsNotSet()
                                                        ? String.Empty
                                                        : question.Choice[2, choiceCount1];
                                    }
                                }
                                MySqlDbAccess.ExecuteNonQuery(cmd, false, connectionString);

                            }

                            sb = new StringBuilder();
                            // fill a pollgroup field - double work if a poll exists 
                            if (question.TopicId > 0)
                            {

                                sb.Append("UPDATE ");
                                sb.Append(MySqlDbAccess.GetObjectName("Topic"));
                                sb.Append(" SET PollID = ?NewPollGroupID WHERE TopicID =?TopicID; ");

                            }

                            // fill a pollgroup field in Forum Table if the call comes from a forum's topic list 
                            if (question.ForumId > 0)
                            {
                                sb.Append("UPDATE ");
                                sb.Append(MySqlDbAccess.GetObjectName("Forum"));
                                sb.Append(" SET PollGroupID= ?NewPollGroupID WHERE ForumID= ?ForumID; ");
                            }

                            // fill a pollgroup field in Category Table if the call comes from a category's topic list 
                            if (question.CategoryId > 0)
                            {
                                sb.Append("UPDATE ");
                                sb.Append(MySqlDbAccess.GetObjectName("Category"));
                                sb.Append(" SET PollGroupID = ?NewPollGroupID WHERE CategoryID= ?CategoryID; ");
                            }

                            using (var cmdPoll = MySqlDbAccess.GetCommand(sb.ToString(), true))
                            {
                                cmdPoll.Transaction = trans;
                                cmdPoll.Parameters.Add("?NewPollGroupID", MySqlDbType.Int32).Value = pollGroupId;
                                if (question.TopicId > 0)
                                {
                                    cmdPoll.Parameters.Add("?TopicID", MySqlDbType.Int32).Value = question.TopicId;
                                }

                                // fill a pollgroup field in Forum Table if the call comes from a forum's topic list 
                                if (question.ForumId > 0)
                                {
                                    cmdPoll.Parameters.Add("?ForumID", MySqlDbType.Int32).Value = question.ForumId;
                                }

                                // fill a pollgroup field in Category Table if the call comes from a category's topic list 
                                if (question.CategoryId > 0)
                                {
                                    cmdPoll.Parameters.Add("?CategoryID", MySqlDbType.Int32).Value = question.CategoryId;
                                }

                                MySqlDbAccess.ExecuteNonQuery(cmdPoll, false, connectionString);

                            }

                            /*if (ret.Value != DBNull.Value)
                       {
                           return (int?)ret.Value;
                       }*/
                            trans.Commit();
                            return pollGroupId;
                        }
                        catch (Exception e)
                        {
                            trans.Rollback();
                            throw new Exception(e.Message);

                        }
                        finally
                        {
                            // connMan.CloseConnection();
                        }
                    }
                }
            }
            return null;

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
            using (var cmd = MySqlDbAccess.GetCommand("poll_update"))
            {
                if (closes == null)
                {
                    closes = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_PollID", MySqlDbType.Int32).Value = pollID;
                cmd.Parameters.Add("i_Question", MySqlDbType.VarChar).Value = question;
                cmd.Parameters.Add("i_Closes", MySqlDbType.DateTime).Value = closes;
                cmd.Parameters.Add("i_QuestionObjectPath", MySqlDbType.VarChar).Value = questionPath;
                cmd.Parameters.Add("i_QuestionMimeType", MySqlDbType.VarChar).Value = questionMime;
                cmd.Parameters.Add("i_IsBounded", MySqlDbType.Byte).Value = isBounded;
                cmd.Parameters.Add("i_IsClosedBounded", MySqlDbType.Byte).Value = isClosedBounded;
                cmd.Parameters.Add("i_AllowMultipleChoices", MySqlDbType.Byte).Value = allowMultipleChoices;
                cmd.Parameters.Add("i_ShowVoters", MySqlDbType.Byte).Value = showVoters;
                cmd.Parameters.Add("i_AllowSkipVote", MySqlDbType.Byte).Value = allowSkipVote;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// The poll_remove.
        /// </summary>
        /// <param name="pollGroupId">
        /// The poll group id. The parameter should always be present. 
        /// </param>
        /// <param name="pollId">
        /// The poll id. If null all polls in a group a deleted. 
        /// </param>
        /// <param name="boardId">
        /// The BoardID id. 
        /// </param>
        /// <param name="removeCompletely">
        /// The RemoveCompletely. If true and pollID is null , all polls in a group are deleted completely, 
        /// else only one poll is deleted completely. 
        /// </param>
        /// <param name="removeEverywhere">
        /// The remove everywhere.
        /// </param>
        public static void poll_remove(
            [NotNull] string connectionString,
            object pollGroupId,
            object pollId,
            object boardId,
            bool removeCompletely,
            bool removeEverywhere)
        {
            using (var cmd = MySqlDbAccess.GetCommand("poll_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_PollGroupID", MySqlDbType.Int32).Value = pollGroupId;
                cmd.Parameters.Add("i_PollID", MySqlDbType.Int32).Value = pollId;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_RemoveCompletely", MySqlDbType.Byte).Value = removeCompletely;
                cmd.Parameters.Add("i_RemoveEverywhere", MySqlDbType.Byte).Value = removeEverywhere;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("pollgroup_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumId;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;

                return MySqlDbAccess.GetData(cmd, connectionString).AsEnumerable().Select(r => new TypedPollGroup(r));
            }
        }

        /// <summary>
        /// The poll_remove.
        /// </summary>
        /// <param name="pollGroupId">
        /// The poll group id. The parameter should always be present. 
        /// </param>
        /// <param name="topicId">
        /// The poll id. If null all polls in a group a deleted. 
        /// </param>
        /// <param name="categoryId">The category ID.</param>
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
            object pollGroupId,
            object topicId,
            object forumId,
            object categoryId,
            object boardId,
            bool removeCompletely,
            bool removeEverywhere)
        {
            using (var cmd = MySqlDbAccess.GetCommand("pollgroup_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_PollGroupID", MySqlDbType.Int32).Value = pollGroupId;
                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicId;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_RemoveCompletely", MySqlDbType.Byte).Value = removeCompletely;
                cmd.Parameters.Add("i_RemoveEverywhere", MySqlDbType.Byte).Value = removeEverywhere;
                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }        

        #endregion

        #region yaf_Rank

        public static DataTable rank_list([NotNull] string connectionString, object boardId, object rankID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("rank_list"))
            {
                if (rankID == null)
                {
                    rankID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_RankID", MySqlDbType.Int32).Value = rankID;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The rank_save.
        /// </summary>
        /// <param name="rankID">
        /// The rank id.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isStart">
        /// The is start.
        /// </param>
        /// <param name="isLadder">
        /// The is ladder.
        /// </param>
        /// <param name="minPosts">
        /// The min posts.
        /// </param>
        /// <param name="rankImage">
        /// The rank image.
        /// </param>
        /// <param name="pmLimit">
        /// The pm limit.
        /// </param>
        /// <param name="style">
        /// The style.
        /// </param>
        /// <param name="sortOrder">
        /// The sort order.
        /// </param>
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
            object pmlimit,
            object style,
            object sortOrder,
            object description,
            object usrSigChars,
            object usrSigBBCodes,
            object usrSigHTMLTags,
            object usrAlbums,
            object usrAlbumImages)
        {
            using (var cmd = MySqlDbAccess.GetCommand("rank_save"))
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
                cmd.Parameters.Add("i_RankID", MySqlDbType.Int32).Value = rankID;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_Name", MySqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("i_IsStart", MySqlDbType.Byte).Value = isStart;
                cmd.Parameters.Add("i_IsLadder", MySqlDbType.Byte).Value = isLadder;
                cmd.Parameters.Add("i_IsGuest", MySqlDbType.Byte).Value = isGuest;
                cmd.Parameters.Add("i_MinPosts", MySqlDbType.Int32).Value = minPosts;
                cmd.Parameters.Add("i_RankImage", MySqlDbType.VarChar).Value = rankImage;
                cmd.Parameters.Add("i_PMLimit", MySqlDbType.Int32).Value = pmlimit;
                cmd.Parameters.Add("i_Style", MySqlDbType.VarChar).Value = style;
                cmd.Parameters.Add("i_SortOrder", MySqlDbType.Int16).Value = sortOrder;
                cmd.Parameters.Add("i_Description", MySqlDbType.VarChar).Value = description;
                cmd.Parameters.Add("i_UsrSigChars", MySqlDbType.Int32).Value = usrSigChars;
                cmd.Parameters.Add("i_UsrSigBBCodes", MySqlDbType.VarChar).Value = usrSigBBCodes;
                cmd.Parameters.Add("i_UsrSigHTMLTags", MySqlDbType.VarChar).Value = usrSigHTMLTags;
                cmd.Parameters.Add("i_UsrAlbums", MySqlDbType.Int32).Value = usrAlbums;
                cmd.Parameters.Add("i_UsrAlbumImages", MySqlDbType.Int32).Value = usrAlbumImages;



                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void rank_delete([NotNull] string connectionString, object rankID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("rank_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_RankID", MySqlDbType.Int32).Value = rankID;
                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_Smiley

        [NotNull]
        public static IEnumerable<TypedSmileyList> SmileyList([NotNull] string connectionString, int boardId, int? smileyID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("smiley_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_SmileyID", MySqlDbType.Int32).Value = smileyID;

                return MySqlDbAccess.GetData(cmd, connectionString).AsEnumerable().Select(r => new TypedSmileyList(r));
            }
        }

        public static DataTable smiley_list([NotNull] string connectionString, object boardId, object smileyID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("smiley_list"))
            {
                if (smileyID == null)
                {
                    smileyID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_SmileyID", MySqlDbType.Int32).Value = smileyID;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }


        public static DataTable smiley_listunique([NotNull] string connectionString, object boardId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("smiley_listunique"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void smiley_delete([NotNull] string connectionString, object smileyID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("smiley_delete"))
            {
                if (smileyID == null)
                {
                    smileyID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_SmileyID", MySqlDbType.Int32).Value = smileyID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("smiley_save"))
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

                cmd.Parameters.Add("i_SmileyID", MySqlDbType.Int32).Value = smileyID;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_Code", MySqlDbType.VarChar).Value = code;
                cmd.Parameters.Add("i_Icon", MySqlDbType.VarChar).Value = icon;
                cmd.Parameters.Add("i_Emoticon", MySqlDbType.VarChar).Value = emoticon;
                cmd.Parameters.Add("i_SortOrder", MySqlDbType.Byte).Value = sortOrder;
                cmd.Parameters.Add("i_Replace", MySqlDbType.Byte).Value = replace;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void smiley_resort([NotNull] string connectionString, object boardId, object smileyID, int move)
        {
            using (var cmd = MySqlDbAccess.GetCommand("smiley_resort"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_SmileyID", MySqlDbType.Int32).Value = smileyID;
                cmd.Parameters.Add("i_Move", MySqlDbType.Int32).Value = move;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion        

        #region yaf_Registry

        /// <summary>
        /// Retrieves entries in the board settings registry
        /// </summary>
        /// <param name="Name">Use to specify return of specific entry only. Setting this to null returns all entries.</param>
        /// <returns>DataTable filled will registry entries</returns>
        public static DataTable registry_list([NotNull] string connectionString, object name, object boardId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("registry_list"))
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

                cmd.Parameters.Add("i_Name", MySqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }


        /// <summary>
        /// Saves a single registry entry pair to the database.
        /// </summary>
        /// <param name="Name">Unique name associated with this entry</param>
        /// <param name="Value">Value associated with this entry which can be null</param>
        public static void registry_save([NotNull] string connectionString, object name, object value)
        {
            using (var cmd = MySqlDbAccess.GetCommand("registry_save"))
            {
                if (value == null)
                {
                    value = DBNull.Value;
                }


                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_Name", MySqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("i_Value", MySqlDbType.LongText).Value = value;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = DBNull.Value;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Saves a single registry entry pair to the database.
        /// </summary>
        /// <param name="Name">Unique name associated with this entry</param>
        /// <param name="Value">Value associated with this entry which can be null</param>
        /// <param name="BoardID">The BoardID for this entry</param>
        public static void registry_save([NotNull] string connectionString, object name, object value, object boardId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("registry_save"))
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

                cmd.Parameters.Add("i_Name", MySqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("i_Value", MySqlDbType.LongText).Value = value;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_System

        /// <summary>
        /// Not in use anymore. Only required for old database versions.
        /// </summary>
        /// <returns></returns>
        public static DataTable system_list([NotNull] string connectionString)
        {
            using (var cmd = MySqlDbAccess.GetCommand("system_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        #endregion

        #region yaf_Topic

        public static void topic_updatetopic([NotNull] string connectionString, int topicId, string topic)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_updatetopic"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicId;
                cmd.Parameters.Add("i_Topic", MySqlDbType.VarChar).Value = topic;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        //TODO: Overloaded method for 1.9.3 FINAL comatability should be deleted beginning with v.2373
        public static int topic_prune([NotNull] string connectionString, object forumID, object days)
        {
            int boardId = 0;
            using (
                var cmd =
                    MySqlDbAccess.GetCommand(
                        String.Format(
                            "SELECT c.BoardID FROM {0} f INNER JOIN {1} c ON f.CategoryID=c.CategoryID  WHERE ForumID = @i_ForumID;",
                            MySqlDbAccess.GetObjectName("Forum"),
                            MySqlDbAccess.GetObjectName("Category")),
                        true))
            {

                cmd.Parameters.Add("@i_ForumID", MySqlDbType.Int32).Value = forumID;

                boardId = Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
            }

            return topic_prune(connectionString, boardId, forumID, days, 1);
        }

        public static int topic_prune(
            [NotNull] string connectionString, object boardId, object forumID, object days, object permDelete)
        {

            using (var cmd = MySqlDbAccess.GetCommand("topic_prune"))
            {
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;
                cmd.Parameters.Add("i_Days", MySqlDbType.Int32).Value = days;
                cmd.Parameters.Add("i_PermDelete", MySqlDbType.Byte).Value = permDelete;
                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                return Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        public static DataTable announcements_list(
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
            using (var cmd = MySqlDbAccess.GetCommand("announcements_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId ?? DBNull.Value;
                cmd.Parameters.Add("i_SinceDate", MySqlDbType.DateTime).Value = sinceDate ?? DBNull.Value;
                cmd.Parameters.Add("i_ToDate", MySqlDbType.DateTime).Value = toDate;
                cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_ShowMoved", MySqlDbType.Byte).Value = showMoved;
                cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;
                cmd.Parameters.Add("i_GetTags", MySqlDbType.Byte).Value = getTags;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("topic_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId ?? DBNull.Value;
                cmd.Parameters.Add("i_SinceDate", MySqlDbType.DateTime).Value = sinceDate ?? DBNull.Value;
                cmd.Parameters.Add("i_ToDate", MySqlDbType.DateTime).Value = toDate;
                cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_ShowMoved", MySqlDbType.Byte).Value = showMoved;
                cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;
                cmd.Parameters.Add("i_GetTags", MySqlDbType.Byte).Value = getTags;
                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("topic_simplelist"))
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
                cmd.Parameters.Add("i_StartID", MySqlDbType.Int32).Value = StartID;
                cmd.Parameters.Add("i_Limit", MySqlDbType.Int32).Value = Limit;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void topic_move(
            [NotNull] string connectionString, object topicID, object forumID, object showMoved, object linkDays)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_move"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;
                cmd.Parameters.Add("i_ShowMoved", MySqlDbType.Byte).Value = showMoved;
                cmd.Parameters.Add("i_LinkDays", MySqlDbType.Int32).Value = linkDays;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static DataTable topic_announcements(
            [NotNull] string connectionString, object boardId, object numOfPostsToRetrieve, object pageUserID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_announcements"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_NumPosts", MySqlDbType.Int32).Value = numOfPostsToRetrieve;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserID;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        /* public static  DataTable topic_latest(object boardId, object numOfPostsToRetrieve, object userID)
         {
             //string providerName = "MySql.Data.MySqlClient.MySqlClientFactory";

             MySqlClientFactory dbf = (MySqlClientFactory)DbProviderFactories.GetFactory(System.Configuration.ConfigurationManager.ConnectionStrings["yafnet1"].ProviderName);
             DataTable dtfc = DbProviderFactories.GetFactoryClasses();
             using (MySqlConnection dbcn = (MySqlConnection)dbf.CreateConnection())
             {
              
                 DataTable dt = null;
                 dbcn.ConnectionString = VZF.Classes.Config.ConnectionString;
                 dbcn.Open();                
                 using (MySqlCommand dbcmd = (MySqlCommand)dbcn.CreateCommand())
                 {
                     dbcmd.CommandType = CommandType.StoredProcedure;
                     dbcmd.CommandText = MySqlDbAccess.GetObjectName("topic_latest");
                     dbcmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;                    
                     dbcmd.Parameters.Add("i_NumPosts", MySqlDbType.Int32).Value = numOfPostsToRetrieve;
                     dbcmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                     MySqlDataReader reader = dbcmd.ExecuteReader();
                     DataTable schemaTable = reader.GetSchemaTable();
                     dt = new DataTable();
                     foreach (DataRow myField in schemaTable.Rows)
                     {

                         String ts = myField["DataType"].ToString();
                         if ((ts == "System.UInt64" || ts == "System.Int64")) ts = "System.Int32";
                         if (ts == "System.SByte") ts = "System.Boolean";
                         if (!dt.Columns.Contains(myField["ColumnName"].ToString()))
                         {
                             dt.Columns.Add(myField["ColumnName"].ToString(), Type.GetType(ts));
                         }
                         else
                         {
                             if (!myField["ColumnName"].ToString().Contains("81_18"))
                             {
                                 dt.Columns.Add(myField["ColumnName"].ToString() + "81_18", Type.GetType(ts));
                             }
                         }

                     }

                     if (reader.HasRows)
                     {
                         while (reader.Read())
                         {
                             DataRow dr = dt.NewRow();

                             foreach (DataColumn column in dt.Columns)
                             {
                                 dr[column] = reader[column.Ordinal];
                             }

                             dt.Rows.Add(dr);
                         }
                     }
                     reader.Close();                   
                     dt.AcceptChanges();
                  
                 }
                  dbcn.Close();
                  return dt;
             }
            
         } */

        public static DataTable topic_latest(
            [NotNull] string connectionString,
            object boardID,
            object numOfPostsToRetrieve,
            object pageUserId,
            bool useStyledNicks,
            bool showNoCountPosts,
            bool findLastRead)
        {

            using (var cmd = MySqlDbAccess.GetCommand("topic_latest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardID;
                cmd.Parameters.Add("i_NumPosts", MySqlDbType.Int32).Value = numOfPostsToRetrieve;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserId;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_ShowNoCountPosts", MySqlDbType.Byte).Value = showNoCountPosts;
                cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;

                return MySqlDbAccess.GetData(cmd, false, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("rss_topic_latest"))
            {
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_NumPosts", MySqlDbType.Int32).Value = numOfPostsToRetrieve;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserId;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_ShowNoCountPosts", MySqlDbType.Byte).Value = showNoCountPosts;
                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("topic_active"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserId ?? DBNull.Value;
                cmd.Parameters.Add("i_SinceDate", MySqlDbType.Timestamp).Value = sinceDate ?? DBNull.Value;
                cmd.Parameters.Add("i_ToDate", MySqlDbType.Timestamp).Value = toDate;
                cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;

                return MySqlDbAccess.GetData(cmd, connectionString);
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

            using (var cmd = MySqlDbAccess.GetCommand("topic_unread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserId ?? DBNull.Value;
                cmd.Parameters.Add("i_SinceDate", MySqlDbType.Timestamp).Value = sinceDate ?? DBNull.Value;
                cmd.Parameters.Add("i_ToDate", MySqlDbType.Timestamp).Value = toDate;
                cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }


        public static DataTable topic_tags(
            [NotNull] string connectionString, [NotNull] object boardId, [NotNull] object pageUserId, [NotNull] object topicId)
        {

            using (var cmd = MySqlDbAccess.GetCommand("topic_tags"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserId ?? DBNull.Value;
                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicId;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable topic_bytags(
            [NotNull] string connectionString,
            [NotNull] object boardId,
            int forumId,
            [NotNull] object pageUserId,
            [NotNull] object tags,
            object sinceDate,
            int pageIndex,
            int pageSize)
        {

            using (var cmd = MySqlDbAccess.GetCommand("topic_bytags"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = pageUserId ?? DBNull.Value;
                cmd.Parameters.Add("i_Tags", MySqlDbType.VarChar).Value = tags;
                cmd.Parameters.Add("i_SinceDate", MySqlDbType.DateTime).Value = sinceDate;
                cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// The topic_unanswered
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        ///  </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="since">
        /// The since.
        /// </param>
        /// <param name="categoryID">
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
            using (var cmd = MySqlDbAccess.GetCommand("topic_unanswered"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserId ?? DBNull.Value;
                cmd.Parameters.Add("i_SinceDate", MySqlDbType.Timestamp).Value = sinceDate ?? DBNull.Value;
                cmd.Parameters.Add("i_ToDate", MySqlDbType.Timestamp).Value = toDate;
                cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Gets all topics where the pageUserid has posted
        /// </summary>
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
            using (var cmd = MySqlDbAccess.GetCommand("topics_byuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserId ?? DBNull.Value;
                cmd.Parameters.Add("i_SinceDate", MySqlDbType.Timestamp).Value = sinceDate ?? DBNull.Value;
                cmd.Parameters.Add("i_ToDate", MySqlDbType.Timestamp).Value = toDate;
                cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
                using (var cmd = MySqlDbAccess.GetCommand("TopicStatus_Delete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("i_TopicStatusID", MySqlDbType.Int32).Value = topicStatusID;
                    MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("TopicStatus_Edit"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_TopicStatusID", MySqlDbType.Int32).Value = topicStatusID;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// List all Topics of the Current Board
        /// </summary>
        /// <param name="boardID">The board ID.</param>
        /// <returns></returns>
        public static DataTable TopicStatus_List([NotNull] string connectionString, [NotNull] object boardID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("TopicStatus_List"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardID;
                return MySqlDbAccess.GetData(cmd, connectionString);
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
                using (var cmd = MySqlDbAccess.GetCommand("TopicStatus_Save"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("i_TopicStatusID", MySqlDbType.Int32).Value = topicStatusID;
                    cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardID;
                    cmd.Parameters.Add("i_TopicStatusName", MySqlDbType.VarChar).Value = topicStatusName;
                    cmd.Parameters.Add("i_DefaultDescription", MySqlDbType.VarChar).Value = defaultDescription;

                    MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
                }
            }
            catch
            {
                // Ignore any errors in this method
            }
        }

        //ABOT NEW 16.04.04:Delete all topic's messages
        private static void topic_deleteAttachments([NotNull] string connectionString, object topicID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_listmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;

                using (DataTable dt = MySqlDbAccess.GetData(cmd, connectionString))
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

            string uploadDir =
                HostingEnvironment.MapPath(
                    String.Concat(
                        BaseUrlBuilder.ServerFileRoot,
                        YafBoardFolders.Current.Uploads,
                        "/",
                        YafBoardFolders.Current.Topics));

            try
            {
                string topicImage = string.Empty;
                var dt = topic_info(connectionString, topicID, false);
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

        public static void topic_delete([NotNull] string connectionString, object topicID, object eraseTopic)
        {
            if (eraseTopic == null)
            {
                eraseTopic = 0;
            }

            if (eraseTopic.ToType<bool>())
            {
                topic_deleteAttachments(connectionString, topicID);

                topic_deleteimages(connectionString, (int)topicID);
            }
            using (var cmd = MySqlDbAccess.GetCommand("topic_delete"))
            {

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;
                cmd.Parameters.Add("i_EraseTopic", MySqlDbType.Byte).Value = eraseTopic;
                cmd.Parameters.Add("i_UpdateLastPost", MySqlDbType.Byte).Value = 1;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static DataTable topic_findprev([NotNull] string connectionString, object topicID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_findprev"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable topic_findnext([NotNull] string connectionString, object topicID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_findnext"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void topic_lock([NotNull] string connectionString, object topicID, object locked)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_lock"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;
                cmd.Parameters.Add("i_Locked", MySqlDbType.Byte).Value = locked;
                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// The topic_save.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="styles">
        /// The styles.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageDescription">
        /// The message description.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <param name="posted">
        /// The posted.
        /// </param>
        /// <param name="blogPostID">
        /// The blog post id.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <param name="tags">
        /// The tags.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public static long topic_save(
            [NotNull] string connectionString,
            object forumID,
            object subject,
            object status,
            object styles,
            object description,
            object message,
            [CanBeNull] object messageDescription,
            object userID,
            object priority,
            object userName,
            object ip,
            object posted,
            object blogPostID,
            object flags,
            out long messageID,
            string tags)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_ForumID", MySqlDbType.Int32).Value = forumID;
                cmd.Parameters.Add("i_Subject", MySqlDbType.VarChar).Value = subject;
                cmd.Parameters.Add("i_status", MySqlDbType.VarChar).Value = status;
                cmd.Parameters.Add("i_Styles", MySqlDbType.VarChar).Value = styles;
                cmd.Parameters.Add("i_Description", MySqlDbType.VarChar).Value = description;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_Message", MySqlDbType.Text).Value = message;
                cmd.Parameters.Add("i_Priority", MySqlDbType.Int16).Value = priority;
                cmd.Parameters.Add("i_UserName", MySqlDbType.VarChar).Value = userName;
                cmd.Parameters.Add("i_IP", MySqlDbType.VarChar).Value = ip;
                cmd.Parameters.Add("i_Posted", MySqlDbType.DateTime).Value = posted;
                cmd.Parameters.Add("i_BlogPostID", MySqlDbType.VarChar).Value = blogPostID;
                cmd.Parameters.Add("i_Flags", MySqlDbType.Int32).Value = flags;
                cmd.Parameters.Add("i_MessageDescription", MySqlDbType.VarChar).Value = messageDescription;
                cmd.Parameters.Add("i_Tags", MySqlDbType.VarChar).Value = tags;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                DataTable dt = MySqlDbAccess.GetData(cmd, connectionString);
                messageID = long.Parse(dt.Rows[0]["MessageID"].ToString());
                return long.Parse(dt.Rows[0]["TopicID"].ToString());
            }
        }

        public static DataRow topic_info([NotNull] string connectionString, object topicID, [NotNull] bool getTags)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_info"))
            {
                if (topicID == null)
                {
                    topicID = DBNull.Value;
                }

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;
                cmd.Parameters.Add("i_ShowDeleted", MySqlDbType.Byte).Value = 0;
                cmd.Parameters.Add("i_GetTags", MySqlDbType.Byte).Value = getTags;
                using (DataTable dt = MySqlDbAccess.GetData(cmd, connectionString))
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// The topic_imagesave.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <param name="imageUrl">
        /// The image url.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="topicImageType">
        /// The topic image type.
        /// </param>
        public static void topic_imagesave(
            [NotNull] string connectionString, object topicID, [NotNull] object imageUrl, Stream stream, object topicImageType)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_imagesave"))
            {
                byte[] data = null;
                if (stream != null)
                {
                    data = new byte[stream.Length];
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    stream.Read(data, 0, (int)stream.Length);
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_TopicID", MySqlDbType.Int32).Value = topicID;
                cmd.Parameters.Add("i_ImageUrl", MySqlDbType.VarChar).Value = imageUrl;
                cmd.Parameters.Add("i_Stream", MySqlDbType.Blob).Value = data;
                cmd.Parameters.Add("i_TopicImageType", MySqlDbType.VarChar).Value = topicImageType;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// The topic_findduplicate.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="topicName">
        /// The topic name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int topic_findduplicate([NotNull] string connectionString, object topicName)
        {
            using (var cmd = MySqlDbAccess.GetCommand("topic_findduplicate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_TopicName", MySqlDbType.VarChar).Value = topicName;
                return Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        /// <summary>
        /// The topic_favorite_details.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
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
        /// The since date.
        /// </param>
        /// <param name="toDate">
        /// The to date.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// The find last read.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>  containing the current user's favorite topics with details..
        /// </returns>
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
            using (var cmd = MySqlDbAccess.GetCommand("topic_favorite_details"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_CategoryID", MySqlDbType.Int32).Value = categoryId;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserId ?? DBNull.Value;
                cmd.Parameters.Add("i_SinceDate", MySqlDbType.Timestamp).Value = sinceDate ?? DBNull.Value;
                cmd.Parameters.Add("i_ToDate", MySqlDbType.Timestamp).Value = toDate;
                cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_FindLastRead", MySqlDbType.Byte).Value = findLastRead;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("topic_favorite_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("i_UserID", MySqlDbType.Int32)).Value = userID;
                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("topic_favorite_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("i_UserID", MySqlDbType.Int32)).Value = userID;
                cmd.Parameters.Add(new MySqlParameter("i_TopicID", MySqlDbType.Int32)).Value = topicID;
                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("topic_favorite_add"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("i_UserID", MySqlDbType.Int32)).Value = userID;
                cmd.Parameters.Add(new MySqlParameter("i_TopicID", MySqlDbType.Int32)).Value = topicID;
                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("replace_words_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_ID", MySqlDbType.Int32).Value = id ?? DBNull.Value;

                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("replace_words_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_ID", MySqlDbType.Int32).Value = id ?? DBNull.Value; 
                cmd.Parameters.Add("i_BadWord", MySqlDbType.VarChar).Value = badword;
                cmd.Parameters.Add("i_GoodWord", MySqlDbType.VarChar).Value = goodword;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Deletes a bad/good word
        /// </summary>
        /// <param name="ID">ID of bad/good word to delete</param>
        public static void replace_words_delete([NotNull] string connectionString, object ID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("replace_words_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_ID", MySqlDbType.Int32).Value = ID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        #endregion

        #region IgnoreUser

        public static void user_addignoreduser([NotNull] string connectionString, object userId, object ignoredUserId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_addignoreduser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserId", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("i_IgnoredUserId", MySqlDbType.Int32).Value = ignoredUserId;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }     

        public static bool user_isuserignored([NotNull] string connectionString, object userId, object ignoredUserId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_isuserignored"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserId", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("i_IgnoredUserId", MySqlDbType.Int32).Value = ignoredUserId;

                return Convert.ToBoolean(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        public static DataTable user_ignoredlist([NotNull] string connectionString, object userId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_ignoredlist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_UserId", MySqlDbType.Int32).Value = userId;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        #endregion
     
        /// <summary>
        /// To return a rather rarely updated active user data
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="showPendingMails">
        /// The show pending mails.
        /// </param>
        /// <param name="showPendingBuddies">
        /// The show pending buddies.
        /// </param>
        /// <param name="showUnreadPMs">
        /// The show unread p ms.
        /// </param>
        /// <param name="showUserAlbums">
        /// The show user albums.
        /// </param>
        /// <param name="styledNicks">
        /// The styled nicks.
        /// </param>
        /// <returns>
        /// The <see cref="DataRow"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        /// <exception cref="ApplicationException">
        /// </exception>
        public static DataRow user_lazydata(
            [NotNull] string connectionString,
            object userId,
            object boardId,
            bool showPendingMails,
            bool showPendingBuddies,
            bool showUnreadPMs,
            bool showUserAlbums,
            bool styledNicks)
        {
            int nTries = 0;
            while (true)
            {
                try
                {
                    using (var cmd = MySqlDbAccess.GetCommand("user_lazydata"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("I_UserID", MySqlDbType.Int32).Value = userId;
                        cmd.Parameters.Add("I_BoardID", MySqlDbType.Int32).Value = boardId;
                        cmd.Parameters.Add("I_ShowPendingMails", MySqlDbType.Byte).Value = showPendingBuddies;
                        cmd.Parameters.Add("I_ShowPendingBuddies", MySqlDbType.Byte).Value = showPendingMails;
                        cmd.Parameters.Add("I_ShowUnreadPMs", MySqlDbType.Byte).Value = showUnreadPMs;
                        cmd.Parameters.Add("I_ShowUserAlbums", MySqlDbType.Byte).Value = showUserAlbums;
                        cmd.Parameters.Add("I_ShowUserStyle", MySqlDbType.Byte).Value = styledNicks;

                        return MySqlDbAccess.GetData(cmd, connectionString).Rows[0];
                    }
                }
                catch (ArgumentOutOfRangeException xx)
                {
                    if (nTries < 3)
                    {
                        /// Transaction (Process ID XXX) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
                    }
                    else
                        throw new ArgumentOutOfRangeException(
                            string.Format(
                                "Number of DataTable columns from DataReader cannot be null. Trys -{0}", nTries),
                            xx);
                }
                catch (MySqlException x)
                {
                    if (x.Number == 1213 && nTries < 3)
                    {
                        /// Transaction (Process ID XXX) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
                    }
                    else
                        throw new ApplicationException(
                            string.Format("Sql Exception with error number {0} (Tries={1})", x.Number, nTries), x);
                }
                ++nTries;
            }
        }

        /// <summary>
        /// The user_listmembers.
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
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <param name="rankId">
        /// The rank id.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use styled nicks.
        /// </param>
        /// <param name="lastUserId">
        /// The last user id.
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
        /// <param name="sortName">
        /// The sort name.
        /// </param>
        /// <param name="sortRank">
        /// The sort rank.
        /// </param>
        /// <param name="sortJoined">
        /// The sort joined.
        /// </param>
        /// <param name="sortPosts">
        /// The sort posts.
        /// </param>
        /// <param name="sortLastVisit">
        /// The sort last visit.
        /// </param>
        /// <param name="numPosts">
        /// The num posts.
        /// </param>
        /// <param name="numPostCompare">
        /// The num post compare.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
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
            string sliterals = literals.ToString();
            if (sliterals.IsSet() && sliterals.Contains("\0"))
            {
                sliterals = null;
            }

            DataTable dt = null;
            using (var cmd = MySqlDbAccess.GetCommand("user_listmembers"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("I_UserID", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("I_Approved", MySqlDbType.Byte).Value = approved;
                cmd.Parameters.Add("I_GroupID", MySqlDbType.Int32).Value = groupId;
                cmd.Parameters.Add("I_RankID", MySqlDbType.Int32).Value = rankId;
                cmd.Parameters.Add("I_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("I_Literals", MySqlDbType.VarChar).Value = sliterals;
                cmd.Parameters.Add("I_Exclude", MySqlDbType.Byte).Value = exclude;
                cmd.Parameters.Add("I_BeginsWith", MySqlDbType.Byte).Value = beginsWith;
                cmd.Parameters.Add("I_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("I_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("I_SortName", MySqlDbType.Int32).Value = sortName ?? 0;
                cmd.Parameters.Add("I_SortRank", MySqlDbType.Int32).Value = sortRank ?? 0;
                cmd.Parameters.Add("I_SortJoined", MySqlDbType.Int32).Value = sortJoined ?? 0;
                cmd.Parameters.Add("I_SortPosts", MySqlDbType.Int32).Value = sortPosts ?? 0;
                cmd.Parameters.Add("I_SortLastVisit", MySqlDbType.Int32).Value = sortLastVisit ?? 0;
                cmd.Parameters.Add("I_NumPosts", MySqlDbType.Int32).Value = numPosts ?? 0;
                cmd.Parameters.Add("I_NumPostsCompare", MySqlDbType.Int32).Value = numPostCompare ?? 0;
                dt = MySqlDbAccess.GetData(cmd, connectionString);
                cmd.Parameters.Clear();
            }

            using (var cmd = MySqlDbAccess.GetCommand("user_listmembers_result"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("I_UserID", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("I_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("I_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("I_SortName", MySqlDbType.Int32).Value = sortName ?? 0;
                cmd.Parameters.Add("I_SortJoined", MySqlDbType.Int32).Value = sortJoined ?? 0;
                cmd.Parameters.Add("I_SortLastVisit", MySqlDbType.Int32).Value = sortLastVisit ?? 0;
                cmd.Parameters.Add("I_NumPostsCompare", MySqlDbType.Int32).Value = numPostCompare ?? 0;
                cmd.Parameters.Add("I_NumPosts", MySqlDbType.Int32).Value = numPosts ?? 0;
                cmd.Parameters.Add("I_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("I_Approved", MySqlDbType.Byte).Value = approved;
                cmd.Parameters.Add("I_GroupID", MySqlDbType.Int32).Value = groupId;
                cmd.Parameters.Add("I_RankID", MySqlDbType.Int32).Value = rankId;
                cmd.Parameters.Add("I_BeginsWith", MySqlDbType.Byte).Value = beginsWith;
                cmd.Parameters.Add("I_Literals", MySqlDbType.VarChar).Value = sliterals;
                cmd.Parameters.Add("I_SortRank", MySqlDbType.Int32).Value = sortRank ?? 0;
                cmd.Parameters.Add("I_SortPosts", MySqlDbType.Int32).Value = sortPosts ?? 0;
                cmd.Parameters.Add("I_Exclude", MySqlDbType.Byte).Value = exclude;
                if (dt != null && dt.Rows.Count > 0)
                {
                    cmd.Parameters.Add("i_firstselectuserid", MySqlDbType.VarChar).Value =
                        dt.Rows[0]["ici_firstselectuserid"];
                    cmd.Parameters.Add("i_firstselectlastvisit", MySqlDbType.DateTime).Value =
                        dt.Rows[0]["ici_firstselectlastvisit"];
                    cmd.Parameters.Add("i_firstselectjoined", MySqlDbType.DateTime).Value =
                        dt.Rows[0]["ici_firstselectjoined"];
                    cmd.Parameters.Add("i_firstselectrankid", MySqlDbType.Int32).Value =
                        dt.Rows[0]["ici_firstselectrankid"];
                    cmd.Parameters.Add("i_firstselectposts", MySqlDbType.Int32).Value =
                        dt.Rows[0]["ici_firstselectposts"];
                }
                else
                {
                    cmd.Parameters.Add("i_firstselectuserid", MySqlDbType.VarChar).Value = DBNull.Value;
                    cmd.Parameters.Add("i_firstselectlastvisit", MySqlDbType.DateTime).Value = DBNull.Value;
                    cmd.Parameters.Add("i_firstselectjoined", MySqlDbType.DateTime).Value = DBNull.Value;
                    cmd.Parameters.Add("i_firstselectrankid", MySqlDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("i_firstselectposts", MySqlDbType.Int32).Value = DBNull.Value;
                }

                return MySqlDbAccess.GetData(cmd, connectionString);
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
        /// <returns>
        /// </returns>
        public static DataTable user_list([NotNull] string connectionString, object boardID, object userID, object approved)
        {
            return user_list(connectionString, boardID, userID, approved, null, null, false);
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
        /// <param name="useStyledNicks">
        /// Return style info.
        /// </param> 
        /// <returns>
        /// </returns>
        public static DataTable user_list(
            [NotNull] string connectionString, object boardID, object userID, object approved, object useStyledNicks)
        {
            return user_list(connectionString, boardID, userID, approved, null, null, useStyledNicks);
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
        /// <returns>
        /// </returns>
        public static DataTable user_list(
            [NotNull] string connectionString, object boardID, object userID, object approved, object groupID, object rankID)
        {
            return user_list(connectionString, boardID, userID, approved, null, null, false);
        }

        /// <summary>
        /// The user_list.
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
        /// <returns>
        /// </returns>
        public static DataTable user_list(
            [NotNull] string connectionString,
            object boardId,
            object userId,
            object approved,
            object groupId,
            object rankId,
            object useStyledNicks)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId ?? DBNull.Value;
                cmd.Parameters.Add("i_Approved", MySqlDbType.Byte).Value = approved ?? DBNull.Value;
                cmd.Parameters.Add("i_GroupID", MySqlDbType.Int32).Value = groupId ?? DBNull.Value;
                cmd.Parameters.Add("i_RankID", MySqlDbType.Int32).Value = rankId ?? DBNull.Value;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                return MySqlDbAccess.GetData(cmd, false, connectionString);
            }
        }

        public static DataTable user_pagedlist(
            [NotNull] string connectionString,
            object boardId,
            object userId,
            object approved,
            object groupId,
            object rankId,
            object useStyledNicks,
            object pageIndex,
            object pageSize)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_pagedlist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId ?? DBNull.Value;
                cmd.Parameters.Add("i_Approved", MySqlDbType.Byte).Value = approved ?? DBNull.Value;
                cmd.Parameters.Add("i_GroupID", MySqlDbType.Int32).Value = groupId ?? DBNull.Value;
                cmd.Parameters.Add("i_RankID", MySqlDbType.Int32).Value = rankId ?? DBNull.Value;
                cmd.Parameters.Add("i_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                cmd.Parameters.Add("i_PageIndex", MySqlDbType.Int32).Value = pageIndex;
                cmd.Parameters.Add("i_PageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                return MySqlDbAccess.GetData(cmd, false, connectionString);
            }
        }       

        
        public static void user_delete([NotNull] string connectionString, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

         //Is not used ?
        /* public static void user_setinfo( int boardId, System.Web.Security.MembershipUser user )
        {
            using (var cmd = MySqlDbAccess.GetCommand(String.Format("UPDATE {0} SET Name=@i_UserName,Email=@i_Email where BoardID=@i_BoardID and ProviderUserKey=@i_ProviderUserKey", MySqlDbAccess.GetObjectName("User")), true))
            {
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add( "@i_UserName", MySqlDbType.VarChar ).Value = user.UserName;
                cmd.Parameters.Add( "@i_Email", MySqlDbType.VarChar ).Value = user.Email;
                cmd.Parameters.Add( "@i_BoardID", MySqlDbType.Int32 ).Value = boardId;
                cmd.Parameters.Add( "@i_ProviderUserKey", MySqlDbType.VarChar ).Value = user.ProviderUserKey;
                
                MySqlDbAccess.ExecuteNonQuery(cmd,connectionString);
            }
        }*/
        
        public static void user_deleteold([NotNull] string connectionString, object boardId, object days)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_deleteold"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_Days", MySqlDbType.Int32).Value = days;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void user_approve([NotNull] string connectionString, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_approve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static void user_approveall([NotNull] string connectionString, object boardId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_approveall"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("user_getsignaturedata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardID;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        /// <summary>
        /// Returns data about albums: allowed number of images and albums
        /// </summary>
        /// <param name="userID">The userID</param>
        /// <param name="boardID">The boardID</param>  
        public static DataTable user_getalbumsdata([NotNull] string connectionString, object userID, object boardID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_getalbumsdata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardID;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static bool user_changepassword(
            [NotNull] string connectionString, object userID, object oldPassword, object newPassword)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_changepassword"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_OldPassword", MySqlDbType.VarChar).Value = oldPassword;
                cmd.Parameters.Add("i_NewPassword", MySqlDbType.VarChar).Value = newPassword;

                return (bool)MySqlDbAccess.ExecuteScalar(cmd, connectionString);
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


            using (var cmd = MySqlDbAccess.GetCommand("user_save"))
            {
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
                if (dSTUser == null)
                {
                    dSTUser = DBNull.Value;
                }
                if (isHidden == null)
                {
                    isHidden = DBNull.Value;
                }
                if (notificationType == null)
                {
                    notificationType = DBNull.Value;
                }
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserName", MySqlDbType.VarChar).Value = userName ?? DBNull.Value;
                cmd.Parameters.Add("i_DisplayName", MySqlDbType.VarChar).Value = displayName ?? DBNull.Value;
                cmd.Parameters.Add("i_Email", MySqlDbType.VarChar).Value = email ?? DBNull.Value;
                cmd.Parameters.Add("i_TimeZone", MySqlDbType.Int32).Value = timeZone;
                cmd.Parameters.Add("i_LanguageFile", MySqlDbType.VarChar).Value = languageFile ?? DBNull.Value;
                cmd.Parameters.Add("i_Culture", MySqlDbType.VarChar).Value = culture;
                cmd.Parameters.Add("i_ThemeFile", MySqlDbType.VarChar).Value = themeFile ?? DBNull.Value;
                cmd.Parameters.Add("i_UseSingleSignOn", MySqlDbType.Byte).Value = useSingleSignOn;
                cmd.Parameters.Add("i_TextEditor", MySqlDbType.VarChar).Value = textEditor;
                cmd.Parameters.Add("i_OverrideDefaultTheme", MySqlDbType.Byte).Value = overrideDefaultThemes;
                cmd.Parameters.Add("i_Approved", MySqlDbType.Byte).Value = approved;
                cmd.Parameters.Add("i_PMNotification", MySqlDbType.Byte).Value = pmNotification;
                cmd.Parameters.Add("i_NotificationType", MySqlDbType.Int32).Value = notificationType;
                cmd.Parameters.Add("i_ProviderUserKey", MySqlDbType.VarChar).Value = DBNull.Value;
                cmd.Parameters.Add("i_AutoWatchTopics", MySqlDbType.Byte).Value = autoWatchTopics;
                cmd.Parameters.Add("i_DSTUser", MySqlDbType.Byte).Value = dSTUser;
                cmd.Parameters.Add("i_HideUser", MySqlDbType.Byte).Value = isHidden;
                cmd.Parameters.Add("i_TopicsPerPage", MySqlDbType.Int32).Value = topicsPerPage;
                cmd.Parameters.Add("i_PostsPerPage", MySqlDbType.Int32).Value = postsPerPage;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("user_adminsave"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("i_Name", MySqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("i_DisplayName", MySqlDbType.VarChar).Value = displayName;
                cmd.Parameters.Add("i_Email", MySqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("i_Flags", MySqlDbType.Int32).Value = flags;
                cmd.Parameters.Add("i_RankID", MySqlDbType.Int32).Value = rankID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        public static DataTable user_emails([NotNull] string connectionString, object boardId, object groupID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_emails"))
            {

                if (groupID == null)
                {
                    groupID = DBNull.Value;
                }


                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_GroupID", MySqlDbType.Int32).Value = groupID;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable user_accessmasks([NotNull] string connectionString, object boardId, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_accessmasks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;

                return userforumaccess_sort_list(
                    connectionString, MySqlDbAccess.GetData(cmd, connectionString), 0, 0, 0);
            }
        }

        public static DataTable user_accessmasksbyforum([NotNull] string connectionString, object boardId, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_accessmasksbyforum"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static DataTable user_accessmasksbygroup([NotNull] string connectionString, object boardId, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_accessmasksbygroup"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        //adds some convenience while editing group's access rights (indent forums)
        private static DataTable userforumaccess_sort_list(
            [NotNull] string connectionString, DataTable listSource, int parentID, int categoryID, int startingIndent)
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
            userforumaccess_sort_list_recursive(
                connectionString, dv.ToTable(), listDestination, parentID, categoryID, startingIndent);
            return listDestination;
        }

        private static void userforumaccess_sort_list_recursive(
            [NotNull] string connectionString,
            DataTable listSource,
            DataTable listDestination,
            int parentID,
            int categoryID,
            int currentIndent)
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

                    // recurse through the list...
                    userforumaccess_sort_list_recursive(
                        connectionString,
                        listSource,
                        listDestination,
                        (int)row["ForumID"],
                        categoryID,
                        currentIndent + 1);
                }
            }
        }
        
        
        public static DataTable user_avatarimage([NotNull] string connectionString, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_avatarimage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        /*	public static int user_get( int boardId, object providerUserKey )
            {
                using (var cmd = MySqlDbAccess.GetCommand( "select UserID from {databaseName}.{objectQualifier}User where BoardID=i_BoardID and ProviderUserKey=i_ProviderUserKey", true ) )
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32 ).Value = boardId;
                    cmd.Parameters.AddWithValue( "i_ProviderUserKey", providerUserKey );
                    return ( int )MySqlDbAccess.ExecuteScalar(cmd,connectionString);
                }
            }*/

        public static int user_get([NotNull] string connectionString, int boardId, object providerUserKey)
        {

            using (var cmd = MySqlDbAccess.GetCommand("user_get"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_ProviderUserKey", MySqlDbType.VarChar).Value = providerUserKey;

                return Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmd, connectionString) ?? 0);
            }
        }


        public static string user_getsignature([NotNull] string connectionString, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_getsignature"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;

                return MySqlDbAccess.ExecuteScalar(cmd, connectionString).ToString();
            }
        }        

        public static void user_deleteavatar([NotNull] string connectionString, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_deleteavatar"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        //Not in use
        /* public static bool user_register( object boardId, object userName, object password, object hash, object email, object location, object homePage, object timeZone, bool approved )
        {
            using ( MySqlDbConnectionManager connMan = new MySqlDbConnectionManager() )
            {
                using ( MySqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( DBAccess.IsolationLevel ) )
                {
                    try
                    {
                        using (var cmd = MySqlDbAccess.GetCommand( "user_save", connMan.DBConnection ) )
                        {
                            cmd.Transaction = trans;
                            cmd.CommandType = CommandType.StoredProcedure;
                            int UserID = 0;
                            cmd.Parameters.Add( "i_UserID", MySqlDbType.Int32 ).Value = UserID;
                            cmd.Parameters.Add( "i_BoardID", MySqlDbType.Int32 ).Value = boardId;
                            cmd.Parameters.Add( "i_UserName", MySqlDbType.VarChar ).Value = userName;							
                            cmd.Parameters.Add( "i_Email", MySqlDbType.VarChar ).Value = email;                           
                            cmd.Parameters.Add( "i_TimeZone", MySqlDbType.Int32 ).Value = timeZone;
                            cmd.Parameters.Add("i_LanguageFile", MySqlDbType.VarChar).Value = DBNull.Value;
                            cmd.Parameters.Add("i_ThemeFile", MySqlDbType.VarChar).Value = DBNull.Value;
                            cmd.Parameters.Add("i_OverrideDefaultTheme", MySqlDbType.Byte).Value = DBNull.Value;
                            cmd.Parameters.Add( "i_Approved", MySqlDbType.Byte ).Value = approved;
                            cmd.Parameters.Add( "i_PMNotification", MySqlDbType.Byte ).Value = 1;
                            cmd.Parameters.Add( "i_ProviderUserKey", MySqlDbType.VarChar ).Value = DBNull.Value;
                            
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                    }
                    catch ( Exception x )
                    {
                        trans.Rollback();
                        VZF.Classes.Data.DB.eventlog_create( null, "user_register in VZF.Classes.Data.DB.cs", x, EventLogTypes.Error );
                        return false;
                    }
                }
            }

            return true;
        } */

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
                using (var cmd = MySqlDbAccess.GetCommand("user_aspnet"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                    cmd.Parameters.Add("i_UserName", MySqlDbType.VarChar).Value = userName;
                    cmd.Parameters.Add("i_DisplayName", MySqlDbType.VarChar).Value = displayName;
                    cmd.Parameters.Add("i_Email", MySqlDbType.VarChar).Value = email;
                    cmd.Parameters.Add("i_ProviderUserKey", MySqlDbType.VarChar).Value = providerUserKey;
                    cmd.Parameters.Add("i_IsApproved", MySqlDbType.Byte).Value = isApproved;
                    cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                    return Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
                }
            }
            catch (Exception x)
            {
                Db.eventlog_create(
                    connectionString, null, "user_aspnet in VZF.Classes.Data.DB.cs", x, EventLogTypes.Error);
                return 0;
            }
        }

        public static int? user_guest([NotNull] string connectionString, object boardId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_guest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;

                return MySqlDbAccess.ExecuteScalar(cmd, connectionString).ToType<int?>();
            }
        }

        public static DataTable user_activity_rank(
            [NotNull] string connectionString, object boardId, object startDate, object displayNumber)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_activity_rank"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_BoardID", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("i_DisplayNumber", MySqlDbType.Int32).Value = displayNumber;
                cmd.Parameters.Add("i_StartDate", MySqlDbType.DateTime).Value = startDate;

                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static void user_addpoints(
            [NotNull] string connectionString, [NotNull] object userID, [CanBeNull] object fromUserID, [NotNull] object points)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_addpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_FromUserID", MySqlDbType.Int32).Value = fromUserID;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;
                cmd.Parameters.Add("i_Points", MySqlDbType.Int32).Value = points;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }
 
        public static int user_getpoints([NotNull] string connectionString, object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_getpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_UserID", MySqlDbType.Int32).Value = userID;

                return (int)MySqlDbAccess.ExecuteScalar(cmd, connectionString);
            }
        }

        /// <summary>
        /// Returns the number of times a specific user with the provided UserID  has thanked others
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static int user_getthanks_from([NotNull] string connectionString, object userID, object pageUserId)
        {

            using (var cmd = MySqlDbAccess.GetCommand("user_getthanks_from"))
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserId;
                return Convert.ToInt32(MySqlDbAccess.ExecuteScalar(cmd, connectionString));
            }
        }

        //<summary> Returns the number of times and posts that other users have thanked the 
        // user with the provided userID.
        public static int[] user_getthanks_to([NotNull] string connectionString, object userID, object pageUserId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_getthanks_to"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlParameter paramThanksToNumber = new MySqlParameter("I_ThanksToNumber", MySqlDbType.Int32);
                paramThanksToNumber.Direction = ParameterDirection.Output;
                MySqlParameter paramThanksToPostsNumber = new MySqlParameter("I_ThanksToPostsNumber", MySqlDbType.Int32);
                paramThanksToPostsNumber.Direction = ParameterDirection.Output;
                cmd.Parameters.Add("I_UserID", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_PageUserID", MySqlDbType.Int32).Value = pageUserId;
                cmd.Parameters.Add(paramThanksToNumber);
                cmd.Parameters.Add(paramThanksToPostsNumber);
                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);

                int thanksToPostsNumber, thanksToNumber;
                if (paramThanksToNumber.Value == DBNull.Value)
                {
                    thanksToNumber = 0;
                    thanksToPostsNumber = 0;
                }
                else
                {
                    thanksToPostsNumber = Convert.ToInt32(paramThanksToPostsNumber.Value);
                    thanksToNumber = Convert.ToInt32(paramThanksToNumber.Value);
                }
                return new int[] { thanksToNumber, thanksToPostsNumber };
            }
        }  
        
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
            using (var cmd = MySqlDbAccess.GetCommand("readtopic_addorupdate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_userid", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_topicid", MySqlDbType.Int32).Value = topicID;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
             using (var cmd = MySqlDbAccess.GetCommand("readtopic_delete"))
             {
                 cmd.CommandType = CommandType.StoredProcedure;
                 cmd.Parameters.Add("i_userid", MySqlDbType.Int32).Value = userID;
                 MySqlDbAccess.ExecuteNonQuery(cmd,connectionString);
             }
         }
         */

        public static DateTime? User_LastRead([NotNull] string connectionString, [NotNull] object userID)
        {
            using (var cmd = MySqlDbAccess.GetCommand("user_lastread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("i_userid", MySqlDbType.Int32)).Value = userID;

                var tableLastRead = MySqlDbAccess.ExecuteScalar(cmd, connectionString);

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
            using (var cmd = MySqlDbAccess.GetCommand("readtopic_lastread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("i_userid", MySqlDbType.Int32)).Value = userID;
                cmd.Parameters.Add(new MySqlParameter("i_topicid", MySqlDbType.Int32)).Value = topicID;

                var tableLastRead = MySqlDbAccess.ExecuteScalar(cmd, connectionString);

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
            using (var cmd = MySqlDbAccess.GetCommand("readforum_addorupdate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("i_userid", MySqlDbType.Int32).Value = userID;
                cmd.Parameters.Add("i_forumid", MySqlDbType.Int32).Value = forumID;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
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
             using (var cmd = MySqlDbAccess.GetCommand("readforum_delete"))
             {
                 cmd.CommandType = CommandType.StoredProcedure;
                 cmd.Parameters.Add(new MySqlParameter("i_userid", MySqlDbType.Int32)).Value = userID;
                 MySqlDbAccess.ExecuteNonQuery(cmd,connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("readforum_lastread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("i_userid", MySqlDbType.Int32)).Value = userID;
                cmd.Parameters.Add(new MySqlParameter("i_forumid", MySqlDbType.Int32)).Value = forumID;

                var tableLastRead = MySqlDbAccess.ExecuteScalar(cmd, connectionString);

                return tableLastRead != null && tableLastRead != DBNull.Value
                           ? (DateTime)tableLastRead
                           : DateTimeHelper.SqlDbMinTime();
            }
        }   

        # region VZ-Team additions

        /// <summary>
        /// The rsstopic_list.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable rsstopic_list([NotNull] string connectionString, int forumId, int start, int count)
        {
            using (var cmd = MySqlDbAccess.GetCommand("rsstopic_list"))
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("i_ForumID", MySqlDbType.Int32)).Value = forumId;
                cmd.Parameters.Add(new MySqlParameter("i_Start", MySqlDbType.Int32)).Value = start;
                cmd.Parameters.Add(new MySqlParameter("i_Limit", MySqlDbType.Int32)).Value = count;

                return MySqlDbAccess.GetData(cmd, false, connectionString);
            }
        }

        /// <summary>
        /// The db_getstats.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        public static void db_getstats([NotNull] string connectionString)
        {
            using (
                var cmd =
                    new MySqlCommand(
                        string.Format(
                            "ANALYZE TABLE {0}.{1}user;", MySqlDbAccess.DatabaseSchemaName, Config.DatabaseObjectQualifier)))
            {
                cmd.CommandType = CommandType.Text;

                // up the command timeout...
                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                // run it...
                MySqlDbAccess.ExecuteNonQuery(cmd, false, connectionString);
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
        public static string db_getstats_new([NotNull] string connectionString)
        {
            try
            {
                using (var connMan = new MySqlDbConnectionManager(connectionString))
                {
                    connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(getStats_InfoMessage);
                    using (
                        var cmd =
                            new MySqlCommand(
                                string.Format(
                                    "ANALYZE TABLE {0}.{1}user;",
                                    MySqlDbAccess.DatabaseSchemaName,
                                    Config.DatabaseObjectQualifier)))
                    {
                        cmd.CommandType = CommandType.Text;

                        // up the command timeout...
                        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                        // run it...
                        MySqlDbAccess.ExecuteNonQuery(cmd, false, connectionString);
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
        /// The db_getstats_table.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable db_getstats_table([NotNull] string connectionString)
        {
            using (var cmd = new MySqlCommand(string.Format("SHOW TABLE STATUS FROM {0};", MySqlDbAccess.DatabaseSchemaName)))
            {
                cmd.CommandType = CommandType.Text;

                return MySqlDbAccess.GetData(cmd, false, connectionString);
            }
        }

        /// <summary>
        /// The db_getstats_alltables.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable db_getstats_alltables([NotNull] string connectionString)
        {
            var sb = new StringBuilder();
            sb.Append(
                string.Format(
                    "SELECT table_name FROM  INFORMATION_SCHEMA.SCHEMATA s LEFT JOIN INFORMATION_SCHEMA.TABLES t ON s.schema_name = t.table_schema WHERE t.engine ='InnoDB'  AND t.TABLE_TYPE='BASE TABLE' AND t.table_schema='{0}' ",
                    MySqlDbAccess.DatabaseSchemaName));
            sb.Append(";");

            using (var cmd = new MySqlCommand(sb.ToString()))
            {
                cmd.CommandType = CommandType.Text;
                return MySqlDbAccess.GetData(cmd, false, connectionString);
            }
        }

        /// <summary>
        /// The db_getstats_warning.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_getstats_warning()
        {
            return string.Empty;
        }

        /// <summary>
        /// The db_getstats_tablex.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_getstats_tablex([NotNull] string connectionString)
        {
            int offset = 15;
            var sb = new StringBuilder();
            sb.Append("___________________________________________________________________________________      ");
            DataTable tables = Db.db_getstats_alltables(connectionString);
            foreach (DataRow drtables in tables.Rows)
            {
                using (
                    var cmd =
                        new MySqlCommand(String.Format("ANALYZE TABLE {0}.{1};", MySqlDbAccess.DatabaseSchemaName, drtables[0])))
                {
                    cmd.CommandType = CommandType.Text;

                    // up the command timeout...
                    cmd.CommandTimeout = 9999;

                    // run it...
                    DataTable dt = MySqlDbAccess.GetData(cmd, false, connectionString);
                    foreach (DataRow dr in dt.Rows)
                    {
                        object[] oa = dr.ItemArray;
                        for (int i = 0; i < oa.Length; i++)
                        {
                            sb.Append("|");

                            switch (i)
                            {
                                case 0:
                                    sb.Append(" Table=");
                                    offset = 30;
                                    break;
                                case 1:
                                    sb.Append(" Op=");
                                    offset = 10;
                                    break;
                                case 2:
                                    sb.Append(" Msg_type=");
                                    offset = 10;
                                    break;
                                case 3:
                                    sb.Append(" Msg_text=");
                                    offset = 10;
                                    break;
                            }

                            sb.Append(oa[i]);
                            int strl = offset - oa[i].ToString().Length;
                            for (int i1 = 1; i1 < strl; i1++)
                            {
                                sb.Append(" ");
                            }
                        }

                        sb.Append("\r\n");
                    }

                    sb.Append("___________________________________________________________________________________");
                    sb.Append("\r\n");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether btn reindex visible.
        /// </summary>
        public static bool btnReindexVisible
        {
            get
            {
                return true;
            }
        }


        // DB Maintenance page panel visibility
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

        /// <summary>
        /// Gets a value indicating whether panel recovery mode.
        /// </summary>
        public static bool PanelRecoveryMode
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether panel reindex.
        /// </summary>
        public static bool PanelReindex
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether panel shrink.
        /// </summary>
        public static bool PanelShrink
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The my_reindex db message.
        /// </summary>
        private static string my_reindexDbMessage;

        /// <summary>
        /// The db_reindex_new.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_reindex_new([NotNull] string connectionString)
        {
            try
            {
                using (var connMan = new MySqlDbConnectionManager(connectionString))
                {
                    connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(my_reindexDb_InfoMessage);
                    using (
                        var cmd =
                            new MySqlCommand(
                                string.Format(
                                    "ANALYZE TABLE {0}.{1}user;",
                                    MySqlDbAccess.DatabaseSchemaName,
                                    Config.DatabaseObjectQualifier)))
                    {
                        var sb = new StringBuilder();
                        cmd.CommandType = CommandType.Text;

                        // up the command timeout...
                        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                        // run it...
                        sb.Append(
                            "SELECT table_name FROM  INFORMATION_SCHEMA.SCHEMATA s LEFT JOIN INFORMATION_SCHEMA.TABLES t ON s.schema_name = t.table_schema WHERE t.engine ='InnoDB'  AND t.TABLE_TYPE='BASE TABLE' ");
                        sb.Append(";");
                        MySqlDbAccess.ExecuteNonQuery(cmd, false, connectionString);
                        return my_reindexDbMessage;
                    }
                }
            }
            finally
            {
                my_reindexDbMessage = string.Empty;
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
        private static void my_reindexDb_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            my_reindexDbMessage += "\r\n{0}".FormatWith(e.Message);
        }

        /// <summary>
        /// The db_reindex_warning.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_reindex_warning()
        {
            return "InnoDB data engine keeps indexes in the same table";
        }

        /// <summary>
        /// The db_reindex_table.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable db_reindex_table([NotNull] string connectionString)
        {
            var sb = new StringBuilder();
            sb.Append(
                "SELECT table_name FROM  INFORMATION_SCHEMA.SCHEMATA s LEFT JOIN INFORMATION_SCHEMA.TABLES t ON s.schema_name = t.table_schema WHERE t.engine ='InnoDB'  AND t.TABLE_TYPE='BASE TABLE' ");
            sb.Append(";");
            DataTable dtc;
            using (var cmd1 = new MySqlCommand(sb.ToString()))
            {
                cmd1.CommandType = CommandType.Text;
                dtc = MySqlDbAccess.GetData(cmd1, connectionString);
            }

            var dtt = new DataTable();
            for (int i = 0; i < dtc.Rows.Count; i++)
            {
                using (
                    var cmd =
                        new MySqlCommand(
                            string.Format(
                                "ANALYZE TABLE {0}.{1}user;", MySqlDbAccess.DatabaseSchemaName, Config.DatabaseObjectQualifier)))
                                {
                    cmd.CommandType = CommandType.Text;
                    DataTable dttmp = MySqlDbAccess.GetData(cmd, false, connectionString);
                    DataRow drow = dttmp.Rows[0];
                    if (dtt.Rows.Count < 1)
                    {
                        dtt = dttmp.Clone();

                    }

                    DataRow ddd = dtt.NewRow();
                    ddd[0] = drow[0];
                    ddd[1] = drow[1];
                    ddd[2] = drow[2];
                    ddd[3] = drow[3];
                    dtt.Rows.Add(ddd);
                }
            }

            return dtt;
        }

        /// <summary>
        /// The my_message run sql.
        /// </summary>
        private static string my_messageRunSql;

        /// <summary>
        /// The db_runsql_new.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="useTransaction">
        /// The use transaction.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_runsql_new([NotNull] string connectionString, string sql, bool useTransaction)
        {
            try
            {
                using (var connMan = new MySqlDbConnectionManager(connectionString))
                {
                    connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(my_runSql_InfoMessage);

                    sql = MySqlDbAccess.GetCommandTextReplaced(sql.Trim());

                    using (var command = new MySqlCommand(sql, connMan.OpenDBConnection(connectionString)))
                    {
                        command.CommandTimeout = 9999;
                        command.Connection = connMan.OpenDBConnection(connectionString);

                        return InnerRunSqlExecuteReader(command, useTransaction);
                    }
                }
            }
            finally
            {
                my_messageRunSql = string.Empty;
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
        private static void my_runSql_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            my_messageRunSql = "\r\n" + e.Message;
        }

        /// <summary>
        /// Called from db_runsql -- just runs a sql command according to specificiations.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        private static string InnerRunSqlExecuteReader(MySqlCommand command, bool useTransaction)
        {
            var results = new StringBuilder();

            try
            {
                   command.Transaction = useTransaction
                                              ? command.Connection.BeginTransaction(MySqlDbAccess.IsolationLevel)
                                              : null;
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader != null)
                    {
                        if (reader.HasRows)
                        {
                            int rowIndex = 1;
                            var columnNames =
                                reader.GetSchemaTable()
                                      .Rows.Cast<DataRow>()
                                      .Select(r => r["ColumnName"].ToString())
                                      .ToList();

                            results.Append("RowNumber");

                            columnNames.ForEach(
                                n =>
                                    {
                                        results.Append(",");
                                        results.Append(n);
                                    });

                            results.AppendLine();

                            while (reader.Read())
                            {
                                results.AppendFormat(@"""{0}""", rowIndex++);

                                // dump all columns...
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
                            if (my_messageRunSql.IsSet())
                            {
                                results.AppendLine(my_messageRunSql);
                                results.AppendLine();
                            }
                            results.AppendLine("No Results Returned.");
                        }

                        reader.Close();
                      
                    }
            }
            catch (Exception x)
            {
                results.AppendLine();
                results.AppendFormat("SQL ERROR: {0}", x);
                if (command.Transaction != null)
                {
                    command.Transaction.Rollback();
                }
            }
           finally
                {
                    if (command.Transaction != null)
                    {
                        command.Transaction.Commit();
                    }
                } 

            return results.ToString();
        }

        /// <summary>
        /// The forumpage_validateversion.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="mid">
        /// The mid.
        /// </param>
        /// <param name="appVersion">
        /// The app version.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string forumpage_validateversion([NotNull] string connectionString, int? mid, int appVersion)
        {
            string redirect = string.Empty;
            try
            {
                DataTable registry = Db.registry_list(connectionString, "Version", DBNull.Value);

                if ((registry.Rows.Count == 0) || (Convert.ToInt32(registry.Rows[0]["Value"]) < appVersion))
                {
                    // needs upgrading...
                    redirect =
                        "install/default.aspx?upgrade={0}&md={1}".FormatWith(
                            registry.Rows.Count != 0 ? Convert.ToInt32(registry.Rows[0]["Value"]) : 0, mid ?? 1);
                }
            }
            catch (MySqlException)
            {
                // needs to be setup...
                redirect = "install/";
            }

            return redirect;
        }

        /// <summary>
        /// The system_deleteinstallobjects.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        public static void system_deleteinstallobjects([NotNull] string connectionString)
        {
            string tSQL = "DROP PROCEDURE" + MySqlDbAccess.GetObjectName("system_initialize");
            using (var cmd = MySqlDbAccess.GetCommand(tSQL, true))
            {
                cmd.CommandType = CommandType.Text;
                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// The system_initialize_replace_entries.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string system_initialize_replace_entries([NotNull] string connectionString, string script)
        {
            bool conEncoding = false;
            string[] options;

            // apply object qualifier
            if (!string.IsNullOrEmpty(Config.DatabaseObjectQualifier))
            {
                script = script.Replace("{objectQualifier}", Config.DatabaseObjectQualifier);
            }
            else
            {
                script = script.Replace("{objectQualifier}", "yaf_");
            }

            string dbcharset = null;
            string dbcollation = null;
            script = MySqlDbAccess.GetCommandTextReplaced(script);

            using (var connMan = new MySqlDbConnectionManager(connectionString))
            {
                options = connMan.DbConnection(connectionString).ConnectionString.Split(';');
            }

            foreach (string str in options)
            {
                string[] optionValue = str.Split('=');

                // apply database name
                if (optionValue[0].Trim().ToLower() == "database")
                {
                    if (optionValue[1].Trim() != MySqlDbAccess.DatabaseSchemaName
                        || !string.IsNullOrEmpty(optionValue[1].Trim()))
                    {
                        script = script.Replace("{databaseName}", optionValue[1].Trim());
                    }
                    else
                    {
                        script = script.Replace("{databaseName}", MySqlDbAccess.DatabaseSchemaName);
                    }
                }

                // apply user name from connection string to override defaults in config
                // currently it's not used
                if (optionValue[0].Trim().ToLowerInvariant().Contains("user id")
                    || optionValue[0].Trim().ToLowerInvariant().Contains("Username")
                    || optionValue[0].Trim().ToLowerInvariant().Contains("User name")
                    || optionValue[0].Trim().ToLowerInvariant().Contains("Uid"))
                {
                    if (optionValue[1].Trim() != Config.DatabaseOwner || !string.IsNullOrEmpty(optionValue[1].Trim()))
                    {
                        script = script.Replace("{databaseOwner}", optionValue[1].Trim());
                    }
                    else
                    {
                        script = script.Replace("{databaseOwner}", Config.DatabaseOwner.Trim());
                    }
                }

                // Encodings
                // apply charset
                if ((str.Contains("Charset") || str.Contains("Character Set")) && string.IsNullOrEmpty(optionValue[1]))
                {
                    // Verify if it's valid       
                    using (var cmd = MySqlDbAccess.GetCommand("SHOW VARIABLES LIKE 'character_set_database'", true))
                    {
                        DataTable dtt = MySqlDbAccess.GetData(cmd, connectionString);
                        if (dtt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtt.Rows)
                            {
                                if (dr["Variable_name"].ToString() == optionValue[1].Trim())
                                {
                                    dbcharset = dr["Value"].ToString();
                                }
                            }

                            conEncoding = true;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Config.DatabaseEncoding))
                {
                    // Verify if it's valid       
                    using (var cmd = MySqlDbAccess.GetCommand("SHOW VARIABLES LIKE 'character_set_database'", true))
                    {
                        DataTable dtt1 = MySqlDbAccess.GetData(cmd, connectionString);
                        if (dtt1.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtt1.Rows)
                            {
                                if (dr["Variable_name"].ToString() == "character_set_database")
                                {
                                    dbcharset = dr["Value"].ToString();
                                }
                            }

                            conEncoding = true;
                        }
                    }
                }
            }

            if (conEncoding)
            {
                if (Config.DatabaseCollation.Contains(dbcharset))
                {
                    dbcollation = Config.DatabaseCollation;
                }

                if (string.IsNullOrEmpty(dbcollation))
                {
                    using (var cmd = MySqlDbAccess.GetCommand("SHOW CHARACTER SET;", true))
                    {
                        DataTable dttt = MySqlDbAccess.GetData(cmd, connectionString);
                        foreach (DataRow dr in dttt.Rows)
                        {
                            if (dr["Charset"].ToString() == dbcharset)
                            {
                                dbcollation = dr["Default collation"].ToString();
                            }
                        }
                    }
                }
            }

            // No entry for encoding in connection string or app.config
            if (!conEncoding)
            {
                using (var cmd = MySqlDbAccess.GetCommand("SHOW VARIABLES LIKE 'collation_database'", true))
                {
                    dbcollation = MySqlDbAccess.GetData(cmd, connectionString).Rows[0]["Value"].ToString();
                }

                using (var cmd = MySqlDbAccess.GetCommand("SHOW VARIABLES LIKE 'character_set_database'", true))
                {
                    dbcharset = MySqlDbAccess.GetData(cmd, connectionString).Rows[0]["Value"].ToString();
                }
            }

            script = script.Replace("{databaseEncoding}_{databaseCollation}", dbcollation);
            script = script.Replace("{databaseEncoding}", dbcharset);

            return script;
        }

        /// <summary>
        /// The system_initialize_executescripts.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        /// <param name="scriptFile">
        /// The script file.
        /// </param>
        /// <param name="useTransactions">
        /// The use transactions.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        public static void system_initialize_executescripts(
            [NotNull] string connectionString, string script, string scriptFile, bool useTransactions)
        {
            script = system_initialize_replace_entries(connectionString, script);

            using (var connMan = new MySqlDbConnectionManager(connectionString))
            {
                // Now we separate string to array
                List<string> statements =
                    System.Text.RegularExpressions.Regex.Split(
                        script, "(?:--GO)", System.Text.RegularExpressions.RegexOptions.IgnoreCase).ToList();

                // use transactions...
                if (useTransactions)
                {
                    var con = connMan.OpenDBConnection(connectionString);
                    using (var trans = con.BeginTransaction(MySqlDbAccess.IsolationLevel))
                    {
                        foreach (string sql0 in statements)
                        {
                            string sql = sql0.Trim();

                            try
                            {
                                if (sql.ToLower().IndexOf("setuser", System.StringComparison.Ordinal) >= 0)
                                {
                                    continue;
                                }

                                if (sql.Length <= 0)
                                {
                                    continue;
                                }

                                using (var cmd = new MySqlCommand())
                                {
                                    cmd.Transaction = trans;
                                    cmd.Connection = con;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = sql.Trim();

                                    // added so command won't timeout anymore...
                                    cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception x)
                            {
                                trans.Rollback();
                                throw new Exception(
                                    string.Format(
                                        "FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}", scriptFile, sql, x.Message));
                            }
                        }

                        trans.Commit();
                    }
                }
                else
                {
                    using (var connect = new MySqlConnection(connectionString))
                    {
                        connect.Open();

                        // don't use transactions
                        foreach (string sql0 in statements)
                        {
                            string sql = sql0.Trim();

                            try
                            {
                                if (sql.ToLower().IndexOf("setuser", System.StringComparison.Ordinal) >= 0
                                    || sql.Length <= 0)
                                {
                                    continue;
                                }

                                using (var cmd = new MySqlCommand())
                                {
                                    cmd.Connection = connect;
                                    if (cmd.Connection.State != ConnectionState.Open)
                                    {
                                        cmd.Connection.Open();
                                    }
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = sql.Trim();

                                    // added so command won't timeout anymore...
                                    cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception x)
                            {
                                throw new Exception(
                                    string.Format(
                                        "FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}", scriptFile, sql, x.Message));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The system_initialize_fixaccess.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="bGrant">
        /// The b grant.
        /// </param>
        public static void system_initialize_fixaccess([NotNull] string connectionString, bool bGrant)
        {
            /* using (VZF.Classes.Data.MySqlDbConnectionManager connMan = new MySqlDbConnectionManager())
              {
                  using (MySqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(VZF.Classes.Data.DBAccess.IsolationLevel))
                  {
                      // REVIEW : Ederon - would "{databaseName}.{objectQualifier}" work, might need only "{objectQualifier}"
                      using (SqlDataAdapter da = new SqlDataAdapter("select Name,IsUserTable = OBJECTPROPERTY(id, N'IsUserTable'),IsScalarFunction = OBJECTPROPERTY(id, N'IsScalarFunction'),IsProcedure = OBJECTPROPERTY(id, N'IsProcedure'),IsView = OBJECTPROPERTY(id, N'IsView') from dbo.sysobjects where Name like '{databaseName}.{objectQualifier}%'", connMan.OpenDBConnection))
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
                                          cmd.Parameters.AddWithValue("@objname", row["Name"]);
                                          cmd.Parameters.AddWithValue("@newowner", "dbo");
                                          try
                                          {
                                              cmd.ExecuteNonQuery();
                                          }
                                          catch (SqlException)
                                          {
                                          }
                                      }
                                      foreach (DataRow row in dt.Select("IsView=1"))
                                      {
                                          cmd.Parameters.Clear();
                                          cmd.Parameters.AddWithValue("@objname", row["Name"]);
                                          cmd.Parameters.AddWithValue("@newowner", "dbo");
                                          try
                                          {
                                              cmd.ExecuteNonQuery();
                                          }
                                          catch (SqlException)
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

        /// <summary>
        /// The system_initialize.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="forumName">
        /// The forum name.
        /// </param>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="languageFile">
        /// The language file.
        /// </param>
        /// <param name="forumEmail">
        /// The forum email.
        /// </param>
        /// <param name="smtpServer">
        /// The smtp server.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="userEmail">
        /// The user email.
        /// </param>
        /// <param name="providerUserKey">
        /// The provider user key.
        /// </param>
        /// <param name="rolePrefix">
        /// The role prefix.
        /// </param>
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
            using (var cmd = MySqlDbAccess.GetCommand("system_initialize"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("i_Name", MySqlDbType.VarChar).Value = forumName;
                cmd.Parameters.Add("i_TimeZone", MySqlDbType.Int32).Value = timeZone;
                cmd.Parameters.Add("i_Culture", MySqlDbType.VarChar).Value = culture;
                cmd.Parameters.Add("i_LanguageFile", MySqlDbType.VarChar).Value = languageFile;
                cmd.Parameters.Add("i_ForumEmail", MySqlDbType.VarChar).Value = forumEmail;
                cmd.Parameters.Add("i_SmtpServer", MySqlDbType.VarChar).Value = string.Empty;
                cmd.Parameters.Add("i_User", MySqlDbType.VarChar).Value = userName;
                cmd.Parameters.Add("i_UserEmail", MySqlDbType.VarChar).Value = userEmail;
                cmd.Parameters.Add("i_UserKey", MySqlDbType.String).Value = providerUserKey;
                cmd.Parameters.Add("i_RolePrefix", MySqlDbType.String).Value = rolePrefix;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// The system_updateversion.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public static void system_updateversion([NotNull] string connectionString, int version, string name)
        {
            using (var cmd = MySqlDbAccess.GetCommand("system_updateversion"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new MySqlParameter("i_Version", MySqlDbType.Int32)).Value = version;
                cmd.Parameters.Add(new MySqlParameter("i_VersionName", MySqlDbType.VarChar)).Value = name;

                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
            }
        }

        /// <summary>
        /// Returns info about all Groups and Rank styles. 
        /// Used in GroupRankStyles cache.
        /// Usage: LegendID = 1 - Select Groups, LegendID = 2 - select Ranks by Name 
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable group_rank_style([NotNull] string connectionString, object boardId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("group_rank_style"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("i_BoardID", MySqlDbType.Int32)).Value = boardId;
                return MySqlDbAccess.GetData(cmd, connectionString);
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
            using (var cmd = MySqlDbAccess.GetCommand("shoutbox_getmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_BoardId", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add("I_NumberOfMessages", MySqlDbType.Int32).Value = numberOfMessages;
                cmd.Parameters.Add("I_StyledNicks", MySqlDbType.Byte).Value = useStyledNicks;
                return MySqlDbAccess.GetData(cmd, connectionString);
            }
        }

        public static bool shoutbox_savemessage(
            [NotNull] string connectionString, int boardId, string message, string userName, int userId, object ip)
        {
            using (var cmd = MySqlDbAccess.GetCommand("shoutbox_savemessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_BoardId", MySqlDbType.Int32).Value = boardId;
                cmd.Parameters.Add(new MySqlParameter("i_UserID", MySqlDbType.Int32)).Value = userId;
                cmd.Parameters.Add(new MySqlParameter("i_UserName", MySqlDbType.VarChar)).Value = userName;
                cmd.Parameters.Add(new MySqlParameter("i_Message", MySqlDbType.Text)).Value = message;
                cmd.Parameters.Add(new MySqlParameter("i_Date", MySqlDbType.DateTime)).Value = DBNull.Value;
                cmd.Parameters.Add(new MySqlParameter("i_IP", MySqlDbType.VarChar)).Value = ip;
                cmd.Parameters.Add("i_UTCTIMESTAMP", MySqlDbType.DateTime).Value = DateTime.UtcNow;
                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
                return true;
            }
        }

        /// <summary>
        /// The shoutbox_clearmessages.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static Boolean shoutbox_clearmessages([NotNull] string connectionString, int boardId)
        {
            using (var cmd = MySqlDbAccess.GetCommand("shoutbox_clearmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("I_BoardId", MySqlDbType.Int32).Value = boardId;
                MySqlDbAccess.ExecuteNonQuery(cmd, connectionString);
                return true;
            }
        }

        #endregion

        #region Touradg Mods

        /// <summary>
        /// The db_shrink_warning.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_shrink_warning()
        {
            // Shinking Operation is not applicable to the db.
            return string.Empty;
        }

        /// <summary>
        /// The db_shrink_new.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_shrink_new([NotNull] string connectionString)
        {
            // Shinking Operation is not applicable to the db.
            return string.Empty;
        }

        // Set Recovery

        /// <summary>
        /// The db_recovery_mode_warning.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string db_recovery_mode_warning()
        {
            // Recovery operation is not aaplicable to the data layer.
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
            // Recovery operation is not aaplicable to the data layer.
            return string.Empty;
        }

        #endregion  

    }
}
