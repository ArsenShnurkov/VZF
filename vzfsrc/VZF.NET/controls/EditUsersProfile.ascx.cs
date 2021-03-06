namespace VZF.Controls
{
    #region Using

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using FarsiLibrary;

    using VZF.Data.Common;

    using YAF.Classes;
    
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using VZF.Utilities;
    using VZF.Utils;
    using VZF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The edit users profile.
    /// </summary>
    public partial class EditUsersProfile : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The admin edit mode.
        /// </summary>
        private bool _adminEditMode;

        /// <summary>
        ///   The current user id.
        /// </summary>
        private int _currentUserId;

        /// <summary>
        ///   The _user data.
        /// </summary>
        private CombinedUserDataHelper _userData;

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

        /// <summary>
        ///   Gets or sets a value indicating whether UpdateEmailFlag.
        /// </summary>
        protected bool UpdateEmailFlag
        {
            get
            {
                return this.ViewState["bUpdateEmail"] != null && Convert.ToBoolean(this.ViewState["bUpdateEmail"]);
            }

            set
            {
                this.ViewState["bUpdateEmail"] = value;
            }
        }

        /// <summary>
        ///   Gets UserData.
        /// </summary>
        [NotNull]
        private CombinedUserDataHelper UserData
        {
            get
            {
                return this._userData ?? (this._userData = new CombinedUserDataHelper(this._currentUserId));
            }
        }

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
            YafBuildLink.Redirect(this._adminEditMode ? ForumPages.admin_users : ForumPages.cp_profile);
        }

        /// <summary>
        /// The email_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Email_TextChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.UpdateEmailFlag = true;
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and DatePicker JS...
                YafContext.Current.PageElements.RegisterJQuery();
                YafContext.Current.PageElements.RegisterJQueryUI();

            var ci = CultureInfo.CreateSpecificCulture(this.GetCulture(true));
           
            if (!string.IsNullOrEmpty(this.GetText("COMMON", "CAL_JQ_CULTURE")))
            {
                var jqueryuiUrl = !Config.JQueryUILangFile.StartsWith("http")
                                      ? YafForumInfo.GetURLToResource(Config.JQueryUILangFile)
                                      : Config.JQueryUILangFile;

                YafContext.Current.PageElements.RegisterJsInclude("datepickerlang", jqueryuiUrl);

                if (ci.IsFarsiCulture())
                {
                    YafContext.Current.PageElements.RegisterJsResourceInclude("datepicker-farsi", "js/jquery.ui.datepicker-farsi.js");
                }
            }

            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "DatePickerJs",
                JavaScriptBlocks.DatePickerLoadJs(
                    this.Birthday.ClientID,
                    this.GetText("COMMON", "CAL_JQ_CULTURE_DFORMAT"),
                    this.GetText("COMMON", "CAL_JQ_CULTURE")));

            YafContext.Current.PageElements.RegisterJsResourceInclude("msdropdown", "js/jquery.msDropDown.js");

             YafContext.Current.PageElements.RegisterJsBlockStartup(
                "dropDownJs", JavaScriptBlocks.DropDownLoadJs(this.Country.ClientID));

            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.msDropDown.css"); 

            base.OnPreRender(e);
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
            this.Page.Form.DefaultButton = this.UpdateProfile.UniqueID;

            this.PageContext.QueryIDs = new QueryStringIDHelper("u");

            if (this._adminEditMode && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
            {
                this._currentUserId = this.PageContext.QueryIDs["u"].ToType<int>();
            }
            else
            {
                this._currentUserId = this.PageContext.PageUserID;
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.LoginInfo.Visible = true;

            // Begin Modifications for enhanced profile
            this.Gender.Items.Add(this.GetText("PROFILE", "gender0"));
            this.Gender.Items.Add(this.GetText("PROFILE", "gender1"));
            this.Gender.Items.Add(this.GetText("PROFILE", "gender2"));

            // End Modifications for enhanced profile
            this.UpdateProfile.Text = this.GetText("COMMON", "SAVE");
            this.Cancel.Text = this.GetText("COMMON", "CANCEL");

            this.ForumSettingsRows.Visible = this.Get<YafBoardSettings>().AllowUserTheme ||
                                             this.Get<YafBoardSettings>().AllowUserLanguage ||
                                             this.Get<YafBoardSettings>().AllowPMEmailNotification;

            this.UserThemeRow.Visible = this.Get<YafBoardSettings>().AllowUserTheme;
            this.TrTextEditors.Visible = this.Get<YafBoardSettings>().AllowUsersTextEditor;
            this.UserLanguageRow.Visible = this.Get<YafBoardSettings>().AllowUserLanguage;
            this.UserLoginRow.Visible = this.Get<YafBoardSettings>().AllowSingleSignOn;
            this.MetaWeblogAPI.Visible = this.Get<YafBoardSettings>().AllowPostToBlog;
            this.LoginInfo.Visible = this.Get<YafBoardSettings>().AllowEmailChange;
            this.DisplayNamePlaceholder.Visible = this.Get<YafBoardSettings>().EnableDisplayName &&
                                                  this.Get<YafBoardSettings>().AllowDisplayNameModification;

            this.BindData();
        }

        /// <summary>
        /// The update profile_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void UpdateProfile_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.HomePage.Text.IsSet())
            {
                // add http:// by default
                if (!Regex.IsMatch(this.HomePage.Text.Trim(), @"^(http|https|ftp|ftps|git|svn|news)\://.*"))
                {
                    this.HomePage.Text = "http://{0}".FormatWith(this.HomePage.Text.Trim());
                }

                if (!ValidationHelper.IsValidURL(this.HomePage.Text))
                {
                    this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_HOME"));
                    return;
                }
            }

            if (this.Weblog.Text.IsSet() && !ValidationHelper.IsValidURL(this.Weblog.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_WEBLOG"));
                return;
            }

            if (this.MSN.Text.IsSet() && !ValidationHelper.IsValidEmail(this.MSN.Text))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_MSN"));
                return;
            }

            if (this.Xmpp.Text.IsSet() && !ValidationHelper.IsValidXmpp(this.Xmpp.Text))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_XMPP"));
                return;
            }

            if (this.ICQ.Text.IsSet() &&
                !(ValidationHelper.IsValidEmail(this.ICQ.Text) || ValidationHelper.IsNumeric(this.ICQ.Text)))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_ICQ"));
                return;
            }

            if (this.Facebook.Text.IsSet() && !ValidationHelper.IsNumeric(this.Facebook.Text))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_FACEBOOK"));
                return;
            }

            string displayName = null;

            if (this.Get<YafBoardSettings>().EnableDisplayName &&
                this.Get<YafBoardSettings>().AllowDisplayNameModification)
            {
                if (this.DisplayName.Text.Trim().Length < 2)
                {
                    this.PageContext.AddLoadMessage(this.GetText("PROFILE", "INVALID_DISPLAYNAME"));
                    return;
                }

                if (this.DisplayName.Text.Trim() != this.UserData.DisplayName)
                {
                    if (this.Get<IUserDisplayName>().GetId(this.DisplayName.Text.Trim()).HasValue)
                    {
                        this.PageContext.AddLoadMessage(
                          this.GetText("REGISTER", "ALREADY_REGISTERED_DISPLAYNAME"));

                        return;
                    }

                    displayName = this.DisplayName.Text.Trim();
                }
            }

            string userName = UserMembershipHelper.GetUserNameFromID(this._currentUserId);
            if (this.UpdateEmailFlag)
            {
                string newEmail = this.Email.Text.Trim();
              
                if (!ValidationHelper.IsValidEmail(newEmail))
                {
                    this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_EMAIL"));
                    return;
                }

                string userNameFromEmail = this.Get<MembershipProvider>().GetUserNameByEmail(this.Email.Text.Trim());
                if (userNameFromEmail.IsSet() && userNameFromEmail != userName)
                 {
                    this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_EMAIL"));
                    return;
                 }

                if (this.Get<YafBoardSettings>().EmailVerification)
                {
                    this.SendEmailVerification(newEmail);
                }
                else
                {
                    // just update the e-mail...
                    try
                    {
                        UserMembershipHelper.UpdateEmail(this._currentUserId, this.Email.Text.Trim());
                    }
                    catch (ApplicationException)
                    {
                        this.PageContext.AddLoadMessage(this.GetText("PROFILE", "DUPLICATED_EMAIL"));
                        
                        return;
                    }
                }
            }

            this.UpdateUserProfile(userName);

            // vzrus: We should do it as we need to write null value to db, else it will be empty. 
            // Localizer currently treats only nulls. 
            object languageFileName = null;
            object culture = this.Culture.SelectedValue;
            object theme = this.Theme.SelectedValue;
            object editor = this.ForumEditor.SelectedValue;

            if (string.IsNullOrEmpty(this.Theme.SelectedValue))
            {
                theme = null;
            }

            if (string.IsNullOrEmpty(this.ForumEditor.SelectedValue))
            {
                editor = null;
            }

            if (string.IsNullOrEmpty(this.Culture.SelectedValue))
            {
                culture = null;
            }
            else
            {
                languageFileName = StaticDataHelper.Cultures().Where(
                    ci => culture.ToString() == ci.IetfLanguageTag).FirstOrDefault().CultureFile;  
            }
           
            // save remaining settings to the DB
            CommonDb.user_save(
                this.PageContext.PageModuleID,
                this._currentUserId,
                this.PageContext.PageBoardID,
                null,
                displayName,
                null,
                this.TimeZones.SelectedValue.ToType<int>(),
                languageFileName,
                culture,
                theme,
                this.SingleSignOn.Checked,
                editor,
                this.UseMobileTheme.Checked,
                null,
                null,
                null,
                this.DSTUser.Checked,
                this.HideMe.Checked,
                null,
                this.TopicsPerPageDDL.SelectedValue,
                this.PostsPerPageDDL.SelectedValue);

            // vzrus: If it's a guest edited by an admin registry value should be changed
            DataTable dt = CommonDb.user_list(PageContext.PageModuleID, this.PageContext.PageBoardID, this._currentUserId, true, null, null, false);

            if (dt.Rows.Count > 0 && dt.Rows[0]["IsGuest"].ToType<bool>())
            {
                CommonDb.registry_save(PageContext.PageModuleID, "timezone", this.TimeZones.SelectedValue, 0);
            }

            // clear the cache for this user...)
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this._currentUserId));

            YafContext.Current.Get<IDataCache>().Clear();

            if (!this._adminEditMode)
            {
                YafBuildLink.Redirect(ForumPages.cp_profile);
            }
            else
            {
                this._userData = null;
                this.BindData();
            }
        }

        /// <summary>
        /// Check if the Selected Country has any Regions
        /// and if yes load them.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void LookForNewRegions(object sender, EventArgs e)
        {
            if (this.Country.SelectedValue != null)
            {
                if (this.Country.SelectedValue.IsSet())
                {
                    this.LookForNewRegionsBind(this.Country.SelectedValue);
                    this.Region.DataBind();
                }
                else
                {
                    this.Region.DataSource = null;
                    this.RegionTr.Visible = false;
                }
            }
            else
            {
                this.Region.DataSource = null;
                this.Region.DataBind();
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.TimeZones.DataSource = StaticDataHelper.TimeZones();
            if (this.Get<YafBoardSettings>().AllowUserTheme)
            {
                this.Theme.DataSource = StaticDataHelper.Themes();
                this.Theme.DataTextField = "Theme";
                this.Theme.DataValueField = "FileName";
            }

            if (this.Get<YafBoardSettings>().AllowUserLanguage)
            {
                var cultures = StaticDataHelper.Cultures().OrderBy(x => x.NativeName);
                var culture = cultures.FirstOrDefault();

                this.Culture.DataSource = cultures;
                this.Culture.DataValueField = "IetfLanguageTag";
                this.Culture.DataTextField = "NativeName";
            }

            this.Country.DataSource = StaticDataHelper.Country().OrderBy(x => x.Name);
            this.Country.DataValueField = "Value";
            this.Country.DataTextField = "Name";

            this.TopicsPerPageDDL.DataSource = this.CreateDataSourceForTopicsPostsDropDownLists(this.Get<YafBoardSettings>().TopicsPerPage);
            this.TopicsPerPageDDL.DataTextField = "ID";
            this.TopicsPerPageDDL.DataValueField = "Number";

            this.PostsPerPageDDL.DataSource = this.CreateDataSourceForTopicsPostsDropDownLists(this.Get<YafBoardSettings>().PostsPerPage);
            this.PostsPerPageDDL.DataTextField = "ID";
            this.PostsPerPageDDL.DataValueField = "Number";

            string currentCultureLocal = this.GetCulture(true);
           
            if (this.UserData.Profile.Country.IsSet())
            {
                this.LookForNewRegionsBind(this.UserData.Profile.Country);
            }

            if (this.Get<YafBoardSettings>().AllowUsersTextEditor)
            {
                this.ForumEditor.DataSource = this.Get<IModuleManager<ForumEditor>>().ActiveAsDataTable("Editors");
                this.ForumEditor.DataValueField = "Value";
                this.ForumEditor.DataTextField = "Name";
            }

            this.DataBind();

            var ci = CultureInfo.CreateSpecificCulture(currentCultureLocal);
           
                if (this.Get<YafBoardSettings>().UseFarsiCalender && ci.IsFarsiCulture())
                {
                    this.Birthday.Text = this.UserData.Profile.Birthday > DateTimeHelper.SqlDbMinTime() ||
                                         this.UserData.Profile.Birthday.IsNullOrEmptyDBField()
                                         ? PersianDateConverter.ToPersianDate(this.UserData.Profile.Birthday).ToString("d")
                                             : PersianDateConverter.ToPersianDate(PersianDate.MinValue).ToString("d");
                }
                else
                {
                    this.Birthday.Text = this.UserData.Profile.Birthday > DateTimeHelper.SqlDbMinTime() ||
                                         this.UserData.Profile.Birthday.IsNullOrEmptyDBField()
                                             ? this.UserData.Profile.Birthday.Date.ToString(
                                                 ci.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture)
                                             : DateTimeHelper.SqlDbMinTime().Date.ToString(
                                                 ci.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture);
                }

            this.Birthday.ToolTip = this.GetText("COMMON", "CAL_JQ_TT");
            this.DisplayName.Text = this.UserData.DisplayName;
            this.City.Text = this.UserData.Profile.City;
            this.Location.Text = this.UserData.Profile.Location;
            this.HomePage.Text = this.UserData.Profile.Homepage;
            this.Email.Text = this.UserData.Email;
            this.Realname.Text = this.UserData.Profile.RealName;
            this.Occupation.Text = this.UserData.Profile.Occupation;
            this.Interests.Text = this.UserData.Profile.Interests;
            this.Weblog.Text = this.UserData.Profile.Blog;
            this.WeblogUrl.Text = this.UserData.Profile.BlogServiceUrl;
            this.WeblogID.Text = this.UserData.Profile.BlogServicePassword;
            this.WeblogUsername.Text = this.UserData.Profile.BlogServiceUsername;
            this.MSN.Text = this.UserData.Profile.MSN;
            this.YIM.Text = this.UserData.Profile.YIM;
            this.AIM.Text = this.UserData.Profile.AIM;
            this.ICQ.Text = this.UserData.Profile.ICQ;
            this.Facebook.Text = this.UserData.Profile.Facebook;
            this.Twitter.Text = this.UserData.Profile.Twitter;
            this.TwitterId.Text = this.UserData.Profile.TwitterId;
            this.Xmpp.Text = this.UserData.Profile.XMPP;
            this.Skype.Text = this.UserData.Profile.Skype;
            this.Gender.SelectedIndex = this.UserData.Profile.Gender;

            if (this.UserData.Profile.Country.IsSet())
            {
                ListItem countryItem = this.Country.Items.FindByValue(this.UserData.Profile.Country.Trim());
                if (countryItem != null)
                {
                    countryItem.Selected = true;
                }
            }

            if (this.TopicsPerPageDDL.Items.Count > 0)
            {
                int tpp = this.UserData.TopicsPerPage.ToType<int>() > this.Get<YafBoardSettings>().TopicsPerPage ? this.Get<YafBoardSettings>().TopicsPerPage : this.UserData.TopicsPerPage.ToType<int>();
                string selected = "0";
                foreach (ListItem item in this.TopicsPerPageDDL.Items)
                {
                    var valueToCompare = item.Value.ToType<int>();

                    if (tpp.ToType<int>() > valueToCompare)
                    {
                        continue;
                    }

                    var postItem = this.TopicsPerPageDDL.Items.FindByValue(valueToCompare.ToString(CultureInfo.InvariantCulture));
                    if (postItem != null)
                    {
                        selected = valueToCompare.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
                }

                this.TopicsPerPageDDL.Items.FindByValue(selected).Selected = true;
            }

            if (this.PostsPerPageDDL.Items.Count > 0)
            {
                int tpp = this.UserData.PostsPerPage.ToType<int>() > this.Get<YafBoardSettings>().PostsPerPage ? this.Get<YafBoardSettings>().PostsPerPage : this.UserData.PostsPerPage.ToType<int>();
                string selected = "0";
                foreach (ListItem item in this.PostsPerPageDDL.Items)
                {
                    var valueToCompare = item.Value.ToType<int>();
                    if (tpp.ToType<int>() > valueToCompare)
                    {
                        continue;
                    }

                    var postItem = this.PostsPerPageDDL.Items.FindByValue(valueToCompare.ToString(CultureInfo.InvariantCulture));
                    if (postItem != null)
                    {
                        selected = valueToCompare.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
                }

                this.PostsPerPageDDL.Items.FindByValue(selected).Selected = true;
            }

            if (this.UserData.Profile.Region.IsSet())
            {
                var regionItem = this.Region.Items.FindByValue(this.UserData.Profile.Region.Trim());
                if (regionItem != null)
                {
                    regionItem.Selected = true;
                }
            }

            ListItem timeZoneItem = this.TimeZones.Items.FindByValue(this.UserData.TimeZone.ToString());
            if (timeZoneItem != null)
            {
                timeZoneItem.Selected = true;
            }

            this.DSTUser.Checked = this.UserData.DSTUser;
            this.HideMe.Checked = this.UserData.IsActiveExcluded &&
                                  (this.Get<YafBoardSettings>().AllowUserHideHimself || this.PageContext.IsAdmin);

            if ((this.Get<YafBoardSettings>().MobileTheme.IsSet() &&
                UserAgentHelper.IsMobileDevice(HttpContext.Current.Request.UserAgent)) ||
                HttpContext.Current.Request.Browser.IsMobileDevice)
            {
                this.UseMobileThemeRow.Visible = true;
                this.UseMobileTheme.Checked = this.UserData.UseMobileTheme;
            }

            if (this.Get<YafBoardSettings>().AllowUserTheme && this.Theme.Items.Count > 0)
            {
                // Allows to use different per-forum themes,
                // While "Allow User Change Theme" option in hostsettings is true
                string themeFile = this.Get<YafBoardSettings>().Theme;

                if (!string.IsNullOrEmpty(this.UserData.ThemeFile))
                {
                    themeFile = this.UserData.ThemeFile;
                }

                ListItem themeItem = this.Theme.Items.FindByValue(themeFile);
                if (themeItem != null)
                {
                    themeItem.Selected = true;
                }
            }

            if (this.Get<YafBoardSettings>().AllowUsersTextEditor && this.ForumEditor.Items.Count > 0)
            {
                // Text editor
                string textEditor = !string.IsNullOrEmpty(this.UserData.TextEditor)
                                        ? this.UserData.TextEditor
                                        : this.Get<YafBoardSettings>().ForumEditor;

                ListItem editorItem = this.ForumEditor.Items.FindByValue(textEditor);
                if (editorItem != null)
                {
                    editorItem.Selected = true;
                }
            }

            if (this.Get<YafBoardSettings>().AllowSingleSignOn)
            {
                this.SingleSignOn.Checked = this.UserData.UseSingleSignOn;
            }

            if (!this.Get<YafBoardSettings>().AllowUserLanguage || this.Culture.Items.Count <= 0)
            {
                return;
            }

            // If 2-letter language code is the same we return Culture, else we return a default full culture from language file
            ListItem foundCultItem = this.Culture.Items.FindByValue(currentCultureLocal);

            if (foundCultItem != null)
            {
                foundCultItem.Selected = true;
            }

            if (!Page.IsPostBack)
            {
                this.Realname.Focus();
            }
        }

        /// <summary>
        /// The send email verification.
        /// </summary>
        /// <param name="newEmail">
        /// The new email.
        /// </param>
        private void SendEmailVerification([NotNull] string newEmail)
        {
            string hashinput = DateTime.UtcNow + this.Email.Text + Security.CreatePassword(20);
            string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

            // Create Email
            var changeEmail = new YafTemplateEmail("CHANGEEMAIL");

            changeEmail.TemplateParams["{user}"] = this.PageContext.PageUserName;
            changeEmail.TemplateParams["{link}"] =
              "{0}\r\n\r\n".FormatWith(YafBuildLink.GetLinkNotEscaped(ForumPages.approve, true, "k={0}", hash));
            changeEmail.TemplateParams["{newemail}"] = this.Email.Text;
            changeEmail.TemplateParams["{key}"] = hash;
            changeEmail.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;
            changeEmail.TemplateParams["{forumlink}"] = YafForumInfo.ForumURL;

            // save a change email reference to the db
            CommonDb.checkemail_save(PageContext.PageModuleID, this._currentUserId, hash, newEmail);

            // send a change email message...
            changeEmail.SendEmail(
              new MailAddress(newEmail), this.GetText("COMMON", "CHANGEEMAIL_SUBJECT"), true);

            // show a confirmation
            this.PageContext.AddLoadMessage(
              this.GetText("PROFILE", "mail_sent").FormatWith(this.Email.Text));
        }

        /// <summary>
        /// The update user profile.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        private void UpdateUserProfile([NotNull] string userName)
        {
            YafUserProfile userProfile = YafUserProfile.GetProfile(userName);

            userProfile.Country = this.Country.SelectedItem != null ? this.Country.SelectedItem.Value.Trim() : string.Empty;
            userProfile.Region = this.Region.SelectedItem != null && this.Country.SelectedItem != null && this.Country.SelectedItem.Value.Trim().IsSet() ? this.Region.SelectedItem.Value.Trim() : string.Empty;
            userProfile.City = this.City.Text.Trim();
            userProfile.Location = this.Location.Text.Trim();
            userProfile.Homepage = this.HomePage.Text.Trim();
            userProfile.MSN = this.MSN.Text.Trim();
            userProfile.YIM = this.YIM.Text.Trim();
            userProfile.AIM = this.AIM.Text.Trim();
            userProfile.ICQ = this.ICQ.Text.Trim();
            userProfile.Facebook = this.Facebook.Text.Trim();
            userProfile.Twitter = this.Twitter.Text.Trim();
            userProfile.TwitterId = this.TwitterId.Text.Trim();
            userProfile.XMPP = this.Xmpp.Text.Trim();
            userProfile.Skype = this.Skype.Text.Trim();
            userProfile.RealName = this.Realname.Text.Trim();
            userProfile.Occupation = this.Occupation.Text.Trim();
            userProfile.Interests = this.Interests.Text.Trim();
            userProfile.Gender = this.Gender.SelectedIndex;
            userProfile.Blog = this.Weblog.Text.Trim();

          
                DateTime userBirthdate;
                var ci = CultureInfo.CreateSpecificCulture(this.GetCulture(true));

                if (this.Get<YafBoardSettings>().UseFarsiCalender && ci.IsFarsiCulture())
                {
                    var persianDate = new PersianDate(this.Birthday.Text);
                    userBirthdate = PersianDateConverter.ToGregorianDateTime(persianDate);

                    if (userBirthdate > DateTimeHelper.SqlDbMinTime().Date)
                    {
                        userProfile.Birthday = userBirthdate.Date;
                    }
                }
                else
                {
                    DateTime.TryParse(this.Birthday.Text, ci, DateTimeStyles.None, out userBirthdate);

                    if (userBirthdate > DateTimeHelper.SqlDbMinTime().Date)
                    {
                        // Attention! This is stored in profile in the user timezone date
                        userProfile.Birthday = userBirthdate.Date;
                    }
                }
            
            userProfile.BlogServiceUrl = this.WeblogUrl.Text.Trim();
            userProfile.BlogServiceUsername = this.WeblogUsername.Text.Trim();
            userProfile.BlogServicePassword = this.WeblogID.Text.Trim();
            
            // Sync to User Profile Mirror table while it's dirty
            SettingsPropertyValueCollection settingsPropertyValueCollection = userProfile.PropertyValues;
            CommonDb.SetPropertyValues(PageContext.PageModuleID, PageContext.PageBoardID, UserMembershipHelper.ApplicationName(), Constants.SpecialObjectNames.UserProfileMirrorTable, this._currentUserId, userProfile.UserName, settingsPropertyValueCollection);
           
            userProfile.Save();
        }

        /// <summary>
        /// Looks for new regions bind.
        /// </summary>
        /// <param name="country">The country.</param>
        private void LookForNewRegionsBind(string country)
        {
            var regionNames = StaticDataHelper.Region(country).OrderBy(s => s.Name);

            // The first row is empty
            if (regionNames.Count() > 1)
            {
                this.Region.DataSource = regionNames;
                this.Region.DataValueField = "Value";
                this.Region.DataTextField = "Name";
                this.RegionTr.Visible = true;
            }
            else
            {
                this.Region.DataSource = null;
                this.Region.DataBind();
                this.RegionTr.Visible = false;
                this.RegionTr.DataBind();
            }
        }

        #endregion


        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <returns>
        /// The get culture.
        /// </returns>
        private string GetCulture(bool overrideByPageUserCulture)
        {
            // Language and culture
            string languageFile = this.Get<YafBoardSettings>().Language;
            string culture4Tag = this.Get<YafBoardSettings>().Culture;
            if (overrideByPageUserCulture)
            {
                if (this.PageContext.CurrentUserData.LanguageFile.IsSet())
                {
                    languageFile = this.PageContext.CurrentUserData.LanguageFile;
                }

                if (this.PageContext.CurrentUserData.CultureUser.IsSet())
                {
                    culture4Tag = this.PageContext.CurrentUserData.CultureUser;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(this.UserData.LanguageFile))
                {
                    languageFile = this.UserData.LanguageFile;
                }

                if (!string.IsNullOrEmpty(this.UserData.CultureUser))
                {
                    culture4Tag = this.UserData.CultureUser;
                }
            }

            // Get first default full culture from a language file tag.
            string langFileCulture = StaticDataHelper.CultureDefaultFromFile(languageFile);
            return langFileCulture.Substring(0, 2) == culture4Tag.Substring(0, 2) ? culture4Tag : langFileCulture;
        }

        /// <summary>
        /// The create data source.
        /// </summary>
        /// <param name="maxNumber">
        /// The max number.
        /// </param>
        /// <returns>
        /// The <see cref="ICollection"/>.
        /// </returns>
        private ICollection CreateDataSourceForTopicsPostsDropDownLists(int maxNumber)
        {
            // Create a table to store data for the DropDownList control.
            var dt = new DataTable();

            // Define the columns of the table.
            dt.Columns.Add(new DataColumn("ID", typeof(int)));
            dt.Columns.Add(new DataColumn("Number", typeof(int)));

            int current = 5;
            while (current < maxNumber)
            {
                dt.Rows.Add(CreateRow(current, current, dt));
                current = current + 10;
              
            }

            dt.Rows.Add(CreateRow(maxNumber, maxNumber, dt));

            // Create a DataView from the DataTable to act as the data source
            // for the DropDownList control.
            var dv = new DataView(dt);
            return dv;
        }

        /// <summary>
        /// The create row.
        /// </summary>
        /// <param name="Text">
        /// The text.
        /// </param>
        /// <param name="Value">
        /// The value.
        /// </param>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The <see cref="DataRow"/>.
        /// </returns>
        private static DataRow CreateRow(int Text, int Value, DataTable dt)
        {
         var dr = dt.NewRow();
         dr[0] = Text;
         dr[1] = Value;
         return dr;
        }
    }
}