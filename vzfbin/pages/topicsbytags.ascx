﻿<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.topicsbytags" Codebehind="topicsbytags.ascx.cs" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="VZF.Controls" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Register TagPrefix="VZF" TagName="TopicLine" Src="../controls/TopicLine.ascx" %>
<%@ Register TagPrefix="VZF" TagName="ForumJumper" Src="../controls/ForumJumper.ascx" %>
<%@ Register TagPrefix="VZF" Namespace="VZF.Controls" Assembly="VZF.Controls" %>

<VZF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator">
</div>  
<table class="command" width="100%">
    <thead>
        <tr>
            <td colspan="2">
                <VZF:Pager runat="server" ID="Pager" UsePostBack="False" />
            </td>
        </tr>
    </thead>
</table>
<table class="content" width="100%">
    <thead>
    <tr class="topicTitle">
        <th id="thTopicTitle" class="header1" colspan='<%# PageContext.IsAdmin ? 7 : 6 %>' runat="server">
            <asp:Label ID="PageTitle" runat="server"></asp:Label>
        </th>
    </tr>
    <tr class="topicSubTitle">
        <th id="thTopicSubTitle" class="header2" colspan='<%# PageContext.IsAdmin ? 7 : 6 %>' runat="server">
            <VZF:LocalizedLabel ID="TagsListLLbl" runat="server"/>
        </th>
    </tr>
    <tr class="topicSubTitle">
        <th class="header2" width="1%">
            &nbsp;
        </th>
        <th id="thSelectIt"  class="header2 headerTopic" align="left" Visible='<%# PageContext.IsAdmin %>' runat="server">
            <VZF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="selectit" />
        </th>
        <th class="header2 headerTopic" align="left">
            <VZF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TOPIC" />
        </th>
        <th class="header2 headerReplies" align="right" width="7%">
            <VZF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="replies" />
        </th>
        <th class="header2 headerViews" align="right" width="7%">
            <VZF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="views" />
        </th>
        <th class="header2 headerLastPost" align="left" width="15%">
            <VZF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="lastpost" />
        </th>
    </tr>
    </thead>
    <tbody>
        <tr>
            <asp:Repeater ID="TopicList" runat="server">
                <ItemTemplate>
                <VZF:TopicLine  runat="server" AltLastPost="<%# this.LastPostImageTT %>" DataRow="<%# Container.DataItem %>" AllowSelection="<%# PageContext.IsAdmin %>" />
                </ItemTemplate>
                <AlternatingItemTemplate>
                     <VZF:TopicLine runat="server" IsAlt="True" AltLastPost="<%# this.LastPostImageTT %>" DataRow="<%# Container.DataItem %>" AllowSelection="<%# PageContext.IsAdmin %>" />
                </AlternatingItemTemplate>
            </asp:Repeater>
        </tr>
    </tbody>
</table>
<table class="command" width="100%" cellspacing="0" cellpadding="0">
    <tfoot>
        <tr>
            <td align="left" colspan="2">
                <VZF:Pager ID="PagerBottom" runat="server" LinkedPager="Pager" UsePostBack="False" />
            </td>
        </tr>
        <tr>
            <td id="trButtons" align="center" colspan="5" runat="server" >
                <VZF:ThemeButton ID="OKButon" runat="server" CssClass="yafcssbigbutton centerItem"
                OnClick="okBtn_click" TextLocalizedPage="COMMON" TextLocalizedTag="OK" TitleLocalizedPage="COMMON" TitleLocalizedTag="OK" CommandName="Action" CommandArgument='<%# this.retBtnArgs %>' />
                <VZF:ThemeButton ID="DeleteSelectedBtn" runat="server" CssClass="yafcssbigbutton centerItem"
                 TextLocalizedPage="TOPICSBYTAGS" TextLocalizedTag="DELETE_SELECTED" TitleLocalizedPage="TOPICSBYTAGS" TitleLocalizedTag="DELETE_SELECTED" OnClick="DeleteSelectedBtn_OnClick" Visible='<%# PageContext.IsAdmin %>'  />
                <VZF:ThemeButton ID="DeleteAllBtn" runat="server" CssClass="yafcssbigbutton centerItem"
                 TextLocalizedPage="TOPICSBYTAGS" TextLocalizedTag="DELETE_ALL" TitleLocalizedPage="TOPICSBYTAGS" TitleLocalizedTag="DELETE_ALL" OnClick="DeleteAllBtn_OnClick" Visible="False"  />
            </td>
        </tr>
    </tfoot>
</table>
<asp:PlaceHolder ID="ForumJumpHolder" runat="server">
    <div id="DivForumJump">
        <VZF:ForumJumper  ID="fj1" runat="server"></VZF:ForumJumper>
    </div>
</asp:PlaceHolder>
<div class="clearItem"></div>
<div id="DivSmartScroller">
    <VZF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
