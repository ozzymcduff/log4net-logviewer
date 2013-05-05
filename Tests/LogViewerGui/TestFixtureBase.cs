using log4net.Core;
using LogViewer;
using LogViewer.Logs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace IntegrationTests.LogViewerGui
{
    public class TestFixtureBase
    {
        protected static KeyValuePair<string, int> Kv(string key, int count)
        {
            return new KeyValuePair<string, int>(key, count);
        }

        protected static LogEntryLevelCount LCount(params KeyValuePair<string, int>[] counts)
        {
            return new LogEntryLevelCount(counts);
        }
        protected ObservableCollection<T> ToObservableCollection<T>(params T[] models)
        {
            return new ObservableCollection<T>(models);
        }

        protected LogEntryViewModel LogVm(Level level) 
        {
            var m = new LogEntryViewModel(new LogEntry() 
            { 
                Data = new LoggingEventData 
                {
                    Level = level
                }
            });
            return m;
        }
    }
}
