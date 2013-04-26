using LogViewer.Logs;
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

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for LogItemViewer.xaml
    /// </summary>
    public partial class LogItemViewer : UserControl
    {
        public LogEntryViewModel Selected
        {
            get { return (LogEntryViewModel)this.DataContext; }
            set { this.DataContext = value; }
        }
        public LogItemViewer()
        {
            InitializeComponent();
        }
        //    this.textBoxTimeStamp.Text = string.Format("{0} {1}", logentry.TimeStamp.ToShortDateString(), logentry.TimeStamp.ToShortTimeString());

    }
}
