using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace LogViewer.Infrastructure
{
    public interface ILogFileReader : IDisposable
    {
        Action<LogEntry> logentry { get; set; }
        Action outOfBounds { get; set; }
        void Init();
        void Read();
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
    public abstract class LogFileReaderBase : ILogFileReader
    {
        public LogFileReaderBase(FileWithPosition file, LogEntryParser parser = null, IInvoker invoker = null)
        {
            this.File = file;
            this.parser = parser ?? new LogEntryParser();
            this.invoker = invoker ?? new DirectInvoker();

        }
        protected LogEntryParser parser;
        protected IInvoker invoker;
        public FileWithPosition File { get; private set; }
        public Action<LogEntry> logentry { get; set; }
        public Action outOfBounds { get; set; }

        public abstract void Init();

        public void Reset()
        {
            this.File.ResetPosition();
        }
        public void Read()
        {
            invoker.Invoke(() =>
            {
                try
                {
                    foreach (var item in File.Read(parser))
                    {
                        logentry(item);
                    }
                }
                catch (OutOfBoundsException)
                {
                    outOfBounds();
                }
            });
        }

        public abstract void Dispose();
    }
    public class Poller : LogFileReaderBase
    {
        private Timer filetimer;
        private long duration;
        public Poller(FileWithPosition file, long duration, LogEntryParser parser = null, IInvoker invoker = null)
            :base(file,parser,invoker)
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
                    foreach (var item in File.Read(parser))
                    {
                        logentry(item);
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

    public class Watcher : LogFileReaderBase
    {
        private FileSystemWatcher _watcher;

        public Watcher(FileWithPosition file, LogEntryParser parser = null, IInvoker invoker = null)
            :base(file,parser,invoker)
        {
        }

        public override void Init()
        {
            Read();
            _watcher = new FileSystemWatcher { Path = System.IO.Path.GetDirectoryName(File.FileName) };
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            _watcher.Changed += FileHasChanged;
            _watcher.EnableRaisingEvents = true;
        }

        private void FileHasChanged(object sender, FileSystemEventArgs e)
        {
            if (File.FileNameInFolder(e.FullPath))//NOTE: Is this really nec.?
            {
                invoker.Invoke(() =>
                {
                    try
                    {
                        foreach (var item in File.Read(parser))
                        {
                            logentry(item);
                        }
                    }
                    catch (OutOfBoundsException)
                    {
                        if (null != outOfBounds)
                        {
                            outOfBounds();
                        }
                        else
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
