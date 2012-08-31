using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using LogViewer;
using NDesk.Options;

namespace LogTail
{
    class Program
    {
        class Watchers
        {
            private readonly IEnumerable<string> _files;
            private List<FileSystemWatcher> watchers;

            public Watchers(IEnumerable<string> files)
            {
                _files = files;
            }

            public void Init()
            {
                watchers = new List<FileSystemWatcher>();
                foreach (var path in _files.Select(Path.GetDirectoryName).Distinct())
                {
                    var watcher = new FileSystemWatcher
                                       {
                                           Path = path,
                                           NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                       };
                    watcher.Changed += FileHasChanged;
                    watchers.Add(watcher);
                }
            }
            private void FileHasChanged(object sender, FileSystemEventArgs e)
            {
                foreach (var fileName in _files)
                {
                    if (Path.GetFullPath(e.FullPath).Equals(Path.GetFullPath(fileName),
                                          StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            var files = new List<string>();
            bool watch = false, monitor = false;
            int lines = 10;
            var p = new OptionSet() {
                { "f|file=",   v => { files.Add (v); } },
                { "w|watch=", v=> { watch=true;}},
                { "m|monitor=", v=> { monitor=true;}},
                { "l|lines=", v=> { lines=Int32.Parse(v);}}
            };

            if (args.Length > 0 && !(args[0].StartsWith("-") || args[0].StartsWith("/")))
            {
                files.Add(args[0]);
            }

            p.Parse(args);
            if (watch)
            {
                InitWatch(files);
            }
            else if (monitor)
            {
                InitMonitor(files);
            }
            else
            {
                TailFiles(lines, files);
            }

        }

        private static void InitMonitor(List<string> files)
        {
            throw new NotImplementedException();
        }

        private static void TailFiles(int lines, List<string> files)
        {
            foreach (var fileName in files)
            {
                using (var file = FileUtil.OpenReadOnly(fileName))
                {
                    var items = new LogEntryParser().Parse(file)
                        .ToArray();
                    foreach (var logEntry in items.Skip(items.Count() - lines))
                    {
                        Console.WriteLine(logEntry.Message);
                    }
                }
            }
        }

        private static void InitWatch(List<string> files)
        {
            var w = new Watchers(files);
            w.Init();
        }

        
    }
}
