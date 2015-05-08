#region

using System;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Abstracts
{
    /// <summary>
    ///     Abstract build of a Menu Value.
    /// </summary>
    [Serializable]
    public abstract class AMenuValue
    {
        public delegate void OnValueChange(ValueChangeArgs args);

        /// <summary>
        ///     Value Container.
        /// </summary>
        public AMenuComponent Container { get; set; }

        /// <summary>
        ///     Value Width.
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        ///     Menu Value Position.
        /// </summary>
        public abstract Vector2 Position { get; set; }

        public event OnValueChange ValueChanged;

        /// <summary>
        ///     Event Handler
        /// </summary>
        /// <param name="_new"></param>
        /// <param name="old"></param>
        public void FireEvent(object _new, object old)
        {
            if (ValueChanged != null)
            {
                ValueChanged(new ValueChangeArgs(_new, old));
            }
        }

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        /// <param name="component">Parent Component</param>
        /// <param name="position">Position</param>
        /// <param name="index">Item Index</param>
        public abstract void OnDraw(AMenuComponent component, Vector2 position, int index);

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public abstract void OnWndProc(WindowsKeys args);

        public abstract void Extract(AMenuValue component);

        public class ValueChangeArgs : EventArgs
        {
            private readonly object _new, old;

            public ValueChangeArgs(object _new, object old)
            {
                this._new = _new;
                this.old = old;
            }

            public object GetNewValue()
            {
                return _new;
            }

            public object GetOldValue()
            {
                return old;
            }
        }
    }
}