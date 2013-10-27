using System;
using System.Collections.Generic;
using System.Linq;

namespace LogViewer.Model
{
    public class LogEntryLevelCount
    {
        private readonly IEnumerable<KeyValuePair<string, int>> _counts;
        public LogEntryLevelCount(IEnumerable<KeyValuePair<string, int>> counts)
        {
            var pairs = counts.ToArray();
            Error = GetOrZero(pairs, "Error");
            Info = GetOrZero(pairs, "Info");
            Warn = GetOrZero(pairs, "Warn");
            Debug = GetOrZero(pairs, "Debug");
            Total = pairs.Select(c => c.Value).Sum();
            this._counts = pairs;
        }

        public int Debug { get; private set; }
        public int Error { get; private set; }
        public int Info { get; private set; }
        public int Warn { get; private set; }
        public int Total { get; private set; }
        public static LogEntryLevelCount operator +(LogEntryLevelCount x, LogEntryLevelCount y)
        {
            var newcounts = new List<KeyValuePair<string, int>>();
            foreach (var key in GetKeys(x._counts, y._counts))
            {
                newcounts.Add(new KeyValuePair<string, int>(key,
                    GetOrZero(x._counts, key) + GetOrZero(y._counts, key)));
            }
            return new LogEntryLevelCount(newcounts);
        }
        public static LogEntryLevelCount operator -(LogEntryLevelCount x, LogEntryLevelCount y)
        {
            var newcounts = new List<KeyValuePair<string, int>>();
            foreach (var key in GetKeys(x._counts, y._counts))
            {
                newcounts.Add(new KeyValuePair<string, int>(key,
                    GetOrZero(x._counts, key) - GetOrZero(y._counts, key)));
            }
            return new LogEntryLevelCount(newcounts);
        }
        private static int GetOrZero(IEnumerable<KeyValuePair<string, int>> c, string key)
        {
            return c.FirstOrDefault(p => p.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Value;
        }
        private static IEnumerable<string> GetKeys(IEnumerable<KeyValuePair<string, int>> x, IEnumerable<KeyValuePair<string, int>> y)
        {
            return y.Select(c => c.Key).Union(x.Select(c => c.Key));
        }
        public override bool Equals(object obj)
        {
            return this.Equals(obj as LogEntryLevelCount);
        }
        public bool Equals(LogEntryLevelCount obj) 
        {
            return _counts.OrderBy(c => c.Key)
                .SequenceEqual(obj._counts.OrderBy(c => c.Key));
        }
        public override int GetHashCode()
        {
            return _counts.GetHashCode();
        }
        public override string ToString()
        {
            return String.Join(",", _counts
                .Select(c => string.Format("{0}={1}", c.Key, c.Value)));
        }
    }
}
