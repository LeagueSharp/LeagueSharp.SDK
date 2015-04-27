using System;

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     Menu StringList
    /// </summary>
    [Serializable]
    public struct MenuStringList
    {
        /// <summary>
        ///     Currently slected Index
        /// </summary>
        public int SelectedIndex;

        /// <summary>
        ///     String Array of the Items
        /// </summary>
        public string[] SList;

        /// <summary>
        ///     Creates the StringList
        /// </summary>
        /// <param name="sList">Array of Strings</param>
        /// <param name="defaultSelectedIndex">Default selected string</param>
        public MenuStringList(string[] sList, int defaultSelectedIndex = 0)
        {
            SList = sList;
            SelectedIndex = defaultSelectedIndex;
        }

        /// <summary>
        ///     Selected value
        /// </summary>
        public string SelectedValue
        {
            get { return SList[SelectedIndex]; }
        }
    }
}   