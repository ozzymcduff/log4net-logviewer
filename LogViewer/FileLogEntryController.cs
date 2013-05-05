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

namespace LogViewer
{
    class FileLogEntryController : DispatcherObject
    {
        class WrappedDispatcher : IInvoker
        {
            private readonly Dispatcher Dispatcher;
            public WrappedDispatcher(Dispatcher dispatcher)
            {
                this.Dispatcher = dispatcher;
            }

            public void Invoke(Action threadStart)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background,
                       threadStart);
            }
        }
        public FileLogEntryController()
        {
            Entries = new ObservableCollection<LogEntryViewModel>();
            ObservableFileName = new Observable<string>();
            wrappedDispatcher = new WrappedDispatcher(this.Dispatcher);
        }
        private WrappedDispatcher wrappedDispatcher;

        private Watcher watcher = null;
        public Observable<string> ObservableFileName { get; private set; }
        public string FileName
        {
            get
            {
                return (watcher != null ? watcher.File.FileName : null);
            }
            set
            {
                ObservableFileName.Value = value;
                if (null == watcher || !watcher.File.FileNameMatch(value))
                {
                    if (watcher != null)
                    {
                        watcher.Dispose();
                        watcher = null;
                    }
                    watcher = new Watcher(new FileWithPosition(value), parser, wrappedDispatcher);
                    watcher.logentry = AddToEntries;
                    watcher.outOfBounds = OutOfBounds;
                    Dispatcher.BeginInvoke(DispatcherPriority.Background,
                      new ThreadStart(() =>
                      {
                          Entries.Clear();
                      }));
                    watcher.Init();
                }
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
    }
}
