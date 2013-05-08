using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;
using log4net;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public IEnumerable<string> Args { get; private set; }
        internal void AddFilenameToRecent(string filename)
        {
            JumpList.AddToRecentCategory(new JumpPath { Path = filename });
        }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            Args = e.Args;
        }

        private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = false;
            _log.Error(e.Exception);
        }
    }
}
