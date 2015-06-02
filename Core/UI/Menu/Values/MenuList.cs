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
    using System.Threading;

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
        ///     Gets the maximum width of the string.
        /// </summary>
        /// <value>
        ///     The maximum width of the string.
        /// </value>
        internal abstract int MaxStringWidth { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether if the user is hovering over the dropdown.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the user is hovering over the dropdown; otherwise, <c>false</c>.
        /// </value>
        internal bool Hovering { get; set; }

        /// <summary>
        /// The index of the option that is currently being hovered at by the user.
        /// </summary>
        internal int HoveringIndex { get; set; }

        /// <summary>
        /// Dropdown menu active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     Gets the selected value as an object.
        /// </summary>
        /// <value>
        ///     The selected value as an object.
        /// </value>
        internal abstract object SelectedValueAsObject { get; }

        /// <summary>
        /// A list of strings that represent the different options
        /// </summary>
        internal abstract string[] ValuesAsStrings { get; }

        private int index;

        /// <summary>
        ///     Gets or sets the index.
        /// </summary>
        /// <value>
        ///     The index.
        /// </value>
        public int Index {
            get
            {
                return index;
            }
            set
            {
                if (value != index)
                {
                    index = value;
                    FireEvent();
                }
            }
        }

        /// <summary>
        /// The amount of options available
        /// </summary>
        internal abstract int Count { get; }

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
        ///     Initializes a new instance of the <see cref="MenuList{T}" /> class based upon the given Enumeration type.
        /// </summary>
        public MenuList()
            : this(Enum.GetValues(typeof(T)).Cast<T>()) {}

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
        ///     Gets the maximum width of the string.
        /// </summary>
        /// <value>
        ///     The maximum width of the string.
        /// </value>
        internal override int MaxStringWidth
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
        internal override object SelectedValueAsObject
        {
            get
            {
                return this.SelectedValue;
            }
        }

        /// <summary>
        ///     Gets the values.
        /// </summary>
        /// <value>
        ///     The values.
        /// </value>
        public List<T> Values { get; private set; }

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
        public override void OnDraw()
        {
            ThemeManager.Current.List.Draw(this);
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!this.Container.Visible)
            {
                return;
            }

            var dropdownRect = ThemeManager.Current.List.DropDownBoundaries(this);
            var entireDropdownRect = ThemeManager.Current.List.DropDownExpandedBoundaries(this);

            if (args.Cursor.IsUnderRectangle(dropdownRect.X, dropdownRect.Y, dropdownRect.Width, dropdownRect.Height))
            {
                Hovering = true;

                if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    Active = !Active;
                }
            }
            else
            {
                Hovering = false;
            }

            const int Buffer = 20;
            if (Active
                && !args.Cursor.IsUnderRectangle(
                    entireDropdownRect.X - Buffer,
                    entireDropdownRect.Y - Buffer,
                    entireDropdownRect.Width + (2 * Buffer),
                    entireDropdownRect.Height + (2 * Buffer)))
            {
                Active = false;
            }

            if (Active)
            {
                Boolean found = false;
                List<Rectangle> dropdownRectangles = ThemeManager.Current.List.DropDownListBoundaries(this);
                for (int i = 0; i < dropdownRectangles.Count; i++)
                {
                    if (args.Cursor.IsUnderRectangle(
                        dropdownRectangles[i].X,
                        dropdownRectangles[i].Y,
                        dropdownRectangles[i].Width,
                        dropdownRectangles[i].Height))
                    {
                        HoveringIndex = i;
                        found = true;
                    }
                }
                if (!found)
                {
                    HoveringIndex = -1;
                }
                else if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    Index = HoveringIndex;
                    args.Process = false;
                }
            }
        }

        #endregion

        internal override string[] ValuesAsStrings
        {
            get
            {
                string[] arr = new string[Values.Count];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = Values[i].ToString();
                }
                return arr;
            }
        }

        internal override int Count
        {
            get
            {
                return Values.Count;
            }
        }
    }
}