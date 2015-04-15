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
        private static readonly List<string> SavedTypes;

        static ObjectHandler()
        {
            // Create the CacheRegion
            Cache.Instance.CreateRegion(CacheRegionName);

            // Initialize the saved type list
            SavedTypes = new List<string>();

            // Create initial list
            foreach (var gameObj in ObjectManager.Get<GameObject>())
            {
                AddSavedType(gameObj.GetType().ToString());
                GetList(gameObj.GetType().ToString()).Add(gameObj);
            }


            // Subscribe to events
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObjectOnOnDelete;
        }

        #region Cache Methods

        /// <summary>
        ///     Gets the List of the object from the Cache.
        /// </summary>
        /// <param name="name">Name of the list(Typically the type)</param>
        /// <returns>List of GameObjects</returns>
        private static List<GameObject> GetList(string name)
        {
            // Gets the internal list, creating it if nessecary.
            object list;

            var contains = Cache.Instance.TryGetValue(name, out list, CacheRegionName);

            if (!contains)
            {
                list = Cache.Instance.AddOrGetExisting(
                    name, new List<GameObject>(), ObjectCache.InfiniteAbsoluteExpiration, CacheRegionName);
            }

            return (List<GameObject>) list;
        }

        /// <summary>
        ///     Adds a saved type to the list.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        private static void AddSavedType(string typeName)
        {
            if (SavedTypes.Contains(typeName))
            {
                return;
            }

            SavedTypes.Add(typeName);
        }

        #endregion

        #region GameObject Events

        private static void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            // Get the list from the Cache
            var list = GetList(sender.Type.ToString());

            // Add the object
            list.Add(sender);

            // Add the saved type
            AddSavedType(sender.Type.ToString());
        }

        private static void GameObjectOnOnDelete(GameObject sender, EventArgs args)
        {
            // Get the list from the Cache
            var list = GetList(sender.Type.ToString());

            // Remove the object
            list.RemoveAll(x => x.NetworkId == sender.NetworkId);
        }

        #endregion

        #region API / Public Methods

        /// <summary>
        ///     Gets all of the heroes in the game, using the Cache.
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
        ///     Gets all of the heroes in the game that are an enemy, using the Cache.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> EnemyHeroes
        {
            get { return AllHeroes.Where(x => x.IsEnemy); }
        }

        /// <summary>
        ///     Gets all of the heroes in the game that are an ally, using the Cache.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> AllyHeroes
        {
            get { return AllHeroes.Where(x => x.IsAlly); }
        }

        /// <summary>
        ///     Gets all of the <see cref="Obj_AI_Base" />'s in the game that are an enemy, using the Cache.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> AllEnemies
        {
            // TODO: Conduct speed test using .Where or using Cache
            get { return GetFast<Obj_AI_Base>().Where(x => x.IsEnemy); }
        }

        /// <summary>
        ///     Gets all of the <see cref="Obj_AI_Base" />'s in the game that are an ally, using the Cache.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> AllAllies
        {
            get { return GetFast<Obj_AI_Base>().Where(x => x.IsAlly); }
        }

        /// <summary>
        ///     Gets the Player, for easy replacing ObjectManager -> ObjectHandler
        /// </summary>
        public static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        
        /// <summary>
        ///     Queries the native <see cref="ObjectManager"/> to retrieve objects of the specific type.
        /// </summary>
        /// <typeparam name="T">Type of GameObject</typeparam>
        /// <returns>An enumerable list of the objects</returns>
        [Obsolete("Use ObjectHandler.GetFast<T>() instead for improved performance.")]
        public static IEnumerable<T> Get<T>() where T : GameObject, new()
        {
            return ObjectManager.Get<T>();
        }

        /// <summary>
        ///     Gets the unit with the specific network ID. Returns a default instance of the type if there is no object with that network id. (Check with IsValid)
        /// </summary>
        /// <typeparam name="T">Type of <see cref="GameObject"/></typeparam>
        /// <param name="networkId">Unit's Network ID</param>
        /// <returns>The unit with the specific network ID</returns>
        public static T GetUnitByNetworkId<T>(int networkId) where T: GameObject, new()
        {
            return SavedTypes.SelectMany(x => GetList(x).OfType<T>().Cast<GameObject>().Where(y => y.NetworkId == networkId)).Cast<T>().FirstOrDefault();
        }

        /// <summary>
        ///     Queries the <see cref="Cache" /> to retrieve objects of the specific type.
        /// </summary>
        /// <returns>IEnumerable of those objects.</returns>
        public static IEnumerable<T> GetFast<T>() where T : GameObject, new()
        {
            var list = new List<GameObject>();

            foreach (var type in SavedTypes)
            {
                list.AddRange(GetList(type).OfType<T>());
            }

            return list.ConvertAll(x => x as T);
        }

        #endregion
    }
}