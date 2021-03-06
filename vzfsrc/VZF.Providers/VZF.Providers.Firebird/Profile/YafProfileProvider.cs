#region copyright
// VZF 
// Copyright (C) 2014-2016 Vladimir Zakharov
//
// http://www.code.coolhobby.ru/

// File yafprofileprovider.cs created  on 2.6.2015 in  6:31 AM.
// Last changed on 5.21.2016 in 1:10 PM.
// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
//  "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
//
#endregion

using System.Web;

namespace YAF.Providers.Profile
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Data;
    using System.Text;
    using System.Web.Profile;

    using VZF.Data.DAL;
    using VZF.Data.Firebird.Mappers;
    using VZF.Data.Utils;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Providers.Utils;
    using YAF.Types.Interfaces;

    /// <summary>
    /// YAF Custom Profile Provider
    /// </summary>
    public class VzfFirebirdProfileProvider : ProfileProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The _conn str app key name.
        /// </summary>
        private static string _connStrAppKeyName = "YafProfileConnectionString";

        /// <summary>
        /// The _connection string.
        /// </summary>
        private static string _connectionString;

        /// <summary>
        /// The _app name.
        /// </summary>
        private string _appName, _connStrName;

        /// <summary>
        /// The _properties setup.
        /// </summary>
        private bool _propertiesSetup = false;

        /// <summary>
        /// The _property lock.
        /// </summary>
        private object _propertyLock = new object();

        /// <summary>
        /// The _settings columns list.
        /// </summary>
        private List<SettingsPropertyColumn> _settingsColumnsList =
            new List<SettingsPropertyColumn>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Connection String App Key Name.
        /// </summary>
        public static string ConnStrAppKeyName
        {
            get
            {
                return _connStrAppKeyName;
            }
        }

        /// <summary>
        /// Gets the Connection String App Key Name.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return _connectionString;
            }

            set
            {
                _connectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string name.
        /// </summary>
        public static string ConnectionStringName { get; set; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return _appName;
            }

            set
            {
                _appName = value;
            }
        }

        /// <summary>
        /// The _user profile cache.
        /// </summary>
        private ConcurrentDictionary<string, SettingsPropertyValueCollection> _userProfileCache = null;

        /// <summary>
        /// Gets UserProfileCache.
        /// </summary>
        private ConcurrentDictionary<string, SettingsPropertyValueCollection> UserProfileCache
        {
            get
            {
                string key = this.GenerateCacheKey("UserProfileDictionary");

                return this._userProfileCache
                       ?? (this._userProfileCache =
                           YafContext.Current.Get<IObjectStore>()
                               .GetOrSet(key, () => new ConcurrentDictionary<string, SettingsPropertyValueCollection>()));
            }
        }

        #endregion

        #region Overriden Public Methods

        /// <summary>
        /// Sets up the profile providers
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            // verify that the configuration section was properly passed
            if (!config.HasKeys())
            {
                throw new ArgumentNullException("config");
            }

            // Application Name
            this._appName = config["applicationName"].ToStringDBNull(Config.ApplicationName);

            // Connection String Name
            this._connStrName = config["connectionStringName"].ToStringDBNull(Config.ConnectionStringName);

            // is the connection string set?
            if (!string.IsNullOrEmpty(this._connStrName))
            {
                string connStr = ConfigurationManager.ConnectionStrings[_connStrName].ConnectionString;
                ConnectionString = connStr;
                ConnectionStringName = SqlDbAccess.GetConnectionStringNameFromConnectionString(connStr);

                // set the app variable...
                if (YafContext.Current.Get<HttpApplicationStateBase>()[_connStrAppKeyName] == null)
                {
                    YafContext.Current.Get<HttpApplicationStateBase>().Add(_connStrAppKeyName, connStr);
                }
                else
                {
                    YafContext.Current.Get<HttpApplicationStateBase>()[_connStrAppKeyName] = connStr;
                }
            }

            base.Initialize(name, config);
        }

        /// <summary>
        /// The load from property collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        protected void LoadFromPropertyCollection(SettingsPropertyCollection collection)
        {
            if (!this._propertiesSetup)
            {
                lock (this._propertyLock)
                {
                    // clear it out just in case something is still in there...
                    this._settingsColumnsList.Clear();
                }

                // validiate all the properties and populate the internal settings collection
                foreach (SettingsProperty property in collection)
                {
                    DbType dbType;
                    int size;

                    // parse custom provider data...
                    this.GetDbTypeAndSizeFromString(
                        property.Attributes["CustomProviderData"].ToString(),
                        out dbType,
                        out size);

                    // default the size to 256 if no size is specified
                    if (dbType == DbType.String && size == -1)
                    {
                        size = 256;
                    }

                    this._settingsColumnsList.Add(new SettingsPropertyColumn(property, dbType, size));
                }

                // sync profile table structure with the FbDB...
                DataTable structure = FbDB.Current.GetProfileStructure(ConnectionStringName);

                // verify all the columns are there...
                foreach (SettingsPropertyColumn column in _settingsColumnsList)
                {
                    // see if this column exists
                    if (!structure.Columns.Contains(column.Settings.Name))
                    {
                        // if not, create it...
                        FbDB.Current.AddProfileColumn(
                            ConnectionStringName,
                            column.Settings.Name,
                            column.DataType.ToString(),
                            column.Size);
                    }
                }

                // it's setup now...
                _propertiesSetup = true;
            }
        }

        /// <summary>
        /// The load from property value collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        protected void LoadFromPropertyValueCollection(SettingsPropertyValueCollection collection)
        {
            if (!_propertiesSetup)
            {
                // clear it out just in case something is still in there...
                _settingsColumnsList.Clear();

                // validiate all the properties and populate the internal settings collection
                foreach (SettingsPropertyValue value in collection)
                {
                    DbType dbType;
                    int size;

                    // parse custom provider data...
                    this.GetDbTypeAndSizeFromString(
                        value.Property.Attributes["CustomProviderData"].ToString(),
                        out dbType,
                        out size);

                    // default the size to 256 if no size is specified
                    if (dbType == DbType.String && size == -1)
                    {
                        size = 256;
                    }

                    this._settingsColumnsList.Add(new SettingsPropertyColumn(value.Property, dbType, size));
                }

                // sync profile table structure with the FbDB...
                DataTable structure = FbDB.Current.GetProfileStructure(ConnectionStringName);

                // verify all the columns are there...
                foreach (SettingsPropertyColumn column in this._settingsColumnsList)
                {
                    // see if this column exists
                    if (!structure.Columns.Contains(column.Settings.Name))
                    {
                        // if not, create it...
                        FbDB.Current.AddProfileColumn(
                            ConnectionStringName,
                            column.Settings.Name,
                            column.DataType.ToString(),
                            column.Size);
                    }
                }

                // it's setup now...
                this._propertiesSetup = true;
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
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        private bool GetDbTypeAndSizeFromString(string providerData, out DbType dbType, out int size)
        {
            size = -1;
            dbType = DbType.String;

            if (String.IsNullOrEmpty(providerData))
            {
                return false;
            }

            // split the data
            string[] chunk = providerData.Split(new char[] { ';' });

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
        /// The delete inactive profiles.
        /// </summary>
        /// <param name="authenticationOption">
        /// The authentication option.
        /// </param>
        /// <param name="userInactiveSinceDate">
        /// The user inactive since date.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int DeleteInactiveProfiles(
            ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate)
        {
            if (authenticationOption == ProfileAuthenticationOption.Anonymous)
            {
                ExceptionReporter.ThrowArgument("PROFILE", "NOANONYMOUS");
            }

            // just clear the whole thing...
            this.ClearUserProfileCache();

            return FbDB.Current.DeleteInactiveProfiles(
                ConnectionStringName,
                this.ApplicationName,
                userInactiveSinceDate);
        }

        /// <summary>
        /// The delete profiles.
        /// </summary>
        /// <param name="usernames">
        /// The usernames.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int DeleteProfiles(string[] usernames)
        {
            if (usernames == null || usernames.Length < 1)
            {
                return 0;
            }

            // make single string of usernames...
            var userNameBuilder = new StringBuilder();
            bool bFirst = true;

            foreach (string t in usernames)
            {
                string username = t.Trim();

                if (username.Length <= 0)
                {
                    continue;
                }

                if (!bFirst)
                {
                    userNameBuilder.Append(",");
                }
                else
                {
                    bFirst = false;
                }
                userNameBuilder.Append(username);

                // delete this user from the cache if they are in there...
                this.DeleteFromProfileCacheIfExists(username.ToLower());
            }

            // call the FbDB...
            return FbDB.Current.DeleteProfiles(ConnectionStringName, this.ApplicationName, userNameBuilder.ToString());
        }

        /// <summary>
        /// The delete profiles.
        /// </summary>
        /// <param name="profiles">
        /// The profiles.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            if (profiles.Count < 1)
            {
                ExceptionReporter.ThrowArgument("PROFILE", "PROFILESEMPTY");
            }

            var usernames = new string[profiles.Count];

            int index = 0;
            foreach (ProfileInfo profile in profiles)
            {
                usernames[index++] = profile.UserName;
            }

            return this.DeleteProfiles(usernames);
        }

        /// <summary>
        /// The find inactive profiles by user name.
        /// </summary>
        /// <param name="authenticationOption">
        /// The authentication option.
        /// </param>
        /// <param name="usernameToMatch">
        /// The username to match.
        /// </param>
        /// <param name="userInactiveSinceDate">
        /// The user inactive since date.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="totalRecords">
        /// The total records.
        /// </param>
        /// <returns>
        /// The <see cref="ProfileInfoCollection"/>.
        /// </returns>
        public override ProfileInfoCollection FindInactiveProfilesByUserName(
            ProfileAuthenticationOption authenticationOption,
            string usernameToMatch,
            DateTime userInactiveSinceDate,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            return this.GetProfileAsCollection(
                authenticationOption,
                pageIndex,
                pageSize,
                usernameToMatch,
                userInactiveSinceDate,
                out totalRecords);
        }

        /// <summary>
        /// The find profiles by user name.
        /// </summary>
        /// <param name="authenticationOption">
        /// The authentication option.
        /// </param>
        /// <param name="usernameToMatch">
        /// The username to match.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="totalRecords">
        /// The total records.
        /// </param>
        /// <returns>
        /// The <see cref="ProfileInfoCollection"/>.
        /// </returns>
        public override ProfileInfoCollection FindProfilesByUserName(
            ProfileAuthenticationOption authenticationOption,
            string usernameToMatch,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            return this.GetProfileAsCollection(
                authenticationOption,
                pageIndex,
                pageSize,
                usernameToMatch,
                null,
                out totalRecords);
        }

        /// <summary>
        /// The get all inactive profiles.
        /// </summary>
        /// <param name="authenticationOption">
        /// The authentication option.
        /// </param>
        /// <param name="userInactiveSinceDate">
        /// The user inactive since date.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="totalRecords">
        /// The total records.
        /// </param>
        /// <returns>
        /// The <see cref="ProfileInfoCollection"/>.
        /// </returns>
        public override ProfileInfoCollection GetAllInactiveProfiles(
            ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            return this.GetProfileAsCollection(
                authenticationOption,
                pageIndex,
                pageSize,
                null,
                userInactiveSinceDate,
                out totalRecords);
        }

        /// <summary>
        /// The get all profiles.
        /// </summary>
        /// <param name="authenticationOption">
        /// The authentication option.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="totalRecords">
        /// The total records.
        /// </param>
        /// <returns>
        /// The <see cref="ProfileInfoCollection"/>.
        /// </returns>
        public override ProfileInfoCollection GetAllProfiles(
            ProfileAuthenticationOption authenticationOption,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            return this.GetProfileAsCollection(authenticationOption, pageIndex, pageSize, null, null, out totalRecords);
        }

        /// <summary>
        /// The get number of inactive profiles.
        /// </summary>
        /// <param name="authenticationOption">
        /// The authentication option.
        /// </param>
        /// <param name="userInactiveSinceDate">
        /// The user inactive since date.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetNumberOfInactiveProfiles(
            ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate)
        {
            if (authenticationOption == ProfileAuthenticationOption.Anonymous)
            {
                ExceptionReporter.ThrowArgument("PROFILE", "NOANONYMOUS");
            }

            return FbDB.Current.GetNumberInactiveProfiles(
                ConnectionStringName,
                this.ApplicationName,
                userInactiveSinceDate);
        }

        /// <summary>
        /// The get profile as collection.
        /// </summary>
        /// <param name="authenticationOption">
        /// The authentication option.
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
        /// <param name="totalRecords">
        /// The total records.
        /// </param>
        /// <returns>
        /// The <see cref="ProfileInfoCollection"/>.
        /// </returns>
        private ProfileInfoCollection GetProfileAsCollection(
            ProfileAuthenticationOption authenticationOption,
            int pageIndex,
            int pageSize,
            object userNameToMatch,
            object inactiveSinceDate,
            out int totalRecords)
        {
            if (authenticationOption == ProfileAuthenticationOption.Anonymous)
            {
                ExceptionReporter.ThrowArgument("PROFILE", "NOANONYMOUS");
            }

            if (pageIndex < 0)
            {
                ExceptionReporter.ThrowArgument("PROFILE", "PAGEINDEXTOOSMALL");
            }

            if (pageSize < 1)
            {
                ExceptionReporter.ThrowArgument("PROFILE", "PAGESIZETOOSMALL");
            }

            // get all the profiles...


            // create an instance for the profiles...
            var profiles = new ProfileInfoCollection();

            DataTable allProfilesDT = FbDB.Current.GetProfiles(
                ConnectionStringName,
                this.ApplicationName,
                pageIndex,
                pageSize,
                userNameToMatch,
                inactiveSinceDate);

            foreach (DataRow profileRow in allProfilesDT.Rows)
            {
                string username = profileRow["Username"].ToString();
                DateTime lastActivity = DateTime.SpecifyKind(
                    Convert.ToDateTime(profileRow["LastActivity"]),
                    DateTimeKind.Utc);
                DateTime lastUpdated = DateTime.SpecifyKind(
                    Convert.ToDateTime(profileRow["LastUpdated"]),
                    DateTimeKind.Utc);

                profiles.Add(new ProfileInfo(username, false, lastActivity, lastUpdated, 0));
            }

            // get the first record which is the count..
            totalRecords = allProfilesDT.Rows.Count > 0 ? Convert.ToInt32(allProfilesDT.Rows[0][0]) : 0;

            return profiles;
        }

        /// <summary>
        /// The get property values.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="SettingsPropertyValueCollection"/>.
        /// </returns>
        public override SettingsPropertyValueCollection GetPropertyValues(
            SettingsContext context,
            SettingsPropertyCollection collection)
        {
            var settingPropertyCollection = new SettingsPropertyValueCollection();

            if (collection.Count < 1)
            {
                return settingPropertyCollection;
            }

            // unboxing fits?
            string username = context["UserName"].ToString();

            if (string.IsNullOrEmpty(username))
            {
                return settingPropertyCollection;
            }

            // (if) this provider doesn't support anonymous users
            if (Convert.ToBoolean(context["IsAuthenticated"]) == GeneralProviderSettings.AllowAnonymous)
            {
                ExceptionReporter.ThrowArgument("PROFILE", "NOANONYMOUS");
            }

            // Migration code
            if (Config.GetConfigValueAsBool("YAF.OldProfileProvider", false))
            {
                // load the property collection (sync profile class)
                this.LoadFromPropertyCollection(collection);

                // see if it's cached...
                if (this.UserProfileCache.ContainsKey(username.ToLower()))
                {
                    // just use the cached version...
                    return this.UserProfileCache[username.ToLower()];
                }
                else
                {
                    // transfer properties regardless...
                    foreach (SettingsProperty prop in collection)
                    {
                        settingPropertyCollection.Add(new SettingsPropertyValue(prop));
                    }

                    // get this profile from the DB				
                    DataTable profileDT = FbDB.Current.GetProfiles(
                        ConnectionStringName,
                        this.ApplicationName,
                        0,
                        1,
                        username,
                        null);

                    if (profileDT.Rows.Count > 0)
                    {
                        DataRow row = profileDT.Rows[0];

                        // load the data into the collection...
                        foreach (SettingsPropertyValue prop in settingPropertyCollection)
                        {
                            object val = row[prop.Name];

                            // Only initialize a SettingsPropertyValue for non-null values
                            if (!(val is DBNull || val == null))
                            {
                                prop.PropertyValue = val;
                                prop.IsDirty = false;
                                prop.Deserialized = true;
                            }
                        }
                    }

                    // save this collection to the cache
                    this.UserProfileCache.AddOrUpdate(
                        username.ToLower(),
                        (k) => settingPropertyCollection,
                        (k, v) => settingPropertyCollection);
                }

                return settingPropertyCollection;
            }

            //End of old
            if (this.UserProfileCache.ContainsKey(username.ToLower()))
            {
                // just use the cached version...
                return this.UserProfileCache[username.ToLower()];
            }

            foreach (SettingsProperty property in collection)
            {
                if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
                {
                    property.SerializeAs = SettingsSerializeAs.String;
                }
                else
                {
                    property.SerializeAs = SettingsSerializeAs.Xml;
                }

                settingPropertyCollection.Add(new SettingsPropertyValue(property));
            }

            // retrieve encoded profile data from the database
            DataTable dt = FbDB.Current.GetProfiles(
                ConnectionStringName,
                this.ApplicationName,
                0,
                1,
                username,
                null);

            if (dt.Rows.Count > 0)
            {
                YAF.Providers.Profile.FbDB.DecodeProfileData(dt.Rows[0], settingPropertyCollection);
            }

            // save this collection to the cache
            this.UserProfileCache.AddOrUpdate(
                username.ToLower(),
                (k) => settingPropertyCollection,
                (k, v) => settingPropertyCollection);

            return settingPropertyCollection;
        }

        /// <summary>
        /// The set property values.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public override void SetPropertyValues(
            SettingsContext context,
            SettingsPropertyValueCollection collection)
        {
            string username = (string)context["UserName"];

            if (string.IsNullOrEmpty(username) || collection.Count < 1)
            {
                return;
            }

            // this provider doesn't support anonymous users
            if (!Convert.ToBoolean(context["IsAuthenticated"]))
            {
                ExceptionReporter.ThrowArgument("PROFILE", "NOANONYMOUS");
            }

            bool itemsToSave = false;

            // First make sure we have at least one item to save
            foreach (SettingsPropertyValue pp in collection)
            {
                if (pp.IsDirty)
                {
                    itemsToSave = true;
                    break;
                }
            }

            if (!itemsToSave)
            {
                return;
            }

            // load the data for the configuration
            this.LoadFromPropertyValueCollection(collection);

            object userID = FbDB.Current.GetProviderUserKey(ConnectionStringName, this.ApplicationName, username);

            if (userID == null)
            {
                return;
            }

            // start saving...
            FbDB.Current.SetProfileProperties(
                ConnectionStringName,
                this.ApplicationName,
                userID,
                collection,
                this._settingsColumnsList);
               
            // erase from the cache
            this.DeleteFromProfileCacheIfExists(username.ToLower());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The delete from profile cache if exists.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        private void DeleteFromProfileCacheIfExists(string key)
        {
            SettingsPropertyValueCollection collection;

            this.UserProfileCache.TryRemove(key, out collection);
        }

        /// <summary>
        /// The clear user profile cache.
        /// </summary>
        private void ClearUserProfileCache()
        {
            YafContext.Current.Get<IObjectStore>().Remove(this.GenerateCacheKey("UserProfileDictionary"));
        }

        /// <summary>
        /// The generate cache key.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GenerateCacheKey(string name)
        {
            return String.Format("VzfFirebirdProfileProvider-{0}-{1}", name, this.ApplicationName);
        }

        #endregion
    }

}

