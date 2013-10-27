using LogViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }

}
