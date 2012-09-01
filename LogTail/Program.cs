using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using LogViewer;
using NDesk.Options;
using System.Threading;

namespace LogTail
{

    class Program
    {
        class Poller
        {
            private readonly FileWithPosition file;
            private LogEntryParser parser;
            private Timer filetimer;
            private long duration;
            public Poller(string file, long duration)
            {
                this.file = new FileWithPosition(file);
                this.duration = duration;
            }

            public void Init()
            {
                parser = new LogEntryParser();
                foreach (var item in file.Read(parser))
                {
                    Console.WriteLine(item.Message);
                }
                filetimer = new Timer(PollFile, null, (long)0, duration);
            }
            private void PollFile(Object stateInfo)
            {
                if (file.FileHasBecomeLarger())
                {
                    foreach (var item in file.Read(parser))
                    {
                        Console.WriteLine(item.Message);
                    }
                }
            }


            internal void Stop()
            {
                if (filetimer != null)
                {
                    filetimer.Dispose();
                    filetimer = null;
                }
            }
        }
        class Watcher
        {
            private readonly FileWithPosition file;
            private LogEntryParser parser;
            private FileSystemWatcher _watcher;
            public Watcher(string file)
            {
                this.file = new FileWithPosition(file);
            }

            public void Init()
            {
                parser = new LogEntryParser();
                foreach (var item in file.Read(parser))
                {
                    Console.WriteLine(item.Message);
                }
                _watcher = new FileSystemWatcher { Path = System.IO.Path.GetDirectoryName(file.FileName) };
                _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
                _watcher.Changed += FileHasChanged;
                _watcher.EnableRaisingEvents = true;
            }

            private void FileHasChanged(object sender, FileSystemEventArgs e)
            {
                if (file.FileNameInFolder(e.FullPath))
                {
                    foreach (var item in file.Read(parser))
                    {
                        Console.WriteLine(item.Message);
                    }
                }
            }


            internal void Stop()
            {
                if (_watcher != null)
                {
                    _watcher.Dispose();
                    _watcher = null;
                }
            }
        }
        static void Main(string[] args)
        {
            var files = new List<string>();
            int monitor = 0;
            int lines = 10;
            var watch = false;
            var p = new OptionSet() {
                { "f|file=",   v => { files.Add (v); } },
                { "m|monitor=", v=> { monitor=Int32.Parse(v);}},
                { "w|watch", v=> { watch = true;}},
                { "l|lines=", v=> { lines=Int32.Parse(v);}}
            };

            if (args.Length > 0 && !(args[0].StartsWith("-") || args[0].StartsWith("/")))
            {
                files.Add(args[0]);
            }

            p.Parse(args);
            if (watch)
            {
                DoWatch(files);
            }
            else if (monitor > 0)
            {
                DoMonitor(files, monitor);
            }
            else
            {
                TailFiles(lines, files);
            }

        }

        private static void DoMonitor(List<string> files, int duration)
        {
            var w = new Poller(files.Single(), duration);
            bool keepAlive = true;
            Thread workerThread = new Thread(w.Init);
            Console.CancelKeyPress += (o, e) => { w.Stop(); keepAlive = false; };
            workerThread.Start();
            while (keepAlive) ;

            workerThread.Join();
        }
        private static void DoWatch(List<string> files)
        {
            var w = new Watcher(files.Single());
            bool keepAlive = true;
            Thread workerThread = new Thread(w.Init);
            Console.CancelKeyPress += (o, e) => { w.Stop(); keepAlive = false; };
            workerThread.Start();
            while (keepAlive) ;

            workerThread.Join();
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
    }
}
