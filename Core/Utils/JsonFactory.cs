// <copyright file="JsonFactory.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Core.Utils
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class JsonFactory
    {
        static JsonFactory()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static T JsonResource<T>(string file, Assembly assembly = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return JsonConvert.DeserializeObject<T>(ResourceFactory.StringResource(file, assembly));
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static object JsonResource(string file, Type type = null, Assembly assembly = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return JsonConvert.DeserializeObject(ResourceFactory.StringResource(file, assembly), type);
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static T JsonFile<T>(string file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static object JsonFile(string file, Type type = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return JsonConvert.DeserializeObject(File.ReadAllText(file), type);
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static T JsonString<T>(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return JsonConvert.DeserializeObject<T>(s);
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static object JsonString(string s, Type type = null)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return JsonConvert.DeserializeObject(s, type);
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static void ToFile(string file, object obj)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            File.WriteAllText(file, JsonConvert.SerializeObject(obj, Formatting.Indented));
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static string ToString(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}