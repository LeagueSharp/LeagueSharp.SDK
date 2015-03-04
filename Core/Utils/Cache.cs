#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

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
        ///     Main Cache
        /// </summary>
        private readonly SortedDictionary<CacheItem, object> cache;

        /// <summary>
        ///     Holds callbacks that are called before cached item is removed
        /// </summary>
        private readonly SortedDictionary<string, CacheEntryUpdateCallback> cacheEntryUpdateCallbacks;

        /// <summary>
        ///     Holds callbacks that are called after cached item is removed
        /// </summary>
        private readonly SortedDictionary<string, CacheEntryRemovedCallback> cacheRemovedCallbacks;

        private Cache()
        {
            // We have to create the default region, else we get exceptions :^(
            cache = new SortedDictionary<CacheItem, object>();

            cacheEntryUpdateCallbacks = new SortedDictionary<string, CacheEntryUpdateCallback>();
            cacheRemovedCallbacks = new SortedDictionary<string, CacheEntryRemovedCallback>();
        }

        /// <summary>
        ///     The capabilities of this implementation of ObjectCache
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
        ///     Returns the name of the Cache
        /// </summary>
        public override string Name
        {
            get { return "CommonEX Cache"; }
        }

        /// <summary>
        ///     Gets/Sets the value of a key in the Cache, using the default region name
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value matching the key</returns>
        public override object this[string key]
        {
            get { return cache[new CacheItem(key)]; }
            set { cache[new CacheItem(key)] = value; }
        }

        /// <summary>
        ///     Calls the <see cref="CacheEntryUpdateCallback" /> for the selected key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="reason">Reason why the value was removed</param>
        /// <param name="regionName">The name of the region in the cache</param>
        private void CallEntryUpdates(string key,
            CacheEntryRemovedReason reason = CacheEntryRemovedReason.Removed,
            string regionName = null)
        {
            CacheEntryUpdateCallback callback;
            var contains = cacheEntryUpdateCallbacks.TryGetValue(key + regionName, out callback);

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
        /// <param name="regionName">The name of the region in the cache</param>
        private void CallEntryRemoved(string key,
            object value,
            CacheEntryRemovedReason reason = CacheEntryRemovedReason.Removed,
            string regionName = null)
        {
            CacheEntryRemovedCallback callback;
            var contains = cacheRemovedCallbacks.TryGetValue(key + regionName, out callback);

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
        ///     Tries to get the value, returns false if the cache does not contain that key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value(Null if doesnt exist)</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>If the cache contains the value</returns>
        public bool TryGetValue(string key, out object value, string regionName = null)
        {
            return cache.TryGetValue(new CacheItem(key, regionName), out value);
        }

        /// <summary>
        ///     Not supported. Please use the events.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns></returns>
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys,
            string regionName = null)
        {
            throw new NotSupportedException("CacheEntryChangeMonitors are not implemented.");
        }

        /// <summary>
        ///     Gets the enumerator of the default internal cache.
        /// </summary>
        /// <returns>Enumerator of cache</returns>
        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return cache.ToDictionary(x => x.Key.Key, x => x.Value).GetEnumerator();
        }

        /// <summary>
        ///     Checks if the cache contains the key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>Wether the key is in the cache</returns>
        public override bool Contains(string key, string regionName = null)
        {
            return cache.Keys.Any(x => x.Key == key && x.RegionName == regionName);
        }

        /// <summary>
        ///     Adds or Gets an existing key/value, executing the function if the key does not exist.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="function">Function that will get the value</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns></returns>
        public object AddOrGetExisting(string key, Func<object> function, string regionName = null)
        {
            var cacheItem = new CacheItem(key, null, regionName);
            object cachedObject;
            var contains = cache.TryGetValue(cacheItem, out cachedObject);

            if (contains)
            {
                return cachedObject;
            }

            var result = function();
            cache.Add(cacheItem, result);

            if (OnEntryAdded != null)
            {
                OnEntryAdded(key, result, regionName);
            }

            return result;
        }

        /// <summary>
        ///     Adds a key and a value, in the cache region. However, if the item exists, it will return the cached item. This
        ///     KeyValuePair does not expire.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="regionName">The anme of the region in the cache.</param>
        /// <returns>The cahced value.</returns>
        public object AddOrGetExisting(string key, object value, string regionName = "Default")
        {
            return AddOrGetExisting(key, value, InfiniteAbsoluteExpiration, regionName);
        }

        /// <summary>
        ///     Adds a key, a value, and an expiration to the cache, returns the value if the key exists
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="absoluteExpiration">Time the keypair expires</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>The object stored in the cache</returns>
        public override object AddOrGetExisting(string key,
            object value,
            DateTimeOffset absoluteExpiration,
            string regionName = null)
        {
           return AddOrGetExisting(new CacheItem(key, value, regionName), new CacheItemPolicy {AbsoluteExpiration = absoluteExpiration}).Value;
        }

        /// <summary>
        ///     Adds a CacheItem following the policy provided to the cache, but returns the CacheItem if it exists.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="policy">Policy</param>
        /// <returns>The CacheItem in the cache</returns>
        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            object internalValue;
            var contains = cache.TryGetValue(value, out internalValue);

            if (contains)
            {
                return new CacheItem(value.Key, internalValue, value.RegionName);
            }

            if (OnEntryAdded != null)
            {
                OnEntryAdded(value.Key, value.Value, value.RegionName);
            }

            // Do not import the value of item.
            cache.Add(new CacheItem(value.Key, value.RegionName), value.Value);

            cacheEntryUpdateCallbacks[value.Key + value.RegionName] = policy.UpdateCallback;
            cacheRemovedCallbacks[value.Key + value.RegionName] = policy.RemovedCallback;

            DelayAction.Add(
                (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!cache.ContainsKey(value))
                    {
                        return;
                    }

                    var cachedValue = cache[value];

                    CallEntryUpdates(value.Key, CacheEntryRemovedReason.Expired, value.RegionName);
                    cache.Remove(value);
                    CallEntryRemoved(value.Key, cachedValue, CacheEntryRemovedReason.Expired, value.RegionName);
                });

            return value;
        }

        /// <summary>
        ///     Adds a key and a value to the cache, following the policy, returns the cached value if it exists.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="policy">Policy</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>Cached value</returns>
        public override object AddOrGetExisting(string key,
            object value,
            CacheItemPolicy policy,
            string regionName = null)
        {
            return AddOrGetExisting(new CacheItem(key, value, regionName), policy);
        }

        /// <summary>
        ///     Gets a value in the cache by the key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>The object the key pairs with</returns>
        public override object Get(string key, string regionName = null)
        {
            return cache[new CacheItem(key, null, regionName)];
        }

        /// <summary>
        ///     Gets a value from the cache, and casts it to the type.
        /// </summary>
        /// <typeparam name="T">Type to cast to</typeparam>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>The object casted into T</returns>
        public T Get<T>(string key, string regionName = null)
        {
            var value = cache[new CacheItem(key, null, regionName)];
            return value is T ? (T) value : default(T);
        }

        /// <summary>
        ///     Gets the CacheItem from the Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="regionName"></param>
        /// <returns>CacheItem in the cache</returns>
        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            return new CacheItem(key, cache[new CacheItem(key, null, regionName)], regionName);
        }

        /// <summary>
        ///     Sets the value of a key in the cache, and expires at a set time. The key does not have to be created.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="absoluteExpiration">Expiration</param>
        /// <param name="regionName">The name of the region in the cache</param>
        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            var cacheItem = new CacheItem(key, null, regionName);

            if (OnValueChanged != null && cache.ContainsKey(cacheItem))
            {
                OnValueChanged(new ValueChangedArgs(key, cache[cacheItem], value, regionName));
            }

            cache[cacheItem] = value;

            DelayAction.Add(
                (int) (absoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!cache.ContainsKey(cacheItem))
                    {
                        return;
                    }

                    var cachedValue = cache[cacheItem];

                    CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                    cache.Remove(cacheItem);
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
            var cacheItem = new CacheItem(item.Key, null, item.RegionName);
            if (OnValueChanged != null && cache.ContainsKey(cacheItem))
            {
                OnValueChanged(new ValueChangedArgs(item.Key, cache[cacheItem], item.Value, item.RegionName));
            }

            cache[cacheItem] = item.Value;

            DelayAction.Add(
                (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!cache.ContainsKey(cacheItem))
                    {
                        return;
                    }

                    var cachedValue = cache[cacheItem];

                    CallEntryUpdates(item.Key, CacheEntryRemovedReason.Expired, item.RegionName);
                    cache.Remove(cacheItem);
                    CallEntryRemoved(item.Key, cachedValue, CacheEntryRemovedReason.Expired, item.RegionName);
                });
        }

        /// <summary>
        ///     Sets the value of a key, following the policy provided.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="policy">Policy</param>
        /// <param name="regionName">The name of the region in the cache</param>
        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            Set(new CacheItem(key, value, regionName), policy);
        }

        /// <summary>
        ///     Gets the values of all the keys provided.
        /// </summary>
        /// <param name="keys">Keys to get the values of</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>Dictionary of all the keys, with the perspective values.</returns>
        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            return keys.Where(x => cache.ContainsKey(new CacheItem(x, null, regionName))).ToDictionary(x => x, x => cache[new CacheItem(x, null, regionName)]);
        }

        /// <summary>
        ///     Removes KeyPair by the key in the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>The value of the key being removed</returns>
        public override object Remove(string key, string regionName = null)
        {
            var cacheItem = new CacheItem(key, null, regionName);

            if (!cache.ContainsKey(cacheItem))
            {
                return null;
            }

            if (cacheEntryUpdateCallbacks.ContainsKey(key + regionName))
            {
                cacheEntryUpdateCallbacks[key + regionName].Invoke(
                    new CacheEntryUpdateArguments(this, CacheEntryRemovedReason.Removed, key, regionName));
            }

            var value = cache[cacheItem];
            cache.Remove(cacheItem);

            if (cacheRemovedCallbacks.ContainsKey(key + regionName))
            {
                cacheRemovedCallbacks[key + regionName].Invoke(
                    new CacheEntryRemovedArguments(
                        this, CacheEntryRemovedReason.Removed, new CacheItem(key, value, regionName)));
            }

            return value;
        }

        /// <summary>
        ///     Gets the count of KeyPairs in the Cache.
        /// </summary>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>Count of KeyPairs in the Cache</returns>
        public override long GetCount(string regionName = null)
        {
            return cache.Count(x => x.Key.RegionName == regionName);
        }

        #region Events

        #region OnEntryAdded

        /// <summary>
        ///     Delegate when a value is added
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="regionName">The name of the region in the cache</param>
        public delegate void OnEntryAddedDelegate(string key, object value, string regionName);

        /// <summary>
        ///     Gets called when a new value is added to the cache.
        /// </summary>
        public static event OnEntryAddedDelegate OnEntryAdded;

        #endregion OnEntryAdded

        #region OnValueChanged

        /// <summary>
        ///     Delegate for <see cref="OnValueChanged" />
        /// </summary>
        /// <param name="args">Arguements</param>
        public delegate void OnValueChangedDelegate(ValueChangedArgs args);

        /// <summary>
        ///     Called when an entry in the cache is modified
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
    ///     Arguements for the <see cref="Cache.OnValueChanged"/> event
    /// </summary>
    public struct ValueChangedArgs
    {
        /// <summary>
        ///     Key
        /// </summary>
        public string Key;

        /// <summary>
        ///     The new value after it was changed
        /// </summary>
        public object NewValue;

        /// <summary>
        ///     The old value prior to changing
        /// </summary>
        public object OldValue;

        /// <summary>
        ///     The name of the region in the cache
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
}