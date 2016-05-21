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
 * File YafBuddy.cs created  on 2.6.2015 in  6:29 AM.
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
  using System.Data;

  using VZF.Data.Common;

  
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;
  using VZF.Utils;

  #endregion

  /// <summary>
  /// Yaf buddies service
  /// </summary>
  public class YafBuddy : IBuddy, IHaveServiceLocator
  {
    #region Constants and Fields

    /// <summary>
    /// The _db broker.
    /// </summary>
    private readonly IDBBroker _dbBroker;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YafBuddy"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="dbBroker">
    /// The db broker.
    /// </param>
    public YafBuddy([NotNull] IServiceLocator serviceLocator, [NotNull] IDBBroker dbBroker)
    {
      this.ServiceLocator = serviceLocator;
      this._dbBroker = dbBroker;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    #endregion

    #region Implemented Interfaces

    #region IBuddy

    /// <summary>
    /// Adds a buddy request.
    /// </summary>
    /// <param name="toUserID">
    /// the to user id.
    /// </param>
    /// <returns>
    /// The name of the second user + whether this request is approved or not. (This request
    ///   is approved without the target user's approval if the target user has sent a buddy request
    ///   to current user too or if the current user is already in the target user's buddy list.
    /// </returns>
    public string[] AddRequest(int toUserID)
    {
      this.ClearCache(toUserID);

      return CommonDb.buddy_addrequest(YafContext.Current.PageModuleID, YafContext.Current.PageUserID, toUserID);
    }

    /// <summary>
    /// Approves all buddy requests for the current user.
    /// </summary>
    /// <param name="mutual">
    /// should the users be added to current user's buddy list too?
    /// </param>
    public void ApproveAllRequests(bool mutual)
    {
      DataTable dt = this.All();
      DataView dv = dt.DefaultView;
      dv.RowFilter = "Approved = 0 AND UserID = {0}".FormatWith(YafContext.Current.PageUserID);
      foreach (DataRowView drv in dv)
      {
        this.ApproveRequest((int)drv["FromUserID"], mutual);
      }
    }

    /// <summary>
    /// Approves a buddy request.
    /// </summary>
    /// <param name="toUserID">
    /// the to user id.
    /// </param>
    /// <param name="mutual">
    /// should the second user be added to current user's buddy list too?
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    public string ApproveRequest(int toUserID, bool mutual)
    {
      this.ClearCache(toUserID);
      return CommonDb.buddy_approveRequest(YafContext.Current.PageModuleID, toUserID, YafContext.Current.PageUserID, mutual);
    }

    /// <summary>
    /// Gets all the buddies of the current user.
    /// </summary>
    /// <returns>
    /// A <see cref="DataTable"/> of all buddies.
    /// </returns>
    public DataTable All()
    {
      return this._dbBroker.UserBuddyList(YafContext.Current.PageUserID);
    }

    /// <summary>
    /// Clears the buddies cache for the current user.
    /// </summary>
    /// <param name="userID">
    /// The User ID.
    /// </param>
    public void ClearCache(int userId)
    {
      this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(YafContext.Current.PageUserID));
      this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));
    }

    /// <summary>
    /// Denies all buddy requests for the current user.
    /// </summary>
    public void DenyAllRequests()
    {
      DataTable dt = this.All();
      DataView dv = dt.DefaultView;
      dv.RowFilter = "Approved = 0 AND UserID = {0}".FormatWith(YafContext.Current.PageUserID);
      foreach (DataRowView drv in dv)
      {
        if (Convert.ToDateTime(drv["Requested"]).AddDays(14) < DateTime.UtcNow)
        {
          this.DenyRequest((int)drv["FromUserID"]);
        }
      }
    }

    /// <summary>
    /// Denies a buddy request.
    /// </summary>
    /// <param name="toUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// the name of the second user.
    /// </returns>
    public string DenyRequest(int toUserID)
    {
      this.ClearCache(toUserID);
      return CommonDb.buddy_denyRequest(YafContext.Current.PageModuleID, toUserID, YafContext.Current.PageUserID);
    }

    /// <summary>
    /// Gets all the buddies for the specified user.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// a <see cref="DataTable"/> of all buddies.
    /// </returns>
    public DataTable GetForUser(int userID)
    {
      return this._dbBroker.UserBuddyList(userID);
    }

    /// <summary>
    /// determines if the "<paramref name="buddyUserID"/>" and current user are buddies.
    /// </summary>
    /// <param name="buddyUserID">
    /// The Buddy User ID.
    /// </param>
    /// <param name="approved">
    /// Just look into approved buddies?
    /// </param>
    /// <returns>
    /// true if they are buddies, <see langword="false"/> if not.
    /// </returns>
    public bool IsBuddy(int buddyUserID, bool approved)
    {
      if (buddyUserID == YafContext.Current.PageUserID)
      {
        return true;
      }

      DataTable userBuddyList = this._dbBroker.UserBuddyList(YafContext.Current.PageUserID);

      if ((userBuddyList != null) && (userBuddyList.Rows.Count > 0))
      {
        // Filter the DataTable.
        if (approved)
        {
          if (userBuddyList.Select("UserID = {0} AND Approved = 1".FormatWith(buddyUserID)).Length > 0)
          {
            return true;
          }
        }
        else
        {
          if (userBuddyList.Select("UserID = {0}".FormatWith(buddyUserID)).Length > 0)
          {
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Removes the "<paramref name="toUserID"/>" from current user's buddy list.
    /// </summary>
    /// <param name="toUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    public string Remove(int toUserID)
    {
      this.ClearCache(toUserID);
      return CommonDb.buddy_remove(YafContext.Current.PageModuleID, YafContext.Current.PageUserID, toUserID);
    }

    #endregion

    #endregion
  }
}