#region

using System;
using System.Collections.Generic;
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
            new Dictionary<Type, Func<AMenuValue>> { { typeof(MenuBool), () => new MenuBool() } };

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
    }
}