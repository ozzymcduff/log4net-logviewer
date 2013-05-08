using LogViewer.Infrastructure;
using System;
using System.Linq;

namespace LogViewer.Model
{
    public class LogEntryViewModel
    {
        public static ImageType ParseImageType(string level)
        {
            switch (level)
            {
                case "ERROR":
                    return ImageType.Error;
                case "INFO":
                    return ImageType.Info;
                case "DEBUG":
                    return ImageType.Debug;
                case "WARN":
                    return ImageType.Warn;
                case "FATAL":
                    return ImageType.Fatal;
                default:
                    return ImageType.Custom;
            }
        }
	
        private readonly LogEntry _logentry;
        public LogEntryViewModel(LogEntry logentry)
        {
            _logentry = logentry;
        }

        public ImageType Image { get { return ParseImageType (_logentry.Data.Level.Name); } }

        public string Level
        {
            get { return _logentry.Data.Level.DisplayName; }
        }
        public string MachineName
        {
            get { return _logentry.MachineName; }
        }
        public string Thread
        {
            get { return _logentry.Data.ThreadName; }
        }
        public DateTime TimeStamp
        {
            get { return _logentry.Data.TimeStamp; }
        }
        public string HostName
        {
            get { return _logentry.HostName; }
        }
        public string UserName
        {
            get { return _logentry.Data.UserName; }
        }
        public string App
        {
            get { return _logentry.Data.LoggerName; }
        }
        public string Class
        {
            get { return LocationInfo().ClassName; }
        }

        private log4net.Core.LocationInfo LocationInfo()
        {
            if (_logentry.Data.LocationInfo!=null)
            return _logentry.Data.LocationInfo;
            return new log4net.Core.LocationInfo(string.Empty,string.Empty,string.Empty,string.Empty);
        }
        public string Method
        {
            get { return LocationInfo().MethodName; }
        }
        public string Line
        {
            get { return LocationInfo().LineNumber; }
        }

        public string File
        {
            get 
            {
                const int MaxPathLength = 30;
                return FileUtil.ShortenPathname(LocationInfo().FileName, MaxPathLength);
            }
        }
        public string FirstPartOfMessage { get { return FirstPartOf(_logentry.Data.Message); } set { } }

        private string FirstPartOf(string p)
        {
            if (string.IsNullOrEmpty(p))
                return string.Empty;
            var firstline = p.Split(new[] { '\n', '\r' },StringSplitOptions.RemoveEmptyEntries)
                .First();
            return firstline;
        }
        public string Message { get { return _logentry.Data.Message; } set { } }
        public string Throwable { get { return _logentry.Data.ExceptionString; } set { } }
    }
    public enum ImageType
    {
        Debug = 0,
        Error = 1,
        Fatal = 2,
        Info = 3,
        Warn = 4,
        Custom = 5
    }

}
