using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.IMenu.Skins
{
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     Defines how to draw and interact with an AMenuComponent.
    /// </summary>
    public interface IDrawable<in T> where T:AMenuComponent
    {
        /// <summary>
        ///     Draws an AMenuComponent
        /// </summary>
        /// <param name="component">The <see cref="MenuBool" /></param>
        void Draw(T component);

        /// <summary>
        /// Calculates the Width of an AMenuComponent
        /// </summary>
        /// <param name="component">menu component</param>
        /// <returns>width</returns>
        int Width(T component);

        /// <summary>
        /// Processes windows messages
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">event data</param>
        void OnWndProc(T component, WindowsKeys args);
    }
}
