using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewer.Logs
{
    public class LogEntryViewModel
    {
        private readonly LogEntry _logentry;
        public LogEntryViewModel(LogEntry logentry)
        {
            _logentry = logentry;
        }

        public LogViewer.LogEntry.ImageType Image { get { return _logentry.Image; } set { } }

        public string Level
        {
            get { return _logentry.Data.Level.DisplayName; }
            set { }
        }
        public string MachineName
        {
            get { return _logentry.MachineName; }
            set { }
        }
        public string Thread
        {
            get { return _logentry.Data.ThreadName; }
            set { }
        }
        public DateTime TimeStamp
        {
            get { return _logentry.Data.TimeStamp; }
            set { }
        }
        public int Item
        {
            get { return _logentry.Item; }
            set { }
        }
        public string HostName
        {
            get { return _logentry.HostName; }
            set { }
        }
        public string UserName
        {
            get { return _logentry.Data.UserName; }
            set { }
        }
        public string App
        {
            get { return _logentry.Data.LoggerName; }
            set { }
        }
        public string Class
        {
            get { return LocationInfo().ClassName; }
            set { }
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
            set { }
        }
        public string Line
        {
            get { return LocationInfo().LineNumber; }
            set { }
        }

        public string File
        {
            get { return LocationInfo().FileName; }
            set { }
        }
        public string Message { get { return _logentry.Data.Message; } set { } }
        public string Throwable { get { return _logentry.Data.ExceptionString; } set { } }
    }

}
