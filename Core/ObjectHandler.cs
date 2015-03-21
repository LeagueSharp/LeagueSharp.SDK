using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using LeagueSharp.CommonEx.Core.Utils;

namespace LeagueSharp.CommonEx.Core
{
    /// <summary>
    ///     Provides cached objects and query functions for objects in League of Legends.
    /// </summary>
    public class ObjectHandler
    {
        private const string CacheRegionName = "ObjectHandler";
        private static readonly bool Loaded;
        private static readonly List<string> SavedTypes;

        static ObjectHandler()
        {
            if (Loaded)
            {
                return;
            }

            Cache.Instance.CreateRegion(CacheRegionName);
            SavedTypes = new List<string>();

            // Add all of the existing objects into the cache.
            foreach (var gameObj in ObjectManager.Get<GameObject>())
            {
                AddSavedType(gameObj);

                object obj;
                var contains = Cache.Instance.TryGetValue(gameObj.GetType().ToString(), out obj, CacheRegionName);

                if (!contains)
                {
                    obj = Cache.Instance.AddOrGetExisting(
                        gameObj.GetType().ToString(), new List<GameObject>(), ObjectCache.InfiniteAbsoluteExpiration,
                        CacheRegionName);
                }

                var list = (List<GameObject>) obj;
                list.Add(gameObj);

                Cache.Instance.Set(
                    gameObj.GetType().ToString(), list, ObjectCache.InfiniteAbsoluteExpiration, CacheRegionName);
            }

            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObjectOnOnDelete;

            Loaded = true;
        }

        /// <summary>
        ///     Gets all of the <see cref="Obj_AI_Hero" />s.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> AllHeroes
        {
            get
            {
                return
                    (IEnumerable<Obj_AI_Hero>)
                         Cache.Instance.AddOrGetExisting("AllHeroes", GetFast<Obj_AI_Hero>, CacheRegionName);
            }
        }

        /// <summary>
        ///     Gets all of the enemy <see cref="Obj_AI_Hero" />s.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> EnemyHeroes
        {
            get
            {
                return
                    (IEnumerable<Obj_AI_Hero>)
                        Cache.Instance.AddOrGetExisting(
                            "Enemies", () => AllHeroes.Where(x => x.IsEnemy), CacheRegionName);
            }
        }

        /// <summary>
        ///     Gets all of the ally <see cref="Obj_AI_Hero" />s.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> AllyHeroes
        {
            get
            {
                return
                    (IEnumerable<Obj_AI_Hero>)
                        Cache.Instance.AddOrGetExisting("Allies", () => AllHeroes.Where(x => x.IsAlly), CacheRegionName);
            }
        }

        /// <summary>
        ///     Gets all of the ally <see cref="Obj_AI_Base" />s.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> Allies
        {
            get { return GetFast<Obj_AI_Base>().Where(x => x.IsAlly); }
        }

        /// <summary>
        ///     Gets all of the enemy <see cref="Obj_AI_Base" />s.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> Enemies
        {
            get { return GetFast<Obj_AI_Base>().Where(x => x.IsEnemy); }
        }

        private static void GameObjectOnOnDelete(GameObject sender, EventArgs args)
        {
            object obj;
            var contains = Cache.Instance.TryGetValue(sender.GetType().ToString(), out obj, CacheRegionName);

            if (!contains)
            {
                // Obj_LampBulb op :D
                return;
            }

            var list = (List<GameObject>) obj;
            list.Remove(sender);

            Cache.Instance.Set(
                sender.GetType().ToString(), list, ObjectCache.InfiniteAbsoluteExpiration, CacheRegionName);
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            AddSavedType(sender);

            object obj;
            var contains = Cache.Instance.TryGetValue(sender.GetType().ToString(), out obj, CacheRegionName);

            if (!contains)
            {
                obj = Cache.Instance.AddOrGetExisting(
                    sender.GetType().ToString(), new List<GameObject>(), ObjectCache.InfiniteAbsoluteExpiration,
                    CacheRegionName);
            }

            var list = (List<GameObject>) obj;
            list.Add(sender);

            Cache.Instance.Set(
                sender.GetType().ToString(), list, ObjectCache.InfiniteAbsoluteExpiration, CacheRegionName);
        }

        private static void AddSavedType(GameObject gameObject)
        {
            if (!SavedTypes.Contains(gameObject.GetType().ToString()))
            {
                SavedTypes.Add(gameObject.GetType().ToString());
            }
        }

        /// <summary>
        ///     Queries <see cref="ObjectManager" />. This is considered slow, and should not be used.
        /// </summary>
        /// <typeparam name="T">Type of object to get</typeparam>
        /// <returns>IEnumerable of the objects.</returns>
        public static IEnumerable<T> Get<T>() where T : GameObject, new()
        {
            return ObjectManager.Get<T>();
        }

        /// <summary>
        ///     Queries the <see cref="Cache" /> for objects of the type.
        /// </summary>
        /// <typeparam name="T">Type of object to get, must be a <see cref="GameObject" /></typeparam>
        /// <returns>IEnumerable of the type.</returns>
        public static IEnumerable<T> GetFast<T>() where T : GameObject, new()
        {
            var list = new List<GameObject>();

            foreach (var savedType in SavedTypes)
            {
                list.AddRange(Cache.Instance.Get<List<GameObject>>(savedType, CacheRegionName).OfType<T>());
            }

            return list.ConvertAll(input => input as T);
        }
    }
}