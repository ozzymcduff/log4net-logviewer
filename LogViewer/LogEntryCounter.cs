using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LogViewer
{
    class LogEntryCounter
    {
        public LogEntryCounter(ObservableCollection<LogEntry> entries)
        {
            this.Entries = entries;
            Entries.CollectionChanged += Entries_CollectionChanged;
            Count = new Observable<LogEntryLevelCount>();
            Count.Value = GetCount(Entries);
        }

        private readonly ObservableCollection<LogEntry> Entries;
        public Observable<LogEntryLevelCount> Count { get; private set; }

        private int GetOrZero(IEnumerable<KeyValuePair<string, int>> c,string key)
        {
            return c.FirstOrDefault(p => p.Key == key).Value;
        }
        private LogEntryLevelCount GetCount(IEnumerable<LogEntry> entries) 
        {
            var counts = entries.GroupBy(e => e.Level).Select(g => new KeyValuePair<string, int>(g.Key, g.Count()));
            return new LogEntryLevelCount() 
            { 
                Error = GetOrZero(counts,"ERROR"),
                Info = GetOrZero(counts, "INFO"),
                Warn = GetOrZero(counts, "WARN"),
                Debug = GetOrZero(counts, "Debug"),
                Total = counts.Select(c=>c.Value).Sum()
            };
        }
        private void Entries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Count.Value = GetCount(Entries);
        }
       
    }
}
