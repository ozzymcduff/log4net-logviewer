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
}
