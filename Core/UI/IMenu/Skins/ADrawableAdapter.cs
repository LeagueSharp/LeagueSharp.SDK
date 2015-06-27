using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.IMenu.Skins
{
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    /// Provides an implementation of <see cref="ADrawable"/> that does nothing. This is used to prevent exceptions when no <see cref="ADrawable"/> exists for a given <see cref="AMenuComponent"/>.
    /// </summary>
    public class ADrawableAdapter : ADrawable
    {
        /// <summary>
        /// Draws the <see cref="AMenuComponent"/>.
        /// </summary>
        public override void Draw()
        {
            //do nothing
        }

        /// <summary>
        /// Calculates the width of this <see cref="AMenuComponent"/>.
        /// </summary>
        /// <returns>The width of this <see cref="AMenuComponent"/>.</returns>
        public override int Width()
        {
            //do nothing
            return 100;
        }

        /// <summary>
        /// Handles the window events for this <see cref="AMenuComponent"/>.
        /// </summary>
        /// <param name="args">Event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            //do nothing
        }

        /// <summary>
        /// Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            //do nothing
        }
    }
}
