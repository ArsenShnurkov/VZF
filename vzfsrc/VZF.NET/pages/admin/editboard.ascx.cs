namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using VZF.Data.Common;
    using VZF.Utils;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Admin Edit Board Page
    /// </summary>
    public partial class editboard : AdminPage
    {
        #region Properties

        /// <summary>
        ///   Gets BoardID.
        /// </summary>
        protected int? BoardID
        {
            get
            {
                if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("b").IsNotSet()) 
                    return null;

                int boardId;
                if (int.TryParse(this.Request.QueryString.GetFirstOrDefault("b"), out boardId))
                {
                    return boardId;
                }

                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The bind data_ access mask id.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void BindData_AccessMaskID([NotNull] object sender, [NotNull] EventArgs e)
        {

            ((DropDownList)sender).DataSource = CommonDb.accessmask_aforumlist(
                mid: this.PageContext.PageModuleID,
                boardId: this.PageContext.PageBoardID,
                accessMaskId: null,
                excludeFlags: AccessFlags.Flags.None.ToInt(),
                pageUserId: null,            
                isAdminMask: true,
                isCommonMask: true);
            ((DropDownList)sender).DataValueField = "AccessMaskID";
            ((DropDownList)sender).DataTextField = "Name";
        }

        /// <summary>
        /// Cancel Edit/Create and return Back to the Boards Listening
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_boards);
        }

        /// <summary>
        /// Show/Hide Create Host Admin User Creating
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CreateAdminUser_CheckedChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.AdminInfo.Visible = this.CreateAdminUser.Checked;
        }

        /// <summary>
        /// The create board.
        /// </summary>
        /// <param name="adminName">The admin name.</param>
        /// <param name="adminPassword">The admin password.</param>
        /// <param name="adminEmail">The admin email.</param>
        /// <param name="adminPasswordQuestion">The admin password question.</param>
        /// <param name="adminPasswordAnswer">The admin password answer.</param>
        /// <param name="boardName">The board name.</param>
        /// <param name="boardMembershipAppName">The board membership app name.</param>
        /// <param name="boardRolesAppName">The board roles app name.</param>
        /// <param name="createUserAndRoles">The create user and roles.</param>
        protected void CreateBoard(
            [NotNull] string adminName,
            [NotNull] string adminPassword,
            [NotNull] string adminEmail,
            [NotNull] string adminPasswordQuestion,
            [NotNull] string adminPasswordAnswer,
            [NotNull] string boardName,
            [NotNull] string boardMembershipAppName,
            [NotNull] string boardRolesAppName,
            bool createUserAndRoles)
        {
            // Store current App Names
            string currentMembershipAppName = this.Get<MembershipProvider>().ApplicationName;
            string currentRolesAppName = this.Get<RoleProvider>().ApplicationName;

            if (boardMembershipAppName.IsSet() && boardRolesAppName.IsSet())
            {
                // Change App Names for new board
                this.Get<MembershipProvider>().ApplicationName = boardMembershipAppName;
                this.Get<MembershipProvider>().ApplicationName = boardRolesAppName;
            }

            int newBoardID;  

            // english.xml by default
            string languageFile =
                StaticDataHelper.Cultures().Where(
                    c => c.IetfLanguageTag.Equals(this.Culture.SelectedValue)).FirstOrDefault().CultureFile;

            if (createUserAndRoles)
            {
                // Create new admin users
                MembershipCreateStatus createStatus;
                MembershipUser newAdmin = this.Get<MembershipProvider>()
                    .CreateUser(
                        adminName,
                        adminPassword,
                        adminEmail,
                        adminPasswordQuestion,
                        adminPasswordAnswer,
                        true,
                        null,
                        out createStatus);

                if (createStatus != MembershipCreateStatus.Success)
                {
                    this.PageContext.AddLoadMessage(
                        "Create User Failed: {0}".FormatWith(this.GetMembershipErrorMessage(createStatus)));
                    throw new ApplicationException(
                        "Create User Failed: {0}".FormatWith(this.GetMembershipErrorMessage(createStatus)));
                }

                // Create groups required for the new board
                RoleMembershipHelper.CreateRole("Administrators");
                RoleMembershipHelper.CreateRole("Registered");

                // Add new admin users to group
                RoleMembershipHelper.AddUserToRole(newAdmin.UserName, "Administrators");

                // Create Board
                newBoardID = CommonDb.board_create(
                    PageContext.PageModuleID,
                    newAdmin.UserName,
                    newAdmin.Email,
                    newAdmin.ProviderUserKey,
                    boardName,
                    this.Culture.SelectedItem.Value,
                    languageFile,
                    boardMembershipAppName,
                    boardRolesAppName,
                    Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty,
                    this.PageContext().IsHostAdmin);
            }
            else
            {
                // new admin
                MembershipUser newAdmin = UserMembershipHelper.GetUser();

                // Create Board
                newBoardID = CommonDb.board_create(
                    PageContext.PageModuleID,
                    newAdmin.UserName,
                    newAdmin.Email,
                    newAdmin.ProviderUserKey,
                    boardName,
                    this.Culture.SelectedItem.Value,
                    languageFile,
                    boardMembershipAppName,
                    boardRolesAppName,
                    Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty,
                    this.PageContext().IsHostAdmin);
            }

            if (newBoardID > 0 && Config.MultiBoardFolders)
            {
                // Successfully created the new board
                string boardFolder = this.Server.MapPath(Path.Combine(Config.BoardRoot, "{0}/".FormatWith(newBoardID)));

                // Create New Folders.
                if (!Directory.Exists(Path.Combine(boardFolder, "Images")))
                {
                    // Create the Images Folders
                    Directory.CreateDirectory(Path.Combine(boardFolder, "Images"));

                    // Create Sub Folders
                    Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Avatars"));
                    Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Categories"));
                    Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Forums"));
                    Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Emoticons"));
                    Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Medals"));
                    Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Ranks"));
                    Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Topics"));
                }

                if (!Directory.Exists(Path.Combine(boardFolder, "Themes")))
                {
                    Directory.CreateDirectory(Path.Combine(boardFolder, "Themes"));

                    // Need to copy default theme to the Themes Folder
                }

                if (!Directory.Exists(Path.Combine(boardFolder, "Uploads")))
                {
                    Directory.CreateDirectory(Path.Combine(boardFolder, "Uploads"));
                }
            }

            // Return application name to as they were before.
            this.Get<MembershipProvider>().ApplicationName = currentMembershipAppName;
            YafContext.Current.Get<RoleProvider>().ApplicationName = currentRolesAppName;
        }

        /// <summary>
        /// Gets the membership error message.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>
        /// The get membership error message.
        /// </returns>
        [NotNull]
        protected string GetMembershipErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return this.GetText("ADMIN_EDITBOARD", "STATUS_DUP_NAME");

                case MembershipCreateStatus.DuplicateEmail:
                    return this.GetText("ADMIN_EDITBOARD", "STATUS_DUP_EMAIL");

                case MembershipCreateStatus.InvalidPassword:
                    return this.GetText("ADMIN_EDITBOARD", "STATUS_INVAL_PASS");

                case MembershipCreateStatus.InvalidEmail:
                    return this.GetText("ADMIN_EDITBOARD", "STATUS_INVAL_MAIL");

                case MembershipCreateStatus.InvalidAnswer:
                    return this.GetText("ADMIN_EDITBOARD", "STATUS_INVAL_ANSWER");

                case MembershipCreateStatus.InvalidQuestion:
                    return this.GetText("ADMIN_EDITBOARD", "STATUS_INVAL_QUESTION");

                case MembershipCreateStatus.InvalidUserName:
                    return this.GetText("ADMIN_EDITBOARD", "STATUS_INVAL_NAME");

                case MembershipCreateStatus.ProviderError:
                    return this.GetText("ADMIN_EDITBOARD", "STATUS_PROVIDER_ERR");

                case MembershipCreateStatus.UserRejected:
                    return this.GetText("ADMIN_EDITBOARD", "STATUS_USR_REJECTED");

                default:
                    return this.GetText("ADMIN_EDITBOARD", "STATUS_UNKNOWN");
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_BOARDS", "TITLE"),
                YafBuildLink.GetLink(ForumPages.admin_editboard));
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITBOARD", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_BOARDS", "TITLE"),
                this.GetText("ADMIN_EDITBOARD", "TITLE"));

            this.Save.Text = this.GetText("SAVE");
            this.Cancel.Text = this.GetText("CANCEL");

            var cultures = StaticDataHelper.Cultures().OrderBy(x => x.NativeName);
            var culture = cultures.FirstOrDefault();

            this.Culture.DataSource = cultures;
            this.Culture.DataValueField = "IetfLanguageTag";
            this.Culture.DataTextField = "NativeName";
           

            this.BindData();

            if (this.Culture.Items.Count > 0)
            {
                this.Culture.Items.FindByValue(this.Get<YafBoardSettings>().Culture).Selected = true;
            }

            if (this.BoardID != null)
            {
                this.CreateNewAdminHolder.Visible = false;

                using (DataTable dt = CommonDb.board_list(PageContext.PageModuleID, this.BoardID))
                {
                    DataRow row = dt.Rows[0];
                    this.Name.Text = (string)row["Name"];
                    this.AllowThreaded.Checked = Convert.ToBoolean(row["AllowThreaded"]);
                    this.BoardMembershipAppName.Text = row["MembershipAppName"].ToString();
                }
            }
            else
            {
                this.UserName.Text = this.User.UserName;
                this.UserEmail.Text = this.User.Email;
            }
        }

        /// <summary>
        /// Save Current board / Create new Board
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.Name.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITBOARD", "MSG_NAME_BOARD"));
                return;
            }

            if (this.BoardID == null && this.CreateAdminUser.Checked)
            {
                if (this.UserName.Text.Trim().Length == 0)
                {
                    this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITBOARD", "MSG_NAME_ADMIN"));
                    return;
                }

                if (this.UserEmail.Text.Trim().Length == 0)
                {
                    this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITBOARD", "MSG_EMAIL_ADMIN"));
                    return;
                }

                if (this.UserPass1.Text.Trim().Length == 0)
                {
                    this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITBOARD", "MSG_PASS_ADMIN"));
                    return;
                }

                if (this.UserPass1.Text != this.UserPass2.Text)
                {
                    this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITBOARD", "MSG_PASS_MATCH"));
                    return;
                }
            }

            if (this.BoardID != null)
            {              
                string langFile =
                StaticDataHelper.Cultures().Where(
                  c => c.IetfLanguageTag.Equals(this.Culture.SelectedValue)).FirstOrDefault().CultureFile;        

                // Save current board settings
                CommonDb.board_save(
                    PageContext.PageModuleID,
                    this.BoardID,
                    langFile,
                    this.Culture.SelectedItem.Value,
                    this.Name.Text.Trim(),
                    this.AllowThreaded.Checked);
            }
            else
            {
                // Create board
                // MEK says : Purposefully set MembershipAppName without including RolesAppName yet, as the current providers don't support different Appnames.
                if (this.CreateAdminUser.Checked)
                {
                    this.CreateBoard(
                        this.UserName.Text.Trim(),
                        this.UserPass1.Text,
                        this.UserEmail.Text.Trim(),
                        this.UserPasswordQuestion.Text.Trim(),
                        this.UserPasswordAnswer.Text.Trim(),
                        this.Name.Text.Trim(),
                        this.BoardMembershipAppName.Text.Trim(),
                        this.BoardMembershipAppName.Text.Trim(),
                        true);
                }
                else
                {
                    // create admin user from logged in user...
                    this.CreateBoard(
                        null,
                        null,
                        null,
                        null,
                        null,
                        this.Name.Text.Trim(),
                        this.BoardMembershipAppName.Text.Trim(),
                        this.BoardMembershipAppName.Text.Trim(),
                        false);
                }
            }

            // Done
            this.PageContext.BoardSettings = null;
            YafBuildLink.Redirect(ForumPages.admin_boards);
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.DataBind();
        }

        #endregion
    }
}