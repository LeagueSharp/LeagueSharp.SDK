using System.IO;

namespace LeagueSharp.CommonEx.Core.Extensions
{
    /// <summary>
    ///     Provides extensions to <see cref="System.IO.Stream" />
    /// </summary>
    public static class Streams
    {
        /// <summary>
        ///     Gets all of the bytes of the stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>All of the bytes of the stream.</returns>
        public static byte[] GetAllBytes(this Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}