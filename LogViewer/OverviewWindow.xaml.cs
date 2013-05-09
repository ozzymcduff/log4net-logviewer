using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using LogViewer.Model;
using System.Windows.Interop;
using System.Drawing;


namespace LogViewer
{
    public partial class OverviewWindow : Window
    {
        private FileLogEntryController filec;
        
        public OverviewWindow()
        {
            filec = new FileLogEntryController();
            
            Title = string.Format("LogViewer  v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
            this.Loaded += new RoutedEventHandler(OverviewWindow_Loaded);
            this.Closed += OverviewWindow_Closed;

            this.Icon = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle, Int32Rect.Empty, null);

            InitializeComponent();
            this.DataContext = filec;
            filec.FileNameChanged += ObservableFileName_PropertyChanged;
        }

        void ObservableFileName_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ((App)App.Current).AddFilenameToRecent(filec.FileName);
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Left:
                    filec.SelectPreviousEntry(entry => true);
                    break;
                case Key.Down:
                case Key.Right:
                    filec.SelectNextEntry(entry => true);
                    break;
                case Key.PageDown:
                    //GetNextPage
                    break;
                case Key.PageUp:
                    //GetPreviousPage
                    break;
                case Key.Home:
                    filec.SelectTop();
                    break;
                case Key.End:
                    filec.SelectBottom();
                    break;
                default:
                    break;
            }
        }

    }
}
