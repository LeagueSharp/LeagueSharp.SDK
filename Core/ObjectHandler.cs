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
        private static readonly bool Loaded;

        static ObjectHandler()
        {
            if (Loaded)
            {
                return;
            }

            Cache.Instance.CreateRegion("ObjectHandler");

            // Add all of the existing objects into the cache.
            foreach (var gameObj in ObjectManager.Get<GameObject>())
            {
                object obj;
                var contains = Cache.Instance.TryGetValue(gameObj.Type.ToString(), out obj, "ObjectHandler");

                if (!contains)
                {
                    obj = Cache.Instance.AddOrGetExisting(
                        gameObj.Type.ToString(), new List<GameObject>(), ObjectCache.InfiniteAbsoluteExpiration,
                        "ObjectHandler");
                }

                var list = (List<GameObject>) obj;
                list.Add(gameObj);

                Cache.Instance.Set(
                    gameObj.Type.ToString(), list, ObjectCache.InfiniteAbsoluteExpiration, "ObjectHandler");
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
                        Cache.Instance.AddOrGetExisting("AllHeroes", ObjectManager.Get<Obj_AI_Hero>, "ObjectHandler");
            }
        }

        /// <summary>
        ///     Gets all of the enemy <see cref="Obj_AI_Hero" />s.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> Enemies
        {
            get
            {
                return
                    (IEnumerable<Obj_AI_Hero>)
                        Cache.Instance.AddOrGetExisting(
                            "Enemies", () => AllHeroes.Where(x => x.IsEnemy), "ObjectHandler");
            }
        }

        /// <summary>
        ///     Gets all of the ally <see cref="Obj_AI_Hero" />s.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> Allies
        {
            get
            {
                return
                    (IEnumerable<Obj_AI_Hero>)
                        Cache.Instance.AddOrGetExisting("Allies", () => AllHeroes.Where(x => x.IsAlly), "ObjectHandler");
            }
        }

        private static void GameObjectOnOnDelete(GameObject sender, EventArgs args)
        {
            object obj;
            var contains = Cache.Instance.TryGetValue(sender.Type.ToString(), out obj, "ObjectHandler");

            if (!contains)
            {
                // Obj_LampBulb op :D
                return;
            }

            var list = (List<GameObject>) obj;
            list.Remove(sender);

            Cache.Instance.Set(sender.Type.ToString(), list, ObjectCache.InfiniteAbsoluteExpiration, "ObjectHandler");
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            object obj;
            var contains = Cache.Instance.TryGetValue(sender.Type.ToString(), out obj, "ObjectHandler");

            if (!contains)
            {
                obj = Cache.Instance.AddOrGetExisting(
                    sender.Type.ToString(), new List<GameObject>(), ObjectCache.InfiniteAbsoluteExpiration,
                    "ObjectHandler");
            }

            var list = (List<GameObject>) obj;
            list.Add(sender);

            Cache.Instance.Set(sender.Type.ToString(), list, ObjectCache.InfiniteAbsoluteExpiration, "ObjectHandler");
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
            var gameObj = (GameObject) Convert.ChangeType(typeof(T), typeof(GameObject));
            return (IEnumerable<T>) Cache.Instance.Get<List<GameObject>>(gameObj.Type.ToString(), "ObjectHandler");
        }
    }
}