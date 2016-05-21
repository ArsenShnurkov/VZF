﻿#region Using

using System;
using System.Web.Services;

using VZF.Data.Common;

using YAF.Classes;

using YAF.Core;
using YAF.Types;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using VZF.Utils;

#endregion

/// <summary>
/// Forum WebService for various Functions
/// </summary>
[WebService(Namespace = "http://yetanotherforum.net/services")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class YafWebService : WebService, IHaveServiceLocator
{
    /// <summary>
    /// Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator
    {
        get
        {
            return YafContext.Current.ServiceLocator;
        }
    }

    #region Public Methods

    /// <summary>
    /// Creates the new topic.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="forumid">The forumid.</param>
    /// <param name="userid">The userid.</param>
    /// <param name="username">The username.</param>
    /// <param name="status">The status.</param>
    /// <param name="styles">The styles.</param>
    /// <param name="description">The description.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="post">The post.</param>
    /// <param name="ip">The ip.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="flags">The flags.</param>
    /// <returns>
    /// Returns the new Message Id
    /// </returns>
    /// <exception cref="SecurityFailureInvalidWebServiceTokenException">
    /// Invalid Secure Web Service Token: Operation Failed
    /// </exception>
    [WebMethod]
    public long CreateNewTopic(
        [NotNull] string token,
        int forumid,
        int userid,
        [NotNull] string username,
        [CanBeNull] string status,
        [CanBeNull] string styles,
        [CanBeNull] string description,
        [NotNull] string subject,
        [NotNull] string post,
        [NotNull] string ip,
        int priority,
        int flags,
        string tags)
    {
        // validate token...
        if (token != YafContext.Current.Get<YafBoardSettings>().WebServiceToken)
        {
            throw new SecurityFailureInvalidWebServiceTokenException(
                "Invalid Secure Web Service Token: Operation Failed");
        }

        long messageId = 0;
        string subjectEncoded = this.Server.HtmlEncode(subject);

        return CommonDb.topic_save(
            YafContext.Current.PageModuleID,
            forumid,
            subjectEncoded,
            status,
            styles,
            description,
            post,
            userid,
            priority,
            this.User != null ? null : (UserMembershipHelper.FindUsersByName(username).Count > 0) ? "{0}_{1}".FormatWith(this.Get<ILocalization>().GetText("COMMON", "GUEST_NAME"), username) : username,
            ip,
            DateTime.UtcNow,
            string.Empty,
            flags,
            null,
            ref messageId, 
            tags);
    }

    /// <summary>
    /// Sets the display name from username.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="username">The username.</param>
    /// <param name="displayName">The display Name.</param>
    /// <returns>
    /// The set display name from username.
    /// </returns>
    /// <exception cref="Exception">
    ///   <c>Exception</c>.
    ///   </exception>
    /// <exception cref="NonUniqueDisplayNameException">
    ///   <c>NonUniqueDisplayNameException</c>.
    ///   </exception>
    /// <exception cref="SecurityFailureInvalidWebServiceTokenException">Invalid Secure Web Service Token: Operation Failed</exception>
    [WebMethod]
    public bool SetDisplayNameFromUsername(
        [NotNull] string token, [NotNull] string username, [NotNull] string displayName)
    {
        // validate token...
        if (token != YafContext.Current.Get<YafBoardSettings>().WebServiceToken)
        {
            throw new SecurityFailureInvalidWebServiceTokenException(
                "Invalid Secure Web Service Token: Operation Failed");
        }

        // get the user id...
        var membershipUser = UserMembershipHelper.GetMembershipUserByName(username);

        if (membershipUser != null)
        {
            var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey);

            var displayNameId = this.Get<IUserDisplayName>().GetId(displayName);

            if (displayNameId.HasValue && displayNameId.Value != userId)
            {
                // problem...
                throw new NonUniqueDisplayNameException(
                    "Display Name must be unique. {0} display name already exists in the database.".FormatWith(
                        displayName));
            }

            var userFields = CommonDb.user_list((int?) YafContext.Current.PageModuleID, Config.BoardId, userId, null).Rows[0];

            CommonDb.user_save(YafContext.Current.PageModuleID, userId,
                Config.BoardId,
                null,
                displayName,
                null,
                userFields["TimeZone"],
                userFields["LanguageFile"],
                userFields["Culture"],
                userFields["ThemeFile"],
                userFields["UseSingleSignOn"],
                userFields["TextEditor"],
                null,
                null,
                null,
                null,
                null,
                null,
                null, 
                userFields["TopicsPerPage"], 
                userFields["PostsPerPage"]);

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));

            return true;
        }

        return false;
    }

    #endregion
}

/// <summary>
/// The security failure invalid web service token exception.
/// </summary>
[Serializable]
public class SecurityFailureInvalidWebServiceTokenException : Exception
{
  #region Constructors and Destructors

  /// <summary>
  /// Initializes a new instance of the <see cref="SecurityFailureInvalidWebServiceTokenException"/> class.
  /// </summary>
  /// <param name="message">
  /// The message.
  /// </param>
  public SecurityFailureInvalidWebServiceTokenException([NotNull] string message)
    : base(message)
  {
  }

  #endregion
}

/// <summary>
/// The non unique display name exception.
/// </summary>
[Serializable]
public class NonUniqueDisplayNameException : Exception
{
  #region Constructors and Destructors

  /// <summary>
  /// Initializes a new instance of the <see cref="NonUniqueDisplayNameException"/> class.
  /// </summary>
  /// <param name="message">
  /// The message.
  /// </param>
  public NonUniqueDisplayNameException([NotNull] string message)
    : base(message)
  {
  }

  #endregion
}