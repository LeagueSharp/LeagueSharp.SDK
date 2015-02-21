#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LeagueSharp.CommonEx.Core.Extensions
{
    /// <summary>
    ///     Enumerable Extensions.
    /// </summary>
    public static class Enumerable
    {
        #region Find

        /// <summary>
        ///     Searches for an element that matches the conditions defined by the specified predicate, and returns the first
        ///     occurrence within the entire IEnumerable.
        /// </summary>
        public static TSource Find<TSource>(this IEnumerable<TSource> source, Predicate<TSource> match)
        {
            return (source as List<TSource> ?? source.ToList()).Find(match);
        }

        /// <summary>
        ///     Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        public static List<TSource> FindAll<TSource>(this IEnumerable<TSource> source, Predicate<TSource> match)
        {
            return (source as List<TSource> ?? source.ToList()).FindAll(match);
        }

        #endregion
    }
}