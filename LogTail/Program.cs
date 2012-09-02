using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogViewer;
using NDesk.Options;
using System.Threading;

namespace LogTail
{

    class Program
    {
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
                Do(new Watcher(new FileWithPosition(files.Single())) { logentry = l => Console.WriteLine(l.Message) });
            }
            else if (monitor > 0)
            {
                Do(new Poller(new FileWithPosition(files.Single()), monitor) { logentry = l => Console.WriteLine(l.Message) });
            }
            else
            {
                TailFiles(lines, files);
            }
        }

        private static void Do(ILogFileReader w) //int duration)
        {
            bool keepAlive = true;
            Thread workerThread = new Thread(w.Init);
            Console.CancelKeyPress += (o, e) => { w.Dispose(); keepAlive = false; };
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
