// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuKeyBind.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Menu KeyBind.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Values
{
    using System;
    using System.Runtime.Serialization;
    using System.Windows.Forms;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.UI.Skins.Default;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Menu KeyBind.
    /// </summary>
    [Serializable]
    public class MenuKeyBind : AMenuValue, ISerializable
    {
        #region Fields

        /// <summary>
        ///     Local Interacting value.
        /// </summary>
        private bool interacting;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuKeyBind" /> class.
        ///     Menu KeyBind Constructor
        /// </summary>
        /// <param name="key">
        ///     The Key to bind
        /// </param>
        /// <param name="type">
        ///     Key bind type
        /// </param>
        public MenuKeyBind(Keys key, KeyBindType type)
        {
            this.Key = key;
            this.Type = type;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuKeyBind" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public MenuKeyBind(SerializationInfo info, StreamingContext context)
        {
            this.Key = (Keys)info.GetValue("key", typeof(Keys));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuKeyBind" /> class.
        ///     Menu KeyBind Constructor
        /// </summary>
        public MenuKeyBind()
        {
        }

        #endregion

        #region Public Properties

        private bool _active;
        /// <summary>
        ///     Gets or sets a value indicating whether the key is active.
        /// </summary>
        public bool Active {
            get
            {
                return _active;
            }
            set
            {
                if (_active != value)
                {
                    _active = value;
                    Container.FireEvent();
                }
            } 
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="MenuKeyBind" /> is interacting.
        /// </summary>
        /// <value>
        ///     <c>true</c> if interacting; otherwise, <c>false</c>.
        /// </value>
        public bool Interacting
        {
            get
            {
                return this.interacting;
            }

            set
            {
                this.interacting = value;
                MenuManager.Instance.ForcedOpen = value;
            }
        }

        /// <summary>
        ///     Gets or sets the KeyBind Key Value.
        /// </summary>
        public Keys Key { get; set; }

        /// <summary>
        ///     KeyBind Item Position.
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        ///     Gets or sets the KeyBind Type.
        /// </summary>
        public KeyBindType Type { get; set; }

        /// <summary>
        ///     KeyBind Item Width.
        /// </summary>
        public override int Width
        {
            get
            {
                return
                    (int)
                    (DefaultSettings.ContainerHeight + ThemeManager.Current.CalcWidthText("[" + this.Key + "]")
                     + DefaultSettings.ContainerTextOffset);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Extracts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Extract(AMenuValue value)
        {
            var keybind = (MenuKeyBind)value;
            this.Key = keybind.Key;
        }

        /// <summary>
        ///     Gets the object data.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("key", this.Key, typeof(Keys));
        }

        /// <summary>
        ///     KeyBind Item Draw callback.
        /// </summary>
        /// <param name="component">Parent Component</param>
        /// <param name="position">The Position</param>
        /// <param name="index">Item Index</param>
        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            if (!this.Position.Equals(position))
            {
                this.Position = position;
            }

            var animation = ThemeManager.Current.Boolean.Animation;

            if (animation != null && animation.IsAnimating())
            {
                animation.OnDraw(component, position, index);

                return;
            }

            ThemeManager.Current.KeyBind.OnDraw(component, position, index);
        }

        /// <summary>
        ///     KeyBind Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!MenuGUI.IsChatOpen)
            {
                switch (args.Msg)
                {
                    case WindowsMessages.KEYDOWN:
                        if (args.Key == this.Key && this.Type == KeyBindType.Press)
                        {
                            Active = true;
                        }
                        break;
                    case WindowsMessages.KEYUP:
                        if (this.Interacting && args.SingleKey != Keys.ShiftKey)
                        {
                            this.Key = args.SingleKey == Keys.Escape ? Keys.None : args.Key;
                            args.Process = false;
                            this.Interacting = false;
                            this.Container.ResetWidth();
                        }
                        else if (args.Key == this.Key && this.Type == KeyBindType.Press)
                        {
                            Active = false;
                        }
                        else if (args.Key == this.Key && this.Type == KeyBindType.Toggle)
                        {
                            this.Active = !this.Active;
                        }
                        break;
                    case WindowsMessages.XBUTTONDOWN:
                        if (args.SideButton == this.Key && this.Type == KeyBindType.Press)
                        {
                            Active = true;
                        }
                        break;
                    case WindowsMessages.XBUTTONUP:
                        if (this.Interacting)
                        {
                            this.Key = args.SideButton;
                            args.Process = false;
                            this.Interacting = false;
                            this.Container.ResetWidth();
                        }
                        else if (args.SideButton == this.Key && this.Type == KeyBindType.Press)
                        {
                            Active = false;
                        }
                        else if (args.SideButton == this.Key && this.Type == KeyBindType.Toggle)
                        {
                            this.Active = !this.Active;
                        }
                        break;
                    case WindowsMessages.MBUTTONDOWN:
                        if (Key == Keys.MButton && this.Type == KeyBindType.Press)
                        {
                            Active = true;
                        }
                        break;
                    case WindowsMessages.MBUTTONUP:
                        if (this.Interacting)
                        {
                            this.Key = Keys.MButton;
                            args.Process = false;
                            this.Interacting = false;
                            this.Container.ResetWidth();
                        }
                        else if (Keys.MButton == this.Key && this.Type == KeyBindType.Press)
                        {
                            Active = false;
                        }
                        else if (Keys.MButton == this.Key && this.Type == KeyBindType.Toggle)
                        {
                            this.Active = !this.Active;
                        }
                        break;
                    case WindowsMessages.RBUTTONDOWN:
                        if (Key == Keys.RButton && this.Type == KeyBindType.Press)
                        {
                            Active = true;
                        }
                        break;
                    case WindowsMessages.RBUTTONUP:
                        if (this.Interacting)
                        {
                            this.Key = Keys.RButton;
                            args.Process = false;
                            this.Interacting = false;
                            this.Container.ResetWidth();
                        }
                        else if (Keys.RButton == this.Key && this.Type == KeyBindType.Press)
                        {
                            Active = false;
                        }
                        else if (Keys.RButton == this.Key && this.Type == KeyBindType.Toggle)
                        {
                            this.Active = !this.Active;
                        }
                        break;
                    case WindowsMessages.LBUTTONDOWN:
                        if (Interacting)
                        {
                            this.Key = Keys.LButton;
                            args.Process = false;
                            this.Interacting = false;
                            this.Container.ResetWidth();
                        } else if (Container.Visible && this.Position.IsValid())
                        {
                            var container = ThemeManager.Current.KeyBind.AdditionalBoundries(
                                this.Position,
                                this.Container);
                            var content = ThemeManager.Current.KeyBind.Bounding(this.Position, this.Container);

                            if (args.Cursor.IsUnderRectangle(
                                container.X,
                                container.Y,
                                container.Width,
                                container.Height))
                            {
                                this.Active = !this.Active;
                            }
                            else if (args.Cursor.IsUnderRectangle(content.X, content.Y, content.Width, content.Height))
                            {
                                this.Interacting = !this.Interacting;
                            }
                            else
                            {
                                if (Key == Keys.LButton && this.Type == KeyBindType.Press)
                                {
                                    Active = true;
                                }
                            }
                        }
                        break;
                    case WindowsMessages.LBUTTONUP:
                        if (Keys.LButton == this.Key && this.Type == KeyBindType.Press)
                        {
                            Active = false;
                        }
                        else if (Keys.LButton == this.Key && this.Type == KeyBindType.Toggle)
                        {
                            this.Active = !this.Active;
                        }
                        break;
                }
            }
        }

        #endregion
    }
}