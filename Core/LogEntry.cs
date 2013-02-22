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

	    public ImageType Image { get; set; }

        public string MachineName 
        {
            get
            {
                var n = "log4jmachinename";
                if (Data.Properties!=null && Data.Properties.Contains(n))
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

        public string Class
        {
            get { return Data.LocationInfo.ClassName; }
        }

        public string Method
        {
            get { return Data.LocationInfo.MethodName; }
        }

        public string File
        {
            get { return Data.LocationInfo.FileName; }
        }

        public string Line
        {
            get { return Data.LocationInfo.LineNumber; }
        }

	    public LogEntry ()
		{
            this.Data = new LoggingEventData();
		    Data.ExceptionString = string.Empty;
		    Data.Domain = string.Empty;
		    Data.UserName = string.Empty;
		    Data.Message = string.Empty;
		    Data.ThreadName = string.Empty;

		    Image = ImageType.Custom;

		    Data.TimeStamp = new DateTime (1970, 1, 1, 0, 0, 0, 0);
		    Item = 0;
		}

	    public static Level GetLevel(string level)
	    {
            //Ugly
            switch (level.ToUpper())
            {
                case "ERROR":
                    return log4net.Core.Level.Error;
                case "INFO":
                    return log4net.Core.Level.Info;
                case "DEBUG":
                    return log4net.Core.Level.Debug;
                case "WARN":
                    return log4net.Core.Level.Warn;
                case "FATAL":
                    return log4net.Core.Level.Fatal;
                default:
                    throw new NotImplementedException(level);
            }
	    }
	}
}
