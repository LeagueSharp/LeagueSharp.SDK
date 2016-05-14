// <copyright file="MenuItem.cs" company="LeagueSharp">
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
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    using LeagueSharp.SDKEx.Utils;

    using SharpDX;

    /// <summary>
    ///     Menu Item
    /// </summary>
    public abstract class MenuItem : AMenuComponent
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuItem" /> class.
        /// </summary>
        internal MenuItem()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuItem" /> class.
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

        #region Delegates

        /// <summary>
        ///     Delegate for <see cref="ValueChanged" />
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">The OnValueChangedEventArgs instance containing the event data.</param>
        public delegate void OnValueChanged(object sender, EventArgs e);

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
                var fileName = this.Name + this.UniqueString + "." + this.GetType().Name + ".bin";
                if (this.Parent == null)
                {
                    return
                        System.IO.Path.Combine(
                            MenuManager.ConfigFolder.CreateSubdirectory(this.AssemblyName).FullName,
                            fileName);
                }

                return System.IO.Path.Combine(this.Parent.Path, fileName);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the settings are loaded.
        /// </summary>
        public bool SettingsLoaded { get; set; }

        /// <summary>
        ///     Returns if the item is toggled.
        /// </summary>
        public override bool Toggled { get; set; }

        /// <summary>
        ///     Returns the item visibility.
        /// </summary>
        public override sealed bool Visible { get; set; }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets the Component Dynamic Object accessibility.
        /// </summary>
        /// <param name="name">
        ///     Child Menu Component name
        /// </param>
        /// <returns>Null, a menu item is unable to hold an access-able sub component</returns>
        public override AMenuComponent this[string name] => null;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        public abstract void Extract(MenuItem component);

        /// <summary>
        ///     Event Handler
        /// </summary>
        public void FireEvent()
        {
            this.Parent?.FireEvent(this);
            this.ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the 1.
        /// </typeparam>
        /// <returns>Returns the value as the given type</returns>
        /// <exception cref="Exception">Cannot cast value  + Value.GetType() +  to  + typeof(T1)</exception>
        public override T GetValue<T>()
        {
            return (T)this;
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>An Exception, there is no child for a MenuItem.</returns>
        /// <exception cref="Exception">Cannot get child of a MenuItem</exception>
        public override T GetValue<T>(string name)
        {
            throw new Exception("Cannot get child of a MenuItem");
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public override void Load()
        {
            if (!this.SettingsLoaded && File.Exists(this.Path) && this.GetType().IsSerializable)
            {
                this.SettingsLoaded = true;
                try
                {
                    var newValue = Deserialize<MenuItem>(File.ReadAllBytes(this.Path));
                    this.Extract(newValue);
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
            if (this.Visible)
            {
                this.Position = position;
                try
                {
                    this.Draw();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
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
            if (!args.Process)
            {
                return;
            }

            this.WndProc(args);
        }

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public override void Save()
        {
            if (this.GetType().IsSerializable)
            {
                File.WriteAllBytes(this.Path, Serialize(this));
            }
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public abstract void WndProc(WindowsKeys args);

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
            return Type.GetType($"{typeName}, {assemblyName}");
        }

        #endregion
    }
}