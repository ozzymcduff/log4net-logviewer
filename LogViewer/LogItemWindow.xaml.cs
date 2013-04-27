﻿using System;
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
    /// <summary>
    /// Interaction logic for LogItemWindow.xaml
    /// </summary>
    public partial class LogItemWindow : Window
    {
        public LogItemWindow()
        {
            InitializeComponent();
        }

        public Logs.LogEntryViewModel Selected 
        { 
            get { return this.logitemviewer.Selected; }
            set { this.logitemviewer.Selected = value; } 
        }
    }
}
