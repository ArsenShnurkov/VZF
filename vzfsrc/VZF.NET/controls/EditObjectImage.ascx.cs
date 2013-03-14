﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Vladimir Zakharov" file="EditObjectImage.ascx.cs">
//   VZF by vzrus
//   Copyright (C) 2006-2013 Vladimir Zakharov
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
//   The EditObjectImage.ascx.cs is mostly based on EditAvatarImage.ascx.cs.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace VZF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    using VZF.Data.Common;

    using YAF.Classes;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The edit users avatar.
    /// </summary>
    public partial class EditObjectImage : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The admin edit mode.
        /// </summary>
        private bool _adminEditMode;

        /// <summary>
        ///   The current user id.
        /// </summary>
        private int _currentUserID;

        /// <summary>
        /// The _current topic id.
        /// </summary>
        private int _currentTopicId;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether InAdminPages.
        /// </summary>
        public bool InAdminPages
        {
            get
            {
                return this._adminEditMode;
            }

            set
            {
                this._adminEditMode = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cancel Editing
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Back_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var ti = this.Request.QueryString.GetFirstOrDefault("ti");
            int tii;

            // check if it's a link from topic edit
            if (ti != null && int.TryParse(this.Request.QueryString.GetFirstOrDefault("ti"), out tii))
            {
                YafBuildLink.Redirect(ForumPages.posts, "t={0}".FormatWith(tii));
            }
        }

        /// <summary>
        /// Delete The Current TopicImage
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteAvatar_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var ti = this.Request.QueryString.GetFirstOrDefault("ti");
            int tii;

            if (ti != null && int.TryParse(this.Request.QueryString.GetFirstOrDefault("ti"), out tii))
            {
                CommonDb.topic_imagesave(
                    PageContext.PageModuleID, this.PageContext.QueryIDs["ti"].ToType<int>(), null, null, null);
            }

            this.BindData();
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.QueryIDs = new QueryStringIDHelper(new[] { "mi", "ti", "fi", "ci", "u" }, false);

            if (this._adminEditMode && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
            {
                this._currentUserID = this.PageContext.QueryIDs["u"].ToType<int>();
            }
            else
            {
                this._currentUserID = this.PageContext.PageUserID;
            }

            var ti = this.Request.QueryString.GetFirstOrDefault("ti");

            // check if it's a link from topic edit
             if (ti != null)
             {
                 int.TryParse(ti, out this._currentTopicId);
             }

               if (this.IsPostBack)
               {
                   return;
               }


               // check if it's a link from topic edit
            /*   if (ti != null && int.TryParse(ti, out tii))
               {
                   // save the image right now...
                   CommonDb.topic_imagesave(
                       PageContext.PageModuleID,
                       ti,
                       "{0}/{1}/{2}".FormatWith(
                           BaseUrlBuilder.BaseUrl,
                           YafBoardFolders.Current.Topics,
                           this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("ti")),
                       null,
                       null);
               } */

            this.UpdateRemote.Text = this.GetText("COMMON", "UPDATE");
            this.UpdateUpload.Text = this.GetText("COMMON", "UPDATE");
            this.Back.Text = this.GetText("COMMON", "BACK");

            this.NoImage.Text = this.GetText("IMAGEADD", "NOIMAGE");

            this.DeleteImage.Text = this.GetText("IMAGEADD", "IMAGEDELETE");
            this.DeleteImage.Attributes["onclick"] =
                "return confirm('{0}?')".FormatWith(this.GetText("IMAGEADD", "IMAGEDELETE"));

            string addAdminParam = string.Empty;
            if (this._adminEditMode)
            {
                addAdminParam = "u={0}".FormatWith(this._currentUserID);
            }

            this.OurImage.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.imageadd, addAdminParam);
            this.OurImage.Text = this.GetText("IMAGEADD", "OURIMAGE_SELECT");

            this.noteRemote.Text = this.GetTextFormatted(
                "NOTE_IMG_REMOTE",
                this.Get<YafBoardSettings>().TopicImageWidth.ToString(),
                this.Get<YafBoardSettings>().TopicImageHeight.ToString());
            this.noteLocal.Text = this.GetTextFormatted(
                "NOTE_IMG_LOCAL",
                this.Get<YafBoardSettings>().TopicImageWidth.ToString(),
                this.Get<YafBoardSettings>().TopicImageHeight,
                (this.Get<YafBoardSettings>().AvatarSize / 1024).ToString());

            this.BindData();
        }

        /// <summary>
        /// Saves the Remote TopicImage
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void RemoteUpdate_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.TopicImage.Text.Length > 0 && !this.TopicImage.Text.StartsWith("http://")
                && !this.TopicImage.Text.StartsWith("https://"))
            {
                this.TopicImage.Text = "http://{0}".FormatWith(this.TopicImage.Text);
            }
            var ti = this.Request.QueryString.GetFirstOrDefault("ti");

            // check if it's a link from topic edit
            if (ti != null)
            {
                int.TryParse(ti, out this._currentTopicId);
            }

            // update
            CommonDb.topic_imagesave(PageContext.PageModuleID, this._currentTopicId, this.TopicImage.Text.Trim(), null, null);

            // clear the URL out...
            this.TopicImage.Text = string.Empty;

            this.BindData();
        }

        /// <summary>
        /// Saves the local TopicImage
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UploadUpdate_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.File.PostedFile == null || this.File.PostedFile.FileName.Trim().Length <= 0
                || this.File.PostedFile.ContentLength <= 0)
            {
                return;
            }

            long x = this.Get<YafBoardSettings>().TopicImageWidth;
            long y = this.Get<YafBoardSettings>().TopicImageHeight;
            int nImageSize = this.Get<YafBoardSettings>().AvatarSize;

            Stream resized = null;

            try
            {
                using (Image img = Image.FromStream(this.File.PostedFile.InputStream))
                {
                    if (img.Width > x || img.Height > y)
                    {
                        this.PageContext.AddLoadMessage(this.GetText("IMAGEADD", "WARN_IMG_TOOBIG").FormatWith(x, y));
                        this.PageContext.AddLoadMessage(
                            this.GetText("IMAGEADD", "WARN_IMG_SIZE").FormatWith(img.Width, img.Height));
                        this.PageContext.AddLoadMessage(this.GetText("IMAGEADD", "WARN_IMG_RESIZED"));

                        resized = ImageHelper.GetResizedImageStreamFromImage(img, x, y);
                    }
                }

                // here we save an uploaded image
                if (this.PageContext.QueryIDs["ti"] != null)
                {
                    CommonDb.topic_imagesave(
                        PageContext.PageModuleID, this.PageContext.QueryIDs["ti"].ToType<int>(), null, null, null);

                    CommonDb.topic_imagesave(
                        PageContext.PageModuleID,
                        this.PageContext.QueryIDs["ti"].ToType<int>(),
                        null,
                        resized ?? this.File.PostedFile.InputStream,
                        this.File.PostedFile.ContentType);
                }

                // clear the cache for this user...
                // this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this._currentUserID));

                if (nImageSize > 0 && this.File.PostedFile.ContentLength >= nImageSize && resized == null)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("IMAGEADD", "WARN_IMG_BIGFILE").FormatWith(nImageSize));
                    this.PageContext.AddLoadMessage(
                        this.GetText("IMAGEADD", "WARN_IMG_FILESIZE").FormatWith(this.File.PostedFile.ContentLength));
                }

                /* this.TopicImg.ImageUrl = "{0}resource.ashx?u={1}&upd={2}".FormatWith(
                     YafForumInfo.ForumClientFileRoot, this._currentUserID, DateTime.Now.Ticks); */
                if (this.PageContext.QueryIDs["ti"] != null)
                {
                    var dt = CommonDb.topic_info(
                        PageContext.PageModuleID, this.PageContext.QueryIDs["ti"].ToType<int>(), false);
                    if (dt != null)
                    {
                        this.TopicImg.ImageUrl =
                            "{0}resource.ashx?ti={1}&upd={2}".FormatWith(
                                YafForumInfo.ForumClientFileRoot, this.PageContext.QueryIDs["ti"].ToType<int>(), DateTime.Now.Ticks);
                    }
                }

                if (this.TopicImg.ImageUrl.IsSet())
                {
                    this.NoImage.Visible = false;
                }
            }
            catch (Exception)
            {
                // image is probably invalid...
                this.PageContext.AddLoadMessage(this.GetText("IMAGEADD", "INVALID_IMG_FILE"));
            }

            // this.BindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            DataRow row = null;
            int t;

            if (this.PageContext.QueryIDs["ti"] != null)
            {
                row = CommonDb.topic_info(
                    PageContext.PageModuleID, this.PageContext.QueryIDs["ti"].ToType<int>(), false);
                if (row != null)
                {
                    t = row["TopicID"].ToType<int>();
                    this.TopicImg.ImageUrl = "{0}resource.ashx?ti={1}".FormatWith(YafForumInfo.ForumClientFileRoot, t);
                    this.TopicImg.Visible = true;
                    this.TopicImage.Text = string.Empty;
                    this.DeleteImage.Visible = false;
                    this.NoImage.Visible = false;

                    if (row["TopicImageBin"] != null && row["TopicImageBin"].ToString().Length > 0)
                    {
                        this.TopicImg.ImageUrl = "{0}resource.ashx?ti={1}".FormatWith(
                            YafForumInfo.ForumClientFileRoot, t);
                        this.TopicImage.Text = string.Empty;
                        this.DeleteImage.Visible = true;
                    }
                    else if (row["TopicImage"].ToString().Length > 0)
                    {
                        // remote
                        this.TopicImg.ImageUrl =
                            "{0}resource.ashx?ti={1}&url={2}&width={3}&height={4}".FormatWith(
                                YafForumInfo.ForumClientFileRoot,
                                t,
                                this.Server.UrlEncode(row["TopicImage"].ToString()),
                                this.Get<YafBoardSettings>().TopicImageWidth,
                                this.Get<YafBoardSettings>().TopicImageHeight);
                        this.TopicImage.Text = row["TopicImage"].ToString();
                        this.DeleteImage.Visible = true;
                    }
                    else
                    {
                        this.TopicImg.ImageUrl = this.Get<ITheme>().GetItem("ICONS", "NEW");
                        this.NoImage.Visible = true;
                    }

                    int rowSpan = 2;

                    this.ImageUploadRow.Visible = this._adminEditMode || this.Get<YafBoardSettings>().AllowTopicImages;
                    this.ImageRemoteRow.Visible = this._adminEditMode || this.Get<YafBoardSettings>().AllowTopicImages;
                    this.ImageOurs.Visible = this._adminEditMode || this.Get<YafBoardSettings>().AvatarGallery;
                    this.ImageOurs.Visible = false;
                  //  if (this._adminEditMode || this.Get<YafBoardSettings>().TopicImage)
                 //   {
                       // rowSpan++;
                  //  }

                    //    if (this._adminEditMode || this.Get<YafBoardSettings>().TopicImage)
                //    {
                        rowSpan++;
                 //   }

                    this.topicImageTD.RowSpan = rowSpan;
                }
            }
        }

        #endregion
    }
}