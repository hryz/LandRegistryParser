using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LandRegistryParser.Models;

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

        public static List<List<T1>> Buffer<T1, T2, T3>(this IEnumerable<T1> sourceStream, List<T2> delimeterStream, 
            Func<T1, T3> mapper, Func<T2, T3> mapperDel ) where T3 : IComparable<T3>
        {
            var result = new List<List<T1>>();
            
            var i = 0;
            var current = new List<T1>();
            var includeTheRest = false;
            foreach (var src in sourceStream)
            {
                var val = mapper(src);
                if (val.CompareTo(mapperDel(delimeterStream[i])) < 0 || includeTheRest)
                {
                    current.Add(src);
                }
                else
                {
                    result.Add(current);
                    current = new List<T1> {src};
                    if (i == delimeterStream.Count - 1)
                    {
                        includeTheRest = true;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            if(includeTheRest)
                result.Add(current);

            return result;
        }

        public static bool IsAlmostEqualTo(this decimal a, decimal b, decimal maxDifference = 10M)
        {
            return Math.Abs(a - b) < maxDifference;
        }
    }
}
