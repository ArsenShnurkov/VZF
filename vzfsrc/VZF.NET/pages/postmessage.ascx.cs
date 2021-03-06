﻿namespace YAF.Pages
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using VZF.Data.Common;

    using YAF.Classes;
    
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using VZF.Utilities;
    using VZF.Utils;
    using VZF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The post message Page.
    /// </summary>
    public partial class postmessage : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The _forum editor.
        /// </summary>
        protected ForumEditor _forumEditor;

        /// <summary>
        ///   The original message.
        /// </summary>
        protected string _originalMessage;

        /// <summary>
        ///   The _owner user id.
        /// </summary>
        protected int _ownerUserId;

        /// <summary>
        ///   The _ux no edit subject.
        /// </summary>
        protected Label _uxNoEditSubject;

        /// <summary>
        ///   Table with choices
        /// </summary>
        protected DataTable choices;

        /// <summary>
        ///   The Spam Approved Indicator
        /// </summary>
        private bool spamApproved = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="postmessage" /> class.
        /// </summary>
        public postmessage()
            : base("POSTMESSAGE")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets EditMessageID.
        /// </summary>
        protected long? EditMessageID
        {
            get
            {
                return this.PageContext.QueryIDs["m"];
            }
        }

        /// <summary>
        ///   Gets or sets OriginalMessage.
        /// </summary>
        protected string OriginalMessage
        {
            get
            {
                return this._originalMessage;
            }

            set
            {
                this._originalMessage = value;
            }
        }

        /// <summary>
        ///   Gets Page.
        /// </summary>
        protected long? PageIndex
        {
            get
            {
                return this.PageContext.QueryIDs["page"];
            }
        }

        /// <summary>
        ///   Gets or sets the PollGroupId if the topic has a poll attached
        /// </summary>
        protected int? PollGroupId { get; set; }

        /// <summary>
        ///   Gets Quoted Message ID.
        /// </summary>
        protected long? QuotedMessageID
        {
            get
            {
                return this.PageContext.QueryIDs["q"];
            }
        }

        /// <summary>
        ///   Gets TopicID.
        /// </summary>
        protected long? TopicID
        {
            get
            {
                return this.PageContext.QueryIDs["t"];
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Canceling Posting New Message Or editing Message.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.TopicID != null || this.EditMessageID != null)
            {
                // reply to existing topic or editing of existing topic
                YafBuildLink.Redirect(ForumPages.posts, "t={0}", this.PageContext.PageTopicID);
            }
            else
            {
                // new topic -- cancel back to forum
                YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
            }
        }

        /// <summary>
        /// The get poll group id.
        /// </summary>
        /// <returns>
        /// Returns the PollGroup Id 
        /// </returns>
        protected int? GetPollGroupID()
        {
            return this.PollGroupId;
        }

        /// <summary>
        /// The handle post to blog.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="subject">
        /// The subject. 
        /// </param>
        /// <returns>
        /// Retuns the Blog Post ID 
        /// </returns>
        protected string HandlePostToBlog([NotNull] string message, [NotNull] string subject)
        {
            string blogPostID = string.Empty;

            // Does user wish to post this to their blog?
            if (this.Get<YafBoardSettings>().AllowPostToBlog && this.PostToBlog.Checked)
            {
                try
                {
                    // Post to blog
                    var blog = new MetaWeblog(this.PageContext.Profile.BlogServiceUrl);
                    blogPostID = blog.newPost(
                        this.PageContext.Profile.BlogServicePassword,
                        this.PageContext.Profile.BlogServiceUsername,
                        this.BlogPassword.Text,
                        subject,
                        message);
                }
                catch
                {
                    this.PageContext.AddLoadMessage(this.GetText("POSTTOBLOG_FAILED"));
                }
            }

            return blogPostID;
        }

        /// <summary>
        /// Verifies the user isn't posting too quickly, if so, tells them to wait.
        /// </summary>
        /// <returns>
        /// True if there is a delay in effect. 
        /// </returns>
        protected bool IsPostReplyDelay()
        {
            // see if there is a post delay
            if (!(this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
                && this.Get<YafBoardSettings>().PostFloodDelay > 0)
            {
                // see if they've past that delay point
                if (this.Get<IYafSession>().LastPost
                    > DateTime.UtcNow.AddSeconds(-this.Get<YafBoardSettings>().PostFloodDelay)
                    && this.EditMessageID == null)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetTextFormatted(
                            "wait",
                            (this.Get<IYafSession>().LastPost - DateTime.UtcNow.AddSeconds(-this.Get<YafBoardSettings>().PostFloodDelay)).Seconds));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles verification of the PostReply. Adds javascript message if there is a problem.
        /// </summary>
        /// <returns>
        /// true if everything is verified 
        /// </returns>
        protected bool IsPostReplyVerified()
        {
            // To avoid posting whitespace(s) or empty messages
            string postedMessage = this._forumEditor.Text.Trim();

            if (postedMessage.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("ISEMPTY"));
                return false;
            }

            // No need to check whitespace if they are actually posting something
            if (this.Get<YafBoardSettings>().MaxPostSize > 0
                && this._forumEditor.Text.Length >= this.Get<YafBoardSettings>().MaxPostSize)
            {
                this.PageContext.AddLoadMessage(this.GetText("ISEXCEEDED"));
                return false;
            }

            // Check if the Entered Guest Username is not too long
            if (this.FromRow.Visible && this.From.Text.Trim().Length > 100)
            {
                this.PageContext.AddLoadMessage(this.GetText("GUEST_NAME_TOOLONG"));

                this.From.Text = this.From.Text.Substring(100);
                return false;
            }

            // Check if the topic name is not too long
            if (this.Get<YafBoardSettings>().MaxWordLength > 0
                &&
                this.TopicSubjectTextBox.Text.Trim().AreAnyWordsOverMaxLength(
                    this.Get<YafBoardSettings>().MaxWordLength))
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("TOPIC_NAME_WORDTOOLONG", this.Get<YafBoardSettings>().MaxWordLength));

                try
                {
                    this.TopicSubjectTextBox.Text =
                        this.TopicSubjectTextBox.Text.Substring(this.Get<YafBoardSettings>().MaxWordLength).Substring(
                            255);
                }
                catch (Exception)
                {
                    this.TopicSubjectTextBox.Text =
                        this.TopicSubjectTextBox.Text.Substring(this.Get<YafBoardSettings>().MaxWordLength);
                }

                return false;
            }

            // Check if the topic description words are not too long
            if (this.Get<YafBoardSettings>().MaxWordLength > 0
                &&
                this.TopicDescriptionTextBox.Text.Trim().AreAnyWordsOverMaxLength(
                    this.Get<YafBoardSettings>().MaxWordLength))
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("TOPIC_DESCRIPTION_WORDTOOLONG", this.Get<YafBoardSettings>().MaxWordLength));

                try
                {
                    this.TopicDescriptionTextBox.Text =
                        this.TopicDescriptionTextBox.Text.Substring(this.Get<YafBoardSettings>().MaxWordLength).Substring(255);
                }
                catch (Exception)
                {
                    this.TopicDescriptionTextBox.Text =
                        this.TopicDescriptionTextBox.Text.Substring(this.Get<YafBoardSettings>().MaxWordLength);
                }

                return false;
            }

            if (this.SubjectRow.Visible && this.TopicSubjectTextBox.Text.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("NEED_SUBJECT"));
                return false;
            }

            if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().AllowCreateTopicsSameName)
                && CommonDb.topic_findduplicate(PageContext.PageModuleID, this.TopicSubjectTextBox.Text.Trim()) == 1 && this.TopicID == null
                && this.EditMessageID == null)
            {
                this.PageContext.AddLoadMessage(this.GetText("SUBJECT_DUPLICATE"));
                return false;
            }

            if (((this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableCaptchaForGuests)
                 || (this.Get<YafBoardSettings>().EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
                && !CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// The new topic.
        /// </summary>
        /// <returns>
        /// Returns if New Topic 
        /// </returns>
        protected bool NewTopic()
        {
            return !(this.PollGroupId > 0);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // get the forum editor based on the settings
            string editorId = this.Get<YafBoardSettings>().ForumEditor;

            if (this.Get<YafBoardSettings>().AllowUsersTextEditor)
            {
                // Text editor
                editorId = !string.IsNullOrEmpty(this.PageContext.TextEditor)
                               ? this.PageContext.TextEditor
                               : this.Get<YafBoardSettings>().ForumEditor;
            }

            // Check if Editor exists, if not fallback to default editorid=1
            this._forumEditor = this.Get<IModuleManager<ForumEditor>>().GetBy(editorId, false)
                                ?? this.Get<IModuleManager<ForumEditor>>().GetBy("1");

            // Override Editor when mobile device with default Yaf BBCode Editor
            if (this.PageContext.IsMobileDevice)
            {
                this._forumEditor = this.Get<IModuleManager<ForumEditor>>().GetBy("1");
            }

            // setup jQuery and YAF JS...
            YafContext.Current.PageElements.RegisterJQuery();

            this.EditorLine.Controls.Add(this._forumEditor);

            // Setup Syntax Highlight JS
            YafContext.Current.PageElements.RegisterJsResourceInclude(
                "syntaxhighlighter", "js/jquery.syntaxhighligher.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.syntaxhighligher.css");
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "syntaxhighlighterjs", JavaScriptBlocks.SyntaxHighlightLoadJs);

            // Setup SpellChecker JS
            YafContext.Current.PageElements.RegisterJsResourceInclude(
                "jqueryspellchecker", "js/jquery.spellchecker.min.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.spellchecker.css");

            var editorClientId = this._forumEditor.ClientID;

            editorClientId = editorClientId.Replace(
                editorClientId.Substring(editorClientId.LastIndexOf("_")), "_YafTextEditor");

            var editorSpellBtnId = "{0}_spell".FormatWith(editorClientId);

            /*if (this._forumEditor.ModuleId.Equals("5") ||
                                this._forumEditor.ModuleId.Equals("0"))
                        {
                                var spellCheckBtn = new Button
                                        {
                                                CssClass = "pbutton", 
                                                ID = "SpellCheckBtn", 
                                                Text = this.GetText("COMMON", "SPELL")
                                        };

                                this.EditorLine.Controls.Add(spellCheckBtn);

                                editorSpellBtnId = spellCheckBtn.ClientID;
                        }*/
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "spellcheckerjs",
                JavaScriptBlocks.SpellCheckerLoadJs(editorClientId, editorSpellBtnId, this.PageContext.CurrentUserData.CultureUser.IsSet()
                        ? this.PageContext.CurrentUserData.CultureUser.Substring(0, 2)
                        : this.Get<YafBoardSettings>().Culture,
                    this.GetText("SPELL_CORRECT")));

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.QueryIDs = new QueryStringIDHelper(new[] { "m", "t", "q", "page" }, false);

            TypedMessageList currentMessage = null;

            DataRow topicInfo = CommonDb.topic_info(this.PageContext.PageModuleID, this.PageContext.PageTopicID, true, false);
            if (topicInfo == null || topicInfo["TopicID"].ToType<int>() != this.PageContext.PageTopicID)
            {
                // new topic
                topicInfo = null;
            }

            PageContext.PageElements.RegisterJsBlockStartup(
                      this, "GotoAnchorJs", JavaScriptBlocks.LoadGotoAnchor("topprev"));

            // we reply to a post with a quote
            if (this.QuotedMessageID != null)
            {
                currentMessage =
                    CommonDb.MessageList(PageContext.PageModuleID, this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("q").ToType<int>()).FirstOrDefault();

                this.OriginalMessage = currentMessage.Message;

                if (currentMessage.TopicID.ToType<int>() != this.PageContext.PageTopicID)
                {
                    YafBuildLink.AccessDenied();
                }

                if (!this.CanQuotePostCheck(topicInfo))
                {
                    YafBuildLink.AccessDenied();
                }

                this.PollGroupId = currentMessage.PollID.ToType<int>().IsNullOrEmptyDBField()
                                       ? 0
                                       : currentMessage.PollID;

                // if this is a quoted message (a new reply with a quote)  we should transfer the TopicId value only to return
                this.PollList.TopicId = this.TopicID.ToType<int>();

                if (this.TopicID == null)
                {
                    this.PollList.TopicId = currentMessage.TopicID.ToType<int>().IsNullOrEmptyDBField()
                                                ? 0
                                                : currentMessage.TopicID.ToType<int>();
                }
            }
            else if (this.EditMessageID != null)
            {
                currentMessage = CommonDb.MessageList(PageContext.PageModuleID, this.EditMessageID.ToType<int>()).FirstOrDefault();

                this.OriginalMessage = currentMessage.Message;

                this._ownerUserId = currentMessage.UserID.ToType<int>();

                if (!this.CanEditPostCheck(currentMessage, topicInfo))
                {
                    YafBuildLink.AccessDenied();
                }

                this.PollGroupId = currentMessage.PollID.ToType<int>().IsNullOrEmptyDBField()
                                       ? 0
                                       : currentMessage.PollID;

                // we edit message and should transfer both the message ID and TopicID for PageLinks. 
                this.PollList.EditMessageId = this.EditMessageID.ToType<int>();

                if (this.TopicID == null)
                {
                    this.PollList.TopicId = currentMessage.TopicID.ToType<int>().IsNullOrEmptyDBField()
                                                ? 0
                                                : currentMessage.TopicID.ToType<int>();
                }
            }

            if (this.PageContext.PageForumID == 0)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>()["t"] == null && this.Get<HttpRequestBase>()["m"] == null
                && !this.PageContext.ForumPostAccess)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>()["t"] != null && !this.PageContext.ForumReplyAccess)
            {
                YafBuildLink.AccessDenied();
            }
            
            // If topic tags are disabled we don't show the tags box, any existing tags will be deleted. 
            // Else we fill in the textbox.
            if (!this.Get<YafBoardSettings>().DeleteTopicTagsIfDisabled)
            {
                // We deal with an existing topic.
                if (topicInfo != null && (topicInfo["UserID"].ToType<int>() == PageContext.CurrentUserData.UserID || PageContext.IsForumModerator || PageContext.IsAdmin))
                {
                        this.Tags.Text = HttpUtility.HtmlEncode(topicInfo["TopicTags"].ToString());
                }
            } 

            if (this.Get<YafBoardSettings>().AllowTopicTags)
            {
                // The topic is added it has not any topic info. We can edit tags for the first message only.
                if (topicInfo == null || (currentMessage != null  && currentMessage.Position == 0))
                {
                    this.TagsRow.Visible = true;
                }
            }

            if (topicInfo == null || (currentMessage != null && currentMessage.Position == 0))
            {
                bool showtopicImage = false;
                if (topicInfo != null)
                {                
                    showtopicImage = topicInfo["UserID"].ToType<int>() == PageContext.PageUserID;
                }

                this.ImageRow.Visible = this.Get<YafBoardSettings>().AllowTopicImages
                                        && (showtopicImage  || PageContext.IsForumModerator);
                            if (this.ImageRow.Visible)
                {
                    this.TopicImageAncor.HRef = YafBuildLink.GetLink(
                        ForumPages.imageadd, "ti={0}&u={1}", topicInfo["TopicID"], PageContext.PageUserID);
                    this.TopicImageAncor.Visible = true;
                    this.TopicImage.Src = this.Get<ITheme>().GetItem("ICONS", "TOPIC_NEW"); /* "{0}{1}/{2}".FormatWith(
                       YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Forums, topicInfo["TopicImage"].ToString()); */
                }
        }

            // "{0}{1}/{2}".FormatWith(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Forums, row["TopicImage"].ToString());

            // Message.EnableRTE = PageContext.BoardSettings.AllowRichEdit;
            this._forumEditor.StyleSheet = this.Get<ITheme>().BuildThemePath("theme.css");
            this._forumEditor.BaseDir = "{0}editors".FormatWith(YafForumInfo.ForumClientFileRoot);

            this.Title.Text = this.GetText("NEWTOPIC");
            
            if (this.Get<YafBoardSettings>().MaxPostSize == 0)
            {
                this.LocalizedLblMaxNumberOfPost.Visible = false;
            }
            else
            {
                this.LocalizedLblMaxNumberOfPost.Visible = true;
                this.LocalizedLblMaxNumberOfPost.Param0 =
                    this.Get<YafBoardSettings>().MaxPostSize.ToString(CultureInfo.InvariantCulture);
            }

            if (!this.IsPostBack)
            {
                if (this.Get<YafBoardSettings>().EnableTopicDescription)
                {
                    this.DescriptionRow.Visible = true;
                }

                if (this.Get<YafBoardSettings>().AllowMessageDescription)
                {
                    this.MessageDescriptionRow.Visible = true;
                }
                
                // helper bool -- true if this is a completely new topic...
                bool isNewTopic = (this.TopicID == null) && (this.QuotedMessageID == null)
                                  && (this.EditMessageID == null);

                this.Priority.Items.Add(new ListItem(this.GetText("normal"), "0"));
                this.Priority.Items.Add(new ListItem(this.GetText("sticky"), "1"));
                this.Priority.Items.Add(new ListItem(this.GetText("announcement"), "2"));
                this.Priority.SelectedIndex = 0;
               

                if (this.Get<YafBoardSettings>().EnableTopicStatus)
                {
                    this.StatusRow.Visible = true;

                    this.TopicStatus.Items.Add(new ListItem("   ", "-1"));

                    foreach (DataRow row in CommonDb.TopicStatus_List(PageContext.PageModuleID, this.PageContext.PageBoardID).Rows)
                    {
                        var text = this.GetText("TOPIC_STATUS", row["TopicStatusName"].ToString());

                        this.TopicStatus.Items.Add(
                            new ListItem(
                                text.IsSet() ? text : row["DefaultDescription"].ToString(),
                                row["TopicStatusName"].ToString()));
                    }

                    this.TopicStatus.SelectedIndex = 0;
                }

                // Allow the Styling of Topic Titles only for Mods or Admins
                if (this.Get<YafBoardSettings>().UseStyledTopicTitles
                    && (this.PageContext.ForumModeratorAccess || this.PageContext.IsAdmin))
                {
                    this.StyleRow.Visible = true;
                }
                else
                {
                    this.StyleRow.Visible = false;
                }

                this.EditReasonRow.Visible = false;

                this.PriorityRow.Visible = this.PageContext.ForumPriorityAccess;

                // Show post to blog option only to a new post
                this.BlogRow.Visible = this.Get<YafBoardSettings>().AllowPostToBlog && isNewTopic
                                       && !this.PageContext.IsGuest;
               

                // update options...
                this.PostOptions1.Visible = !this.PageContext.IsGuest;
                this.PostOptions1.PersistantOptionVisible = this.PageContext.ForumPriorityAccess;
                this.PostOptions1.AttachOptionVisible = this.PageContext.ForumUploadAccess;
                this.PostOptions1.WatchOptionVisible = !this.PageContext.IsGuest;
                this.PostOptions1.PollOptionVisible = this.PageContext.ForumPollAccess && isNewTopic;
                this.PostOptions1.TopicImageAttachVisible = this.Get<YafBoardSettings>().AllowTopicImages;
                ////this.Attachments1.Visible = !this.PageContext.IsGuest;

                // get topic and forum information
                /*DataRow topicInfo = CommonDb.topic_info(YafContext.Current.PageModuleID,this.PageContext.PageTopicID);
                                using (DataTable dt = CommonDb.forum_list(YafContext.Current.PageModuleID,this.PageContext.PageBoardID, this.PageContext.PageForumID))
                                {
                                        DataRow forumInfo = dt.Rows[0];
                                }*/

                if (!this.PageContext.IsGuest)
                {
                    /*this.Attachments1.Visible = this.PageContext.ForumUploadAccess;
                                        this.Attachments1.Forum = forumInfo;
                                        this.Attachments1.Topic = topicInfo;
                                        // todo message id*/
                    this.PostOptions1.WatchChecked = this.PageContext.PageTopicID > 0
                                                         ? this.TopicWatchedId(
                                                             this.PageContext.PageUserID, this.PageContext.PageTopicID).HasValue
                                                         : new CombinedUserDataHelper(this.PageContext.PageUserID).AutoWatchTopics;
                }

                if ((this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableCaptchaForGuests)
                    || (this.Get<YafBoardSettings>().EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
                {
                    this.imgCaptcha.ImageUrl = "{0}resource.ashx?c=1".FormatWith(YafForumInfo.ForumClientFileRoot);
                    this.tr_captcha1.Visible = true;
                    this.tr_captcha2.Visible = true;
                }

                if (this.PageContext.Settings.LockedForum == 0)
                {
                    this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
                    this.PageLinks.AddLink(
                        this.PageContext.PageCategoryName,
                        YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
                }

                this.PageLinks.AddForumLinks(this.PageContext.PageForumID);

                // check if it's a reply to a topic...
                if (this.TopicID != null)
                {
                    this.InitReplyToTopic();

                    this.PollList.TopicId = this.TopicID.ToType<int>();
                }

                // If currentRow != null, we are quoting a post in a new reply, or editing an existing post
                if (currentMessage != null)
                {
                    this.OriginalMessage = currentMessage.Message;

                    if (this.QuotedMessageID != null)
                    {
                        if (this.Get<IYafSession>().MultiQuoteIds != null)
                        {
                            if (
                                !this.Get<IYafSession>().MultiQuoteIds.Contains(
                                    this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("q").ToType<int>()))
                            {
                                this.Get<IYafSession>().MultiQuoteIds.Add(
                                    this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("q").ToType<int>());
                            }

                            var messages = CommonDb.post_list(PageContext.PageModuleID, this.TopicID,
                                this.PageContext.PageUserID,
                                this.PageContext.PageUserID,
                                0,
                                false,
                                false,
                                false,
                                DateTimeHelper.SqlDbMinTime(),
                                DateTime.UtcNow,
                                DateTimeHelper.SqlDbMinTime(),
                                DateTime.UtcNow,
                                this.PageIndex.ToType<int>(),
                                this.PageContext.PostsPerPage,
                                1,
                                0,
                                0,
                                false,
                                0,
                                -1,
                                DateTimeHelper.SqlDbMinTime());

                            // quoting a reply to a topic...
                            foreach (var msg in
                                this.Get<IYafSession>().MultiQuoteIds.ToArray().Select(
                                    id =>
                                    messages.AsEnumerable().Select(t => new TypedMessageList(t)).Where(
                                        m => m.MessageID == id.ToType<int>())).SelectMany(
                                            quotedMessage => quotedMessage))
                            {
                                this.InitQuotedReply(msg);
                            }

                            // Clear Multiquotes
                            this.Get<IYafSession>().MultiQuoteIds = null;
                        }
                        else
                        {
                            this.InitQuotedReply(currentMessage);
                        }

                        this.PollList.TopicId = this.TopicID.ToType<int>();
                    }
                    else if (this.EditMessageID != null)
                    {
                        // editing a message...
                        this.InitEditedPost(currentMessage);
                        this.PollList.EditMessageId = this.EditMessageID.ToType<int>();
                    }

                    this.PollGroupId = currentMessage.PollID.ToType<int>().IsNullOrEmptyDBField()
                                           ? 0
                                           : currentMessage.PollID.ToType<int>();
                }

                // add the "New Topic" page link last...
                if (isNewTopic)
                {
                    this.PageLinks.AddLink(this.GetText("NEWTOPIC"));
                }

                // form user is only for "Guest"
                this.From.Text = this.GetText("COMMON", "GUEST_NAME");
                if (this.User != null)
                {
                    this.FromRow.Visible = false;
                }

                /*   if (this.TopicID == null)
                                {
                                        this.PollList.TopicId = (currentRow["TopicID"].IsNullOrEmptyDBField() ? 0 : Convert.ToInt32(currentRow["TopicID"]));
                                } */
            }

            this.PollList.PollGroupId = this.PollGroupId;
        }

        /// <summary>
        /// The post reply handle edit post.
        /// </summary>
        /// <returns>
        /// Returns the Message Id 
        /// </returns>
        protected long PostReplyHandleEditPost()
        {
            if (!this.PageContext.ForumEditAccess)
            {
                YafBuildLink.AccessDenied();
            }

            string subjectSave = string.Empty;
            string descriptionSave = string.Empty;
            string stylesSave = string.Empty;

            if (this.TopicSubjectTextBox.Enabled)
            {
                subjectSave = this.TopicSubjectTextBox.Text;
            }

            if (this.TopicDescriptionTextBox.Enabled)
            {
                descriptionSave = this.TopicDescriptionTextBox.Text;
            }

            if (this.TopicStylesTextBox.Enabled)
            {
                stylesSave = this.TopicStylesTextBox.Text;
            }

            // Mek Suggestion: This should be removed, resetting flags on edit is a bit lame.
            // Ederon : now it should be better, but all this code around forum/topic/message flags needs revamp
            // retrieve message flags
            var messageFlags = new MessageFlags(CommonDb.MessageList(PageContext.PageModuleID, (int)this.EditMessageID).First().Flags.BitValue)
                {
                    IsHtml = this._forumEditor.UsesHTML,
                    IsBBCode = this._forumEditor.UsesBBCode,
                    IsPersistent = this.PostOptions1.PersistantChecked
                };

            bool isModeratorChanged = this.PageContext.PageUserID != this._ownerUserId;

            string tags = this.Tags.Text.Trim();
            CommonDb.message_update(
                PageContext.PageModuleID,
                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"),
                this.Priority.SelectedValue,
                this._forumEditor.Text.Trim(),
                descriptionSave.Trim(),
                this.TopicStatus.SelectedValue.Equals("-1") || this.TopicStatus.SelectedIndex.Equals(0)
                    ? string.Empty
                    : this.TopicStatus.SelectedValue,
                stylesSave.Trim(),
                subjectSave.Trim(),
                messageFlags.BitValue,
                this.HtmlEncode(this.ReasonEditor.Text),
                isModeratorChanged,
                this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess,
                this.OriginalMessage,
                this.PageContext.PageUserID,
                this.MessageDescriptionTextBox.Text,
                tags);

            long messageId = this.EditMessageID.Value;

            this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            this.HandlePostToBlog(this._forumEditor.Text, this.TopicSubjectTextBox.Text);

            // remove cache if it exists...
            this.Get<IDataCache>().Remove(
                Constants.Cache.FirstPostCleaned.FormatWith(this.PageContext.PageBoardID, this.TopicID));

            return messageId;
        }

        /// <summary>
        /// The post reply handle new post.
        /// </summary>
        /// <param name="topicId">
        /// The topic Id. 
        /// </param>
        /// <returns>
        /// Returns the Message Id. 
        /// </returns>
        protected long PostReplyHandleNewPost(out long topicId)
        {
            long messageId = 0;

            if (!this.PageContext.ForumPostAccess)
            {
                YafBuildLink.AccessDenied();
            }

            // Check if Forum is Moderated
            DataRow forumInfo;
            bool isForumModerated = false;

            using (DataTable dt = CommonDb.forum_list(PageContext.PageModuleID, this.PageContext.PageBoardID, this.PageContext.PageForumID))
            {
                forumInfo = dt.Rows[0];
            }

            if (forumInfo != null)
            {
                isForumModerated = forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsModerated);
            }

            // If Forum is Moderated
            if (isForumModerated)
            {
                this.spamApproved = false;
            }

            // Bypass Approval if Admin or Moderator
            if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
            {
                this.spamApproved = true;
            }

            // make message flags
            var messageFlags = new MessageFlags
                {
                    IsHtml = this._forumEditor.UsesHTML,
                    IsBBCode = this._forumEditor.UsesBBCode,
                    IsPersistent = this.PostOptions1.PersistantChecked,
                    IsApproved = this.spamApproved
                };

            string blogPostID = this.HandlePostToBlog(this._forumEditor.Text, this.TopicSubjectTextBox.Text);                    

            // Save to Db
            topicId = CommonDb.topic_save(
                PageContext.PageModuleID,
                this.PageContext.PageForumID,
                this.TopicSubjectTextBox.Text.RemoveDoubleWhiteSpaces().Truncate(250),
                this.TopicStatus.SelectedValue.Equals("-1") || this.TopicStatus.SelectedIndex.Equals(0) ? string.Empty : this.TopicStatus.SelectedValue,
                this.TopicStylesTextBox.Text.Trim(),
                this.TopicDescriptionTextBox.Text.RemoveDoubleWhiteSpaces().Truncate(250),
                this._forumEditor.Text,
                this.PageContext.PageUserID,
                this.Priority.SelectedValue,
                this.User != null ? null : (UserMembershipHelper.FindUsersByName(this.From.Text.Trim()).Count > 0) ? "{0}_{1}".FormatWith(this.GetText("COMMON", "GUEST_NAME"), this.From.Text.Trim()) : this.From.Text.Trim(),
                this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                DateTime.UtcNow,
                blogPostID,
                messageFlags.BitValue,
                this.MessageDescriptionTextBox.Text.RemoveDoubleWhiteSpaces().Truncate(250),
                ref messageId,
                this.Tags.Text.RemoveDoubleWhiteSpaces());

            this.UpdateWatchTopic(this.PageContext.PageUserID, (int)topicId);

            // clear caches as stats changed
            if (messageFlags.IsApproved)
            {
                this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);
                this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
            }

            return messageId;
        }

        /// <summary>
        /// The post reply handle reply to topic.
        /// </summary>
        /// <param name="isSpamApproved">
        /// The is Spam Approved. 
        /// </param>
        /// <returns>
        /// Returns the Message Id. 
        /// </returns>
        protected long PostReplyHandleReplyToTopic(bool isSpamApproved)
        {
            long messageId = 0;

            if (!this.PageContext.ForumReplyAccess)
            {
                YafBuildLink.AccessDenied();
            }

            // Check if Forum is Moderated
            DataRow forumInfo;
            bool isForumModerated = false;

            using (DataTable dt = CommonDb.forum_list(PageContext.PageModuleID, this.PageContext.PageBoardID, this.PageContext.PageForumID))
            {
                forumInfo = dt.Rows[0];
            }

            if (forumInfo != null)
            {
                isForumModerated = forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsModerated);
            }

            // If Forum is Moderated
            if (isForumModerated)
            {
                isSpamApproved = false;
            }

            // Bypass Approval if Admin or Moderator
            if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
            {
                isSpamApproved = true;
            }

            object replyTo = (this.QuotedMessageID != null) ? this.QuotedMessageID.Value : -1;

            // make message flags
            var messageFlags = new MessageFlags
                {
                    IsHtml = this._forumEditor.UsesHTML,
                    IsBBCode = this._forumEditor.UsesBBCode,
                    IsPersistent = this.PostOptions1.PersistantChecked,
                    IsApproved = isSpamApproved
                };

            CommonDb.message_save(PageContext.PageModuleID, this.TopicID.Value,
                this.PageContext.PageUserID,
                this._forumEditor.Text,
                this.User != null ? null : (UserMembershipHelper.FindUsersByName(this.From.Text.Trim()).Count > 0) ? "{0}_{1}".FormatWith(this.GetText("COMMON", "GUEST_NAME"), this.From.Text.Trim()) : this.From.Text.Trim(),
                this.Get<HttpRequestBase>().GetUserRealIPAddress(), 
                DateTime.UtcNow,
                replyTo,
                messageFlags.BitValue,
                this.MessageDescriptionTextBox.Text.IsSet() ? this.MessageDescriptionTextBox.Text : null,
                ref messageId);

            this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            if (messageFlags.IsApproved)
            {
                this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);
                this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
            }

            return messageId;
        }

        /// <summary>
        /// Handles the PostReply click including: Replying, Editing and New post.
        /// </summary>
        /// <param name="sender">
        /// The Sender Object. 
        /// </param>
        /// <param name="e">
        /// The Event Arguments. 
        /// </param>
        protected void PostReply_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostReplyVerified())
            {
                return;
            }

            if (this.IsPostReplyDelay())
            {
                return;
            }

            // Check for SPAM
            if (!this.PageContext.IsAdmin || !this.PageContext.ForumModeratorAccess)
            {
                if (YafSpamCheck.IsPostSpam(
                    this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                    this.TopicSubjectTextBox.Text,
                    this._forumEditor.Text))
                {
                    if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(1))
                    {
                        this.spamApproved = false;
                    }
                    else if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(2))
                    {
                        this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"));
                        return;
                    }
                }
            }

            bool tagCountIsAllowed = false;
            bool forbiddenSymbols = false;

            if (!this.CheckTagLength(this.Tags.Text.Trim(), this.Get<YafBoardSettings>().TagMaxLength, this.Get<YafBoardSettings>().TagTopicMaxCount, this.Get<YafBoardSettings>().TagForbiddenSymbols, out tagCountIsAllowed, out forbiddenSymbols))
            {
                this.PageContext.AddLoadMessage(this.GetTextFormatted("TAG_TOOLONG", this.Get<YafBoardSettings>().TagMaxLength));
                return;
            }

            if (!tagCountIsAllowed)
            {
                this.PageContext.AddLoadMessage(this.GetTextFormatted("TAG_TOOMANY", this.Get<YafBoardSettings>().TagTopicMaxCount));
                return;
            }

            if (forbiddenSymbols)
            {
                this.PageContext.AddLoadMessage(this.GetTextFormatted("TAG_FORBIDDENSYMBOLS", this.Get<YafBoardSettings>().TagForbiddenSymbols));
                return;
            }

            // vzrus: automatically strip html tags from Topic Titles and Description
            // this.TopicSubjectTextBox.Text = HtmlHelper.StripHtml(this.TopicSubjectTextBox.Text);
            // this.TopicDescriptionTextBox.Text = HtmlHelper.StripHtml(this.TopicDescriptionTextBox.Text);

            // update the last post time...
            this.Get<IYafSession>().LastPost = DateTime.UtcNow.AddSeconds(30);

            long messageId;
            long newTopic = 0;

            if (this.TopicID != null)
            {
                // Reply to topic
                messageId = this.PostReplyHandleReplyToTopic(this.spamApproved);
                newTopic = this.TopicID.ToType<long>();
            }
            else if (this.EditMessageID != null)
            {
                // Edit existing post
                messageId = this.PostReplyHandleEditPost();
            }
            else
            {
                // New post
                messageId = this.PostReplyHandleNewPost(out newTopic);
            }

            // Check if message is approved
            bool isApproved = false;
            var dt = CommonDb.MessageList(PageContext.PageModuleID, (int)messageId);
            var typedMessageLists = dt as IList<TypedMessageList> ?? dt.ToList();
            if (typedMessageLists.Any())
            {
                isApproved = typedMessageLists.First().Flags.IsApproved;
            }

            // vzrus^ the poll access controls are enabled and this is a new topic - we add the variables
            string attachp = string.Empty;
            string retforum = string.Empty;

            if (this.PageContext.ForumPollAccess && this.PostOptions1.PollOptionVisible && newTopic > 0)
            {
                // new topic poll token
                attachp = "&t={0}".FormatWith(newTopic);

                // new return forum poll token
                retforum = "&f={0}".FormatWith(this.PageContext.PageForumID);
            }

            if (this.PostOptions1.TopicImageChecked && this.Get<YafBoardSettings>().AllowTopicImages)
            {
                attachp += "&ti=1".FormatWith(messageId);
            }

            // Create notification emails
            if (isApproved)
            {
                this.Get<ISendNotification>().ToWatchingUsers(messageId.ToType<int>());

                if (this.PageContext.ForumUploadAccess && this.PostOptions1.AttachChecked)
                {
                    // 't' variable is required only for poll and this is a attach poll token for attachments page
                    if (!this.PostOptions1.PollChecked)
                    {
                        attachp = string.Empty;
                    }

                    // redirect to the attachment page...
                    YafBuildLink.Redirect(ForumPages.attachments, "m={0}{1}", messageId, attachp);
                }
                else
                {
                    if (attachp.IsNotSet() || (!this.PostOptions1.PollChecked)) 
                    {
                        // regular redirect...
                        // YafBuildLink.Redirect(ForumPages.posts, "m={0}#post{0}", messageId);
                        YafBuildLink.Redirect(ForumPages.posts, "m={0}#post{0}", messageId);
                    }
                    else
                    {
                        // poll edit redirect...
                        YafBuildLink.Redirect(ForumPages.polledit, "{0}", attachp);
                    }
                }
            }
            else
            {
                // Not Approved
                if (this.Get<YafBoardSettings>().EmailModeratorsOnModeratedPost)
                {
                    // not approved, notify moderators
                    this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApproval(
                        this.PageContext.PageForumID, (int)messageId);
                }

                // 't' variable is required only for poll and this is a attach poll token for attachments page
                if (!this.PostOptions1.PollChecked)
                {
                    attachp = string.Empty;
                }
              
                if (this.PostOptions1.AttachChecked && this.PageContext.ForumUploadAccess)
                {
                    // redirect to the attachment page...
                    YafBuildLink.Redirect(ForumPages.attachments, "m={0}&ra=1{1}{2}", messageId, attachp, retforum);
                }
                else
                {                  
                    // redirect to the image page...
                    // YafBuildLink.Redirect(ForumPages.imageadd, "m={0}&ra=1{1}{2}", messageId, attachp, retforum);

                    // Tell user that his message will have to be approved by a moderator
                    string url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.PageContext.PageForumID);

                    if (this.PageContext.PageTopicID > 0)
                    {
                        url = YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID);
                    }

                    if (attachp.Length <= 0)
                    {
                        YafBuildLink.Redirect(ForumPages.info, "i=1&url={0}", this.Server.UrlEncode(url));
                    }
                    else
                    {
                        YafBuildLink.Redirect(ForumPages.polledit, "&ra=1{0}{1}", attachp, retforum);
                    }

                    if (Config.IsRainbow)
                    {
                        YafBuildLink.Redirect(ForumPages.info, "i=1");
                    }
                }
            }
        }

        /// <summary>
        /// Previews the new Message
        /// </summary>
        /// <param name="sender">
        /// The Sender Object. 
        /// </param>
        /// <param name="e">
        /// The Event Arguments. 
        /// </param>
        protected void Preview_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PreviewRow.Visible = true;

            this.PreviewMessagePost.MessageFlags.IsHtml = this._forumEditor.UsesHTML;
            this.PreviewMessagePost.MessageFlags.IsBBCode = this._forumEditor.UsesBBCode;
            this.PreviewMessagePost.Message = this._forumEditor.Text;

            if (!this.Get<YafBoardSettings>().AllowSignatures)
            {
                return;
            }

            string userSig = CommonDb.user_getsignature(PageContext.PageModuleID, this.PageContext.PageUserID);

            if (userSig.IsSet())
            {
                this.PreviewMessagePost.Signature = userSig;
            }
        }

        /// <summary>
        /// The can edit post check.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="topicInfo">
        /// The topic Info. 
        /// </param>
        /// <returns>
        /// Returns if user can edit post check. 
        /// </returns>
        private bool CanEditPostCheck([NotNull] TypedMessageList message, DataRow topicInfo)
        {
            bool postLocked = false;

            if (!this.PageContext.IsAdmin && this.Get<YafBoardSettings>().LockPosts > 0)
            {
                var edited = message.Edited.ToType<DateTime>();

                if (edited.AddDays(this.Get<YafBoardSettings>().LockPosts) < DateTime.UtcNow)
                {
                    postLocked = true;
                }
            }

            DataRow forumInfo;

            // get  forum information
            using (DataTable dt = CommonDb.forum_list(PageContext.PageModuleID, this.PageContext.PageBoardID, this.PageContext.PageForumID))
            {
                forumInfo = dt.Rows[0];
            }

            // Ederon : 9/9/2007 - moderator can edit in locked topics
            return ((!postLocked && !forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsLocked)
                     && !topicInfo["Flags"].BinaryAnd(TopicFlags.Flags.IsLocked)
                     && (message.UserID.ToType<int>() == this.PageContext.PageUserID))
                    || this.PageContext.ForumModeratorAccess) && this.PageContext.ForumEditAccess;
        }

        /// <summary>
        /// The can have poll.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <returns>
        /// The can have poll. 
        /// </returns>
        private bool CanHavePoll([NotNull] DataRow message)
        {
            return (this.TopicID == null && this.QuotedMessageID == null && this.EditMessageID == null)
                   || (message != null && message["Position"].ToType<int>() == 0);
        }

        /// <summary>
        /// Determines whether this instance [can quote post check] the specified topic info.
        /// </summary>
        /// <param name="topicInfo">
        /// The topic info. 
        /// </param>
        /// <returns>
        /// The can quote post check. 
        /// </returns>
        private bool CanQuotePostCheck(DataRow topicInfo)
        {
            DataRow forumInfo;

            // get topic and forum information
            using (DataTable dt = CommonDb.forum_list(PageContext.PageModuleID, this.PageContext.PageBoardID, this.PageContext.PageForumID))
            {
                forumInfo = dt.Rows[0];
            }

            if (topicInfo == null || forumInfo == null)
            {
                return false;
            }

            // Ederon : 9/9/2007 - moderator can reply to locked topics
            return (!forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsLocked)
                    && !topicInfo["Flags"].BinaryAnd(TopicFlags.Flags.IsLocked) || this.PageContext.ForumModeratorAccess)
                   && this.PageContext.ForumReplyAccess;
        }

        /// <summary>
        /// The get poll id.
        /// </summary>
        /// <returns>
        /// The get poll id. 
        /// </returns>
        [CanBeNull]
        private object GetPollID()
        {
            return null;
        }

        /// <summary>
        /// The has poll.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <returns>
        /// The has poll. 
        /// </returns>
        private bool HasPoll([NotNull] DataRow message)
        {
            return message != null && message["PollID"] != DBNull.Value && message["PollID"] != null;
        }

        /// <summary>
        /// The init edited post.
        /// </summary>
        /// <param name="currentMessage">
        /// The current message. 
        /// </param>
        private void InitEditedPost([NotNull] TypedMessageList currentMessage)
        {
            if (this._forumEditor.UsesHTML && currentMessage.Flags.IsBBCode)
            {
                // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
                currentMessage.Message = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(currentMessage.Message);
            }

            if (this._forumEditor.UsesBBCode && currentMessage.Flags.IsHtml)
            {
                // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
                currentMessage.Message = this.Get<IBBCode>().ConvertHtmltoBBCodeForEdit(currentMessage.Message);
            }

            this._forumEditor.Text = currentMessage.Message;

            this.Title.Text = this.GetText("EDIT");
            this.PostReply.TextLocalizedTag = "SAVE";
            this.PostReply.TextLocalizedPage = "COMMON";

            // add topic link...
            this.PageLinks.AddLink(
                this.Server.HtmlDecode(currentMessage.Topic),
                YafBuildLink.GetLink(ForumPages.posts, "m={0}", this.EditMessageID));

            // editing..
            this.PageLinks.AddLink(this.GetText("EDIT"));

            string blogPostId = currentMessage.BlogPostID;
            if (blogPostId != string.Empty)
            {
                // The user used this post to blog
                this.BlogPostID.Value = blogPostId;
                this.PostToBlog.Checked = true;

                // this.BlogRow.Visible = true;
            }

            this.TopicSubjectTextBox.Text = this.Server.HtmlDecode(currentMessage.Topic);
            this.TopicDescriptionTextBox.Text = this.Server.HtmlDecode(currentMessage.Description);

            if ((currentMessage.TopicOwnerID.ToType<int>() == currentMessage.UserID.ToType<int>())
                || this.PageContext.ForumModeratorAccess)
            {
                // allow editing of the topic subject
                this.TopicSubjectTextBox.Enabled = true;
                if (this.Get<YafBoardSettings>().EnableTopicDescription)
                {
                    this.DescriptionRow.Visible = true;
                }

                if (this.Get<YafBoardSettings>().EnableTopicStatus)
                {
                    this.StatusRow.Visible = true;
                }

                this.TopicStatus.Enabled = true;
            }
            else
            {
                this.TopicSubjectTextBox.Enabled = false;
                this.TopicDescriptionTextBox.Enabled = false;
                this.TopicStatus.Enabled = false;
            }

            // Allow the Styling of Topic Titles only for Mods or Admins
            if (this.Get<YafBoardSettings>().UseStyledTopicTitles
                && (this.PageContext.ForumModeratorAccess || this.PageContext.IsAdmin))
            {
                this.StyleRow.Visible = true;
            }
            else
            {
                this.StyleRow.Visible = false;
                this.TopicStylesTextBox.Enabled = false;
            }

            this.TopicStylesTextBox.Text = currentMessage.Styles;

            this.Priority.SelectedItem.Selected = false;
            this.Priority.Items.FindByValue(currentMessage.Priority.ToString()).Selected = true;

            if (this.TopicStatus.SelectedItem != null)
            {
                this.TopicStatus.SelectedItem.Selected = false;
            }

            if (this.TopicStatus.Items.FindByValue(currentMessage.Status) != null)
            {
                this.TopicStatus.Items.FindByValue(currentMessage.Status).Selected = true;
            }

            this.EditReasonRow.Visible = true;
            this.ReasonEditor.Text = this.Server.HtmlDecode(currentMessage.EditReason);
            this.PostOptions1.PersistantChecked = currentMessage.Flags.IsPersistent;

            // this.Attachments1.MessageID = (int)this.EditMessageID;
        }

        /// <summary>
        /// The initialize quoted reply.
        /// </summary>
        /// <param name="message">
        /// The current TypedMessage. 
        /// </param>
        private void InitQuotedReply(TypedMessageList message)
        {
            var messageContent = message.Message;

            if (this.Get<YafBoardSettings>().RemoveNestedQuotes)
            {
                messageContent = this.Get<IFormatMessage>().RemoveNestedQuotes(messageContent);
            }

            if (this._forumEditor.UsesHTML && message.Flags.IsBBCode)
            {
                // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
                messageContent = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(messageContent);
            }

            if (this._forumEditor.UsesBBCode && message.Flags.IsHtml)
            {
                // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
                messageContent = this.Get<IBBCode>().ConvertHtmltoBBCodeForEdit(messageContent);
            }

            // Ensure quoted replies have bad words removed from them
            messageContent = this.Get<IBadWordReplace>().Replace(messageContent);

            // Remove HIDDEN Text
            messageContent = this.Get<IFormatMessage>().RemoveHiddenBBCodeContent(messageContent);

            // Quote the original message
            this._forumEditor.Text +=
                "[quote={0};{1}]{2}[/quote]\n\n".FormatWith(
                    this.Get<IUserDisplayName>().GetName(message.UserID.ToType<int>()),
                    message.MessageID,
                    messageContent).TrimStart();
        }

        /// <summary>
        /// The init reply to topic.
        /// </summary>
        private void InitReplyToTopic()
        {
            DataRow topic = CommonDb.topic_info(this.PageContext.PageModuleID, this.TopicID, true, false);
            var topicFlags = new TopicFlags(topic["Flags"].ToType<int>());

            // Ederon : 9/9/2007 - moderators can reply in locked topics
            if (topicFlags.IsLocked && !this.PageContext.ForumModeratorAccess)
            {
                // YafBuildLink.Redirect(ForumPages.topics);
                this.Response.Redirect(this.Get<HttpRequestBase>().UrlReferrer.ToString());
            }

            this.PriorityRow.Visible = false;
            this.SubjectRow.Visible = false;
            this.DescriptionRow.Visible = false;
            this.StatusRow.Visible = false;
            this.StyleRow.Visible = false;
            this.Title.Text = this.GetText("reply");

            // add topic link...
            this.PageLinks.AddLink(
                this.Server.HtmlDecode(topic["Topic"].ToString()),
                YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.TopicID));

            // add "reply" text...
            this.PageLinks.AddLink(this.GetText("reply"));

            // show attach file option if its a reply...
            if (this.PageContext.ForumUploadAccess)
            {
                this.PostOptions1.Visible = true;
                this.PostOptions1.AttachOptionVisible = true;

                // this.Attachments1.Visible = true;
            }

            this.PostOptions1.TopicImageAttachVisible = false;

            // show the last posts AJAX frame...
            this.LastPosts1.Visible = true;
            this.LastPosts1.TopicID = this.TopicID.Value;
        }

        /// <summary>
        /// Returns true if the topic is set to watch for userId
        /// </summary>
        /// <param name="userId">
        /// The user Id. 
        /// </param>
        /// <param name="topicId">
        /// The topic Id. 
        /// </param>
        /// <returns>
        /// The topic watched id. 
        /// </returns>
        private int? TopicWatchedId(int userId, int topicId)
        {
            return CommonDb.watchtopic_check(PageContext.PageModuleID, userId, topicId).GetFirstRowColumnAsValue<int?>("WatchTopicID", null);
        }

        /// <summary>
        /// Updates Watch Topic based on controls/settings for user...
        /// </summary>
        /// <param name="userId">
        /// The user Id. 
        /// </param>
        /// <param name="topicId">
        /// The topic Id. 
        /// </param>
        private void UpdateWatchTopic(int userId, int topicId)
        {
            var topicWatchedID = this.TopicWatchedId(userId, topicId);

            if (topicWatchedID.HasValue && !this.PostOptions1.WatchChecked)
            {
                // unsubscribe...
                CommonDb.watchtopic_delete(PageContext.PageModuleID, topicWatchedID.Value);
            }
            else if (!topicWatchedID.HasValue && this.PostOptions1.WatchChecked)
            {
                // subscribe to this topic...
                this.WatchTopic(userId, topicId);
            }
        }

        /// <summary>
        /// Checks if this topic is watched, if not, adds it.
        /// </summary>
        /// <param name="userId">
        /// The user Id. 
        /// </param>
        /// <param name="topicId">
        /// The topic Id. 
        /// </param>
        private void WatchTopic(int userId, int topicId)
        {
            if (!this.TopicWatchedId(userId, topicId).HasValue)
            {
                // subscribe to this forum
                CommonDb.watchtopic_add(PageContext.PageModuleID, userId, topicId);
            }
        }

        /// <summary>
        /// Tag Length checker.
        /// </summary>
        /// <param name="tags">
        /// </param>
        /// <param name="tagLength">
        /// </param>
        /// <param name="tagCount">
        /// The tag Count.
        /// </param>
        /// <param name="symbols">
        /// The symbols.
        /// </param>
        /// <param name="tagCountIsAllowed">
        /// The tag Count Is Allowed.
        /// </param>
        /// <param name="forbiddenSymbols">
        /// The forbidden Symbols.
        /// </param>
        /// <returns>
        /// </returns>
        private bool CheckTagLength(string tags, int tagLength, int tagCount, string symbols, out bool tagCountIsAllowed, out bool forbiddenSymbols)
        {
            forbiddenSymbols = false;
            tagCountIsAllowed = true;
            if (tags.IsSet() && symbols.Any(tags.Replace(",",string.Empty).Contains))
            {
                forbiddenSymbols = true;
                return true;
            }

            string[] tagsArr = tags.Split(',');
            int i = 0;
            foreach (var s in tagsArr)
            {
                if (s.Length > tagLength)
                {
                    return false;
                }

                if (i > tagCount)
                {
                    tagCountIsAllowed = false;
                    return true;
                }

                i++;
            }

            return true;
        }

        #endregion
    }
}