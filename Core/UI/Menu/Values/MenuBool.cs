#region

using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     Menu Bool Value.
    /// </summary>
    public class MenuBool : AMenuValue
    {
        /// <summary>
        ///     Boolean Item Width requirement.
        /// </summary>
        public override int Width
        {
            get { return 0; }
        }

        /// <summary>
        ///     Bool Value.
        /// </summary>
        public bool Bool { get; set; }

        /// <summary>
        ///     Boolean Item Draw callback.
        /// </summary>
        public override void OnDraw() {}

        /// <summary>
        ///     Boolean Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public override void OnWndProc(WindowsKeys args) {}
    }
}