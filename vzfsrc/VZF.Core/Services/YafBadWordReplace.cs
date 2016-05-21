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
 * File YafBadWordReplace.cs created  on 2.6.2015 in  6:29 AM.
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

namespace YAF.Core.Services
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Text.RegularExpressions;

  using VZF.Data.Common;

  
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using VZF.Utils;

  #endregion

  /// <summary>
  /// The yaf bad word replace.
  /// </summary>
  public class YafBadWordReplace : IBadWordReplace
  {
    #region Constants and Fields

    /// <summary>
    ///   The _options.
    /// </summary>
    private const RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Compiled
                               /*| RegexOptions.Singleline | RegexOptions.Multiline*/;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YafBadWordReplace"/> class.
    /// </summary>
    /// <param name="objectStore">
    /// The object Store.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public YafBadWordReplace([NotNull] IObjectStore objectStore, [NotNull] ILogger logger)
    {
      this.ObjectStore = objectStore;
      this.Logger = logger;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    /// Gets or sets ObjectStore.
    /// </summary>
    public IObjectStore ObjectStore { get; set; }

    /// <summary>
    ///   Gets ReplaceItems.
    /// </summary>
    public IEnumerable<BadWordReplaceItem> ReplaceItems
    {
      get
      {
        var replaceItems = this.ObjectStore.GetOrSet(
          Constants.Cache.ReplaceWords, 
          () =>
            {
                var replaceWords = CommonDb.replace_words_list(YafContext.Current.PageModuleID, YafContext.Current.PageBoardID, null).AsEnumerable();

              // move to collection...
              return
                replaceWords.Select(
                  row => new BadWordReplaceItem(row.Field<string>("goodword"), row.Field<string>("badword"), _options)).ToList();
            });

        return replaceItems;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IBadWordReplace

    /// <summary>
    /// Searches through SearchText and replaces "bad words" with "good words"
    /// as defined in the database.
    /// </summary>
    /// <param name="searchText">
    /// The string to search through.
    /// </param>
    /// <returns>
    /// The replace.
    /// </returns>
    /// <exception cref="Exception">
    /// <c>Exception</c>.
    /// </exception>
    [NotNull]
    public string Replace([NotNull] string searchText)
    {
      if (searchText.IsNotSet())
      {
        return searchText;
      }

      string strReturn = searchText;

      foreach (BadWordReplaceItem item in this.ReplaceItems)
      {
        try
        {
            if (item.BadWordRegEx != null && item.Active)
          {
            strReturn = item.BadWordRegEx.Replace(strReturn, item.GoodWord);
          }
        }

#if DEBUG
        catch (Exception e)
        {
          throw new Exception("Bad Word Regular Expression Failed: " + e.Message, e);
        }

#else
        catch (Exception x)
        {
          // disable this regular expression henceforth...
          item.Active = false;
          this.Logger.Warn(x, "Couldn't run RegEx for Bad Word Replace value: {0}", item.BadWordRegEx);
        }

#endif
      }

      return strReturn;
    }

    #endregion

    #endregion
  }
}