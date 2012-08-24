using System;
using System.IO;
using System.Linq;
using Core;
using LogViewer;

namespace LogTail
{
    class Program
    {
        static void Main(string[] args)
        {
            var entries = new LogEntryParser().Parse(FileUtil.ReadAllText(args[0]));
            var tailnum = 10;
            if (args.Length >= 2 && !string.IsNullOrEmpty(args[1])) 
            {
                tailnum = Int32.Parse(args[1]);
            }
            foreach (var logEntry in entries.Skip(entries.Count()-tailnum))
            {
                Console.WriteLine(logEntry.Message);
            }
        }
    }
}
