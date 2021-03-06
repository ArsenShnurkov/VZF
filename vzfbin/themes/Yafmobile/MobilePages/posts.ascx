﻿<%@ Control Language="c#" CodeBehind="../../../pages/posts.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.posts" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="VZF" TagName="DisplayPost" Src="DisplayPost.ascx" %>
<%@ Register TagPrefix="VZF" TagName="DisplayAd" Src="DisplayAd.ascx" %>
<%@ Register TagPrefix="VZF" TagName="PollList" Src="../../../controls/PollList.ascx" %>
<%@ Register TagPrefix="VZF" TagName="ForumJumper" Src="~/controls/ForumJumper.ascx" %>
<%@ Register TagPrefix="VZF" Namespace="VZF.Controls" Assembly="VZF" %>
<VZF:PageLinks ID="PageLinks" runat="server" />
<VZF:PollList ID="PollList" TopicId='<%# PageContext.PageTopicID %>' ShowButtons='<%# ShowPollButtons() %>' Visible='<%# PollGroupId() > 0 %>' PollGroupId='<%# PollGroupId() %>' runat="server"/>
<a id="top" name="top"></a>

<div>
            <span id="dvFavorite1">
                <VZF:ThemeButton ID="TagFavorite1" runat="server" CssClass="yafcssbigbutton rightItem"
                    TextLocalizedTag="BUTTON_TAGFAVORITE" TitleLocalizedTag="BUTTON_TAGFAVORITE_TT" />
            </span>        
            <asp:PlaceHolder ID="NoButtonsMobile" runat="server" Visible="false">
            <VZF:ThemeButton ID="MoveTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="MoveTopic_Click" TextLocalizedTag="BUTTON_MOVETOPIC" TitleLocalizedTag="BUTTON_MOVETOPIC_TT" />
            <VZF:ThemeButton ID="UnlockTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="UnlockTopic_Click" TextLocalizedTag="BUTTON_UNLOCKTOPIC" TitleLocalizedTag="BUTTON_UNLOCKTOPIC_TT" />
            <VZF:ThemeButton ID="LockTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="LockTopic_Click" TextLocalizedTag="BUTTON_LOCKTOPIC" TitleLocalizedTag="BUTTON_LOCKTOPIC_TT" />
            <VZF:ThemeButton ID="DeleteTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="DeleteTopic_Click" OnLoad="DeleteTopic_Load" TextLocalizedTag="BUTTON_DELETETOPIC"
                TitleLocalizedTag="BUTTON_DELETETOPIC_TT" />
            </asp:PlaceHolder>
            <VZF:ThemeButton ID="NewTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="NewTopic_Click" TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" />
            <VZF:ThemeButton ID="PostReplyLink1" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="PostReplyLink_Click" TextLocalizedTag="BUTTON_POSTREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT" />
</div>

<VZF:Pager ID="Pager" runat="server" UsePostBack="False" />
<table class="content postHeader" width="100%">
    <tr class="postTitle" >
        <td class="header1">
            <div class="leftItem">
              <asp:HyperLink ID="TopicLink" runat="server" CssClass="HeaderTopicLink">
                <asp:Label ID="TopicTitle" runat="server" />
              </asp:HyperLink>
            </div>
            <div class="rightItem"  runat="server" visible="false">
                <asp:HyperLink ID="ShareLink" runat="server" CssClass="PopMenuLink">
                    <VZF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="Share" />
                </asp:HyperLink>
                <asp:HyperLink ID="OptionsLink" runat="server">
                    <VZF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="Options" />
                </asp:HyperLink>
                <asp:PlaceHolder ID="ViewOptions" runat="server">
                    <asp:HyperLink ID="ViewLink" runat="server">
                        <VZF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="View" />
                    </asp:HyperLink>
                </asp:PlaceHolder>
                <asp:HyperLink ID="ImageMessageLink" runat="server" CssClass="GoToLink">
                     <VZF:ThemeImage ID="LastPostedImage" runat="server" Style="border: 0" />
                </asp:HyperLink>
                <asp:HyperLink ID="ImageLastUnreadMessageLink" runat="server" CssClass="GoToLink">
                     <VZF:ThemeImage ID="LastUnreadImage" runat="server"  Style="border: 0" />
                </asp:HyperLink>
            </div>
        </td>
    </tr>
    <tr class="header2 postNavigation"  runat="server" visible="false">
        <td class="header2links">
            <asp:LinkButton ID="PrevTopic" runat="server" CssClass="header2link" OnClick="PrevTopic_Click">
                <VZF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="prevtopic" />
            </asp:LinkButton>
            <asp:LinkButton ID="NextTopic" runat="server" CssClass="header2link" OnClick="NextTopic_Click">
                <VZF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="nexttopic" />
            </asp:LinkButton>
            <div id="Div1" runat="server" visible="false">
                <asp:LinkButton ID="TrackTopic" runat="server" CssClass="header2link" OnClick="TrackTopic_Click">
                    <VZF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="watchtopic" />
                </asp:LinkButton>
                <asp:LinkButton ID="EmailTopic" runat="server" CssClass="header2link" OnClick="EmailTopic_Click">
                    <VZF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="emailtopic" />
                </asp:LinkButton>
                <asp:LinkButton ID="PrintTopic" runat="server" CssClass="header2link" OnClick="PrintTopic_Click">
                    <VZF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="printtopic" />
                </asp:LinkButton>
                <asp:HyperLink ID="RssTopic" runat="server" CssClass="header2link">
                    <VZF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="rsstopic" />
                </asp:HyperLink>
            </div>
        </td>
    </tr>
