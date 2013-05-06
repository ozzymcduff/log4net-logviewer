using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using LogViewer.Logs;

namespace LogViewer
{
    public class LogEntryCounter
    {
        public LogEntryCounter(ObservableCollection<LogEntryViewModel> entries)
        {
            this.Entries = entries;
            Count = new Observable<LogEntryLevelCount>();
            Count.Value = GetCount(Entries.ToArray());
            Entries.CollectionChanged += Entries_CollectionChanged;
        }

        private readonly ObservableCollection<LogEntryViewModel> Entries;
        public Observable<LogEntryLevelCount> Count { get; private set; }


        private LogEntryLevelCount GetCount(params LogEntryViewModel[] entries)
        {
            var counts = entries.GroupBy(e => e.Level)
                .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()))
                .ToArray();
            return new LogEntryLevelCount(counts);
        }
        private void Entries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var newItems = new List<LogEntryViewModel>();
                        foreach (LogEntryViewModel item in e.NewItems)
                        {
                            newItems.Add(item);
                        }

                        Count.Value += GetCount(newItems.ToArray());
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var removeItems = new List<LogEntryViewModel>();
                        foreach (LogEntryViewModel item in e.OldItems)
                        {
                            removeItems.Add(item);
                        }

                        Count.Value -= GetCount(removeItems.ToArray());
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                        throw new NotImplementedException();
                case NotifyCollectionChangedAction.Reset:
                    {
                        Count.Value = GetCount();
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
