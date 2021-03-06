<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="../../../controls/mytopicslist.ascx.cs"
    Inherits="VZF.Controls.MyTopicsList" %>
<%@ Register TagPrefix="VZF" TagName="TopicLine" Src="TopicLine.ascx" %>
<table class="command" cellspacing="0" cellpadding="0" width="100%" style="padding-bottom: 10px;">
    <tr>
        <td align="right">
            <VZF:LocalizedLabel ID="SinceLabel" runat="server" LocalizedTag="SINCE" />
            <asp:DropDownList ID="Since" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Since_SelectedIndexChanged" />
        </td>
    </tr>
</table>
<table class="command" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td>
            <VZF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
<table class="content" cellspacing="1" cellpadding="0" width="100%">

    <asp:Repeater ID="TopicList" runat="server">
        <ItemTemplate>
            <%# PrintForumName((System.Data.DataRowView)Container.DataItem) %>
            <VZF:TopicLine ID="TopicLine1" runat="server" FindUnread="true" DataRow="<%# Container.DataItem %>" />
        </ItemTemplate>
    </asp:Repeater>
    <tr>
        <td class="footer1" align="right" width="100%" colspan="6">
            <asp:LinkButton runat="server" OnClick="MarkAll_Click" ID="MarkAll" />
            <VZF:RssFeedLink ID="RssFeed" runat="server" Visible="False" />
            <VZF:RssFeedLink ID="AtomFeed" runat="server" Visible="False" />   
        </td>
    </tr>
</table>
<table class="command" width="100%" cellspacing="0" cellpadding="0">
    <tr>
        <td>
            <VZF:Pager runat="server" ID="PagerBottom" LinkedPager="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
