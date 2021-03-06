﻿#region copyright
/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 *
 * http://www.yetanotherforum.net/
 *
 * This file can contain some changes in 2014-2016 by Vladimir Zakharov(vzrus)
 * for VZF forum
 *
 * http://www.code.coolhobby.ru/
 * 
 * File UserAgentHelper.cs created  on 2.6.2015 in  6:31 AM.
 * Last changed on 5.21.2016 in 12:59 PM.
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

using System.Web.Configuration;

namespace VZF.Utils.Helpers
{
    #region Using

    using System;
    using System.Linq;
    using System.Web;

    using YAF.Classes;
    using YAF.Types;

    #endregion

    /// <summary>
    /// Helper for Figuring out the User Agent.
    /// </summary>
    public static class UserAgentHelper
    {
        #region Constants and Fields

        /// <summary>
        /// The spider contains.
        /// </summary>
        private static readonly string[] SpiderContains = Config.CrawlerUserAgentTokens;

        #endregion

        #region Public Methods

        /// <summary>
        /// Is this user agent IE v6?
        /// </summary>
        /// <returns>
        /// The is browser i e 6.
        /// </returns>
        public static bool IsBrowserIe6()
        {
            return HttpContext.Current.Request.Browser.Browser.Contains("IE")
                   && HttpContext.Current.Request.Browser.Version.StartsWith("6.");
        }

        /// <summary>
        /// Validates if the user agent owner is a feed reader
        /// </summary>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <returns>
        /// The is feed reader.
        /// </returns>
        public static bool IsFeedReader([CanBeNull] string userAgent)
        {
            string[] agentContains = {"Windows-RSS-Platform","FeedDemon","Feedreader","Apple-PubSub","FeedBurner"};

            return userAgent.IsSet()
                   && agentContains.Any(
                       agentContain => userAgent.ToLowerInvariant().Contains(agentContain.ToLowerInvariant()));
        }

        public static string FeedReaderName([CanBeNull] string userAgent)
        {
            string[] agentContains = { "Windows-RSS-Platform", "FeedDemon", "Feedreader", "Apple-PubSub", "FeedBurner" };

            return userAgent.IsSet()
                   ? agentContains.FirstOrDefault(
                   agentContain => userAgent.ToLowerInvariant().Contains(agentContain.ToLowerInvariant())) : string.Empty;
        }

        /// <summary>
        /// Validates if the user agent is a known ignored UA string
        /// </summary>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <returns>
        /// The true if the UA string pattern should not be displayed in active users.
        /// </returns>
        public static bool IsIgnoredForDisplay([CanBeNull] string userAgent)
        {
            if (userAgent.IsSet())
            {
                // Apple-PubSub - Safary RSS reader
                string[] stringContains ={"PlaceHolder"};

                return stringContains.Any(x => userAgent.ToLowerInvariant().Contains(x.ToLowerInvariant()));
            }

            return false;
        }

        /// <summary>
        /// Tests if the user agent is a mobile device.
        /// </summary>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <returns>
        /// The is mobile device.
        /// </returns>
        public static bool IsMobileDevice([CanBeNull] string userAgent)
        {
            var mobileContains =
                Config.MobileUserAgents.Split(',').Where(m => m.IsSet()).Select(m => m.Trim().ToLowerInvariant());

            return userAgent.IsSet()
                   && mobileContains.Any(s => userAgent.IndexOf(s, StringComparison.OrdinalIgnoreCase) > 0);
        }

        /// <summary>
        /// Returns if a forbidden bot.
        /// </summary>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <returns>
        /// The is forbidden ad bot.
        /// </returns>
        public static bool IsForbiddenAdBot([CanBeNull] string userAgent)
        {
            var botSignatures =
                Config.AdBotsForbiddenSignatures.Where(m => m.IsSet()).Select(m => m.Trim().ToLowerInvariant());

            return userAgent.IsSet()
                   && botSignatures.Any(s => userAgent.IndexOf(s, StringComparison.OrdinalIgnoreCase) > 0);
        }

        /// <summary>
        /// Sets if a user agent pattern is not checked against cookies support and JS.
        /// </summary>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <returns>
        /// The Is Not Checked For Cookies And JS.
        /// </returns>
        public static bool IsNotCheckedForCookiesAndJs([CanBeNull] string userAgent)
        {
            if (userAgent.IsSet())
            {
                string[] userAgentContains = { "W3C_Validator" };
                return userAgentContains.Any(x => userAgent.ToLowerInvariant().Contains(x.ToLowerInvariant()));
            }

            return false;
        }

