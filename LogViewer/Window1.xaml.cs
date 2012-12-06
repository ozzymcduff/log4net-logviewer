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


namespace LogViewer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private FileLogEntryController filec;
        private LogEntryCounter lcount;
        private RecentFileList recentFileList;
        //private App app { get { return ((App)App.Current); } }
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
        public Observable<string> ObservableFileName
        {
            get { return filec.ObservableFileName; }
        }

        public ObservableCollection<LogEntry> Entries
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
        public Window1()
        {
            filec = new FileLogEntryController();
            lcount = new LogEntryCounter(filec.Entries);
            recentFileList = new RecentFileList(new XmlPersister(ApplicationAttributes.Get()));
            InitializeComponent();
            menu1.DataContext = this;
            listView1.ItemsSource = Entries;
            countpanel.DataContext = Count;
            textboxFileName.DataContext = filec.ObservableFileName;
            listView1.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ListView1_HeaderClicked));

            recentFileList.MenuClick += (s, e) => OpenFile(e.Filepath);
            Title = string.Format("LogViewer  v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
            
            this.Loaded+=new RoutedEventHandler(Window1_Loaded);
        }


        private void OpenFile(string fileName)
        {
            this.fileName = fileName;
        }

        private void Window1_Loaded(object sender, RoutedEventArgs args) 
        {
            if (((App)App.Current).Args.Any() && File.Exists(((App)App.Current).Args.First()))
            {
                fileName = ((App)App.Current).Args.First();
            }
        }

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LogEntry logentry = this.listView1.SelectedItem as LogEntry;
            if (null != logentry)
            {
                logitemviewer.Selected = logentry;
            }
        }

        private ListSortDirection _Direction = ListSortDirection.Descending;

        private void ListView1_HeaderClicked(object sender, RoutedEventArgs e)
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

        private void listView1_Drop(object sender, DragEventArgs e)
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

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private int CurrentIndex = 0;

        private void Find(int Direction)
        {
            if (textBoxFind.Text.Length > 0)
            {
                if (Direction == 0)
                {
                    for (int i = CurrentIndex + 1; i < listView1.Items.Count; i++)
                    {
                        LogEntry item = (LogEntry)listView1.Items[i];
                        if (item.Data.Message.Contains(textBoxFind.Text))
                        {
                            listView1.SelectedIndex = i;
                            listView1.ScrollIntoView(listView1.SelectedItem);
                            ListViewItem lvi = listView1.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                            lvi.BringIntoView();
                            lvi.Focus();
                            CurrentIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = CurrentIndex - 1; i > 0 && i < listView1.Items.Count; i--)
                    {
                        LogEntry item = (LogEntry)listView1.Items[i];
                        if (item.Data.Message.Contains(textBoxFind.Text))
                        {
                            listView1.SelectedIndex = i;
                            listView1.ScrollIntoView(listView1.SelectedItem);
                            ListViewItem lvi = listView1.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                            lvi.BringIntoView();
                            lvi.Focus();
                            CurrentIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void buttonFindNext_Click(object sender, RoutedEventArgs e)
        {
            Find(0);
        }

        private void buttonFindPrevious_Click(object sender, RoutedEventArgs e)
        {
            Find(1);
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
