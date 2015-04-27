using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    /// Serialization class used to convert an object to a byte array and vice versa.
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Convert an object to a byte array
        /// </summary>
        /// <param name="obj">Object that will be converted</param>
        /// <returns>Returns the given object as a byte array.</returns>
        public static byte[] Serialize(Object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var bf = new BinaryFormatter();
            var ms = new System.IO.MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        /// Convert a byte array to an Object
        /// </summary>
        /// <param name="arrBytes">Byte array that will be converted</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns the given byte array as an object.</returns>
        public static T Deserialize<T>(byte[] arrBytes)
        {
            var memStream = new System.IO.MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, System.IO.SeekOrigin.Begin);
            return (T)binForm.Deserialize(memStream);
        }

        /// <summary>
        /// Returns the md5 hash from a string.
        /// </summary>
        /// <param name="s">String that will be converted to MD5.</param>
        /// <returns>Returns the md5 hash from a string.</returns>
        public static string Md5Hash(string s)
        {
            var sb = new StringBuilder();
            HashAlgorithm algorithm = MD5.Create();
            var h = algorithm.ComputeHash(Encoding.UTF8.GetBytes(s));

            foreach (var b in h)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
