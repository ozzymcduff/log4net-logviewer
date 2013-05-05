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
using System.Windows.Shapes;

namespace LogViewer
{
    public partial class LogItemWindow : Window
    {
        public LogItemWindow()
        {
            this.Visibility = Visibility.Hidden;

            InitializeComponent();
            this.Closing +=LogItemWindow_Closing;
        }
        private FileLogEntryController _controller;
        public FileLogEntryController Controller { 
            get { return _controller; }
            set
            {
                if (_controller != null) throw new Exception();
                _controller = value;
                _controller.ObservableSelected.PropertyChanged += ObservableSelected_PropertyChanged;
            } 
        }

        void ObservableSelected_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var send = (Observable<Logs.LogEntryViewModel>)sender;
            this.logitemviewer.Selected = send.Value;
            if (send.Value != null)
            {
                this.Visibility = Visibility.Visible;
                this.Activate();
            }
            else
            {
                this.Visibility = Visibility.Hidden;
            }

        }
        private void LogItemWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Left:
                    Controller.SelectPreviousEntry(entry => true);
                    break;
                case Key.Down:
                case Key.Right:
                    Controller.SelectNextEntry(entry => true);
                    break;
                case Key.PageDown:
                    //GetNextPage
                    break;
                case Key.PageUp:
                    //GetPreviousPage
                    break;
                case Key.Home:
                    //GetTop
                    break;
                case Key.End:
                    //GetBottom
                    break;
                default:
                    break;
            }
        }
    }
}
