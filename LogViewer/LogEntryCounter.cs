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


        private LogEntryLevelCount GetCount(IEnumerable<LogEntry> entries)
        {
            var counts = entries.GroupBy(e => e.Level).Select(g => new KeyValuePair<string, int>(g.Key, g.Count()));
            return new LogEntryLevelCount(counts);
        }
        private void Entries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var newItems = new List<LogEntry>();
                        foreach (LogEntry item in e.NewItems)
                        {
                            newItems.Add(item);
                        }

                        Count.Value += GetCount(newItems);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var removeItems = new List<LogEntry>();
                        foreach (LogEntry item in e.OldItems)
                        {
                            removeItems.Add(item);
                        }

                        Count.Value -= GetCount(removeItems);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        throw new NotImplementedException();
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        Count.Value = GetCount(Entries);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
