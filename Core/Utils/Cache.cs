#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using LeagueSharp.CommonEx.Core.Enumerations;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Provides an implementation of ObjectCache, for any object. Check <see cref="DefaultCacheCapabilities" /> for
    ///     implemented abilities.
    /// </summary>
    public class Cache : ObjectCache
    {
        /// <summary>
        ///     Holds callbacks that are called before cached item is removed.
        /// </summary>
        private readonly SortedDictionary<string, CacheEntryUpdateCallback> _cacheEntryUpdateCallbacks;

        /// <summary>
        ///     Holds callbacks that are called after cached item is removed.
        /// </summary>
        private readonly SortedDictionary<string, CacheEntryRemovedCallback> _cacheRemovedCallbacks;

        /// <summary>
        ///     Main Cache.
        /// </summary>
        internal readonly Dictionary<string, Dictionary<string, object>> InternalCache;

        /// <summary>
        ///     Private Constructor.
        /// </summary>
        private Cache()
        {
            InternalCache = new Dictionary<string, Dictionary<string, object>>();
            CreateRegion("Default");

            _cacheEntryUpdateCallbacks = new SortedDictionary<string, CacheEntryUpdateCallback>();
            _cacheRemovedCallbacks = new SortedDictionary<string, CacheEntryRemovedCallback>();
        }

        /// <summary>
        ///     The capabilities of this implementation of ObjectCache.
        /// </summary>
        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get
            {
                return DefaultCacheCapabilities.AbsoluteExpirations | DefaultCacheCapabilities.CacheRegions |
                       DefaultCacheCapabilities.CacheEntryRemovedCallback |
                       DefaultCacheCapabilities.CacheEntryUpdateCallback;
            }
        }

        /// <summary>
        ///     Returns the name of the Cache.
        /// </summary>
        public override string Name
        {
            get { return "CommonEX Cache"; }
        }

        /// <summary>
        ///     Gets/Sets the value of a key in the Cache, using the default region name.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value matching the key</returns>
        public override object this[string key]
        {
            get { return InternalCache["Default"][key]; }
            set { InternalCache["Default"][key] = value; }
        }

        /// <summary>
        ///     Calls the <see cref="CacheEntryUpdateCallback" /> for the selected key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="reason">Reason why the value was removed</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        private void CallEntryUpdates(string key,
            CacheEntryRemovedReason reason = CacheEntryRemovedReason.Removed,
            string regionName = null)
        {
            CacheEntryUpdateCallback callback;
            var contains = _cacheEntryUpdateCallbacks.TryGetValue(key + regionName, out callback);

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

        /// <summary>
        ///     Calls the <see cref="CacheEntryRemovedCallback" /> for the selected key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="reason">Reason why the value was removed</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        private void CallEntryRemoved(string key,
            object value,
            CacheEntryRemovedReason reason = CacheEntryRemovedReason.Removed,
            string regionName = null)
        {
            CacheEntryRemovedCallback callback;
            var contains = _cacheRemovedCallbacks.TryGetValue(key + regionName, out callback);

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
                    LogLevel.Error, "An exception occured while invoking the CacheEntryRemovedCallback: {0}", e);
            }
        }

        /// <summary>
        ///     Creates a new region to use. The default region is automatically created.
        /// </summary>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        public void CreateRegion(string regionName)
        {
            InternalCache.Add(regionName, new Dictionary<string, object>());
        }

        /// <summary>
        ///     Tries to get the value, returns false if the InternalCache does not contain that key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value(Null if doesnt exist)</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>If the InternalCache contains the value</returns>
        public bool TryGetValue(string key, out object value, string regionName = null)
        {
            regionName = regionName ?? "Default";
            return InternalCache[regionName].TryGetValue(key, out value);
        }

        /// <summary>
        ///     Not supported. Please use the events.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns></returns>
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys,
            string regionName = null)
        {
            throw new NotSupportedException("CacheEntryChangeMonitors are not implemented.");
        }

        /// <summary>
        ///     Gets the enumerator of the default internal InternalCache.
        /// </summary>
        /// <returns>Enumerator of InternalCache</returns>
        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return InternalCache["Default"].GetEnumerator();
        }

        /// <summary>
        ///     Checks if the InternalCache contains the key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>Wether the key is in the InternalCache</returns>
        public override bool Contains(string key, string regionName = null)
        {
            regionName = regionName ?? "Default";
            return InternalCache[regionName].ContainsKey(key);
        }

        /// <summary>
        ///     Adds or Gets an existing key/value, executing the function if the key does not exist.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="function">Function that will get the value</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns></returns>
        public object AddOrGetExisting(string key, Func<object> function, string regionName = null)
        {
            regionName = regionName ?? "Default";

            object cachedObject;
            var contains = InternalCache[regionName].TryGetValue(key, out cachedObject);

            if (contains)
            {
                return cachedObject;
            }

            var result = function();
            InternalCache[regionName].Add(key, result);

            if (OnEntryAdded != null)
            {
                OnEntryAdded(this, new EntryAddedArgs { Key = key, Value = result, RegionName = regionName });
            }

            return result;
        }

        /// <summary>
        ///     Adds a key and a value, in the InternalCache region. However, if the item exists, it will return the cached item.
        ///     This
        ///     KeyValuePair does not expire.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="regionName">The anme of the region in the InternalCache.</param>
        /// <returns>The cahced value.</returns>
        public object AddOrGetExisting(string key, object value, string regionName = "Default")
        {
            return AddOrGetExisting(key, value, InfiniteAbsoluteExpiration, regionName);
        }

        /// <summary>
        ///     Adds a key, a value, and an expiration to the InternalCache, returns the value if the key exists.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="absoluteExpiration">Time the keypair expires</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>The object stored in the InternalCache</returns>
        public override object AddOrGetExisting(string key,
            object value,
            DateTimeOffset absoluteExpiration,
            string regionName = null)
        {
            regionName = regionName ?? "Default";

            object internalValue;
            var contains = InternalCache[regionName].TryGetValue(key, out internalValue);

            if (contains)
            {
                return internalValue;
            }

            if (OnEntryAdded != null)
            {
                OnEntryAdded(this, new EntryAddedArgs { Key = key, Value = value, RegionName = regionName });
            }

            InternalCache[regionName].Add(key, value);

            DelayAction.Add(
                (int) (absoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!InternalCache[regionName].ContainsKey(key))
                    {
                        return;
                    }

                    var cacheValue = InternalCache[regionName][key];

                    CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                    InternalCache[regionName].Remove(key);
                    CallEntryRemoved(key, cacheValue, CacheEntryRemovedReason.Expired, regionName);
                });

            return value;
        }

        /// <summary>
        ///     Adds a CacheItem following the policy provided to the InternalCache, but returns the CacheItem if it exists.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="policy">Policy</param>
        /// <returns>The CacheItem in the InternalCache</returns>
        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            var regionName = value.RegionName ?? "Default";

            object internalValue;
            var contains = InternalCache[regionName].TryGetValue(value.Key, out internalValue);

            if (contains)
            {
                return new CacheItem(value.Key, internalValue, regionName);
            }

            if (OnEntryAdded != null)
            {
                OnEntryAdded(this, new EntryAddedArgs { Key = value.Key, Value = value.Value, RegionName = regionName });
            }

            InternalCache[regionName].Add(value.Key, value.Value);

            _cacheEntryUpdateCallbacks[value.Key + value.RegionName] = policy.UpdateCallback;
            _cacheRemovedCallbacks[value.Key + value.RegionName] = policy.RemovedCallback;

            DelayAction.Add(
                (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!InternalCache[regionName].ContainsKey(value.Key))
                    {
                        return;
                    }

                    var cachedValue = InternalCache[regionName][value.Key];

                    CallEntryUpdates(value.Key, CacheEntryRemovedReason.Expired, value.RegionName);
                    InternalCache[regionName].Remove(value.Key);
                    CallEntryRemoved(value.Key, cachedValue, CacheEntryRemovedReason.Expired, value.RegionName);
                });

            return new CacheItem(value.Key, value.Value, regionName);
        }

        /// <summary>
        ///     Adds a key and a value to the InternalCache, following the policy, returns the cached value if it exists.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="policy">Policy</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>Cached value</returns>
        public override object AddOrGetExisting(string key,
            object value,
            CacheItemPolicy policy,
            string regionName = null)
        {
            regionName = regionName ?? "Default";

            object internalValue;
            var contains = InternalCache[regionName].TryGetValue(key, out internalValue);

            if (contains)
            {
                return internalValue;
            }

            if (OnEntryAdded != null)
            {
                OnEntryAdded(this, new EntryAddedArgs { Key = key, Value = value, RegionName = regionName });
            }

            InternalCache[regionName].Add(key, value);

            _cacheEntryUpdateCallbacks[key + regionName] = policy.UpdateCallback;
            _cacheRemovedCallbacks[key + regionName] = policy.RemovedCallback;

            DelayAction.Add(
                (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!InternalCache[regionName].ContainsKey(key))
                    {
                        return;
                    }

                    var cachedValue = InternalCache[regionName][key];

                    CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                    InternalCache[regionName].Remove(key);
                    CallEntryRemoved(key, cachedValue, CacheEntryRemovedReason.Expired, regionName);
                });

            return value;
        }

        /// <summary>
        ///     Gets a value in the InternalCache by the key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>The object the key pairs with</returns>
        public override object Get(string key, string regionName = null)
        {
            regionName = regionName ?? "Default";
            return InternalCache[regionName][key];
        }

        /// <summary>
        ///     Gets a value from the InternalCache, and casts it to the type.
        /// </summary>
        /// <typeparam name="T">Type to cast to</typeparam>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>The object casted into T</returns>
        public T Get<T>(string key, string regionName = null)
        {
            regionName = regionName ?? "Default";

            var value = InternalCache[regionName][key];
            return value is T ? (T) value : default(T);
        }

        /// <summary>
        ///     Gets the CacheItem from the Key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="regionName"></param>
        /// <returns>CacheItem in the InternalCache</returns>
        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            regionName = regionName ?? "Default";
            return new CacheItem(key, InternalCache[regionName][key], regionName);
        }

        /// <summary>
        ///     Sets the value of a key in the InternalCache, and expires at a set time. The key does not have to be created.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="absoluteExpiration">Expiration</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            regionName = regionName ?? "Default";

            if (OnValueChanged != null && InternalCache[regionName].ContainsKey(key))
            {
                OnValueChanged(this, new ValueChangedArgs(key, InternalCache[regionName][key], value, regionName));
            }

            InternalCache[regionName][key] = value;

            DelayAction.Add(
                (int) (absoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!InternalCache[regionName].ContainsKey(key))
                    {
                        return;
                    }

                    var cachedValue = InternalCache[regionName][key];

                    CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                    InternalCache[regionName].Remove(key);
                    CallEntryRemoved(key, cachedValue, CacheEntryRemovedReason.Expired, regionName);
                });
        }

        /// <summary>
        ///     Sets the value of a key by the CacheItem, following the policy provided.
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="policy">Policy</param>
        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            var regionName = item.RegionName ?? "Default";

            if (OnValueChanged != null && InternalCache[regionName].ContainsKey(item.Key))
            {
                OnValueChanged(
                    this, new ValueChangedArgs(item.Key, InternalCache[regionName][item.Key], item.Value, regionName));
            }

            InternalCache[regionName][item.Key] = item.Value;

            DelayAction.Add(
                (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!InternalCache[regionName].ContainsKey(item.Key))
                    {
                        return;
                    }

                    var cachedValue = InternalCache[regionName][item.Key];

                    CallEntryUpdates(item.Key, CacheEntryRemovedReason.Expired, item.RegionName);
                    InternalCache[regionName].Remove(item.Key);
                    CallEntryRemoved(item.Key, cachedValue, CacheEntryRemovedReason.Expired, item.RegionName);
                });
        }

        /// <summary>
        ///     Sets the value of a key, following the policy provided.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="policy">Policy</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            regionName = regionName ?? "Default";

            if (OnValueChanged != null && InternalCache[regionName].ContainsKey(key))
            {
                OnValueChanged(this, new ValueChangedArgs(key, InternalCache[regionName][key], value, regionName));
            }

            InternalCache[regionName][key] = value;

            DelayAction.Add(
                (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!InternalCache[regionName].ContainsKey(key))
                    {
                        return;
                    }

                    var cachedValue = InternalCache[regionName][key];

                    CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                    InternalCache[regionName].Remove(key);
                    CallEntryRemoved(key, cachedValue, CacheEntryRemovedReason.Expired, regionName);
                });
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
            return keys.Where(x => InternalCache[regionName].ContainsKey(x))
                .ToDictionary(x => x, x => InternalCache[regionName][x]);
        }

        /// <summary>
        ///     Removes KeyPair by the key in the InternalCache.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>The value of the key being removed</returns>
        public override object Remove(string key, string regionName = null)
        {
            regionName = regionName ?? "Default";

            if (!InternalCache[regionName].ContainsKey(key))
            {
                return null;
            }

            if (_cacheEntryUpdateCallbacks.ContainsKey(key + regionName))
            {
                _cacheEntryUpdateCallbacks[key + regionName].Invoke(
                    new CacheEntryUpdateArguments(this, CacheEntryRemovedReason.Removed, key, regionName));
            }

            var value = InternalCache[regionName][key];
            InternalCache[regionName].Remove(key);

            if (_cacheRemovedCallbacks.ContainsKey(key + regionName))
            {
                _cacheRemovedCallbacks[key + regionName].Invoke(
                    new CacheEntryRemovedArguments(
                        this, CacheEntryRemovedReason.Removed, new CacheItem(key, value, regionName)));
            }

            return value;
        }

        /// <summary>
        ///     Gets the count of KeyPairs in the Cache.
        /// </summary>
        /// <param name="regionName">The name of the region in the InternalCache</param>
        /// <returns>Count of KeyPairs in the Cache</returns>
        public override long GetCount(string regionName = null)
        {
            regionName = regionName ?? "Default";
            return InternalCache[regionName].Count;
        }

        #region Events

        #region OnEntryAdded

        /// <summary>
        ///     Delegate when a value is added.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Passed Arguments Container</param>
        public delegate void OnEntryAddedDelegate(object sender, EntryAddedArgs e);

        /// <summary>
        ///     Gets called when a new value is added to the InternalCache.
        /// </summary>
        public static event OnEntryAddedDelegate OnEntryAdded;

        #endregion OnEntryAdded

        #region OnValueChanged

        /// <summary>
        ///     Delegate for <see cref="OnValueChanged" />.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguements</param>
        public delegate void OnValueChangedDelegate(object sender, ValueChangedArgs e);

        /// <summary>
        ///     Called when an entry in the InternalCache is modified.
        /// </summary>
        public static event OnValueChangedDelegate OnValueChanged;

        #endregion OnValueChanged

        #endregion Events

        #region Singleton

        private static Cache _instance;

        /// <summary>
        ///     Gets the instance of Cache
        /// </summary>
        public static Cache Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = new Cache();
                return _instance;
            }
        }

        #endregion
    }

    /// <summary>
    ///     Arguements for the <see cref="Cache.OnValueChanged" /> event.
    /// </summary>
    public class ValueChangedArgs : EventArgs
    {
        /// <summary>
        ///     Key.
        /// </summary>
        public string Key;

        /// <summary>
        ///     The new value after it was changed.
        /// </summary>
        public object NewValue;

        /// <summary>
        ///     The old value prior to changing.
        /// </summary>
        public object OldValue;

        /// <summary>
        ///     The name of the region in the InternalCache.
        /// </summary>
        public string RegionName;

        internal ValueChangedArgs(string key, object oldValue, object newValue, string regionName)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
            RegionName = regionName;
        }
    }

    /// <summary>
    ///     Arguments for the <see cref="Cache.OnEntryAddedDelegate" /> event.
    /// </summary>
    public class EntryAddedArgs : EventArgs
    {
        /// <summary>
        ///     Key.
        /// </summary>
        public string Key;

        /// <summary>
        ///     The name of the region in the InternalCache.
        /// </summary>
        public string RegionName;

        /// <summary>
        ///     Value.
        /// </summary>
        public object Value;
    }
}