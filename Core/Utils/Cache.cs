using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace LeagueSharp.CommonEx.Core.Utils
{
    class Cache : ObjectCache
    {
        //TODO: Add monitors | Asignee - Chewy

        private static Cache _instance;

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

        private readonly Dictionary<string, Object> cache;        

        private Cache()
        {
            cache = new Dictionary<string, object>();
        }

        /// <summary>
        ///     NOT SUPPORTED(Yet)
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotSupportedException();
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
        ///     Adds a key, a value, and an expiration to the cache, returns the value if the key exists
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="absoluteExpiration">Time the keypair expires</param>
        /// <param name="regionName">The name of the region in the cache</param>
        /// <returns>The object stored in the cache</returns>
        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            if (!cache.ContainsKey(key + regionName))
            {
                cache.Add(key + regionName, value);
                DelayAction.Add(
                    (int) (absoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                    {
                        if (cache.ContainsKey(key + regionName))
                        {
                            cache.Remove(key + regionName);
                        }
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

                DelayAction.Add((int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                {
                    if (cache.ContainsKey(value.Key + value.RegionName))
                    {
                        cache.Remove(value.Key + value.RegionName);
                    }
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
        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            if (!cache.ContainsKey(key + regionName))
            {
                cache.Add(key + regionName, value);

                DelayAction.Add(
                    (int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
                    {
                        if (cache.ContainsKey(key + regionName))
                        {
                            cache.Remove(key + regionName);
                        }
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

            DelayAction.Add((int) (absoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
            {
                if (cache.ContainsKey(key))
                {
                    cache.Remove(key + regionName);
                }
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

            DelayAction.Add((int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
            {
                if(cache.ContainsKey(item.Key + item.RegionName))
                {
                    cache.Remove(item.Key + item.RegionName);
                }
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

            DelayAction.Add((int) (policy.AbsoluteExpiration - DateTime.Now).TotalMilliseconds, delegate
            {
                if (cache.ContainsKey(key + regionName))
                {
                    cache.Remove(key + regionName);
                }
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
            if (cache.ContainsKey(key + regionName))
            {
                var value = cache[key + regionName];
                cache.Remove(key + regionName);

                return value;
            }

            return null;
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
        ///     The capabilities of this instance of ObjectCache
        /// </summary>
        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { return DefaultCacheCapabilities.AbsoluteExpirations | DefaultCacheCapabilities.CacheRegions; }
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
}
