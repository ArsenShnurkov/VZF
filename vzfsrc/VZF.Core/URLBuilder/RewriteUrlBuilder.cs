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
 * File RewriteUrlBuilder.cs created  on 2.6.2015 in  6:29 AM.
 * Last changed on 5.21.2016 in 1:05 PM.
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

namespace YAF.Core
{
    #region Using

    using System;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Web.Caching;

    using VZF.Data.Common;

    using YAF.Classes;
    
    using YAF.Types.Constants;
    using VZF.Utils;
    using VZF.Utils.Helpers.StringUtils;

    #endregion

    /// <summary>
    /// The rewrite url builder.
    /// </summary>
    public class RewriteUrlBuilder : BaseUrlBuilder
    {
        #region Constants and Fields

        /// <summary>
        /// The cache size.
        /// </summary>
        private int _cacheSize = 1000;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets CacheSize.
        /// </summary>
        protected int CacheSize
        {
            get
            {
                return this._cacheSize;
            }

            set
            {
                if (this._cacheSize > 0)
                {
                    this._cacheSize = value;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Build the url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>
        /// The build url.
        /// </returns>
        public override string BuildUrl(string url)
        {
            string newUrl = "{0}{1}?{2}".FormatWith(AppPath, Config.ForceScriptName ?? ScriptName, url);

            // create scriptName
            string scriptName = "{0}{1}".FormatWith(AppPath, Config.ForceScriptName ?? ScriptName);

            // get the base script file from the config -- defaults to, well, default.aspx :)
            string scriptFile = Config.BaseScriptFile;

            if (url.IsNotSet())
            {
                 return newUrl;
            }

            if (scriptName.EndsWith(scriptFile))
            {
                string before = scriptName.Remove(scriptName.LastIndexOf(scriptFile));

                var parser = new SimpleURLParameterParser(url);

                // create "rewritten" url...
                newUrl = before + Config.UrlRewritingPrefix;

                string useKey = string.Empty;
                string description = string.Empty;
                string pageName = parser["g"];
                bool isFeed = false;
                //// const bool showKey = false;
                bool handlePage = false;

                switch (parser["g"])
                {
                    case "topics":
                        useKey = "f";
                        description = this.GetForumName(parser[useKey].ToType<int>());
                        handlePage = true;
                        break;
                    case "posts":
                        if (parser["t"].IsSet())
                        {
                            useKey = "t";
                            pageName += "t";
                            description = this.GetTopicName(parser[useKey].ToType<int>());
                        }
                        else if (parser["m"].IsSet())
                        {
                            useKey = "m";
                            pageName += "m";

                            try
                            {
                                description = this.GetTopicNameFromMessage(parser[useKey].ToType<int>());
                            }
                            catch (Exception)
                            {
                                description = "posts";
                            }
                        }

                        handlePage = true;
                        break;
                    case "profile":
                        useKey = "u";

                        // description = GetProfileName( Convert.ToInt32( parser [useKey] ) );
                        break;
                    case "forum":
                        if (parser["c"].IsSet())
                        {
                            useKey = "c";
                            description = this.GetCategoryName(parser[useKey].ToType<int>());
                        }

                        break;
                    case "rsstopic":
                        if (parser["pg"].IsSet())
                        {
                            useKey = "pg";
                            description = parser[useKey].ToEnum<YafRssFeeds>().ToString().ToLower(); 
                            //// pageName += "pg";
                           /* if (parser[useKey].ToType<int>() == YafRssFeeds.Active.ToInt())
                            {
                                description = "active";
                            }
                            else if (parser[useKey].ToType<int>() == YafRssFeeds.Favorite.ToInt())
                            {
                                description = "favorite";
                            }
                            else if (parser[useKey].ToType<int>() == YafRssFeeds.Forum.ToInt())
                            {
                                description = "forum";
                            }
                            else if (parser[useKey].ToType<int>() == YafRssFeeds.LatestAnnouncements.ToInt())
                            {
                                description = "latestannouncements";
                            }
                            else if (parser[useKey].ToType<int>() == YafRssFeeds.LatestPosts.ToInt())
                            {
                                description = "latestposts";
                            }
                            else if (parser[useKey].ToType<int>() == YafRssFeeds.Posts.ToInt())
                            {
                                description = "posts";
                            }
                            else if (parser[useKey].ToType<int>() == YafRssFeeds.Topics.ToInt())
                            {
                                description = "topics";
                            } */
                        }

                        string useKey2;
                        if (parser["f"].IsSet())
                        {
                            useKey2 = "f";
                            description += this.GetForumName(parser[useKey2].ToType<int>());
                        }

                        if (parser["t"].IsSet())
                        {
                            useKey2 = "t";
                            description += this.GetTopicName(parser[useKey2].ToType<int>());
                        }

                        if (parser["ft"].IsSet())
                        {
                            useKey2 = "ft";

                            if (parser[useKey2].ToType<int>() == YafSyndicationFormats.Atom.ToInt())
                            {
                                description += "-atom";
                            }
                            else
                            {
                                description += "-rss";
                            }
                        }

                        handlePage = true;
                        isFeed = true;
                        break;
                }

                newUrl += pageName;

                if (useKey.Length > 0)
                {
                    newUrl += parser[useKey];
                }

                if (handlePage && parser["p"] != null && !isFeed)
                {
                    int page = parser["p"].ToType<int>();
                    if (page != 1)
                    {
                        newUrl += "p{0}".FormatWith(page);
                    }

                    parser.Parameters.Remove("p");
                }

                if (isFeed)
                {
                    if (parser["ft"] != null)
                    {
                        int page = parser["ft"].ToType<int>();
                        newUrl += "ft{0}".FormatWith(page);
                        parser.Parameters.Remove("ft");
                    }

                    if (parser["f"] != null)
                    {
                        int page = parser["f"].ToType<int>();
                        newUrl += "f{0}".FormatWith(page);
                        parser.Parameters.Remove("f");
                    }

                    if (parser["t"] != null)
                    {
                        int page = parser["t"].ToType<int>();
                        newUrl += "t{0}".FormatWith(page);
                        parser.Parameters.Remove("t");
                    }
                }

                if (parser["find"] != null)
                {
                    newUrl += "find{0}".FormatWith(parser["find"].Trim());
                    parser.Parameters.Remove("find");
                }

                if (description.Length > 0)
                {
                    if (description.EndsWith("-"))
                    {
                        description = description.Remove(description.Length - 1, 1);
                    }

                    newUrl += "_{0}".FormatWith(description);
                }

                newUrl += ".aspx";
                /*  if (!isFeed)
                 {
                     newUrl += ".aspx";
                 }
                 else
                 {
                     newUrl += ".xml"; 
                 } */

                string restURL = parser.CreateQueryString(new[] { "g", useKey });

                // append to the url if there are additional (unsupported) parameters
                if (restURL.Length > 0)
                {
                    newUrl += "?{0}".FormatWith(restURL);
                }

                // see if we can just use the default (/)
                if (newUrl.EndsWith("{0}forum.aspx".FormatWith(Config.UrlRewritingPrefix)))
                {
                    // remove in favor of just slash...
                    newUrl =
                        newUrl.Remove(
                            newUrl.LastIndexOf(
                                "{0}forum.aspx".FormatWith(Config.UrlRewritingPrefix)),
                            "{0}forum.aspx".FormatWith(Config.UrlRewritingPrefix).Length);
                }

                // add anchor
                if (parser.HasAnchor)
                {
                    newUrl += "#{0}".FormatWith(parser.Anchor);
                }
            }

            // just make sure & is &amp; ...
            newUrl = newUrl.Replace("&amp;", "&").Replace("&", "&amp;");

            return newUrl;
        }

        #endregion

        #region Methods

        /// <summary>
        /// High range.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The high range.
        /// </returns>
        protected int HighRange(int id)
        {
            return (int)(Math.Ceiling((double)(id / this._cacheSize)) * this._cacheSize);
        }

        /// <summary>
        /// Low range.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The low range.
        /// </returns>
        protected int LowRange(int id)
        {
            return (int)(Math.Floor((double)(id / this._cacheSize)) * this._cacheSize);
        }

        /// <summary>
        /// Cleans the string for URL.
        /// </summary>
        /// <param name="str">The str.</param>
        /// <returns>
        /// The clean string for url.
        /// </returns>
        protected static string CleanStringForURL(string str)
        {
            var sb = new StringBuilder();

            // trim...
            str = Config.UrlRewritingMode == "Unicode"
                      ? HttpUtility.UrlDecode(str.Trim())
                      : HttpContext.Current.Server.HtmlDecode(str.Trim());

            // fix ampersand...
            str = str.Replace("&", "and");

            // normalize the Unicode
            str = str.Normalize(NormalizationForm.FormD);

            switch (Config.UrlRewritingMode)
            {
                case "Unicode":
                    {
                        foreach (char currentChar in str)
                        {
                            if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                            {
                                sb.Append('-');
                            }
                            else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark
                                     && !char.IsSymbol(currentChar))
                            {
                                sb.Append(currentChar);
                            }
                        }

                        string strNew = sb.ToString();

                        while (strNew.EndsWith("-"))
                        {
                            strNew = strNew.Remove(strNew.Length - 1, 1);
                        }

                        return strNew.Length.Equals(0) ? "Default" : HttpUtility.UrlEncode(strNew);
                    }

                case "Translit":
                    {
                        string strUnidecode;

                        try
                        {
                            strUnidecode = str.Unidecode().Replace(" ", "-");
                        }
                        catch (Exception)
                        {
                            strUnidecode = str;
                        }

                        foreach (char currentChar in strUnidecode)
                        {
                            if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                            {
                                sb.Append('-');
                            }
                            else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark
                                     && !char.IsSymbol(currentChar))
                            {
                                sb.Append(currentChar);
                            }
                        }

                        string strNew = sb.ToString();

                        while (strNew.EndsWith("-"))
                        {
                            strNew = strNew.Remove(strNew.Length - 1, 1);
                        }

                        return strNew.Length.Equals(0) ? "Default" : strNew;
                    }

                default:
                    {
                        foreach (char currentChar in str)
                        {
                            if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                            {
                                sb.Append('-');
                            }
                            else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark
                                     && !char.IsSymbol(currentChar) && currentChar < 128)
                            {
                                sb.Append(currentChar);
                            }
                        }

                        string strNew = sb.ToString();

                        while (strNew.EndsWith("-"))
                        {
                            strNew = strNew.Remove(strNew.Length - 1, 1);
                        }

                        return strNew.Length.Equals(0) ? "Default" : strNew;
                    }
            }
        }

