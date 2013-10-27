using System;
using log4net.Core;

namespace LogViewer
{
    public class LogEntry
    {
        public LoggingEventData Data;
        public string Identity
        {
            get
            {
                var n = "log4net:Identity";
                if (Data.Properties != null && Data.Properties.Contains(n))
                    return Data.Properties[n].ToString();
                return null;
            }
        }

        public string MachineName
        {
            get
            {
                var n = "log4jmachinename";
                if (Data.Properties != null && Data.Properties.Contains(n))
                    return Data.Properties[n].ToString();
                return null;
            }
        }

        public string HostName
        {
            get
            {
                var n = "log4net:HostName";
                if (Data.Properties != null && Data.Properties.Contains(n))
                    return Data.Properties[n].ToString();
                return null;
            }
        }

        public LogEntry()
        {
            this.Data = new LoggingEventData();
            Data.ExceptionString = string.Empty;
            Data.Domain = string.Empty;
            Data.UserName = string.Empty;
            Data.Message = string.Empty;
            Data.ThreadName = string.Empty;

            Data.TimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        }
    }
}
