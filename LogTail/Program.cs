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
            PatternLayout layout=null;
			var help = false;
            var p = new OptionSet() {
                { "f|file=",   v => { files.Add(v); } },
				{ "m|monitor=", v => { monitor=Int32.Parse(v);}},
                { "w|watch", v => { watch = true;}},
                { "l|lines=", v => { lines=Int32.Parse(v);}},
				{ "h|?|help", v => { help = true;}},
                { "y|layout=",v=> { layout=new PatternLayout(v);}}
            };
			var detectedFiles = args
				.Where(a=>!(a.StartsWith("-") || a.StartsWith("/")))
			    .Where(a=> Uri.IsWellFormedUriString(a, UriKind.RelativeOrAbsolute))
				.Where(a=> File.Exists(a));
            files.AddRange(detectedFiles);

            p.Parse(args);
            Action<LogEntry> showentry;
            if (null != layout)
            {
                showentry = l => layout.Format(Console.Out, new LoggingEvent(l.GetData()));
            }
            else
            {
                showentry = l => Console.WriteLine(l.Message);
            }
			if (help)
			{
				Console.WriteLine(@"Usage:
-f|file={a filename}
	The file to watch, monitor or 

-l|lines={tail x lines}	
	Display the last x lines. Defaults to 10 lines. 

-y|layout={pattern layout syntax as defined in log4net.Layout.PatternLayout}

-h|?|help
	Display help

For instance to :
LogTail.exe logfile.xml
LogTail.exe -file=logfile.xml
");
				return;
			}
			
            if (watch)
            {
                Do(new Watcher(new FileWithPosition(files.Single())) 
				{ 
					logentry = showentry 
				});
				return;
            }
            if (monitor > 0)
            {
                Do(new Poller(new FileWithPosition(files.Single()), monitor) 
				{
					logentry = showentry 
				});
				return;
            }
            
            if (files.Any()){
                TailFiles(lines ?? 10, files, showentry);
				return;
			}
			else
			{
				using (Stream stdin = Console.OpenStandardInput())
				using (Stream stdout = Console.OpenStandardOutput())
				using (StreamWriter writer = new  StreamWriter(stdout))
				{
					var items = new LogEntryParser().Parse(stdin).ToArray();
					foreach (var logEntry in items.Skip(items.Count() - (lines??10)))
                    {
                        writer.WriteLine(logEntry.Message);
                    }
				}
				return;
			}
        }

        private static void Do(ILogFileReader w)
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
    }
}
