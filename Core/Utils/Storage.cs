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
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Permissions;

    /// <summary>
    ///     The storage, main purpose is to save share-able settings between assemblies.
    /// </summary>
    [Serializable]
    public class Storage : ISerializable
    {
        #region Static Fields

        /// <summary>
        ///     The storage list.
        /// </summary>
        private static readonly List<Storage> StorageList = new List<Storage>();

        #endregion

        #region Fields

        /// <summary>
        ///     The storage contents.
        /// </summary>
        private readonly Hashtable contents = new Hashtable();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Storage" /> class.
        /// </summary>
        /// <param name="storageName">
        ///     The storage name
        /// </param>
        public Storage(string storageName = "Generic")
        {
            if (Path.GetInvalidFileNameChars().Any(storageName.Contains))
            {
                throw new InvalidDataException("Storage name can't have invalid file name characters.");
            }

            this.StorageName = storageName;
            StorageList.Add(this);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Storage" /> class.
        /// </summary>
        /// <param name="info">
        ///     The info
        /// </param>
        /// <param name="context">
        ///     The context
        /// </param>
        protected Storage(SerializationInfo info, StreamingContext context)
        {
            this.contents = (Hashtable)info.GetValue("contents", typeof(Hashtable));
        }

        /// <summary>
        ///     Initializes static members of the <see cref="Storage" /> class.
        /// </summary>
        static Storage()
        {
            StoragePath = Path.Combine(Constants.LeagueSharpAppData, "Storage");
            if (!Directory.Exists(StoragePath))
            {
                Directory.CreateDirectory(StoragePath);
            }

            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
                {
                    foreach (var storage in StorageList)
                    {
                        storage.Save();
                    }
                };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the storage name.
        /// </summary>
        public string StorageName { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the storage path.
        /// </summary>
        private static string StoragePath { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Add a content to the storage.
        /// </summary>
        /// <param name="key">
        ///     The key
        /// </param>
        /// <param name="value">
        ///     The value
        /// </param>
        /// <returns>
        ///     Whether the content was added towards the storage.
        /// </returns>
        public bool Add(string key, object value)
        {
            if (!this.contents.ContainsKey(key))
            {
                this.contents.Add(key, value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Get a content from the storage.
        /// </summary>
        /// <param name="key">
        ///     The key
        /// </param>
        /// <typeparam name="T">
        ///     The value type
        /// </typeparam>
        /// <returns>
        ///     The value with the requested value type.
        /// </returns>
        public T Get<T>(string key)
        {
            return this.contents.ContainsKey(key) ? (T)this.contents[key] : default(T);
        }

        /// <summary>
        ///     Remove a content from the storage.
        /// </summary>
        /// <param name="key">
        ///     The key
        /// </param>
        /// <returns>
        ///     Whether the value was removed the storage.
        /// </returns>
        public bool Remove(string key)
        {
            if (this.contents.Contains(key))
            {
                this.contents.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Saves all of the current storage contents.
        /// </summary>
        public void Save()
        {
            var path = Path.Combine(StoragePath, this.StorageName + ".storage");
            using (var stream = File.OpenWrite(path))
            {
                var binary = new BinaryFormatter();
                binary.Serialize(stream, this);
            }
        }

        /// <summary>
        ///     Updates a content based on the given key.
        /// </summary>
        /// <param name="key">
        ///     The key
        /// </param>
        /// <param name="value">
        ///     The value
        /// </param>
        /// <returns>
        ///     Whether the operation was successful and the value was updated.
        /// </returns>
        public bool Update(string key, object value)
        {
            if (this.contents.ContainsKey(key))
            {
                this.contents[key] = value;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Loads a saved storage.
        /// </summary>
        /// <param name="storageName">
        ///     Storage name
        /// </param>
        /// <returns>
        ///     The storage instance.
        /// </returns>
        public static Storage Load(string storageName = "Generic")
        {
            if (Path.GetInvalidFileNameChars().Any(storageName.Contains))
            {
                throw new InvalidDataException("Storage name can't have invalid file name characters.");
            }

            var path = Path.Combine(StoragePath, storageName + ".storage");
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    var binary = new BinaryFormatter();
                    return (Storage)binary.Deserialize(stream);
                }
            }

            return null;
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.
        /// </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("contents", this.contents, typeof(Hashtable));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.
        /// </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("contents", this.contents, typeof(Hashtable));
        }

        #endregion
    }
}