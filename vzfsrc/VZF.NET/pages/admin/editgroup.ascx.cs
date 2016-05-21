namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using VZF.Data.Common;

    
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using VZF.Utils;
    using VZF.Utils.Helpers;
    using System.Collections.Generic;
    using YAF.Classes;
    using VZF.Utilities;
    using System.Web;
 

    #endregion

    /// <summary>
    /// Interface for creating or editing user roles/groups.
    /// </summary>
    public partial class editgroup : AdminPage
    {
        #region Methods

        public DataTable AccessMasksList { get; set; }

        private Dictionary<int, DataTable> personalAccessMaskListIds;

        /// <summary>
        /// Handles databinding event of initial access maks dropdown control.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void BindData_AccessMaskID([NotNull] object sender, [NotNull] EventArgs e)
        {
            // We don't change access masks if it's a guest
            if (!this.IsGuestX.Checked)
            {
                // get sender object as dropdown list
                var c = (DropDownList) sender;

                // list all access masks as data source
                c.DataSource = this.AccessMasksList;

                // set value and text field names
                c.DataValueField = "AccessMaskID";
                c.DataTextField = "Name";
            }
            else
            {
                
            }
        }

        /// <summary>
        /// Handles click on cancel button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // go back to roles administration
            YafBuildLink.Redirect(ForumPages.admin_groups);
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

            // admin index
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_GROUPS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_groups));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITGROUP", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
               this.GetText("ADMIN_ADMIN", "Administration"),
               this.GetText("ADMIN_GROUPS", "TITLE"),
               this.GetText("ADMIN_EDITGROUP", "TITLE"));
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // this needs to be done just once, not during postbacks
            if (this.IsPostBack)
            {
                return;
            }

            // create page links
            this.CreatePageLinks();

            this.Save.Text = this.GetText("COMMON", "SAVE");
            this.Cancel.Text = this.GetText("COMMON", "CANCEL");

            if (Config.LargeForumTree && this.Request.QueryString.GetFirstOrDefault("i") != null)
            {
                string args = "&links=1";
                int? groupId = null;
                if (this.Request.QueryString.GetFirstOrDefault("i") !=null)
                {
                    groupId = this.Request.QueryString.GetFirstOrDefault("i").ToType<int>();
                    args = args + "&amdd={0}".FormatWith(groupId);
                }

                this.treeRow.Visible = true;
                YafContext.Current.PageElements.RegisterJQueryUI();
                YafContext.Current.PageElements.RegisterJsResourceInclude("fancytree", "js/jquery.fancytree-all.min.js");
                YafContext.Current.PageElements.RegisterCssIncludeResource("css/fancytree/{0}/ui.fancytree.css".FormatWith(YafContext.Current.Get<YafBoardSettings>().FancyTreeTheme));
                YafContext.Current.PageElements.RegisterJsResourceInclude("ftreeeditgroupjs",
                   "js/fancytree.vzf.nodesadminsetgroupaccess.js");

                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "fancytreeeditgroupscr",
                     "fancyTreeSetNodesGroupAccessLazyJS('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');"
                        .FormatWith(
                        Config.JQueryAlias,
                        "treegroupaccess",
                        PageContext.PageUserID,
                        PageContext.PageBoardID,
                        groupId,                                        
                        args,
                        string.Empty,
                        "{0}resource.ashx?".FormatWith(YafForumInfo.ForumClientFileRoot),
                        "&forumUrl={0}".FormatWith(HttpUtility.UrlDecode(YafBuildLink.GetBasePath()))));
            }

            // bind data
            this.BindData();
            // is this editing of existing role or creation of new one?
            if (this.Request.QueryString.GetFirstOrDefault("i") == null)
            {
                return;
            }

            // we are not creating new role
            this.NewGroupRow.Visible = false;

            // get data about edited role
            using (
                DataTable dt = CommonDb.group_list(this.PageContext.PageModuleID, this.PageContext.PageBoardID, this.Request.QueryString.GetFirstOrDefault("i"), 0, 1000000))
            {
                // get it as row
                DataRow row = dt.Rows[0];

                // get role flags
                var flags = new GroupFlags(row["Flags"]);

                // set controls to role values
                this.Name.Text = (string)row["Name"];

                this.IsAdminX.Checked = flags.IsAdmin;
                this.IsAdminX.Enabled = !flags.IsGuest;

                this.IsStartX.Checked = flags.IsStart;
                this.IsStartX.Enabled = !flags.IsGuest;

                this.IsModeratorX.Checked = flags.IsModerator;
                this.IsModeratorX.Enabled = !flags.IsGuest;

                this.IsHiddenX.Checked = flags.IsHidden;
                // this.IsHiddenX.Enabled = !flags.IsGuest;

                this.PMLimit.Text = row["PMLimit"].ToString();
                this.PMLimit.Enabled = !flags.IsGuest;

                this.StyleTextBox.Text = row["Style"].ToString();

                this.Priority.Text = row["SortOrder"].ToString();

                this.UsrAlbums.Text = row["UsrAlbums"].ToString();
                this.UsrAlbums.Enabled = !flags.IsGuest;

                this.UsrAlbumImages.Text = row["UsrAlbumImages"].ToString();
                this.UsrAlbumImages.Enabled = !flags.IsGuest;

                this.UsrSigChars.Text = row["UsrSigChars"].ToString();
                this.UsrSigChars.Enabled = !flags.IsGuest;

                this.UsrSigBBCodes.Text = row["UsrSigBBCodes"].ToString();
                this.UsrSigBBCodes.Enabled = !flags.IsGuest;

                this.UsrSigHTMLTags.Text = row["UsrSigHTMLTags"].ToString();
                this.UsrSigHTMLTags.Enabled = !flags.IsGuest;

                this.PersonalGroupsNumber.Text = row["UsrPersonalGroups"].ToString();
                this.PersonalGroupsNumber.Enabled = !flags.IsGuest;

                this.PersonalForumsNumber.Text = row["UsrPersonalForums"].ToString();
                this.PersonalForumsNumber.Enabled = !flags.IsGuest;

                this.PersonalAccessMasksNumber.Text = row["UsrPersonalMasks"].ToString();
                this.PersonalAccessMasksNumber.Enabled = !flags.IsGuest;

               // this.UserNickStyleEditor.Styles = row["Style"].ToString();

                this.Description.Text = row["Description"].ToString();

                this.IsGuestX.Checked = flags.IsGuest;

                // IsGuest flag can be set for only one role. if it isn't for this, disable that row
                if (flags.IsGuest)
                {
                    this.IsGuestTR.Visible = true;
                    this.IsGuestX.Enabled = !flags.IsGuest;
                    this.AccessList.Visible = false;
                }

            }
        }

        /// <summary>
        /// Handles click on save button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!ValidationHelper.IsValidInt(this.PMLimit.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_VALID_NUMBER"));
                return;
            }

            if (!ValidationHelper.IsValidInt(this.Priority.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_INTEGER"));
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrAlbums.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_ALBUM_NUMBER"));
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrSigChars.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_SIG_NUMBER"));
                return;
            }

            if (!ValidationHelper.IsValidInt(this.PersonalForumsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_PFORUMS_NUMBER"));
                return;
            }

            if (!ValidationHelper.IsValidInt(this.PersonalAccessMasksNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_PFORUMS_MASKSNUMBER"));
                return;
            }

            if (!ValidationHelper.IsValidInt(this.PersonalGroupsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_PFORUMS_PGROUPSNUMBER"));
                return;
            }


            if (!ValidationHelper.IsValidInt(this.UsrAlbumImages.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_TOTAL_NUMBER"));
                return;
            }

            // Role
            long roleID = 0;

            // get role ID from page's parameter
            if (this.Request.QueryString.GetFirstOrDefault("i") != null)
            {
                roleID = long.Parse(this.Request.QueryString.GetFirstOrDefault("i"));
            }

            // get new and old name
            string roleName = this.Name.Text.Trim();
            string oldRoleName = string.Empty;

            // if we are editing exising role, get it's original name
            if (roleID != 0)
            {
                // get the current role name in the DB
                using (DataTable dt = CommonDb.group_list(this.PageContext.PageModuleID, YafContext.Current.PageBoardID, roleID, 0, 1000000))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        oldRoleName = row["Name"].ToString();
                    }
                }
            }

            // save role and get its ID if it's new (if it's old role, we get it anyway)
            roleID = CommonDb.group_save(PageContext.PageModuleID, roleID,
              this.PageContext.PageBoardID,
              roleName,
              this.IsAdminX.Checked,
              this.IsGuestX.Checked,
              this.IsStartX.Checked,
              this.IsModeratorX.Checked,
              this.IsHiddenX.Checked,
              AccessMaskID.SelectedValue,
              this.PMLimit.Text.Trim(),
              this.StyleTextBox.Text.Trim(),
              this.Priority.Text.Trim(),
              this.Description.Text,
              this.UsrSigChars.Text,
              this.UsrSigBBCodes.Text,
              this.UsrSigHTMLTags.Text,
              this.UsrAlbums.Text.Trim(),
              this.UsrAlbumImages.Text.Trim(),
              PageContext.PageUserID,
              false,
              this.PersonalForumsNumber.Text.Trim(),
              this.PersonalAccessMasksNumber.Text.Trim(),
              this.PersonalGroupsNumber.Text.Trim());

            // empty out access table
            CommonDb.activeaccess_reset(PageContext.PageModuleID);

            // see if need to rename an existing role...
            if (oldRoleName.IsSet() && roleName != oldRoleName && RoleMembershipHelper.RoleExists(oldRoleName) && !RoleMembershipHelper.RoleExists(roleName) && !this.IsGuestX.Checked)
            {
                // transfer users in addition to changing the name of the role...
                var users = this.Get<RoleProvider>().GetUsersInRole(oldRoleName);

                // delete the old role...
                RoleMembershipHelper.DeleteRole(oldRoleName, false);

                // create new role...
                RoleMembershipHelper.CreateRole(roleName);

                if (users.Any())
                {
                    // put users into new role...
                    this.Get<RoleProvider>().AddUsersToRoles(users, new[] { roleName });
                }
            }
            else if (!RoleMembershipHelper.RoleExists(roleName) && !this.IsGuestX.Checked)
            {
                // if role doesn't exist in provider's data source, create it

                // simply create it
                RoleMembershipHelper.CreateRole(roleName);
            }

            // Clearing cache with old user lazy data...
            this.Get<IDataCache>().Remove(k => k.StartsWith(Constants.Cache.ActiveUserLazyData.FormatWith(String.Empty)));
            
            // Access masks for a newly created or an existing role
            if (this.Request.QueryString.GetFirstOrDefault("i") != null)
            {
                    // go trhough all forums
                    for (int i = 0; i < this.AccessList.Items.Count; i++)
                    {
                        // get current repeater item
                        RepeaterItem item = this.AccessList.Items[i];

                        // get forum ID
                        int forumID = int.Parse(((Label)item.FindControl("ForumID")).Text);

                        // save forum access masks for this role
                        CommonDb.forumaccess_save(PageContext.PageModuleID, forumID, roleID,
                                                  ((DropDownList)item.FindControl("AccessmaskID")).SelectedValue);
                    }

                YafBuildLink.Redirect(ForumPages.admin_groups);
            }

            // remove caching in case something got updated...
            this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);

            // Done, redirect to role editing page
            YafBuildLink.Redirect(ForumPages.admin_editgroup, "i={0}", roleID);
        }

        /// <summary>
        /// Handles pre-render event of each forum's access mask dropdown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void SetDropDownIndex([NotNull] object sender, [NotNull] EventArgs e)
        {
            // get dropdown which raised this event
            var list = (DropDownList)sender;

            // select value from the list
            ListItem item = list.Items.FindByValue(list.Attributes["value"]);

            // verify something was found...
            if (item != null)
            {
                item.Selected = true;
            }

        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // set datasource of access list (list of forums and role's access masks) if we are editing existing mask
            if (!Config.LargeForumTree && this.Request.QueryString.GetFirstOrDefault("i") != null)
            {
                this.AccessList.DataSource = CommonDb.forumaccess_group(PageContext.PageModuleID, this.Request.QueryString.GetFirstOrDefault("i"), PageContext.PageUserID, false);
            }

            // TODO: here should be a swithch between common forums and personal forums
            this.AccessMasksList = 
                CommonDb.accessmask_aforumlist(mid: this.PageContext.PageModuleID, boardId: this.PageContext.PageBoardID, 
                accessMaskId: null, excludeFlags: 0, pageUserId: null,
                isAdminMask: true, isCommonMask: true);
            // bind data to controls
            this.DataBind();
        }

        #endregion
    }
}