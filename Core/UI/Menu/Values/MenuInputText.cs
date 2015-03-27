#region

using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     InputText Menu Item.
    /// </summary>
    public class MenuInputText : AMenuValue
    {
        /// <summary>
        ///     InputText Constructor.
        /// </summary>
        /// <param name="text">text string</param>
        public MenuInputText(string text = null)
        {
            Text = text ?? "";
        }

        /// <summary>
        ///     InputText text string.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Returns if the InputText is focused.
        /// </summary>
        public bool IsFocused { get; set; }

        /// <summary>
        ///     InputText Item Width.
        /// </summary>
        public override int Width
        {
            get { return 0; }
        }

        /// <summary>
        ///     InputText Item Draw callback.
        /// </summary>
        public override void OnDraw() {}

        /// <summary>
        ///     InputText Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" />
        /// </param>
        public override void OnWndProc(WindowsKeys args) {}
    }
}