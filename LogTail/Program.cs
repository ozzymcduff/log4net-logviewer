using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core;

namespace LogTail
{
    class Program
    {
        static void Main(string[] args)
        {
            var entries = new LogEntryParser().Parse(File.ReadAllText(args[0]));
            var tailnum = 10;
            if (!string.IsNullOrEmpty(args[1])) 
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
