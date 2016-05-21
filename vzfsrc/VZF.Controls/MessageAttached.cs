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
 * File MessageAttached.cs created  on 2.6.2015 in  6:29 AM.
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

namespace VZF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    using VZF.Data.Common;

    using YAF.Classes;
    
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using VZF.Utils;

    #endregion

    /// <summary>
    /// The message attached.
    /// </summary>
    public class MessageAttached : BaseControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _user name.
        /// </summary>
        private string _userName = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets MessageID.
        /// </summary>
        public int MessageID { get; set; }

        /// <summary>
        ///   Gets or sets UserName.
        /// </summary>
        public string UserName
        {
            get
            {
                return this._userName;
            }

            set
            {
                this._userName = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            writer.BeginRender();
            writer.WriteBeginTag("div");
            writer.WriteAttribute("id", this.ClientID);
            writer.Write(HtmlTextWriter.TagRightChar);

            if (this.MessageID != 0)
            {
                this.RenderAttachedFiles(writer);
            }

            base.Render(writer);

            writer.WriteEndTag("div");
            writer.EndRender();
        }

        /// <summary>
        /// The render attached files.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected void RenderAttachedFiles([NotNull] HtmlTextWriter writer)
        {
            string[] aImageExtensions = { "jpg", "gif", "png", "bmp" };

            string stats = this.GetText("ATTACHMENTINFO");
            string strFileIcon = this.PageContext.Get<ITheme>().GetItem("ICONS", "ATTACHED_FILE");

            ////string attachGroupId = Guid.NewGuid().ToString().Substring(0, 5);

            YafContext.Current.Get<HttpSessionStateBase>()["imagePreviewWidth"] =
                this.Get<YafBoardSettings>().ImageAttachmentResizeWidth;
            YafContext.Current.Get<HttpSessionStateBase>()["imagePreviewHeight"] =
                this.Get<YafBoardSettings>().ImageAttachmentResizeHeight;
            YafContext.Current.Get<HttpSessionStateBase>()["imagePreviewCropped"] =
                this.Get<YafBoardSettings>().ImageAttachmentResizeCropped;
            YafContext.Current.Get<HttpSessionStateBase>()["localizationFile"] =
                this.Get<ILocalization>().LanguageFileName;

            using (DataTable attachListDT = CommonDb.attachment_list(PageContext.PageModuleID, this.MessageID, null, null,0,1000000))
            {
                // show file then image attachments...
                int tmpDisplaySort = 0;

                writer.Write(@"<div class=""fileattach smallfont ceebox"">");

                while (tmpDisplaySort <= 1)
                {
                    bool bFirstItem = true;

                    foreach (DataRow dr in attachListDT.Rows)
                    {
                        string strFilename = Convert.ToString(dr["FileName"]).ToLower();
                        bool bShowImage = false;

                        // verify it's not too large to display
                        // Ederon : 02/17/2009 - made it board setting
                        if (dr["Bytes"].ToType<int>() <= this.Get<YafBoardSettings>().PictureAttachmentDisplayTreshold)
                        {
                            // is it an image file?
                            bShowImage = aImageExtensions.Any(t => strFilename.ToLower().EndsWith(t));
                        }

                        if (bShowImage && tmpDisplaySort == 1)
                        {
                            if (bFirstItem)
                            {
                                writer.Write(@"<div class=""imgtitle"">");

                                writer.Write(
                                    this.GetText("IMAGE_ATTACHMENT_TEXT"),
                                    this.HtmlEncode(Convert.ToString(this.UserName)));

                                writer.Write("</div>");

                                bFirstItem = false;
                            }

                            // Ederon : download rights
                            if (this.PageContext.ForumDownloadAccess || this.PageContext.ForumModeratorAccess)
                            {
                                // user has rights to download, show him image
                                if (!this.Get<YafBoardSettings>().EnableImageAttachmentResize)
                                {
                                    writer.Write(
                                        @"<div class=""attachedimg""><img src=""{0}resource.ashx?a={1}"" alt=""{2}"" /></div>",
                                        YafForumInfo.ForumClientFileRoot,
                                        dr["AttachmentID"],
                                        this.HtmlEncode(dr["FileName"]));
                                }
                                else
                                {
                                    var attachFilesText =
                                        "{0} {1}".FormatWith(
                                            this.GetText("IMAGE_ATTACHMENT_TEXT").FormatWith(
                                                this.HtmlEncode(Convert.ToString(this.UserName))),
                                            this.HtmlEncode(dr["FileName"]));

                                    // TommyB: Start MOD: Preview Images
                                    writer.Write(
                                        @"<div class=""attachedimg"" style=""display: inline;""><a href=""{0}resource.ashx?i={1}"" title=""{2}"" title=""{2}"" date-img=""{0}resource.ashx?a={1}""><img src=""{0}resource.ashx?p={1}"" alt=""{3}"" title=""{2}"" /></a></div>",
                                        YafForumInfo.ForumClientFileRoot,
                                        dr["AttachmentID"],
                                        attachFilesText,
                                        this.HtmlEncode(dr["FileName"]));

                                    // TommyB: End MOD: Preview Images
                                }
                            }
                            else
                            {
                                int kb = (1023 + (int)dr["Bytes"]) / 1024;

                                // user doesn't have rights to download, don't show the image
                                writer.Write(@"<div class=""attachedfile"">");

                                writer.Write(
                                    @"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>",
                                    strFileIcon,
                                    dr["FileName"],
                                    stats.FormatWith(kb, dr["Downloads"]));

                                writer.Write(@"</div>");
                            }
                        }
                        else if (!bShowImage && tmpDisplaySort == 0)
                        {
                            if (bFirstItem)
                            {
                                writer.Write(@"<div class=""filetitle"">{0}</div>", this.GetText("ATTACHMENTS"));
                                bFirstItem = false;
                            }

                            // regular file attachment
                            int kb = (1023 + (int)dr["Bytes"]) / 1024;

                            writer.Write(@"<div class=""attachedfile"">");

                            // Ederon : download rights
                            if (this.PageContext.ForumDownloadAccess || this.PageContext.ForumModeratorAccess)
                            {
                                writer.Write(
                                    @"<img border=""0"" alt="""" src=""{0}"" /> <a class=""attachedImageLink {{html:false,image:false,video:false}}"" href=""{1}resource.ashx?a={2}"">{3}</a> <span class=""attachmentinfo"">{4}</span>",
                                    strFileIcon,
                                    YafForumInfo.ForumClientFileRoot,
                                    dr["AttachmentID"],
                                    dr["FileName"],
                                    stats.FormatWith(kb, dr["Downloads"]));
                            }
                            else
                            {
                                writer.Write(
                                    @"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>",
                                    strFileIcon,
                                    dr["FileName"],
                                    stats.FormatWith(kb, dr["Downloads"]));
                            }

                            writer.Write(@"</div>");
                        }
                    }

                    // now show images
                    tmpDisplaySort++;
                }

                if (!this.PageContext.ForumDownloadAccess)
                {
                    writer.Write(@"<br /><div class=""attachmentinfo"">");

                    writer.Write(
                        this.PageContext.IsGuest
                            ? this.GetText("POSTS", "CANT_DOWNLOAD_REGISTER")
                            : this.GetText("POSTS", "CANT_DOWNLOAD"));

                    writer.Write(@"</div>");
                }

                writer.Write(@"</div>");
            }
        }

        #endregion
    }
}