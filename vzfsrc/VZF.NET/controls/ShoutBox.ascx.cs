﻿/******************************************************************************************************
//  Original code by: DLESKTECH at http://www.dlesktech.com/support.aspx
//  Modifications by: KASL Technologies at www.kasltechnologies.com
//  Mod date:7/21/2009
//  Mods: working smileys, moved smilies to bottom, added clear button for admin, new stored procedure
//  Mods: fixed the time to show the viewers time not the server time
//  Mods: added small chat window popup that runs separately from forum
//  Note: flyout button opens smaller chat window
//  Note: clear button removes message more than 24hrs old from db
 */

using VZF.Utils.Helpers;

namespace VZF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;

    using VZF.Data.Common;
    using VZF.Utils;

    using YAF;
    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The shout box.
    /// </summary>
    public partial class ShoutBox : BaseUserControl
    {
        #region Constructors and Destructors

        #endregion

        #region Properties

        /// <summary>
        /// Gets ShoutBoxMessages.
        /// </summary>
        public IEnumerable<DataRow> ShoutBoxMessages
        {
            get
            {
                return this.Get<IDBBroker>().GetShoutBoxMessages(YafContext.Current.PageBoardID);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The data bind.
        /// </summary>
        public override void DataBind()
        {
            this.BindData();
            base.DataBind();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the smilies on click string.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The format smilies on click string.
        /// </returns>
        protected static string FormatSmiliesOnClickString([NotNull] string code, [NotNull] string path)
        {
            code = Regex.Replace(code, "['\")(\\\\]", "\\$0");
            string onClickScript = "insertsmiley('{0}','{1}');return false;".FormatWith(code, path);
            return onClickScript;
        }

        /// <summary>
        /// Changes The CollapsiblePanelState of the Shoutbox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void CollapsibleImageShoutBox_Click([NotNull] object sender, [NotNull] ImageClickEventArgs e)
        {
            this.DataBind();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.CollapsibleImageShoutBox.DefaultState = (CollapsiblePanelState)this.Get<YafBoardSettings>().ShoutboxDefaultState;
            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            var dd = Page.FindControlRecursiveAs<Forum>("forum").BoardID;
            if (!this.Get<YafBoardSettings>().ShowShoutbox
                                         && !this.Get<IPermissions>()
                                                .Check(this.Get<YafBoardSettings>().ShoutboxViewPermissions))
            {
                return;
            }

                if (this.PageContext.IsAdmin)
                {
                    this.btnClear.Visible = true;
                }
                this.shoutBoxPanel.Visible = true;
            

            YafContext.Current.PageElements.RegisterJsResourceInclude("yafPageMethodjs", "js/jquery.pagemethod.js");

            if (this.IsPostBack)
            {
                return;
            }

            this.btnFlyOut.Text = this.GetText("SHOUTBOX", "FLYOUT");
            this.btnClear.Text = this.GetText("SHOUTBOX", "CLEAR");
            this.btnButton.Text = this.GetText("SHOUTBOX", "SUBMIT");

            this.FlyOutHolder.Visible = !YafControlSettings.Current.Popup;
            this.CollapsibleImageShoutBox.Visible = !YafControlSettings.Current.Popup;

            this.DataBind();
        }

        /// <summary>
        /// Submit new Message
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Submit_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            string username = this.PageContext.PageUserName;

            if (username != null && this.messageTextBox.Text != string.Empty)
            {
                CommonDb.shoutbox_savemessage(
                    PageContext.PageModuleID,
                    this.PageContext.PageBoardID,
                    this.messageTextBox.Text,
                    username,
                    this.PageContext.PageUserID,
                    this.Get<HttpRequestBase>().GetUserRealIPAddress());

                this.Get<IDataCache>().Remove(Constants.Cache.Shoutbox);
            }

            this.DataBind();
            this.messageTextBox.Text = string.Empty;

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);

            if (scriptManager != null)
            {
                scriptManager.SetFocus(this.messageTextBox);
            }
        }

        /// <summary>
        /// Clears the Shoutbox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Clear_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            CommonDb.shoutbox_clearmessages(PageContext.PageModuleID, this.PageContext.PageBoardID);

            // cleared... re-load from cache...
            this.Get<IDataCache>().Remove(Constants.Cache.Shoutbox);
            this.DataBind();
        }

        /// <summary>
        /// Refreshes the Shoutbox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Refresh_Click(object sender, EventArgs e)
        {
            this.DataBind();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            if (!this.shoutBoxPlaceHolder.Visible)
            {
                return;
            }

            this.shoutBoxRepeater.DataSource = this.ShoutBoxMessages;

            if (this.Get<YafBoardSettings>().ShowShoutboxSmiles)
            {
                this.smiliesRepeater.DataSource = CommonDb.smiley_listunique(PageContext.PageModuleID, this.PageContext.PageBoardID);
            }
        }

        #endregion
    }
}