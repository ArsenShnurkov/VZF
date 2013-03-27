﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace VZF.Data.MsSql
{
    #region Using

    using System;
    using System.Data;
    using System.Data.SqlClient;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Handlers;

    using YAF.Types.Handlers;

    #endregion

    /// <summary>
    /// Provides open/close management for DB Connections
    /// </summary>
    public class MsSqlDbConnectionManager : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The _connection.
        /// </summary>
        protected SqlConnection _connection;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafDBConnManager"/> class.
        /// </summary>
        public MsSqlDbConnectionManager(string connectionString)
        {
            // just initalize it (not open)
            this.InitConnection(connectionString);
        }

        protected MsSqlDbConnectionManager()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Events

        /// <summary>
        ///   The info message.
        /// </summary>
        public event YafDBConnInfoMessageEventHandler InfoMessage;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets ConnectionString.
        /// </summary>
        public virtual string ConnectionString
        {
            get
            {
                return Config.ConnectionString;
            }
        }


        /// <summary>
        ///   Gets the current DB Connection in any state.
        /// </summary>
        public SqlConnection DBConnection(string connectionString)
        {

            this.InitConnection(connectionString);
            return this._connection;

        }


        /// <summary>
        /// Gets an open connection to the DB. Can be called any number of times.
        /// </summary>
        public SqlConnection OpenDBConnection(string connectionString)
        {

            this.InitConnection(connectionString);

            if (this._connection.State != ConnectionState.Open)
            {
                // open it up...
                this._connection.Open();
            }

            return this._connection;

        }
        #endregion

        #region Implemented Interfaces

        #region IDbConnectionManager

        /// <summary>
        /// The close connection.
        /// </summary>
        public void CloseConnection()
        {
            if (this._connection != null && this._connection.State != ConnectionState.Closed)
            {
                this._connection.Close();
            }
        }

        /// <summary>
        /// The init connection.
        /// </summary>
        public void InitConnection(string connectionString)
        {
            if (this._connection == null)
            {
                // create the connection
                this._connection = new SqlConnection { ConnectionString = connectionString };
                this._connection.InfoMessage += new SqlInfoMessageEventHandler(this.Connection_InfoMessage);

            }
            else if (this._connection.State != ConnectionState.Open)
            {
                // verify the connection string is in there...
                this._connection.ConnectionString = connectionString;
            }

        }

        #endregion

        #region IDisposable

        /// <summary>
        /// The dispose.
        /// </summary>
        public virtual void Dispose()
        {
            // close and delete connection
            this.CloseConnection();
            this._connection = null;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The connection_ info message.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Connection_InfoMessage([NotNull] object sender, [NotNull] SqlInfoMessageEventArgs e)
        {
            if (this.InfoMessage != null)
            {
                this.InfoMessage(this, new YafDBConnInfoMessageEventArgs(e.Message));
            }
        }

        #endregion
    }
}
