namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using VZF.Data.Common;
    using VZF.Types.Data;
    using VZF.Types.Objects;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using VZF.Utilities;
    using VZF.Utils;
    using VZF.Utils.Helpers;
    using YAF.Core.Tasks;

    #endregion

    /// <summary>
    /// The control generates test data for different data layers.
    /// </summary>
    public partial class test_data : AdminPage
    {
        // private string regBase = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";

        #region Constants and Fields

        /// <summary>
        ///   The board create limit.
        /// </summary>
        private const int BoardCreateLimit = 100;

        /// <summary>
        ///   The category create limit.
        /// </summary>
        private const int categoryCreateLimit = 100;

        /// <summary>
        ///   The create common limit.
        /// </summary>
        private const int createCommonLimit = 9999;

        /// <summary>
        ///   The pmessage prefix.
        /// </summary>
        private const string pmessagePrefix = "pmsg-";

        /// <summary>
        ///   The random guid.
        /// </summary>
        private string randomGuid = Guid.NewGuid().ToString();

        #endregion

        #region Methods

        /// <summary>
        /// The cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_test_data);
        }

        /// <summary>
        /// The categories boards options_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void CategoriesBoardsOptions_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.CategoriesBoardsList.Visible = this.CategoriesBoardsOptions.SelectedIndex == 3;
        }

        /// <summary>
        /// The create test data_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void CreateTestData_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Page.IsValid)
            {
                return;
            }

            if (!this.ValidateControlsValues())
            {
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Test Data Generator reports: ");
            sb.AppendLine("Created:");
            sb.Append(this.CreateUsers());
            sb.Append(this.CreateBoards());
            sb.Append(this.CreateCategories());
            sb.Append("; ");
            sb.AppendFormat("{0} Forums, ", this.CreateForums());
            sb.AppendFormat("{0} Topics, ", this.CreateTopics(0, 0, 0));
            sb.AppendFormat("{0} Messages, ", this.CreatePosts(0, 0, 0));
            sb.AppendFormat("{0} Private Messages, ", this.CreatePMessages());

            string mesRetStr = sb.ToString();

            CommonDb.eventlog_create(PageContext.PageModuleID, this.PageContext.PageUserID, this.GetType().ToString(), mesRetStr, EventLogTypes.Information);

            this.PageContext.AddLoadMessage(mesRetStr);
            YafBuildLink.Redirect(ForumPages.admin_test_data);
        }

        /// <summary>
        /// The forums category_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ForumsCategory_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            DataTable forumsCategory = CommonDb.forum_listall_fromCat(PageContext.PageModuleID, this.PageContext.PageBoardID, this.ForumsCategory.SelectedValue.ToType<int>(), false);
            this.ForumsParent.DataSource = forumsCategory;
            this.ForumsParent.DataBind();
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and Jquery Ui Tabs.
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJQueryUI();

            YafContext.Current.PageElements.RegisterJsBlock(
                "TestDataTabsJs",
                JavaScriptBlocks.JqueryUITabsLoadJs(
                    this.TestDataTabs.ClientID,
                    this.hidLastTab.ClientID,
                    this.hidLastTabId.ClientID,
                    false));

            base.OnPreRender(e);
        }

        /// <summary>
        /// The p messages boards options_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PMessagesBoardsOptions_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PMessagesBoardsList.Visible = this.PMessagesBoardsOptions.SelectedIndex == 3;
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_TEST_DATA", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_TEST_DATA", "TITLE"));

            this.Populate_Controls();

            const string BoardOptionsCurrentBoardIn = "In Current Board";
            const string BoardOptionsAllBoardsIn = "In All Boards";
            const string BoardOptionsAllBoardsButCurrentIn = "In All But Current";
            const string BoardOptionsAllBoardsSpecificIn = "In A Specific Board";

            this.TimeZones.DataSource = StaticDataHelper.TimeZones();

            DataTable categories = CommonDb.category_list(PageContext.PageModuleID, this.PageContext.PageBoardID, null);

            this.ForumsCategory.DataSource = categories;
            this.TopicsCategory.DataSource = categories;
            this.PostsCategory.DataSource = categories;

            // Access Mask Lists               
            this.ForumsStartMask.DataSource = CommonDb.accessmask_aforumlist(
                   mid: this.PageContext.PageModuleID,
                   boardId: this.PageContext.PageBoardID,
                   accessMaskId: null,
                   excludeFlags: 0,
                   pageUserId: null,               
                   isAdminMask: true,
                   isCommonMask: true);
            this.ForumsAdminMask.DataSource = this.ForumsStartMask.DataSource;

            this.ForumsGroups.DataSource = CommonDb.group_list(this.PageContext.PageModuleID, this.PageContext.PageBoardID, null, 0, 1000000);

            // Board lists
            this.UsersBoardsList.DataSource = CommonDb.board_list(PageContext.PageModuleID, null);
            this.CategoriesBoardsList.DataSource = this.UsersBoardsList.DataSource;
            this.PMessagesBoardsList.DataSource = this.UsersBoardsList.DataSource;

            this.DataBind();

            if (this.ForumsAdminMask.Items.Count > 0)
            {
                this.ForumsAdminMask.SelectedIndex = this.ForumsAdminMask.Items.Count - 1;
            }

            if (this.ForumsStartMask.Items.Count > 1)
            {
                this.ForumsStartMask.SelectedIndex = 1;
            }

            this.TopicsCategory.ClearSelection();
            this.PostsCategory.ClearSelection();

            this.ForumsCategory.SelectedIndex = -1;

            this.TimeZones.Items.FindByValue("0").Selected = true;

            this.From.Text = this.PageContext.User.UserName;
            this.To.Text = this.PageContext.User.UserName;

            this.TopicsPriorityList.Items.Add(new ListItem("Normal", "0"));
            this.TopicsPriorityList.Items.Add(new ListItem("Sticky", "1"));
            this.TopicsPriorityList.Items.Add(new ListItem("Announcement", "2"));

            this.TopicsPriorityList.SelectedIndex = 0;

            this.UsersBoardsOptions.Items.Add(new ListItem(BoardOptionsCurrentBoardIn, "0"));
            this.UsersBoardsOptions.Items.Add(new ListItem(BoardOptionsAllBoardsIn, "1"));
            this.UsersBoardsOptions.Items.Add(new ListItem(BoardOptionsAllBoardsButCurrentIn, "2"));
            this.UsersBoardsOptions.Items.Add(new ListItem(BoardOptionsAllBoardsSpecificIn, "3"));

            this.UsersBoardsOptions.SelectedIndex = 0;

            this.CategoriesBoardsOptions.Items.Add(new ListItem(BoardOptionsCurrentBoardIn, "0"));
            this.CategoriesBoardsOptions.Items.Add(new ListItem(BoardOptionsAllBoardsIn, "1"));
            this.CategoriesBoardsOptions.Items.Add(new ListItem(BoardOptionsAllBoardsButCurrentIn, "2"));
            this.CategoriesBoardsOptions.Items.Add(new ListItem(BoardOptionsAllBoardsSpecificIn, "3"));

            this.CategoriesBoardsOptions.SelectedIndex = 0;

            this.PMessagesBoardsOptions.Items.Add(new ListItem(BoardOptionsCurrentBoardIn, "0"));
            this.PMessagesBoardsOptions.Items.Add(new ListItem(BoardOptionsAllBoardsIn, "1"));
            this.PMessagesBoardsOptions.Items.Add(new ListItem(BoardOptionsAllBoardsButCurrentIn, "2"));
            this.PMessagesBoardsOptions.Items.Add(new ListItem(BoardOptionsAllBoardsSpecificIn, "3"));

            this.PMessagesBoardsOptions.SelectedIndex = 0;
        }

        /// <summary>
        /// The posts category_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PostsCategory_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            DataTable posts_category = CommonDb.forum_listall_fromCat(PageContext.PageModuleID, this.PageContext.PageBoardID, this.PostsCategory.SelectedValue.ToType<int>(),false);
            this.PostsForum.DataSource = posts_category;
            this.PostsForum.DataBind();
        }

        /// <summary>
        /// The posts forum_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PostsForum_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            int _forumID;

            if (!int.TryParse(this.PostsForum.SelectedValue, out _forumID))
            {
                return;
            }

            DataTable topics = CommonDb.topic_list(this.PageContext.PageModuleID, this.PostsForum.SelectedValue.ToType<int>(),
                this.PageContext.PageUserID,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                0,
                100,
                false,
                false, false,
                false,
                false);

            this.PostsTopic.DataSource = topics;
            this.PostsTopic.DataBind();
        }

        /// <summary>
        /// The topics category_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void TopicsCategory_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            DataTable topic_forums = CommonDb.forum_listall_fromCat(PageContext.PageModuleID, this.PageContext.PageBoardID, this.TopicsCategory.SelectedValue.ToType<int>(),false);
            this.TopicsForum.DataSource = topic_forums;
            this.TopicsForum.DataBind();
        }

        /// <summary>
        /// The users boards options_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void UsersBoardsOptions_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.UsersBoardsList.Visible = this.UsersBoardsOptions.SelectedIndex == 3;
        }

        /// <summary>
        /// The create boards.
        /// </summary>
        /// <returns>
        /// The number of created boards.
        /// </returns>
        private string CreateBoards()
        {
            int boardNumber;
            int usersNumber;
            if (!int.TryParse(this.BoardNumber.Text.Trim(), out boardNumber))
            {
                return null;
            }

            if (boardNumber <= 0)
            {
                return null;
            }

            if (!int.TryParse(this.BoardsUsersNumber.Text.Trim(), out usersNumber))
            {
                return null;
            }

            if (usersNumber < 0)
            {
                return null;
            }

            if (boardNumber > BoardCreateLimit)
            {
                boardNumber = BoardCreateLimit;
            }

            this.BoardMembershipName.Text = null;
            this.BoardRolesName.Text = null;
            int i;
            for (i = 0; i < boardNumber; i++)
            {
                string boardName = this.BoardPrefixTB.Text.Trim() + Guid.NewGuid();
                int curboard = CommonDb.board_create(
                    PageContext.PageModuleID, 
                    this.PageContext.User.UserName,
                    this.PageContext.User.Email,
                    this.PageContext.User.ProviderUserKey,
                    boardName,
                    "en-US",
                    "english.xml",
                    this.BoardMembershipName.Text.Trim(),
                    this.BoardRolesName.Text.Trim(),
                    Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty,
                    this.PageContext.IsHostAdmin);

                this.CreateUsers(curboard, usersNumber);
            }

            return i + " Boards, " + usersNumber + " Users in each Board; ";
        }

        /// <summary>
        /// Create categories from boards
        /// </summary>
        /// <param name="boardID">
        /// The boardID 
        /// </param>
        /// <returns>
        /// The create categories.
        /// </returns>
        private string CreateCategories(int boardID)
        {
            const string noCategories = "0 categories";

            const bool excludeCurrentBoardB = false;

            const bool useListB = false;

            int numCategoriesInt = 0;
            if (!int.TryParse(this.BoardsCategoriesNumber.Text.Trim(), out numCategoriesInt))
            {
                return noCategories;
            }

            if (numCategoriesInt < 0)
            {
                return noCategories;
            }

            if (numCategoriesInt > categoryCreateLimit)
            {
                numCategoriesInt = categoryCreateLimit;
            }

            int numForums;
            if (!int.TryParse(this.BoardsForumsNumber.Text.Trim(), out numForums))
            {
                return noCategories;
            }

            if (numForums < 0)
            {
                return noCategories;
            }

            int _numTopics;
            if (!int.TryParse(this.BoardsTopicsNumber.Text.Trim(), out _numTopics))
            {
                return noCategories;
            }

            if (_numTopics < 0)
            {
                return noCategories;
            }

            int numMessages;
            if (!int.TryParse(this.BoardsMessagesNumber.Text.Trim(), out numMessages))
            {
                return noCategories;
            }

            if (numMessages < 0)
            {
                return noCategories;
            }

            return this.CreateCategoriesBase(
                boardID, 1, numForums, _numTopics, numMessages, numCategoriesInt, excludeCurrentBoardB, useListB);
        }

        /// <summary>
        /// Create categories from Categories
        /// </summary>
        /// <returns>
        /// The create categories.
        /// </returns>
        private string CreateCategories()
        {
            const string NoCategories = "0 categories";
            int boardID = 0;

            // int categoriesLimit = 1;
            bool excludeCurrentBoard = false;

            int numForums;
            if (!int.TryParse(this.CategoriesForumsNumber.Text.Trim(), out numForums))
            {
                return NoCategories;
            }

            if (numForums < 0)
            {
                return NoCategories;
            }

            int numTopics;
            if (!int.TryParse(this.CategoriesTopicsNumber.Text.Trim(), out numTopics))
            {
                return "0 Categories";
            }

            if (numTopics < 0)
            {
                return NoCategories;
            }

            int numMessages;
            if (!int.TryParse(this.CategoriesMessagesNumber.Text.Trim(), out numMessages))
            {
                return NoCategories;
            }

            if (numMessages < 0)
            {
                return NoCategories;
            }

            int numCategories;
            int boardCount = 1;
            bool useList = false;
            switch (this.CategoriesBoardsOptions.SelectedIndex)
            {
                case 0:
                    boardID = YafContext.Current.PageBoardID;
                    break;
                case 1:
                    boardCount = this.CategoriesBoardsList.Items.Count;
                    useList = true;
                    break;
                case 2:
                    boardCount = this.CategoriesBoardsList.Items.Count - 1;
                    excludeCurrentBoard = true;
                    useList = true;
                    break;
                case 3:
                    boardID = this.CategoriesBoardsList.SelectedValue.ToType<int>();
                    break;
            }

            if (!int.TryParse(this.CategoriesNumber.Text.Trim(), out numCategories))
            {
                return NoCategories;
            }

            if (numCategories <= 0)
            {
                return NoCategories;
            }

            if (numCategories > categoryCreateLimit)
            {
                numCategories = categoryCreateLimit;
            }

            return this.CreateCategoriesBase(
                boardID,
                boardCount,
                numForums,
                numTopics,
                numMessages,
                numCategories,
                excludeCurrentBoard,
                useList);
        }

        /// <summary>
        /// The create categories base.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="boardCount">
        /// The board count.
        /// </param>
        /// <param name="numForums">
        /// The num forums.
        /// </param>
        /// <param name="numTopics">
        /// The num topics.
        /// </param>
        /// <param name="numMessages">
        /// The num messages.
        /// </param>
        /// <param name="numCategories">
        /// The num categories.
        /// </param>
        /// <param name="excludeCurrentBoard">
        /// The exclude current board.
        /// </param>
        /// <param name="useList">
        /// The use list.
        /// </param>
        /// <returns>
        /// The create categories base.
        /// </returns>
        private string CreateCategoriesBase(
            int boardID,
            int boardCount,
            int numForums,
            int numTopics,
            int numMessages,
            int numCategories,
            bool excludeCurrentBoard,
            bool useList)
        {
            int ib;
            for (ib = 0; ib < boardCount; ib++)
            {
                if (useList)
                {
                    boardID = this.CategoriesBoardsList.Items[ib].Value.ToType<int>();
                }

                int i;
                if (!(excludeCurrentBoard && boardID == YafContext.Current.PageBoardID))
                {
                    for (i = 0; i < numCategories; i++)
                    {
                        string catName = this.CategoryPrefixTB.Text.Trim() + Guid.NewGuid();

                        // TODO: should return number of categories created 
                        string failureMassage;
                        CategorySaveTask.Start(PageContext.PageModuleID, boardID, 0, catName, null, 100,true, null, null, out failureMassage);
                        DataTable dt = CommonDb.category_simplelist(PageContext.PageModuleID, 0, 10000);

                        foreach (DataRow dr in dt.Rows.Cast<DataRow>().Where(dr => dr["Name"].ToString() == catName))
                        {
                            this.CreateForums(dr["CategoryID"].ToType<int>(), null, numForums, numTopics, numMessages);
                        }

                        // We don't have last category index, so not implemented.... CreateForums( categoryID,numForums,numTopics,numMessages )
                    }
                }

                i = 0;
            }

            return "{0} Categories, ".FormatWith(ib);
        }

        /// <summary>
        /// Create forums from Forums page
        /// </summary>
        /// <returns>
        /// The create forums.
        /// </returns>
        private int CreateForums()
        {
            object parentID = null;
            int parentIDInt;
            if (int.TryParse(this.ForumsParent.Text.Trim(), out parentIDInt))
            {
                parentID = parentIDInt;
            }

            int numTopics;
            if (!int.TryParse(this.ForumsTopicsNumber.Text.Trim(), out numTopics))
            {
                return 0;
            }

            if (numTopics < 0)
            {
                return 0;
            }

            int numPosts;
            if (!int.TryParse(this.ForumsMessagesNumber.Text.Trim(), out numPosts))
            {
                return 0;
            }

            if (numPosts < 0)
            {
                return 0;
            }

            int numForums;
            if (!int.TryParse(this.ForumsNumber.Text.Trim(), out numForums))
            {
                return 0;
            }

            if (numForums <= 0)
            {
                return 0;
            }

            int categoryID;
            if (!int.TryParse(this.ForumsCategory.SelectedValue, out categoryID))
            {
                return 0;
            }

            if (numForums > createCommonLimit)
            {
                numForums = createCommonLimit;
            }

            return this.CreateForums(categoryID, parentID, numForums, numTopics, numPosts);
        }

        /// <summary>
        /// Create forums from Categories
        /// </summary>
        /// <param name="categoryID">
        /// </param>
        /// <param name="parentID">
        /// The parent ID.
        /// </param>
        /// <param name="numForums">
        /// The num Forums.
        /// </param>
        /// <param name="_topicsToCreate">
        /// Number of topics to create.
        /// </param>
        /// <param name="_messagesToCreate">
        /// Number of messages to create.
        /// </param>
        /// <returns>
        /// The create forums.
        /// </returns>
        private int CreateForums(
            int categoryID, [NotNull] object parentID, int numForums, int _topicsToCreate, int _messagesToCreate)
        {
            bool countMessagesInStatistics = this.ForumsCountMessages.Text.Trim().IsNotSet();

            bool isHiddenIfNoAccess = true;

            // ForumsCategory.Items.FindByValue("0").Selected = true; 
            long uniqueForum = 0;
            int iforums;
            for (iforums = 0; iforums < numForums; iforums++)
            {
                long forumId = 0;
                this.randomGuid = Guid.NewGuid().ToString();
                DataTable _accessForumList = CommonDb.forumaccess_list(
                    this.PageContext.PageModuleID, 
                    forumId, 
                    null, 
                    false, true, true);
                string errorMessage;
                forumId = ForumSaveTask.Start(
                    PageContext.PageModuleID,
                    forumId,
                    categoryID,
                    parentID,
                    this.ForumPrefixTB.Text.Trim() + this.randomGuid,
                    "Description of " + this.ForumPrefixTB.Text.Trim() + this.randomGuid,
                    100,
                    false,
                    isHiddenIfNoAccess,
                    countMessagesInStatistics,
                    false,
                    this.ForumsStartMask.SelectedValue,
                    null,
                    null,
                    null,
                    null,
                    false,
                    PageContext.PageUserID,
                    false,
                    true,
                    null,
                    null,
                    out errorMessage);

                if (forumId <= 0)
                {
                    continue;
                }

                for (int i1 = 0; i1 < _accessForumList.Rows.Count; i1++)
                {
                    CommonDb.forumaccess_save(
                        PageContext.PageModuleID,
                        forumId,
                        _accessForumList.Rows[i1]["GroupID"],
                        _accessForumList.Rows[i1]["AccessMaskID"].ToType<int>());
                }

                CommonDb.forumaccess_save(PageContext.PageModuleID, forumId, this.ForumsGroups.SelectedValue, this.ForumsAdminMask.SelectedValue);

                if (_topicsToCreate <= 0)
                {
                    continue;
                }

                if (uniqueForum == forumId)
                {
                    continue;
                }

                this.CreateTopics(forumId.ToType<int>(), _topicsToCreate, _messagesToCreate);
                uniqueForum = forumId;
            }

            return iforums;
        }

        /// <summary>
        /// The create p messages.
        /// </summary>
        /// <returns>
        /// The number of created p messages.
        /// </returns>
        private int CreatePMessages()
        {
            int numPMessages;
            if (!int.TryParse(this.PMessagesNumber.Text.Trim(), out numPMessages))
            {
                return 0;
            }

            if (numPMessages <= 0)
            {
                return 0;
            }

            string fromUser = this.From.Text.Trim();
            string toUser = this.To.Text.Trim();
            if (numPMessages > createCommonLimit)
            {
                numPMessages = createCommonLimit;
            }

            int i;
            for (i = 0; i < numPMessages; i++)
            {
                this.randomGuid = Guid.NewGuid().ToString();
                CommonDb.pmessage_save(
                    PageContext.PageModuleID,
                    CommonDb.user_get(
                        PageContext.PageModuleID,
                        YafContext.Current.PageBoardID,
                        Membership.GetUser(fromUser).ProviderUserKey),
                    CommonDb.user_get(
                        PageContext.PageModuleID,
                        YafContext.Current.PageBoardID,
                        Membership.GetUser(toUser).ProviderUserKey),
                    this.TopicPrefixTB.Text.Trim() + this.randomGuid,
                    "{0}{1}   {2}".FormatWith(pmessagePrefix, this.randomGuid, this.PMessageText.Text.Trim()),
                    6, 
                    -1);
            }

            if (this.MarkRead.Checked)
            {
                int userAid = CommonDb.user_get(PageContext.PageModuleID, YafContext.Current.PageBoardID, Membership.GetUser(toUser).ProviderUserKey);
                foreach (DataRow dr in CommonDb.pmessage_list(PageContext.PageModuleID, null, userAid, null).Rows)
                {
                    CommonDb.pmessage_markread(PageContext.PageModuleID, dr["PMessageID"]);

                    // Clearing cache with old permissions data...
                    this.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userAid));
                }
            }

            return i;
        }

        /// <summary>
        /// The create posts.
        /// </summary>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <param name="numMessages">
        /// The num messages.
        /// </param>
        /// <returns>
        /// The number of created posts.
        /// </returns>
        private int CreatePosts(int forumID, int topicID, int numMessages)
        {
            if (numMessages <= 0)
            {
                if (!int.TryParse(this.PostsNumber.Text.Trim(), out numMessages))
                {
                    return 0;
                }
            }

            if (numMessages <= 0)
            {
                return 0;
            }

            int categoryID;
            if (!int.TryParse(this.PostsCategory.SelectedValue, out categoryID))
            {
                return 0;
            }

            if (forumID <= 0)
            {
                if (!int.TryParse(this.PostsForum.SelectedValue, out forumID))
                {
                    return 0;
                }
            }

            if (topicID <= 0)
            {
                if (!int.TryParse(this.PostsTopic.SelectedValue, out topicID))
                {
                    return 0;
                }
            }

            // if ( numMessages > createCommonLimit ) numMessages = createCommonLimit;        
            long messageid = 0;
            int iposts;
            const int ReplyTo = -1;
            for (iposts = 0; iposts < numMessages; iposts++)
            {
                this.randomGuid = Guid.NewGuid().ToString();
                CommonDb.message_save(
                    PageContext.PageModuleID, 
                    topicID,
                    this.PageContext.PageUserID,
                    "msgd-" + this.randomGuid + "  " + this.MyMessage.Text.Trim(),
                    null,
                    this.Get<HttpRequestBase>().GetUserRealIPAddress(), 
                    null,
                    ReplyTo,
                    this.GetMessageFlags(),
                    null,
                    ref messageid);

                // User != null ? null : From.Text
            }

            return iposts;
        }

        /// <summary>
        /// The create topics.
        /// </summary>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="numTopics">
        /// The num topics.
        /// </param>
        /// <param name="_messagesToCreate">
        /// The _messages to create.
        /// </param>
        /// <returns>
        /// Number of created topics.
        /// </returns>
        private int CreateTopics(int forumID, int numTopics, int _messagesToCreate)
        {
            object priority = 0;
            if (forumID <= 0)
            {
                priority = this.TopicsPriorityList.SelectedValue;
            }

            if (numTopics <= 0)
            {
                if (!int.TryParse(this.TopicsNumber.Text.Trim(), out numTopics))
                {
                    return 0;
                }
            }

            if (numTopics <= 0)
            {
                return 0;
            }

            int categoryId;

            if (!int.TryParse(this.TopicsCategory.SelectedValue, out categoryId))
            {
                return 0;
            }

            if (forumID <= 0)
            {
                if (!int.TryParse(this.TopicsForum.SelectedValue, out forumID))
                {
                    return 0;
                }
            }

            if (_messagesToCreate <= 0)
            {
                if (!int.TryParse(this.TopicsMessagesNumber.Text.Trim(), out _messagesToCreate))
                {
                    return 0;
                }
            }

            if (_messagesToCreate < 0)
            {
                return 0;
            }

            // if ( numTopics > createCommonLimit ) numTopics = createCommonLimit;         
            int itopics;
            for (itopics = 0; itopics < numTopics; itopics++)
            {
                long messageid = 0;
                this.randomGuid = Guid.NewGuid().ToString();
                object pollID = null;

                long topicId = CommonDb.topic_save(
                    PageContext.PageModuleID,
                    forumID,
                    this.TopicPrefixTB.Text.Trim() + this.randomGuid,
                    string.Empty,
                    string.Empty,
                    "{0}{1}descr".FormatWith(this.TopicPrefixTB.Text.Trim(), this.randomGuid),
                    this.MessageContentPrefixTB.Text.Trim() + this.randomGuid,
                    this.PageContext.PageUserID,
                    priority,
                    (UserMembershipHelper.FindUsersByName(this.PageContext.User.UserName).Count > 0)
                        ? "{0}_{1}".FormatWith(this.GetText("COMMON", "GUEST_NAME"), this.PageContext.User.UserName)
                        : this.PageContext.User.UserName,
                    this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    DateTime.UtcNow,
                    string.Empty,
                    this.GetMessageFlags(),
                    null,
                    ref messageid,
                    string.Empty);

                if (this.PollCreate.Checked)
                {
                   
                    var choiceList = new List<PollChoice>();

                    // Fill in choice list
                    choiceList.Add(
                        new PollChoice()
                            {
                                Choice = "ans1-" + this.randomGuid,
                                ObjectPath = null,
                                MimeType = null,
                                Votes = 0
                            });
                    choiceList.Add(
                        new PollChoice()
                            {
                                Choice = "ans2-" + this.randomGuid,
                                ObjectPath = null,
                                MimeType = null,
                                Votes = 0
                            });
                   
                    CommonDb.poll_save(new PollGroup
                                        {
                                            mid = PageContext.PageModuleID,
                                            CategoryId = null,
                                            ForumId = null,
                                            TopicId = (int?)topicId,
                                            BoardId = null,
                                            UserId = PageContext.PageUserID,
                                            Flags = new PollGroupFlags { IsBound = false },
                                            Polls =
                                                new List<Poll>
                                                    {
                                                        new Poll
                                                            {
                                                                Question =
                                                                    "question-"
                                                                    + this.randomGuid,
                                                                Choices = choiceList,
                                                                UserID =
                                                                    this.PageContext.PageUserID,
                                                                ObjectPath = null,
                                                                MimeType = null,
                                                                Flags =
                                                                    new PollFlags
                                                                        {
                                                                            IsClosedBound
                                                                                =
                                                                                false,
                                                                            AllowMultipleChoices
                                                                                =
                                                                                false,
                                                                            ShowVoters
                                                                                =
                                                                                false,
                                                                            AllowSkipVote
                                                                                =
                                                                                false
                                                                        }
                                                            }
                                                    }
                                        });

                }

                if (_messagesToCreate > 0)
                {
                    this.CreatePosts(forumID, topicId.ToType<int>(), _messagesToCreate);
                }

                // User != null ? null : From.Text
            }

            return itopics;
        }

        /// <summary>
        /// From Users Tab
        /// </summary>
        /// <returns>
        /// The create users.
        /// </returns>
        private string CreateUsers()
        {
            int boardID = 0;
            int _users_Number;
            const int _outCounter = 0;
            int _countLimit = 1;
            bool _excludeCurrentBoard = false;

            if (!int.TryParse(this.UsersNumber.Text.Trim(), out _users_Number))
            {
                return null;
            }

            if (_users_Number <= 0)
            {
                return null;
            }

            switch (this.UsersBoardsOptions.SelectedIndex)
            {
                case 0:
                    boardID = YafContext.Current.PageBoardID;
                    break;
                case 1:
                    _countLimit = this.UsersBoardsList.Items.Count;
                    break;
                case 2:
                    _countLimit = this.UsersBoardsList.Items.Count - 1;
                    _excludeCurrentBoard = true;
                    break;
                case 3:
                    boardID = this.UsersBoardsList.SelectedValue.ToType<int>();
                    break;
            }

            return this.CreateUsers(0, _users_Number, _outCounter, _countLimit, _excludeCurrentBoard);
        }

        /// <summary>
        /// The create users.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="_users_Number">
        /// The _users_ number.
        /// </param>
        private void CreateUsers(int boardID, int _users_Number)
        {
            int _outCounter = 0;
            int _countLimit = 1;
            bool _excludeCurrentBoard = false;

            this.CreateUsers(boardID, _users_Number, _outCounter, _countLimit, _excludeCurrentBoard);
        }

        /// <summary>
        /// The create users.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="_users_Number">
        /// The _users_ number.
        /// </param>
        /// <param name="_outCounter">
        /// The _out counter.
        /// </param>
        /// <param name="_countLimit">
        /// The _count limit.
        /// </param>
        /// <param name="_excludeCurrentBoard">
        /// The _exclude current board.
        /// </param>
        /// <returns>
        /// The string with number of created users.
        /// </returns>
        private string CreateUsers(
            int boardID, int _users_Number, int _outCounter, int _countLimit, bool _excludeCurrentBoard)
        {
            int iboards;

            // if ( _users_Number > createCommonLimit ) _users_Number = createCommonLimit;
            for (iboards = 0; iboards < _countLimit; iboards++)
            {
                boardID = this.UsersBoardsList.Items[iboards].Value.ToType<int>();
                int i;
                for (i = 0; i < this.UsersNumber.Text.Trim().ToType<int>(); i++)
                {
                    this.randomGuid = Guid.NewGuid().ToString();
                    string newEmail = this.UserPrefixTB.Text.Trim() + this.randomGuid + "@test.info";
                    string newUsername = this.UserPrefixTB.Text.Trim() + this.randomGuid;

                    if (UserMembershipHelper.UserExists(newUsername, newEmail))
                    {
                        continue;
                    }

                    string hashinput = DateTime.UtcNow + newEmail + Security.CreatePassword(20);
                    string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

                    MembershipCreateStatus status;
                    MembershipUser user = this.Get<MembershipProvider>().CreateUser(
                        newUsername,
                        this.Password.Text.Trim(),
                        newEmail,
                        this.Question.Text.Trim(),
                        this.Answer.Text.Trim(),
                        !this.Get<YafBoardSettings>().EmailVerification,
                        null,
                        out status);

                    if (status != MembershipCreateStatus.Success)
                    {
                        continue;
                    }

                    // setup inital roles (if any) for this user
                    RoleMembershipHelper.SetupUserRoles(boardID, newUsername);

                    // create the user in the YAF DB as well as sync roles...
                    int? userID = RoleMembershipHelper.CreateForumUser(user, boardID);

                    // create profile
                    YafUserProfile userProfile = YafUserProfile.GetProfile(newUsername);

                    // setup their inital profile information
                    userProfile.Location = this.Location.Text.Trim();
                    userProfile.Homepage = this.HomePage.Text.Trim();
                    userProfile.Save();

                    // save the time zone...
                    if (
                        !(this.UsersBoardsList.Items[iboards].Value.ToType<int>() == YafContext.Current.PageBoardID &&
                          _excludeCurrentBoard))
                    {
                        CommonDb.user_save(
                            this.PageContext.PageModuleID,
                            CommonDb.user_get(this.PageContext.PageModuleID, boardID, user.ProviderUserKey),
                            boardID,
                            null,
                            null,
                            null,
                            this.TimeZones.SelectedValue.ToType<int>(),
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null, 
                            20, 
                            20);
                        _outCounter++;
                    }
                }
            }

            return _outCounter + " Users in " + iboards + " Board(s); ";
        }

        /// <summary>
        /// The get message flags.
        /// </summary>
        /// <returns>
        /// The method returns message flags.
        /// </returns>
        private int GetMessageFlags()
        {
            var editorModule = this.PageContext.EditorModuleManager.GetBy(this.Get<YafBoardSettings>().ForumEditor);

            var topicFlags = new MessageFlags
                {
                    IsHtml = editorModule.UsesHTML,
                    IsBBCode = editorModule.UsesBBCode,
                    IsPersistent = false,
                    IsApproved = this.PageContext.IsAdmin
                };

            // Bypass Approval if Admin or Moderator.
            return topicFlags.BitValue;
        }

        /// <summary>
        /// The populate_ controls.
        /// </summary>
        private void Populate_Controls()
        {
        }

        /// <summary>
        /// The validate controls values.
        /// </summary>
        /// <returns>
        /// The method returns true if all controls values are valid.
        /// </returns>
        private bool ValidateControlsValues()
        {
            if (Membership.GetUser(this.From.Text.Trim()) == null)
            {
                this.PageContext.AddLoadMessage("You should enter valid 'from' user name.");
                return false;
            }

            if (Membership.GetUser(this.To.Text.Trim()) == null)
            {
                this.PageContext.AddLoadMessage("You should enter valid 'to' user name.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.PMessagesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for pmessage number.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.UsersNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for users.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.CategoriesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for categories.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.BoardNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for boards.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.ForumsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for forums.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.TopicsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter  integer value for topics.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.PostsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for generated posts .");
                return false;
            }

            // **************************
            if (!ValidationHelper.IsValidInt(this.BoardsTopicsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for topics generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.BoardsForumsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for forums generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.BoardsCategoriesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for categories generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.BoardsMessagesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
                return false;
            }

            // ****************************
            if (!ValidationHelper.IsValidInt(this.CategoriesTopicsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for forums generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.CategoriesForumsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for categories generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.CategoriesMessagesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
                return false;
            }

            // *************************
            if (!ValidationHelper.IsValidInt(this.ForumsTopicsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for categories generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.ForumsMessagesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
                return false;
            }

            // **********************************                
            if (!ValidationHelper.IsValidInt(this.TopicsMessagesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.BoardsUsersNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for users generated with boards.");
                return false;
            }

            return true;
        }

        #endregion
    }
}