﻿namespace YAF.Editors
{
  #region Using

  using System;
  using System.Linq;
  using System.Reflection;
  using System.Web.UI;

  using YAF.Core;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The rich class editor.
  /// </summary>
  public abstract class RichClassEditor : ForumEditor
  {
    #region Constants and Fields

    /// <summary>
    ///   The _editor.
    /// </summary>
    protected Control _editor;

    /// <summary>
    ///   The _init.
    /// </summary>
    protected bool _init;

    /// <summary>
    ///   The _style sheet.
    /// </summary>
    protected string _styleSheet;

    /// <summary>
    ///   The _typ editor.
    /// </summary>
    protected Type _typEditor;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "RichClassEditor" /> class.
    /// </summary>
    protected RichClassEditor()
    {
      this._init = false;
      this._styleSheet = string.Empty;
      this._editor = null;
      this._typEditor = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RichClassEditor"/> class.
    /// </summary>
    /// <param name="classBinStr">
    /// The class bin str.
    /// </param>
    protected RichClassEditor([NotNull] string classBinStr)
    {
      this._init = false;
      this._styleSheet = string.Empty;
      this._editor = null;

      try
      {
        this._typEditor = Type.GetType(classBinStr, true);
      }
      catch (Exception)
      {
        /*
#if DEBUG
				throw new Exception( "Unable to load editor class/dll: " + classBinStr + " Exception: " + x.Message );
#else
				VZF.Classes.Data.CommonDb.eventlog_create(null, this.GetType().ToString(), x, EventLogTypes.Error);
#endif
*/
      }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets a value indicating whether Active.
    /// </summary>
    public override bool Active
    {
      get
      {
        return this._typEditor != null;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether IsInitialized.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        return this._init;
      }
    }

    /// <summary>
    ///   Gets or sets StyleSheet.
    /// </summary>
    public override string StyleSheet
    {
      get
      {
        return this._styleSheet;
      }

      set
      {
        this._styleSheet = value;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether UsesBBCode.
    /// </summary>
    public override bool UsesBBCode
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether UsesHTML.
    /// </summary>
    public override bool UsesHTML
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    ///   Gets SafeID.
    /// </summary>
    [NotNull]
    protected virtual string SafeID
    {
      get
      {
        if (this._init)
        {
          return this._editor.ClientID.Replace("$", "_");
        }

        return string.Empty;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get interface in assembly.
    /// </summary>
    /// <param name="assembly">
    /// The c assembly.
    /// </param>
    /// <param name="className">
    /// The class name.
    /// </param>
    /// <returns>
    /// </returns>
    protected Type GetInterfaceInAssembly([NotNull] Assembly assembly, [NotNull] string className)
    {
      CodeContracts.ArgumentNotNull(assembly, "assembly");
      CodeContracts.ArgumentNotNull(className, "className");

      return assembly.GetExportedTypes().FirstOrDefault(typ => typ.FullName == className);
    }

    /// <summary>
    /// The init editor object.
    /// </summary>
    /// <returns>
    /// The init editor object.
    /// </returns>
    protected bool InitEditorObject()
    {
      try
      {
        if (!this._init && this._typEditor != null)
        {
          // create instance of main class
          this._editor = (Control)Activator.CreateInstance(this._typEditor);
          this._init = true;
        }
      }
      catch (Exception)
      {
        // dll is not accessible
        return false;
      }

      return true;
    }

    #endregion
  }
}