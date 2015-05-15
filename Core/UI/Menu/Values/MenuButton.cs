using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.Values
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    /// A Button designed to perform an action when clicked
    /// </summary>
    public class MenuButton : AMenuValue
    {
        /// <summary>
        /// The action
        /// </summary>
        public delegate void ButtonAction();

        /// <summary>
        /// The text on the button
        /// </summary>
        public string ButtonText { get; set; }

        /// <summary>
        /// True if the mouse is hovering over it, false otherwise
        /// </summary>
        public bool Hovering { get; set; }

        /// <summary>
        /// The action to be performed when the button is clicked
        /// </summary>
        public ButtonAction Action { get; set; }

        /// <summary>
        /// Makes a new instance of MenuButton with the specified buttonText
        /// </summary>
        /// <param name="buttonText"></param>
        public MenuButton(string buttonText)
        {
            ButtonText = buttonText;
        }

        public override Vector2 Position { get; set; }

        public override int Width
        {
            get
            {
                return ThemeManager.Current.Button.Width(this);
            }
        }

        public override void Extract(AMenuValue component)
        {
            //donothing
        }

        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            Position = position;
            ThemeManager.Current.Button.OnDraw(component, position, index);
        }

        public override void OnWndProc(WindowsKeys args)
        {
            if (!Container.Visible)
            {
                return;
            }

            var rect = ThemeManager.Current.Button.ButtonBoundaries(Position, Container);

            if (args.Cursor.IsUnderRectangle(rect.X, rect.Y, rect.Width, rect.Height))
            {
                Hovering = true;
                if (args.Msg == WindowsMessages.LBUTTONDOWN && Action != null)
                {
                    Action();
                }
            }
            else
            {
                Hovering = false;
            }
        }
    }
}