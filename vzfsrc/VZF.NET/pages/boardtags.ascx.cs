﻿namespace YAF.Pages
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using VZF.Data.Common;
    using VZF.Utils;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    /// <summary>
    /// The board tags.
    /// </summary>
    public partial class boardtags : ForumPage
    {
        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            if (!this.Get<YafBoardSettings>().ShowBoardTags)
            {
                YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
            }

            this.PageLinksTop.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinksTop.AddLink(this.GetText("TAGSBOARD", "TITLE"), string.Empty);

            // this.AlphaSort1.PagerPage = ForumPages.boardtags;
            this.BindData();
        }

        /// <summary>
        /// The ok button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void OkButtonClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.forum);
        }

        #region Public Methods

        /// <summary>
        /// The search_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Search_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// The search_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Reset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-direct to self.
            YafBuildLink.Redirect(ForumPages.boardtags);
        }

        /// <summary>
        /// The pager_ page change.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Pager_PageChange(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// The get tag class.
        /// </summary>
        /// <param name="tagFrequency">
        /// The tag frequency.
        /// </param>
        /// <param name="highestFrequency">
        /// The highest frequency.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetTagClass(int tagFrequency, int highestFrequency)
        {
            string tag = "tag0";

            if (tagFrequency == 0 || highestFrequency == 0)
            {
                return tag;
            }

            var percentageFrequency = ((((tagFrequency * 100) / highestFrequency) / 10) * 10) - 10;
            if (percentageFrequency <= 0)
            {
                percentageFrequency = 1;
            }

            return "tag{0}".FormatWith(Convert.ToUInt16(percentageFrequency));
        }

        /// <summary>
        /// The get tag cloud html.
        /// </summary>
        /// <param name="tagNameWithFrequencies">
        /// The tag Name With Frequencies.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTagCloudHtml(DataTable tagNameWithFrequencies)
        {
            if (tagNameWithFrequencies == null || tagNameWithFrequencies.Rows.Count <= 0)
            {
                return null;
            }

            string tagLabel = null;
            if (this.Get<YafBoardSettings>().AllowTopicTags && tagNameWithFrequencies.Rows.Count > 0)
            {
                var tagsTopicLineIcon = this.Get<ITheme>().GetItem("ICONS", "TOPIC_TAG");

                if (tagsTopicLineIcon.IsSet())
                {
                    tagLabel = "<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" style=\"border: 0;width:16px;height:16px\" />&nbsp;"
                            .FormatWith(tagsTopicLineIcon, this.Get<ILocalization>().GetText("POSTS", "TAGS_POSTS"));
                }
            }

            var tagCloudString = new StringBuilder();
            int tagCount = 0;
            foreach (DataRow tag in tagNameWithFrequencies.Rows)
            {
                string tagClass = ".tagcloud " + GetTagClass(tag["TagCount"].ToType<int>(), tag["MaxTagCount"].ToType<int>());

                string numberOfTags = null;
                if (this.Get<YafBoardSettings>().ShowNumberOfTags)
                {
                    numberOfTags = "({0})".FormatWith(tag["TagCount"].ToType<int>());
                }

                string addQweryParams = "b={0}".FormatWith(this.PageContext.PageBoardID);

                string targetUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.topicsbytags, "tagid={0}&{1}".FormatWith(tag["TagID"], addQweryParams));
                string tagItem = "<a class=\"{0}\" href=\"{1}\">{2}{3}</a>&nbsp;&nbsp;".FormatWith(
                    tagClass, targetUrl, this.HtmlEncode(tag["Tag"]), numberOfTags);

                tagCloudString.Append(tagItem);
                tagCount++;
            }

            if (tagCount > 0)
            {
                return tagLabel + tagCloudString;
            }

            return null;
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.Get<YafBoardSettings>().BoardTagsPerPage; 
            char selectedCharLetter = this.AlphaSort1.CurrentLetter;
            bool beginsWith = this.UserSearchName.Text.IsNotSet()
                              || !(selectedCharLetter == char.MinValue || selectedCharLetter == '#');

            string selectedLetter = this.UserSearchName.Text.IsSet() ? this.UserSearchName.Text.Trim() : (!(selectedCharLetter == char.MinValue || selectedCharLetter == '#') ? selectedCharLetter.ToString(CultureInfo.InvariantCulture) : string.Empty);

            using (var dtTopics = CommonDb.forum_tags(
                    this.PageContext.PageModuleID,
                    this.PageContext.PageBoardID,
                    this.PageContext.PageUserID,
                    null,
                    this.PagerTop.CurrentPageIndex,
                    this.PagerTop.PageSize,
                    selectedLetter,
                    beginsWith))
            {
                if (dtTopics != null && dtTopics.Rows.Count > 0)
                {
                    this.PagerTop.Count = dtTopics.AsEnumerable().First().Field<int>("TotalCount");
                }

                this.TagLinks.InnerHtml = this.GetTagCloudHtml(dtTopics);
            }
        }

        #endregion
    }
}