namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Data;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;

  using VZF.Data.Common;

  using YAF.Classes;
  
  using YAF.Core;
  using YAF.Core.BBCode;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using VZF.Utils;

  #endregion

  /// <summary>
  /// Summary description for smilies_edit.
  /// </summary>
  public partial class smilies_edit : AdminPage
  {
    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      this.save.Click += this.save_Click;
      this.cancel.Click += this.cancel_Click;

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      this.InitializeComponent();
      base.OnInit(e);
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

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(this.GetText("ADMIN_SMILIES", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_smilies));
        this.PageLinks.AddLink(this.GetText("ADMIN_SMILIES_EDIT", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
              this.GetText("ADMIN_ADMIN", "Administration"),
              this.GetText("ADMIN_SMILIES", "TITLE"),
              this.GetText("ADMIN_SMILIES_EDIT", "TITLE"));

        this.cancel.Text = this.GetText("COMMON", "CANCEL");
        this.save.Text = this.GetText("COMMON", "SAVE");

        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      using (var dt = new DataTable("Files"))
      {
        dt.Columns.Add("FileID", typeof(long));
        dt.Columns.Add("FileName", typeof(string));
        dt.Columns.Add("Description", typeof(string));
        DataRow dr = dt.NewRow();
        dr["FileID"] = 0;
        dr["FileName"] = "../spacer.gif"; // use blank.gif for Description Entry
        dr["Description"] = this.GetText("ADMIN_SMILIES_EDIT", "SELECT_SMILEY");
        dt.Rows.Add(dr);

        var dir =
          new DirectoryInfo(
            this.Request.MapPath(
              "{0}{1}".FormatWith(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Emoticons)));
        FileInfo[] files = dir.GetFiles("*.*");
        long nFileID = 1;

        foreach (FileInfo file in from file in files
                                  let sExt = file.Extension.ToLower()
                                  where sExt == ".png" || sExt == ".gif" || sExt == ".jpg"
                                  select file)
        {
            dr = dt.NewRow();
            dr["FileID"] = nFileID++;
            dr["FileName"] = file.Name;
            dr["Description"] = file.Name;
            dt.Rows.Add(dr);
        }

        this.Icon.DataSource = dt;
        this.Icon.DataValueField = "FileName";
        this.Icon.DataTextField = "Description";
      }

      this.DataBind();

      if (this.Request["s"] != null)
      {
        using (
          DataTable dt = CommonDb.smiley_list(PageContext.PageModuleID, this.PageContext.PageBoardID, this.Request.QueryString.GetFirstOrDefault("s")))
        {
          if (dt.Rows.Count > 0)
          {
            this.Code.Text = dt.Rows[0]["Code"].ToString();
            this.Emotion.Text = dt.Rows[0]["Emoticon"].ToString();
            if (this.Icon.Items.FindByText(dt.Rows[0]["Icon"].ToString()) != null)
            {
              this.Icon.Items.FindByText(dt.Rows[0]["Icon"].ToString()).Selected = true;
            }

            this.Preview.Src = "{0}{1}/{2}".FormatWith(
              YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons, dt.Rows[0]["Icon"]);
            this.SortOrder.Text = dt.Rows[0]["SortOrder"].ToString(); // Ederon : 9/4/2007
          }
        }
      }
      else
      {
        this.Preview.Src = "{0}images/spacer.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
      }

      this.Icon.Attributes["onchange"] =
        "getElementById('{2}').src='{0}{1}/' + this.value".FormatWith(
          YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons, this.Preview.ClientID);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
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
    private void cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_smilies);
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
    private void save_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      string code = this.Code.Text.Trim();
      string emotion = this.Emotion.Text.Trim();
      string icon = this.Icon.SelectedItem.Text.Trim();
      int sortOrder;

      if (emotion.Length > 50)
      {
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_SMILIES_EDIT", "MSG_TOO_LONG"));
        return;
      }

      if (code.Length == 0)
      {
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_SMILIES_EDIT", "MSG_CODE_MISSING"));
        return;
      }

      if (code.Length > 10)
      {
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_SMILIES_EDIT", "MSG_CODE_LONG"));
        return;
      }

      if (!new Regex(@"\[.+\]").IsMatch(code))
      {
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_SMILIES_EDIT", "MSG_CODE_BRACK"));
        return;
      }

      if (emotion.Length == 0)
      {
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_SMILIES_EDIT", "MSG_NO_EMOTICON"));
        return;
      }

      if (this.Icon.SelectedIndex < 1)
      {
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_SMILIES_EDIT", "MSG_NO_ICON"));
        return;
      }

      // Ederon 9/4/2007
      if (!int.TryParse(this.SortOrder.Text, out sortOrder) || sortOrder < 0 || sortOrder > 255)
      {
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_SMILIES_EDIT", "MSG_SORT_NMBR"));
        return;
      }

      CommonDb.smiley_save(PageContext.PageModuleID, this.Request.QueryString.GetFirstOrDefault("s"), this.PageContext.PageBoardID, code, icon, emotion, sortOrder, false);

      // invalidate the cache...
      this.Get<IDataCache>().Remove(Constants.Cache.Smilies);
      this.Get<IObjectStore>().RemoveOf<IProcessReplaceRules>();

      YafBuildLink.Redirect(ForumPages.admin_smilies);
    }

    #endregion
  }
}