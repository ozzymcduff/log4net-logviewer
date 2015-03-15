using System;
using System.Collections.Generic;

namespace LogViewer.Infrastructure
{
    internal static class Extensions
    {
        private static bool ReturnsTrue<T>(T element) { return true; }
        public static T Next<T>(this IList<T> that, int index, Func<T, bool> accept = null)
        {
            if (null == accept) accept = ReturnsTrue<T>;
            for (int i = index + 1; i < that.Count; i++)
            {
                var item = that[i];
                if (accept(item))
                    return item;
            }
            return default(T);
        }
        public static T Previous<T>(this IList<T> that, int index, Func<T, bool> accept=null)
        {
            if (null == accept) accept = ReturnsTrue<T>;
            for (int i = index - 1; 0 <= i; i--)
            {
                var item = that[i];
                if (accept(item))
                    return item;
            }
            return default(T);
        }
    }
}
