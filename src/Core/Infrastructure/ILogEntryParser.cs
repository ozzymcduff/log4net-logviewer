using System.Collections.Generic;
using System.IO;

namespace LogViewer.Infrastructure
{
	public interface ILogEntryParser<TLogEntry>
	{
		IEnumerable<TLogEntry> Parse(Stream stream);
	}
}
