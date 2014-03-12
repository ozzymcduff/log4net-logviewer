using System;
using System.Threading;
using System.IO;

namespace LogViewer.Infrastructure
{
    public interface ILogFileWatcher : IDisposable
    {
        event Action<LogEntry> LogEntry;
        event Action OutOfBounds;
        void Init();
        void Reset();
    }
    public interface IInvoker
    {
        void Invoke(Action run);
    }
    public class DirectInvoker : IInvoker
    {
        public void Invoke(Action run)
        {
            run();
        }
    }
    public abstract class LogFileWatcherBase : ILogFileWatcher
    {
        public LogFileWatcherBase(IFileWithPosition file, LogEntryParser parser = null, IInvoker invoker = null)
        {
            this.File = file;
            this.parser = parser ?? new LogEntryParser();
            this.invoker = invoker ?? new DirectInvoker();

        }
        protected LogEntryParser parser;
        protected IInvoker invoker;
        public IFileWithPosition File { get; private set; }
        
        public event Action<LogEntry> LogEntry;
        public event Action OutOfBounds;

        public abstract void Init();

        protected void InvokeLogEntry(LogEntry entry)
        {
            LogEntry(entry);
        }
        protected bool TryInvokeOutOfBounds() 
        {
            if (null == OutOfBounds) return false;
            OutOfBounds();
            return true;
        }

        public void Reset()
        {
            File.ResetPosition();
            Read();
        }
        public void Read()
        {
            invoker.Invoke(() =>
            {
                try
                {
                    foreach (var item in File.Read(stream => parser.Parse(stream)))
                    {
                        LogEntry(item);
                    }
                }
                catch (OutOfBoundsException)
                {
                    OutOfBounds();
                }
            });
        }

        public abstract void Dispose();
    }
    public class Poller : LogFileWatcherBase
    {
        private Timer filetimer;
        private long duration;
        public Poller(IFileWithPosition file, long duration, LogEntryParser parser = null, IInvoker invoker = null)
            : base(file, parser, invoker)
        {
            this.duration = duration;
        }

        public override void Init()
        {
            Read();
            filetimer = new Timer(PollFile, null, (long)0, duration);
        }
        private void PollFile(Object stateInfo)
        {
            if (File.FileHasBecomeLarger())
            {
                invoker.Invoke(() =>
                {
                    foreach (var item in File.Read(stream => parser.Parse(stream)))
                    {
                        InvokeLogEntry(item);
                    }
                });
            }
        }

        public override void Dispose()
        {
            if (filetimer != null)
            {
                filetimer.Dispose();
                filetimer = null;
            }
        }

    }

    public class Watcher : LogFileWatcherBase
    {
        private FileSystemWatcher _watcher;

        public Watcher(IFileWithPosition file, LogEntryParser parser = null, IInvoker invoker = null)
            : base(file, parser, invoker)
        {
        }

        public override void Init()
        {
            Read();
            _watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(File.FileName),
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };
            _watcher.Changed += FileHasChanged;
            
        }

        private void FileHasChanged(object sender, FileSystemEventArgs e)
        {
            if (File.FileNameInFolder(e.FullPath))//NOTE: Is this really nec.?
            {
                invoker.Invoke(() =>
                {
                    try
                    {
                        foreach (var item in File.Read(stream => { return parser.Parse(stream); }))
                        {
                            InvokeLogEntry(item);
                        }
                    }
                    catch (OutOfBoundsException)
                    {
                        if (!TryInvokeOutOfBounds())
                        {
                            throw;
                        }
                    }
                });
            }
        }

        public override void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
        }

    }
}
