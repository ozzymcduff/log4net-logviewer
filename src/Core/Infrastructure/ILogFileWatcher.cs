using System;
using System.Threading;
using System.IO;

namespace LogViewer.Infrastructure
{
    public interface ILogFileWatcher<TLogEntry> : IDisposable
    {
        event Action<TLogEntry> LogEntry;
        event Action OutOfBounds;
		event Action<Exception> ExceptionOccurred;
		void Init();
        void Reset();
    }
}
