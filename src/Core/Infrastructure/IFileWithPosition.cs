using System;
using System.IO;

namespace LogViewer.Infrastructure
{
	public interface IFileWithPosition
	{
		T Read<T>(Func<Stream, T> action);
		void ResetPosition();

		string FileName { get; }

		bool FileNameInFolder(string filename);

		bool FileHasBecomeLarger();

		long Position();
    }
}
