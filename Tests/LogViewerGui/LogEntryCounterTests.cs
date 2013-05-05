using log4net.Core;
using LogViewer;
using LogViewer.Logs;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LogVmT = LogViewer.Logs.LogEntryViewModel;
namespace IntegrationTests.LogViewerGui
{
    [TestFixture]
    public class LogEntryCounterTests : TestFixtureBase
    {
        [Test]
        public void Can_count()
        {
            var col = new ObservableCollection<LogVmT>(new []{
                LogVm(Level.Error), LogVm(Level.Error),
                LogVm(Level.Info)
            });
            var counter = new LogEntryCounter(col);
            Assert.That(counter.Count.Value, 
                Is.EqualTo(LCount(Kv("ERROR",2), Kv("INFO",1))));
        }
    }
}
