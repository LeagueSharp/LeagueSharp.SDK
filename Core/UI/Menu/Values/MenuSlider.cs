using System;

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     Menu Slider
    /// </summary>
    [Serializable]
    public struct MenuSlider
    {
        private int _value;

        /// <summary>
        ///     Max Value of the Slider
        /// </summary>
        public int MaxValue;

        /// <summary>
        ///     Min Value of the Slider
        /// </summary>
        public int MinValue;

        /// <summary>
        ///     Creates the Slider
        /// </summary>
        /// <param name="value">Value of the Slider</param>
        /// <param name="minValue">Min Value of the Slider</param>
        /// <param name="maxValue">Max Value of the Slider</param>
        public MenuSlider(int value = 0, int minValue = 0, int maxValue = 100)
        {
            MaxValue = System.Math.Max(maxValue, minValue);
            MinValue = System.Math.Min(maxValue, minValue);
            _value = value;
        }

        /// <summary>
        ///     Selected value of the Slider
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { _value = System.Math.Min(System.Math.Max(value, MinValue), MaxValue); }
        }
    }
}