#region

using System.Windows.Forms;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     Menu KeyBind.
    /// </summary>
    public class MenuKeyBind : AMenuValue
    {
        /// <summary>
        ///     Menu KeyBind Constructor
        /// </summary>
        /// <param name="key">The Key to bind</param>
        /// <param name="type">Key bind type</param>
        public MenuKeyBind(Keys key, KeyBindType type)
        {
            Key = key;
            Type = type;
        }

        /// <summary>
        ///     Menu KeyBind Constructor
        /// </summary>
        /// <param name="key">The Key to bind</param>
        public MenuKeyBind(Keys key)
        {
            Key = key;
        }

        /// <summary>
        ///     Menu KeyBind Constructor
        /// </summary>
        public MenuKeyBind() {}

        /// <summary>
        ///     KeyBind Item Width.
        /// </summary>
        public override int Width
        {
            get { return 0; }
        }

        /// <summary>
        ///     Returns if the key is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     KeyBind Type.
        /// </summary>
        public KeyBindType Type { get; set; }

        /// <summary>
        ///     KeyBind Key Value.
        /// </summary>
        public Keys Key { get; set; }

        /// <summary>
        ///     KeyBind Item Draw callback.
        /// </summary>
        public override void OnDraw(Vector2 position) {}

        /// <summary>
        ///     KeyBind Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" />
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (args.Msg == WindowsMessages.KEYDOWN && args.Key == Key && Type == KeyBindType.Press)
            {
                Active = true;
            }
            else if (args.Msg == WindowsMessages.KEYUP && args.Key == Key && Type == KeyBindType.Press)
            {
                Active = false;
            }
            else if (args.Msg == WindowsMessages.KEYUP && args.Key == Key && Type == KeyBindType.Toggle)
            {
                Active = !Active;
            }
        }
    }
}