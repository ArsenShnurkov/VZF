﻿namespace VZF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Text;
    using System.Web.UI.HtmlControls;

    using VZF.Data.Common;

    using YAF.Classes;
    
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using VZF.Utils;
    using VZF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The forum statistics.
    /// </summary>
    public partial class ForumStatistics : BaseUserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ForumStatistics" /> class.
        /// </summary>
        public ForumStatistics()
        {
            this.Load += this.ForumStatistics_Load;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The format active users.
        /// </summary>
        /// <param name="activeStats">
        /// The active stats.
        /// </param>
        /// <returns>
        /// Returns the formated active users.
        /// </returns>
        [NotNull]
        protected string FormatActiveUsers([NotNull] DataRow activeStats)
        {
            var sb = new StringBuilder();

            int activeUsers = activeStats["ActiveUsers"].ToType<int>();
            int activeHidden = activeStats["ActiveHidden"].ToType<int>();
            int activeMembers = activeStats["ActiveMembers"].ToType<int>();
            int activeGuests = activeStats["ActiveGuests"].ToType<int>();

            // show hidden count to admin...
            if (this.PageContext.IsAdmin)
            {
                activeUsers += activeHidden;
            }

            bool canViewActive = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ActiveUsersViewPermissions);
            bool showGuestTotal = (activeGuests > 0) &&
                                  (this.Get<YafBoardSettings>().ShowGuestsInDetailedActiveList ||
                                   this.Get<YafBoardSettings>().ShowCrawlersInActiveList);
            bool showActiveHidden = (activeHidden > 0) && this.PageContext.IsAdmin;
            if (canViewActive &&
                (showGuestTotal || (activeMembers > 0 && (showGuestTotal || activeGuests <= 0)) ||
                 (showActiveHidden && (activeMembers > 0) && showGuestTotal)))
            {
                // always show active users...       
                sb.Append(
                    "<a href=\"{1}\" title=\"{2}\">{0}</a>".FormatWith(
                        this.GetTextFormatted(
                            activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2", activeUsers),
                        YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 0),
                        this.GetText("COMMON", "VIEW_FULLINFO"),
                        PageContext.IsCrawler ? " rel=\"nofolow\"" : string.Empty));
            }
            else
            {
                // no link because no permissions...
                sb.Append(
                  this.GetTextFormatted(
                    activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2", activeUsers));
            }

            if (activeMembers > 0)
            {
                sb.Append(
                    canViewActive
                        ? ", <a href=\"{1}\" title=\"{2}\"{3}>{0}</a>".FormatWith(
                            this.GetTextFormatted(
                                activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers),
                            YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 1),
                            this.GetText("COMMON", "VIEW_FULLINFO"),
                            PageContext.IsCrawler ? " rel=\"nofolow\"" : string.Empty)
                        : ", {0}".FormatWith(
                            this.GetTextFormatted(
                                activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers)));
            }

            if (activeGuests > 0)
            {
                if (canViewActive &&
                    (this.Get<YafBoardSettings>().ShowGuestsInDetailedActiveList ||
                     this.Get<YafBoardSettings>().ShowCrawlersInActiveList))
                {
                    sb.Append(
                        ", <a href=\"{1}\" title=\"{2}\"{3}>{0}</a>".FormatWith(
                            this.GetTextFormatted(
                                activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests),
                            YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 2),
                            this.GetText("COMMON", "VIEW_FULLINFO"),
                            PageContext.IsCrawler ? " rel=\"nofolow\"" : string.Empty));
                }
                else
                {
                    sb.Append(
                      ", {0}".FormatWith(
                        this.GetTextFormatted(
                          activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests)));
                }
            }

            if (activeHidden > 0 && this.PageContext.IsAdmin)
            {
                // vzrus: was temporary left as is, only admins can view hidden users online, why not everyone?
                if (activeHidden > 0 && this.PageContext.IsAdmin)
                {
                    sb.Append(
                      ", <a href=\"{1}\" title=\"{2}\">{0}</a>".FormatWith(
                        this.GetTextFormatted("ACTIVE_USERS_HIDDEN", activeHidden),
                        YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 3),
                        this.GetText("COMMON", "VIEW_FULLINFO")));
                }
                else
                {
                    sb.Append(
                      ", {0}".FormatWith(this.GetTextFormatted("ACTIVE_USERS_HIDDEN", activeHidden)));
                }
            }

            sb.Append(
              " {0}".FormatWith(
                this.GetTextFormatted(
                  "ACTIVE_USERS_TIME", this.Get<YafBoardSettings>().ActiveListTime)));

            return sb.ToString();
        }



        /// <summary>
        /// Gets the todays birthdays.
        /// </summary>
        /// TODO: Make DST shift for the user
        private void GetTodaysBirthdays()
        {
            if (!this.Get<YafBoardSettings>().ShowTodaysBirthdays)
            {
                return;
            }

            this.StatsTodaysBirthdays.Text = this.GetText("stats_birthdays");

            var users = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.TodaysBirthdays,
                () => CommonDb.User_ListTodaysBirthdays(PageContext.PageModuleID, this.PageContext.PageBoardID, this.Get<YafBoardSettings>().UseStyledNicks),
                TimeSpan.FromHours(1));

            if (users == null || users.Rows.Count <= 0)
            {
                return;
            }

            foreach (DataRow user in users.Rows)
            {
                DateTime birth;

                if (!DateTime.TryParse(user["Birthday"].ToString(), out birth))
                {
                    continue;
                }

                int tz;

                if (!int.TryParse(user["TimeZone"].ToString(), out tz))
                {
                    tz = 0;
                }
 
                // Get the user birhday based on his timezone date.
                var dtt = birth.AddYears(DateTime.UtcNow.Year - birth.Year);
              
                // The user can be congratulated. The time zone in profile is saved in the list user timezone
                if (DateTime.UtcNow <= dtt.AddMinutes(-tz).ToUniversalTime()
                    || DateTime.UtcNow >= dtt.AddMinutes(-tz + 1440).ToUniversalTime())
                {
                    continue;
                }

                this.BirthdayUsers.Controls.Add(
                    new UserLink
                    {
                        UserID = (int) user["UserID"],
                        ReplaceName = this.Get<YafBoardSettings>().EnableDisplayName
                                              ? user["UserDisplayName"].ToString()
                                              : user["UserName"].ToString(),
                        Style = this.Get<IStyleTransform>().DecodeStyleByString(user["Style"].ToString(), false),
                        PostfixText =  " ({0})".FormatWith(DateTime.Now.Year - user["Birthday"].ToType<DateTime>().Year)
                        });

                var separator = new HtmlGenericControl {InnerHtml = "&nbsp;,&nbsp;"};

                this.BirthdayUsers.Controls.Add(separator);
                if (!this.BirthdayUsers.Visible)
                {
                    this.BirthdayUsers.Visible = true;
                }
            }

            if (this.BirthdayUsers.Visible)
            {
                // Remove last Separator
                this.BirthdayUsers.Controls.RemoveAt(this.BirthdayUsers.Controls.Count - 1);
            }
        }

        /// <summary>
        /// The forum statistics_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ForumStatistics_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Active users : Call this before forum_stats to clean up active users
            var activeUsers = this.Get<IDataCache>().GetOrSet(
              Constants.Cache.UsersOnlineStatus,
              () => this.Get<IDBBroker>().GetActiveList(false, this.Get<YafBoardSettings>().ShowCrawlersInActiveList),
              TimeSpan.FromMilliseconds(this.Get<YafBoardSettings>().OnlineStatusCacheTimeout));

            this.ActiveUsers1.ActiveUserTable = activeUsers;

            // "Active Users" Count and Most Users Count 
            DataRow activeStats = CommonDb.active_stats(PageContext.PageModuleID, this.PageContext.PageBoardID);
            this.ActiveUserCount.Text = this.FormatActiveUsers(activeStats);
            int daysMostActive = this.Get<YafBoardSettings>().MostActiveUserDays;
            if (daysMostActive > 1)
            {
                this.MosActiveForLink.Text = this.GetTextFormatted(
                    "MOSTACTIVEUSERS_FOR_LINK", this.Get<YafBoardSettings>().MostActiveUserDays);
            }

            if (daysMostActive <= 0)
            {
                this.MosActiveForLink.Visible = false;
            }
            else
            {
                if (daysMostActive == 1)
                {
                    this.MosActiveForLink.Text = this.GetText(
                        "MOSTACTIVEUSERS_FORTODAY_LINK");
                }
            }

            if (this.Get<YafBoardSettings>().ShowBoardTags && this.Get<YafBoardSettings>().BoardTagsPerPage > 0)
            {
                this.BoardTagsLink.Visible = this.Get<YafBoardSettings>().TagTopicMaxCount > 0;
                this.BoardTagsLink.Text = this.GetText("TAGSBOARD", "TAGS_BOARD_LNK");
            }

            // Tommy MOD "Recent Users" Count.
            if (this.Get<YafBoardSettings>().ShowRecentUsers)
            {
                var activeUsers30Day = this.Get<IDataCache>().GetOrSet(
                    Constants.Cache.VisitorsInTheLast30Days,
                    () => this.Get<IDBBroker>().GetRecentUsers(60 * 24 * 30),
                    TimeSpan.FromMinutes(this.Get<YafBoardSettings>().ForumStatisticsCacheTimeout));
                if (activeUsers30Day != null && activeUsers30Day.Rows.Count > 0)
                {
                    var activeUsers1Day1 =
                        activeUsers30Day.Select("LastVisit >= '{0}'".FormatWith(DateTime.UtcNow.AddDays(-1)));
                    this.RecentUsersCount.Text = this.GetTextFormatted(
                        "RECENT_ONLINE_USERS", activeUsers1Day1.Length, activeUsers30Day.Rows.Count);
                    this.RecentUsersPlaceHolder.Visible = true;
                    if (activeUsers1Day1.Length > 0)
                    {
                        this.RecentUsers.ActiveUserTable = activeUsers1Day1.CopyToDataTable();
                        this.RecentUsers.Visible = true;
                    }
                }
            }
            else
            {
                this.RecentUsersPlaceHolder.Visible = false;
            }

            // Forum Statistics
            var postsStatisticsDataRow = this.Get<IDataCache>().GetOrSet(
              Constants.Cache.BoardStats,
              () =>
              {
                  // get the post stats
                  var dr = CommonDb.board_poststats(PageContext.PageModuleID, this.PageContext.PageBoardID, this.Get<YafBoardSettings>().UseStyledNicks, true);
              
                  // Set colorOnly parameter to false, as we get here color from data field in the place
                  dr.LastUserStyle = this.Get<YafBoardSettings>().UseStyledNicks
                                          ? this.Get<IStyleTransform>().DecodeStyleByString(
                                          dr.LastUserStyle ?? string.Empty, false)
                                          : null;
                  return dr;
              },
              TimeSpan.FromMinutes(this.Get<YafBoardSettings>().ForumStatisticsCacheTimeout));

            // Forum Statistics
            var userStatisticsDataRow = this.Get<IDataCache>().GetOrSet(
              Constants.Cache.BoardUserStats,
              () => CommonDb.board_userstats(PageContext.PageModuleID, this.PageContext.PageBoardID).Table,
              TimeSpan.FromMinutes(this.Get<YafBoardSettings>().BoardUserStatsCacheTimeout)).Rows[0];

            // show max users...
            if (!userStatisticsDataRow.IsNull("MaxUsers"))
            {
                this.MostUsersCount.Text = this.GetTextFormatted(
                  "MAX_ONLINE",
                  userStatisticsDataRow["MaxUsers"],
                  this.Get<IDateTime>().FormatDateTimeTopic(userStatisticsDataRow["MaxUsersWhen"]));
            }
            else
            {
                this.MostUsersCount.Text = this.GetTextFormatted(
                  "MAX_ONLINE", activeStats["ActiveUsers"], this.Get<IDateTime>().FormatDateTimeTopic(DateTime.UtcNow));
            }

            // Posts and Topic Count...
            this.StatsPostsTopicCount.Text = this.GetTextFormatted(
              "stats_posts",
              postsStatisticsDataRow.Posts,
              postsStatisticsDataRow.Topics,
              postsStatisticsDataRow.Forums);

            // Last post
            if (postsStatisticsDataRow.LastPost != null)
            {
                this.StatsLastPostHolder.Visible = true;

                this.LastPostUserLink.UserID = postsStatisticsDataRow.LastUserID ?? 0;
                this.LastPostUserLink.ReplaceName = this.Get<YafBoardSettings>().EnableDisplayName
                                                        ? postsStatisticsDataRow.LastUserDisplayName
                                                        : postsStatisticsDataRow.LastUser;
                this.LastPostUserLink.Style = postsStatisticsDataRow.LastUserStyle;
                this.StatsLastPost.Text = this.GetTextFormatted(
                    "stats_lastpost",
                    new DisplayDateTime
                        {
                            DateTime = postsStatisticsDataRow.LastPost,
                            Format = DateTimeFormat.BothTopic
                        }.RenderToString());
            }
            else
            {
                this.StatsLastPostHolder.Visible = false;
            }

            // Member Count
            this.StatsMembersCount.Text = this.GetTextFormatted(
              "stats_members", userStatisticsDataRow["members"]);

            // Newest Member
            this.StatsNewestMember.Text = this.GetText("stats_lastmember");
            this.NewestMemberUserLink.UserID = userStatisticsDataRow["LastMemberID"].ToType<int>();
            this.NewestMemberUserLink.ReplaceName = this.Get<YafBoardSettings>().EnableDisplayName
                                                        ? userStatisticsDataRow["LastMemberDisplayName"].ToString()
                                                        : userStatisticsDataRow["LastMember"].ToString();
            this.CollapsibleImage.ToolTip = this.GetText("COMMON", "SHOWHIDE");

            this.GetTodaysBirthdays();
            this.ForumStatsImage.NavigateUrl = YafBuildLink.GetLink(ForumPages.mostactiveusers);
        }

        /// <summary>
        /// The mos active for link_ click.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void MosActiveForLink_Click(object o, EventArgs args)
        {
            Response.Redirect(YafBuildLink.GetLink(ForumPages.mostactiveusers));
        }

        /// <summary>
        /// The board tags link_ click.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void BoardTagsLink_Click(object o, EventArgs args)
        {
            Response.Redirect(YafBuildLink.GetLink(ForumPages.boardtags));
        }

        #endregion
    }
}