using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewer
{
    public interface IPersist
    {
        List<string> RecentFiles(int max);
        void InsertFile(string filepath, int max);
        void RemoveFile(string filepath, int max);
    }
}
