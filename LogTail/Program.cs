using System;
using System.IO;
using System.Linq;
using Core;

namespace LogTail
{
    class FileUtil
    {
        public static string ReadAllText(string path)
        {
            using (var file = new FileStream(path,FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(file))
            {
                return reader.ReadToEnd();
            }
        }
    }
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
