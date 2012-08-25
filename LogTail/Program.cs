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
            using (var file = FileUtil.OpenReadOnly(FileUtil.ReadAllText(args[0])))
            {
                var items = new LogEntryParser().Parse(file)
                    .ToArray();
                var tailnum = 10;
                if (args.Length >= 2 && !string.IsNullOrEmpty(args[1]))
                {
                    tailnum = Int32.Parse(args[1]);
                }
                foreach (var logEntry in items.Skip(items.Count() - tailnum))
                {
                    Console.WriteLine(logEntry.Message);
                }
            }
        }
    }
}
