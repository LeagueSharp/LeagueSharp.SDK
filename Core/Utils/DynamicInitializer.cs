// <copyright file="DynamicInitializer.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Utils
{
    using System;
    using System.Reflection.Emit;

    using LeagueSharp.SDK.Enumerations;

    /// <summary>
    ///     Dynamic instantiation from classes
    /// </summary>
    public class DynamicInitializer
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The new instance.
        /// </summary>
        /// <typeparam name="TV">The type of the v.</typeparam>
        /// <returns></returns>
        public static TV NewInstance<TV>() where TV : class
        {
            return ObjectGenerator(typeof(TV)) as TV;
        }

        /// <summary>
        ///     The new instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object NewInstance(Type type)
        {
            return ObjectGenerator(type);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Objects the generator.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static object ObjectGenerator(Type type)
        {
            try
            {
                var target = type.GetConstructor(Type.EmptyTypes);
                if (target?.DeclaringType != null)
                {
                    var dynamic = new DynamicMethod(string.Empty, type, new Type[0], target.DeclaringType);
                    var il = dynamic.GetILGenerator();
                    if (target.DeclaringType != null)
                    {
                        il.DeclareLocal(target.DeclaringType);
                        il.Emit(OpCodes.Newobj, target);
                        il.Emit(OpCodes.Stloc_0);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ret);

                        var method = (Func<object>)dynamic.CreateDelegate(typeof(Func<object>));
                        return method();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Write()(LogLevel.Error, ex);
            }
            return null;
        }

        #endregion
    }
}