        /// <summary>
        /// Gets the name of the cache.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get cache name.
        /// </returns>
        protected string GetCacheName(string type, int id)
        {
            return @"urlRewritingDT-{0}-Range-{1}-to-{2}".FormatWith(type, this.HighRange(id), this.LowRange(id));
        }

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get category name.
        /// </returns>
        protected string GetCategoryName(int id)
        {
            const string Type = "Category";
            const string PrimaryKey = "CategoryID";
            const string NameField = "Name";

            DataRow row = this.GetDataRowFromCache(Type, id);

            if (row == null)
            {
                // get the section desired...
                DataTable list = CommonDb.category_simplelist(YafContext.Current.PageModuleID, this.LowRange(id), this.CacheSize);

                // set it up in the cache
                row = this.SetupDataToCache(ref list, Type, id, PrimaryKey);

                if (row == null)
                {
                    return string.Empty;
                }
            }

            return CleanStringForURL(row[NameField].ToString());
        }

        /// <summary>
        /// The get data row from cache.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        ///  Cached data Row
        /// </returns>
        protected DataRow GetDataRowFromCache(string type, int id)
        {
            // get the datatable and find the value
            var list = HttpContext.Current.Cache[this.GetCacheName(type, id)] as DataTable;

            if (list != null)
            {
                DataRow row = list.Rows.Find(id);

                // valid, return...
                if (row != null)
                {
                    return row;
                }

                // invalidate this cache section
                HttpContext.Current.Cache.Remove(this.GetCacheName(type, id));
            }

            return null;
        }

