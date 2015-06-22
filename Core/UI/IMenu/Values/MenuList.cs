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
namespace LeagueSharp.SDK.Core.UI.IMenu.Values
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Skins;
    using LeagueSharp.SDK.Core.UI.IMenu.Skins.Default;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     A list of values.
    /// </summary>
    [Serializable]
    public abstract class MenuList : MenuItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuList" /> class.
        /// </summary>
        /// <param name="name">Internal name of the component</param>
        /// <param name="displayName">Display name of the component</param>
        /// <param name="uniqueString">String to make this component unique</param>
        protected MenuList(string name, string displayName, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            
        }

        internal MenuList()
            : base()
        {
            
        }

        #region Fields

        /// <summary>
        ///     The index.
        /// </summary>
        private int index;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the dropdown menu is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     Gets the amount of options available
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether if the user is hovering over the dropdown.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the user is hovering over the dropdown; otherwise, <c>false</c>.
        /// </value>
        public bool Hovering { get; set; }

        /// <summary>
        ///     Gets or sets the index of the option that is currently being hovered at by the user.
        /// </summary>
        public int HoveringIndex { get; set; }

        /// <summary>
        ///     Gets or sets the index.
        /// </summary>
        /// <value>
        ///     The index.
        /// </value>
        public int Index
        {
            get
            {
                return this.index;
            }

            set
            {
                if (value != this.index)
                {
                    this.index = value;
                    this.FireEvent();
                }
            }
        }

        /// <summary>
        ///     Gets the maximum width of the string.
        /// </summary>
        /// <value>
        ///     The maximum width of the string.
        /// </value>
        public abstract int MaxStringWidth { get; }

        /// <summary>
        ///     Gets the selected value as an object.
        /// </summary>
        /// <value>
        ///     The selected value as an object.
        /// </value>
        public abstract object SelectedValueAsObject { get; }

        /// <summary>
        ///     Gets a list of strings that represent the different options
        /// </summary>
        public abstract string[] ValuesAsStrings { get; }

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
        public MenuList(string name, string displayName, IEnumerable<T> objects, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.Values = objects.ToList();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuList{T}" /> class based upon the given Enumeration type.
        /// </summary>
        public MenuList(string name, string displayName, string uniqueString = "")
            : this(name, displayName, Enum.GetValues(typeof(T)).Cast<T>(), uniqueString)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuList{T}" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected MenuList(SerializationInfo info, StreamingContext context)
        {
            this.Index = (int)info.GetValue("index", typeof(int));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the count.
        /// </summary>
        public override int Count
        {
            get
            {
                return this.Values.Count;
            }
        }

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
                    foreach (var newWidth in
                        this.Values.Select(obj => MenuSettings.Font.MeasureText(null, obj.ToString(), 0).Width)
                            .Where(newWidth => newWidth > this.width))
                    {
                        this.width = newWidth;
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
        public override object SelectedValueAsObject
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
        ///     Gets the values as strings.
        /// </summary>
        public override string[] ValuesAsStrings
        {
            get
            {
                var arr = new string[this.Values.Count];
                for (var i = 0; i < arr.Length; i++)
                {
                    arr[i] = this.Values[i].ToString();
                }

                return arr;
            }
        }

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
        public override void Extract(MenuItem component)
        {
            this.Index = ((MenuList<T>)component).Index;
        }

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        public override void Draw()
        {
            ThemeManager.Current.List.Draw(this);
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public override void WndProc(WindowsKeys args)
        {
            if (!this.Visible)
            {
                return;
            }

            var dropdownRect = ThemeManager.Current.List.DropDownBoundaries(this);
            var entireDropdownRect = ThemeManager.Current.List.DropDownExpandedBoundaries(this);

            if (args.Cursor.IsUnderRectangle(dropdownRect.X, dropdownRect.Y, dropdownRect.Width, dropdownRect.Height))
            {
                this.Hovering = true;

                if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    this.Active = !this.Active;
                }
            }
            else
            {
                this.Hovering = false;
            }

            const int Buffer = 20;
            if (this.Active
                && !args.Cursor.IsUnderRectangle(
                    entireDropdownRect.X - Buffer, 
                    entireDropdownRect.Y - Buffer, 
                    entireDropdownRect.Width + (2 * Buffer), 
                    entireDropdownRect.Height + (2 * Buffer)))
            {
                this.Active = false;
            }

            if (this.Active)
            {
                var found = false;
                var dropdownRectangles = ThemeManager.Current.List.DropDownListBoundaries(this);
                for (var i = 0; i < dropdownRectangles.Count; i++)
                {
                    if (args.Cursor.IsUnderRectangle(
                        dropdownRectangles[i].X, 
                        dropdownRectangles[i].Y, 
                        dropdownRectangles[i].Width, 
                        dropdownRectangles[i].Height))
                    {
                        this.HoveringIndex = i;
                        found = true;
                    }
                }

                if (!found)
                {
                    this.HoveringIndex = -1;
                }
                else if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    this.Index = this.HoveringIndex;
                    args.Process = false;
                }
            }
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("index", this.Index, typeof(int));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("index", this.Index, typeof(int));
        }

        #endregion
    }
}