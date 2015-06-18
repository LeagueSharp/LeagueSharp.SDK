// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Storage.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The storage, main purpose is to save share-able settings between assemblies.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Utils
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The storage, main purpose is to save share-able settings between assemblies.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class Storage : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Storage" /> class.
        /// </summary>
        /// <param name="storageName">
        ///     The storage name
        /// </param>
        public Storage(string storageName = "Generic")
        {
            this.StorageName = storageName;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Storage" /> class.
        /// </summary>
        /// <param name="storageName">
        ///     The storage name
        /// </param>
        /// <param name="types">
        ///     The types
        /// </param>
        public Storage(string storageName = "Generic", List<Type> types = null)
        {
            this.StorageName = storageName;
            this.TypeList = types ?? new List<Type>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the storage name.
        /// </summary>
        public string StorageName { get; set; }

        /// <summary>
        ///     Gets or sets the type list.
        /// </summary>
        public List<Type> TypeList { get; set; }

        #endregion
    }
}