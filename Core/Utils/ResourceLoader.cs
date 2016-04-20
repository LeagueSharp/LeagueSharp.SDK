// <copyright file="ResourceLoader.cs" company="LeagueSharp">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp.SDK.Enumerations;

    internal static class ResourceLoader
    {
        #region Public Methods and Operators

        public static void Initialize()
        {
            var importClasses =
                typeof(ResourceLoader).Assembly.DefinedTypes.Where(
                    t => t.IsClass && t.IsDefined(typeof(ResourceImportAttribute), false));

            foreach (var c in importClasses)
            {
                foreach (var member in GetFieldsAndProperties(c))
                {
                    try
                    {
                        var valueType = member.GetMemberType();
                        var import = member.GetCustomAttribute<ResourceImportAttribute>();
                        var value = JsonFactory.JsonResource(import.File, valueType);

                        if (import.Filter != null)
                        {
                            if (
                                !import.Filter.GetInterfaces()
                                     .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IFilter<>)))
                            {
                                throw new Exception($"{nameof(import.Filter)} does not implement {nameof(IFilter)}");
                            }

                            var filterInstance = Activator.CreateInstance(import.Filter);
                            var apply = import.Filter.GetMethod("Apply", BindingFlags.Public | BindingFlags.Instance);

                            value = apply.Invoke(filterInstance, new[] { value });
                        }

                        member.SetValue(null, value);
                    }
                    catch (Exception e)
                    {
                        Logging.Write()(LogLevel.Error, e);
                    }
                }
            }
        }

        #endregion

        #region Methods

        private static IEnumerable<MemberInfo> GetFieldsAndProperties(
            Type type,
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        {
            return type.GetMembers(flags).Where(m => m.IsDefined(typeof(ResourceImportAttribute), false));
        }

        private static Type GetMemberType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;

                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;

                default:
                    throw new ArgumentException(
                        $"{nameof(MemberInfo)} must be if type {nameof(FieldInfo)} or {nameof(PropertyInfo)}",
                        nameof(member));
            }
        }

        private static void SetValue(this MemberInfo member, object target, object value)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)member).SetValue(target, value);
                    break;

                case MemberTypes.Property:
                    ((PropertyInfo)member).SetValue(target, value, null);
                    break;

                default:
                    throw new ArgumentException(
                        $"{nameof(MemberInfo)} must be if type {nameof(FieldInfo)} or {nameof(PropertyInfo)}",
                        nameof(member));
            }
        }

        #endregion
    }
}