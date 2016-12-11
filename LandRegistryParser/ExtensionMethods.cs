using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LandRegistryParser
{
    public static class ExtensionMethods
    {
        public static bool StartsWithUpper(this string s)
        {
            return s.Substring(0, 1) == s.Substring(0, 1).ToUpper(CultureInfo.InvariantCulture);
        }

        public static bool StartsWithLower(this string s)
        {
            return s.Substring(0, 1) == s.Substring(0, 1).ToLower(CultureInfo.InvariantCulture);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var elem in enumerable)
            {
                action.Invoke(elem);
            }
        }

        public static IEnumerable<T[]> Buffer<T,U,V>(this IEnumerable<T> sourceStream, IEnumerable<U> delimeterStream, 
            Func<T,V> sourceMapper, Func<U,V> delimeterMapper) where V : IComparable<V>
        {
            var dels = delimeterStream.Zip(delimeterStream.Skip(1), (d1, d2) => new { From = d1, Till = d2, IsLast = false });

            var before = Enumerable.Repeat(sourceStream
                .Where(s => sourceMapper(s).CompareTo(delimeterMapper(dels.First().From)) < 0)
                .ToArray(), 1);

            var within = dels.Select(d => sourceStream.Where(s => sourceMapper(s).CompareTo(delimeterMapper(d.From)) >= 0 
                                                                && sourceMapper(s).CompareTo(delimeterMapper(d.Till)) < 0).ToArray());

            var after = Enumerable.Repeat(sourceStream
                .Where(s => sourceMapper(s).CompareTo(delimeterMapper(dels.Last().Till)) >= 0)
                .ToArray(), 1);

            return before.Union(within).Union(after);
        }
    }
}
