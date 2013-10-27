using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Xml;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Linq;
namespace LogViewer
{
    public class RecentFileList
    {
        private readonly IPersist Persister;

        public RecentFileList(IPersist persister)
        {
            this.Persister = persister;

            this.FileList = new ObservableCollection<RecentFile>();
            LoadRecent();
        }

        private void LoadRecent()
        {
            this.FileList.Clear();
            var index = 0;
            foreach (var file in Persister.RecentFiles().Select(r => new RecentFile(++index, r)).ToArray())
            {
                this.FileList.Add(file);
            }
        }

        public ObservableCollection<RecentFile> FileList { get; private set; }

        internal void AddFilenameToRecent(string filepath)
        {
            Persister.InsertFile(filepath);
            LoadRecent();
        }
    }
}
