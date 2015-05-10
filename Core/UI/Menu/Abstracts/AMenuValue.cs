#region

using System;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Abstracts
{
    /// <summary>
    ///     Abstract build of a Menu Value.
    /// </summary>
    [Serializable]
    public abstract class AMenuValue
    {

        /// <summary>
        ///     Value Container.
        /// </summary>
        public MenuItem Container { get; set; }

        /// <summary>
        ///     Value Width.
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        ///     Menu Value Position.
        /// </summary>
        public abstract Vector2 Position { get; set; }

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        /// <param name="component">Parent Component</param>
        /// <param name="position">Position</param>
        /// <param name="index">Item Index</param>
        public abstract void OnDraw(AMenuComponent component, Vector2 position, int index);

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public abstract void OnWndProc(WindowsKeys args);

        /// <summary>
        /// Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public abstract void Extract(AMenuValue component);
    }
}