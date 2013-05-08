using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace LogViewer
{
    public partial class LogListView : UserControl
    {
        public FileLogEntryController Filec
        {
            get { return (FileLogEntryController)this.DataContext; }
        }
        public LogListView()
        {
            InitializeComponent();
            logitemsView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(LogitemsViewHeaderClicked));
        }

        private void LogitemsDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    var a = (Array)e.Data.GetData(DataFormats.FileDrop);
                    if (a != null)
                    {
                        Filec.FileName = a.GetValue(0).ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in Drag Drop: " + ex.Message);
                }
            }
        }

        private ListSortDirection _direction = ListSortDirection.Descending;

        private void LogitemsViewHeaderClicked(object sender, RoutedEventArgs e)
        {
            var header = e.OriginalSource as GridViewColumnHeader;
            var source = e.Source as ListView;

            var dataView = CollectionViewSource.GetDefaultView(source.ItemsSource);
            dataView.SortDescriptions.Clear();
            _direction = _direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            var description = new SortDescription(header.Content.ToString(), _direction);
            dataView.SortDescriptions.Add(description);
            dataView.Refresh();
        }
    }
}
