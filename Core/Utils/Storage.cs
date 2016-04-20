// <copyright file="Storage.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using LeagueSharp.SDK.Enumerations;

    /// <summary>
    ///     The storage, main purpose is to save share-able settings between assemblies.
    /// </summary>
    [DataContract]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class Storage : Attribute
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
        [DataMember]
        private readonly Hashtable contents = new Hashtable();

        /// <summary>
        ///     The storage name.
        /// </summary>
        [DataMember]
        private string storageName;

        #endregion

        #region Constructors and Destructors

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

            AppDomain.CurrentDomain.DomainUnload += (sender, args) =>
                {
                    foreach (var storage in StorageList)
                    {
                        storage.Save();
                    }
                };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Storage" /> class.
        /// </summary>
        /// <param name="storageName">
        ///     The storage name
        /// </param>
        /// <param name="types">
        ///     The types.
        /// </param>
        public Storage(string storageName = "Generic", List<Type> types = null)
        {
            if (Path.GetInvalidFileNameChars().Any(storageName.Contains))
            {
                throw new InvalidDataException("Storage name can't have invalid file name characters.");
            }

            this.StorageName = storageName;
            this.StorageTypes = types ?? new List<Type>();

            StorageList.Add(this);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Storage" /> class.
        /// </summary>
        /// <param name="storageName">
        ///     The storage name.
        /// </param>
        /// <param name="isAttribute">
        ///     Indicates whether the storage is placed as an attribute.
        /// </param>
        public Storage(string storageName, bool isAttribute)
        {
            if (Path.GetInvalidFileNameChars().Any(storageName.Contains))
            {
                throw new InvalidDataException("Storage name can't have invalid file name characters.");
            }

            this.StorageName = storageName;
            if (!isAttribute)
            {
                this.StorageTypes = new List<Type>();
                StorageList.Add(this);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Storage" /> class.
        /// </summary>
        /// <param name="storageName">
        ///     The storage name.
        /// </param>
        /// <param name="type">
        ///     The parent type which is currently holding the field or property.
        /// </param>
        public Storage(string storageName, Type type)
        {
            if (Path.GetInvalidFileNameChars().Any(storageName.Contains))
            {
                throw new InvalidDataException("Storage name can't have invalid file name characters.");
            }

            this.StorageName = storageName;
            foreach (var storage in StorageList.Where(storage => storage.StorageName == storageName))
            {
                storage.StorageTypes.Add(type);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the contents.
        /// </summary>
        public IDictionary<string, object> Contents
        {
            get
            {
                return this.contents.Cast<DictionaryEntry>().ToDictionary(e => (string)e.Key, e => e.Value);
            }
        }

        /// <summary>
        ///     Gets or sets the storage name.
        /// </summary>
        public string StorageName
        {
            get
            {
                return this.storageName;
            }

            set
            {
                if (!Path.GetInvalidFileNameChars().Any(value.Contains))
                {
                    this.storageName = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the storage types.
        /// </summary>
        public List<Type> StorageTypes { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the storage path.
        /// </summary>
        private static string StoragePath { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Checks if a certain storage exists.
        /// </summary>
        /// <param name="storageName">
        ///     The storage name.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool Exists(string storageName = "Generic")
        {
            if (Path.GetInvalidFileNameChars().Any(storageName.Contains))
            {
                throw new InvalidDataException("Storage name can't have invalid file name characters.");
            }

            return File.Exists(Path.Combine(StoragePath, storageName + ".storage"));
        }

        /// <summary>
        ///     Loads a saved storage.
        /// </summary>
        /// <param name="storageName">
        ///     Storage name
        /// </param>
        /// <param name="types">
        ///     The types.
        /// </param>
        /// <returns>
        ///     The storage instance.
        /// </returns>
        public static Storage Load(string storageName = "Generic", List<Type> types = null)
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
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    var storageInstance = BinarySerializer.Deserialize<Storage>(bytes);
                    storageInstance.StorageTypes = types ?? new List<Type>();
                    StorageList.Add(storageInstance);

                    return storageInstance;
                }
            }

            return null;
        }

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
            foreach (var type in this.StorageTypes)
            {
                foreach (var field in
                    type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                        .Where(f => f.IsDefined(typeof(Storage))))
                {
                    try
                    {
                        if (this.StorageName == ((Storage)field.GetCustomAttribute(typeof(Storage), false)).StorageName)
                        {
                            if (!this.Add(field.Name, field.GetValue(null)))
                            {
                                this.Update(field.Name, field.GetValue(null));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.Write()(LogLevel.Error, "Unable to save field {0}\n{1}", field.Name, e);
                    }
                }

                foreach (var property in
                    type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                        .Where(p => p.IsDefined(typeof(Storage))))
                {
                    try
                    {
                        if (this.StorageName
                            == ((Storage)property.GetCustomAttribute(typeof(Storage), false)).StorageName)
                        {
                            if (!this.Add(property.Name, property.GetValue(null)))
                            {
                                this.Update(property.Name, property.GetValue(null));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.Write()(LogLevel.Error, "Unable to save property {0}\n{1}", property.Name, e);
                    }
                }
            }

            var path = Path.Combine(StoragePath, this.StorageName + ".storage");
            using (var stream = File.OpenWrite(path))
            {
                var bytes = BinarySerializer.Serialize(this);
                stream.Write(bytes, 0, bytes.Length);
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

        #endregion
    }
}