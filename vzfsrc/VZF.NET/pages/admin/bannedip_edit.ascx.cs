namespace YAF.Pages.Admin
{
  #region Using

    using System;
    using System.Data;
    using System.Text;

    using VZF.Data.Common;

    using YAF.Classes;
    
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using VZF.Utils;

    #endregion

  /// <summary>
  /// Admin Banned ip edit/add page.
  /// </summary>
  public partial class bannedip_edit : AdminPage
  {
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
      YafBuildLink.Redirect(ForumPages.admin_bannedip);
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

        this.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_bannedip));

        // current page label (no link)
        this.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP_EDIT", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
           this.GetText("ADMIN_ADMIN", "Administration"),
           this.GetText("ADMIN_BANNEDIP", "TITLE"),
           this.GetText("ADMIN_BANNEDIP_EDIT", "TITLE"));

        this.save.Text = this.GetText("COMMON", "SAVE");
        this.cancel.Text = this.GetText("COMMON", "CANCEL");

        this.BindData();
    }

    /// <summary>
    /// The save_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
            // IPv4
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.mask.Text.Trim(), @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                // IPv4 CIDR Range
                if (!System.Text.RegularExpressions.Regex.IsMatch(this.mask.Text.Trim(), @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(\/([0-9]|[1-2][0-9]|3[0-2]))$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    // leave it for coptability, checks for * in address for ranges
                    string[] ipParts = this.mask.Text.Trim().Split('.');

                    // do some validation...
                    var ipError = new StringBuilder();

                    if (ipParts.Length != 4)
                    {
                        ipError.AppendLine(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_ADRESS"));
                    }

                    foreach (string ip in ipParts)
                    {
                        // see if they are numbers...
                        ulong number;
                        if (!ulong.TryParse(ip, out number))
                        {
                            if (ip.Trim() != "*")
                            {
                                if (ip.Trim().Length == 0)
                                {
                                    ipError.AppendLine(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_VALUE"));
                                }
                                else
                                {
                                    ipError.AppendFormat(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_SECTION"), ip);
                                }

                                break;
                            }
                        }
                        else
                        {
                            // try parse succeeded... verify number amount...
                            if (number > 255)
                            {
                                ipError.AppendFormat(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_LESS"), ip);
                            }
                        }
                    }

                    // show error(s) if not valid...
                    if (ipError.Length > 0)
                    {
                        this.PageContext.AddLoadMessage(ipError.ToString());
                        return;
                    }
                }
            }
           
            // IPv6 CIDR Range
            // System.Text.RegularExpressions.Regex.Matches(this.mask.Text.Trim(), @"^s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:)))(%.+)?s*", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            // IPv6 CIDR Range
            // System.Text.RegularExpressions.Regex.Matches(this.mask.Text.Trim(), @"^s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:)))(%.+)?s*(\/(d|dd|1[0-1]d|12[0-8]))$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

      CommonDb.bannedip_save(PageContext.PageModuleID, this.Request.QueryString.GetFirstOrDefault("i"), 
        this.PageContext.PageBoardID, 
        this.mask.Text.Trim(), 
        this.BanReason.Text.Trim(), 
        this.PageContext.PageUserID);
        this.Get<ILogger>().IpBanSet(this.PageContext.PageUserID, "YAF.Pages.Admin.bannedip_edit", "IP or mask {0} was saved by {1}.".FormatWith(this.mask.Text.Trim(), this.Get<YafBoardSettings>().EnableDisplayName ? this.PageContext.CurrentUserData.DisplayName : this.PageContext.CurrentUserData.UserName));
    
      // clear cache of banned IPs for this board
      this.Get<IDataCache>().Remove(Constants.Cache.BannedIP);
      
      // go back to banned IP's administration page
      YafBuildLink.Redirect(ForumPages.admin_bannedip);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        if (this.Request.QueryString.GetFirstOrDefault("i") == null)
        {
            return;
        }

        DataRow row =
            CommonDb.bannedip_list(PageContext.PageModuleID, this.PageContext.PageBoardID, this.Request.QueryString.GetFirstOrDefault("i"), null, null).Rows[0];

        this.mask.Text = row["Mask"].ToString();
        this.BanReason.Text = row["Reason"].ToString();
    }

    #endregion
  }
}