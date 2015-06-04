// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultValue.cs" company="LeagueSharp">
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
//   Default Value utility, functions to quickly return the value from the <see cref="DefaultValueAttribute" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    ///     Default Value utility, functions to quickly return the value from the <see cref="DefaultValueAttribute" />
    /// </summary>
    public class DefaultValue
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the default value.
        /// </summary>
        /// <param name="type">
        ///     The type
        /// </param>
        /// <param name="field">
        ///     The field name
        /// </param>
        /// <typeparam name="T">
        ///     The return type.
        /// </typeparam>
        /// <returns>
        ///     The default value from the <see cref="DefaultValueAttribute" /> in the given requested type.
        /// </returns>
        public T GetDefaultValue<T>(Type type, string field)
        {
            return
                (T)
                ((DefaultValueAttribute)
                 TypeDescriptor.GetProperties(type)[field].Attributes[typeof(DefaultValueAttribute)]).Value;
        }

        /// <summary>
        ///     Gets the default value.
        /// </summary>
        /// <param name="object">
        ///     The object.
        /// </param>
        /// <param name="field">
        ///     The field name
        /// </param>
        /// <typeparam name="T">
        ///     The return type.
        /// </typeparam>
        /// <returns>
        ///     The default value from the <see cref="DefaultValueAttribute" /> in the given requested type.
        /// </returns>
        public T GetDefaultValue<T>(object @object, string field)
        {
            return
                (T)
                ((DefaultValueAttribute)
                 TypeDescriptor.GetProperties(@object)[field].Attributes[typeof(DefaultValueAttribute)]).Value;
        }

        /// <summary>
        ///     Gets the default value.
        /// </summary>
        /// <param name="object">
        ///     The object.
        /// </param>
        /// <returns>
        ///     The default value from the <see cref="DefaultValueAttribute" /> in the given requested type.
        /// </returns>
        public IDictionary<string, object> GetDefaultValues(object @object)
        {
            return
                TypeDescriptor.GetProperties(@object)
                    .Cast<PropertyDescriptor>()
                    .Where(
                        property =>
                        Attribute.IsDefined(@object.GetType().GetProperty(property.Name), typeof(DefaultValueAttribute)))
                    .ToDictionary(
                        property => property.Name, 
                        property => ((DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)]).Value);
        }

        /// <summary>
        ///     Gets the default value.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The default value from the <see cref="DefaultValueAttribute" /> in the given requested type.
        /// </returns>
        public IDictionary<string, object> GetDefaultValues(Type type)
        {
            return
                TypeDescriptor.GetProperties(type)
                    .Cast<PropertyDescriptor>()
                    .Where(
                        property => Attribute.IsDefined(type.GetProperty(property.Name), typeof(DefaultValueAttribute)))
                    .ToDictionary(
                        property => property.Name, 
                        property => ((DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)]).Value);
        }

        #endregion
    }
}