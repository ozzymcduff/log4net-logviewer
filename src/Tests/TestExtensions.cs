using LogViewer;
using System;

namespace IntegrationTests
{
    public static class TestExtensions
    {
        public static string Class(this LogEntry that)
        {
            return that.Data.LocationInfo.ClassName;
        }

        public static string Method(this LogEntry that)
        {
            return that.Data.LocationInfo.MethodName;
        }

        public static string File(this LogEntry that)
        {
            return that.Data.LocationInfo.FileName;
        }

        public static string Line(this LogEntry that)
        {
            return that.Data.LocationInfo.LineNumber;
        }
		public static T Tap<T>(this T that, Action<T> tapaction)
		{
			tapaction(that);
			return that;
		}
	}

}
