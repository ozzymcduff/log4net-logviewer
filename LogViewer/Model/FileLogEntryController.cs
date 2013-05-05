using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using Timer = System.Timers.Timer;
using LogViewer.Logs;
using LogViewer.Infrastructure;

namespace LogViewer
{
    public class FileLogEntryController 
    {
        class WrappedDispatcher : DispatcherObject, IInvoker, IWrapDispatcher
        {
            public WrappedDispatcher()
            {
            }

            public void Invoke(Action threadStart)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background,
                       threadStart);
            }
            public void BeginInvoke(DispatcherPriority dispatcherPriority, ThreadStart threadStart) 
            {
                Dispatcher.BeginInvoke(dispatcherPriority,
                       threadStart);
            }
        }
        Func<string, LogEntryParser, IWrapDispatcher, ILogFileReader> watcherFactory;

        public FileLogEntryController(IWrapDispatcher dispatcher = null, Func<string, LogEntryParser, IWrapDispatcher, ILogFileReader> watcherFactory = null)
        {
            Entries = new ObservableCollection<LogEntryViewModel>();
            ObservableFileName = new Observable<string>();
            ObservableSelected = new Observable<LogEntryViewModel>();
            ObservableFileName.PropertyChanged += ObservableFileName_PropertyChanged;
            this.watcherFactory = watcherFactory??CreateWatcher;
            wrappedDispatcher = dispatcher ?? new WrappedDispatcher();
            ObservableCurrentIndex = new Observable<int>();
        }

        private static ILogFileReader CreateWatcher(string value, LogEntryParser parser, IWrapDispatcher wrappedDispatcher)
        {
            return new Watcher(new FileWithPosition(value), parser, wrappedDispatcher);
        }

        void ObservableFileName_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var value = ((Observable<string>)sender).Value;
            if (null == watcher || !watcher.File.FileNameMatch(value))
            {
                if (watcher != null)
                {
                    watcher.Dispose();
                    watcher = null;
                }
                watcher = watcherFactory(value, parser, wrappedDispatcher);
                watcher.logentry = AddToEntries;
                watcher.outOfBounds = OutOfBounds;
                wrappedDispatcher.BeginInvoke(DispatcherPriority.Background,
                  new ThreadStart(() =>
                  {
                      Entries.Clear();
                  }));
                watcher.Init();
            }
        }
        private IWrapDispatcher wrappedDispatcher;

        private ILogFileReader watcher = null;
        public Observable<string> ObservableFileName { get; private set; }
        public string FileName
        {
            get
            {
                return ObservableFileName.Value;
            }
            set
            {
                ObservableFileName.Value = value;
            }
        }
        private void AddToEntries(LogEntry entry)
        {
            Entries.Add(new LogEntryViewModel(entry));
        }
        public ObservableCollection<LogEntryViewModel> Entries { get; set; }
        private LogEntryParser parser = new LogEntryParser();
        private void OutOfBounds() 
        {
            watcher.Reset();
            Entries.Clear();
            watcher.Read();
        }
        public Observable<LogEntryViewModel> ObservableSelected { get; set; }
        public LogEntryViewModel Selected
        {
            get { return ObservableSelected.Value; }
            set { ObservableSelected.Value = value; }
        }
        public Observable<int> ObservableCurrentIndex { get; set; }
        public int CurrentIndex { get { return ObservableCurrentIndex.Value; } set { ObservableCurrentIndex.Value = value; } }

        public void SelectNextEntry(Func<LogEntryViewModel, bool> accept) 
        {
            var nextitem = this.Entries.Next(CurrentIndex,accept);
            if (nextitem != null) 
            {
                Selected = nextitem;
            }
        }
        public void SelectPreviousEntry(Func<LogEntryViewModel, bool> accept)
        {
            var previous = this.Entries.Previous(CurrentIndex, accept);
            if (previous != null)
            {
                Selected = previous;
            }
        }
    }
    public interface IWrapDispatcher : IInvoker
    {
        void BeginInvoke(DispatcherPriority dispatcherPriority, ThreadStart threadStart);
    }
}
