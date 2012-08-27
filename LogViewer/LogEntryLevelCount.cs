using System;
using System.Collections.Generic;
using System.Linq;
namespace LogViewer
{
    public class LogEntryLevelCount
    {
        private IEnumerable<KeyValuePair<string, int>> counts;
        public LogEntryLevelCount(IEnumerable<KeyValuePair<string, int>> counts)
        {
            Error = GetOrZero(counts, "Error");
            Info = GetOrZero(counts, "Info");
            Warn = GetOrZero(counts, "Warn");
            Debug = GetOrZero(counts, "Debug");
            Total = counts.Select(c => c.Value).Sum();
            this.counts = counts;
        }

        public int Debug { get; private set; }
        public int Error { get; private set; }
        public int Info { get; private set; }
        public int Warn { get; private set; }
        public int Total { get; private set; }
        public static LogEntryLevelCount operator +(LogEntryLevelCount x, LogEntryLevelCount y)
        {
            var newcounts = new List<KeyValuePair<string, int>>();
            foreach (var key in GetKeys(x.counts, y.counts))
            {
                newcounts.Add(new KeyValuePair<string, int>(key,
                    GetOrZero(x.counts, key) + GetOrZero(y.counts, key)));
            }
            return new LogEntryLevelCount(newcounts);
        }
        public static LogEntryLevelCount operator -(LogEntryLevelCount x, LogEntryLevelCount y)
        {
            var newcounts = new List<KeyValuePair<string, int>>();
            foreach (var key in GetKeys(x.counts, y.counts))
            {
                newcounts.Add(new KeyValuePair<string, int>(key,
                    GetOrZero(x.counts, key) - GetOrZero(y.counts, key)));
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

    }
}
