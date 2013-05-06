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
    public class FileLogEntryController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

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

        public FileLogEntryController(IWrapDispatcher dispatcher = null, Func<string, LogEntryParser, IWrapDispatcher, ILogFileReader> watcherFactory = null, RecentFileList recentFileList=null)
        {
            Entries = new ObservableCollection<LogEntryViewModel>();
            FileNameChanged += ObservableFileName_PropertyChanged;
            this.watcherFactory = watcherFactory??CreateWatcher;
            wrappedDispatcher = dispatcher ?? new WrappedDispatcher();
            Counter = new LogEntryCounter(Entries);
            this.recentFileList = recentFileList??new RecentFileList(new XmlPersister(ApplicationAttributes.Get()));
        }

        private static ILogFileReader CreateWatcher(string value, LogEntryParser parser, IWrapDispatcher wrappedDispatcher)
        {
            return new Watcher(new FileWithPosition(value), parser, wrappedDispatcher);
        }

        void ObservableFileName_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var value = _filename;
            WatchFile(value);
            recentFileList.AddFilenameToRecent(value);
        }

        private void WatchFile(string value)
        {
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
        
        public ObservableCollection<RecentFile> FileList 
        { 
            get
            {
                return this.recentFileList.FileList;
            }
        }
        private IWrapDispatcher wrappedDispatcher;

        private ILogFileReader watcher = null;
        private string _filename;
        public string FileName
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
                NotifyFileNameChanged();
            }
        }
        public event PropertyChangedEventHandler FileNameChanged;
        public void NotifyFileNameChanged()
        {
            NotifyPropertyChanged("FileName");
            if (FileNameChanged != null)
                FileNameChanged(this, new PropertyChangedEventArgs("FileName"));
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
        private LogEntryViewModel _selected;
        public LogEntryViewModel Selected
        {
            get { return _selected; }
            set 
            { 
                _selected = value; 
                NotifyPropertyChanged("Selected"); 
            }
        }
        private int _currentIndex;
        private RecentFileList recentFileList;
        public int CurrentIndex 
        { 
            get { return _currentIndex; }
            set 
            {
                _currentIndex = value; 
                NotifyPropertyChanged("CurrentIndex"); 
            } 
        }

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
        public LogEntryCounter Counter { get; set; }


    }
    public interface IWrapDispatcher : IInvoker
    {
        void BeginInvoke(DispatcherPriority dispatcherPriority, ThreadStart threadStart);
    }
}
