// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuList.cs" company="LeagueSharp">
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
//   A list of values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Values
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.UI.Skins.Default;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     A list of values.
    /// </summary>
    [Serializable]
    public abstract class MenuList : AMenuValue
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the user is hovering over the left arrow..
        /// </summary>
        /// <value>
        ///     <c>true</c> if the user is hovering over the left arrow; otherwise, <c>false</c>.
        /// </value>
        public bool LeftArrowHover { get; protected set; }

        /// <summary>
        ///     Gets the maximum width of the string.
        /// </summary>
        /// <value>
        ///     The maximum width of the string.
        /// </value>
        public abstract int MaxStringWidth { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether if the user is hovering over the right arrow.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the user is hovering over the right arrow; otherwise, <c>false</c>.
        /// </value>
        public bool RightArrowHover { get; protected set; }

        /// <summary>
        ///     Gets the selected value as an object.
        /// </summary>
        /// <value>
        ///     The selected value as an object.
        /// </value>
        public abstract object SelectedValueAsObject { get; }

        #endregion
    }

    /// <summary>
    ///     A list of values with a specific type.
    /// </summary>
    /// <typeparam name="T">Type of object in the list</typeparam>
    [Serializable]
    public class MenuList<T> : MenuList, ISerializable
    {
        #region Fields

        /// <summary>
        ///     Local width.
        /// </summary>
        private int width;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuList{T}" /> class.
        /// </summary>
        /// <param name="objects">The objects.</param>
        public MenuList(IEnumerable<T> objects)
        {
            this.Values = objects.ToList();
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="MenuList{T}" /> class based upon the given Enum type.
        /// </summary>
        public MenuList() : this(Enum.GetValues(typeof(T)).Cast<T>()) {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuList{T}" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public MenuList(SerializationInfo info, StreamingContext context)
        {
            this.Index = (int)info.GetValue("index", typeof(int));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the index.
        /// </summary>
        /// <value>
        ///     The index.
        /// </value>
        public int Index { get; set; }

        /// <summary>
        ///     Gets the maximum width of the string.
        /// </summary>
        /// <value>
        ///     The maximum width of the string.
        /// </value>
        public override int MaxStringWidth
        {
            get
            {
                if (this.width == 0)
                {
                    foreach (var obj in this.Values)
                    {
                        var newWidth = DefaultSettings.Font.MeasureText(null, obj.ToString(), 0).Width;
                        if (newWidth > this.width)
                        {
                            this.width = newWidth;
                        }
                    }
                }

                return this.width;
            }
        }

        /// <summary>
        ///     Menu Value Position.
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        ///     Gets the selected value.
        /// </summary>
        /// <value>
        ///     The selected value.
        /// </value>
        public T SelectedValue
        {
            get
            {
                return this.Values[this.Index];
            }
        }

        /// <summary>
        ///     Gets the selected value as an object.
        /// </summary>
        /// <value>
        ///     The selected value as an object.
        /// </value>
        public override object SelectedValueAsObject
        {
            get
            {
                return this.SelectedValue;
            }
        }

        /// <summary>
        ///     Gets or sets the values.
        /// </summary>
        /// <value>
        ///     The values.
        /// </value>
        public List<T> Values { get; set; }

        /// <summary>
        ///     Value Width.
        /// </summary>
        public override int Width
        {
            get
            {
                return ThemeManager.Current.List.Width(this);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public override void Extract(AMenuValue component)
        {
            this.Index = ((MenuList<T>)component).Index;
        }

        /// <summary>
        ///     Gets the object data.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("index", this.Index, typeof(int));
        }

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        /// <param name="component">Parent Component</param>
        /// <param name="position">The Position</param>
        /// <param name="index">Item Index</param>
        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            this.Position = position;

            ThemeManager.Current.List.OnDraw(component, position, index);
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            var rightArrowRect = ThemeManager.Current.List.RightArrow(this.Position, this.Container, this);
            var leftArrowRect = ThemeManager.Current.List.LeftArrow(this.Position, this.Container, this);
            if (args.Cursor.IsUnderRectangle(
                rightArrowRect.X, 
                rightArrowRect.Y, 
                rightArrowRect.Width, 
                rightArrowRect.Height))
            {
                this.RightArrowHover = true;
                if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    this.Index = (this.Index + 1) % this.Values.Count;
                    this.Container.FireEvent();
                }
            }
            else
            {
                this.RightArrowHover = false;
            }

            if (args.Cursor.IsUnderRectangle(
                leftArrowRect.X, 
                leftArrowRect.Y, 
                leftArrowRect.Width, 
                leftArrowRect.Height))
            {
                this.LeftArrowHover = true;
                if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    if (this.Index == 0)
                    {
                        this.Index = this.Values.Count - 1;
                    }
                    else
                    {
                        this.Index = (this.Index - 1) % this.Values.Count;
                        this.Container.FireEvent();
                    }
                }
            }
            else
            {
                this.LeftArrowHover = false;
            }
        }

        #endregion
    }
}