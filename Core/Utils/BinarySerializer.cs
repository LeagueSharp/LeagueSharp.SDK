// <copyright file="BinarySerializer.cs" company="LeagueSharp">
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
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    ///     The binary serializer.
    /// </summary>
    public class BinarySerializer
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Deserializes a binary file to the given type as object.
        /// </summary>
        /// <param name="data">
        ///     The binary data
        /// </param>
        /// <typeparam name="T">
        ///     The type to deserialize the object to
        /// </typeparam>
        /// <returns>
        ///     The deserialized object instance.
        /// </returns>
        public static T Deserialize<T>(byte[] data)
        {
            using (
                var reader = XmlDictionaryReader.CreateBinaryReader(
                    new MemoryStream(data),
                    XmlDictionaryReaderQuotas.Max))
            {
                return (T)new DataContractSerializer(typeof(T)).ReadObject(reader);
            }
        }

        /// <summary>
        ///     Serializes the object to a binary file.
        /// </summary>
        /// <param name="obj">
        ///     The object
        /// </param>
        /// <typeparam name="T">
        ///     The type of the object
        /// </typeparam>
        /// <returns>
        ///     The serialized object binary data.
        /// </returns>
        public static byte[] Serialize<T>(T obj)
        {
            var stream = new MemoryStream();
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
            {
                new DataContractSerializer(typeof(T)).WriteObject(writer, obj);
            }

            return stream.ToArray();
        }

        #endregion
    }
}