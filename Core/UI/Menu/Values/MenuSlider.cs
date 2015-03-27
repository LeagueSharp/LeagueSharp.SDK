#region

using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     Menu Slider.
    /// </summary>
    public class MenuSlider : AMenuValue
    {
        /// <summary>
        ///     Menu Slider Constructor.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="minValue">Minimum Value Boundary</param>
        /// <param name="maxValue">Maximum Value Boundary</param>
        public MenuSlider(int value = 0, int minValue = 0, int maxValue = 100)
        {
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        /// <summary>
        ///     Slider Current Value.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        ///     Slider Minimum Value.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        ///     Slider Maximum Value.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        ///     Slider Item Width.
        /// </summary>
        public override int Width
        {
            get { return 0; }
        }

        /// <summary>
        ///     Slider Item Draw callback.
        /// </summary>
        public override void OnDraw() {}

        /// <summary>
        ///     Slider Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" />
        /// </param>
        public override void OnWndProc(WindowsKeys args) {}
    }
}