namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    /// VirtualKeys class used to manage virtual keys.
    /// </summary>
    public static class VirtualKeys
    {
        /// <summary>
        /// Fixes the given virtual key.
        /// </summary>
        /// <param name="key">Key as byte</param>
        /// <returns>Returns the fixed virtual key.</returns>
        public static byte FixVirtualKey(byte key)
        {
            switch (key)
            {
                case 160:
                    return 0x10;
                case 161:
                    return 0x10;
                case 162:
                    return 0x11;
                case 163:
                    return 0x11;
            }

            return key;
        }

        /// <summary>
        /// Converts a given Key to text.
        /// </summary>
        /// <param name="vKey">Key as uint</param>
        /// <returns>Returns the given Key as string</returns>
        public static string KeyToText(uint vKey)
        {
            /*A-Z */
            if (vKey >= 65 && vKey <= 90)
            {
                return ((char)vKey).ToString();
            }

            /*F1-F12*/
            if (vKey >= 112 && vKey <= 123)
            {
                return ("F" + (vKey - 111));
            }

            switch (vKey)
            {
                case 9:
                    return "Tab";
                case 16:
                    return "Shift";
                case 17:
                    return "Ctrl";
                case 20:
                    return "CAPS";
                case 27:
                    return "ESC";
                case 32:
                    return "Space";
                case 45:
                    return "Insert";
                case 220:
                    return "º";
                default:
                    return vKey.ToString();
            }
        }
    }
}
