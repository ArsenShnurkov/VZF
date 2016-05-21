﻿
#region copyright
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
 * File ActiveLocation.cs created  on 2.6.2015 in  6:29 AM.
 * Last changed on 5.21.2016 in 1:07 PM.
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


namespace YAF.Types.Interfaces
{
    /// <summary>
    /// The digest interface
    /// </summary>
    public interface IDigest
    {
        #region Public Methods

        /// <summary>
        /// Gets the digest HTML.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="webServiceToken">The web service token.</param>
        /// <param name="showErrors">if set to <c>true</c> [show errors].</param>
        /// <returns>
        /// The get digest html.
        /// </returns>
        string GetDigestHtml(int userId, int boardId, string webServiceToken, bool showErrors = false);

        /// <summary>
        /// Gets the digest URL.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="webServiceToken">The web service token.</param>
        /// <returns>
        /// The get digest url.
        /// </returns>
        string GetDigestUrl(int userId, int boardId, string webServiceToken);

        /// <summary>
        /// Gets the digest URL.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="webServiceToken">The web service token.</param>
        /// <param name="showErrors">Show digest generation errors</param>
        /// <returns>
        /// The get digest url.
        /// </returns>
        string GetDigestUrl(int userId, int boardId, string webServiceToken, bool showErrors);

        /// <summary>
        /// Sends the digest html to the email/name specified.
        /// </summary>
        /// <param name="digestHtml">The digest html.</param>
        /// <param name="forumName">The forum name.</param>
        /// <param name="forumEmail">The forum email.</param>
        /// <param name="toEmail">The to email.</param>
        /// <param name="toName">The to name.</param>
        /// <param name="sendQueued">The send queued.</param>
        void SendDigest(
            [NotNull] string digestHtml,
            [NotNull] string forumName,
            [NotNull] string forumEmail,
            [NotNull] string toEmail,
            [CanBeNull] string toName,
            bool sendQueued);

        #endregion
    }
}