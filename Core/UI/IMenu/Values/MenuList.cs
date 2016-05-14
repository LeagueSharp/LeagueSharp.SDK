// <copyright file="MenuList.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDKEx.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using LeagueSharp.SDKEx.UI.Skins;
    using LeagueSharp.SDKEx.Utils;

    /// <summary>
    ///     A list of values.
    /// </summary>
    [Serializable]
    public abstract class MenuList : MenuItem
    {
        #region Fields

        /// <summary>
        ///     The index.
        /// </summary>
        protected int ListIndex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuList" /> class.
        /// </summary>
        internal MenuList()
        {
        }

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
                return this.ListIndex;
            }

            set
            {
                int newValue;
                if (value < 0)
                {
                    newValue = 0;
                }
                else if (value >= this.Count)
                {
                    newValue = this.Count - 1;
                }
                else
                {
                    newValue = value;
                }

                if (newValue != this.ListIndex)
                {
                    this.ListIndex = newValue;
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

        #region Methods

        /// <summary>
        ///     Builds an <see cref="ADrawable" /> for this component.
        /// </summary>
        /// <param name="theme">
        ///     The theme.
        /// </param>
        /// <returns>
        ///     The <see cref="ADrawable" /> instance.
        /// </returns>
        protected override ADrawable BuildHandler(ITheme theme)
        {
            return theme.BuildListHandler(this);
        }

        #endregion
    }

    /// <summary>
    ///     A list of values with a specific type.
    /// </summary>
    /// <typeparam name="T">
    ///     Type of object in the list
    /// </typeparam>
    [Serializable]
    public class MenuList<T> : MenuList, ISerializable
    {
        #region Fields

        private T[] values;

        /// <summary>
        ///     Local width.
        /// </summary>
        private int width;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuList{T}" /> class.
        /// </summary>
        /// <param name="name">
        ///     The internal name of this component
        /// </param>
        /// <param name="displayName">
        ///     The display name of this component
        /// </param>
        /// <param name="objects">
        ///     The objects.
        /// </param>
        /// <param name="uniqueString">
        ///     String used in saving settings
        /// </param>
        public MenuList(string name, string displayName, IEnumerable<T> objects, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.values = objects.ToArray();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuList{T}" /> class based upon the given Enumeration type.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="displayName">
        ///     The display Name.
        /// </param>
        /// <param name="uniqueString">
        ///     The unique String.
        /// </param>
        public MenuList(string name, string displayName, string uniqueString = "")
            : this(name, displayName, Enum.GetValues(typeof(T)).Cast<T>(), uniqueString)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuList{T}" /> class.
        /// </summary>
        /// <param name="info">
        ///     The information.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected MenuList(SerializationInfo info, StreamingContext context)
        {
            this.ListIndex = (int)info.GetValue("index", typeof(int));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the count.
        /// </summary>
        public override int Count => this.Values.Length;

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

            set
            {
                for (var i = 0; i < this.Values.Length; i++)
                {
                    if (this.Values[i].Equals(value))
                    {
                        this.Index = i;
                        return;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the selected value as an object.
        /// </summary>
        /// <value>
        ///     The selected value as an object.
        /// </value>
        public override object SelectedValueAsObject => this.SelectedValue;

        /// <summary>
        ///     Gets the values.
        /// </summary>
        /// <value>
        ///     The values.
        /// </value>
        public T[] Values
        {
            get
            {
                return this.values;
            }
            set
            {
                this.values = value;
                if (this.Index >= this.values.Length)
                {
                    this.Index = this.values.Length - 1;
                }
            }
        }

        /// <summary>
        ///     Gets the values as strings.
        /// </summary>
        public override string[] ValuesAsStrings
        {
            get
            {
                return this.Values.Select(arg => arg.ToString()).ToArray();
            }
        }

        /// <summary>
        ///     Value Width.
        /// </summary>
        public override int Width => this.Handler.Width();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        public override void Draw()
        {
            this.Handler.Draw();
        }

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        public override void Extract(MenuItem component)
        {
            this.Index = ((MenuList<T>)component).Index;
        }

        /// <summary>
        ///     Resets the MenuItem back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            // Do nothing.
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        public override void WndProc(WindowsKeys args)
        {
            this.Handler.OnWndProc(args);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.
        /// </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">
        ///     The caller does not have the required permission. =
        /// </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("index", this.Index, typeof(int));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.
        /// </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">
        ///     The caller does not have the required permission. =
        /// </exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("index", this.Index, typeof(int));
        }

        #endregion
    }
}