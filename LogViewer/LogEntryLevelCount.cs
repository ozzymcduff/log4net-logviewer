using System;

namespace LogViewer
{
    public class LogEntryLevelCount
    {
        public int Debug { get; set; }
        public int Error { get; set; }
        public int Info { get; set; }
        public int Warn { get; set; }
        public int Total { get; set; }
    }
}
