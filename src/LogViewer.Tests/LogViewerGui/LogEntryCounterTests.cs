using LogViewer.Model;
using log4net.Core;
using Xunit;
using System.Collections.ObjectModel;
using LogVmT = LogViewer.Model.LogEntryViewModel;
namespace IntegrationTests.LogViewerGui
{
	public class LogEntryCounterTests : TestFixtureBase
    {
        [Fact]
        public void Can_count()
        {
            var col = new ObservableCollection<LogVmT>(new []{
                LogVm(Level.Error), LogVm(Level.Error),
                LogVm(Level.Info)
            });
            var counter = new LogEntryCounter(col);
            Assert.Equal(LCount(Kv("ERROR",2), Kv("INFO",1)), counter.Count.Value);
        }

        [Fact]
        public void Can_add()
        {
            var col = new ObservableCollection<LogVmT>(new[]{
                LogVm(Level.Error), LogVm(Level.Error),
                LogVm(Level.Info)
            });
            var counter = new LogEntryCounter(col);
            col.Add(LogVm(Level.Debug));
            Assert.Equal(LCount(Kv("ERROR", 2), Kv("INFO", 1), Kv("DEBUG",1)),counter.Count.Value);
        }

        [Fact]
        public void Can_clear()
        {
            var col = new ObservableCollection<LogVmT>(new[]{
                LogVm(Level.Error), LogVm(Level.Error),
                LogVm(Level.Info)
            });
            var counter = new LogEntryCounter(col);
            col.Clear();
            Assert.Equal(LCount(),counter.Count.Value);
        }
    }
}
