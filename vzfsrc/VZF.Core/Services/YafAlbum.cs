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
 * File YafAlbum.cs created  on 2.6.2015 in  6:29 AM.
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
    using System.IO;
    using System.Linq;

    using VZF.Data.Common;

    
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using VZF.Utils;

  #endregion

    /// <summary>
  /// Album Service for the current user.
  /// </summary>
  public class YafAlbum
  {
    #region Public Methods

    /// <summary>
    /// Deletes the specified album/image.
    /// </summary>
    /// <param name="upDir">
    /// The Upload dir.
    /// </param>
    /// <param name="albumID">
    /// The album id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="imageID">
    /// The image id.
    /// </param>
    public static void Album_Image_Delete([NotNull] object upDir, [CanBeNull] object albumID, int userID, [NotNull] object imageID)
    {
      if (albumID != null)
      {
        var dt = CommonDb.album_image_list(YafContext.Current.PageModuleID, albumID, null);

        foreach (var fullName in from DataRow dr in dt.Rows
                                 select "{0}/{1}.{2}.{3}.yafalbum".FormatWith(upDir, userID, albumID, dr["FileName"]) into fullName 
                                 let file = new FileInfo(fullName) 
                                 where file.Exists 
                                 select fullName)
        {
            File.Delete(fullName);
        }

        CommonDb.album_delete(YafContext.Current.PageModuleID, albumID);
      }
      else
      {
          using (DataTable dt = CommonDb.album_image_list(YafContext.Current.PageModuleID, null, imageID))
        {
          var dr = dt.Rows[0];
          var fileName = dr["FileName"].ToString();
          var imgAlbumID = dr["albumID"].ToString();
          var fullName = "{0}/{1}.{2}.{3}.yafalbum".FormatWith(upDir, userID, imgAlbumID, fileName);
          var file = new FileInfo(fullName);
          if (file.Exists)
          {
            File.Delete(fullName);
          }
        }

        CommonDb.album_image_delete(YafContext.Current.PageModuleID, imageID);
      }
    }

    /// <summary>
    /// The change album title.
    /// </summary>
    /// <param name="albumID">
    /// The album id.
    /// </param>
    /// <param name="newTitle">
    /// The New title.
    /// </param>
    /// <returns>
    /// the return object.
    /// </returns>
    public static ReturnClass ChangeAlbumTitle(int albumID, [NotNull] string newTitle)
    {
      // load the DB so YafContext can work...
      CodeContracts.ArgumentNotNull(newTitle, "newTitle");

      YafContext.Current.Get<StartupInitializeDb>().Run();

      // newTitle = System.Web.HttpUtility.HtmlEncode(newTitle);
      CommonDb.album_save(YafContext.Current.PageModuleID, albumID, null, newTitle, null);

      var returnObject = new ReturnClass { NewTitle = newTitle };

        returnObject.NewTitle = (newTitle == string.Empty)
                                ? YafContext.Current.Get<ILocalization>().GetText("ALBUM", "ALBUM_CHANGE_TITLE")
                                : newTitle;
      returnObject.Id = "0{0}".FormatWith(albumID.ToString());
      return returnObject;
    }

    /// <summary>
    /// The change image caption.
    /// </summary>
    /// <param name="imageID">
    /// The Image id.
    /// </param>
    /// <param name="newCaption">
    /// The New caption.
    /// </param>
    /// <returns>
    /// the return object.
    /// </returns>
    public static ReturnClass ChangeImageCaption(int imageID, [NotNull] string newCaption)
    {
      // load the DB so YafContext can work...
      CodeContracts.ArgumentNotNull(newCaption, "newCaption");

      YafContext.Current.Get<StartupInitializeDb>().Run();

      // newCaption = System.Web.HttpUtility.HtmlEncode(newCaption);
      CommonDb.album_image_save(YafContext.Current.PageModuleID, imageID, null, newCaption, null, null, null);
      var returnObject = new ReturnClass { NewTitle = newCaption };

        returnObject.NewTitle = (newCaption == string.Empty)
                                ? YafContext.Current.Get<ILocalization>().GetText("ALBUM", "ALBUM_IMAGE_CHANGE_CAPTION")
                                : newCaption;
      returnObject.Id = imageID.ToString();
      return returnObject;
    }

    #endregion

    /// <summary>
    /// the HTML elements class.
    /// </summary>
    [Serializable]
    public class ReturnClass
    {
      #region Properties

      /// <summary>
      ///  Gets or sets the Album/Image's Id
      /// </summary>
      public string Id { get; set; }

      /// <summary>
      ///   Gets or sets the album/image's new Title/Caption
      /// </summary>
      public string NewTitle { get; set; }

      #endregion
    }
  }
}