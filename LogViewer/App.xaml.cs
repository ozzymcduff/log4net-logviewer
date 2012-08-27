using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Shell;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IEnumerable<string> Args { get; private set; }

        internal void AddFilenameToRecent(string filename)
        {
            JumpList.AddToRecentCategory(new JumpPath() { Path = filename });
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            this.Args = e.Args != null ? e.Args : new string[0];
        }
    }
}
