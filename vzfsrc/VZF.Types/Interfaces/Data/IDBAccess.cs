/* Yet Another Forum.NET
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
namespace YAF.Types.Interfaces
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// DBAccess Interface
    /// </summary>
    public interface IDbAccess
    {
        /// <summary>
        /// Gets the current connection manager.
        /// </summary>
        /// <returns></returns>
        IDbConnectionManager GetConnectionManager(string connectionString);

        /// <summary>
        /// Sets the connection manager adapter.
        /// </summary>
        /// <typeparam name="TManager"></typeparam>
        void SetConnectionManagerAdapter<TManager>() where TManager : IDbConnectionManager;

        /// <summary>
        /// Filter list of result filters.
        /// </summary>
        IList<IDataTableResultFilter> ResultFilterList { get; }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="connectionString">
        /// The connection String.
        /// </param>
        /// <returns>
        /// The execute scalar object.
        /// </returns>
        object ExecuteScalar(IDbCommand cmd, bool transaction, string connectionString);
      
 
    }
}