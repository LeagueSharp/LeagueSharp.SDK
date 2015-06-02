// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuItem.cs" company="LeagueSharp">
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
//   Abstract build of a Menu Item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Skins;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Abstract build of a Menu Item.
    /// </summary>
    public abstract class MenuItem : AMenuComponent
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuItem" /> class.
        ///     Menu Item Constructor
        /// </summary>
        /// <param name="name">
        ///     Item Name
        /// </param>
        /// <param name="displayName">
        ///     Item Display Name
        /// </param>
        /// <param name="uniqueString">
        ///     Unique string
        /// </param>
        protected MenuItem(string name, string displayName, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the item value as a generic object.
        /// </summary>
        public abstract object ValueAsObject { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Event handler
        /// </summary>
        public abstract void FireEvent();

        #endregion
    }

    /// <summary>
    ///     Menu Item
    /// </summary>
    /// <typeparam name="T">
    ///     <see cref="AMenuValue" /> type
    /// </typeparam>
    public class MenuItem<T> : MenuItem
        where T : AMenuValue
    {
        #region Fields

        /// <summary>
        ///     Local Value of the MenuItem Type.
        /// </summary>
        private T value;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuItem{T}" /> class.
        ///     Menu Item Constructor
        /// </summary>
        /// <param name="name">
        ///     Item Name
        /// </param>
        /// <param name="displayName">
        ///     Item Display Name
        /// </param>
        /// <param name="uniqueString">
        ///     Unique string
        /// </param>
        public MenuItem(string name, string displayName, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.Value = MenuFactory.Create<T>();
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Delegate for <see cref="ValueChanged" />
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The OnValueChangedEventArgs instance containing the event data.</param>
        public delegate void OnValueChanged(object sender, ValueChangedEventArgs<T> args);

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when a value is changed.
        /// </summary>
        public event OnValueChanged ValueChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the path.
        /// </summary>
        /// <value>
        ///     The path.
        /// </value>
        public override string Path
        {
            get
            {
                if (this.Parent == null)
                {
                    return
                        System.IO.Path.Combine(
                            MenuManager.ConfigFolder.CreateSubdirectory(this.AssemblyName).FullName, 
                            this.Name + this.UniqueString + ".bin");
                }

                return System.IO.Path.Combine(this.Parent.Path, this.Name + this.UniqueString + ".bin");
            }
        }

        /// <summary>
        ///     Item Position
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the settings are loaded.
        /// </summary>
        public bool SettingsLoaded { get; set; }

        /// <summary>
        ///     Returns if the item is toggled.
        /// </summary>
        public override bool Toggled { get; set; }

        /// <summary>
        ///     Gets or sets the Value Container.
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                if (this.value != null)
                {
                    this.value.Container = this;
                }
            }
        }

        /// <summary>
        ///     Returns the item value as a generic object.
        /// </summary>
        public override object ValueAsObject
        {
            get
            {
                return this.Value;
            }
        }

        /// <summary>
        ///     Returns the item visibility.
        /// </summary>
        public override sealed bool Visible { get; set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public override int Width
        {
            get
            {
                return ThemeManager.Current.CalcWidthItem(this) + this.Value.Width;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets the Component Dynamic Object accessibility.
        /// </summary>
        /// <param name="name">Child Menu Component name</param>
        /// <returns>Null, a menu item is unable to hold an access-able sub component</returns>
        public override AMenuComponent this[string name]
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Event Handler
        /// </summary>
        public override void FireEvent()
        {
            if (this.Parent != null)
            {
                this.Parent.FireEvent(this);
            }

            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new ValueChangedEventArgs<T>(this.Value));
            }
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <returns>Returns the value as the given type</returns>
        /// <exception cref="Exception">Cannot cast value  + Value.GetType() +  to  + typeof(T1)</exception>
        public override T1 GetValue<T1>()
        {
            var val = this.Value as T1;
            if (val != null)
            {
                return val;
            }

            throw new Exception("Cannot cast value " + this.Value.GetType() + " to " + typeof(T1));
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>An Exception, there is no child for a MenuItem.</returns>
        /// <exception cref="Exception">Cannot get child of a MenuItem</exception>
        public override T2 GetValue<T2>(string name)
        {
            throw new Exception("Cannot get child of a MenuItem");
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public override void Load()
        {
            if (!this.SettingsLoaded && File.Exists(this.Path) && typeof(T).IsSerializable)
            {
                this.SettingsLoaded = true;
                try
                {
                    var newValue = Deserialize<T>(File.ReadAllBytes(this.Path));
                    if (this.Value != null)
                    {
                        this.Value.Extract(newValue);
                    }
                    else
                    {
                        this.Value = newValue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        ///     Item Draw callback.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        public override void OnDraw(Vector2 position)
        {
            if (this.value == null)
            {
                Logging.Write()(LogLevel.Error, "Attempting to draw a null value item. [Item Name: {0}]", this.Name);
                return;
            }

            if (this.Visible)
            {
                this.Position = position;
                this.value.OnDraw();
            }
        }

        /// <summary>
        ///     Item Update callback.
        /// </summary>
        public override void OnUpdate()
        {
        }

        /// <summary>
        ///     Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (this.value == null || !args.Process)
            {
                return;
            }

            this.value.OnWndProc(args);
        }

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public override void Save()
        {
            if (this.Value != null && this.Value.GetType().IsSerializable)
            {
                File.WriteAllBytes(this.Path, Serialize(this.Value));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Convert a byte array to an Object.
        /// </summary>
        /// <param name="arrBytes">
        ///     Byte array
        /// </param>
        /// <typeparam name="T3">
        ///     Object casting type
        /// </typeparam>
        /// <returns>
        ///     Object from the byte array as given type.
        /// </returns>
        internal static T3 Deserialize<T3>(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter { Binder = new AllowAllAssemblyVersionsDeserializationBinder() };
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (T3)binForm.Deserialize(memStream);
        }

        /// <summary>
        ///     Convert an Object to a byte array.
        /// </summary>
        /// <param name="obj">
        ///     The Object
        /// </param>
        /// <returns>
        ///     Byte array from the given Object.
        /// </returns>
        internal static byte[] Serialize(object obj)
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

        #endregion
    }

    /// <summary>
    ///     Allow all assembly versions deserialization binder.
    /// </summary>
    internal sealed class AllowAllAssemblyVersionsDeserializationBinder : SerializationBinder
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The bind to type.
        /// </summary>
        /// <param name="assemblyName">
        ///     The assembly name
        /// </param>
        /// <param name="typeName">
        ///     The type name
        /// </param>
        /// <returns>
        ///     The type which has been bind.
        /// </returns>
        [SuppressMessage("ReSharper", "RedundantAssignment", Justification = "Overriden for memory purposes.")]
        public override Type BindToType(string assemblyName, string typeName)
        {
            // In this case we are always using the current assembly
            assemblyName = Assembly.GetExecutingAssembly().FullName;

            // Get the type using the typeName and assemblyName
            return Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
        }

        #endregion
    }
}