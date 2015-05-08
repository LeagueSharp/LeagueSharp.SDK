using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.UI.Skins.Default;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    [Serializable]
    public abstract class MenuList : AMenuValue
    {
        public bool RightArrowHover { get; protected set; }

        public bool LeftArrowHover { get; protected set; }
        
        public abstract object SelectedValueAsObject { get; }

        public abstract int MaxStringWidth { get; }
    }


    [Serializable]
    public class MenuList<T> : MenuList, ISerializable
    {

        public int Index { get; set; }

        public MenuList(IEnumerable<T> objects)
        {
            Values = objects.ToList();
        }

        public MenuList(SerializationInfo info, StreamingContext context)
        {
            Index = (int)info.GetValue("index", typeof(int));
        }

        public List<T> Values { get; set; }

        public T SelectedValue
        {
            get { return Values[Index]; }
        }

        public override object SelectedValueAsObject
        {
            get { return SelectedValue; }
        }

        private int _width;

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

        public override int Width
        {
            get { return ThemeManager.Current.List.Width(this); }
        }

        public override Vector2 Position { get; set; }

        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            Position = position;

            ThemeManager.Current.List.OnDraw(component, position, index);
        }

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
                    }
                }
            }
            else
            {
                LeftArrowHover = false;
            }
        }


        public override void Extract(AMenuValue component)
        {
            Index =((MenuList<T>) component).Index;
        }



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("index", Index, typeof(int));
        }
    }
}