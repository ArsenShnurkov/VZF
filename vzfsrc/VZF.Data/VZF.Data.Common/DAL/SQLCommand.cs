﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Vladimir Zakharov" file="SQLCommand.cs">
//   VZF by vzrus
//   Copyright (C) 2006-2014 Vladimir Zakharov
//   https://github.com/vzrus
//   http://sourceforge.net/projects/yaf-datalayers/
//    This program is free software; you can redistribute it and/or
//   modify it under the terms of the GNU General Public License
//   as published by the Free Software Foundation; version 2 only 
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//    
//    You should have received a copy of the GNU General Public License
//   along with this program; if not, write to the Free Software
//   Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. 
// </copyright>
// <summary>
//   The DataSourceInformation.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace VZF.Data.Common.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;

    using VZF.Data.Utils;
    using VZF.Utils;
    using VZF.Utils.Helpers;

    using YAF.Classes;

    /// <summary>
    /// The sql command.
    /// </summary>
    public sealed class SQLCommand : IDisposable
    {
        /// <summary>
        /// The _dataSourceDictionary object.
        /// </summary>
        private static readonly Dictionary<string, DataSource> 
                _dataSourceDictionary = new Dictionary<string, DataSource>();

        /// <summary>
        /// The _sync object.
        /// </summary>
        private static readonly object _syncObject = new object();

        /// <summary>
        /// The _parameters.
        /// </summary>
        private readonly ParameterDictionary _parameters = 
                new ParameterDictionary();

        /// <summary>
        /// The _command text.
        /// </summary>
        private readonly StringBuilder _commandText = new StringBuilder();

        /// <summary>
        /// The _in transaction.
        /// </summary>
        private bool _inTransaction;

        private IsolationLevel _insolationLevel = IsolationLevel.ReadUncommitted;

        /// <summary>
        /// The _data source.
        /// </summary>
        private DataSource _dataSource;

        /// <summary>
        /// The _transaction.
        /// </summary>
        private DbTransaction _transaction;

        /// <summary>
        /// The _disposed.
        /// </summary>
        private int _disposed;

        /// <summary>
        /// The _connection always opened.
        /// </summary>
        private bool _connectionAlwaysOpened = false;

        /// <summary>
        /// The _connection.
        /// </summary>
        private DbConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLCommand"/> class.
        /// </summary>
        /// <param name="connectionName">
        /// The connection name.
        /// </param>
        public SQLCommand(string connectionName)
        {
            
            this.Initialize(connectionName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLCommand"/> class.
        /// </summary>
        /// <param name="connectionName">
        /// The connection name.
        /// </param>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        public SQLCommand(string connectionName, string connectionString, string providerName)
        {
            foreach (var adapter in Config.NetAdaptersOpenedConnection.Split(','))
            {
                if (providerName.StartsWith(adapter))
                {
                    this._connectionAlwaysOpened = true;
                }
            }

            DataSource dataSource;
            lock (_syncObject)
            {
                if (!_dataSourceDictionary.TryGetValue(
                          connectionName, out dataSource))
                {
                    dataSource = new DataSource(connectionName, connectionString, providerName);
                    _dataSourceDictionary.Add(connectionName, dataSource);
                }
            }

            this._dataSource = dataSource;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SQLCommand"/> class. 
        /// </summary>
        ~SQLCommand()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        public StringBuilder CommandText
        {
            get { return this._commandText; }
        }


        /// <summary>
        /// Gets a value indicating whether in transaction.
        /// </summary>
        public bool InTransaction
        {
            get { return this._inTransaction; }
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        public DataSource DataSource
        {
            get { return this._dataSource; }
        }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        public DbTransaction Transaction
        {
            get { return this._transaction; }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public ParameterDictionary Parameters
        {
            get { return this._parameters; }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="connectionName">
        /// The connection name.
        /// </param>
        public void Initialize(string connectionName)
        {
            DataSource dataSource;
            lock (_syncObject)
            {
                dataSource = this.GetDataSource(connectionName);
            }

            this._dataSource = dataSource;
        }

        /// <summary>
        /// The get data source.
        /// </summary>
        /// <param name="connectionName">
        /// The connection name.
        /// </param>
        /// <returns>
        /// The <see cref="DataSource"/>.
        /// </returns>
        private DataSource GetDataSource(string connectionName)
        {
            DataSource dataSource;
            if (_dataSourceDictionary.TryGetValue(connectionName, out dataSource))
            {
                return dataSource;
            }

            dataSource = new DataSource(connectionName);
            _dataSourceDictionary.Add(connectionName, dataSource);

            return dataSource;
        }

        /// <summary>
        /// The execute reader.
        /// </summary>
        /// <returns>
        /// The <see cref="DbDataReader"/>.
        /// </returns>
        public DbDataReader ExecuteReader()
        {
            var behavior = this.InTransaction ? 
                CommandBehavior.Default : CommandBehavior.CloseConnection;

            return this.ExecuteReader(behavior, CommandType.Text, 30);
        }

        /// <summary>
        /// The execute reader.
        /// </summary>
        /// <param name="commandBehavior">
        /// The command behavior.
        /// </param>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="commandTimeOut">
        /// The command time out.
        /// </param>
        /// <returns>
        /// The <see cref="DbDataReader"/>.
        /// </returns>
        public DbDataReader ExecuteReader(CommandBehavior commandBehavior, CommandType commandType, int? commandTimeOut)
        {
            var conn = this.GetConnection();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = this._commandText.ToString();
                    cmd.CommandType = commandType;
                    cmd.CommandTimeout = commandTimeOut ?? cmd.CommandTimeout;
                    try
                    {
                        foreach (var parameter in this.Parameters.Values)
                        {
                            cmd.Parameters.Add(parameter);
                        }

                        return cmd.ExecuteReader(commandBehavior);
                    }
                    finally
                    {
                        cmd.Parameters.Clear();
                    }
                }
            }
            finally
            {
                // this is a special case even if this object
                // is NOT part of a transaction so handle it 
                // differently than other cases
                if ((commandBehavior & CommandBehavior.CloseConnection) == 
                     CommandBehavior.CloseConnection)
                {
                    // get rid of the connection
                    // so the connection won't be reused
                    // if not in a transaction
                    // and the SQLCommand is reused.
                    this._connection = null;
                }
            }
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ExecuteScalar()
        {
            return this.ExecuteScalar(CommandType.Text, null);
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ExecuteScalar(CommandType commandType)
        {
            return this.ExecuteScalar(commandType, null);
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="commandTimeout">
        /// The command timeout.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ExecuteScalar(CommandType commandType, int? commandTimeout)
        {
            var conn = this.GetConnection();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = this._commandText.ToString();
                    cmd.CommandType = commandType;
                    cmd.CommandTimeout = commandTimeout ?? cmd.CommandTimeout;
                    try
                    {
                        foreach (var parameter in this.Parameters.Values)
                        {
                            cmd.Parameters.Add(parameter);
                        }

                        return cmd.ExecuteScalar();
                    }
                    finally
                    {
                        cmd.Parameters.Clear();
                    }
                }
            }
            finally
            {
                if (!this._connectionAlwaysOpened || !this.InTransaction)
                {
                    this.DisposeConnection();
                    this._connection = null;
                }
            }
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T ExecuteScalar<T>()
        {
            return (T)this.ExecuteScalar(CommandType.Text, null);
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T ExecuteScalar<T>(CommandType commandType)
        {
            return (T)this.ExecuteScalar(commandType, null);
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="commandTimeout">
        /// The command timeout.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T ExecuteScalar<T>(CommandType commandType, int? commandTimeout)
        {
            return (T)this.ExecuteScalar(commandType, commandTimeout);
        }

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ExecuteNonQuery(CommandType commandType)
        {
            return this.ExecuteNonQuery(commandType, null);
        }

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="commandTimeout">
        /// The command timeout.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ExecuteNonQuery(CommandType commandType, int? commandTimeout)
        {
            var conn = this.GetConnection();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = this._commandText.ToString();
                cmd.CommandTimeout = commandTimeout ?? cmd.CommandTimeout;
                cmd.CommandType = commandType;
                try
                {
                    foreach (var parameter in this.Parameters.Values)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    return cmd.ExecuteNonQuery();
                }
                finally
                {
                    cmd.Parameters.Clear();
                }
            }
        }

        /// <summary>
        /// The execute data set.
        /// </summary>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        public DataSet ExecuteDataSet()
        {
            var conn = this.GetConnection();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = this._commandText.ToString();

                        // cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.Text;
                        using (var da = this._dataSource.Factory.CreateDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            var dt = new DataSet();
                            da.Fill(dt);
                            return dt;
                        }
                    }
                    finally
                    {
                        cmd.Parameters.Clear();
                    }
                }
            }
            finally
            {
                this.DisposeConnection();
            }
        }

        /// <summary>
        /// The execute data table from reader.
        /// </summary>
        /// <param name="commandBehavior">
        /// The command behavior.
        /// </param>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="inTransaction">
        /// The in transaction.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public DataTable ExecuteDataTableFromReader(CommandBehavior commandBehavior, CommandType commandType, bool inTransaction)
        {
            this._inTransaction = inTransaction;
            var conn = this.GetConnection();
            var dt = new DataTable();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = this._commandText.ToString();
                    cmd.CommandType = commandType;
                    cmd.CommandTimeout = Config.SqlCommandTimeout.IsSet() ? Config.SqlCommandTimeout.ToType<int>() : cmd.CommandTimeout;
                    foreach (var parameter in this.Parameters.Values)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    var qc = new QueryCounter(cmd.CommandText);
                    try
                    {
                        cmd.CommandText = this._commandText.ToString();
                        
                        cmd.CommandType = commandType;
                        Trace.WriteLine(cmd.ToDebugString(), "DbAccess");

                        if (this.InTransaction)
                        {
                            using (this._transaction = conn.BeginTransaction(this._insolationLevel))
                            {
                                cmd.Transaction = this._transaction;
                               
                                IDataReader reader = cmd.ExecuteReader(commandBehavior);

                                // Retrieve column schema into our DataTable.                          
                                // dt = TableSchemaReader(dt, reader);
                                dt = GetTableColumns(dt, reader);
                                if (reader.FieldCount > 0)
                                {
                                    while (reader.Read())
                                    {
                                        DataRow dr = dt.NewRow();

                                        foreach (DataColumn column in dt.Columns)
                                        {   
                                            dr[column] = TypeChecker(column, reader[column.Ordinal]);
                                        }

                                        dt.Rows.Add(dr);
                                    }
                                }

                                reader.Close();
                                this._transaction.Commit();

                                dt.AcceptChanges();


                                return dt;
                            }
                        }
                        else
                        {
                            IDataReader reader = cmd.ExecuteReader();

                            // Retrieve column schema into our DataTable.                        
                            dt = GetTableColumns(dt, reader);
                         
                            if (reader.FieldCount > 0)
                            {
                                while (reader.Read())
                                {
                                    DataRow dr = dt.NewRow();

                                    foreach (DataColumn column in dt.Columns)
                                    {
                                        dr[column] = TypeChecker(column, reader[column.Ordinal]);
                                    }

                                    dt.Rows.Add(dr);
                                }
                            }

                            reader.Close();
                            dt.AcceptChanges();

                            return dt;
                        }
                    }
                    finally
                    {
                        cmd.Parameters.Clear();
                        qc.Dispose();
                    }
                }
            }
            finally
            { 
                this.DisposeConnection();
            }
        }

        /// <summary>
        /// The type checker.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="readerValue">
        /// The reader value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object TypeChecker(DataColumn column, object readerValue)
        {
            var o = readerValue;

            if (column.DataType.ToString() == "System.Boolean")
            {
                return readerValue.ToType<int>() == 1;
            }

            return o;
        }

        private static DataTable GetTableColumns(DataTable dummyTable, IDataReader reader)
        {
            DataTable schemaTable = reader.GetSchemaTable();
            
            foreach (DataRow myField in schemaTable.Rows)
            {
                string ts = myField["DataType"].ToString();
                if (ts == "System.Int16")
                {
                    ts = "System.Boolean";
                }

                if (ts == "System.String" && (myField["ColumnSize"].ToType<int>() == 1))
                {
                    ts = "System.Boolean";
                }

                if (!dummyTable.Columns.Contains(myField["ColumnName"].ToString()))
                {
                    dummyTable.Columns.Add(myField["ColumnName"].ToString(), Type.GetType(ts));
                }
                else
                {
                    if (!myField["ColumnName"].ToString().Contains("81_18"))
                    {
                        dummyTable.Columns.Add(myField["ColumnName"] + "81_18", Type.GetType(ts));
                    }
                }
            }

            return dummyTable;
        }



        /// <summary>
        /// The begin transaction.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void BeginTransaction()
        {
            this._transaction = this._connection.BeginTransaction();
        }

        /// <summary>
        /// The begin transaction.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level.
        /// </param>
        public void BeginTransaction( IsolationLevel isolationLevel)
         {
           this._transaction = this._connection.BeginTransaction(isolationLevel);
         }

        /// <summary>
        /// The commit transaction.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The rollback transaction.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The wrap object name.
        /// </summary>
        /// <param name="objectName">
        /// The object name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string WrapObjectName(string objectName)
        {
            return this._dataSource.WrapObjectName(objectName);
        }

        /// <summary>
        /// The create parameter.
        /// </summary>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="DbParameter"/>.
        /// </returns>
        public DbParameter CreateParameter(DbType dbType, string name, object value)
        {
            var p = this._dataSource.Factory.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            p.DbType = dbType;
            return p;
        }

        /// <summary>
        /// The generate new parameter name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GenerateNewParameterName()
        {
            return this._dataSource.GenerateNewParameterName();
        }

        /// <summary>
        /// The get parameter name.
        /// </summary>
        /// <param name="dbParameter">
        /// The db parameter.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetParameterName(DbParameter dbParameter)
        {
            return this._dataSource.GetParameterName(dbParameter.ParameterName);
        }

        /// <summary>
        /// The get parameter name.
        /// </summary>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetParameterName(string parameterName)
        {
            return this._dataSource.GetParameterName(parameterName);
        }

        /// <summary>
        /// The create parameter.
        /// </summary>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="DbParameter"/>.
        /// </returns>
        public DbParameter CreateParameter(DbType dbType, object value)
        {
            return CreateParameter(dbType, this._dataSource.GenerateNewParameterName(), value);
        }

        /// <summary>
        /// The get connection.
        /// </summary>
        /// <param name="inTransaction">
        /// The in transaction.
        /// </param>
        /// <returns>
        /// The <see cref="DbConnection"/>.
        /// </returns>
        public DbConnection GetConnection()
        {
            // While I am not going to cover it here,
            // you would get the transactions existing connection 
            // from that transaction, or if need be,
            // get a new connection for a seperate database
            if (this._connection != null && this._connection.State ==
                 ConnectionState.Closed)
            {
                this.DisposeConnection();
            }
            
            this._connection = this._connection ?? this.GetNewConnection();

            return this._connection;
        }

        /// <summary>
        /// The dispose connection.
        /// </summary>
        public void DisposeConnection()
        {
            if (!this.InTransaction && this._connection != null)
            {
                this._connection.Dispose();
                this._connection = null;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (Interlocked.Increment(ref this._disposed) == 1)
            {
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }

                if (this._connection != null)
                {
                    if (this.InTransaction)
                    {
                        // rollback 
                    }

                    this.DisposeConnection();
                }

                this._dataSource = null;
            }

            Interlocked.Exchange(ref this._disposed, 1);
        }

        /// <summary>
        /// The get new connection.
        /// </summary>
        /// <returns>
        /// The <see cref="DbConnection"/>.
        /// </returns>
        private DbConnection GetNewConnection()
        {
            return this._dataSource.GetNewConnection();
        }
    }
}

