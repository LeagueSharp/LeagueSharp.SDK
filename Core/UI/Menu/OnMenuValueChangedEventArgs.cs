using System;
using LeagueSharp.CommonEx.Core.UI.Abstracts;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Arguements for the OnValueChanged event.
    /// </summary>
    public class OnMenuValueChangedEventArgs
    {
        /// <summary>
        ///  The new Value.
        /// </summary>
        public Menu Menu { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnMenuValueChangedEventArgs"/> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public OnMenuValueChangedEventArgs(Menu menu)
        {
            Menu = menu;
        }
    }
}