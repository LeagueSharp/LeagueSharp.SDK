// <copyright file="Cache.cs" company="LeagueSharp">
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Caching;

    /// <summary>
    ///     Represents the type that implements an in-memory cache.
    /// </summary>
    public class Cache : ObjectCache
    {
        #region Constants

        /// <summary>
        ///     The default cache region name.
        /// </summary>
        public const string DefaultCacheRegionName = "Default";

        #endregion

        #region Static Fields

        /// <summary>
        ///     The instance
        /// </summary>
        private static Cache instance;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static Cache Instance => instance ?? (instance = new Cache());

        /// <summary>Gets a description of the features that a cache implementation provides.</summary>
        /// <returns>A bitwise combination of flags that indicate the default capabilities of a cache implementation.</returns>
        public override DefaultCacheCapabilities DefaultCacheCapabilities
            =>
                DefaultCacheCapabilities.AbsoluteExpirations | DefaultCacheCapabilities.CacheEntryRemovedCallback
                | DefaultCacheCapabilities.CacheEntryUpdateCallback | DefaultCacheCapabilities.CacheRegions
                | DefaultCacheCapabilities.InMemoryProvider;

        /// <summary>Gets the name of a specific <see cref="T:System.Runtime.Caching.ObjectCache" /> instance. </summary>
        /// <returns>The name of a specific cache instance.</returns>
        public override string Name => "SDK-Cache";

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the internal cache.
        /// </summary>
        /// <value>
        ///     The internal cache.
        /// </value>
        private Dictionary<string, Dictionary<string, CacheEntryItem>> InternalCache { get; } =
            new Dictionary<string, Dictionary<string, CacheEntryItem>>();

        #endregion

        #region Public Indexers

        /// <summary>Gets or sets the default indexer for the <see cref="T:System.Runtime.Caching.ObjectCache" /> class.</summary>
        /// <returns>A key that serves as an indexer into the cache instance.</returns>
        /// <param name="key">A unique identifier for a cache entry in the cache. </param>
        public override object this[string key]
        {
            get
            {
                return this.Get(key);
            }
            set
            {
                this.Set(key, value, new CacheItemPolicy());
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Tries to insert a cache entry into the cache as a <see cref="T:System.Runtime.Caching.CacheItem" /> instance,
        ///     and adds details about how the entry should be evicted.
        /// </summary>
        /// <returns>
        ///     true if insertion succeeded, or false if there is an already an entry in the cache that has the same key as
        ///     <paramref name="item" />.
        /// </returns>
        /// <param name="item">The object to add.</param>
        /// <param name="policy">
        ///     An object that contains eviction details for the cache entry. This object provides more options
        ///     for eviction than a simple absolute expiration.
        /// </param>
        public override bool Add(CacheItem item, CacheItemPolicy policy)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            var cacheRegion = this.AddOrGetExistingCacheRegion(item);

            if (cacheRegion.ContainsKey(item.Key))
            {
                return false;
            }

            var entryItem = new CacheEntryItem(item, policy);
            cacheRegion.Add(item.Key, entryItem);

            this.ValidatePolicy(entryItem);

            return true;
        }

        /// <summary>Inserts a cache entry into the cache without overwriting any existing cache entry. </summary>
        /// <returns>
        ///     true if insertion succeeded, or false if there is an already an entry in the cache that has the same key as
        ///     <paramref name="key" />.
        /// </returns>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert. </param>
        /// <param name="absoluteExpiration">
        ///     The fixed date and time at which the cache entry will expire. This parameter is
        ///     required when the <see cref="ObjectCache.Add(string,object,System.DateTimeOffset,string)" /> method is called.
        /// </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache entry can be added, if regions are
        ///     implemented. Because regions are not implemented in .NET Framework 4, the default value is null.
        /// </param>
        public override bool Add(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            return this.Add(
                new CacheItem(key, value),
                new CacheItemPolicy() { AbsoluteExpiration = absoluteExpiration });
        }

        /// <summary>Inserts a cache entry into the cache, specifying information about how the entry will be evicted.</summary>
        /// <returns>
        ///     true if the insertion try succeeds, or false if there is an already an entry in the cache with the same key as
        ///     <paramref name="key" />.
        /// </returns>
        /// <param name="key">A unique identifier for the cache entry. </param>
        /// <param name="value">The object to insert. </param>
        /// <param name="policy">
        ///     An object that contains eviction details for the cache entry. This object provides more options
        ///     for eviction than a simple absolute expiration.
        /// </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache entry can be added, if regions are
        ///     implemented. The default value for the optional parameter is null.
        /// </param>
        public override bool Add(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            return this.Add(new CacheItem(key, value, regionName), policy);
        }

        /// <summary>
        ///     Inserts a cache entry into the cache, by using a key, an object for the
        ///     cache entry, an absolute expiration value, and an optional region to add the cache into.
        /// </summary>
        /// <returns>If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.</returns>
        /// <param name="key">A unique identifier for the cache entry. </param>
        /// <param name="value">The object to insert. </param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire. </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache entry can be added, if regions are
        ///     implemented. The default value for the optional parameter is null.
        /// </param>
        public override object AddOrGetExisting(
            string key,
            object value,
            DateTimeOffset absoluteExpiration,
            string regionName = null)
        {
            return
                this.AddOrGetExisting(
                    new CacheItem(key, value, regionName),
                    new CacheItemPolicy() { AbsoluteExpiration = absoluteExpiration }).Value;
        }

        /// <summary>
        ///     Inserts the specified <see cref="T:System.Runtime.Caching.CacheItem" />
        ///     object into the cache, specifying information about how the entry will be evicted.
        /// </summary>
        /// <returns>If a cache entry with the same key exists, the specified cache entry; otherwise, null.</returns>
        /// <param name="value">The object to insert. </param>
        /// <param name="policy">
        ///     An object that contains eviction details for the cache entry. This object provides more options
        ///     for eviction than a simple absolute expiration.
        /// </param>
        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            var cacheRegion = this.AddOrGetExistingCacheRegion(value);

            if (cacheRegion.ContainsKey(value.Key))
            {
                return new CacheItem(value.Key, this.Get(value.Key, value.RegionName), value.RegionName);
            }

            this.Add(value, policy);
            return value;
        }

        /// <summary>
        ///     Inserts a cache entry into the cache, specifying a key and a value for the
        ///     cache entry, and information about how the entry will be evicted.
        /// </summary>
        /// <returns>If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.</returns>
        /// <param name="key">A unique identifier for the cache entry. </param>
        /// <param name="value">The object to insert.</param>
        /// <param name="policy">
        ///     An object that contains eviction details for the cache entry. This object provides more options
        ///     for eviction than a simple absolute expiration.
        /// </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache entry can be added, if regions are
        ///     implemented. The default value for the optional parameter is null.
        /// </param>
        public override object AddOrGetExisting(
            string key,
            object value,
            CacheItemPolicy policy,
            string regionName = null)
        {
            return this.AddOrGetExisting(new CacheItem(key, value, regionName), policy);
        }

        /// <summary>Checks whether the cache entry already exists in the cache.</summary>
        /// <returns>true if the cache contains a cache entry with the same key value as <paramref name="key" />; otherwise, false. </returns>
        /// <param name="key">A unique identifier for the cache entry. </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache where the cache can be found, if regions are
        ///     implemented. The default value for the optional parameter is null.
        /// </param>
        public override bool Contains(string key, string regionName = null)
        {
            return this.InternalCache[regionName ?? DefaultCacheRegionName].ContainsKey(key);
        }

        /// <summary>
        ///     Creates a <see cref="T:System.Runtime.Caching.CacheEntryChangeMonitor" />
        ///     object that can trigger events in response to changes to specified cache entries.
        /// </summary>
        /// <returns>A change monitor that monitors cache entries in the cache. </returns>
        /// <param name="keys">The unique identifiers for cache entries to monitor. </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache where the cache keys in the <paramref name="keys" />
        ///     parameter exist, if regions are implemented. The default value for the optional parameter is null.
        /// </param>
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(
            IEnumerable<string> keys,
            string regionName = null)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            var keyList = keys as IList<string> ?? keys.ToList();

            if (keyList.Any(key => key == null))
            {
                throw new ArgumentException("Collection contains a null element.");
            }

            return new CacheEntryChangeMonitorImpl(
                Guid.NewGuid().ToString(),
                new ReadOnlyCollection<string>(keyList),
                DateTime.Now,
                regionName ?? DefaultCacheRegionName);
        }

        /// <summary>Gets the specified cache entry from the cache as an object.</summary>
        /// <returns>The cache entry that is identified by <paramref name="key" />. </returns>
        /// <param name="key">A unique identifier for the cache entry to get. </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache entry was added, if regions are
        ///     implemented. The default value for the optional parameter is null.
        /// </param>
        public override object Get(string key, string regionName = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return this.GetCacheItem(key, regionName)?.Value;
        }

        /// <summary>
        ///     Gets the specified cache entry from the cache as a
        ///     <see cref="T:System.Runtime.Caching.CacheItem" /> instance.
        /// </summary>
        /// <returns>The cache entry that is identified by <paramref name="key" />.</returns>
        /// <param name="key">A unique identifier for the cache entry to get. </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache was added, if regions are
        ///     implemented. Because regions are not implemented in .NET Framework 4, the default is null.
        /// </param>
        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var item = this.AddOrGetExistingCacheRegion(regionName)[key];
            this.ValidatePolicy(item);

            return item.ToCacheItem();
        }

        /// <summary>Gets the total number of cache entries in the cache. </summary>
        /// <returns>
        ///     The number of cache entries in the cache. If <paramref name="regionName" /> is not null, the count indicates
        ///     the number of entries that are in the specified cache region.
        /// </returns>
        /// <param name="regionName">
        ///     Optional. A named region in the cache for which the cache entry count should be computed, if
        ///     regions are implemented. The default value for the optional parameter is null.
        /// </param>
        public override long GetCount(string regionName = null)
        {
            return this.AddOrGetExistingCacheRegion(regionName).Count;
        }

        /// <summary>Gets a set of cache entries that correspond to the specified keys.</summary>
        /// <returns>A dictionary of key/value pairs that represent cache entries. </returns>
        /// <param name="keys">A collection of unique identifiers for the cache entries to get. </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache entry or entries were added, if
        ///     regions are implemented. The default value for the optional parameter is null.
        /// </param>
        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            var cacheRegion = this.AddOrGetExistingCacheRegion(regionName);
            var values = keys.Where(x => cacheRegion.ContainsKey(x)).ToDictionary(x => x, x => cacheRegion[x]);

            foreach (var item in values.Values)
            {
                this.ValidatePolicy(item);
            }

            return values.ToDictionary(x => x.Key, x => x.Value.Value);
        }

        /// <summary>Gets a set of cache entries that correspond to the specified keys.</summary>
        /// <returns>A dictionary of key/value pairs that represent cache entries. </returns>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache entry or entries were added, if
        ///     regions are implemented. Because regions are not implemented in .NET Framework 4, the default is null.
        /// </param>
        /// <param name="keys">A collection of unique identifiers for the cache entries to get. </param>
        public override IDictionary<string, object> GetValues(string regionName, params string[] keys)
        {
            return this.GetValues(keys, regionName);
        }

        /// <summary>Removes the cache entry from the cache.</summary>
        /// <returns>
        ///     An object that represents the value of the removed cache entry that was specified by the key, or null if the
        ///     specified entry was not found.
        /// </returns>
        /// <param name="key">A unique identifier for the cache entry. </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache entry was added, if regions are
        ///     implemented. The default value for the optional parameter is null.
        /// </param>
        public override object Remove(string key, string regionName = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            CacheEntryItem item;
            var cacheRegion = this.AddOrGetExistingCacheRegion(regionName);

            if (!cacheRegion.TryGetValue(key, out item))
            {
                return null;
            }

            item.EntryRemovedCallback(
                new CacheEntryRemovedArguments(this, CacheEntryRemovedReason.Removed, item.ToCacheItem()));
            item.EntryUpdateCallback(
                new CacheEntryUpdateArguments(this, CacheEntryRemovedReason.Removed, item.Key, item.RegionName));

            cacheRegion.Remove(key);

            return item.Value;
        }

        /// <summary>
        ///     Inserts a cache entry into the cache, specifying time-based expiration
        ///     details.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry. </param>
        /// <param name="value">The object to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache entry can be added, if regions are
        ///     implemented. The default value for the optional parameter is null.
        /// </param>
        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            this.Set(
                new CacheItem(key, value, regionName),
                new CacheItemPolicy() { AbsoluteExpiration = absoluteExpiration });
        }

        /// <summary>
        ///     Inserts the cache entry into the cache as a
        ///     <see cref="T:System.Runtime.Caching.CacheItem" /> instance, specifying information about how the entry will be
        ///     evicted.
        /// </summary>
        /// <param name="item">The cache item to add.</param>
        /// <param name="policy">
        ///     An object that contains eviction details for the cache entry. This object provides more options
        ///     for eviction than a simple absolute expiration.
        /// </param>
        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            CacheEntryItem entry;
            var cacheRegion = this.AddOrGetExistingCacheRegion(item.RegionName);

            if (!cacheRegion.TryGetValue(item.Key, out entry))
            {
                this.Add(item, policy);
            }
            else
            {
                cacheRegion[item.Key] = new CacheEntryItem(item, policy);
                this.ValidatePolicy(cacheRegion[item.Key]);
            }
        }

        /// <summary>Inserts a cache entry into the cache. </summary>
        /// <param name="key">A unique identifier for the cache entry. </param>
        /// <param name="value">The object to insert.</param>
        /// <param name="policy">
        ///     An object that contains eviction details for the cache entry. This object provides more options
        ///     for eviction than a simple absolute expiration.
        /// </param>
        /// <param name="regionName">
        ///     Optional. A named region in the cache to which the cache entry can be added, if regions are
        ///     implemented. The default value for the optional parameter is null.
        /// </param>
        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            this.Set(new CacheItem(key, value, regionName), policy);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an enumerator that can be used to iterate through a collection of
        ///     cache entries.
        /// </summary>
        /// <returns>The enumerator object that provides access to the cache entries in the cache.</returns>
        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return
                this.InternalCache[DefaultCacheRegionName].ToDictionary(x => x.Key, x => x.Value.Value).GetEnumerator();
        }

        private Dictionary<string, CacheEntryItem> AddOrGetExistingCacheRegion(CacheItem item)
        {
            return this.AddOrGetExistingCacheRegion(item.RegionName);
        }

        private Dictionary<string, CacheEntryItem> AddOrGetExistingCacheRegion(string regionName)
        {
            Dictionary<string, CacheEntryItem> cacheRegion;

            if (this.InternalCache.TryGetValue(regionName ?? DefaultCacheRegionName, out cacheRegion))
            {
                return cacheRegion;
            }

            cacheRegion = new Dictionary<string, CacheEntryItem>();
            this.InternalCache[regionName ?? DefaultCacheRegionName] = cacheRegion;

            return cacheRegion;
        }

        private void ValidatePolicy(CacheEntryItem item)
        {
            if (DateTime.Now - item.LastAccessed < item.SlidingExpiration
                && item.Expiration.Ticks - DateTime.Now.Ticks > 0)
            {
                item.LastAccessed = DateTime.Now;
                return;
            }

            item.EntryRemovedCallback(
                new CacheEntryRemovedArguments(this, CacheEntryRemovedReason.Expired, item.ToCacheItem()));
            this.InternalCache[item.RegionName].Remove(item.Key);
        }

        #endregion
    }

    /// <summary>
    ///     An entry in the cache.
    /// </summary>
    internal class CacheEntryItem
    {
        #region Constructors and Destructors

        public CacheEntryItem(CacheItem item, CacheItemPolicy policy)
        {
            this.Key = item.Key;
            this.Value = item.Value;
            this.RegionName = item.RegionName;

            this.Expiration = policy.AbsoluteExpiration;
            this.ChangeMonitors = policy.ChangeMonitors;
            this.EntryRemovedCallback = policy.RemovedCallback;
            this.EntryUpdateCallback = policy.UpdateCallback;
            this.SlidingExpiration = policy.SlidingExpiration;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the change monitors.
        /// </summary>
        /// <value>
        ///     The change monitors.
        /// </value>
        public Collection<ChangeMonitor> ChangeMonitors { get; set; }

        /// <summary>
        ///     Gets or sets the entry removed callback.
        /// </summary>
        /// <value>
        ///     The entry removed callback.
        /// </value>
        public CacheEntryRemovedCallback EntryRemovedCallback { get; set; }

        /// <summary>
        ///     Gets or sets the entry update callback.
        /// </summary>
        /// <value>
        ///     The entry update callback.
        /// </value>
        public CacheEntryUpdateCallback EntryUpdateCallback { get; set; }

        /// <summary>
        ///     Gets or sets the expiration.
        /// </summary>
        /// <value>
        ///     The expiration.
        /// </value>
        public DateTimeOffset Expiration { get; set; }

        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        public string Key { get; }

        /// <summary>
        ///     Gets or sets the last accessed.
        /// </summary>
        /// <value>
        ///     The last accessed.
        /// </value>
        public DateTime LastAccessed { get; set; }

        /// <summary>
        ///     Gets or sets the name of the region.
        /// </summary>
        /// <value>
        ///     The name of the region.
        /// </value>
        public string RegionName { get; set; }

        /// <summary>
        ///     Gets or sets the sliding expiration.
        /// </summary>
        /// <value>
        ///     The sliding expiration.
        /// </value>
        public TimeSpan SlidingExpiration { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public object Value { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Converts this instance into a <see cref="CacheItem" />
        /// </summary>
        /// <returns></returns>
        public CacheItem ToCacheItem()
        {
            return new CacheItem(this.Key, this.Value, this.RegionName);
        }

        #endregion
    }

    /// <summary>
    ///     The <see cref="CacheEntryChangeMonitor" /> implemenation for the <see cref="Cache" /> class.
    /// </summary>
    /// <seealso cref="System.Runtime.Caching.CacheEntryChangeMonitor" />
    public class CacheEntryChangeMonitorImpl : CacheEntryChangeMonitor
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CacheEntryChangeMonitorImpl" /> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="cacheKeys">The cache keys.</param>
        /// <param name="lastModified">The last modified.</param>
        /// <param name="regionName">Name of the region.</param>
        internal CacheEntryChangeMonitorImpl(
            string uniqueId,
            ReadOnlyCollection<string> cacheKeys,
            DateTimeOffset lastModified,
            string regionName)
        {
            this.UniqueId = uniqueId;
            this.CacheKeys = cacheKeys;
            this.LastModified = lastModified;
            this.RegionName = regionName;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a collection of cache keys that are monitored for changes. </summary>
        /// <returns>A collection of cache keys.</returns>
        public override ReadOnlyCollection<string> CacheKeys { get; }

        /// <summary>Gets a value that indicates the latest time (in UTC time) that the monitored cache entry was changed.</summary>
        /// <returns>The elapsed time.</returns>
        public override DateTimeOffset LastModified { get; }

        /// <summary>Gets the name of a region of the cache.</summary>
        /// <returns>The name of a region in the cache. </returns>
        public override string RegionName { get; }

        /// <summary>Gets a value that represents the <see cref="T:System.Runtime.Caching.ChangeMonitor" /> class instance.</summary>
        /// <returns>The identifier for a change-monitor instance.</returns>
        public override string UniqueId { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Releases all managed and unmanaged resources and any references to the
        ///     <see cref="T:System.Runtime.Caching.ChangeMonitor" /> instance. This overload must be implemented by derived
        ///     change-monitor classes.
        /// </summary>
        /// <param name="disposing">
        ///     true to release managed and unmanaged resources and any references to a
        ///     <see cref="T:System.Runtime.Caching.ChangeMonitor" /> instance; false to release only unmanaged resources. When
        ///     false is passed, the <see cref="M:System.Runtime.Caching.ChangeMonitor.Dispose(System.Boolean)" /> method is called
        ///     by a finalizer thread and any external managed references are likely no longer valid because they have already been
        ///     garbage collected.
        /// </param>
        protected override void Dispose(bool disposing)
        {
        }

        #endregion
    }
}