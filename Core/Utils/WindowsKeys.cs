using System.Windows.Forms;
using LeagueSharp.CommonEx.Core.Enumerations;

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Utility class to translate Windows Messages into keys or into <see cref="WindowsMessages" />
    /// </summary>
    public class WindowsKeys
    {
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