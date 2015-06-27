namespace LeagueSharp.SDK.Core.Utils.Tween
{
    using System;

    /// <summary>
    ///     A "library" of commonly used scale functions
    /// </summary>
    public class ScaleFuncs
    {
        #region Constants

        private const float HalfPi = Pi / 2f;

        private const float Pi = (float)Math.PI;

        #endregion

        #region Public Methods and Operators

        public static float CubicIn(float progress)
        {
            return EaseInPower(progress, 2);
        }

        public static float CubicInOut(float progress)
        {
            return EaseInOutPower(progress, 2);
        }

        public static float CubicOut(float progress)
        {
            return EaseOutPower(progress, 2);
        }

        public static float Linear(float progress)
        {
            return progress;
        }

        public static float QuadraticIn(float progress)
        {
            return EaseInPower(progress, 2);
        }

        public static float QuadraticInOut(float progress)
        {
            return EaseInOutPower(progress, 2);
        }

        public static float QuadraticOut(float progress)
        {
            return EaseOutPower(progress, 2);
        }

        public static float QuarticIn(float progress)
        {
            return EaseInPower(progress, 2);
        }

        public static float QuarticInOut(float progress)
        {
            return EaseInOutPower(progress, 2);
        }

        public static float QuarticOut(float progress)
        {
            return EaseOutPower(progress, 2);
        }

        public static float QuinticIn(float progress)
        {
            return EaseInPower(progress, 2);
        }

        public static float QuinticInOut(float progress)
        {
            return EaseInOutPower(progress, 2);
        }

        public static float QuinticOut(float progress)
        {
            return EaseOutPower(progress, 2);
        }

        public static float SineIn(float progress)
        {
            return (float)Math.Sin(progress * HalfPi - HalfPi) + 1f;
        }

        public static float SineInOut(float progress)
        {
            return (float)(Math.Sin(progress * Pi - HalfPi) + 1f) / 2f;
        }

        public static float SineOut(float progress)
        {
            return (float)Math.Sin(progress * HalfPi);
        }

        #endregion

        #region Methods

        private static float EaseInOutPower(float progress, int power)
        {
            progress *= 2f;

            if (progress >= 1f)
            {
                return (float)Math.Pow(progress, power) / 2f;
            }

            var sign = power % 2 == 0 ? -1 : 1;
            return (float)(sign / 2f * (Math.Pow(progress - 2f, power) + sign * 2f));
        }

        private static float EaseInPower(float progress, int power)
        {
            // I've switched ease in's and ease out's sources to better match how objects
            // should act upon tweening: going out slow to fast, and coming in fast to slow.
            var sign = power % 2 == 0 ? -1 : 1;
            return (float)(sign * (Math.Pow(progress - 1f, power) + sign));
        }

        private static float EaseOutPower(float progress, int power)
        {
            // See ease in.
            return (float)Math.Pow(progress, power);
        }

        #endregion
    }
}