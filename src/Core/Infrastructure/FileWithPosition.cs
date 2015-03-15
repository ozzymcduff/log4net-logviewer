using System;
using System.IO;

namespace LogViewer.Infrastructure
{
	public class FileWithPosition : IFileWithPosition
	{
		public string FileName { get; private set; }
		private long position = 0;
		public FileWithPosition(string filename)
		{
			FileName = filename;
		}

		public T Read<T>(Func<Stream, T> action)
		{
			using (var filestream = FileUtil.OpenReadOnly(FileName, position))
			{
				var val = action(filestream);
				position = filestream.Position;
				return val;
			}
		}

		public bool FileNameInFolder(string folder)
		{
			var fullpath = Path.GetFullPath(Path.GetDirectoryName(folder));
			var filepath = Path.GetFullPath(Path.GetDirectoryName(FileName));
			return fullpath.Equals(filepath,
				StringComparison.InvariantCultureIgnoreCase);
		}

		public void ResetPosition()
		{
			position = 0;
		}
	}
}
