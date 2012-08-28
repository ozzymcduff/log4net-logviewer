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
        private string fileName;
        private long lastposition = 0;
        private int itemindex = 1;
        public Observable<string> ObservableFileName { get; private set; }
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                var oldf = fileName;
                var newf = value;
                fileName = value;
                ObservableFileName.Value = fileName;
                if (null == oldf && newf != null
                    || !Path.GetFullPath(newf).Equals(Path.GetFullPath(oldf), StringComparison.InvariantCultureIgnoreCase))
                {
                    lastposition = 0;
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
            filetimer = new Timer(10000);

            // Hook up the Elapsed event for the timer.
            filetimer.Elapsed += PollFile;
            filetimer.Interval = 2000;
            filetimer.Enabled = true;
        }

        private void PollFile(object sender, ElapsedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                using (var file = FileUtil.OpenReadOnly(FileName))
                {
                    if (file.Length > lastposition)
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

        public ObservableCollection<LogEntry> Entries { get; set; }
        private LogEntryParser parser = new LogEntryParser();
        private Timer filetimer;

        private void ReadFile()
        {
            try
            {
                using (var file = FileUtil.OpenReadOnly(FileName, lastposition))
                {
                    foreach (var item in parser.Parse(file))
                    {
                        item.Item = itemindex++;
                        Entries.Add(item);
                    }
                    lastposition = file.Position;
                }
            }
            catch (OutOfBoundsException)
            {
                lastposition = 0;
                Entries.Clear();
                itemindex = 1;
                ReadFile();
            }

        }

        private void InitWatcher()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
            if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(FileName))
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
            if (e.FullPath.Equals(System.IO.Path.GetFullPath(FileName), StringComparison.InvariantCultureIgnoreCase))
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
