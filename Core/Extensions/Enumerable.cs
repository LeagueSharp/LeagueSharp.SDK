#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LeagueSharp.CommonEx.Core.Enumerations;
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

        /// <summary>
        ///     Logs an exception to the console and a file.
        /// </summary>
        /// <param name="exception">Exception to log.</param>
        public static void Log(this Exception exception)
        {
            Logging.Write(true)(LogLevel.Error, exception.Message);
        }

        #region FlagsAttribute

        /// <summary>
        ///     Sets the given flags to the struct source, given struct must be an enumeration with flag attributes.
        /// </summary>
        /// <typeparam name="T">Enumeration with Flag Attributes (struct)</typeparam>
        /// <param name="value">The enumeration</param>
        /// <param name="flags">The flags to be set</param>
        /// <param name="status">Turn flags on or off</param>
        /// <returns>Enumeration with Flag Attributes (struct)</returns>
        public static T SetFlags<T>(this T value, T flags, bool status = true) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(string.Format("Type '{0}' is not an enum.", typeof(T).FullName));
            }
            if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
            {
                throw new ArgumentException(
                    string.Format("Type '{0}' doesn't have the 'Flags' attribute.", typeof(T).FullName));
            }
            return
                (T)
                    Enum.ToObject(
                        typeof(T),
                        status
                            ? Convert.ToInt64(flags) | Convert.ToInt64(value)
                            : ~Convert.ToInt64(flags) & Convert.ToInt64(value));
        }

        /// <summary>
        ///     Retrieves all of the flags from a specific struct source.
        /// </summary>
        /// <typeparam name="T">Enumeration with Flag Attributes (struct)</typeparam>
        /// <param name="value">The enumeration</param>
        /// <returns>Enumeration with Flag Attributes (struct)</returns>
        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(string.Format("Type '{0}' is not an enum.", typeof(T).FullName));
            }
            if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
            {
                throw new ArgumentException(
                    string.Format("Type '{0}' doesn't have the 'Flags' attribute.", typeof(T).FullName));
            }

            return
                Enum.GetValues(typeof(T)).Cast<T>().Where(flag => (Convert.ToInt64(value) & Convert.ToInt64(flag)) != 0);
        }

        /// <summary>
        ///     Clears all given flags from a specific struct soruce.
        /// </summary>
        /// <typeparam name="T">Enumeration with Flag Attributes (struct)</typeparam>
        /// <param name="value">The enumeration</param>
        /// <param name="flags">Flags to be cleared</param>
        /// <returns>Enumeration with Flag Attributes (struct)</returns>
        public static T ClearFlags<T>(this T value, T flags) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(string.Format("Type '{0}' is not an enum.", typeof(T).FullName));
            }
            if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
            {
                throw new ArgumentException(
                    string.Format("Type '{0}' doesn't have the 'Flags' attribute.", typeof(T).FullName));
            }

            return value.SetFlags(flags, false);
        }

        /// <summary>
        ///     Combines flags from an enumerable list to a new given struct source.
        /// </summary>
        /// <typeparam name="T">Enumeration with Flag Attributes (struct)</typeparam>
        /// <param name="flags">Flags</param>
        /// <returns>Enumeration with Flag Attributes (struct)</returns>
        public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(string.Format("Type '{0}' is not an enum.", typeof(T).FullName));
            }
            if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
            {
                throw new ArgumentException(
                    string.Format("Type '{0}' doesn't have the 'Flags' attribute.", typeof(T).FullName));
            }

            return (T) Enum.ToObject(typeof(T), flags.Aggregate(0L, (current, flag) => current | Convert.ToInt64(flag)));
        }

        /// <summary>
        ///     Gets a flag attribute description.
        /// </summary>
        /// <typeparam name="T">Enumeration with Flag Attributes (struct)</typeparam>
        /// <param name="value">The enumeration</param>
        /// <returns>Enumeration with Flag Attributes (struct)</returns>
        public static string GetFlagDescription<T>(this T value) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(string.Format("Type '{0}' is not an enum.", typeof(T).FullName));
            }
            if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
            {
                throw new ArgumentException(
                    string.Format("Type '{0}' doesn't have the 'Flags' attribute.", typeof(T).FullName));
            }

            var name = Enum.GetName(typeof(T), value);
            if (string.IsNullOrEmpty(name))
            {
                var field = typeof(T).GetField(name);
                if (field != null)
                {
                    var attributeDesc =
                        Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    return attributeDesc != null ? attributeDesc.Description : null;
                }
            }

            return null;
        }

        #endregion

        #region IEnumerable

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