        /// <summary>
        /// Validates if the user agent is a search engine spider
        /// </summary>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <returns>
        /// The is search engine spider.
        /// </returns>
        public static bool IsSearchEngineSpider([CanBeNull] string userAgent)
        {
            if (userAgent.IsNotSet())
            {
                return false;
            }

            string detectName;
            userAgent = userAgent.ToLowerInvariant();
            foreach (string spiderAll in SpiderContains)
            {
                if (spiderAll.Contains(":"))
                {
                    var namesArr = spiderAll.Split(new[] { ':' });
                    detectName = namesArr[0].Trim();

                }
                else
                {
                    detectName = spiderAll.Trim();

                }
                if (userAgent.Contains(detectName.ToLowerInvariant()))
                {
                    return true;

                }
            }

            return false;
        }

        /// <summary>
        /// Returns a platform user friendly name.
        /// </summary>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <param name="isCrawler">
        /// Is Crawler.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        /// <param name="browser">
        /// The browser.
        /// </param>
        /// <param name="isSearchEngine">
        /// Is search engine.
        /// </param>
        /// <param name="isIgnoredForDisplay">
        /// Is ignored for display. </param>
        public static void Platform(
            [CanBeNull] string userAgent,
            bool isCrawler,
            [NotNull] ref string platform,
            [NotNull] ref string browser,
            out bool isSearchEngine,
            out bool isIgnoredForDisplay)
        {
            CodeContracts.ArgumentNotNull(platform, "platform");

            isSearchEngine = false;

            if (userAgent.IsNotSet())
            {
                platform = "[Empty]";
                isIgnoredForDisplay = true;

                return;
            }
           
            if (userAgent.IndexOf("Windows NT 6.1", StringComparison.Ordinal) >= 0)
            {
                platform = "Win7";
            }
            else if (userAgent.IndexOf("Windows NT 6.2", StringComparison.Ordinal) >= 0)
            {
                platform = "Win8";
            }
            else if (userAgent.IndexOf("Windows NT 6.0", StringComparison.Ordinal) >= 0)
            {
                platform = "Vista";
            }
            else if (userAgent.IndexOf("Windows NT 5.1", StringComparison.Ordinal) >= 0)
            {
                platform = "WinXP";
            }
            else if (userAgent.IndexOf("Linux", StringComparison.Ordinal) >= 0)
            {
                platform = "Linux";
            }
            else if (userAgent.IndexOf("iPad", StringComparison.Ordinal) >= 0)
            {
                platform = "iPad(iOS)";
            }
            else if (userAgent.IndexOf("iPhone", StringComparison.Ordinal) >= 0)
            {
                platform = "iPhone(iOS)";
            }
            else if (userAgent.IndexOf("iPod", StringComparison.Ordinal) >= 0)
            {
                platform = "iPod(iOS)";
            }
            else if (userAgent.IndexOf("WindowsMobile", StringComparison.Ordinal) >= 0)
            {
                platform = "WindowsMobile";
            }
            else if (userAgent.IndexOf("webOS", StringComparison.Ordinal) >= 0)
            {
                platform = "WebOS";
            }
            else if (userAgent.IndexOf("Windows Phone OS", StringComparison.Ordinal) >= 0)
            {
                platform = "Windows Phone";
            }
            else if (userAgent.IndexOf("Android", StringComparison.Ordinal) >= 0)
            {
                platform = "Android";
            }
            else if (userAgent.IndexOf("Mac OS X", StringComparison.Ordinal) >= 0)
            {
                platform = "Mac OS X";
            }
            else if (userAgent.IndexOf("Windows NT 5.2", StringComparison.Ordinal) >= 0)
            {
                platform = "Win2003";
            }
            else if (userAgent.IndexOf("FreeBSD", StringComparison.Ordinal) >= 0)
            {
                platform = "FreeBSD";
            }

            // check if it's a search engine spider or an ignored UI string...
            var san = SearchEngineSpiderName(userAgent);
            if (san.IsSet())
            {
                browser = san;
            }
            if (FeedReaderName(userAgent).IsSet())
            {
                browser = FeedReaderName(userAgent);
            }

            isSearchEngine = san.IsSet() || isCrawler;
            isIgnoredForDisplay = IsIgnoredForDisplay(userAgent) | isSearchEngine;
        }
        
        /// <summary>
        /// Validates if the user agent is a search engine spider
        /// </summary>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <returns>
        /// The is search engine spider.
        /// </returns>
        public static string SearchEngineSpiderName([CanBeNull] string userAgent)
        {
            if (userAgent.IsNotSet())
            {
                return null;
            }

            string detectName;
            string displayName;
            userAgent = userAgent.ToLowerInvariant();
            foreach (string spiderAll in SpiderContains)
            {
                if (spiderAll.Contains(":"))
                {
                    var namesArr = spiderAll.Split(new[] {':'});
                    detectName = namesArr[0].Trim();
                    displayName = namesArr[1].Trim();

                }
                else
                {
                    detectName =
                        displayName = spiderAll.Trim();

                }
                if (userAgent.Contains(detectName.ToLowerInvariant()))
                {
                    return displayName;

                }
            }

            return null;
        }


        #endregion
    }
}