﻿<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.moderating" CodeBehind="moderating.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="VZF.Controls" %>
<%@ Register TagPrefix="VZF" Namespace="VZF.Controls" Assembly="VZF.Controls" %>
<VZF:PageLinks runat="server" ID="PageLinks" />
<%@ Register TagPrefix="VZF" TagName="TopicLine" Src="../controls/TopicLine.ascx" %>
<table class="content" cellspacing="1" cellpadding="0" width="100%">
    <tr>
        <td class="header1" colspan="4">
            <VZF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="MEMBERS" LocalizedPage="MODERATE" />
        </td>
    </tr>
    <tr class="header2">
        <td>
            <VZF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="USER" LocalizedPage="MODERATE" />
        </td>
        <td align="center">
            <VZF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ACCEPTED" LocalizedPage="MODERATE" />
        </td>
        <td>
            <VZF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ACCESSMASK" LocalizedPage="MODERATE" />
        </td>
        <td>
            &nbsp;
        </td>
    </tr>
    <asp:Repeater runat="server" ID="UserList" OnItemCommand="UserList_ItemCommand">
        <ItemTemplate>
            <tr class="post">
                <td>
                    <%# this.Get<YafBoardSettings>().EnableDisplayName ? Eval("DisplayName") : Eval("Name") %>
                </td>
                <td align="center">
                     <img id="AccessYesNo" alt="?" src='<%#  (bool)Eval("Accepted") ? YafContext.Current.Get<ITheme>().GetItem("ICONS", "FORUM_HASACCESS") : YafContext.Current.Get<ITheme>().GetItem("ICONS", "FORUM_HASNOACCESS") %>' runat="server" /> 
                </td>
                 <td>
                    <%# Eval("Access") %>
                </td>
                <td>
                     <VZF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" CommandName='edit' CommandArgument='<%# Eval("UserID") %>' TitleLocalizedTag="EDIT" ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON" runat="server"></VZF:ThemeButton>
                    <VZF:ThemeButton ID="ThemeButtonRemove" CssClass="yaflittlebutton" OnLoad="DeleteUser_Load"  CommandName='remove' CommandArgument='<%#Eval("UserID") %>' TitleLocalizedTag="REMOVE" ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON" runat="server"></VZF:ThemeButton>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr class="footer1">
        <td colspan="4">
            <VZF:ThemeButton ID="AddUser" runat="server" CssClass="yafcssbigbutton leftItem" TextLocalizedPage="MODERATE" TitleLocalizedPage="MODERATE"
    TextLocalizedTag="INVITE" TitleLocalizedTag="INVITE"
     OnClick="AddUser_Click" />
        </td>
    </tr>
</table>
<br />
<VZF:ThemeButton ID="RestoreTopic" runat="server" CssClass="yafcssbigbutton rightItem"
    TextLocalizedTag="BUTTON_RESTORETOPIC" TitleLocalizedTag="BUTTON_RESTORETOPIC_TT"
    OnLoad="Restore_Load" OnClick="RestoreTopics_Click" Visible="false" />
<VZF:ThemeButton ID="DeleteTopic" runat="server" CssClass="yafcssbigbutton rightItem"
    TextLocalizedTag="BUTTON_DELETETOPIC" TitleLocalizedTag="BUTTON_DELETETOPIC_TT"
    OnLoad="Delete_Load" OnClick="DeleteTopics_Click" />
<VZF:ThemeButton ID="EraseTopic" runat="server" CssClass="yafcssbigbutton rightItem"
    TextLocalizedTag="BUTTON_ERASETOPIC" TitleLocalizedTag="BUTTON_ERASETOPIC_TT"
    OnLoad="Erase_Load" OnClick="EraseTopics_Click" Visible="false" />
<table class="content" width="100%">
    <asp:Repeater ID="Announcements" runat="server">
        <HeaderTemplate>
            <tr class="topicTitle">
                <th class="header1" colspan="6">
                    <VZF:ThemeImage ID="ai" ThemePage="ICONS" ThemeTag="ANOUNCEMENT_T_SICON" LocalizedTitlePage="TOPICS" LocalizedTitleTag="ANNOUNCEMENTS_TITLE"  runat="server"/>
                    <VZF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="ANNOUNCEMENTS_TITLE" />
                </th>
            </tr>
            <tr class="topicSubTitle">
                <th class="header2" width="1%">
                    &nbsp;
                </th>
                <th class="header2 headerTopic" align="left">
                    <VZF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="topics" />
                </th>
                <th class="header2 headerReplies" align="right" width="7%">
                    <VZF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="replies" />
                </th>
                <th class="header2 headerViews" align="right" width="7%">
                    <VZF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="views" />
                </th>
                <th class="header2 headerLastPost" align="left" width="15%">
                    <VZF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="lastpost" />
                </th>
            </tr>
        </HeaderTemplate>
        <ItemTemplate>
             <VZF:TopicLine runat="server" DataRow="<%# Container.DataItem %>" AllowSelection="true" />
        </ItemTemplate>
       <FooterTemplate>
           <tfoot visible="false">
               <tr>
                   <td colspan="6" class="header2"></td>
               </tr>
           </tfoot>
       </FooterTemplate>
    </asp:Repeater>
</table>
<VZF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" UsePostBack="True" />
<table class="content" cellspacing="1" cellpadding="0" width="100%">
    <tr>
        <td class="header1" colspan="6">
            <VZF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="title" LocalizedPage="MODERATE" />
        </td>
    </tr>
    <tr>
        <td class="header2" width="1%">
            &nbsp;
        </td>
        <td class="header2" width="1%">
            &nbsp;
        </td>
        <td class="header2" align="left">
            <VZF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="topics" LocalizedPage="MODERATE" />
        </td>
        <td class="header2" align="center" width="7%">
            <VZF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="replies" LocalizedPage="MODERATE" />
        </td>
        <td class="header2" align="center" width="7%">
            <VZF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="views" LocalizedPage="MODERATE" />
        </td>
        <td class="header2" align="center" width="15%">
            <VZF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="lastpost" LocalizedPage="MODERATE" />
        </td>
    </tr>
    <asp:Repeater ID="topiclist" runat="server" OnItemCommand="topiclist_ItemCommand">
        <ItemTemplate>
             <tr>
            <VZF:TopicLine runat="server" DataRow="<%# Container.DataItem %>" AllowSelection="true" />
             </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr>
        <td class="footer1" colspan="6">
            &nbsp;
        </td>
    </tr>
</table>
<VZF:ThemeButton ID="RestoreTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
    TextLocalizedTag="BUTTON_RESTORETOPIC" TitleLocalizedTag="BUTTON_RESTORETOPIC_TT"
    OnLoad="Restore_Load" OnClick="RestoreTopics_Click" Visible="false" />
<VZF:ThemeButton ID="DeleteTopics2" runat="server" CssClass="yafcssbigbutton rightItem"
    TextLocalizedTag="BUTTON_DELETETOPIC" TitleLocalizedTag="BUTTON_DELETETOPIC_TT"
    OnLoad="Delete_Load" OnClick="DeleteTopics_Click" />
<VZF:ThemeButton ID="EraseTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
    TextLocalizedTag="BUTTON_ERASETOPIC" TitleLocalizedTag="BUTTON_ERASETOPIC_TT"
    OnLoad="Erase_Load" OnClick="EraseTopics_Click" Visible="false" />
<VZF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" UsePostBack="True" />
<div id="DivSmartScroller">
    <VZF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