        /// <summary>
        /// Gets the name of the forum.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get forum name.
        /// </returns>
        protected string GetForumName(int id)
        {
            const string Type = "Forum";
            const string PrimaryKey = "ForumID";
            const string NameField = "Name";

            DataRow row = this.GetDataRowFromCache(Type, id);

            if (row == null)
            {
                // get the section desired...
                DataTable list = CommonDb.forum_simplelist(YafContext.Current.PageModuleID, this.LowRange(id), this.CacheSize);

                // set it up in the cache
                row = this.SetupDataToCache(ref list, Type, id, PrimaryKey);

                if (row == null)
                {
                    return string.Empty;
                }
            }

            return CleanStringForURL(row[NameField].ToString());
        }

        /// <summary>
        /// Gets the name of the profile.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get profile name.
        /// </returns>
        protected string GetProfileName(int id)
        {
            const string Type = "Profile";
            const string PrimaryKey = "UserID";
            const string NameField = "Name";

            DataRow row = this.GetDataRowFromCache(Type, id);

            if (row == null)
            {
                // get the section desired...
                DataTable list = CommonDb.user_simplelist(YafContext.Current.PageModuleID, this.LowRange(id), this.CacheSize);

                // set it up in the cache
                row = this.SetupDataToCache(ref list, Type, id, PrimaryKey);

                if (row == null)
                {
                    return string.Empty;
                }
            }

            return CleanStringForURL(row[NameField].ToString());
        }

