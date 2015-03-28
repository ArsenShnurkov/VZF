﻿<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.hostsettings"
    CodeBehind="hostsettings.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
    <%@ Import Namespace="VZF.Controls" %>
<%@ Register TagPrefix="VZF" Namespace="VZF.Controls" Assembly="VZF.Controls" %>
<VZF:PageLinks runat="server" ID="PageLinks" />
<VZF:AdminMenu runat="server" ID="Adminmenu1">
  <asp:Panel id="HostSettingsTabs" runat="server">
    <ul>
        <li><a href="#View1"><VZF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View2"><VZF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HOST_FEATURES" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View3"><VZF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HOST_DISPLAY" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View4"><VZF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HOST_ADVERTS" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View5"><VZF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="HOST_EDITORS" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View6"><VZF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="HOST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View7"><VZF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="HOST_TEMPLATES" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View8"><VZF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="HOST_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View9"><VZF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="HOST_CACHE" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View10"><VZF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="HOST_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View11"><VZF:LocalizedLabel ID="LocalizedLabel34" runat="server" LocalizedTag="HOST_LOG" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View12"><VZF:LocalizedLabel ID="LocalizedLabel36" runat="server" LocalizedTag="HOST_SYNDICATION" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
    </ul>
    <div id="View1">
        <table class="content" cellspacing="2" width="100%" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                           <VZF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="HEADER_SETUP" LocalizedPage="ADMIN_HOSTSETTINGS" />                           
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="SERVER_VERSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:Label ID="SQLVersion" runat="server" CssClass="smallfont"></asp:Label>
                        </td>
                    </tr>
                   <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel231" runat="server" LocalizedTag="APP_OS_NAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:Label ID="AppOSName" runat="server" CssClass="smallfont"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel232" runat="server" LocalizedTag="APP_RUNTIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:Label ID="AppRuntime" runat="server" CssClass="smallfont"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel233" runat="server" LocalizedTag="APP_CORES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:Label ID="AppCores" runat="server" CssClass="smallfont"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel234" runat="server" LocalizedTag="APP_MEMORY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:Label ID="AppMemory" runat="server" CssClass="smallfont"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="SERVERTIME_CORRECT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <strong><%# DateTime.UtcNow %></strong>.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ServerTimeCorrection" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="EMAIL_VERIFICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EmailVerification" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel235" runat="server" LocalizedTag="RESTART_APP_LIMIT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox ID="RestartApplicationLimit" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                           <VZF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="FILE_TABLE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseFileTable" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="ABANDON_TRACKUSR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AbandonSessionsForDontTrack" runat="server"></asp:CheckBox>
                        </td>
                    </tr>

                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="MAX_ATTACHMENTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MaxNumberOfAttachments" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="MAX_FILESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MaxFileSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="POSTEDIT_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="EditTimeOut" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="WSERVICE_TOKEN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="WebServiceToken" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="NAME_LENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserNameMaxLength" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="MAX_POST_CHARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post"> 
                            <asp:TextBox Style="width:350px" ID="MaxReportPostChars" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="MAX_POST_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MaxPostSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="FLOOT_DELAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PostFloodDelay" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel15" runat="server" LocalizedTag="REFERRER_CHECK" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DoUrlReferrerSecurityCheck" runat="server"></asp:CheckBox>
                        </td>
                    </tr>      
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel16" runat="server" LocalizedTag="CREATE_NNTPNAMES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="CreateNntpUsers" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel238" runat="server" LocalizedTag="NNTP_ARTICLESFETCHTIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="NntpArticlesRetrieveTime" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel239" runat="server" LocalizedTag="NNTP_TOPICPERIOD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="NntpTopicProtectionPeriod" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel32" runat="server" LocalizedTag="HEADER_SPAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel185" runat="server" LocalizedTag="CHECK_FOR_SPAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="SpamServiceType" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel186" runat="server" LocalizedTag="AKISMET_KEY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AkismetApiKey" Style="width:350px" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel187" runat="server" LocalizedTag="SPAM_MESSAGE_HANDLING" LocalizedPage="ADMIN_HOSTSETTINGS" Suffix=":" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="SpamMessageHandling" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedTag="HEADER_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel17" runat="server" LocalizedTag="DISABLE_REGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DisableRegistrations" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel18" runat="server" LocalizedTag="LOGIN_REDIR_URL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="CustomLoginRedirectUrl" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel19" runat="server" LocalizedTag="REQUIRE_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="RequireLogin" runat="server" />
                        </td>
                    </tr>
                    <tr>
                         <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel191" runat="server" LocalizedTag="ENABLE_SSO" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowSingleSignOn" runat="server" />
                        </td>
                    </tr>
                    <tr>
                         <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel192" runat="server" LocalizedTag="SSO_AUTO_REGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="RegisterNewFacebookUser" runat="server" />
                        </td>
                    </tr>
                    <tr>
                         <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel20" runat="server" LocalizedTag="MODAL_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseLoginBox" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel22" runat="server" LocalizedTag="HEADER_IMAGE_ATTACH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel21" runat="server" LocalizedTag="DISPLAY_TRESHOLD_IMGATTACH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PictureAttachmentDisplayTreshold" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel22" runat="server" LocalizedTag="IMAGE_ATTACH_RESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableImageAttachmentResize" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel217" runat="server" LocalizedTag="POSTED_IMAGE_RESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ResizePostedImages" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel23" runat="server" LocalizedTag="IMAGE_RESIZE_WIDTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ImageAttachmentResizeWidth" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel24" runat="server" LocalizedTag="IMAGE_RESIZE_HEIGHT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ImageAttachmentResizeHeight" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel25" runat="server" LocalizedTag="CROP_IMAGE_ATTACH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ImageAttachmentResizeCropped" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
    </div>
    <div id="View2">
        <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="HEADER_FEATURES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel193" runat="server" LocalizedTag="USE_READ_TRACKING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseReadTrackingByDatabase" runat="server"></asp:CheckBox>
                        </td>
                    </tr>       
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel86" runat="server" LocalizedTag="USE_FARSI_CALENDER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseFarsiCalender" runat="server"></asp:CheckBox>
                        </td>
                    </tr>    
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel91" runat="server" LocalizedTag="SHOW_RELATIVE_TIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowRelativeTime" runat="server"></asp:CheckBox>
                        </td>
                    </tr> 
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel92" runat="server" LocalizedTag="TIMEAGO_INTERVAL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox ID="RelativeTimeRefreshTime" Style="width:350px" runat="server"></asp:TextBox>
                        </td>
                    </tr>                             
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel93" runat="server" LocalizedTag="DYNAMIC_METATAGS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AddDynamicPageMetaTags" runat="server"></asp:CheckBox>
                        </td>
                    </tr> 
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel94" runat="server" LocalizedTag="ALLOW_DISPLAY_GENDER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowGenderInUserBox" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel202" runat="server" LocalizedTag="ALLOW_DISPLAY_COUNTRY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowCountryInfoInUserBox" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel95" runat="server" LocalizedTag="ALLOW_USER_HIDE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserHideHimself" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel96" runat="server" LocalizedTag="ENABLE_DISPLAY_NAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableDisplayName" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel85" runat="server" LocalizedTag="STYLED_NICKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox runat="server" ID="UseStyledNicks" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel209" runat="server" LocalizedTag="STYLED_TOPIC_TITLES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox runat="server" ID="UseStyledTopicTitles" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel97" runat="server" LocalizedTag="ALLOW_MODIFY_DISPLAYNAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowDisplayNameModification" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="AllowMessageDescriptionHelpLabel" runat="server" LocalizedTag="ALLOW_MESSAGEDESCRIPTION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowMessageDescription" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
            
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel98" runat="server" LocalizedTag="MEMBERLIST_PAGE_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="MemberListPageSize" />                          
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel139" runat="server" LocalizedTag="MYTOPICSLIST_PAGE_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="MyTopicsListPageSize" />                          
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel99" runat="server" LocalizedTag="SHOW_USER_STATUS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowUserOnlineStatus" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel100" runat="server" LocalizedTag="ALLOW_THANKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableThanksMod" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel102" runat="server" LocalizedTag="SHOW_THANK_DATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowThanksDate" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel101" runat="server" LocalizedTag="ENABLE_BUDDYLIST" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableBuddyList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel103" runat="server" LocalizedTag="REMOVE_NESTED_QUOTES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="RemoveNestedQuotes" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel229" runat="server" LocalizedTag="FORUM_DESCCANBENULL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ForumDescriptionCanBeNull" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel104" runat="server" LocalizedTag="DISABLE_NOFOLLOW_ONOLDERPOSTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="DisableNoFollowLinksAfterDay" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel107" runat="server" LocalizedTag="DAYS_BEFORE_POSTLOCK" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="LockPosts" runat="server"></asp:TextBox>
                        </td>
                    </tr>                            
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel111" runat="server" LocalizedTag="ALLOW_POSTBLOG" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPostToBlog" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel113" runat="server" LocalizedTag="ALLOW_QUICK_ANSWER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowQuickAnswer" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel105" runat="server" LocalizedTag="ALLOW_SHARE_TOPIC" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ShowShareTopicTo" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel184" runat="server" LocalizedTag="ENABLE_RETWEET_MSG" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ShowRetweetMessageTo" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel173" runat="server" LocalizedTag="TWITTER_USERNAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="TwitterUserName" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel112" runat="server" LocalizedTag="ALLOW_EMAIL_TOPIC" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailTopic" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel188" runat="server" LocalizedTag="ALLOW_TOPIC_DESCRIPTION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableTopicDescription" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel194" runat="server" LocalizedTag="ALLOW_TOPIC_STATUS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableTopicStatus" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel218" runat="server" LocalizedTag="ENABLE_HOVERCARDS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableUserInfoHoverCards" runat="server"></asp:CheckBox>
                        </td>
                    </tr><tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel225" runat="server" LocalizedTag="ALLOW_TOPICIMAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowTopicImages" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                               <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel228" runat="server" LocalizedTag="ALLOW_REMOTETOPICIMAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AlowRemoteTopicImages" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel226" runat="server" LocalizedTag="TOPICIMAGE_WIDTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox ID="TopicImageWidth" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel227" runat="server" LocalizedTag="TOPICIMAGE_HEIGHT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox ID="TopicImageHeight" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel37" runat="server" LocalizedTag="HEADER_TOPICTAGS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel220" runat="server" LocalizedTag="ALLOW_TOPICTAGS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowTopicTags" runat="server"></asp:CheckBox>
                        </td>
                    </tr> 
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel224" runat="server" LocalizedTag="SHOW_TOPICTAGSCOUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowNumberOfTags" runat="server"></asp:CheckBox>
                        </td>
                    </tr> 
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel221" runat="server" LocalizedTag="ALLOW_AUTOTOPICTAGS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowAutoTopicTags" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="TagTopicMaxCountLbl" runat="server" LocalizedTag="TAG_TOPICMAXCOUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox ID="TagTopicMaxCount" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="TagMaxLengthLbl" runat="server" LocalizedTag="TAG_MAXLENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox ID="TagMaxLength" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="ShowBoardTagsLbl" runat="server" LocalizedTag="SHOW_BOARDTAGS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowBoardTags" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="BoardTagsPerPageLbl" runat="server" LocalizedTag="BOARDTAGPERPAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox ID="BoardTagsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel222" runat="server" LocalizedTag="TAG_FORBIDDENSYMB" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox ID="TagForbiddenSymbols" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel230" runat="server" LocalizedTag="HOVERCARD_DELAY" LocalizedPage="ADMIN_HOSTSETTINGS" />   
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="HoverCardOpenDelay" runat="server"></asp:TextBox>
                        </td>   
                    </tr>
         
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel23" runat="server" LocalizedTag="HEADER_POLL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel115" runat="server" LocalizedTag="MAX_ALLOWED_POLLS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AllowedPollNumber" MaxLength="2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel116" runat="server" LocalizedTag="MAX_ALLOWED_CHOICES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AllowedPollChoiceNumber" MaxLength="2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel117" runat="server" LocalizedTag="POLLVOTING_PERIP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="PollVoteTiedToIP" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel118" runat="server" LocalizedTag="ALLOW_CHANGE_AFTERVOTE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPollChangesAfterFirstVote" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel119" runat="server" LocalizedTag="ALLOW_MULTI_VOTING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowMultipleChoices" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel120" runat="server" LocalizedTag="ALLOW_HIDE_POLLRESULTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUsersHidePollResults" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel121" runat="server" LocalizedTag="ALLOW_GUESTS_VIEWPOLL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowGuestsViewPollOptions" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel122" runat="server" LocalizedTag="ALLOW_USERS_POLLIMAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUsersImagedPoll" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                   <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel123" runat="server" LocalizedTag="POLL_IMAGE_FILESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PollImageMaxFileSize" MaxLength="4" runat="server"></asp:TextBox>
                        </td>
                    </tr>                  
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel24" runat="server" LocalizedTag="HEADER_PMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel124" runat="server" LocalizedTag="ALLOW_PMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPrivateMessages" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel125" runat="server" LocalizedTag="ALLOW_PM_NOTIFICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPMEmailNotification" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel126" runat="server" LocalizedTag="MAX_PM_RECIPIENTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PrivateMessageMaxRecipients" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel25" runat="server" LocalizedTag="HEADER_ALBUM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel127" runat="server" LocalizedTag="ENABLE_ABLBUMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableAlbum" runat="server"></asp:CheckBox>
                        </td>
                    </tr>                    
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel128" runat="server" LocalizedTag="MAX_IMAGE_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AlbumImagesSizeMax" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel129" runat="server" LocalizedTag="ALBUMS_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AlbumsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel130" runat="server" LocalizedTag="IMAGES_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AlbumImagesPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel31" runat="server" LocalizedTag="HEADER_HOTTOPICS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>                
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel197" runat="server" LocalizedTag="POPULAR_VIEWS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PopularTopicViews" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel198" runat="server" LocalizedTag="POPULAR_REPLYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PopularTopicReplys" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel199" runat="server" LocalizedTag="POPULAR_DAYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PopularTopicDays" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel30" runat="server" LocalizedTag="HEADER_GEOLOCATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel108" runat="server" LocalizedTag="IP_INFOSERVICE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableIPInfoService" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel109" runat="server" LocalizedTag="IP_INFOSERVICE_XMLURL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="IPLocatorUrlPath" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel195" runat="server" LocalizedTag="IP_INFOSERVICE_DATAMAPPING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="IPLocatorResultsMap" runat="server"></asp:TextBox>
                        </td>
                    </tr>       
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel110" runat="server" LocalizedTag="IPINFO_ULRL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="IPInfoPageURL" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel33" runat="server" LocalizedTag="HEADER_REPUTATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel106" runat="server" LocalizedTag="DISPLAY_POINTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DisplayPoints" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel203" runat="server" LocalizedTag="ENABLE_USERREPUTATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableUserReputation" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel204" runat="server" LocalizedTag="REPUTATION_ALLOWNEGATIVE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ReputationAllowNegative" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel207" runat="server" LocalizedTag="REPUTATION_MIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ReputationMaxNegative" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel208" runat="server" LocalizedTag="REPUTATION_MAX" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ReputationMaxPositive" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel205" runat="server" LocalizedTag="REPUTATION_MINUP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ReputationMinUpVoting" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel206" runat="server" LocalizedTag="REPUTATION_MINDOWN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ReputationMinDownVoting" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel28" runat="server" LocalizedTag="HEADER_CAPTCHA" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel143" runat="server" LocalizedTag="CAPTCHA_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="CaptchaSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel144" runat="server" LocalizedTag="RECAPTCHA_PUBLIC_KEY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="RecaptchaPublicKey" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel145" runat="server" LocalizedTag="RECAPTCHA_PRIVATE_KEY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="RecaptchaPrivateKey" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel181" runat="server" LocalizedTag="RECAPTCHA_MULTI_INSTANCE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="RecaptureMultipleInstances" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel146" runat="server" LocalizedTag="CAPTCHA_GUEST_POSTING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableCaptchaForGuests" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel147" runat="server" LocalizedTag="ENABLE_CAPTCHA_FORPOST" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableCaptchaForPost" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel148" runat="server" LocalizedTag="CAPTCHA_FOR_REGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="CaptchaTypeRegister" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel35" runat="server" LocalizedTag="HEADER_MESSAGE_NOTIFICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel216" runat="server" LocalizedTag="MESSAGE_SYSTEM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="MessageNotificationSystem" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel214" runat="server" LocalizedTag="NOTIFICATION_DURATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MessageNotifcationDuration" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel215" runat="server" LocalizedTag="NOTIFICATION_MOBILE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="NotifcationNativeOnMobile" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
    </div>
    <div id="View3">
        <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="HEADER_DISPLAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel152" runat="server" LocalizedTag="ACTIVE_USERTIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ActiveListTime" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel153" runat="server" LocalizedTag="SHOW_AVATARS_TOPICLISTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowAvatarsInTopic" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel154" runat="server" LocalizedTag="SHOW_MOVED_TOPICS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowMoved" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel237" runat="server" LocalizedTag="SHOW_DELETEDINTOPICS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowDeletedTopicsInTopicListForModerators" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
            
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel155" runat="server" LocalizedTag="SHOW_MODLIST" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowModeratorList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel200" runat="server" LocalizedTag="SHOW_MODLIST_ASCOLUMN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowModeratorListAsColumn" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel156" runat="server" LocalizedTag="SHOW_GUESTS_INACTIVE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowGuestsInDetailedActiveList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel157" runat="server" LocalizedTag="SHOW_BOTS_INACTIVE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowCrawlersInActiveList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel158" runat="server" LocalizedTag="SHOW_DEL_MESSAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowDeletedMessages" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel159" runat="server" LocalizedTag="SHOW_DEL_MESSAGES_TOALL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowDeletedMessagesToAll" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="BlankLinksHelpLabel" runat="server" LocalizedTag="SHOW_LINKS_NEWWINDOW" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="BlankLinks" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="ShowLastUnreadPostHelpLabel" runat="server" LocalizedTag="SHOW_UNREAD_LINKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowLastUnreadPost" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel161" runat="server" LocalizedTag="SHOW_NOCOUNT_POSTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="NoCountForumsInActiveDiscussions" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel162" runat="server" LocalizedTag="SHOW_FORUM_STATS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowForumStatistics" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel190" runat="server" LocalizedTag="SHOW_RECENT_USERS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowRecentUsers" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                    <td class="postheader">
                    <VZF:HelpLabel ID="HelpLabel201" runat="server" LocalizedTag="SHOW_TODAYS_BIRTHDAYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                     </td>
                      <td class="post">
                      <asp:CheckBox ID="ShowTodaysBirthdays" runat="server"></asp:CheckBox>
                      </td>
                      </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel163" runat="server" LocalizedTag="SHOW_ACTIVE_DISCUSSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowActiveDiscussions" runat="server"></asp:CheckBox>
                        </td>
                    </tr>                    
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel164" runat="server" LocalizedTag="SHOW_FORUM_JUMP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowForumJump" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel167" runat="server" LocalizedTag="SHOW_GROUPS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowGroups" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel168" runat="server" LocalizedTag="SHOW_GROUPS_INPROFILE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowGroupsProfile" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel169" runat="server" LocalizedTag="SHOW_MEDALS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowMedals" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel170" runat="server" LocalizedTag="SHOW_USERSBROWSING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowBrowsingUsers" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel171" runat="server" LocalizedTag="SHOW_RENDERTIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowPageGenerationTime" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel172" runat="server" LocalizedTag="SHOW_YAFVERSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowYAFVersion" runat="server"></asp:CheckBox>
                        </td>
                    </tr>        
                    <tr>
                       <td class="postheader" style="width:450px">
                            <VZF:HelpLabel ID="HelpLabel182" runat="server" LocalizedTag="SHOWHELP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ShowHelpTo" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:450px">
                            <VZF:HelpLabel ID="HelpLabel183" runat="server" LocalizedTag="SHOWTEAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ShowTeamTo" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel174" runat="server" LocalizedTag="SHOW_JOINDATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DisplayJoinDate" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel175" runat="server" LocalizedTag="RULES_ONREGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowRulesForRegistration" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel176" runat="server" LocalizedTag="LASTPOST_COUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ActiveDiscussionsCount" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel177" runat="server" LocalizedTag="NOFOLLOW_LINKTAGS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseNoFollowLinks" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel178" runat="server" LocalizedTag="POSTS_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PostsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel179" runat="server" LocalizedTag="TOPICS_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="TopicsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel223" runat="server" LocalizedTag="MOSTACTIVEUSER_DAYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MostActiveUserDays" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel38" runat="server" LocalizedTag="HEADER_SHOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel165" runat="server" LocalizedTag="SHOW_SHOUTBOX" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowShoutbox" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel114" runat="server" LocalizedTag="VIEWSHOUTBOX_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ShoutboxViewPermissions" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel236" runat="server" LocalizedTag="SHOUTBOX_DEFAULTSTATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ShoutboxDefaultState" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel166" runat="server" LocalizedTag="SHOW_SHOUTBOX_SMILIES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowShoutboxSmiles" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel219" runat="server" LocalizedTag="SHOUTBOX_COUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ShoutboxShowMessageCount" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
    </div>
    <div id="View4">
        <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="HEADER_ADVERTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader"  style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel26" runat="server" LocalizedTag="POST_AD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px;" TextMode="MultiLine" runat="server" ID="AdPost" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel27" runat="server" LocalizedTag="SHOWAD_LOGINUSERS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox runat="server" ID="ShowAdsToSignedInUsers" />
                        </td>
                    </tr>
                </table>
    </div>
    <div id="View5">
        <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="HEADER_EDITORS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader"  style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel88" runat="server" LocalizedTag="FORUM_EDITOR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ForumEditor" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel160" runat="server" LocalizedTag="ALLOW_USERTEXTEDITOR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox runat="server" ID="AllowUsersTextEditor" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel87" runat="server" LocalizedTag="ACCEPT_HTML" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px;" ID="AcceptedHTML" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                </table>
    </div>
    <div id="View6">
        <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="HEADER_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel84" runat="server" LocalizedTag="USER_CHANGE_THEME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserTheme" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel83" runat="server" LocalizedTag="USER_CHANGE_LANGUAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserLanguage" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel82" runat="server" LocalizedTag="ALLOW_SIGNATURE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowSignatures" runat="server"></asp:CheckBox>
                        </td>
                    </tr>                   
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel81" runat="server" LocalizedTag="ALLOW_SENDMAIL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailSending" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel80" runat="server" LocalizedTag="ALLOW_EMAIL_CHANGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailChange" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel79" runat="server" LocalizedTag="ALLOW_PASS_CHANGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPasswordChange" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel78" runat="server" LocalizedTag="ALLOW_MOD_VIEWIP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowModeratorsViewIPs" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel77" runat="server" LocalizedTag="ALLOW_NOTIFICATION_ONALL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowNotificationAllPostsAllTopics" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel76" runat="server" LocalizedTag="REPORT_POST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ReportPostPermissions" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                         <td class="postheader">
                             <VZF:HelpLabel ID="HelpLabel89" runat="server" LocalizedTag="ALLOW_TOPICS_DUPLICATENAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="AllowCreateTopicsSameName" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                             <VZF:HelpLabel ID="HelpLabel90" runat="server" LocalizedTag="ALLOW_FORUMS_DUPLICATENAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                       <td class="post">
                            <asp:CheckBox ID="AllowForumsWithSameName" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel75" runat="server" LocalizedTag="VIEWPROFILE_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ProfileViewPermissions" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel74" runat="server" LocalizedTag="VIEWMEMBERLIST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="MembersListViewPermissions" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel73" runat="server" LocalizedTag="VIEWACTIVE_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ActiveUsersViewPermissions" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel72" runat="server" LocalizedTag="MAX_WORD_LENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MaxWordLength" MaxLength="2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel71" runat="server" LocalizedTag="SSL_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseSSLToLogIn" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                           <VZF:HelpLabel ID="HelpLabel70" runat="server" LocalizedTag="SSL_REGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseSSLToRegister" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
    </div>
    <div id="View7">
        <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="HEADER_TEMPLATES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel57" runat="server" LocalizedTag="USERBOX_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBox" TextMode="MultiLine" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel58" runat="server" LocalizedTag="AVATAR_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxAvatar" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel59" runat="server" LocalizedTag="MEDALS_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxMedals" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel60" runat="server" LocalizedTag="RANKIMAGE_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxRankImage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel61" runat="server" LocalizedTag="RANK_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxRank" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel62" runat="server" LocalizedTag="GROUPS_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxGroups" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel63" runat="server" LocalizedTag="JOINDATE_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxJoinDate" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel64" runat="server" LocalizedTag="POSTS_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxPosts" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel65" runat="server" LocalizedTag="REPUTATION_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxReputation" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel196" runat="server" LocalizedTag="COUNTRYIMAGE_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxCountryImage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel66" runat="server" LocalizedTag="LOCATION_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxLocation" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel67" runat="server" LocalizedTag="GENDER_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxGender" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel68" runat="server" LocalizedTag="THANKS_FROM_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxThanksFrom" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel69" runat="server" LocalizedTag="THANKS_TO_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxThanksTo" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
    </div>
    <div id="View8">
        <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedTag="HEADER_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel189" runat="server" LocalizedTag="AVATAR_GALLERY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarGallery" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel50" runat="server" LocalizedTag="REMOTE_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarRemote" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel51" runat="server" LocalizedTag="AVATAR_UPLOAD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarUpload" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel52" runat="server" LocalizedTag="ALLOW_GRAVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarGravatar" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel53" runat="server" LocalizedTag="GRAVATAR_RATING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="GravatarRating" runat="server">
                                <asp:ListItem Value="G"></asp:ListItem>
                                <asp:ListItem Value="PG"></asp:ListItem>
                                <asp:ListItem Value="R"></asp:ListItem>
                                <asp:ListItem Value="X"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel54" runat="server" LocalizedTag="AVATAR_WIDTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AvatarWidth" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel55" runat="server" LocalizedTag="AVATAR_HEIGHT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AvatarHeight" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel56" runat="server" LocalizedTag="AVATAR_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AvatarSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
    </div>
    <div id="View9">
        <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedTag="HEADER_CACHE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel41" runat="server" LocalizedTag="STATS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="ForumStatisticsCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="ForumStatisticsCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="ForumStatisticsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel42" runat="server" LocalizedTag="USRSTATS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="BoardUserStatsCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="BoardUserStatsCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="BoardUserStatsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel43" runat="server" LocalizedTag="DISCUSSIONS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="ActiveDiscussionsCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="ActiveDiscussionsCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="ActiveDiscussionsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel44" runat="server" LocalizedTag="CAT_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="BoardCategoriesCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="BoardCategoriesCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="BoardCategoriesCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel45" runat="server" LocalizedTag="MOD_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="BoardModeratorsCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="BoardModeratorsCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="BoardModeratorsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel46" runat="server" LocalizedTag="REPLACE_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="ReplaceRulesCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="ReplaceRulesCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="ReplaceRulesCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel47" runat="server" LocalizedTag="SEO_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="FirstPostCacheTimeout" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel48" runat="server" LocalizedTag="ONLINE_STATUS_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="OnlineStatusCacheTimeout" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel49" runat="server" LocalizedTag="LAZY_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="ActiveUserLazyDataCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="ActiveUserLazyDataCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>' runat="server" OnClick="UserLazyDataCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="footer1" colspan="2" style="text-align:center">
                           <asp:Button  CssClass="pbutton" runat="server" ID="ResetCacheAll" Text='<%# this.GetText("ADMIN_HOSTSETTINGS", "CLEAR_CACHE") %>'  OnClick="ResetCacheAll_Click" />
                        </td>
                    </tr>
                </table>
    </div>
    <div id="View10">
                <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedTag="HEADER_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader"  style="width:50%">
                            <VZF:HelpLabel ID="HelpLabel28" runat="server" LocalizedTag="MAX_SEARCH_RESULTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%" ID="ReturnSearchMax" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel29" runat="server" LocalizedTag="SQL_FULLTEXT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseFullTextSearch" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel30" runat="server" LocalizedTag="SEARCH_MINLENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%" ID="SearchStringMinLength" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel31" runat="server" LocalizedTag="SEARCH_MAXLENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%" ID="SearchStringMaxLength" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel32" runat="server" LocalizedTag="SEARCH_PATTERN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%" ID="SearchStringPattern" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel33" runat="server" LocalizedTag="SEARCH_PERMISS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:99%" ID="SearchPermissions" runat="server">
                               </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel34" runat="server" LocalizedTag="SEARCH_ENGINE1" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px" ID="SearchEngine1" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel35" runat="server" LocalizedTag="SEARCH_ENGINE1_PARAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px" ID="SearchEngine1Parameters" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel36" runat="server" LocalizedTag="SEARCH_ENGINE2" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px" ID="SearchEngine2" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel40" runat="server" LocalizedTag="SEARCH_ENGINE2_PARAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px" ID="SearchEngine2Parameters" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel37" runat="server" LocalizedTag="EXTERN_SEARCH_PERMISS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:99%" ID="ExternalSearchPermissions" runat="server">                               
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel38" runat="server" LocalizedTag="EXTERN_NEWWINDOW" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ExternalSearchInNewWindow" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel39" runat="server" LocalizedTag="QUICK_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowQuickSearch" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
        </div>
        <div id="View11">
                <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel29" runat="server" LocalizedTag="HEADER_LOG" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel140" runat="server" LocalizedTag="EVENTLOG_MAX_MESSAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="EventLogMaxMessages" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel141" runat="server" LocalizedTag="EVENTLOG_MAX_DAYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="EventLogMaxDays" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel149" runat="server" LocalizedTag="MESSAGE_CHANGE_HISTORY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MessageHistoryDaysToLog" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel150" runat="server" LocalizedTag="ENABLE_LOCATIONPATH_ERRORS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableActiveLocationErrorsLog" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel151" runat="server" LocalizedTag="UNHANDLED_USERAGENT_LOG" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UserAgentBadLog" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel27" runat="server" LocalizedTag="HEADER_LOGSCOPE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="LogErrorLabel" runat="server" LocalizedTag="LOG_ERROR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="LogError" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                      <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel142" runat="server" LocalizedTag="LOG_WARNING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="LogWarning" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                      <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel180" runat="server" LocalizedTag="LOG_INFORMATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="LogInformation" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                      <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel210" runat="server" LocalizedTag="LOG_SQLERROR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="LogSqlError" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                      <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel211" runat="server" LocalizedTag="LOG_BANNEDIP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="LogBannedIP" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel212" runat="server" LocalizedTag="LOG_USERDELETED" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="LogUserDeleted" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel213" runat="server" LocalizedTag="LOG_SUSPENDEDANDCONTRA" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="LogUserSuspendedUnsuspended" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
        </div>
              <div id="View12">
                <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                     <tr>
                        <td class="header1" colspan="2">
                            <VZF:LocalizedLabel ID="LocalizedLabel26" runat="server" LocalizedTag="HEADER_SYNDICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel131" runat="server" LocalizedTag="SHOW_RSS_LINKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowRSSLink" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel132" runat="server" LocalizedTag="SHOW_ATOM_LINKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowAtomLink" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="TOPICFEED_COUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="TopicsFeedItemsCount" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel133" runat="server" LocalizedTag="POSTS_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>              
                        <td class="post">
                              <asp:DropDownList Style="width:350px" ID="PostsFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel134" runat="server" LocalizedTag="LASTPOSTS_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>              
                        <td class="post">
                              <asp:DropDownList Style="width:350px" ID="PostLatestFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel135" runat="server" LocalizedTag="FORUM_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>              
                        <td class="post">
                              <asp:DropDownList Style="width:350px" ID="ForumFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel136" runat="server" LocalizedTag="TOPIC_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>              
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="TopicsFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel137" runat="server" LocalizedTag="ACTIVETOPIC_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>              
                        <td class="post">
                              <asp:DropDownList Style="width:350px" ID="ActiveTopicFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <VZF:HelpLabel ID="HelpLabel138" runat="server" LocalizedTag="FAVTOPIC_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>              
                        <td class="post">
                              <asp:DropDownList Style="width:350px" ID="FavoriteTopicFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>        
                </table>
        </div>
    </asp:Panel>
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="postfooter" align="center">
                <asp:Button ID="Save" runat="server" Text='<%# this.GetText("ADMIN_HOSTSETTINGS", "SAVE_SETTINGS") %>'  CssClass="pbutton" OnClick="Save_Click">
                </asp:Button>
            </td>
        </tr>
    </table>
</VZF:AdminMenu>
<asp:HiddenField runat="server" ID="hidLastTab" Value="0" />
<asp:HiddenField runat="server" ID="hidLastTabId" Value="0" />
<VZF:SmartScroller ID="SmartScroller1" runat="server" />
