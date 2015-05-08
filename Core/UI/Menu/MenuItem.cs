#region

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Abstract build of a Menu Item.
    /// </summary>
    public abstract class MenuItem : AMenuComponent
    {
        /// <summary>
        ///     Menu Item Constructor
        /// </summary>
        /// <param name="name">Item Name</param>
        /// <param name="displayName">Item Display Name</param>
        /// <param name="uniqueString">Unique string</param>
        protected MenuItem(string name, string displayName, string uniqueString = "")
            : base(name, displayName, uniqueString) {}

        /// <summary>
        ///     Returns the item value as a generic object.
        /// </summary>
        public abstract object ValueAsObject { get; }
    }


    /// <summary>
    ///     Menu Item
    /// </summary>
    /// <typeparam name="T">
    ///     <see cref="AMenuValue" />
    /// </typeparam>
    public class MenuItem<T> : MenuItem where T : AMenuValue
    {
        /// <summary>
        ///     Local Value of the MenuItem Type.
        /// </summary>
        private T _value;

        /// <summary>
        ///     Menu Item Constructor
        /// </summary>
        /// <param name="name">Item Name</param>
        /// <param name="displayName">Item Display Name</param>
        /// <param name="uniqueString">Unique string</param>
        public MenuItem(string name, string displayName, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            Value = MenuFactory.Create<T>();
        }


        /// <summary>
        ///     Value Container.
        /// </summary>
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _value.Container = this;
            }
        }

        /// <summary>
        ///     Component Dynamic Object accessability.
        /// </summary>
        /// <param name="name">Child Menu Component name</param>
        /// <returns>Null, a menu item is unable to hold an accessable sub component</returns>
        public override AMenuComponent this[string name]
        {
            get { return null; }
        }

        /// <summary>
        ///     Returns the item visibility.
        /// </summary>
        public override sealed bool Visible { get; set; }

        /// <summary>
        ///     Returns if the item is toggled.
        /// </summary>
        public override bool Toggled { get; set; }

        /// <summary>
        ///     Item Position
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        ///     Returns the item value as a generic object.
        /// </summary>
        public override object ValueAsObject
        {
            get { return Value; }
        }

        public override int Width
        {
            get { return ThemeManager.Current.CalcWidthItem(this) + Value.Width; }
        }

        public override string Path
        {
            get
            {
                if (Parent == null)
                {
                    return System.IO.Path.Combine(
                        MenuManager.ConfigFolder.CreateSubdirectory(AssemblyName).FullName, Name + UniqueString + ".bin");
                }
                return System.IO.Path.Combine(Parent.Path, Name + UniqueString + ".bin");
            }
        }

        /// <summary>
        ///     Item Draw callback.
        /// </summary>
        public override void OnDraw(Vector2 position, int index)
        {
            if (_value == null)
            {
                Logging.Write()(LogLevel.Error, "Attempting to draw a null value item. [Item Name: {0}]", Name);
                return;
            }

            if (Visible)
            {
                _value.OnDraw(this, position, index);
            }
        }

        /// <summary>
        ///     Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" />
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (_value == null)
            {
                Logging.Write()(
                    LogLevel.Error,
                    "Attempting to pass a windows process message to a null value item. [Item Name: {0}]", Name);
                return;
            }

            if (Visible)
            {
                _value.OnWndProc(args);
            }
        }

        /// <summary>
        ///     Item Update callback.
        /// </summary>
        public override void OnUpdate() {}

        public override T1 GetValue<T1>()
        {
            var val = Value as T1;
            if (val != null)
            {
                return val;
            }
            throw new Exception("Cannot cast value " + Value.GetType() + " to " + typeof(T1));
        }

        public override T2 GetValue<T2>(string name)
        {
            throw new Exception("Cannot get child of a MenuItem");
        }


        public override void Save()
        {
            if (Value != null && Value.GetType().IsSerializable)
            {
                File.WriteAllBytes(Path, Serialize(Value));
            }
        }

        internal static byte[] Serialize(Object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        // Convert a byte array to an Object
        internal static T3 Deserialize<T3>(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (T3) binForm.Deserialize(memStream);
        }

        public override void Load()
        {
            if (File.Exists(Path))
            {
                var newValue = Deserialize<T>(File.ReadAllBytes(Path));
                if (Value != null)
                {
                    Value.Extract(newValue);
                }
                else
                {
                    Value = newValue;
                }
            }
        }
    }
}