        /// <summary>
        /// Gets the name of the topic.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get topic name.
        /// </returns>
        protected string GetTopicName(int id)
        {
            const string TSype = "Topic";
            const string PrimaryKey = "TopicID";
            const string NameField = "Topic";

            DataRow row = this.GetDataRowFromCache(TSype, id);

            if (row == null)
            {
                // get the section desired...
                DataTable list = CommonDb.topic_simplelist(YafContext.Current.PageModuleID, this.LowRange(id), this.CacheSize);

                // set it up in the cache
                row = this.SetupDataToCache(ref list, TSype, id, PrimaryKey);

                if (row == null)
                {
                    return string.Empty;
                }
            }

            return CleanStringForURL(row[NameField].ToString());
        }

        /// <summary>
        /// Gets the topic name from message.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The get topic name from message.
        /// </returns>
        protected string GetTopicNameFromMessage(int id)
        {
            const string Type = "Message";
            const string PrimaryKey = "MessageID";

            DataRow row = this.GetDataRowFromCache(Type, id);

            if (row == null)
            {
                // get the section desired...
                DataTable list = CommonDb.message_simplelist(YafContext.Current.PageModuleID, this.LowRange(id), this.CacheSize);

                // set it up in the cache
                row = this.SetupDataToCache(ref list, Type, id, PrimaryKey);

                if (row == null)
                {
                    return string.Empty;
                }
            }

            return this.GetTopicName(row["TopicID"].ToType<int>());
        }

        /// <summary>
        /// The setup data to cache.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        /// <returns>
        /// The Data row
        /// </returns>
        protected DataRow SetupDataToCache(ref DataTable list, string type, int id, string primaryKey)
        {
            DataRow row = null;

            if (list != null)
            {
                list.Columns[primaryKey].Unique = true;
                list.PrimaryKey = new[] { list.Columns[primaryKey] };

                // store it for the future
                var randomValue = new Random();
                HttpContext.Current.Cache.Insert(
                    this.GetCacheName(type, id),
                    list,
                    null,
                    DateTime.UtcNow.AddMinutes(randomValue.Next(5, 15)),
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null);

                // find and return profile..
                row = list.Rows.Find(id);

                if (row == null)
                {
                    // invalidate this cache section
                    HttpContext.Current.Cache.Remove(this.GetCacheName(type, id));
                }
            }

            return row;
        }

        #endregion
    }
}