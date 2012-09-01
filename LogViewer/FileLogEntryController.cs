using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using Timer = System.Timers.Timer;

namespace LogViewer
{
    class FileLogEntryController : DispatcherObject
    {
        public FileLogEntryController()
        {
            Entries = new ObservableCollection<LogEntry>();
            ObservableFileName = new Observable<string>();
            InitPoll();
        }
        private FileSystemWatcher _watcher;
        private FileWithPosition file=null;
        private int itemindex = 1;
        public Observable<string> ObservableFileName { get; private set; }
        public string FileName
        {
            get
            {
                return (file != null ? file.FileName : null);
            }
            set
            {
                ObservableFileName.Value = value;
                if (null == file || !file.FileNameMatch(value))
                {
                    itemindex = 1;
                    Dispatcher.BeginInvoke(DispatcherPriority.Background,
                      new ThreadStart(() =>
                      {
                          Entries.Clear();
                      }));
                    InitWatcher();
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Background,
                      new ThreadStart(() =>
                      {
                          ReadFile();
                      }));
            }
        }

        private void InitPoll()
        {
            filetimer = new System.Timers.Timer(10000);

            // Hook up the Elapsed event for the timer.
            filetimer.Elapsed += PollFile;
            filetimer.Interval = 2000;
            filetimer.Enabled = true;
        }

        private void PollFile(object sender, ElapsedEventArgs e)
        {
            if (null != file && file.FileHasBecomeLarger())
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new ThreadStart(() =>
                 {
                     ReadFile();
                 }));
            }
        }

        public ObservableCollection<LogEntry> Entries { get; set; }
        private LogEntryParser parser = new LogEntryParser();
        private Timer filetimer;
        private readonly Object _readfilelock = new Object();
        private void ReadFile()
        {
            lock (_readfilelock)
            {
                try
                {
                    foreach (var item in file.Read(parser))
                    {
                        item.Item = itemindex++;
                        Entries.Add(item);
                    }
                }
                catch (OutOfBoundsException)
                {
                    file.ResetPosition();
                    Entries.Clear();
                    itemindex = 1;
                    ReadFile();
                }
            }
        }

        private void InitWatcher()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
            if (null == file || !file.Exists())
            {
                return;
            }
            _watcher = new FileSystemWatcher { Path = System.IO.Path.GetDirectoryName(FileName) };
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            _watcher.Changed += FileHasChanged;
            _watcher.EnableRaisingEvents = true;
        }

        private void FileHasChanged(object sender, FileSystemEventArgs e)
        {
            if (file.FileNameInFolder(e.FullPath))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background,
                     new ThreadStart(() =>
                     {
                         ReadFile();
                     }));
            }
        }
    }
}
