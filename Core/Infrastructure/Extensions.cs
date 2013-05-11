using System;
using System.Collections.Generic;

namespace LogViewer.Infrastructure
{
    public static class Extensions
    {
        private static bool True<T>(T element) { return true; }
        public static T Next<T>(this IList<T> that, int index, Func<T, bool> accept = null)
        {
            if (null == accept) accept = True<T>;
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
            if (null == accept) accept = True<T>;
            for (int i = index - 1; 0 <= i; i--)
            {
                var item = that[i];
                if (accept(item))
                    return item;
            }
            return default(T);
        }
        public static T Tap<T>(this T that, Action<T> tapaction) 
        {
            tapaction(that);
            return that;
        }
    }
}
