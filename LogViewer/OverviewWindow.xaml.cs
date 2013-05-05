using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Interop;
using System.Reflection;
using System.Threading;
using System.Collections.ObjectModel;
using LogViewer.Logs;


namespace LogViewer
{
    public partial class OverviewWindow : Window
    {
        private FileLogEntryController filec;
        private LogEntryCounter lcount;
        private RecentFileList recentFileList;
        private LogItemWindow _currentLogItemWindow;
        
        public OverviewWindow()
        {
            filec = new FileLogEntryController();
            lcount = new LogEntryCounter(filec.Entries);
            recentFileList = new RecentFileList(new XmlPersister(ApplicationAttributes.Get()));
            
            Title = string.Format("LogViewer  v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
            _currentLogItemWindow = new LogItemWindow();
            _currentLogItemWindow.InitializeComponent();
            _currentLogItemWindow.Controller = filec;
            this.Loaded += new RoutedEventHandler(OverviewWindow_Loaded);
            this.Closed += OverviewWindow_Closed;

            InitializeComponent();

            menu1.DataContext = recentFileList;
            loglistview.DataContext = filec;
            countpanel.DataContext = lcount.Count;
            textboxFileName.DataContext = filec.ObservableFileName;

            filec.ObservableFileName.PropertyChanged += ObservableFileName_PropertyChanged;
        }

        void ObservableFileName_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var send = (Observable<string>)sender;
            ((App)App.Current).AddFilenameToRecent(send.Value);
            recentFileList.AddFilenameToRecent(send.Value);
        }

        void OverviewWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenFile(string fileName)
        {
            this.filec.FileName= fileName;
        }

        private void OverviewWindow_Loaded(object sender, RoutedEventArgs args) 
        {
            if (((App)App.Current).Args.Any() && File.Exists(((App)App.Current).Args.First()))
            {
                this.filec.FileName = ((App)App.Current).Args.First();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (null != this._currentLogItemWindow)
                this._currentLogItemWindow.Close();
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog oOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (oOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.filec.FileName = oOpenFileDialog.FileName;
            }
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void textBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (textBoxFind.Text.Length > 0)
                {
                    filec.SelectNextEntry(entry => entry.Message.Contains(textBoxFind.Text));
                }
            }
        }

        private void RecentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var context = ((System.Windows.Controls.MenuItem)sender).DataContext as RecentFile;
            if (null != context)
            {
                this.filec.FileName = context.Filepath;
            }
        }
    }
}
