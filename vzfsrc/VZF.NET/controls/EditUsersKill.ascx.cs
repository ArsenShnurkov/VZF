namespace VZF.Controls
{
  #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
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
  /// The edit users kill.
  /// </summary>
  public partial class EditUsersKill : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _all posts by user.
    /// </summary>
    private DataTable _allPostsByUser;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets AllPostsByUser.
    /// </summary>
    public DataTable AllPostsByUser
    {
      get
      {
          return this._allPostsByUser ??
                 (this._allPostsByUser =
                  CommonDb.post_alluser(PageContext.PageModuleID, this.PageContext.PageBoardID, this.CurrentUserID, this.PageContext.PageUserID, null));
      }
    }

    /// <summary>
    ///   Gets IPAddresses.
    /// </summary>
    [NotNull]
    public List<string> IPAddresses
    {
      get
      {
        return this.AllPostsByUser.GetColumnAsList<string>("IP").OrderBy(x => x).Distinct().ToList();
      }
    }

    /// <summary>
    ///   Gets CurrentUserID.
    /// </summary>
    protected long? CurrentUserID
    {
      get
      {
        return this.PageContext.QueryIDs["u"];
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The kill_ on click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Kill_OnClick([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.BanIps.Checked)
      {
        this.BanUserIps();
      }

      this.DeletePosts();

      MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID);
      this.PageContext.AddLoadMessage(this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_USER_KILLED").FormatWith(user.UserName));

      // update the displayed data...
      this.BindData();
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
      // init ids...
      this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

      // this needs to be done just once, not during postbacks
        if (this.IsPostBack)
        {
            return;
        }

        MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID);
        var userData = new CombinedUserDataHelper(user, (int)this.CurrentUserID.Value);

        this.ViewPostsLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
            ForumPages.search, 
            "postedby={0}&u={1}", 
            !userData.IsGuest ?  (this.Get<YafBoardSettings>().EnableDisplayName ? userData.DisplayName : userData.UserName)
            : (UserMembershipHelper.GuestUserName), userData.UserID);

        // bind data
        this.BindData();
    }

    /// <summary>
    /// The ban user ips.
    /// </summary>
    private void BanUserIps()
    {

      var ips = this.IPAddresses;
      var allIps = CommonDb.bannedip_list(PageContext.PageModuleID, this.PageContext.PageBoardID, null, 0, 1000000).GetColumnAsList<string>("Mask").ToList();

      // remove all IPs from ips if they already exist in allIps...
      ips.RemoveAll(allIps.Contains);

      // ban user ips...
      string name = UserMembershipHelper.GetDisplayNameFromID(this.CurrentUserID == null ? -1 : (int)this.CurrentUserID);

      if (string.IsNullOrEmpty(name))
      {
        name = UserMembershipHelper.GetUserNameFromID(this.CurrentUserID == null ? -1 : (int)this.CurrentUserID);
      }

      this.IPAddresses.ForEach(
        x =>
        CommonDb.bannedip_save(PageContext.PageModuleID, null, 
          this.PageContext.PageBoardID, 
          x, 
          this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "LINK_USER_BAN").FormatWith(
              this.CurrentUserID, 
              YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.CurrentUserID), 
              this.HtmlEncode(name)), 
          this.PageContext.PageUserID));

      // clear cache of banned IPs for this board
      this.Get<IDataCache>().Remove(Constants.Cache.BannedIP);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      // load ip address history for user...
      this.IpAddresses.Text = this.IPAddresses.ToDelimitedString("<br />");

      // show post count...
      this.PostCount.Text = this.AllPostsByUser.Rows.Count.ToString();

      this.DataBind();
    }

    /// <summary>
    /// The delete posts.
    /// </summary>
    private void DeletePosts()
    {
      // delete posts...
      var messageIds =
        (from m in this.AllPostsByUser.AsEnumerable() select m.Field<int>("MessageID")).Distinct().ToList();

      messageIds.ForEach(x => CommonDb.message_delete(PageContext.PageModuleID, x, true, string.Empty, 1, true));
    }

    #endregion
  }
}