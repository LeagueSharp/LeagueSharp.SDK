namespace LeagueSharp.SDK.Core.Utils.Tween
{
    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     A "library" of commonly used lerp functions
    /// </summary>
    public class LerpFuncs
    {
        #region Public Methods and Operators

        public static byte LerpByte(byte start, byte end, float progress)
        {
            return (byte)(start + (end - start) * progress);
        }

        public static char LerpChar(char start, char end, float progress)
        {
            return (char)(start + (end - start) * progress);
        }

        public static Color LerpColor(Color start, Color end, float progress)
        {
            return Color.FromArgb(
                LerpByte(start.A, end.A, progress),
                LerpByte(start.R, end.R, progress),
                LerpByte(start.G, end.G, progress),
                LerpByte(start.B, end.B, progress));
        }

        public static SharpDX.Color LerpColor(SharpDX.Color start, SharpDX.Color end, float progress)
        {
            return new SharpDX.Color(
                LerpByte(start.A, end.A, progress),
                LerpByte(start.R, end.R, progress),
                LerpByte(start.G, end.G, progress),
                LerpByte(start.B, end.B, progress));
        }

        public static double LerpDouble(double start, double end, float progress)
        {
            return start + (end - start) * progress;
        }

        public static float LerpFloat(float start, float end, float progress)
        {
            return start + (end - start) * progress;
        }

        public static int LerpInt(int start, int end, float progress)
        {
            return (int)(start + (end - start) * progress);
        }

        public static long LerpLong(long start, long end, float progress)
        {
            return (long)(start + (end - start) * progress);
        }

        public static sbyte LerpSByte(sbyte start, sbyte end, float progress)
        {
            return (sbyte)(start + (end - start) * progress);
        }

        public static short LerpShort(short start, short end, float progress)
        {
            return (short)(start + (end - start) * progress);
        }

        public static uint LerpUInt(uint start, uint end, float progress)
        {
            return (uint)(start + (end - start) * progress);
        }

        public static ulong LerpULong(ulong start, ulong end, float progress)
        {
            return (ulong)(start + (end - start) * progress);
        }

        public static ushort LerpUShort(ushort start, ushort end, float progress)
        {
            return (ushort)(start + (end - start) * progress);
        }

        public static Vector2 LerpVector2(Vector2 start, Vector2 end, float progress)
        {
            return new Vector2(LerpFloat(start.X, end.X, progress), LerpFloat(start.Y, end.Y, progress));
        }

        public static Vector3 LerpVector3(Vector3 start, Vector3 end, float progress)
        {
            return new Vector3(
                LerpFloat(start.X, end.X, progress),
                LerpFloat(start.Y, end.Y, progress),
                LerpFloat(start.Z, end.Z, progress));
        }

        public static Vector4 LerpVector4(Vector4 start, Vector4 end, float progress)
        {
            return new Vector4(
                LerpFloat(start.X, end.X, progress),
                LerpFloat(start.Y, end.Y, progress),
                LerpFloat(start.Z, end.Z, progress),
                LerpFloat(start.W, end.W, progress));
        }

        #endregion
    }
}