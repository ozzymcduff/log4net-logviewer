using System;

namespace LogViewer
{
	public class LogEntry
	{
		public enum ImageType
		{
			Debug = 0,
			Error = 1,
			Fatal = 2,
			Info = 3,
			Warn = 4,
			Custom = 5
		}


		public int Item { get; set; }

		public DateTime TimeStamp { get; set; }

		public ImageType Image { get; set; }

		public string Level { get; set; }

		public string Thread { get; set; }

		public string Message { get; set; }

		public string MachineName { get; set; }

		public string UserName { get; set; }

		public string HostName { get; set; }

		public string App { get; set; }

		public string Throwable { get; set; }

		public string Class { get; set; }

		public string Method { get; set; }

		public string File { get; set; }

		public string Line { get; set; }

		public string Logger{ get; set; }

		public LogEntry ()
		{
			Line = string.Empty;
			File = string.Empty;
			Method = string.Empty;
			Class = string.Empty;
			Throwable = string.Empty;
			App = string.Empty;
			HostName = string.Empty;
			UserName = string.Empty;
			MachineName = string.Empty;
			Message = string.Empty;
			Thread = string.Empty;
			Level = string.Empty;
			Image = ImageType.Custom;
			TimeStamp = new DateTime (1970, 1, 1, 0, 0, 0, 0);
			Item = 0;
		}

	}
}
