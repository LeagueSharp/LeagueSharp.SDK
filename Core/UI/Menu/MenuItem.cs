#region

using LeagueSharp.CommonEx.Core.UI.Abstracts;
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
        protected MenuItem(string name, string displayName) : base(name, displayName) {}

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
        public MenuItem(string name, string displayName) : base(name, displayName)
        {
            MenuFactory.Create<T>();
            Enabled = Visible = true;
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
        ///     Returns if the item is enabled.
        /// </summary>
        public override sealed bool Enabled { get; set; }

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

        /// <summary>
        ///     Item Draw callback.
        /// </summary>
        public override void OnDraw(Vector2 position, int index)
        {
            if (Visible)
            {
                _value.OnDraw(position);
            }
        }

        /// <summary>
        ///     Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" />
        /// </param>
        public override void OnWndProc(WindowsKeys args) {}

        /// <summary>
        ///     Item Update callback.
        /// </summary>
        public override void OnUpdate() {}
    }
}