using LogViewer.Infrastructure;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace LogViewer.Model
{
    public class FileLogEntryController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        class WrappedDispatcher : DispatcherObject, IInvoker
        {
            public WrappedDispatcher()
            {
            }

            public void Invoke(Action threadStart)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background,
                       threadStart);
            }
        }
        Func<string, LogEntryParser, ILogFileWatcher> watcherFactory;

        public FileLogEntryController(IInvoker dispatcher = null, Func<string, LogEntryParser, ILogFileWatcher> createLogFileWatcher = null, IPersist persist = null)
        {
            Entries = new ObservableCollection<LogEntryViewModel>();
            this.watcherFactory = createLogFileWatcher??CreateLogFileWatcher;
            wrappedDispatcher = dispatcher ?? new WrappedDispatcher();
            Counter = new LogEntryCounter(Entries);
            this.recentFileList = new RecentFileList(persist?? new XmlPersister(ApplicationAttributes.Get(),9));
        }

        private ILogFileWatcher CreateLogFileWatcher(string value, LogEntryParser parser)
        {
            return new Watcher(new FileWithPosition(value), parser, wrappedDispatcher);
        }

        private void WatchFile(string value)
        {
            if (watcher != null)
            {
                watcher.Dispose();
                watcher = null;
            }
            watcher = watcherFactory(value, parser);
            watcher.LogEntry += AddToEntries;
            watcher.OutOfBounds += OutOfBounds;
            wrappedDispatcher.Invoke(() =>Entries.Clear());
            watcher.Init();
        }
        
        public ObservableCollection<RecentFile> FileList 
        { 
            get
            {
                return this.recentFileList.FileList;
            }
        }
        private IInvoker wrappedDispatcher;

        private ILogFileWatcher watcher = null;
        private string _filename;
        public string FileName
        {
            get
            {
                return _filename;
            }
            set
            {
                if (value != _filename)
                {
                    _filename = value;
                    WatchFile(value);
                    recentFileList.AddFilenameToRecent(value);

                    NotifyFileNameChanged();
                }
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
            Entries.Clear();
            watcher.Reset();
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

        public void SelectTop()
        {
            var first = this.Entries.FirstOrDefault();
            if (first!=null)
            {
                Selected = first;
            }
        }
        public void SelectBottom()
        {
            var last = this.Entries.LastOrDefault();
            if (last != null)
            {
                Selected = last;
            }
        }
    }
}
