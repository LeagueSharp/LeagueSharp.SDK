#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.Extensions
{
    /// <summary>
    ///     Enumerable Extensions.
    /// </summary>
    public static class Enumerable
    {
        #region MinOrDefault

        /// <summary>
        ///     Gets the minimun value of an IEnumerable by the comparer, or returns the default.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <typeparam name="TR">Type result of comparer</typeparam>
        /// <param name="container">Container of values to search through</param>
        /// <param name="comparer">Function to compare the values</param>
        /// <returns>The minimum of the objects</returns>
        public static T MinOrDefault<T, TR>(this IEnumerable<T> container, Func<T, TR> comparer) where TR : IComparable
        {
            var enumerator = container.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(T);
            }

            var minElem = enumerator.Current;
            var minVal = comparer(minElem);

            while (enumerator.MoveNext())
            {
                var currVal = comparer(enumerator.Current);

                if (currVal.CompareTo(minVal) >= 0)
                {
                    continue;
                }

                minVal = currVal;
                minElem = enumerator.Current;
            }

            return minElem;
        }

        #endregion

        #region MaxOrDefault

        /// <summary>
        ///     Gets the maximum value of an IEnumerable by the comparer, or returns the default.
        /// </summary>
        /// <param name="container">Container of values to search through</param>
        /// <param name="comparer">Function to compare values</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <typeparam name="TR">Type result of comparer</typeparam>
        /// <returns>The maximums of the objects</returns>
        public static T MaxOrDefault<T, TR>(this IEnumerable<T> container, Func<T, TR> comparer) where TR : IComparable
        {
            var enumerator = container.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(T);
            }

            var maxElem = enumerator.Current;
            var maxVal = comparer(maxElem);

            while (enumerator.MoveNext())
            {
                var currVal = comparer(enumerator.Current);

                if (currVal.CompareTo(maxVal) <= 0)
                {
                    continue;
                }

                maxVal = currVal;
                maxElem = enumerator.Current;
            }

            return maxElem;
        }

        #endregion

        #region Find

        /// <summary>
        ///     Finds match in a container of values.
        /// </summary>
        /// <param name="source">List of all the values</param>
        /// <param name="match">Method that determines whether the specified object meets the criteria</param>
        /// <returns>Matches based on the predicate</returns>
        public static TSource Find<TSource>(this IEnumerable<TSource> source, Predicate<TSource> match)
        {
            return (source as List<TSource> ?? source.ToList()).Find(match);
        }

        #endregion

        #region In

        /// <summary>
        ///     Determines if a list contains any of the values.
        /// </summary>
        /// <param name="source">Container of objects</param>
        /// <param name="list">Any object that should be in the container</param>
        /// <typeparam name="T">Type of object to look for</typeparam>
        /// <returns>If the contianer contains any values.</returns>
        public static bool In<T>(this T source, params T[] list)
        {
            return list.Contains(source);
        }

        #endregion

        #region ForEach

        /// <summary>
        ///     Does an action over a list of types.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="list">List of values</param>
        /// <param name="action">Function to call foreach value</param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        #endregion

        #region To

        /// <summary>
        ///     Converts an item to another Type
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="object">The object to conver to</param>
        /// <returns>The converted object</returns>
        public static T To<T>(this IConvertible @object)
        {
            return (T) Convert.ChangeType(@object, typeof(T));
        }

        #endregion

        #region Logging

        /// <summary>
        ///     Logs an exception to the console and a file.
        /// </summary>
        /// <param name="exception">Exception to log.</param>
        public static void Log(this Exception exception)
        {
            Logging.Write(true)(LogLevel.Error, exception.Message);
        }

        #endregion

        #region GetCombinations

        /*
         from: https://stackoverflow.com/questions/10515449/generate-all-combinations-for-a-list-of-strings :^)
         */

        /// <summary>
        ///     Returns all the subgroup combinations that can be made from a group
        /// </summary>
        /// <param name="allValues">List of <see cref="Vector2" /></param>
        /// <returns>Double list of vectors.</returns>
        public static IEnumerable<List<Vector2>> GetCombinations(this IReadOnlyCollection<Vector2> allValues)
        {
            var collection = new List<List<Vector2>>();

            for (var counter = 0; counter < (1 << allValues.Count); ++counter)
            {
                var combination = allValues.Where((t, i) => (counter & (1 << i)) == 0).ToList();

                collection.Add(combination);
            }

            return collection;
        }

        #endregion GetCombinations

        #region Standard Deviation

        /// <summary>
        ///     Standard Devitation of the values list.
        /// </summary>
        /// <param name="values">Values list</param>
        /// <returns>Standard Devitation</returns>
        public static double StandardDeviation(this IEnumerable<int> values)
        {
            var enumerable = values as int[] ?? values.ToArray();
            var avg = enumerable.Average();
            return System.Math.Sqrt(enumerable.Average(v => System.Math.Pow(v - avg, 2)));
        }

        #endregion
    }
}