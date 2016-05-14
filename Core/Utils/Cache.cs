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

namespace LeagueSharp.SDKEx.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;

    using LeagueSharp.SDKEx.Enumerations;

    /// <summary>
    ///     Provides an implementation of ObjectCache, for any object. Check <see cref="DefaultCacheCapabilities" /> for
    ///     implemented abilities.
    /// </summary>
    public class Cache : ObjectCache
    {
        #region Static Fields

        /// <summary>
        ///     The instance.
        /// </summary>
        private static Cache instance;

        #endregion

        #region Fields

        /// <summary>
        ///     Main Cache.
        /// </summary>
        internal readonly Dictionary<string, Dictionary<string, object>> InternalCache;

        /// <summary>
        ///     Holds callbacks that are called before cached item is removed.
        /// </summary>
        private readonly SortedDictionary<string, CacheEntryUpdateCallback> cacheEntryUpdateCallbacks;

        /// <summary>
        ///     Holds callbacks that are called after cached item is removed.
        /// </summary>
        private readonly SortedDictionary<string, CacheEntryRemovedCallback> cacheRemovedCallbacks;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="Cache" /> class from being created.
        /// </summary>
        private Cache()
        {
            this.InternalCache = new Dictionary<string, Dictionary<string, object>>();
            this.CreateRegion("Default");

            this.cacheEntryUpdateCallbacks = new SortedDictionary<string, CacheEntryUpdateCallback>();
            this.cacheRemovedCallbacks = new SortedDictionary<string, CacheEntryRemovedCallback>();
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Delegate when a value is added.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Passed Arguments Container</param>
        public delegate void OnEntryAddedDelegate(object sender, EntryAddedArgs e);

        /// <summary>
        ///     Delegate for <see cref="OnValueChanged" />.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Arguments</param>
        public delegate void OnValueChangedDelegate(object sender, ValueChangedArgs e);

        #endregion

        #region Public Events

        /// <summary>
        ///     Gets called when a new value is added to the InternalCache.
        /// </summary>
        public static event OnEntryAddedDelegate OnEntryAdded;

        /// <summary>
        ///     Called when an entry in the InternalCache is modified.
        /// </summary>
        public static event OnValueChangedDelegate OnValueChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance of Cache
        /// </summary>
        public static Cache Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = new Cache();
                return instance;
            }
        }

        /// <summary>
        ///     The capabilities of this implementation of ObjectCache.
        /// </summary>
        public override DefaultCacheCapabilities DefaultCacheCapabilities
            =>
                DefaultCacheCapabilities.AbsoluteExpirations | DefaultCacheCapabilities.CacheRegions
                | DefaultCacheCapabilities.CacheEntryRemovedCallback | DefaultCacheCapabilities.CacheEntryUpdateCallback
            ;

        /// <summary>
        ///     Returns the name of the Cache.
        /// </summary>
        public override string Name => "SDK Cache";

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets/Sets the value of a key in the Cache, using the default region name.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>Value matching the key</returns>
        public override object this[string key]
        {
            get
            {
                return this.InternalCache["Default"][key];
            }

            set
            {
                this.InternalCache["Default"][key] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds or Gets an existing key/value, executing the function if the key does not exist.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="function">Function that will get the value</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>The Object of the given function</returns>
        public object AddOrGetExisting(string key, Func<object> function, string regionName = null)
        {
            regionName = regionName ?? "Default";

            object cachedObject;
            var contains = this.InternalCache[regionName].TryGetValue(key, out cachedObject);

            if (contains)
            {
                return cachedObject;
            }

            var result = function();
            this.InternalCache[regionName].Add(key, result);

            OnEntryAdded?.Invoke(this, new EntryAddedArgs { Key = key, Value = result, RegionName = regionName });

            return result;
        }

        /// <summary>
        ///     Adds a key and a value, in the InternalCache region. However, if the item exists, it will return the cached item.
        ///     This
        ///     KeyValuePair does not expire.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="value">The Value</param>
        /// <param name="regionName">The name of the region in the InternalCache.</param>
        /// <returns>The cached value.</returns>
        public object AddOrGetExisting(string key, object value, string regionName = "Default")
        {
            return this.AddOrGetExisting(key, value, InfiniteAbsoluteExpiration, regionName);
        }

        /// <summary>
        ///     Adds a key, a value, and an expiration to the InternalCache, returns the value if the key exists.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="value">The Value</param>
        /// <param name="absoluteExpiration">Time the KeyPair expires</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>The object stored in the InternalCache</returns>
        public override object AddOrGetExisting(
            string key,
            object value,
            DateTimeOffset absoluteExpiration,
            string regionName = null)
        {
            regionName = regionName ?? "Default";

            object internalValue;
            var contains = this.InternalCache[regionName].TryGetValue(key, out internalValue);

            if (contains)
            {
                return internalValue;
            }

            OnEntryAdded?.Invoke(this, new EntryAddedArgs { Key = key, Value = value, RegionName = regionName });

            this.InternalCache[regionName].Add(key, value);

            DelayAction.Add(
                (int)(absoluteExpiration - DateTime.Now).TotalMilliseconds,
                () =>
                    {
                        if (!this.InternalCache[regionName].ContainsKey(key))
                        {
                            return;
                        }

                        var cacheValue = this.InternalCache[regionName][key];

                        this.CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                        this.InternalCache[regionName].Remove(key);
                        this.CallEntryRemoved(key, cacheValue, CacheEntryRemovedReason.Expired, regionName);
                    });

            return value;
        }

        /// <summary>
        ///     Adds a CacheItem following the policy provided to the InternalCache, but returns the CacheItem if it exists.
        /// </summary>
        /// <param name="value">The Value</param>
        /// <param name="policy">The Policy</param>
        /// <returns>The CacheItem in the InternalCache</returns>
        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            var regionName = value.RegionName ?? "Default";

            object internalValue;
            var contains = this.InternalCache[regionName].TryGetValue(value.Key, out internalValue);

            if (contains)
            {
                return new CacheItem(value.Key, internalValue, regionName);
            }

            OnEntryAdded?.Invoke(
                this,
                new EntryAddedArgs { Key = value.Key, Value = value.Value, RegionName = regionName });

            this.InternalCache[regionName].Add(value.Key, value.Value);

            this.cacheEntryUpdateCallbacks[value.Key + value.RegionName] = policy.UpdateCallback;
            this.cacheRemovedCallbacks[value.Key + value.RegionName] = policy.RemovedCallback;

            DelayAction.Add(
                (int)(policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds,
                () =>
                    {
                        if (!this.InternalCache[regionName].ContainsKey(value.Key))
                        {
                            return;
                        }

                        var cachedValue = this.InternalCache[regionName][value.Key];

                        this.CallEntryUpdates(value.Key, CacheEntryRemovedReason.Expired, value.RegionName);
                        this.InternalCache[regionName].Remove(value.Key);
                        this.CallEntryRemoved(value.Key, cachedValue, CacheEntryRemovedReason.Expired, value.RegionName);
                    });

            return new CacheItem(value.Key, value.Value, regionName);
        }

        /// <summary>
        ///     Adds a key and a value to the InternalCache, following the policy, returns the cached value if it exists.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="value">The Value</param>
        /// <param name="policy">The Policy</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>Cached value</returns>
        public override object AddOrGetExisting(
            string key,
            object value,
            CacheItemPolicy policy,
            string regionName = null)
        {
            regionName = regionName ?? "Default";

            object internalValue;
            var contains = this.InternalCache[regionName].TryGetValue(key, out internalValue);

            if (contains)
            {
                return internalValue;
            }

            OnEntryAdded?.Invoke(this, new EntryAddedArgs { Key = key, Value = value, RegionName = regionName });

            this.InternalCache[regionName].Add(key, value);

            this.cacheEntryUpdateCallbacks[key + regionName] = policy.UpdateCallback;
            this.cacheRemovedCallbacks[key + regionName] = policy.RemovedCallback;

            DelayAction.Add(
                (int)(policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds,
                () =>
                    {
                        if (!this.InternalCache[regionName].ContainsKey(key))
                        {
                            return;
                        }

                        var cachedValue = this.InternalCache[regionName][key];

                        this.CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                        this.InternalCache[regionName].Remove(key);
                        this.CallEntryRemoved(key, cachedValue, CacheEntryRemovedReason.Expired, regionName);
                    });

            return value;
        }

        /// <summary>
        ///     Checks if the InternalCache contains the key.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>Whether the key is in the InternalCache</returns>
        public override bool Contains(string key, string regionName = null)
        {
            regionName = regionName ?? "Default";
            return this.InternalCache[regionName].ContainsKey(key);
        }

        /// <summary>
        ///     Not supported. Please use the events.
        /// </summary>
        /// <param name="keys">The Keys</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns><see cref="CacheEntryChangeMonitor" /> instance</returns>
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(
            IEnumerable<string> keys,
            string regionName = null)
        {
            throw new NotSupportedException("CacheEntryChangeMonitors are not implemented.");
        }

        /// <summary>
        ///     Creates a new region to use. The default region is automatically created.
        /// </summary>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        public void CreateRegion(string regionName)
        {
            this.InternalCache.Add(regionName, new Dictionary<string, object>());
        }

        /// <summary>
        ///     Gets a value in the InternalCache by the key.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>The object the key pairs with</returns>
        public override object Get(string key, string regionName = null)
        {
            regionName = regionName ?? "Default";
            return this.InternalCache[regionName][key];
        }

        /// <summary>
        ///     Gets a value from the InternalCache, and casts it to the type.
        /// </summary>
        /// <typeparam name="T">Type to cast to</typeparam>
        /// <param name="key">The Key</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>The object casted into T</returns>
        public T Get<T>(string key, string regionName = null)
        {
            regionName = regionName ?? "Default";

            var value = this.InternalCache[regionName][key];
            return value is T ? (T)value : default(T);
        }

        /// <summary>
        ///     Gets the CacheItem from the Key.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="regionName">The Region Name</param>
        /// <returns>CacheItem in the InternalCache</returns>
        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            regionName = regionName ?? "Default";
            return new CacheItem(key, this.InternalCache[regionName][key], regionName);
        }

        /// <summary>
        ///     Gets the count of KeyPairs in the Cache.
        /// </summary>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>Count of KeyPairs in the Cache</returns>
        public override long GetCount(string regionName = null)
        {
            regionName = regionName ?? "Default";
            return this.InternalCache[regionName].Count;
        }

        /// <summary>
        ///     Gets the values of all the keys provided.
        /// </summary>
        /// <param name="keys">Keys to get the values of</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>Dictionary of all the keys, with the perspective values.</returns>
        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            regionName = regionName ?? "Default";
            return keys.Where(x => this.InternalCache[regionName].ContainsKey(x))
                .ToDictionary(x => x, x => this.InternalCache[regionName][x]);
        }

        /// <summary>
        ///     Removes KeyPair by the key in the InternalCache.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>The value of the key being removed</returns>
        public override object Remove(string key, string regionName = null)
        {
            regionName = regionName ?? "Default";

            if (!this.InternalCache[regionName].ContainsKey(key))
            {
                return null;
            }

            if (this.cacheEntryUpdateCallbacks.ContainsKey(key + regionName))
            {
                this.cacheEntryUpdateCallbacks[key + regionName].Invoke(
                    new CacheEntryUpdateArguments(this, CacheEntryRemovedReason.Removed, key, regionName));
            }

            var value = this.InternalCache[regionName][key];
            this.InternalCache[regionName].Remove(key);

            if (this.cacheRemovedCallbacks.ContainsKey(key + regionName))
            {
                this.cacheRemovedCallbacks[key + regionName].Invoke(
                    new CacheEntryRemovedArguments(
                        this,
                        CacheEntryRemovedReason.Removed,
                        new CacheItem(key, value, regionName)));
            }

            return value;
        }

        /// <summary>
        ///     Sets the value of a key in the InternalCache, and expires at a set time. The key does not have to be created.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="value">The Value</param>
        /// <param name="absoluteExpiration">The Expiration</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            regionName = regionName ?? "Default";

            if (OnValueChanged != null && this.InternalCache[regionName].ContainsKey(key))
            {
                OnValueChanged(this, new ValueChangedArgs(key, this.InternalCache[regionName][key], value, regionName));
            }

            this.InternalCache[regionName][key] = value;

            DelayAction.Add(
                (int)(absoluteExpiration - DateTime.Now).TotalMilliseconds,
                () =>
                    {
                        if (!this.InternalCache[regionName].ContainsKey(key))
                        {
                            return;
                        }

                        var cachedValue = this.InternalCache[regionName][key];

                        this.CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                        this.InternalCache[regionName].Remove(key);
                        this.CallEntryRemoved(key, cachedValue, CacheEntryRemovedReason.Expired, regionName);
                    });
        }

        /// <summary>
        ///     Sets the value of a key by the CacheItem, following the policy provided.
        /// </summary>
        /// <param name="item">The Item</param>
        /// <param name="policy">The Policy</param>
        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            var regionName = item.RegionName ?? "Default";

            if (OnValueChanged != null && this.InternalCache[regionName].ContainsKey(item.Key))
            {
                OnValueChanged(
                    this,
                    new ValueChangedArgs(item.Key, this.InternalCache[regionName][item.Key], item.Value, regionName));
            }

            this.InternalCache[regionName][item.Key] = item.Value;

            DelayAction.Add(
                (int)(policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds,
                () =>
                    {
                        if (!this.InternalCache[regionName].ContainsKey(item.Key))
                        {
                            return;
                        }

                        var cachedValue = this.InternalCache[regionName][item.Key];

                        this.CallEntryUpdates(item.Key, CacheEntryRemovedReason.Expired, item.RegionName);
                        this.InternalCache[regionName].Remove(item.Key);
                        this.CallEntryRemoved(item.Key, cachedValue, CacheEntryRemovedReason.Expired, item.RegionName);
                    });
        }

        /// <summary>
        ///     Sets the value of a key, following the policy provided.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="value">The Value</param>
        /// <param name="policy">The Policy</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            regionName = regionName ?? "Default";

            if (OnValueChanged != null && this.InternalCache[regionName].ContainsKey(key))
            {
                OnValueChanged(this, new ValueChangedArgs(key, this.InternalCache[regionName][key], value, regionName));
            }

            this.InternalCache[regionName][key] = value;

            DelayAction.Add(
                (int)(policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds,
                () =>
                    {
                        if (!this.InternalCache[regionName].ContainsKey(key))
                        {
                            return;
                        }

                        var cachedValue = this.InternalCache[regionName][key];

                        this.CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                        this.InternalCache[regionName].Remove(key);
                        this.CallEntryRemoved(key, cachedValue, CacheEntryRemovedReason.Expired, regionName);
                    });
        }

        /// <summary>
        ///     Tries to get the value, returns false if the InternalCache does not contain that key.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="value">Value(Null if doesn't exist)</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>If the InternalCache contains the value</returns>
        public bool TryGetValue(string key, out object value, string regionName = null)
            => this.InternalCache[regionName ?? "Default"].TryGetValue(key, out value);

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the enumerator of the default internal InternalCache.
        /// </summary>
        /// <returns>Enumerator of InternalCache</returns>
        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => this.InternalCache["Default"].GetEnumerator();

        /// <summary>
        ///     Calls the <see cref="CacheEntryRemovedCallback" /> for the selected key.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="value">The Value</param>
        /// <param name="reason">Reason why the value was removed</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        private void CallEntryRemoved(
            string key,
            object value,
            CacheEntryRemovedReason reason = CacheEntryRemovedReason.Removed,
            string regionName = null)
        {
            CacheEntryRemovedCallback callback;
            var contains = this.cacheRemovedCallbacks.TryGetValue(key + regionName, out callback);

            if (!contains)
            {
                return;
            }

            try
            {
                callback.Invoke(new CacheEntryRemovedArguments(this, reason, new CacheItem(key, value, regionName)));
            }
            catch (Exception e)
            {
                Logging.Write()(
                    LogLevel.Error,
                    "An exception occured while invoking the CacheEntryRemovedCallback: {0}",
                    e);
            }
        }

        /// <summary>
        ///     Calls the <see cref="CacheEntryUpdateCallback" /> for the selected key.
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="reason">Reason why the value was removed</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        private void CallEntryUpdates(
            string key,
            CacheEntryRemovedReason reason = CacheEntryRemovedReason.Removed,
            string regionName = null)
        {
            CacheEntryUpdateCallback callback;
            var contains = this.cacheEntryUpdateCallbacks.TryGetValue(key + regionName, out callback);

            if (!contains)
            {
                return;
            }

            try
            {
                callback.Invoke(new CacheEntryUpdateArguments(this, reason, key, regionName));
            }
            catch (Exception e)
            {
                Logging.Write()(LogLevel.Error, "An exception occured while invoking the EntryUpdateCallbacks: {0}", e);
            }
        }

        #endregion
    }

    /// <summary>
    ///     Arguments for the <see cref="Cache.OnValueChanged" /> event.
    /// </summary>
    public class ValueChangedArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueChangedArgs" /> class.
        /// </summary>
        /// <param name="key">
        ///     The key
        /// </param>
        /// <param name="oldValue">
        ///     The old value
        /// </param>
        /// <param name="newValue">
        ///     The new value
        /// </param>
        /// <param name="regionName">
        ///     The region name
        /// </param>
        internal ValueChangedArgs(string key, object oldValue, object newValue, string regionName)
        {
            this.Key = key;
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.RegionName = regionName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the new value after it was changed.
        /// </summary>
        public object NewValue { get; set; }

        /// <summary>
        ///     Gets or sets the old value prior to changing.
        /// </summary>
        public object OldValue { get; set; }

        /// <summary>
        ///     Gets or sets the name of the region in the InternalCache.
        /// </summary>
        public string RegionName { get; set; }

        #endregion
    }

    /// <summary>
    ///     Arguments for the <see cref="Cache.OnEntryAddedDelegate" /> event.
    /// </summary>
    public class EntryAddedArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the Key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the name of the region in the InternalCache.
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        ///     Gets or sets the Value.
        /// </summary>
        public object Value { get; set; }

        #endregion
    }
}