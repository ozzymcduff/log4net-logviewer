using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;

namespace LogViewer
{
    class FileLogEntryController : DispatcherObject
    {
        public FileLogEntryController()
        {
            Entries = new ObservableCollection<LogEntry>();
            ObservableFileName = new Observable<string>();
        }
        private FileSystemWatcher _watcher;
        private string fileName;
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
        public ObservableCollection<LogEntry> Entries { get; set; }
        private LogEntryParser parser = new LogEntryParser();

        private void ReadFile()
        {
            Entries.Clear();
            using (var file = FileUtil.OpenReadOnly(FileName))
            {
                var items = parser.Parse(file);
                foreach (var item in items)
                {
                    Entries.Add(item);
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
