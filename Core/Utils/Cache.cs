using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Caching;

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Provides an implementation of ObjectCache, for any object.
    /// </summary>
    public class Cache : ObjectCache
    {
        //TODO: Fully implement monitors | Asignee - Chewy

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

        /// <summary>
        ///     Main Cache
        /// </summary>
        private readonly Dictionary<string, object> cache;

        /// <summary>
        ///     Holds callbacks that are called before cached item is removed
        /// </summary>
        private readonly Dictionary<string, CacheEntryUpdateCallback> cacheEntryUpdateCallbacks;

        /// <summary>
        ///     Holds callbacks that are called after cached item is removed
        /// </summary>
        private readonly Dictionary<string, CacheEntryRemovedCallback> cacheRemovedCallbacks;

        private Cache()
        {
            cache = new Dictionary<string, object>();
            cacheEntryUpdateCallbacks = new Dictionary<string, CacheEntryUpdateCallback>();
            cacheRemovedCallbacks = new Dictionary<string, CacheEntryRemovedCallback>();
        }

        /// <summary>
        ///     Calls the <see cref="CacheEntryUpdateCallback"/> for the selected key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="reason">Reason why the value was removed</param>
        /// <param name="regionName">The name of the region in the cache</param>
        private void CallEntryUpdates(string key,
            CacheEntryRemovedReason reason = CacheEntryRemovedReason.Removed,
            string regionName = null)
        {
            if (cacheEntryUpdateCallbacks.ContainsKey(key + regionName))
            {
                try
                {
                    cacheEntryUpdateCallbacks[key + regionName].Invoke(
                        new CacheEntryUpdateArguments(this, reason, key, regionName));
                }
                catch (Exception e)
                {
                    Logging.Write()(
                        LogLevel.Error, "An exception occured while invoking the EntryUpdateCallbacks: {0}", e);
                }
            }
        }

        /// <summary>
        ///     Calls the <see cref="CacheEntryRemovedCallback"/> for the selected key.
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
            if (cacheRemovedCallbacks.ContainsKey(key + regionName))
            {
                try
                {
                    cacheRemovedCallbacks[key + regionName].Invoke(
                        new CacheEntryRemovedArguments(this, reason, new CacheItem(key, value, regionName)));
                }
                catch (Exception e)
                {
                    Logging.Write()(
                        LogLevel.Error, "An exception occured while invoking the CacheEntryRemovedCallback: {0}", e);
                }
            }
        }

        /// <summary>
        ///     Creats an <see cref="CacheEntryChangeMonitor"/> of the selected keys
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns></returns>
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys,
            string regionName = null)
        {
            return new CacheEntryMonitor(
                keys as ReadOnlyCollection<string>, InfiniteAbsoluteExpiration, regionName,
                new Random(Environment.TickCount).Next());
        }

        /// <summary>
        ///     Gets the enumerator of the internal cache.
        /// </summary>
        /// <returns>Enumerator of cache</returns>
        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return cache.GetEnumerator();
        }

        /// <summary>
        ///     Checks if the cache contains the key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>Wether the key is in the cache</returns>
        public override bool Contains(string key, string regionName = null)
        {
            return cache.ContainsKey(key + regionName);
        }

        /// <summary>
        ///     Adds a key and a value, in the cache region. However, if the item exists, it will return the cached item. This KeyValuePair does not expire.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="regionName">The anme of the region in the cache.</param>
        /// <returns>The cahced value.</returns>
        public object AddOrGetExisting(string key, object value, string regionName = null)
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
            if (!cache.ContainsKey(key + regionName))
            {
                cache.Add(key + regionName, value);
                DelayAction.Add(
                    (int) (absoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                    {
                        if (!cache.ContainsKey(key + regionName))
                        {
                            return;
                        }

                        var cacheValue = cache[key + regionName];

                        CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                        cache.Remove(key + regionName);
                        CallEntryRemoved(key, cacheValue, CacheEntryRemovedReason.Expired, regionName);
                    });
            }
            else
            {
                return cache[key];
            }

            return null;
        }

        /// <summary>
        ///     Adds a CacheItem following the policy provided to the cache, but returns the CacheItem if it exists.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="policy">Policy</param>
        /// <returns>The CacheItem in the cache</returns>
        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            if (!cache.ContainsKey(value.Key + value.RegionName))
            {
                cache.Add(value.Key + value.RegionName, value.Value);

                cacheEntryUpdateCallbacks[value.Key + value.RegionName] = policy.UpdateCallback;
                cacheRemovedCallbacks[value.Key + value.RegionName] = policy.RemovedCallback;

                DelayAction.Add(
                    (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                    {
                        if (!cache.ContainsKey(value.Key + value.RegionName))
                        {
                            return;
                        }

                        var cachedValue = cache[value.Key + value.RegionName];

                        CallEntryUpdates(value.Key, CacheEntryRemovedReason.Expired, value.RegionName);
                        cache.Remove(value.Key + value.RegionName);
                        CallEntryRemoved(value.Key, cachedValue, CacheEntryRemovedReason.Expired, value.RegionName);
                    });
            }

            value.Value = cache[value.Key];
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
            if (!cache.ContainsKey(key + regionName))
            {
                cache.Add(key + regionName, value);

                cacheEntryUpdateCallbacks[key + regionName] = policy.UpdateCallback;
                cacheRemovedCallbacks[key + regionName] = policy.RemovedCallback;

                DelayAction.Add(
                    (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                    {
                        if (!cache.ContainsKey(key + regionName))
                        {
                            return;
                        }

                        var cachedValue = cache[key + regionName];

                        CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                        cache.Remove(key + regionName);
                        CallEntryRemoved(key, cachedValue, CacheEntryRemovedReason.Expired, regionName);
                    });
            }
            else
            {
                return cache[key];
            }

            return value;
        }

        /// <summary>
        ///     Gets a value in the cache by the key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>The object the key pairs with</returns>
        public override object Get(string key, string regionName = null)
        {
            return cache[key + regionName];
        }

        /// <summary>
        ///     Gets the CacheItem from the Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="regionName"></param>
        /// <returns>CacheItem in the cache</returns>
        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            return new CacheItem(key, cache[key + regionName], regionName);
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
            cache[key + regionName] = value;

            DelayAction.Add(
                (int) (absoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!cache.ContainsKey(key))
                    {
                        return;
                    }

                    var cachedValue = cache[key + regionName];

                    CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                    cache.Remove(key + regionName);
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
            cache[item.Key + item.RegionName] = item.Value;

            DelayAction.Add(
                (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!cache.ContainsKey(item.Key + item.RegionName))
                    {
                        return;
                    }

                    var cachedValue = cache[item.Key + item.RegionName];

                    CallEntryUpdates(item.Key, CacheEntryRemovedReason.Expired, item.RegionName);
                    cache.Remove(item.Key + item.RegionName);
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
            cache[key + regionName] = value;

            DelayAction.Add(
                (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (!cache.ContainsKey(key + regionName))
                    {
                        return;
                    }

                    var cachedValue = cache[key + regionName];

                    CallEntryUpdates(key, CacheEntryRemovedReason.Expired, regionName);
                    cache.Remove(key + regionName);
                    CallEntryRemoved(key, cachedValue, CacheEntryRemovedReason.Expired, regionName);
                });
        }

        /// <summary>
        ///     Gets the values of all the keys provided.
        /// </summary>
        /// <param name="keys">Keys to get the values of</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>Dictionary of all the keys, with the perspective values.</returns>
        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            return keys.Where(x => cache.ContainsKey(x + regionName)).ToDictionary(x => x, x => cache[x + regionName]);
        }

        /// <summary>
        ///     Removes KeyPair by the key in the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>The value of the key being removed</returns>
        public override object Remove(string key, string regionName = null)
        {
            if (!cache.ContainsKey(key + regionName))
            {
                return null;
            }

            if (cacheEntryUpdateCallbacks.ContainsKey(key + regionName))
            {
                cacheEntryUpdateCallbacks[key + regionName].Invoke(
                    new CacheEntryUpdateArguments(this, CacheEntryRemovedReason.Removed, key, regionName));
            }

            var value = cache[key + regionName];
            cache.Remove(key + regionName);

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
            return regionName != null ? cache.Count(x => x.Key.EndsWith(regionName)) : cache.Count;
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
        ///     Gets/Sets the value of a key in the Cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value matching the key</returns>
        public override object this[string key]
        {
            get { return cache[key]; }
            set { cache[key] = value; }
        }
    }

    /// <summary>
    ///     Not fully implemented, implementation of CacheEntryChangeMonitor 
    /// </summary>
    public class CacheEntryMonitor : CacheEntryChangeMonitor
    {
        private readonly ReadOnlyCollection<string> keys;
        private readonly DateTimeOffset lastModified;
        private readonly string regionName;
        private readonly int random;

        /// <summary>
        ///     Creates new CacheEntryMonitor
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="lastModified">Key's values last changed</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <param name="random">Unique ID</param>
        public CacheEntryMonitor(ReadOnlyCollection<string> keys,
            DateTimeOffset lastModified,
            string regionName,
            int random)
        {
            this.keys = keys;
            this.lastModified = lastModified;
            this.regionName = regionName;
            this.random = random;
        }

        protected override void Dispose(bool disposing) {}

        /// <summary>
        ///     Gets the unique id of this monitor
        /// </summary>
        public override string UniqueId
        {
            get { return "CommonEX Cache" + random; }
        }

        /// <summary>
        ///     Keys this monitor is monitoring
        /// </summary>
        public override ReadOnlyCollection<string> CacheKeys
        {
            get { return keys; }
        }

        /// <summary>
        ///     Last time keys were modified
        /// </summary>
        public override DateTimeOffset LastModified
        {
            get { return lastModified; }
        }

        /// <summary>
        ///     Region Name
        /// </summary>
        public override string RegionName
        {
            get { return regionName; }
        }
    }
}