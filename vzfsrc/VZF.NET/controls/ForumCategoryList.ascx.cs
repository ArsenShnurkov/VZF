﻿/* Yet Another Forum.NET
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
namespace VZF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    using VZF.Data.Common;
    using VZF.Data.DAL;

    using YAF.Classes;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Extensions;
    using VZF.Utils;

    #endregion

    /// <summary>
    /// The forum category list.
    /// </summary>
    public partial class ForumCategoryList : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns>
        /// The column count.
        /// </returns>
        protected int ColumnCount()
        {
            int cnt = 5;

            if (this.Get<YafBoardSettings>().ShowModeratorList && this.Get<YafBoardSettings>().ShowModeratorListAsColumn)
            {
                cnt++;
            }

            return cnt;
        }

        /// <summary>
        /// The mark all_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void MarkAll_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var markAll = (LinkButton)sender;

            object categoryId = null;

            int icategoryId;
            if (int.TryParse(markAll.CommandArgument, out icategoryId))
            {
                categoryId = icategoryId;
            }

            DataTable dt = CommonDb.forum_listread(
                mid: PageContext.PageModuleID,
                boardId: this.PageContext.PageBoardID,
                userId: this.PageContext.PageUserID,
                categoryId: categoryId,
                parentId: null,
                useStyledNicks: false,
                findLastRead: false,
                showPersonalForums: true,
                showCommonForums: true,
                forumCreatedByUserId: null);

            this.Get<IReadTrackCurrentUser>().SetForumRead(dt.AsEnumerable().Select(r => r["ForumID"].ToType<int>()));

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
            this.BindData();
        }

        /// <summary>
        /// Bind Data
        /// </summary>
        private void BindData()
        {
            DataSet ds = this.Get<IDBBroker>()
                             .BoardLayout(
                                 this.PageContext.PageBoardID,
                                 this.PageContext.PageUserID,
                                 this.PageContext.PageCategoryID,
                                 null);
            /* this.CategoryList.FindControlRecursiveAs<HtmlTableCell>("Td1").Visible = PageContext.BoardSettings.ShowModeratorList &&
                              PageContext.BoardSettings.ShowModeratorListAsColumn; */
            this.CategoryList.DataSource = ds.Tables[ObjectName.GetVzfObjectName("Category", YafContext.Current.PageModuleID)];
            this.CategoryList.DataBind();
        }

        #endregion
    }
}