﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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

using System.Globalization;

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using VZF.Data.Common;

    using YAF.Classes;
    
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using VZF.Utils;
    using VZF.Utils.Helpers;
    using VZF.Types.Objects;

    #endregion

    /// <summary>
    /// The Board Settings Admin Page.
    /// </summary>
    public partial class boardsettings : AdminPage
    {
        #region Methods

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
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_BOARDSETTINGS", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_BOARDSETTINGS", "TITLE"));

            this.Save.Text = this.GetText("COMMON", "SAVE");

            // create list boxes by populating datasources from Data class
            var themeData = StaticDataHelper.Themes().Where(x => !x.IsMobile);

            if (themeData.Any())
            {
                this.Theme.DataSource = themeData;
                this.Theme.DataTextField = "Theme";
                this.Theme.DataValueField = "FileName";
            }

            var mobileThemeData = StaticDataHelper.Themes().Where(x => x.IsMobile);

            if (mobileThemeData.Any())
            {
                var mobileThemes = mobileThemeData.ToList();

                // Add Dummy Disabled Mobile Theme Item to allow disabling the Mobile Theme
                mobileThemes.Insert(0,new ForumTheme()
                    {
                        Theme = "[ {0} ]".FormatWith(this.GetText("ADMIN_COMMON", "DISABLED")),
                        FileName = string.Empty,
                        IsMobile = false
                    }
                    );      

                this.MobileTheme.DataSource = mobileThemes;
                this.MobileTheme.DataTextField = "Theme";
                this.MobileTheme.DataValueField = "FileName";
            }        

            this.Culture.DataSource = StaticDataHelper.Cultures().OrderBy(x => x.NativeName);
            this.Culture.DataValueField = "IetfLanguageTag";
            this.Culture.DataTextField = "NativeName";

            this.ShowTopic.DataSource = StaticDataHelper.TopicTimes();
            this.ShowTopic.DataTextField = "TopicText";
            this.ShowTopic.DataValueField = "TopicValue";

            this.FileExtensionAllow.DataSource = StaticDataHelper.AllowDisallow();
            this.FileExtensionAllow.DataTextField = "Text";
            this.FileExtensionAllow.DataValueField = "Value";

            this.JqueryUITheme.DataSource = StaticDataHelper.JqueryUIThemes();
            this.JqueryUITheme.DataTextField = "Theme";
            this.JqueryUITheme.DataValueField = "Theme";

            this.FancyTreeTheme.DataSource = StaticDataHelper.FancyTreeThemes();
            this.FancyTreeTheme.DataTextField = "Theme";
            this.FancyTreeTheme.DataValueField = "Theme";

            this.BindData();

            // bind poll group list
            var pollGroup =
                CommonDb.PollGroupList(PageContext.PageModuleID, this.PageContext.PageUserID, null, this.PageContext.PageBoardID).Distinct(
                    new AreEqualFunc<TypedPollGroup>((v1, v2) => v1.PollGroupID == v2.PollGroupID)).ToList();

            pollGroup.Insert(0, new TypedPollGroup(string.Empty, -1));

            // TODO: vzrus needs some work, will be in polls only until feature is debugged there.
            this.PollGroupListDropDown.Items.AddRange(
                pollGroup.Select(x => new ListItem(x.Question, x.PollGroupID.ToString())).ToArray());

            // population default notification setting options...
            var items = EnumHelper.EnumToDictionary<UserNotificationSetting>();

            if (!this.Get<YafBoardSettings>().AllowNotificationAllPostsAllTopics)
            {
                // remove it...
                items.Remove(UserNotificationSetting.AllTopics.ToInt());
            }

            var notificationItems =
                items.Select(
                    x => new ListItem(HtmlHelper.StripHtml(this.GetText("CP_SUBSCRIPTIONS", x.Value)), x.Key.ToString()))
                    .ToArray();

            this.DefaultNotificationSetting.Items.AddRange(notificationItems);

            SetSelectedOnList(ref this.JqueryUITheme, this.Get<YafBoardSettings>().JqueryUITheme);
            SetSelectedOnList(ref this.FancyTreeTheme, this.Get<YafBoardSettings>().FancyTreeTheme);

            // Get first default full culture from a language file tag.
            string langFileCulture = StaticDataHelper.CultureDefaultFromFile(this.Get<YafBoardSettings>().Language)
                                     ?? "en-US";

            SetSelectedOnList(ref this.Theme, this.Get<YafBoardSettings>().Theme);
            SetSelectedOnList(ref this.MobileTheme, this.Get<YafBoardSettings>().MobileTheme);

            // If 2-letter language code is the same we return Culture, else we return  a default full culture from language file
            /* SetSelectedOnList(
                ref this.Culture, 
                langFileCulture.Substring(0, 2) == this.Get<YafBoardSettings>().Culture
                  ? this.Get<YafBoardSettings>().Culture
                  : langFileCulture);*/
            SetSelectedOnList(ref this.Culture, this.Get<YafBoardSettings>().Culture);
            if (this.Culture.SelectedIndex == 0)
            {
                // If 2-letter language code is the same we return Culture, else we return  a default full culture from language file
                SetSelectedOnList(
                    ref this.Culture,
                    langFileCulture.Substring(0, 2) == this.Get<YafBoardSettings>().Culture
                        ? this.Get<YafBoardSettings>().Culture
                        : langFileCulture);
            }

            SetSelectedOnList(ref this.ShowTopic, this.Get<YafBoardSettings>().ShowTopicsDefault.ToString());
            SetSelectedOnList(
                ref this.FileExtensionAllow, this.Get<YafBoardSettings>().FileExtensionAreAllowed ? "0" : "1");

            SetSelectedOnList(
                ref this.DefaultNotificationSetting,
                this.Get<YafBoardSettings>().DefaultNotificationSetting.ToInt().ToString());

            this.NotificationOnUserRegisterEmailList.Text =
                this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList;
            this.AllowThemedLogo.Checked = this.Get<YafBoardSettings>().AllowThemedLogo;
            this.EmailModeratorsOnModeratedPost.Checked = this.Get<YafBoardSettings>().EmailModeratorsOnModeratedPost;
            this.EmailModeratorsOnReportedPost.Checked = this.Get<YafBoardSettings>().EmailModeratorsOnReportedPost;
            this.AllowDigestEmail.Checked = this.Get<YafBoardSettings>().AllowDigestEmail;
            this.DefaultSendDigestEmail.Checked = this.Get<YafBoardSettings>().DefaultSendDigestEmail;
            this.JqueryUIThemeCDNHosted.Checked = this.Get<YafBoardSettings>().JqueryUIThemeCDNHosted;
            this.ForumEmail.Text = this.Get<YafBoardSettings>().ForumEmail;
            this.PersonalForumsInCategories.Checked = this.Get<YafBoardSettings>().AllowPersonalForumsInCategories;
            this.PersonalForumsAsSubForums.Checked = this.Get<YafBoardSettings>().AllowPersonalForumsAsSubForums;
            this.AllowPersonalMasksOnlyForPersonalForums.Checked = this.Get<YafBoardSettings>().AllowPersonalMasksOnlyForPersonalForums;
            this.AllowPersonalGroupsOnlyForPersonalForums.Checked = this.Get<YafBoardSettings>().AllowPersonalGroupsOnlyForPersonalForums;
            
            this.CopyrightRemovalKey.Text = this.Get<YafBoardSettings>().CopyrightRemovalDomainKey;

            this.DigestSendEveryXHours.Text = this.Get<YafBoardSettings>().DigestSendEveryXHours.ToString(CultureInfo.InvariantCulture);

            if (this.Get<YafBoardSettings>().BoardPollID > 0)
            {
                this.PollGroupListDropDown.SelectedValue = this.Get<YafBoardSettings>().BoardPollID.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                this.PollGroupListDropDown.SelectedIndex = 0;
            }

            this.PollGroupList.Visible = true;

            // Copyright Linkback Algorithm
            // Please keep if you haven't purchased a removal or commercial license.
            this.CopyrightHolder.Visible = true;
        }

        /// <summary>
        /// Save the Board Settings
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // english.xml by default
            string languageFile = 
                StaticDataHelper.Cultures().Where(
                    c => c.IetfLanguageTag.Equals(this.Culture.SelectedValue)).FirstOrDefault().CultureFile;
           
            CommonDb.board_save(PageContext.PageModuleID, this.PageContext.PageBoardID,
                languageFile,
                this.Culture.SelectedValue,
                this.Name.Text,
                this.AllowThreaded.Checked);

            // save poll group
            this.Get<YafBoardSettings>().BoardPollID = this.PollGroupListDropDown.SelectedIndex.ToType<int>() > 0
                                                           ? this.PollGroupListDropDown.SelectedValue.ToType<int>()
                                                           : 0;

            this.Get<YafBoardSettings>().Language = languageFile;
            this.Get<YafBoardSettings>().Culture = this.Culture.SelectedValue;
            this.Get<YafBoardSettings>().Theme = this.Theme.SelectedValue;

            // allow null/empty as a mobile theme many not be desired.
            this.Get<YafBoardSettings>().MobileTheme = this.MobileTheme.SelectedValue ?? string.Empty;

            this.Get<YafBoardSettings>().ShowTopicsDefault = this.ShowTopic.SelectedValue.ToType<int>();
            this.Get<YafBoardSettings>().AllowThemedLogo = this.AllowThemedLogo.Checked;
            this.Get<YafBoardSettings>().FileExtensionAreAllowed = this.FileExtensionAllow.SelectedValue.ToType<int>()
                                                                   == 0;
            this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList =
                this.NotificationOnUserRegisterEmailList.Text.Trim();

            this.Get<YafBoardSettings>().EmailModeratorsOnModeratedPost = this.EmailModeratorsOnModeratedPost.Checked;
            this.Get<YafBoardSettings>().EmailModeratorsOnReportedPost = this.EmailModeratorsOnReportedPost.Checked;
            this.Get<YafBoardSettings>().AllowDigestEmail = this.AllowDigestEmail.Checked;

            this.Get<YafBoardSettings>().AllowPersonalForumsAsSubForums = this.PersonalForumsAsSubForums.Checked;
            this.Get<YafBoardSettings>().AllowPersonalForumsInCategories = this.PersonalForumsInCategories.Checked;
            this.Get<YafBoardSettings>().AllowPersonalMasksOnlyForPersonalForums = this.AllowPersonalMasksOnlyForPersonalForums.Checked;
            this.Get<YafBoardSettings>().AllowPersonalGroupsOnlyForPersonalForums = this.AllowPersonalGroupsOnlyForPersonalForums.Checked;

            this.Get<YafBoardSettings>().DefaultSendDigestEmail = this.DefaultSendDigestEmail.Checked;
            this.Get<YafBoardSettings>().DefaultNotificationSetting =
                this.DefaultNotificationSetting.SelectedValue.ToEnum<UserNotificationSetting>();

            this.Get<YafBoardSettings>().ForumEmail = this.ForumEmail.Text;
            this.Get<YafBoardSettings>().CopyrightRemovalDomainKey = this.CopyrightRemovalKey.Text.Trim();
            this.Get<YafBoardSettings>().JqueryUITheme = this.JqueryUITheme.SelectedValue;
            this.Get<YafBoardSettings>().FancyTreeTheme = this.FancyTreeTheme.SelectedValue;
            this.Get<YafBoardSettings>().JqueryUIThemeCDNHosted = this.JqueryUIThemeCDNHosted.Checked;

            int hours;

            if (!int.TryParse(this.DigestSendEveryXHours.Text, out hours))
            {
                hours = 24;
            }

            this.Get<YafBoardSettings>().DigestSendEveryXHours = hours;

            // save the settings to the database
            ((YafLoadBoardSettings)this.Get<YafBoardSettings>()).SaveRegistry();

            // Reload forum settings
            this.PageContext.BoardSettings = null;

            // Clearing cache with old users permissions data to get new default styles...
            this.Get<IDataCache>().Remove(x => x.StartsWith(Constants.Cache.ActiveUserLazyData));
            YafBuildLink.Redirect(ForumPages.admin_admin);
        }

        /// <summary>
        /// The set selected on list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        private static void SetSelectedOnList([NotNull] ref DropDownList list, [NotNull] string value)
        {
            ListItem selItem = list.Items.FindByValue(value);

            if (selItem != null)
            {
                selItem.Selected = true;
            }
            else if (list.Items.Count > 0)
            {
                // select the first...
                list.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            DataRow row;
            using (DataTable dt = CommonDb.board_list(PageContext.PageModuleID, this.PageContext.PageBoardID))
            {
                row = dt.Rows[0];
            }

            this.DataBind();
            this.Name.Text = row["Name"].ToString();
            this.AllowThreaded.Checked = Convert.ToBoolean(row["AllowThreaded"]);
        }

        #endregion
    }
}