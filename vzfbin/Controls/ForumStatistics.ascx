﻿<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
	Inherits="VZF.Controls.ForumStatistics" Codebehind="ForumStatistics.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<asp:UpdatePanel ID="UpdateStatsPanel" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<table class="content forumStatisticsContent" cellspacing="1" cellpadding="0" width="100%">
			<tr>
				<td class="header1" colspan="2">
					<VZF:CollapsibleImage ID="CollapsibleImage" runat="server" BorderWidth="0" Style="vertical-align: middle"
						PanelID='InformationPanel' AttachedControlID="InformationPlaceHolder" />&nbsp;&nbsp;<VZF:LocalizedLabel
							ID="InformationHeader" runat="server" LocalizedTag="INFORMATION" />
				</td>
			</tr>
			<asp:PlaceHolder runat="server" ID="InformationPlaceHolder">
				<tr>
					<td class="header2" colspan="2">
						<VZF:LocalizedLabel ID="ActiveUsersLabel" runat="server" LocalizedTag="ACTIVE_USERS" />
					</td>
				</tr>
				<tr>
					<td class="post" width="1%">
						<VZF:ThemeImage ID="ForumUsersImage" runat="server" ThemeTag="FORUM_USERS" />
					</td>
					<td class="post">
						<asp:Label runat="server" ID="ActiveUserCount" />
						<br />
						<asp:Label runat="server" ID="MostUsersCount" />
						<br />
						<VZF:ActiveUsers ID="ActiveUsers1" runat="server">
						</VZF:ActiveUsers>
					</td>
				</tr>
				<asp:PlaceHolder runat="server" ID="RecentUsersPlaceHolder" Visible="False" >
				<tr>
					<td class="header2" colspan="2">
						<VZF:LocalizedLabel ID="RecentUsersLabel" runat="server" LocalizedTag="RECENT_USERS" />
					</td>
				</tr>
				<tr>
					<td class="post" width="1%">
						<VZF:ThemeImage ID="ThemeImage1" runat="server" ThemeTag="FORUM_USERS" />
					</td>
					<td class="post">
						<asp:Label runat="server" ID="RecentUsersCount" />
						<br />
						<VZF:ActiveUsers ID="RecentUsers" runat="server" InstantId="RecentUsersOneDay" Visible="False">
						</VZF:ActiveUsers>
					</td>
				</tr>
				</asp:PlaceHolder>
				<tr>
					<td class="header2" colspan="2">
						<VZF:LocalizedLabel ID="StatsHeader" runat="server" LocalizedTag="STATS" />
					</td>
				</tr>
				<tr>
					<td class="post" width="1%">
						<VZF:ThemeButton ID="ForumStatsImage" runat="server" ImageThemeTag="FORUM_STATS" NavigateUrl="<%# YafBuildLink.GetLink(ForumPages.mostactiveusers) %>"   />
					</td> 
					<td class="post">
						<asp:Label ID="StatsPostsTopicCount" runat="server" />
						<br />
						<asp:PlaceHolder runat="server" ID="StatsLastPostHolder" Visible="False">
							<asp:Label ID="StatsLastPost" runat="server" />&nbsp;<VZF:UserLink ID="LastPostUserLink"
								runat="server" />.
							<br />
						</asp:PlaceHolder>
						<asp:Label ID="StatsMembersCount" runat="server" />
						<br />
						<asp:Label ID="StatsNewestMember" runat="server" />&nbsp;<VZF:UserLink ID="NewestMemberUserLink"
							runat="server" />.
						<br />
						<asp:PlaceHolder ID="BirthdayUsers" runat="server" Visible="false">
						  <asp:Label ID="StatsTodaysBirthdays" runat="server" />
						</asp:PlaceHolder>
					</td>
				</tr>
				<tr class="post">
						<td colspan="2" align="right">
						<VZF:ThemeImage ID="ThemeImage2" runat="server" ThemePage="ICONS" ThemeTag="MENU3ICON" />&nbsp;<asp:LinkButton runat="server" OnClick="BoardTagsLink_Click" ID="BoardTagsLink" Text='<%# this.GetText("TAGSBOARD","TAGS_BOARD_LNK") %>' />&nbsp;<VZF:ThemeImage ID="ThemeImage3" runat="server" ThemePage="ICONS" ThemeTag="MENU3ICON" /><asp:LinkButton runat="server" OnClick="MosActiveForLink_Click" ID="MosActiveForLink" Text='<%# this.GetTextFormatted("MOSTACTIVEUSERS_FOR_LINK",7) %>' />
						</td>
				</tr>
			</asp:PlaceHolder>
		</table>
	</ContentTemplate>
</asp:UpdatePanel>
