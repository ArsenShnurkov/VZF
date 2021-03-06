namespace YAF.Pages
{
    // YAF.Pages
    #region

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;

    using VZF.Data.Common;
    using VZF.Types.Data;

    using YAF.Classes;
    
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using VZF.Utils;

    #endregion

    /// <summary>
    /// The Members List Page
    /// </summary>
    public partial class members : ForumPage
    {
        #region Fields
        /// <summary>
        /// The _userListDataTable.
        /// </summary>
        private DataTable _userListDataTable;
        /*
        /// <summary>
        /// The _sortName.
        /// </summary>
        private bool _sortName = true;

        /// <summary>
        /// The _sortRank.
        /// </summary>
        private bool _sortRank;

        /// <summary>
        /// The _sortPosts.
        /// </summary>
        private bool _sortPosts;

        /// <summary>
        /// The _sortJoined.
        /// </summary>
        private bool _sortJoined; 
        */
        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "members" /> class.
        /// </summary>
        public members()
            : base("MEMBERS")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The search_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void Search_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// The search_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void Reset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-direct to self.
            YafBuildLink.Redirect(ForumPages.members);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the avatar Url for the user
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="avatarString">The avatar string.</param>
        /// <param name="hasAvatarImage">if set to <c>true</c> [has avatar image].</param>
        /// <param name="email">The email.</param>
        /// <returns>Returns the File Url</returns>
        protected string GetAvatarUrlFileName(int userId, string avatarString, bool hasAvatarImage, string email)
        {
            string avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(
                userId, avatarString, hasAvatarImage, email);

            return avatarUrl.IsNotSet()
                       ? "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot)
                       : avatarUrl;
        }

        /// <summary>
        /// protects from script in "location" field
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The get string safely.
        /// </returns>
        protected string GetStringSafely(object value)
        {
            return value == null ? string.Empty : this.HtmlEncode(value.ToString());
        }

        /// <summary>
        /// Get all users from user_list for this board.
        /// </summary>
        /// <param name="literals">
        /// The literals.
        /// </param>
        /// <param name="lastUserId">
        /// The last User Id.
        /// </param>
        /// <param name="specialSymbol">
        /// The special Symbol.
        /// </param>
        /// <param name="totalCount">
        /// The total Count.
        /// </param>
        /// <returns>
        /// The Members List
        /// </returns>
        protected DataTable GetUserList(string literals, int lastUserId, bool specialSymbol, out int totalCount)
        {
            this._userListDataTable = CommonDb.user_listmembers(PageContext.PageModuleID, PageContext.PageBoardID,
                null,
                true,
                this.Group.SelectedIndex <= 0 ? null : this.Group.SelectedValue,
                this.Ranks.SelectedIndex <= 0 ? null : this.Ranks.SelectedValue,
                this.Get<YafBoardSettings>().UseStyledNicks,
                lastUserId,
                literals,
                specialSymbol,
                specialSymbol,
                this.Pager.CurrentPageIndex,
                Pager.PageSize,
                (int?)ViewState["SortNameField"],
                (int?)ViewState["SortRankNameField"],
                (int?)ViewState["SortJoinedField"],
                (int?)ViewState["SortNumPostsField"],
                (int?)ViewState["SortLastVisitField"],
                NumPostsTB.Text.Trim().IsSet() ? NumPostsTB.Text.Trim().ToType<int>() : 0,
                this.NumPostDDL.SelectedIndex < 0 ? 3 : (NumPostsTB.Text.Trim().IsSet() ? this.NumPostDDL.SelectedValue.ToType<int>() : 0));

            if (this.Get<YafBoardSettings>().UseStyledNicks)
            {
                new StyleTransform(this.Get<ITheme>()).DecodeStyleByTable(ref this._userListDataTable, false);
            }

            if (this._userListDataTable.Rows.Count > 0)
            {
                // commits the deletes to the table
                totalCount = (int)this._userListDataTable.Rows[0]["TotalCount"];
            }
            else
            {
                totalCount = 0;
            }

            return this._userListDataTable;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.DefaultButton = this.SearchByUserName.UniqueID;

            if (this.IsPostBack)
            {
                return;
            }

            this.SearchByUserName.Focus();

            this.ViewState["SortNameField"] = 1;
            this.ViewState["SortRankNameField"] = 0;
            this.ViewState["SortJoinedField"] = 0;
            this.ViewState["SortNumPostsField"] = 0;
            this.ViewState["SortLastVisitField"] = 0;

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

            //// this.SetSort("Name", true);

            this.UserName.Text = this.GetText("username");
            this.Rank.Text = this.GetText("rank");
            this.Joined.Text = this.GetText("joined");
            this.Posts.Text = this.GetText("posts");
            this.LastVisitLB.Text = this.GetText("members", "lastvisit");

            using (var dt = CommonDb.group_list(this.PageContext.PageModuleID, this.PageContext.PageBoardID, null, 0, 1000000))
            {
                var dtt = dt.Clone();
                foreach (DataRow row in dt.Rows)
                {
                    // get role flags
                    if (new GroupFlags(row["Flags"]).IsGuest)
                    {
                        continue;
                    }

                    var drows = dtt.NewRow();
                    drows.ItemArray = row.ItemArray;
                    dtt.Rows.Add(drows);
                }

                // add empty item for no filtering
                DataRow newRow = dtt.NewRow();
                newRow["Name"] = this.GetText("ALL");
                newRow["GroupID"] = DBNull.Value;
                dtt.Rows.InsertAt(newRow, 0);

                // commits the deletes to the table
                dtt.AcceptChanges();

                this.Group.DataSource = dtt;
                this.Group.DataTextField = "Name";
                this.Group.DataValueField = "GroupID";
                this.Group.DataBind();
            }

            this.NumPostDDL.Items.Add(new ListItem(this.GetText("MEMBERS", "NUMPOSTSEQUAL"), "1"));
            this.NumPostDDL.Items.Add(new ListItem(this.GetText("MEMBERS", "NUMPOSTSLESSOREQUAL"), "2"));
            this.NumPostDDL.Items.Add(new ListItem(this.GetText("MEMBERS", "NUMPOSTSMOREOREQUAL"), "3"));

            this.NumPostDDL.DataBind();

            // get list of user ranks for filtering
            var rankList = CommonDb.rank_list(PageContext.PageModuleID, this.PageContext.PageBoardID, null).ToList();

            // add empty for for no filtering
            rankList.Insert(0, new rank_list_Result(0, this.GetText("ALL")));

            for (int index = 0; index < rankList.Count; index++)
            {
                var drow = rankList[index];
                if ((drow.Flags & RankFlags.Flags.IsHidden.ToInt()) == RankFlags.Flags.IsHidden.ToInt())
                {
                    rankList.Remove(drow);
                }
            }

            this.Ranks.DataSource = rankList;
            this.Ranks.DataTextField = "Name";
            this.Ranks.DataValueField = "RankID";
            this.Ranks.DataBind();

            this.BindData();
        }

        /// <summary>
        /// The pager_ page change.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Pager_PageChange(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// The joined_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Joined_Click(object sender, EventArgs e)
        {
            this.SetSort("Joined");

            this.ViewState["SortNameField"] = 0;
            this.ViewState["SortRankNameField"] = 0;
            this.ViewState["SortNumPostsField"] = 0;
            this.ViewState["SortLastVisitField"] = 0;

            this.BindData();
        }

        /// <summary>
        /// The LastVisitLB Click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void LastVisitLB_Click(object sender, EventArgs e)
        {
            this.SetSort("LastVisit");

            this.ViewState["SortNameField"] = 0;
            this.ViewState["SortRankNameField"] = 0;
            this.ViewState["SortJoinedField"] = 0;
            this.ViewState["SortNumPostsField"] = 0;

            this.BindData();
        }

        /// <summary>
        /// The posts_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Posts_Click(object sender, EventArgs e)
        {
            this.SetSort("NumPosts");

            this.ViewState["SortNameField"] = 0;
            this.ViewState["SortRankNameField"] = 0;
            this.ViewState["SortJoinedField"] = 0;
            this.ViewState["SortLastVisitField"] = 0;

            this.BindData();
        }

        /// <summary>
        /// The rank_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Rank_Click(object sender, EventArgs e)
        {
            this.SetSort("RankName");

            this.ViewState["SortNameField"] = 0;
            this.ViewState["SortJoinedField"] = 0;
            this.ViewState["SortNumPostsField"] = 0;
            this.ViewState["SortLastVisitField"] = 0;

            this.BindData();
        }

        /// <summary>
        /// The user name_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void UserName_Click(object sender, EventArgs e)
        {
            this.SetSort("Name");
          
            this.ViewState["SortRankNameField"] = 0;
            this.ViewState["SortJoinedField"] = 0;
            this.ViewState["SortNumPostsField"] = 0;
            this.ViewState["SortLastVisitField"] = 0;

            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            // this.AlphaSort1.PagerPage = ForumPages.members;
            this.Pager.PageSize = this.Get<YafBoardSettings>().MemberListPageSize;
            char selectedCharLetter = this.AlphaSort1.CurrentLetter;

            // get the user list...
            int totalCount;

            string selectedLetter = this.UserSearchName.Text.IsSet() ? this.UserSearchName.Text.Trim() : selectedCharLetter.ToString(CultureInfo.InvariantCulture);
            
            int numpostsTb;

            if (this.NumPostsTB.Text.Trim().IsSet() &&
                (!int.TryParse(this.NumPostsTB.Text.Trim(), out numpostsTb) || numpostsTb < 0 || numpostsTb > int.MaxValue))
            {
                PageContext.AddLoadMessage(this.GetText("MEMBERS", "INVALIDPOSTSVALUE"));
                return;
            }

            if (this.NumPostsTB.Text.Trim().IsNotSet())
            {
                this.NumPostsTB.Text = "0";
                this.NumPostDDL.SelectedValue = "3";
            }

            // get the user list...
            this._userListDataTable = this.GetUserList(
                selectedLetter,
                0,
                this.UserSearchName.Text.IsNotSet() || (selectedCharLetter == char.MinValue || selectedCharLetter == '#'),
                out totalCount);
            
            // get the view from the datatable
            DataView userListDataView = _userListDataTable.DefaultView;
            string nameField = this.Get<YafBoardSettings>().EnableDisplayName ? "DisplayName" : "Name";
            
            this.Pager.Count = totalCount;
            this.MemberList.DataSource = this._userListDataTable;
            this.DataBind();

            // handle the sort fields at the top
            // TODO: make these "sorts" into controls
            // this.SortAscendingName = (string)this.ViewState["SortField"] == nameField;
            // this.SortAscendingName.Value = "Name";
            switch ((int?)ViewState["SortNameField"])
            {
                case 1:
                    this.SortUserName.Src = this.GetThemeContents(
                   "SORT", "ASCENDING");
                     this.SortUserName.Visible = true;
                    break;
                case 2:
                     this.SortUserName.Src = this.GetThemeContents(
                   "SORT", "DESCENDING");
                     this.SortUserName.Visible = true;
                    break;
                default:
                    ViewState["SortNameField"] = 0;
                    this.SortUserName.Visible = false;
                    break;
            }

            switch ((int?)ViewState["SortRankNameField"])
            {
                case 1:
                    this.SortRank.Src = this.GetThemeContents(
                   "SORT", "ASCENDING");
                     this.SortRank.Visible = true;
                    break;
                case 2:
                     this.SortRank.Src = this.GetThemeContents(
                   "SORT", "DESCENDING");
                     this.SortRank.Visible = true;
                    break;
                default:
                    ViewState["SortRankNameField"] = 0;
                    this.SortRank.Visible = false;
                    break;
            } 
 
             switch ((int?)ViewState["SortJoinedField"])
            {
                case 1:
                    this.SortJoined.Src = this.GetThemeContents(
                   "SORT", "ASCENDING");
                     this.SortJoined.Visible = true;
                    break;
                case 2:
                     this.SortJoined.Src = this.GetThemeContents(
                   "SORT", "DESCENDING");
                     this.SortJoined.Visible = true;
                    break;
                default:
                    ViewState["SortJoinedField"] = 0;
                    this.SortJoined.Visible = false;
                    break;
            }

             switch ((int?)ViewState["SortNumPostsField"])
            {
                case 1:
                    this.SortPosts.Src = this.GetThemeContents(
                   "SORT", "ASCENDING");
                     this.SortPosts.Visible = true;
                    break;
                case 2:
                     this.SortPosts.Src = this.GetThemeContents(
                   "SORT", "DESCENDING");
                     this.SortPosts.Visible = true;
                    break;
                default:
                    ViewState["SortNumPostsField"] = 0;
                    this.SortPosts.Visible = false;
                    break;
            } 

             switch ((int?)ViewState["SortLastVisitField"])
            {
                case 1:
                    this.SortLastVisit.Src = this.GetThemeContents(
                   "SORT", "ASCENDING");
                     this.SortLastVisit.Visible = true;
                    break;
                case 2:
                     this.SortLastVisit.Src = this.GetThemeContents(
                   "SORT", "DESCENDING");
                     this.SortLastVisit.Visible = true;
                    break;
                default:
                    ViewState["SortLastVisitField"] = 0;
                    this.SortLastVisit.Visible = false;
                    break;
            } 
        }



        /// <summary>
        /// Helper function for setting up the current sort on the member list view
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        private void SetSort(string field)
        {
            switch (field)
            {
                case "Name":
                    this.ViewState["SortNameField"] = this.ViewState["SortNameField"] == null ? 0 : 
                    (this.ViewState["SortNameField"].ToType<int>() == 1 ? 2 : 1);
                    break;
                case "RankName":
                    this.ViewState["SortRankNameField"] = this.ViewState["SortRankNameField"] == null ? 0 :
                  (this.ViewState["SortRankNameField"].ToType<int>() == 1 ? 2 : 1);
                  break;
                case "Joined":
                    this.ViewState["SortJoinedField"] = this.ViewState["SortJoinedField"] == null ? 0 :
                  (this.ViewState["SortJoinedField"].ToType<int>() == 1 ? 2 : 1);
                  break;
                case "NumPosts":
                  this.ViewState["SortNumPostsField"] = this.ViewState["SortNumPostsField"] == null ? 0 :
                 (this.ViewState["SortNumPostsField"].ToType<int>() == 1 ? 2 : 1);
                 break;
                case "LastVisit":
                   this.ViewState["SortLastVisitField"] = this.ViewState["SortLastVisitField"] == null  ? 0 :
                 (this.ViewState["SortLastVisitField"].ToType<int>() == 1 ? 2 : 1);
                 break;
            }
        }
        #endregion
    }
}