using System;

namespace LogViewer.Infrastructure
{
	public abstract class LogFileWatcherBase<TLogEntry> : ILogFileWatcher<TLogEntry>
	{
		public LogFileWatcherBase(IFileWithPosition file, ILogEntryParser<TLogEntry> parser, Invoker invoker)
		{
			this.File = file;
			this.parser = parser;
			this.invoker = invoker ?? DirectInvoker.Invoke;
		}

		protected ILogEntryParser<TLogEntry> parser;
		protected Invoker invoker;
		public IFileWithPosition File { get; private set; }

		public event Action<TLogEntry> LogEntry;
		public event Action OutOfBounds;
		public event Action<Exception> ExceptionOccurred;

		public abstract void Init();

		protected void InvokeLogEntry(TLogEntry entry)
		{
			LogEntry(entry);
		}
		protected bool TryInvokeOutOfBounds()
		{
			if (null == OutOfBounds) return false;
			OutOfBounds();
			return true;
		}

		protected bool TryInvokeExceptionOccurred(Exception ex)
		{
			if (null == ExceptionOccurred) return false;
			ExceptionOccurred(ex);
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
					if (!TryInvokeOutOfBounds())
					{
						throw;
					}
				}
				catch (Exception e)
				{
					if (!TryInvokeExceptionOccurred(e))
					{
						throw;
					}
				}
			});
		}

		public abstract void Dispose();
	}
}
