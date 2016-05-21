﻿namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Net.Mail;
  using System.Web.Security;

  using VZF.Data.Common;

  using YAF.Classes;
  
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using VZF.Utils;
  using VZF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for reguser.
  /// </summary>
  public partial class reguser : AdminPage
  {
    #region Methods

    /// <summary>
    /// The forum register_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ForumRegister_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.Page.IsValid)
        {
            return;
        }

        string newEmail = this.Email.Text.Trim();
        string newUsername = this.UserName.Text.Trim();

        if (!ValidationHelper.IsValidEmail(newEmail))
        {
            this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_INVALID_MAIL"));
            return;
        }

        if (UserMembershipHelper.UserExists(this.UserName.Text.Trim(), newEmail))
        {
            this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_NAME_EXISTS"));
            return;
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
            // error of some kind
            this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_ERROR_CREATE").FormatWith(status));
            return;
        }

        // setup inital roles (if any) for this user
        RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, newUsername);

        // create the user in the YAF DB as well as sync roles...
        int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

        // create profile
        YafUserProfile userProfile = YafUserProfile.GetProfile(newUsername);

        // setup their inital profile information
        userProfile.Location = this.Location.Text.Trim();
        userProfile.Homepage = this.HomePage.Text.Trim();
        userProfile.Save();

        // save the time zone...
        CommonDb.user_save(this.PageContext.PageModuleID, UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey), 
            this.PageContext.PageBoardID, 
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

        if (this.Get<YafBoardSettings>().EmailVerification)
        {
            // save verification record...
            CommonDb.checkemail_save(PageContext.PageModuleID, userID, hash, user.Email);

            // send template email
            var verifyEmail = new YafTemplateEmail("VERIFYEMAIL");

            verifyEmail.TemplateParams["{link}"] = YafBuildLink.GetLink(ForumPages.approve, true, "k={0}", hash);
            verifyEmail.TemplateParams["{key}"] = hash;
            verifyEmail.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;
            verifyEmail.TemplateParams["{forumlink}"] = "{0}".FormatWith(this.ForumURL);

            string subject =
                this.GetText("COMMON", "EMAILVERIFICATION_SUBJECT").FormatWith(
                    this.Get<YafBoardSettings>().Name);

            verifyEmail.SendEmail(new MailAddress(newEmail, newUsername), subject, true);
        }

        bool autoWatchTopicsEnabled =
            this.Get<YafBoardSettings>().DefaultNotificationSetting.Equals(
                UserNotificationSetting.TopicsIPostToOrSubscribeTo);

        CommonDb.user_savenotification(PageContext.PageModuleID, UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey),
            true,
            autoWatchTopicsEnabled,
            this.Get<YafBoardSettings>().DefaultNotificationSetting,
            this.Get<YafBoardSettings>().DefaultSendDigestEmail);


        // success
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_CREATED").FormatWith(this.UserName.Text.Trim()));
        YafBuildLink.Redirect(ForumPages.admin_reguser);
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
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

        this.PageLinks.AddLink(this.GetText("ADMIN_USERS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_users));

        // current page label (no link)
        this.PageLinks.AddLink(this.GetText("ADMIN_REGUSER", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
           this.GetText("ADMIN_ADMIN", "Administration"),
           this.GetText("ADMIN_USERS", "TITLE"),
           this.GetText("ADMIN_REGUSER", "TITLE"));

        this.ForumRegister.Text = this.GetText("ADMIN_REGUSER", "REGISTER");
        this.cancel.Text = this.GetText("COMMON", "CANCEL");

        this.TimeZones.DataSource = StaticDataHelper.TimeZones();
        this.DataBind();
        this.TimeZones.Items.FindByValue("0").Selected = true;
    }

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_users);
    }

    #endregion
  }
}