</table>
<asp:Repeater ID="MessageList" runat="server" OnItemCreated="MessageList_OnItemCreated">
    <ItemTemplate>
        <table class="content postContainer" width="100%">
            <%# GetThreadedRow(Container.DataItem) %>
            <VZF:DisplayPost ID="DisplayPost1" runat="server" DataRow="<%# (DataRow)Container.DataItem %>"
                Visible="<%#IsCurrentMessage(Container.DataItem)%>" IsThreaded="<%#IsThreaded%>" />
            <VZF:DisplayAd ID="DisplayAd" runat="server" Visible="False" />
        </table>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <table class="content postContainer_Alt" width="100%">
            <%# GetThreadedRow(Container.DataItem) %>
            <VZF:DisplayPost ID="DisplayPostAlt" runat="server" DataRow="<%# (DataRow)Container.DataItem %>"
                IsAlt="True" Visible="<%#IsCurrentMessage(Container.DataItem)%>" IsThreaded="<%#IsThreaded%>" />
            <VZF:DisplayAd ID="DisplayAd" runat="server" Visible="False" />
        </table>
    </AlternatingItemTemplate>
</asp:Repeater>
<asp:PlaceHolder ID="QuickReplyPlaceHolder" runat="server">
    <table class="content postQuickReply" width="100%">
        <tr>
            <td colspan="3" class="post" style="padding: 0px;">
                <VZF:DataPanel runat="server" ID="DataPanel1" AllowTitleExpandCollapse="true" TitleStyle-CssClass="header2"
                    TitleStyle-Font-Bold="true" Collapsed="false">
                    <div class="post quickReplyLine" id="QuickReplyLine" runat="server">
                    </div>
                    <div id="CaptchaDiv" align="center" visible="false" runat="server">
                        <br />
                        <table class="content">
                            <tr>
                                <td class="header2">
                                    <VZF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="Captcha_Image" />
                                </td>
                            </tr>
                            <tr>
                                <td class="post" align="center">
                                    <asp:Image ID="imgCaptcha" runat="server" AlternateText="Captcha" />
                                </td>
                            </tr>
                            <tr>
                                <td class="post">
                                    <VZF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="Captcha_Enter" />
                                    <asp:TextBox ID="tbCaptcha" runat="server" />
                                </td>
                            </tr>
                        </table>
                        <br />
                    </div>
                    &nbsp;<div align="center" style="margin: 7px;">
                        <asp:Button ID="QuickReply" CssClass="pbutton" runat="server" />
                        &nbsp;</div>
                </VZF:DataPanel>
            </td>
        </tr>
    </table>
</asp:PlaceHolder>
<VZF:Pager ID="PagerBottom" runat="server" LinkedPager="Pager" UsePostBack="false" />
<asp:PlaceHolder ID="mobileHolder" runat="server" Visible="false">
<table class="content postForumUsers" width="100%">
    <VZF:ForumUsers ID="ForumUsers1" runat="server" />
</table>
<table cellpadding="0" cellspacing="0" class="command" width="100%">
    <tr>
        <td>
            <span id="dvFavorite2">
                <VZF:ThemeButton ID="TagFavorite2" runat="server" CssClass="yafcssbigbutton rightItem"
                    TextLocalizedTag="BUTTON_TAGFAVORITE" TitleLocalizedTag="BUTTON_TAGFAVORITE_TT" />
            </span>        
            <VZF:ThemeButton ID="MoveTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="MoveTopic_Click" TextLocalizedTag="BUTTON_MOVETOPIC" TitleLocalizedTag="BUTTON_MOVETOPIC_TT" />
            <VZF:ThemeButton ID="UnlockTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="UnlockTopic_Click" TextLocalizedTag="BUTTON_UNLOCKTOPIC" TitleLocalizedTag="BUTTON_UNLOCKTOPIC_TT" />
            <VZF:ThemeButton ID="LockTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="LockTopic_Click" TextLocalizedTag="BUTTON_LOCKTOPIC" TitleLocalizedTag="BUTTON_LOCKTOPIC_TT" />
            <VZF:ThemeButton ID="DeleteTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="DeleteTopic_Click" OnLoad="DeleteTopic_Load" TextLocalizedTag="BUTTON_DELETETOPIC"
                TitleLocalizedTag="BUTTON_DELETETOPIC_TT" />
            <VZF:ThemeButton ID="NewTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="NewTopic_Click" TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" />
            <VZF:ThemeButton ID="PostReplyLink2" runat="server" CssClass="yafcssbigbutton rightItem"
                OnClick="PostReplyLink_Click" TextLocalizedTag="BUTTON_POSTREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT" />
        </td>
    </tr>
</table>



<VZF:PageLinks ID="PageLinksBottom" runat="server" LinkedPageLinkID="PageLinks" />
<VZF:SimpleTagCloud ID="Stc1" runat="server"/>  
<asp:PlaceHolder ID="ForumJumpHolder" runat="server">
    <div id="DivForumJump">
        <VZF:ForumJumper  ID="fj1" runat="server"></VZF:ForumJumper>
    </div>
</asp:PlaceHolder>
<div id="DivPageAccess" class="smallfont">
    <VZF:PageAccess ID="PageAccess1" runat="server" />
</div>
</asp:PlaceHolder>
<div id="DivSmartScroller">
    <VZF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
<VZF:PopMenu ID="ShareMenu" runat="server" Control="ShareLink" />
<asp:UpdatePanel ID="PopupMenuUpdatePanel" runat="server">
    <ContentTemplate>
        <VZF:PopMenu runat="server" ID="OptionsMenu" Control="OptionsLink" />
        <span id="WatchTopicID" runat="server" visible="false"></span>
    </ContentTemplate>
</asp:UpdatePanel>
<VZF:PopMenu ID="ViewMenu" runat="server" Control="ViewLink" />