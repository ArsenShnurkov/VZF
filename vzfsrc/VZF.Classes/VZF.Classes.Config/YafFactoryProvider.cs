#region copyright
/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 *
 * http://www.yetanotherforum.net/
 *
 * This file can contain some changes in 2014-2016 by Vladimir Zakharov(vzrus)
 * for VZF forum
 *
 * http://www.code.coolhobby.ru/
 * 
 * File YafFactoryProvider.cs created  on 2.6.2015 in  6:29 AM.
 * Last changed on 5.21.2016 in 1:08 PM.
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
#endregion

namespace YAF.Classes
{
  #region Using

  using System;

  using YAF.Classes.Pattern;
  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The yaf provider.
  /// </summary>
  public static class YafFactoryProvider
  {
    #region Constants and Fields

    /// <summary>
    /// The _builder factory.
    /// </summary>
    private static ITypeFactoryInstance<IUrlBuilder> _builderFactory;

    /// <summary>
    /// The _user display name factory.
    /// </summary>
    private static ITypeFactoryInstance<IUserDisplayName> _userDisplayNameFactory;

    #endregion

    #region Properties

    /// <summary>
    /// Gets UrlBuilder.
    /// </summary>
    public static IUrlBuilder UrlBuilder
    {
      get
      {
        if (_builderFactory == null)
        {
          _builderFactory = new TypeFactoryInstanceApplicationBoardScope<IUrlBuilder>(UrlBuilderType);
        }

        return _builderFactory.Get();
      }
    }

    /// <summary>
    /// Gets current <see cref="IUserDisplayName"/>.
    /// </summary>
    public static IUserDisplayName UserDisplayName
    {
      get
      {
        if (_userDisplayNameFactory == null)
        {
          _userDisplayNameFactory = new TypeFactoryInstanceApplicationBoardScope<IUserDisplayName>(UserDisplayNameType);
        }

        return _userDisplayNameFactory.Get();
      }
    }

    /// <summary>
    /// Gets UrlBuilderType.
    /// </summary>
    private static string UrlBuilderType
    {
      get
      {
        var urlAssembly = Config.GetProvider("UrlBuilder");

        if (!String.IsNullOrEmpty(urlAssembly))
        {
          return urlAssembly;
        }
        else if (Config.IsDotNetNuke)
        {
            urlAssembly = "YAF.DotNetNuke.DotNetNukeUrlBuilder,YAF.DotNetNuke.Module";
        }
        else if (Config.IsMojoPortal)
        {
            urlAssembly = "YAF.Mojo.MojoPortalUrlBuilder,YAF.Mojo";
        }
        else if (Config.IsRainbow)
        {
            urlAssembly = "yaf_rainbow.RainbowUrlBuilder,yaf_rainbow";
        }
        else if (Config.IsPortal)
        {
          urlAssembly = "Portal.UrlBuilder,Portal";
        }
        else if (Config.IsPortalomatic)
        {
          urlAssembly = "Portalomatic.NET.Utils.URLBuilder,Portalomatic.NET.Utils";
        }
        else if (Config.EnableUrlRewriting)
        {
          urlAssembly = "YAF.Core.RewriteUrlBuilder,VZF.Core";
        }
        else
        {
          urlAssembly = "YAF.Classes.DefaultUrlBuilder";
        }

        return urlAssembly;
      }
    }

    /// <summary>
    /// Gets UserDisplayNameType.
    /// </summary>
    private static string UserDisplayNameType
    {
      get
      {
        string urlAssembly;

        if (!String.IsNullOrEmpty(Config.GetProvider("UserDisplayName")))
        {
          urlAssembly = Config.GetProvider("UserDisplayName");
        }
        else
        {
          urlAssembly = "YAF.Classes.Core.DefaultUserDisplayName,YAF.Classes.Core";
        }

        return urlAssembly;
      }
    }

    #endregion
  }
}