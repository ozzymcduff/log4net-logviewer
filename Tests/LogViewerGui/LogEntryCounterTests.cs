using LogViewer.Model;
using log4net.Core;
using LogViewer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LogVmT = LogViewer.Model.LogEntryViewModel;
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

        [Test]
        public void Can_add()
        {
            var col = new ObservableCollection<LogVmT>(new[]{
                LogVm(Level.Error), LogVm(Level.Error),
                LogVm(Level.Info)
            });
            var counter = new LogEntryCounter(col);
            col.Add(LogVm(Level.Debug));
            Assert.That(counter.Count.Value,
                Is.EqualTo(LCount(Kv("ERROR", 2), Kv("INFO", 1), Kv("DEBUG",1))));
        }

        [Test]
        public void Can_clear()
        {
            var col = new ObservableCollection<LogVmT>(new[]{
                LogVm(Level.Error), LogVm(Level.Error),
                LogVm(Level.Info)
            });
            var counter = new LogEntryCounter(col);
            col.Clear();
            Assert.That(counter.Count.Value,
                Is.EqualTo(LCount()));
        }
    }
}
