#region

using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Abstracts
{
    /// <summary>
    ///     Abstract build of a Menu Value.
    /// </summary>
    public abstract class AMenuValue
    {
        /// <summary>
        ///     Value Container.
        /// </summary>
        public AMenuComponent Container { get; set; }

        /// <summary>
        ///     Value Width.
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        public abstract void OnDraw();

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public abstract void OnWndProc(WindowsKeys args);
    }
}