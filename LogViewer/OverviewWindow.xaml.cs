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
        private string fileName
        {
            get { return filec.FileName; }
            set
            {
                filec.FileName = value;
                ((App)App.Current).AddFilenameToRecent(value);
                recentFileList.AddFilenameToRecent(value);
            }
        }
        private LogItemWindow _currentLogItemWindow { get; set; }
        public LogEntryViewModel Selected
        {
            get
            {
                return _currentLogItemWindow.Selected;
            }
            set
            {
                if (value != null)
                {
                    _currentLogItemWindow.Selected = value;
                    _currentLogItemWindow.Visibility = Visibility.Visible;
                    _currentLogItemWindow.Activate();
                }
                else
                {
                    _currentLogItemWindow.Visibility = Visibility.Hidden;
                }
            }
        }

        public Observable<string> ObservableFileName
        {
            get { return filec.ObservableFileName; }
        }

        public ObservableCollection<LogEntryViewModel> Entries
        {
            get { return filec.Entries; }
        }
        public Observable<LogEntryLevelCount> Count
        {
            get { return lcount.Count; }
        }
        public ObservableCollection<RecentFile> RecentFiles
        {
            get { return recentFileList.FileList; }
        }
        public OverviewWindow()
        {
            filec = new FileLogEntryController();
            lcount = new LogEntryCounter(filec.Entries);
            recentFileList = new RecentFileList(new XmlPersister(ApplicationAttributes.Get()));
            InitializeComponent();
            menu1.DataContext = this;
            logitemsView.ItemsSource = Entries;
            countpanel.DataContext = Count;
            textboxFileName.DataContext = filec.ObservableFileName;
            logitemsView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(logitemsView_HeaderClicked));

            recentFileList.MenuClick += (s, e) => OpenFile(e.Filepath);
            Title = string.Format("LogViewer  v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
            _currentLogItemWindow = new LogItemWindow();
            _currentLogItemWindow.InitializeComponent();

            this.Loaded+=new RoutedEventHandler(OverviewWindow_Loaded);
            this.Closed += OverviewWindow_Closed;
        }

        void OverviewWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenFile(string fileName)
        {
            this.fileName = fileName;
        }

        private void OverviewWindow_Loaded(object sender, RoutedEventArgs args) 
        {
            if (((App)App.Current).Args.Any() && File.Exists(((App)App.Current).Args.First()))
            {
                fileName = ((App)App.Current).Args.First();
            }
        }

        private void logitems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LogEntryViewModel logentry = this.logitemsView.SelectedItem as LogEntryViewModel;
            if (null != logentry)
            {
                Selected = logentry;
            }
        }

        private ListSortDirection _Direction = ListSortDirection.Descending;

        private void logitemsView_HeaderClicked(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
            ListView source = e.Source as ListView;

            ICollectionView dataView = CollectionViewSource.GetDefaultView(source.ItemsSource);
            dataView.SortDescriptions.Clear();
            _Direction = _Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            SortDescription description = new SortDescription(header.Content.ToString(), _Direction);
            dataView.SortDescriptions.Add(description);
            dataView.Refresh();
        }

        private void logitems_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
                    if (a != null)
                    {
                        fileName = a.GetValue(0).ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in Drag Drop: " + ex.Message);
                }
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
                fileName = oOpenFileDialog.FileName;
            }
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private int CurrentIndex = 0;

        private void Find(int Direction)
        {
            if (textBoxFind.Text.Length > 0)
            {
                if (Direction == 0)
                {
                    for (int i = CurrentIndex + 1; i < logitemsView.Items.Count; i++)
                    {
                        LogEntryViewModel item = (LogEntryViewModel)logitemsView.Items[i];
                        if (item.Message.Contains(textBoxFind.Text))
                        {
                            logitemsView.SelectedIndex = i;
                            logitemsView.ScrollIntoView(logitemsView.SelectedItem);
                            ListViewItem lvi = logitemsView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                            lvi.BringIntoView();
                            lvi.Focus();
                            CurrentIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = CurrentIndex - 1; i > 0 && i < logitemsView.Items.Count; i--)
                    {
                        LogEntryViewModel item = (LogEntryViewModel)logitemsView.Items[i];
                        if (item.Message.Contains(textBoxFind.Text))
                        {
                            logitemsView.SelectedIndex = i;
                            logitemsView.ScrollIntoView(logitemsView.SelectedItem);
                            ListViewItem lvi = logitemsView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                            lvi.BringIntoView();
                            lvi.Focus();
                            CurrentIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void textBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (textBoxFind.Text.Length > 0)
                {
                    Find(0);
                }
            }
        }

        private void RecentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var context = ((System.Windows.Controls.MenuItem)sender).DataContext as RecentFile;
            if (null != context)
            {
                fileName = context.Filepath;
            }
        }
    }
}
