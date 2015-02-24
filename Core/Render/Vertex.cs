namespace LeagueSharp.CommonEx.Core.Render
{
    /// <summary>
    ///     Custom Render Vertex, Vector4 and Color assignments for D3D9 drawing (DrawPrimitiveUP).
    /// </summary>
    public struct Vertex
    {
        /// <summary>
        ///     RBGA Color value.
        /// </summary>
        public int Color;

        /// <summary>
        ///     4th vector of the Vector4. (W-axis).
        /// </summary>
        public float Rhw;

        /// <summary>
        ///     1st vector of the Vector4. (X-axis).
        /// </summary>
        public float X;

        /// <summary>
        ///     2nd vector of the Vector4. (Y-axis).
        /// </summary>
        public float Y;

        /// <summary>
        ///     3rd vector of the Vector4. (Z-axis).
        /// </summary>
        public float Z;
    }
}