#region

using System;
using System.Runtime.Serialization;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.UI.Skins.Default;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     Menu Bool.
    /// </summary>
    [Serializable]
    public class MenuBool : AMenuValue, ISerializable
    {
        /// <summary>
        ///     Constructor for MenuBool
        /// </summary>
        /// <param name="value">Bool Value</param>
        public MenuBool(bool value = false)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuBool"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public MenuBool(SerializationInfo info, StreamingContext context)
        {
            Value = (bool) info.GetValue("value", typeof(bool));
        }

        /// <summary>
        ///     Boolean Item Width requirement.
        /// </summary>
        public override int Width
        {
            get { return (int) DefaultSettings.ContainerHeight; }
        }

        /// <summary>
        ///     Bool Value.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        ///     Boolean Item Position.
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("value", Value, typeof(bool));
        }

        /// <summary>
        ///     Boolean Item Draw callback.
        /// </summary>
        /// <param name="component">Parent Component</param>
        /// <param name="position">Position</param>
        /// <param name="index">Item Index</param>
        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            Position = position;

            Theme.Animation animation = ThemeManager.Current.Boolean.Animation;

            if (animation != null && animation.IsAnimating())
            {
                animation.OnDraw(component, position, index);

                return;
            }
            ThemeManager.Current.Boolean.OnDraw(component, position, index);
        }

        /// <summary>
        ///     Boolean Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">WindowsKeys</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (args.Msg == WindowsMessages.LBUTTONDOWN && Position.IsValid())
            {
                Rectangle rect = ThemeManager.Current.Boolean.AdditionalBoundries(Position, Container);

                if (args.Cursor.IsUnderRectangle(rect.X, rect.Y, rect.Width, rect.Height))
                {
                    Value = !Value;
                    Container.FireEvent();
                }
            }
        }

        /// <summary>
        /// Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public override void Extract(AMenuValue component)
        {
            Value = ((MenuBool) component).Value;
        }
    }
}