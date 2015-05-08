using System;
using LeagueSharp.CommonEx.Core.UI.Abstracts;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Arguements for the OnValueChanged event.
    /// </summary>
    public class OnValueChangedEventArgs : EventArgs
    {
        private readonly object _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnValueChangedEventArgs"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public OnValueChangedEventArgs(object value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>() where T : AMenuValue
        {
            return (T) _value;
        }
    }
}