using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogViewer;
#if NDESK_OPTIONS
using NDesk.Options;
#else
using Mono.Options;
#endif
using System.Threading;
using log4net.Core;
using log4net.Layout;
using LogViewer.Infrastructure;
using System.Text.RegularExpressions;

namespace LogTail
{

    class Program
    {
        static void Main(string[] args)
        {
            var files = new List<string>();
            int monitor = 0;
            int? lines = null;
            var watch = false;
            LayoutSkeleton layout = null;
            var help = false;
            var p = new OptionSet() {
                { "i|file=",   v => { files.Add(v); } },
                { "m|monitor=", v => { monitor=ParseInterval(v);}},
                { "w|watch", v => { watch = true;}},
                { "l|lines=", v => { lines=Int32.Parse(v);}},
                { "h|?|help", v => { help = true;}},
                { "y|layout=",v=> { layout=new PatternLayout(v);}},
                { "f|format=", v=> { layout = GetFormatLayout(v); }}
            };
            var detectedFiles = args
                .Where(a => !(a.StartsWith("-") || a.StartsWith("/")))
                .Where(a => File.Exists(a));
            files.AddRange(detectedFiles);

            p.Parse(args);
            if (layout == null)
            {
                layout = new SimpleLayout();
            }
            Action<TextWriter,LogEntry> showentry = (writer, l) => layout.Format(writer, new LoggingEvent(l.Data));
            
            if (help)
            {
                Console.WriteLine(@"Usage:
-i|file={a filename}
    The (input) file to watch, monitor or 

-l|lines={tail x lines}	
    Display the last x lines. Defaults to 10 lines. 

-y|layout={pattern layout syntax as defined in log4net.Layout.PatternLayout}

-f|format={a named layout format}
    The available formats are:
        -f=minusminus Information delimited by newline and ----------------------

-h|?|help
    Display help

-w|watch
    Use file system watcher to watch file

-m|monitor=seconds
    Use polling to check for changes of the file.

For instance to :
LogTail.exe -f=logfile.xml
LogTail.exe -file=logfile.xml

If you are in powershell (or cygwin) you can do the following:
cat yourlogfile.xml | LogTail.exe
");
                return;
            }

            if (watch)
            {
				var w = new Watcher<LogEntry>(new FileWithPosition(files.Single()), new LogEntryParser());
				w.LogEntry += (entry) => showentry(Console.Out, entry);

				Do(w);
                return;
            }
            if (monitor > 0)
            {
				var w = new Poller<LogEntry>(new FileWithPosition(files.Single()), monitor, new LogEntryParser());
				w.LogEntry += (entry) => showentry(Console.Out, entry);
				Do(w);
                return;
            }

            if (files.Any())
            {
                TailFiles(lines ?? 10, files, (entry) => showentry(Console.Out, entry));
                return;
            }
            else
            {
                Console.WriteLine("No files, listening to standard input.");
                using (Stream stdin = Console.OpenStandardInput())
                using (Stream stdout = Console.OpenStandardOutput())
                using (StreamWriter writer = new StreamWriter(stdout))
                {
                    var items = new LogEntryParser().Parse(stdin).ToArray();
                    foreach (var logEntry in items.Skip(items.Count() - (lines ?? 10)))
                    {
                        showentry(writer, logEntry);
                    }
                }
                return;
            }
        }

        private static int ParseInterval(string v)
        {
            return Int32.Parse(v) * 1000;
        }

        private static void Do(ILogFileWatcher<LogEntry> w)
        {
            bool keepAlive = true;
            Thread workerThread = new Thread(w.Init);
            Console.CancelKeyPress += (o, e) => { w.Dispose(); keepAlive = false; };
            workerThread.Start();
            while (keepAlive) ;

            workerThread.Join();
        }

        private static void TailFiles(int lines, List<string> files, Action<LogEntry> showentry)
        {
            foreach (var fileName in files)
            {
                using (var file = FileUtil.OpenReadOnly(fileName))
                {
                    var items = new LogEntryParser().Parse(file)
                        .ToArray();
                    foreach (var logEntry in items.Skip(items.Count() - lines))
                    {
                        showentry(logEntry);
                    }
                }
            }
        }

        private static LayoutSkeleton GetFormatLayout(string format)
        {
            switch (format)
            {
                case "minusminus":
                    return
                        new PatternLayout(
                            "%newline----------------------BEGIN LOG----------------------%newline##date: %newline%date %newline##level: %newline%-5level %newline##logger: %newline%logger %newline########message:########%newline%message %newline########exception:########%newline %exception%newline----------------------END LOG----------------------%newline");
                default:
                    Console.Error.WriteLine("Unknown format '{0}'", format);
                    return null;
            }
        }

    }
}
