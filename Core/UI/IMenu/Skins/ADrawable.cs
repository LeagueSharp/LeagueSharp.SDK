using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.IMenu.Skins
{
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    /// Defines a handler which is responsible for the drawing and interactions of an <see cref="AMenuComponent"/>.
    /// </summary>
    public abstract class ADrawable
    {
        /// <summary>
        /// Draws the <see cref="AMenuComponent"/>.
        /// </summary>
        public abstract void Draw();
        
        /// <summary>
        /// Calculates the width of this <see cref="AMenuComponent"/>.
        /// </summary>
        /// <returns>The width of this <see cref="AMenuComponent"/>.</returns>
        public abstract int Width();

        /// <summary>
        /// Handles the window events for this <see cref="AMenuComponent"/>.
        /// </summary>
        /// <param name="args">Event data</param>
        public abstract void OnWndProc(WindowsKeys args);

        /// <summary>
        /// Disposes any resources used in this handler.
        /// </summary>
        public abstract void Dispose();
    }

    /// <summary>
    /// Defines a handler which is responsible for the drawing and interactions of an <see cref="AMenuComponent"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ADrawable<T>:ADrawable where T:AMenuComponent
    {

        /// <summary>
        /// The <see cref="AMenuComponent"/> where this ADrawable is reponsible for.
        /// </summary>
        protected T Component { get; private set; }

        /// <summary>
        /// Creates a new handler responsible for the given <see cref="AMenuComponent"/>.
        /// </summary>
        /// <param name="component">The menu component</param>
        protected ADrawable(T component)
        {
            Component = component;
        }
        
    }
}
