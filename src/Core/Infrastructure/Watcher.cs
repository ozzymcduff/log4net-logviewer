using System.IO;

namespace LogViewer.Infrastructure
{
	public class Watcher <TLogEntry> : LogFileWatcherBase<TLogEntry>
	{
		private FileSystemWatcher _watcher;

		public Watcher(IFileWithPosition file, ILogEntryParser<TLogEntry> parser, Invoker invoker=null)
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
				Read();
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
