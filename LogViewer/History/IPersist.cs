using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewer
{
    public interface IPersist
    {
        List<string> RecentFiles();
        void InsertFile(string filepath);
        void RemoveFile(string filepath);
    }
}
