#region

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Values;

#endregion

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu Value Factory.
    /// </summary>
    public class MenuFactory
    {
        /// <summary>
        ///     Default Menu Values.
        /// </summary>
        private static readonly IDictionary<Type, Func<AMenuValue>> DefaultMenuValueByType =
            new Dictionary<Type, Func<AMenuValue>>
            {
                { typeof(MenuBool), () => new MenuBool() },
                { typeof(MenuKeyBind), () => new MenuKeyBind() },
                { typeof(MenuSlider), () => new MenuSlider() },
                { typeof(MenuInputText), () => new MenuInputText() }
            };

        /// <summary>
        ///     Creates a new value out of the given type.
        /// </summary>
        /// <typeparam name="T"><see cref="AMenuValue"/></typeparam>
        /// <returns>Created given type</returns>
        public static T Create<T>() where T : AMenuValue
        {
            if (DefaultMenuValueByType.ContainsKey(typeof(T)))
            {
                return (T) DefaultMenuValueByType[typeof(T)].Invoke();
            }
            return default(T);
        }

        /// <summary>
        ///     Stores the new factory values into the menu factory container.
        /// </summary>
        /// <typeparam name="T"><see cref="AMenuValue"/> to contain into the factory value container</typeparam>
        /// <param name="value">Standard build for the constructor</param>
        /// <returns></returns>
        public static bool CreateFactory<T>(AMenuValue value) where T : AMenuValue
        {
            if (!DefaultMenuValueByType.ContainsKey(typeof(T)))
            {
                DefaultMenuValueByType.Add(typeof(T), () => value);
                return true;
            }
            return false;
        }
    }
}