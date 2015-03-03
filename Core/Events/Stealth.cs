using System.Linq;

namespace LeagueSharp.CommonEx.Core.Events
{
    /// <summary>
    ///     Provides events for OnStealth
    /// </summary>
    public class Stealth
    {
        /// <summary>
        ///     Delegate for the <see cref="OnStealth" /> event.
        /// </summary>
        /// <param name="sender">The hero who went invisible. Can be ally/enemy.</param>
        public delegate void OnStealthDelegate(Obj_AI_Hero sender);

        /// <summary>
        ///     All of the spells which make the unit invisible.
        /// </summary>
        public static string[] StealthSpells =
        {
            "KhazixR", "RengarR", "AkaliSmokeBomb", "Decieve", "TalonR",
            "TwitchHideInShadows", "MonkeyKingW"
        };

        static Stealth()
        {
            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;
        }

        private static void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (StealthSpells.Any(x => x == args.SData.Name))
            {
                FireOnStealth((Obj_AI_Hero) sender);
            }
        }

        /// <summary>
        ///     Gets fired when any hero is invisible.
        /// </summary>
        public static event OnStealthDelegate OnStealth;

        private static void FireOnStealth(Obj_AI_Hero sender)
        {
            var handler = OnStealth;

            if (handler != null)
            {
                handler(sender);
            }
        }
    }
}