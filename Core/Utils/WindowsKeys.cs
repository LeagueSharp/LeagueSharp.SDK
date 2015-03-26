using System.Windows.Forms;
using LeagueSharp.CommonEx.Core.Enumerations;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Utility class to translate Windows Messages into keys or into <see cref="WindowsMessages" />
    /// </summary>
    public class WindowsKeys
    {
        private readonly WndEventArgs _args;

        /// <summary>
        ///     WindowsKeys constructor
        /// </summary>
        /// <param name="args">
        ///     <see cref="WndEventArgs" />
        /// </param>
        public WindowsKeys(WndEventArgs args)
        {
            _args = args;
            Cursor = Utils.Cursor.Position;
        }

        /// <summary>
        ///     Process key action.
        /// </summary>
        public bool Process
        {
            get { return _args.Process; }
            set { _args.Process = value; }
        }

        /// <summary>
        ///     Cursor Position
        /// </summary>
        public Vector2 Cursor { get; private set; }

        /// <summary>
        ///     The full name of the mapped key.
        /// </summary>
        public Keys Key
        {
            get
            {
                Keys keyData;
                if ((Keys) ((int) _args.WParam) != Control.ModifierKeys)
                {
                    keyData = (Keys) ((int) _args.WParam) | Control.ModifierKeys;
                }
                else
                {
                    keyData = (Keys) ((int) _args.WParam);
                }
                return keyData;
            }
        }

        /// <summary>
        ///     The single name of the mapped key.
        /// </summary>
        public Keys SingleKey
        {
            get { return (Keys) ((int) _args.WParam); }
        }

        /// <summary>
        ///     The message of the key.
        /// </summary>
        public WindowsMessages Msg
        {
            get { return (WindowsMessages) _args.Msg; }
        }

        /// <summary>
        ///     Returns the set collection of keys from the <see cref="WndEventArgs" />.
        /// </summary>
        /// <param name="args"><see cref="WndEventArgs" /> data</param>
        /// <returns><see cref="System.Windows.Forms.Keys" /> data</returns>
        public static Keys GetKeys(WndEventArgs args)
        {
            Keys keyData;
            if ((Keys) ((int) args.WParam) != Control.ModifierKeys)
            {
                keyData = (Keys) ((int) args.WParam) | Control.ModifierKeys;
            }
            else
            {
                keyData = (Keys) ((int) args.WParam);
            }
            return keyData;
        }

        /// <summary>
        ///     Returns the key from the <see cref="WndEventArgs" />.
        /// </summary>
        /// <param name="args"><see cref="WndEventArgs" /> data</param>
        /// <returns><see cref="System.Windows.Forms.Keys" /> data</returns>
        public static Keys GetKey(WndEventArgs args)
        {
            return (Keys) ((int) args.WParam);
        }

        /// <summary>
        ///     Returns the translated windows message from the <see cref="WndEventArgs" />
        /// </summary>
        /// <param name="args"><see cref="WndEventArgs" /> data</param>
        /// <returns>
        ///     <see cref="WindowsMessages" />
        /// </returns>
        public static WindowsMessages GetWindowsMessage(WndEventArgs args)
        {
            return (WindowsMessages) args.Msg;
        }
    }
}