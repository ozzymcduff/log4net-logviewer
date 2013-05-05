using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewer.Infrastructure
{
    public static class Extensions
    {
        public static T Next<T>(this IList<T> that, int index, Func<T, bool> accept)
        {
            for (int i = index + 1; i < that.Count; i++)
            {
                var item = that[i];
                if (accept(item))
                    return item;
            }
            return default(T);
        }
        public static T Previous<T>(this IList<T> that, int index, Func<T, bool> accept)
        {
            for (int i = index - 1; 0 < i; i--)
            {
                var item = that[i];
                if (accept(item))
                    return item;
            }
            return default(T);
        }
    }
}
