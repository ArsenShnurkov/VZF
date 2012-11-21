/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Extensions;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Renders the "Last Post" part of the Forum Topics
    /// </summary>
    public partial class ForumLastPost : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The Go to last post Image ToolTip.
        /// </summary>
        private string _alt;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ForumLastPost" /> class.
        /// </summary>
        public ForumLastPost()
        {
            this.PreRender += this.ForumLastPost_PreRender;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Alt.
        /// </summary>
        [NotNull]
        public string Alt
        {
            get
            {
                return string.IsNullOrEmpty(this._alt) ? string.Empty : this._alt;
            }

            set
            {
                this._alt = value;
            }
        }

        /// <summary>
        ///   Gets or sets DataRow.
        /// </summary>
        public DataRow DataRow { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// The get avatar url from id.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// Returns the Avatar Url for the User
        /// </returns>
        protected string GetAvatarUrlFromID(int userID)
        {
            string avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userID);

            if (avatarUrl.IsNotSet())
            {
                avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
            }

            return avatarUrl;
        }

        protected string GetAvatarTitle([NotNull] DataRow row)
        {
            var avatarTitle = this.GetTextFormatted(
                  "USER_AVATAR",
                  this.HtmlEncode(row[this.Get<YafBoardSettings>().EnableDisplayName ? "LastUserDisplayName" : "LastUser"].
                      ToString()));
            return avatarTitle;
        }
        /// <summary>
        /// Handles the PreRender event of the ForumLastPost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ForumLastPost_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.DataRow == null)
            {
                return;
            }

            var showLastLinks = true;

            if (this.DataRow["ReadAccess"].ToType<int>() == 0)
            {
                this.TopicInPlaceHolder.Visible = false;
                showLastLinks = false;
            }

            if (this.DataRow["LastPosted"] != DBNull.Value)
            {
                // Last Post Date
                this.LastPostDate.DateTime = this.DataRow["LastPosted"];

                // Topic Link
                this.topicLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, "t={0}", this.DataRow["LastTopicID"]);


                this.topicLink.ToolTip = this.GetText("COMMON", "VIEW_TOPIC");

                var styles = this.Get<YafBoardSettings>().UseStyledTopicTitles
                                 ? this.Get<IStyleTransform>().DecodeStyleByString(
                                     this.DataRow["LastTopicStyles"].ToString(), false)
                                 : string.Empty;

                if (styles.IsSet())
                {
                    this.topicLink.Attributes.Add("style", styles);
                }

                // Topic Status
                if (this.DataRow["LastTopicStatus"].ToString().IsSet() && this.Get<YafBoardSettings>().EnableTopicStatus)
                {
                    var topicStatusIcon = this.Get<ITheme>().GetItem(
                        "TOPIC_STATUS", this.DataRow["LastTopicStatus"].ToString());

                    if (topicStatusIcon.IsSet() && !topicStatusIcon.Contains("[TOPIC_STATUS."))
                    {
                        this.topicLink.Text =
                            "<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" style=\"border: 0;width:16px;height:16px\" />&nbsp;{2}"
                                .FormatWith(
                                    this.Get<ITheme>().GetItem(
                                        "TOPIC_STATUS", this.DataRow["LastTopicStatus"].ToString()),
                                    this.GetText("TOPIC_STATUS", this.DataRow["LastTopicStatus"].ToString()),
                                    this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.DataRow["LastTopicName"])).Truncate(50));
                    }
                    else
                    {
                        this.topicLink.Text =
                            "[{0}]&nbsp;{1}".FormatWith(
                                this.GetText("TOPIC_STATUS", this.DataRow["LastTopicStatus"].ToString()),
                                this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.DataRow["LastTopicName"])).Truncate(50));
                    }
                }
                else
                {
                    this.topicLink.Text =
                        this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.DataRow["LastTopicName"].ToString())).Truncate(50);
                }

                // Last Topic User
                this.ProfileUserLink.UserID = this.DataRow["LastUserID"].ToType<int>();
                this.ProfileUserLink.Style = this.Get<YafBoardSettings>().UseStyledNicks
                                                 ? this.Get<IStyleTransform>().DecodeStyleByString(
                                                     this.DataRow["Style"].ToString(), false)
                                                 : string.Empty;
                this.ProfileUserLink.ReplaceName =
                    this.DataRow[this.Get<YafBoardSettings>().EnableDisplayName ? "LastUserDisplayName" : "LastUser"].
                        ToString();

                if (string.IsNullOrEmpty(this.Alt))
                {
                    this.Alt = this.GetText("GO_LAST_POST");
                }

                this.LastTopicImgLink.ToolTip = this.Alt;

                var lastRead =
                    this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                        forumId: this.DataRow["ForumID"].ToType<int>(),
                        topicId: this.DataRow["LastTopicID"].ToType<int>(),
                        forumReadOverride: this.DataRow["LastForumAccess"].ToType<DateTime?>(),
                        topicReadOverride: this.DataRow["LastTopicAccess"].ToType<DateTime?>());

                var showNewIcon = DateTime.Parse(Convert.ToString(this.DataRow["LastPosted"])) > lastRead;

                this.LastTopicImgLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, "m={0}&find=lastpost", this.DataRow["LastMessageID"]);

                this.Icon.ThemeTag = showNewIcon ? "ICON_NEWEST" : "ICON_LATEST";

                this.Icon.Alt = this.LastTopicImgLink.ToolTip;

                this.ImageLastUnreadMessageLink.Visible = this.Get<YafBoardSettings>().ShowLastUnreadPost;

                if (this.ImageLastUnreadMessageLink.Visible)
                {
                    this.ImageLastUnreadMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                        ForumPages.posts, "t={0}&find=unread", this.DataRow["LastTopicID"]);

                    this.LastUnreadImage.LocalizedTitle = this.GetText("DEFAULT", "GO_LASTUNREAD_POST");

                    this.LastUnreadImage.ThemeTag = showNewIcon ? "ICON_NEWEST_UNREAD" : "ICON_LATEST_UNREAD";
                }
                // temporarily desabled.
                if (false && showLastLinks && this.Get<YafBoardSettings>().ShowAvatarsInTopic)
                {
                    this.userAvatar.Src = this.GetAvatarUrlFromID(this.DataRow["LastUserID"].ToType<int>());
                    this.userAvatar.Alt = this.userAvatar.Attributes["title"] = GetAvatarTitle(this.DataRow);
                    this.userAvatar.Visible = true;
                }
                this.LastTopicImgLink.Enabled = this.ImageLastUnreadMessageLink.Enabled = showLastLinks;

                this.LastPostedHolder.Visible = true;
                this.NoPostsLabel.Visible = false;
            }
            else
            {
                // show "no posts"
                this.LastPostedHolder.Visible = false;
                this.NoPostsLabel.Visible = true;
            }
        }

        #endregion
    }
}