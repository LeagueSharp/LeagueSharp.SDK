using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.UI.Skins.Default;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    using LeagueSharp.SDK.Core.Enumerations;

    /// <summary>
    ///     A list of values.
    /// </summary>
    [Serializable]
    public abstract class MenuList : AMenuValue
    {
        /// <summary>
        /// Gets or sets a value if the user is hovering over the right arrow.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the user is hovering over the right arrow; otherwise, <c>false</c>.
        /// </value>
        public bool RightArrowHover { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is hovering over the left arrow..
        /// </summary>
        /// <value>
        ///   <c>true</c> ithe user is hovering over the left arrow; otherwise, <c>false</c>.
        /// </value>
        public bool LeftArrowHover { get; protected set; }

        /// <summary>
        /// Gets the selected value as an object.
        /// </summary>
        /// <value>
        /// The selected value as an object.
        /// </value>
        public abstract object SelectedValueAsObject { get; }

        /// <summary>
        /// Gets the maximum width of the string.
        /// </summary>
        /// <value>
        /// The maximum width of the string.
        /// </value>
        public abstract int MaxStringWidth { get; }
    }


    /// <summary>
    ///     A list of values with a specific type.
    /// </summary>
    /// <typeparam name="T">Type of object in the list</typeparam>
    [Serializable]
    public class MenuList<T> : MenuList, ISerializable
    {

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuList{T}"/> class.
        /// </summary>
        /// <param name="objects">The objects.</param>
        public MenuList(IEnumerable<T> objects)
        {
            Values = objects.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuList{T}"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public MenuList(SerializationInfo info, StreamingContext context)
        {
            Index = (int)info.GetValue("index", typeof(int));
        }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public List<T> Values { get; set; }

        /// <summary>
        /// Gets the selected value.
        /// </summary>
        /// <value>
        /// The selected value.
        /// </value>
        public T SelectedValue
        {
            get { return Values[Index]; }
        }

        /// <summary>
        /// Gets the selected value as an object.
        /// </summary>
        /// <value>
        /// The selected value as an object.
        /// </value>
        public override object SelectedValueAsObject
        {
            get { return SelectedValue; }
        }

        private int _width;

        /// <summary>
        /// Gets the maximum width of the string.
        /// </summary>
        /// <value>
        /// The maximum width of the string.
        /// </value>
        public override int MaxStringWidth
        {
            get
            {
                if (_width == 0)
                {
                    foreach (T obj in Values)
                    {
                        int newWidth = DefaultSettings.Font.MeasureText(null, obj.ToString(), 0).Width;
                        if (newWidth > _width)
                        {
                            _width = newWidth;
                        }
                    }
                }
                return _width;
            }
        }

        /// <summary>
        /// Value Width.
        /// </summary>
        public override int Width
        {
            get { return ThemeManager.Current.List.Width(this); }
        }

        /// <summary>
        /// Menu Value Position.
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        /// Drawing callback.
        /// </summary>
        /// <param name="component">Parent Component</param>
        /// <param name="position">Position</param>
        /// <param name="index">Item Index</param>
        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            Position = position;

            ThemeManager.Current.List.OnDraw(component, position, index);
        }

        /// <summary>
        /// Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public override void OnWndProc(WindowsKeys args)
        {
            Rectangle rightArrowRect = ThemeManager.Current.List.RightArrow(Position, Container, this);
            Rectangle leftArrowRect = ThemeManager.Current.List.LeftArrow(Position, Container, this);
            if (args.Cursor.IsUnderRectangle(rightArrowRect.X, rightArrowRect.Y, rightArrowRect.Width, rightArrowRect.Height))
            {
                RightArrowHover = true;
                if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    Index = (Index + 1) % Values.Count;
                    Container.FireEvent();
                }
            }
            else
            {
                RightArrowHover = false;
            }

            if (args.Cursor.IsUnderRectangle(leftArrowRect.X, leftArrowRect.Y, leftArrowRect.Width, leftArrowRect.Height))
            {
                LeftArrowHover = true;
                if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    if (Index == 0)
                    {
                        Index = Values.Count - 1;
                    }
                    else
                    {
                        Index = (Index - 1) % Values.Count;
                        Container.FireEvent();
                    }
                }
            }
            else
            {
                LeftArrowHover = false;
            }
        }


        /// <summary>
        /// Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public override void Extract(AMenuValue component)
        {
            Index =((MenuList<T>) component).Index;
        }



        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("index", Index, typeof(int));
        }
    }
}