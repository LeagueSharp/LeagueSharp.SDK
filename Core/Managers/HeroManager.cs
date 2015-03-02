using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Utils;

namespace LeagueSharp.CommonEx.Core.Managers
{
    /// <summary>
    ///     Caches all hereos.
    /// </summary>
    public class HeroManager
    {
        /// <summary>
        ///     Gets all of the <see cref="Obj_AI_Hero" />s.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> AllHeroes
        {
            get
            {
                return
                    (IEnumerable<Obj_AI_Hero>)
                        Cache.Instance.AddOrGetExisting("AllHeroes", ObjectManager.Get<Obj_AI_Hero>, "HeroManager");
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
                        Cache.Instance.AddOrGetExisting("Enemies", () => AllHeroes.Where(x => x.IsEnemy), "HeroManager");
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
                        Cache.Instance.AddOrGetExisting("Allies", () => AllHeroes.Where(x => x.IsAlly), "HeroManager");
            }
        }
    }
}