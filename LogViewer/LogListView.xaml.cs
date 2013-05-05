using LogViewer.Logs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for LogListView.xaml
    /// </summary>
    public partial class LogListView : UserControl
    {

        public FileLogEntryController filec
        {
            get { return (FileLogEntryController)this.DataContext; }
        }
        private int CurrentIndex { get { return filec.CurrentIndex; } set { filec.CurrentIndex = value; } }
        public LogListView()
        {
            InitializeComponent();
            logitemsView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(logitemsView_HeaderClicked));
            logitemsView.SelectionChanged +=logitemsView_SelectionChanged;
            this.DataContextChanged += LogListView_DataContextChanged;
        }

        void LogListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (filec != null)
            {
                filec.ObservableSelected.PropertyChanged += ObservableSelected_PropertyChanged;
            }
        }

        private void ObservableSelected_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var send = (Observable<LogEntryViewModel>)sender;
            logitemsView.SelectedItem = send.Value;
        }

        private void logitemsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filec.CurrentIndex = logitemsView.SelectedIndex;
            filec.Selected = (LogEntryViewModel) logitemsView.SelectedItem;
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
                        filec.FileName = a.GetValue(0).ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in Drag Drop: " + ex.Message);
                }
            }
        }

        private ListSortDirection _Direction = ListSortDirection.Descending;

        private void logitemsView_HeaderClicked(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
            ListView source = e.Source as ListView;

            var dataView = CollectionViewSource.GetDefaultView(source.ItemsSource);
            dataView.SortDescriptions.Clear();
            _Direction = _Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            SortDescription description = new SortDescription(header.Content.ToString(), _Direction);
            dataView.SortDescriptions.Add(description);
            dataView.Refresh();
        }

        private void logitems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var logentry = this.logitemsView.SelectedItem as LogEntryViewModel;
            if (null != logentry)
            {
                filec.Selected = logentry;
            }
        }
    }
}
