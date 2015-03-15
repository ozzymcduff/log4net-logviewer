using System;
using System.Threading;

namespace LogViewer.Infrastructure
{
	public class Poller<TLogEntry> : LogFileWatcherBase<TLogEntry>
	{
		private Timer filetimer;
		private long duration;
		public Poller(IFileWithPosition file, long duration, ILogEntryParser<TLogEntry> parser, Invoker invoker=null)
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
			Read();
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
}
