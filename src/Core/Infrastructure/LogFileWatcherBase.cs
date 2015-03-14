using System;
using System.Threading;
using System.IO;

namespace LogViewer.Infrastructure
{
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
}
