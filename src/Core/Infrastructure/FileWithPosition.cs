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

		public bool FileHasBecomeLarger()
		{
			using (var f = FileUtil.OpenReadOnly(FileName))
			{
				return (f.Length > position);
			}
		}

		public bool FileNameMatch(string otherFileName)
		{
			return !string.IsNullOrEmpty(otherFileName) && Path.GetFullPath(otherFileName).Equals(Path.GetFullPath(this.FileName),
				StringComparison.InvariantCultureIgnoreCase);
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

		public bool Exists()
		{
			return File.Exists(FileName);
		}
	}
}
