#region

using System;
using System.Runtime.Serialization;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     Menu Slider.
    /// </summary>
    [Serializable]
    public class MenuSlider : AMenuValue, ISerializable
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
        /// Initializes a new instance of the <see cref="MenuSlider"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public MenuSlider(SerializationInfo info, StreamingContext context)
        {
            Value = (int) info.GetValue("value", typeof(int));
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MenuSlider"/> is interacting.
        /// </summary>
        /// <value>
        ///   <c>true</c> if interacting; otherwise, <c>false</c>.
        /// </value>
        public bool Interacting { get; private set; }

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
            get { return 100; }
        }

        /// <summary>
        ///     Slider Item Position.
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("value", Value, typeof(int));
        }

        /// <summary>
        ///     Slider Item Draw callback.
        /// </summary>
        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            if (!Position.Equals(position))
            {
                Position = position;
            }

            Theme.Animation animation = ThemeManager.Current.Boolean.Animation;
            if (animation != null && animation.IsAnimating())
            {
                animation.OnDraw(component, position, index);

                return;
            }
            ThemeManager.Current.Slider.OnDraw(component, position, index);
        }

        /// <summary>
        ///     Slider Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" />
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (args.Msg == WindowsMessages.MOUSEMOVE && Interacting)
            {
                CalculateNewValue(args);
            }
            else if (args.Msg == WindowsMessages.LBUTTONDOWN && !Interacting)
            {
                Rectangle container = ThemeManager.Current.Slider.Bounding(Position, Container);

                if (args.Cursor.IsUnderRectangle(container.X, container.Y, container.Width, container.Height))
                {
                    Interacting = true;
                    CalculateNewValue(args);
                }
            }
            else if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                Interacting = false;
            }
        }

        private void CalculateNewValue(WindowsKeys args)
        {
            var newValue =
                (int)
                    System.Math.Round(
                        (MinValue + ((args.Cursor.X - Position.X) * (MaxValue - MinValue)) / Container.MenuWidth));
            if (newValue < MinValue)
            {
                newValue = MinValue;
            }
            else if (newValue > MaxValue)
            {
                newValue = MaxValue;
            }
            if (newValue != Value)
            {
                Value = newValue;
                FireEvent(this);
            }
           
        }

        /// <summary>
        /// Extracts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Extract(AMenuValue value)
        {
            Value = ((MenuSlider) value).Value;
        }
    }
}