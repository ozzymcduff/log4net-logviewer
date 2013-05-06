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
    public partial class LogListView : UserControl
    {
        public FileLogEntryController filec
        {
            get { return (FileLogEntryController)this.DataContext; }
        }
        public LogListView()
        {
            InitializeComponent();
            logitemsView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(logitemsView_HeaderClicked));
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
    }
}
