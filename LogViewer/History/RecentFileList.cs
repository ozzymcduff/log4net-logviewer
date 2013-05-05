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

        public int MaxNumberOfFiles { get; set; }

        public event EventHandler<MenuClickEventArgs> MenuClick;

        public RecentFileList(IPersist persister)
        {
            this.Persister = persister;

            MaxNumberOfFiles = 9;
            this.FileList = new ObservableCollection<RecentFile>();
            LoadRecent();
        }

        private void LoadRecent()
        {
            this.FileList.Clear();
            var index = 0;
            foreach (var file in Persister.RecentFiles(MaxNumberOfFiles).Select(r => new RecentFile(++index, r)).ToArray())
            {
                this.FileList.Add(file);
            }
        }

        public ObservableCollection<RecentFile> FileList { get; private set; }

        //void InsertMenuItems()
        //{
        //    if (_RecentFiles == null) return;
        //    if (_RecentFiles.Count == 0) return;

        //    int iMenuItem = FileMenu.Items.IndexOf(this);
        //    foreach (RecentFile r in _RecentFiles)
        //    {
        //        string header = GetMenuItemText(r.Number + 1, r.Filepath, r.DisplayPath);

        //        r.MenuItem = new MenuItem { Header = header };
        //        r.MenuItem.Click += MenuItem_Click;

        //        FileMenu.Items.Insert(iMenuItem++, r.MenuItem);
        //    }
        //}



     

        public class MenuClickEventArgs : EventArgs
        {
            public string Filepath { get; private set; }

            public MenuClickEventArgs(string filepath)
            {
                this.Filepath = filepath;
            }
        }

        void MenuItem_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            OnMenuClick(menuItem);
        }

        protected virtual void OnMenuClick(MenuItem menuItem)
        {
            //string filepath = GetFilepath(menuItem);

            //if (String.IsNullOrEmpty(filepath)) return;

            //EventHandler<MenuClickEventArgs> dMenuClick = MenuClick;
            //if (dMenuClick != null) dMenuClick(menuItem, new MenuClickEventArgs(filepath));
        }

        //string GetFilepath(MenuItem menuItem)
        //{
        //    foreach (RecentFile r in _RecentFiles)
        //        if (r.MenuItem == menuItem)
        //            return r.Filepath;

        //    return String.Empty;
        //}

        internal void AddFilenameToRecent(string filepath)
        {
            Persister.InsertFile(filepath, MaxNumberOfFiles);
            LoadRecent();
        }
    }
}
