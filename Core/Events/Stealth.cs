using System;
using System.Linq;

namespace LeagueSharp.CommonEx.Core.Events
{
    /// <summary>
    ///     Provides events for OnStealth
    /// </summary>
    public class Stealth
    {
        /// <summary>
        ///     All of the spells which make the unit invisible.
        /// </summary>
        public static string[] StealthSpells =
        {
            "KhazixR", "RengarR", "AkaliSmokeBomb", "Decieve", "TalonR",
            "TwitchHideInShadows", "MonkeyKingW"
        };

        /// <summary>
        ///     Static constructor.
        /// </summary>
        static Stealth()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        /// <summary>
        ///     Function is called when a notify event comes from the OnprocessSpellCast delegate, on a spell execution.
        /// </summary>
        /// <param name="sender">Sender in Obj_AI_Base form</param>
        /// <param name="args">Spell Data</param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (StealthSpells.Any(x => x == args.SData.Name))
            {
                FireOnStealth(
                    new OnStealthEventArgs
                    {
                        Sender = (Obj_AI_Hero) sender,
                        Spell = args.SData,
                        StartTime = args.TimeCast,
                        EndTime = args.TimeSpellEnd
                    });
            }
        }

        /// <summary>
        ///     Gets fired when any hero is invisible.
        /// </summary>
        public static event Action<OnStealthEventArgs> OnStealth;

        /// <summary>
        /// </summary>
        /// <param name="args">OnStealthEventArgs <see cref="OnStealthEventArgs" /></param>
        private static void FireOnStealth(OnStealthEventArgs args)
        {
            var handler = OnStealth;

            if (handler != null)
            {
                handler(args);
            }
        }
    }

    /// <summary>
    ///     On Stealth Event Data, contains useful information that is passed with OnStealth
    ///     <seealso cref="Stealth.OnStealth" />
    /// </summary>
    public struct OnStealthEventArgs
    {
        /// <summary>
        ///     Spell Time End
        /// </summary>
        public float EndTime;

        /// <summary>
        ///     Stealth Sender
        /// </summary>
        public Obj_AI_Hero Sender;

        /// <summary>
        ///     Stealth Spell Data
        /// </summary>
        public SpellData Spell;

        /// <summary>
        ///     Spell Start Time
        /// </summary>
        public float StartTime;
    }
}