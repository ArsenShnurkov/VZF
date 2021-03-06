#region copyright
/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 *
 * http://www.yetanotherforum.net/
 *
 * This file can contain some changes in 2014-2016 by Vladimir Zakharov(vzrus)
 * for VZF forum
 *
 * http://www.code.coolhobby.ru/
 * 
 * File DB.cs created  on 2.6.2015 in  6:31 AM.
 * Last changed on 5.21.2016 in 1:11 PM.
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
#endregion

namespace YAF.Providers.Profile
{
  #region Using

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;   
    using System.Text;

    using VZF.Data.Common;
    using VZF.Data.DAL;
    using VZF.Data.MsSql.Mappers;
    using VZF.Data.Utils;
    using VZF.Utils;

    using YAF.Classes;
    using YAF.Classes.Pattern;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The db.
  /// </summary>
  public class DB
  {
    #region Constants and Fields

    /// <summary>
    ///   The _db access.
    /// </summary>
    // private readonly MsSqlDbAccess _msSqlDbAccess = new MsSqlDbAccess();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "DB" /> class.
    /// </summary>
    public DB()
    {
      // MsSqlDbAccess.SetConnectionManagerAdapter<MsSqlProfileDbConnectionManager>();
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Current.
    /// </summary>
    public static DB Current
    {
      get
      {
        return PageSingleton<DB>.Instance;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add profile column.
    /// </summary>
    /// <param name="connectionStringName">
    /// The connection String Name.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="columnType">
    /// The column type.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    public void AddProfileColumn(string connectionStringName, [NotNull] string name, string type, int size)
    {
         type = DataTypeMappers.typeToDbValueMap(name, type, size);

        using (var sc = new VzfSqlCommand(connectionStringName))
        {
            string sql = string.Format("ALTER TABLE {0} ADD [{1}] {2} NULL", SqlDbAccess.GetVzfObjectNameFromConnectionString("prov_Profile", connectionStringName), name, type);
            sc.CommandText.AppendQuery(sql);
            sc.ExecuteNonQuery(CommandType.Text, false);
        }
    }

    /// <summary>
    /// The delete inactive profiles.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="inactiveSinceDate">
    /// The inactive since date.
    /// </param>
    /// <returns>
    /// The delete inactive profiles.
    /// </returns>
    public int DeleteInactiveProfiles(string connectionStringName, [NotNull] object appName, [NotNull] object inactiveSinceDate)
    {    
      using (var sc = new VzfSqlCommand(connectionStringName))
      {
          sc.Parameters.Add(sc.CreateParameter(DbType.String, "@ApplicationName", appName));
          sc.Parameters.Add(sc.CreateParameter(DbType.DateTime, "@InactiveSinceDate", inactiveSinceDate));

          sc.CommandText.AppendObjectQuery("prov_profile_deleteinactive", connectionStringName);
          return Convert.ToInt32(sc.ExecuteScalar(CommandType.StoredProcedure, false));
      }
    }

    /// <summary>
    /// The delete profiles.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="userNames">
    /// The user names.
    /// </param>
    /// <returns>
    /// The delete profiles.
    /// </returns>
    public int DeleteProfiles(string connectionStringName, [NotNull] object appName, [NotNull] object userNames)
    {
      using (var sc = new VzfSqlCommand(connectionStringName))
      {
          sc.Parameters.Add(sc.CreateParameter(DbType.String, "@ApplicationName", appName)); 
          sc.Parameters.Add(sc.CreateParameter(DbType.String, "@UserNames", userNames));

          sc.CommandText.AppendObjectQuery("prov_profile_deleteprofiles", connectionStringName);
          return Convert.ToInt32(sc.ExecuteScalar(CommandType.StoredProcedure, false));
      }
    }

    /// <summary>
    /// The get number inactive profiles.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="inactiveSinceDate">
    /// The inactive since date.
    /// </param>
    /// <returns>
    /// The get number inactive profiles.
    /// </returns>
    public int GetNumberInactiveProfiles(string connectionStringName, [NotNull] object appName, [NotNull] object inactiveSinceDate)
    {
      using (var sc = new VzfSqlCommand(connectionStringName))
      {
          sc.Parameters.Add(sc.CreateParameter(DbType.String, "@ApplicationName", appName));
          sc.Parameters.Add(sc.CreateParameter(DbType.DateTime, "@InactiveSinceDate", inactiveSinceDate));

          sc.CommandText.AppendObjectQuery("prov_profile_getnumberinactiveprofiles", connectionStringName);
          return Convert.ToInt32(sc.ExecuteScalar(CommandType.StoredProcedure, false));
      }
    }

    /// <summary>
    /// The get profile structure.
    /// </summary>
    /// <returns>
    /// </returns>
    public DataTable GetProfileStructure(string connectionStringName)
    {
        return CommonDb.GetProfileStructure(connectionStringName, "prov_Profile");
    }

    /// <summary>
    /// The get profiles.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="userNameToMatch">
    /// The user name to match.
    /// </param>
    /// <param name="inactiveSinceDate">
    /// The inactive since date.
    /// </param>
    /// <returns>
    /// </returns>
    public DataSet GetProfiles(string connectionStringName, [NotNull] object appName, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object userNameToMatch, [NotNull] object inactiveSinceDate)
    {
      using (var sc = new VzfSqlCommand(connectionStringName))
      {
          sc.Parameters.Add(sc.CreateParameter(DbType.String, "@ApplicationName", appName));
          sc.Parameters.Add(sc.CreateParameter(DbType.Int32, "@PageIndex", pageIndex));
          sc.Parameters.Add(sc.CreateParameter(DbType.Int32, "@PageSize", pageSize));
          sc.Parameters.Add(sc.CreateParameter(DbType.String, "@UserNameToMatch", userNameToMatch));
          sc.Parameters.Add(sc.CreateParameter(DbType.DateTime, "@InactiveSinceDate", inactiveSinceDate));     

          sc.CommandText.AppendObjectQuery("prov_profile_getprofiles", connectionStringName);
          return sc.ExecuteDataSet(CommandType.StoredProcedure);
      }
    }

    /// <summary>
    /// The get db type and size from string.
    /// </summary>
    /// <param name="providerData">
    /// The provider data.
    /// </param>
    /// <param name="dbType">
    /// The db type.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    /// <returns>
    /// The get db type and size from string.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// </exception>
    public static bool GetDbTypeAndSizeFromString(string providerData, out DbType dbType, out int size)
    {
        size = -1;
        dbType = DbType.String;

        if (providerData.IsNotSet())
        {
            return false;
        }

        // split the data
        string[] chunk = providerData.Split(new[] { ';' });

        // first item is the column name...
        string paramName = DataTypeMappers.FromDbValueMap(chunk[1]);       
    
        // get the datatype and ignore case...
        dbType = (DbType)Enum.Parse(typeof(DbType), paramName, true);

        if (chunk.Length > 2)
        {
            // handle size...
            if (!int.TryParse(chunk[2], out size))
            {
                throw new ArgumentException("Unable to parse as integer: " + chunk[2]);
            }
        }

        return true;
    }

    /// <summary>
    /// The get provider user key.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <returns>
    /// The get provider user key.
    /// </returns>
    public object GetProviderUserKey(string connectionStringName, [NotNull] object appName, [NotNull] object username)
    {
        DataRow row = Membership.DB.Current.GetUser(connectionStringName, appName.ToString(), null, username.ToString(), false);

      if (row != null)
      {
        return row["UserID"];
      }

      return null;
    }

    /// <summary>
    /// The set profile properties.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="values">
    /// The values.
    /// </param>
    /// <param name="settingsColumnsList">
    /// The settings columns list.
    /// </param>
    public void SetProfileProperties(string connectionStringName, [NotNull] object appName, [NotNull] object userID, [NotNull] SettingsPropertyValueCollection values, [NotNull] List<SettingsPropertyColumn> settingsColumnsList)
    {
        using (var sc = new VzfSqlCommand(connectionStringName))
        {
            // Build up strings used in the query
            var columnStr = new StringBuilder();
            var valueStr = new StringBuilder();
            var setStr = new StringBuilder();

            settingsColumnsList.ForEach((column) =>
            {
                if (values[column.Settings.Name].IsDirty)
                {                   

                    var nameParam = values[column.Settings.Name].Name;
                    var valParam = values[column.Settings.Name].PropertyValue;                   

                    nameParam = "@" + nameParam;
                  
                    sc.Parameters.Add(sc.CreateParameter(column.DataType, nameParam, valParam));
                    
                    valueStr.Append(nameParam);
                    valueStr.Append(",");

                    columnStr.Append(column.Settings.Name);
                    columnStr.Append(",");                    

                    setStr.Append(column.Settings.Name);
                    setStr.Append("=");
                    setStr.Append(nameParam);
                    setStr.Append(",");
                }
            });

            columnStr.Append("LastUpdatedDate ");
            valueStr.Append("@LastUpdatedDate");
            setStr.Append("LastUpdatedDate=@LastUpdatedDate");
            sc.Parameters.Add(sc.CreateParameter(DbType.DateTime, "@LastUpdatedDate", DateTime.UtcNow));

            sc.Parameters.Add(sc.CreateParameter(DbType.String, "@UserID", userID));

            string table = SqlDbAccess.GetVzfObjectNameFromConnectionString("prov_Profile", connectionStringName);

            StringBuilder sqlCommand = new StringBuilder("IF EXISTS (SELECT top 1 1 FROM ").Append(table);
            sqlCommand.Append(" WHERE UserID = @UserID) ");

            sqlCommand.Append("BEGIN UPDATE ").Append(table).Append(" SET ").Append(setStr.ToString().Trim(','));
            sqlCommand.Append(" WHERE UserID = @UserID");

            sqlCommand.Append(" END ELSE BEGIN INSERT ").Append(table).Append(" (UserID,").Append(columnStr.ToString().Trim(','));
            sqlCommand.Append(") VALUES (@UserID,").Append(valueStr.ToString().Trim(',')).Append(
              ") END");

            sc.CommandText.AppendQuery(sqlCommand.ToString());
            sc.ExecuteNonQuery(CommandType.Text, false);
        }
    }

    #endregion

    /*
		public static void ValidateAddColumnInProfile( string columnName, SqlDbType columnType )
		{
			SqlCommand cmd = new SqlCommand( sprocName );
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue( "@ApplicationName", appName );
			cmd.Parameters.AddWithValue( "@Username", username );
			cmd.Parameters.AddWithValue( "@IsUserAnonymous", isAnonymous );

			return cmd;
		}
		*/
  }
}