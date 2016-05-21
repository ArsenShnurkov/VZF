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
 * File CodeRegexReplaceRule.cs created  on 2.6.2015 in  6:29 AM.
 * Last changed on 5.21.2016 in 1:04 PM.
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

namespace YAF.Core.BBCode.ReplaceRules
{
  using System.Text.RegularExpressions;

  using YAF.Types.Interfaces;

  /// <summary>
  /// Simple code block regular express replace
  /// </summary>
  public class CodeRegexReplaceRule : SimpleRegexReplaceRule
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    public CodeRegexReplaceRule(Regex regExSearch, string regExReplace)
      : base(regExSearch, regExReplace)
    {
      // default high rank...
      this.RuleRank = 2;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public override void Replace(ref string text, IReplaceBlocks replacement)
    {
      Match m = this._regExSearch.Match(text);
      while (m.Success)
      {
        string replaceItem = this._regExReplace.Replace("${inner}", this.GetInnerValue(m.Groups["inner"].Value));

        int replaceIndex = replacement.Add(replaceItem);
        text = text.Substring(0, m.Groups[0].Index) + replacement.Get(replaceIndex) +
               text.Substring(m.Groups[0].Index + m.Groups[0].Length);

        m = this._regExSearch.Match(text);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// This just overrides how the inner value is handled
    /// </summary>
    /// <param name="innerValue">
    /// </param>
    /// <returns>
    /// The get inner value.
    /// </returns>
    protected override string GetInnerValue(string innerValue)
    {
      innerValue = innerValue.Replace("\t", "&nbsp; &nbsp;&nbsp;");
      innerValue = innerValue.Replace("[", "&#91;");
      innerValue = innerValue.Replace("]", "&#93;");
      innerValue = innerValue.Replace("<", "&lt;");
      innerValue = innerValue.Replace(">", "&gt;");
      innerValue = innerValue.Replace("\r\n", "<br />");
      // TODO: vzrus there should not be contsructions with string.Replace and double whitespace to replace.
      // it can lead to server overloads in some situations. Seems OK.
      // TODO : tha_watcha _this creates duplicated whitespace, in simple texts its not really needed, to replace it.
      //innerValue = Regex.Replace(innerValue, @"\s+", " &nbsp;").Trim();
      // vzrus: No matter I mean comstructions like .Replace("  "," ")
      return innerValue;
    }

    #endregion
  }
}